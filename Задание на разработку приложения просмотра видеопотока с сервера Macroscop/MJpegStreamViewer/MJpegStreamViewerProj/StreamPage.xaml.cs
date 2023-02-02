using System;
using System.IO;
using System.Net;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Animation;
using System.Threading.Tasks;

namespace MJpegStreamViewerProj
{
    public sealed partial class StreamPage : Page
    {
        private readonly byte[] JpegSOI = new byte[] { 0xff, 0xd8 };    //первые байты Jpeg
        private readonly byte[] JpegEOI = new byte[] { 0xff, 0xd9 };    //последние байты Jpeg

        private int ChunkSize = 4096;
        private const int MAX_BUFFER_SIZE = 1024 * 1024 * 10;           //размер буфера 10MB
        private const string pauseText = "Пауза";
        private const string fpsRespErrorText = "Ошибка распознавания доступной частоты кадров";

        private bool streamIsActive;                                    //флаг активности воспроизведения        
        private BitmapImage image = new BitmapImage();                  //текущий кадр
        private List<int> defFpsList { get; } = new List<int>()         //список доступных частот кадров трансляций по умолчанию
        {
            30, 25, 15, 5, 60
        };
        public List<int> fpsList;                     //список доступных частот кадров трансляций

        public List<string> channelSList = new List<string>();          //список названий каналов
        public string serverUriPart = "";                               //первая часть URI адреса запроса канала
        public List<string> paramsUriSList = new List<string>();        //список из параметров к первой части URI адресов каналов
        private bool extOptIsOn;

        private event EventHandler<FrameReadyEventArgs> FrameReady;
        private event EventHandler<ErrorEventArgs> Error;

        public void ErrorEventTrigger(EventHandler<ErrorEventArgs> ErrorHandler, Exception ex)
        {
            ErrorHandler?.Invoke(this, new ErrorEventArgs
            {
                Message = ex.Message,
                ErrorCode = ex.GetHashCode(),
            });
        }

        public class FrameReadyEventArgs : EventArgs
        {
            public byte[] FrameBuffer;
        }

        public sealed class ErrorEventArgs : EventArgs
        {
            public string Message { get; set; }
            public int ErrorCode { get; set; }
        }

        public StreamPage()
        {
            this.InitializeComponent();
            this.FrameReady += ShowImage;
            this.Error += ShowError;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ServicePointManager.DefaultConnectionLimit = 1;
            PageDataObject pdo = (PageDataObject)e.Parameter;            //объект класса receivedDataObject для передачи данных о каналах между страницами
            channelSList = pdo.ChannelStringList;
            paramsUriSList = pdo.UriParamsStringList;
            serverUriPart = pdo.ServerUriPart;
            int chosenInd = pdo.ChosenIndex;
            extOptIsOn = pdo.ExtOptions;
            channelsCBox.SelectedIndex = chosenInd;

            ImageCenterText.Visibility = Visibility.Collapsed;
            streamIsActive = false;

            fpsList = new List<int>();

            try
            {
                int fps = Convert.ToInt32(paramsUriSList.ElementAt(chosenInd).Split("&fps=")[1]);
                fpsList.Clear();
                fpsList.Add(fps);
                foreach (int fpsitem in defFpsList)
                {
                    if (fps != fpsitem)
                        fpsList.Add(fpsitem);
                };
                fpsCBox.SelectedIndex = 0;
            }
            catch
            {
                StopStream(fpsRespErrorText);
            }

            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)               //возврат на главную страницу
        {
            StopStream("");
            //SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
            PageDataObject pdo = new PageDataObject();
            pdo.ExtOptions = extOptIsOn;
            Frame.Navigate(typeof(MainPage), pdo, new DrillInNavigationTransitionInfo());
        }

        private async void ShowImage(object sender, FrameReadyEventArgs e)
        {
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(e.FrameBuffer);
                    await writer.StoreAsync();
                }
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    image.SetSource(stream);
                    imgOfStream.Source = image;
                });
            }
        }

        private void ShowError(object sender, ErrorEventArgs e)
        {
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                StopStream(e.Message);                              //остановка воспроизведения и вывод сообщения о ошибке в центр окна
                PlayBtn.IsChecked = false;                          //принудительное переключение кнопки PlayBtn на случай ошибки
            });
        }

        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            if (streamIsActive)
            {
                StopStream(pauseText);
                return;
            };
            StartStream(serverUriPart + paramsUriSList.ElementAt(channelsCBox.SelectedIndex).Split("&fps=")[0] + "&fps=" + Convert.ToString(fpsCBox.SelectedItem));
            //StartStream(new Uri("http://demo.macroscop.com:8080/mobile?login=root&channelid=2016897c-8be5-4a80-b1a3-7f79a9ec729c&resolutionX=640&resolutionY=480&fps=25"));            
        }

        private void StartStream(string streamUriString)
        {
            streamIsActive = true;
            ImageCenterText.Text = "";
            ImageCenterText.Visibility = Visibility.Collapsed;
            shadeRectangle.Visibility = Visibility.Collapsed;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(streamUriString));
            request.BeginGetResponse(OnGetResponse, request);
        }

        private void StopStream(string stopReasonMsg)
        {
            streamIsActive = false;
            ImageCenterText.Text = stopReasonMsg;
            ImageCenterText.Visibility = Visibility.Visible;
            shadeRectangle.Visibility = Visibility.Visible;
            Thread.Sleep(500);
            GC.Collect();                                                                       //принудительная "уборка мусора"
        }

        private void OnGetResponse(IAsyncResult asyncResult)                                    //метод запускаемый асинхронно и принимающий Http-ответы
        {
            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            request.Timeout = 300;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult);
                Stream stream = response.GetResponseStream();
                ResponseMJpegStream(stream);
                stream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                ErrorEventTrigger(this.Error, ex);
            }
            request.Abort();
            GC.Collect();                                                                       //принудительная "уборка мусора"
        }

        public void ResponseMJpegStream(Stream stream)                                                 //метод распознающий поток MJPEG видеоданных
        {
            BinaryReader binaryReader = new BinaryReader(stream);
            int currentPosition = 0;
            byte[] buffer = new byte[ChunkSize];                                                   //буфер для считываемых кадров
            byte[] currentChunk;                                                                //чанк для считывания потока в буфер
            byte[] FrameBuffer;                                                                 //буфер для кадра
            int collectorCounter = 0;
            while (streamIsActive)
            {
                currentChunk = binaryReader.ReadBytes(ChunkSize);                               //считывание полученных байт в чанк
                if (buffer.Length < currentPosition + currentChunk.Length)
                {
                    if (buffer.Length < MAX_BUFFER_SIZE)
                    {
                        Array.Resize(ref buffer, currentPosition + currentChunk.Length);        //изменение размера буфера для добавления нового чанка
                    }
                    else                                                                        //сброс буфера если его размер больше 10MB
                    {
                        currentPosition = 0;
                        Array.Resize(ref buffer, ChunkSize);
                    }
                }
                Array.Copy(currentChunk, 0, buffer, currentPosition, currentChunk.Length);      //копирование текущего чанка в буфер начиная с текущей позиции
                currentPosition += currentChunk.Length;                                         //увеличение текущей позиции в буфере на размер чанка
                int soi = buffer.Find(JpegSOI, currentPosition, 0);                             //поиск позиции начальных байт JPEG в буфере при помощи перегруженного метода Find
                if (soi != -1)                                                                  //если нахождение прошло успешно
                {
                    int eoi = buffer.Find(JpegEOI, currentPosition, startAt: soi);              //нахождение позиции последних битов JPEG кадра в буфере
                    if (eoi != -1)                                                              //если нахождение прошло успешно
                    {
                        if (eoi > soi)                                                          //если начало и конец кадра расчитаны верно
                        {
                            int endOfCurrentImage = eoi + JpegEOI.Length;                       //расчет размера текущего кадра
                            FrameBuffer = new byte[endOfCurrentImage - soi];                    //пересоздание массива необходимого, для кадра, размера
                            Array.Copy(buffer, soi, FrameBuffer, 0, FrameBuffer.Length);        //копирование данных из буфера в только-что созданный массив
                            this.FrameReady?.Invoke(this, new FrameReadyEventArgs               //вызов метода запуска события обновления картинки imOfStream
                            {
                                FrameBuffer = FrameBuffer,
                            });
                            int remainingSize = currentPosition - endOfCurrentImage;            //расчет размера буфера для оставшихся кадров
                            Array.Copy(buffer, endOfCurrentImage, buffer, 0, remainingSize);    //копирование байтов буфера, начиная с текущей позиции, в начало буфера
                            currentPosition = remainingSize;                                    //обновление значения теущей позиции в измененном буфере
                            ChunkSize = Convert.ToInt32(FrameBuffer.Length * 0.5d);             //пересчет размера чанка чтобы избежать множественных считываний
                        }
                    }
                }
                if (collectorCounter >= 100)
                { GC.Collect(); collectorCounter = 0; }                                         //принудительная "уборка мусора"
                else
                    collectorCounter++;
            }
            buffer = new byte[0];
            binaryReader.Close();
            binaryReader.Dispose();
        }

        private void FpsCBoxSelectionChanged(object sender, SelectionChangedEventArgs e) { SelectionChanged(); }       //метод привязанный к событию изменения качества потока

        private void ChannelsCBoxSelectionChanged(object sender, SelectionChangedEventArgs e) { SelectionChanged(); }  //метод привязанный к событию изменения канала

        private void SelectionChanged()                                             //метод для смены канала или качества видеопотока
        {
            if (streamIsActive)
            {
                StopStream("");
                StartStream(serverUriPart + (paramsUriSList.ElementAt(channelsCBox.SelectedIndex)).Split("&fps=")[0] + "&fps=" + Convert.ToString(fpsCBox.SelectedItem));
            }
        }
    }

    internal static class Extensions
    {
        public static int Find(this byte[] buff, byte[] pattern, int limit = int.MaxValue, int startAt = 0)     //метод-расширение для поиска пограничных байт JPEG в буфере начиная с определенной позиции
        {
            int patter_match_counter = 0;
            int i = 0;
            for (i = startAt; i < buff.Length && patter_match_counter < pattern.Length && i < limit; i++)
            {
                if (buff[i] == pattern[patter_match_counter])   //если фрагмент буфера совпадает с пограничными байтами JPEG
                {
                    patter_match_counter++;
                }
                else
                {
                    patter_match_counter = 0;
                }
            }       //по достижении patter_match_counter размера длинны пограничных байт JPEG и изменении счетчика i ...

            if (patter_match_counter == pattern.Length)
            {
                return i - pattern.Length;                      //возвратить значение позиции буфера с совпадающими байтами
            }
            else
            {
                return -1;                                      //возвратить значение -1 при неудачном поиске
            }
        }
    }
}

