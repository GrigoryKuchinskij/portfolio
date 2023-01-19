using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Mvvm;
using PalindromeCheckClient.ViewModels;

namespace PalindromeCheckClient.Models
{
    public class MainWindowModel : BindableBase
    {
        private static readonly string _DefIP = "127.0.0.1";
        private static readonly string _DefPort = "8090";

        public string URI { set; get; }
        public string FolderPath { get; set; }
        public bool WaitingForCommand { get; set; } = true;

        private readonly Dispatcher _dispatcher;

        private ObservableCollection<FileDataItem> ObservableFileDataCollection = new ObservableCollection<FileDataItem>();
        public ReadOnlyObservableCollection<FileDataItem> PublicFileDataCollectionForDG;
        private ObservableCollection<PalindromeStatusItem> PalindromeStatusObservableCollection = new ObservableCollection<PalindromeStatusItem>();
        public ReadOnlyObservableCollection<PalindromeStatusItem> PalindromeStatusPublicCollection;

        public MainWindowModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            PublicFileDataCollectionForDG = new ReadOnlyObservableCollection<FileDataItem>(ObservableFileDataCollection);
            PalindromeStatusPublicCollection = new ReadOnlyObservableCollection<PalindromeStatusItem>(PalindromeStatusObservableCollection);
            URI = "http://"+ _DefIP + ":"+ _DefPort + "/";
        }

        public void CheckPalindrome()
        {
            WaitingForCommand = false;
            RaisePropertyChanged(nameof(MainWindowViewModel.WaitingForCommand));
            Thread thread = new Thread( StartSendFiles )
            { IsBackground = false };
            thread.Start();

            //GC.Collect();
        }

        int defWaitServ = 50, waitServ;
        bool limitReached = false;

        private void StartSendFiles()
        {
            int i = 0, availableTreads = -1, curTreads = 0; 
            waitServ = defWaitServ;
            _dispatcher.Invoke(new Action(() => {
                PalindromeStatusObservableCollection.Clear();
            }));
            foreach (FileDataItem item in ObservableFileDataCollection)
            {
                item.Procd = false;
            }
            while (true)                                                                //проход по всем элементам списка
            {
                if (limitReached && availableTreads == -1)                              //если сервер перегружался и число доступных потоков ещё не задано
                {
                    availableTreads = curTreads;                                        //присвоить счетчик текущих потоков => числу доступных потоков
                }
                else                                                                    //иначе, увеличить счетчик текущих потоков ...
                {
                    curTreads++;
                }

                if (i >= ObservableFileDataCollection.Count)                                   //если последний объект списка
                {
                    int unProcd = 0;
                    foreach (FileDataItem dataItem in ObservableFileDataCollection)
                    {
                        if (dataItem.Procd == false)
                        {
                            unProcd++;
                        }
                    }

                    if (unProcd > 0)                                                    //если есть необработанные строки
                    {
                        availableTreads = Math.Min(unProcd, availableTreads);
                        curTreads = 0;
                        i = 0;                                                          //начать с начала
                        continue;
                    }
                    else                                                                //и если все строки обработаны
                    {
                        break;                                                          //закончить
                    };
                };

                if (ObservableFileDataCollection[i].Procd)                                     //если текущая строка обработана
                {
                    i++;
                    continue;                                                           //пропустить
                };

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URI);
                request.Method = "POST";
                string postData = "IsPalindrome=&word=" + ObservableFileDataCollection[i].Text + "&index=" + i;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentType = "text/html";
                request.ContentLength = byteArray.Length;
                try
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }
                catch
                {
                    MessageBox.Show("Невозможно соединиться с удаленным сервером");     //System.Net.WebException: "Невозможно соединиться с удаленным сервером";
                    break;
                };

                var thread =
                    new Thread(new ThreadStart(() =>
                    {
                        ReceiveHtml(request, out string html);                          //отправка запроса к серверу и получение ответной строки
                        ParseHtml(html, out string answer, out int ind);                //распознавание из строки индекса и ответа
                        ProcessAnswerAndShow(answer, ind);                              //обработка ответа и вывод в строку datagrid
                    }))
                    { IsBackground = true };
                thread.Start();
                i++;
                if (availableTreads != -1)                                                     //если число доступных потоков задано
                    Thread.Sleep(
                        Math.Max(waitServ / availableTreads, defWaitServ)                      //задержка до обработки следущей строки исходя из числа доступных потоков (с учётом минимальной стандартной задержки)
                        );
                else Thread.Sleep(
                    Math.Max(waitServ / curTreads, defWaitServ)                         //иначе => задержка до обработки следущей строки исходя из числа текущих потоков (с учётом минимальной стандартной задержки)
                    );
            }
            waitServ = defWaitServ;
            limitReached = false;
            
            WaitingForCommand = true;
            RaisePropertyChanged(nameof(MainWindowViewModel.WaitingForCommand));
        }

        private void ProcessAnswerAndShow(string answer, int ind)
        {
            if (ind != -1)                                                  //если отсутствует ошибка
            {
                _dispatcher.Invoke(new Action(() =>
                {
                    while (PalindromeStatusObservableCollection.Count <= ind)
                        PalindromeStatusObservableCollection.Add(new PalindromeStatusItem { Value = "<Ожидание сервера>" });
                }));
                bool waitContain = answer.Contains("wait=");
                if (waitContain)                                            //если сервер перегружен
                {
                    if (!limitReached)                                      //если сервер еще не перегружался
                    {
                        if (Int32.TryParse(answer.Substring(5), out int time))
                        {
                            TranslateAnswer(ref answer);                    //перевод ответа для вывода в datagrid
                            waitServ = time * 1000;                         //задать значение задержки отправки запросов
                            limitReached = true;                            //флаг: сервер был перегружен
                        };
                    };
                }
                else
                {
                    if (ind <= PalindromeStatusObservableCollection.Count && ind <= ObservableFileDataCollection.Count && ObservableFileDataCollection[ind].Procd == false)
                    {                                                       //отбрасываются обработанные строки и строки вне диапазона 
                        _dispatcher.Invoke(new Action(() =>
                        {
                            TranslateAnswer(ref answer);                    //перевод ответа для вывода в datagrid

                            PalindromeStatusObservableCollection[ind] = new PalindromeStatusItem { Value = answer };
                            //вставка ответа в datagrid
                            ObservableFileDataCollection[ind].Procd = true;        //пометить строку как обработанную
                        }));
                    };
                };
                RaisePropertyChanged(nameof(MainWindowViewModel.PalindromeStatusDGItems));
            }
            //GC.Collect();
        }

        private delegate void delegateForInvoke();

        private static void ReceiveHtml(HttpWebRequest request, out string html)
        {
            html = "";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK) return;

                var stream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                    html = HttpUtility.UrlDecode(html, Encoding.UTF8);
                }
                stream.Close();
                response.Close();
            }
            catch (System.Net.WebException) { html = ""; return; }
            
            request.Abort();
        }

        private static void ParseHtml(string htmlText, out string answer, out int ind)
        {
            answer = ""; ind = -1;
            var htmlM = htmlText.Split('<');
            bool succ = false;

            foreach (string tag in htmlM)
            {
                if (tag.Trim().StartsWith("input"))
                {
                    if (tag.Contains(@"""answer"""))
                        answer = ExtractValue(tag);
                    //else if (tag.Contains(@"""word"""))
                    //    word = ExtractValue(tag);
                    else if (tag.Contains(@"""index"""))
                        succ = Int32.TryParse(ExtractValue(tag),out ind);
                }
            }
            if (!succ) ind = -1;
        }

        private static void TranslateAnswer(ref string answer)
        {
            switch (answer.ToLower())
            {
                case "true":
                    answer = "Палиндром";
                    break;
                case "false":
                    answer = "Не палиндром";
                    break;
                default:
                    if (!answer.Contains("wait="))  //если ответ не содержит true, false или wait то вернуть error
                        answer = "error";
                    break;
            }
        }

        private static string ExtractValue(string text)
        {
            int begInd = text.IndexOf(@"value=""") + 7;
            int endInd = text.IndexOf(@"""", begInd);
            return text.Substring(begInd, endInd - begInd);
        }

        public string ShowOpenDialogForFolderPath()
        {
            using (CommonOpenFileDialog dialogDirectory = new CommonOpenFileDialog { IsFolderPicker = true })
            {
                return FolderPath = dialogDirectory.ShowDialog() == CommonFileDialogResult.Ok ? dialogDirectory.FileName : null;
            }
        }

        public int FillDataGridWithFolderPath() => FillDataGridWithFolderPath(FolderPath);

        public int FillDataGridWithFolderPath(string folderPath)
        {
            ObservableFileDataCollection.Clear();
            PalindromeStatusObservableCollection.Clear();
            try
            {
                List<string> filesname = Directory.GetFiles(folderPath, "*.txt").ToList();
                foreach (var filePath in filesname)
                {
                    StreamReader stream = new StreamReader(filePath, Encoding.UTF8);
                    Guid guid = Guid.NewGuid();
                    string guidS = guid.ToString();
                    ObservableFileDataCollection.Add(new FileDataItem { Text = stream.ReadToEnd().Replace("\r", "").Replace("\n", ""), Procd = false });
                }
                RaisePropertyChanged(nameof(MainWindowViewModel.FolderPath));
                RaisePropertyChanged(nameof(MainWindowViewModel.FilesToCheckDGItems));
                RaisePropertyChanged(nameof(MainWindowViewModel.PalindromeStatusDGItems));
                return 0;
            }
            catch { return -1; }
        }
    }
}
