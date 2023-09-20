using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Prism.Commands;
using System.Net;
using MJpegStreamViewerWPF.ViewModels;
using System.Collections.ObjectModel;
using System.IO;
using Windows.UI.Core;
using System.Xml;
using static MJpegStreamViewerWPF.XmlResponse;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using System.Windows.Controls;
using System.Net.Http;

namespace MJpegStreamViewerWPF.Models
{
    public class MainPageModel : BindableBase
    {
        public readonly MJPEGStream MJPEGStream;

        public PageData PageDataObject { get; set; } = new PageData
        {
            SelectedIndex = 0,
            ChannelsList = new ObservableCollection<string>(),
            UriParamsList = new ObservableCollection<string>()
        };

        public MainPageModel()
        {
            MJPEGStream = new MJPEGStream();
            MJPEGStream.FrameReady += ShowImage;
            MJPEGStream.Error += ShowMessage;
        }

        private const string confErrorMsg = "Ошибка запроса конфигурации. ";
        private const string requestUriInpErrorMsg = "Ошибка ввода URL запроса доступных каналов.";
        private const string fpsRespErrorText = "Ошибка распознавания доступной частоты кадров";

        public async Task GetChannels(string UriForConfigRequest)
        {
            PageDataObject.ChannelsList = new ObservableCollection<string>();
            PageDataObject.UriParamsList = new ObservableCollection<string>();
            try
            {
                if (UriForConfigRequest.Trim() != "" && (UriForConfigRequest.Trim().IndexOf("http://") == 0 || UriForConfigRequest.Trim().IndexOf("https://") == 0))
                {
                    List<XmlResponse.XMLDataItem> responseList = new List<XmlResponse.XMLDataItem>();
                    await Task.Run(async () =>
                    {
                        HttpClient client = new HttpClient();
                        HttpResponseMessage response = await client.GetAsync(UriForConfigRequest);
                        response.EnsureSuccessStatusCode();
                        System.IO.Stream stream = response.Content.ReadAsStream();
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(stream);
                        stream.Close();
                        List<ChannelDataItem> ChannelIdList = new List<ChannelDataItem>();
                        List<QualityDataItem> QualityList = new List<QualityDataItem>();
                        responseList = XmlResponse.ReadXMLDataFromDocument(xmlDoc, ChannelIdList, QualityList);
                    });
                    XmlResponse.WriteXMLDataIntoObservableCollections(responseList, PageDataObject.UriParamsList, PageDataObject.ChannelsList);
                }
                else
                {
                    throw new UriFormatException(requestUriInpErrorMsg);
                }
                try
                {
                    int fps = Convert.ToInt32(PageDataObject.UriParamsList.ElementAt(PageDataObject.SelectedIndex).Split("&fps=")[1]);
                    FpsCollection.Add(fps);
                    foreach (int fpsitem in defaultFpsList)
                    {
                        if (fps != fpsitem)
                            FpsCollection.Add(fpsitem);
                    };
                    FpsCollSelectedIndex = 0;
                }
                catch
                {
                    throw new Exception(fpsRespErrorText);
                }
            }
            catch (UriFormatException ex) { throw ex; }
            catch
            {
                PageDataObject.ChannelsList.Clear();
                PageDataObject.UriParamsList.Clear();
                Exception Exception = new Exception(confErrorMsg);
                throw Exception;
            }

        }

        public string TextMessage { get; set; }

        private void ShowMessage(object sender, MJPEGStream.ErrorEventArgs e)
        {
            TextMessage = e.Message;
            MJPEGStream.Stop();         //остановка воспроизведения и вывод сообщения о ошибке в центр окна
        }

        public void StreamStart(string UriForStream)
        {
            if (!MJPEGStream.IsActive)
            {
                Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    if (!string.IsNullOrWhiteSpace(UriForStream))
                        MJPEGStream.Start(UriForStream);
                }));
            };
        }

        public void StreamStop()
        {
            if (MJPEGStream.IsActive)
            {
                Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    MJPEGStream.Stop();
                }));
            };
        }

        public async Task StreamRestart(string UriForStream)
        {
            StreamStop();
            await Task.Delay(400);
            StreamStart(UriForStream);
            return;
        }

        public bool StreamIsActive { get => MJPEGStream.IsActive; }
        public WriteableBitmap Image { get; set; }
        public ObservableCollection<int> FpsCollection { get; set; } = new ObservableCollection<int>();         //список доступных частот кадров трансляций
        public int FpsCollSelectedIndex { get; set; } = 0;

        private readonly List<int> defaultFpsList = new List<int>()         //список доступных частот кадров трансляций по умолчанию
        {
            30, 25, 15, 5, 60
        };

        private void ShowImage(object sender, MJPEGStream.FrameReadyEventArgs e)
        {
            BitmapImage bitmap;
            using (MemoryStream stream = new MemoryStream(e.FrameBuffer))
            {
                try
                {
                    //stream.WriteAsync(e.FrameBuffer);
                    //stream.Seek(0, SeekOrigin.Begin);
                    bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap?.EndInit();
                    bitmap?.Freeze();
                    Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        if (bitmap is not null)
                        {
                            Image = new WriteableBitmap(bitmap);
                            RaisePropertyChanged(nameof(MainPageVM.Image));
                        }
                    }));
                } catch { }
                stream.Close();
                stream.Dispose();
            }
        }
    }
}
