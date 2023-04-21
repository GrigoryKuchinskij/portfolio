using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Mvvm;
using PalindromeCheckClient.ViewModels;
using System.Xml;
using System.Reflection;

namespace PalindromeCheckClient.Models
{
    public class MainWindowModel : BindableBase
    {
        private static readonly string _DefIP = "127.0.0.1";
        private static readonly string _DefPort = "8090";

        public string URI { set; get; }
        public string FolderPath { get; set; }
        public bool IsBusy { get; set; } = false;

        private readonly Dispatcher dispatcher;

        private ObservableCollection<FileDataItem> _observableFileDataCollection = new ObservableCollection<FileDataItem>();
        public ReadOnlyObservableCollection<FileDataItem> PublicFileDataCollectionForDG;
        private ObservableCollection<PalindromeStatusItem> _palindromeStatusObservableCollection = new ObservableCollection<PalindromeStatusItem>();
        public ReadOnlyObservableCollection<PalindromeStatusItem> PalindromeStatusPublicCollection;

        public MainWindowModel()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            PublicFileDataCollectionForDG = new ReadOnlyObservableCollection<FileDataItem>(_observableFileDataCollection);
            PalindromeStatusPublicCollection = new ReadOnlyObservableCollection<PalindromeStatusItem>(_palindromeStatusObservableCollection);
            URI = "http://" + _DefIP + ":" + _DefPort + "/";
        }

        public void CheckPalindrome()
        {
            IsBusy = true;
            RaisePropertyChanged(nameof(MainWindowViewModel.IsIdle));
            Thread thread = new Thread(StartSendFiles)
            { IsBackground = false };
            thread.Start();
        }

        private readonly int defWaitServ = 50;
        private int waitServ;
        bool limitReached = false;

        private void StartSendFiles()
        {
            int index = 0, availableTreads = -1, OccupiedTreads = 0;
            waitServ = defWaitServ;
            dispatcher.Invoke(new Action(() => _palindromeStatusObservableCollection.Clear()));
            foreach (FileDataItem item in _observableFileDataCollection)
                item.IsProcessed = false;
            while (true)                                                                //проход по всем элементам списка
            {
                if (index >= _observableFileDataCollection.Count)                       //если индекс строки за границей коллекции
                {
                    int unProcd = 0;
                    foreach (FileDataItem dataItem in _observableFileDataCollection)
                        if (dataItem.IsProcessed == false)
                            unProcd++;                                                  //считать необработанные строки
                    if (unProcd == 0) break;                                            //и если все строки обработаны закончить
                    availableTreads = Math.Min(unProcd, availableTreads);               //если есть необработанные строки
                    OccupiedTreads = 0;
                    index = 0;                                                          //начать с начала
                    continue;
                };

                if (limitReached && availableTreads == -1)                              //если сервер перегружался и число доступных потоков ещё не задано
                    availableTreads = OccupiedTreads;                                   //присвоить счетчик занятых потоков => числу доступных потоков
                else                                                                    //иначе, увеличить счетчик занятых потоков ...
                    OccupiedTreads++;

                if (_observableFileDataCollection[index].IsProcessed)                   //если текущая строка обработана
                {
                    index++;
                    continue;                                                           //пропустить
                };

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URI);
                try
                {
                    SendRequest(request, _observableFileDataCollection[index].Text, index);
                }
                catch
                {
                    MessageBox.Show("Невозможно соединиться с удаленным сервером");
                    break;
                };

                var thread = new Thread(new ThreadStart(() =>
                {
                    ReceiveHtml(request, out string html);                              //отправка запроса к серверу и получение ответной строки
                    ParseHtml(html, out string recivedAnswer, out int recivedIndex);    //распознавание из строки индекса и ответа
                    ProcessAnswerAndShow(recivedAnswer, recivedIndex);                  //обработка ответа и вывод в строку datagrid
                }))
                { IsBackground = true };

                thread.Start();

                if (availableTreads != -1)                                              //если число доступных потоков задано
                    Thread.Sleep(Math.Max(waitServ / availableTreads, defWaitServ));    //задержка до обработки следущей строки исходя из числа доступных потоков (с учётом минимальной стандартной задержки)
                else
                    Thread.Sleep(Math.Max(waitServ / OccupiedTreads, defWaitServ));     //иначе => задержка до обработки следущей строки исходя из числа занятых потоков (с учётом минимальной стандартной задержки)
                index++;
            }
            waitServ = defWaitServ;
            limitReached = false;
            IsBusy = false;
            RaisePropertyChanged(nameof(MainWindowViewModel.IsIdle));
        }

        private static void SendRequest(HttpWebRequest webRequest, string word, int index)
        {
            webRequest.Method = "POST";
            string postData = "IsPalindrome=&word=" + word + "&index=" + index;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            webRequest.ContentType = "text/html";
            webRequest.ContentLength = byteArray.Length;

            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
        }

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
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(htmlText);
            var rootNode = xmlDocument.FirstChild;
            bool isParsed = false;
            foreach (XmlNode childNode in rootNode)
            {
                if (childNode.Name.ToLower() == "answer")
                    answer = childNode.InnerText;
                else if (childNode.Name.ToLower() == "index")
                    isParsed = Int32.TryParse(childNode.InnerText, out ind);
            }
            if (!isParsed) ind = -1;
        }

        private void ProcessAnswerAndShow(string answer, int ind)
        {
            if (ind == -1) return;                                              //если присутствует ошибка
            dispatcher.Invoke(new Action(() =>
            {
                while (_palindromeStatusObservableCollection.Count <= ind)
                    _palindromeStatusObservableCollection.Add(new PalindromeStatusItem { Value = "<Ожидание сервера>" });
            }));
            if (answer.Contains("wait="))                               //если сервер перегружен
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
                if (ind <= _palindromeStatusObservableCollection.Count && ind <= _observableFileDataCollection.Count && _observableFileDataCollection[ind].IsProcessed == false)
                {                                                       //отбрасываются обработанные строки и строки вне диапазона 
                    dispatcher.Invoke(new Action(() =>
                    {
                        TranslateAnswer(ref answer);                    //перевод ответа для вывода в datagrid

                        _palindromeStatusObservableCollection[ind] = new PalindromeStatusItem { Value = answer };
                        //вставка ответа в datagrid
                        _observableFileDataCollection[ind].IsProcessed = true;        //пометить строку как обработанную
                    }));
                };
            };
            RaisePropertyChanged(nameof(MainWindowViewModel.PalindromeStatusDGItems));
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

        public CommonFileDialogResult GetFolderPathDialog()
        {
            using (CommonOpenFileDialog dialogDirectory = new CommonOpenFileDialog { IsFolderPicker = true })
            {
                CommonFileDialogResult dialogResult = dialogDirectory.ShowDialog();
                FolderPath = dialogResult == CommonFileDialogResult.Ok ? dialogDirectory.FileName : null;
                return dialogResult;
            }
        }

        public int FillDataGridFromFolder() => FillDataGridFromFolder(FolderPath);

        public int FillDataGridFromFolder(string folderPath)
        {
            _observableFileDataCollection.Clear();
            _palindromeStatusObservableCollection.Clear();
            try
            {
                List<string> filesname = Directory.GetFiles(folderPath, "*.txt").ToList();
                foreach (var filePath in filesname)
                {
                    StreamReader stream = new StreamReader(filePath, Encoding.UTF8);
                    Guid guid = Guid.NewGuid();
                    string guidS = guid.ToString();
                    _observableFileDataCollection.Add(new FileDataItem { Text = stream.ReadToEnd().Replace("\r", "").Replace("\n", ""), IsProcessed = false });
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
