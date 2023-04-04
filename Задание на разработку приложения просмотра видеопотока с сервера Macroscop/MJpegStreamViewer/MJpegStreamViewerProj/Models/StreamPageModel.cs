using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MJpegStreamViewerProj.ViewModels;
using static MJpegStreamViewerProj.Models.StreamPageModel;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;

namespace MJpegStreamViewerProj.Models
{
    public class StreamPageModel : BindableBase
    {
        private readonly CoreDispatcher coreDispatcher;
        private readonly MJPEGStream MJPEGStream = new MJPEGStream();
        public StreamPageModel()
        {
            coreDispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            MJPEGStream.FrameReady += ShowImage;
            MJPEGStream.Error += ShowError;
        }

        public PageData PageDataObject { get; set; }
        private const string pauseText = "Пауза";
        //private const string restartText = "Переключение канала";
        private const string fpsRespErrorText = "Ошибка распознавания доступной частоты кадров";
        private string imageCenterText = pauseText;
        public string ImageCenterText 
        { 
            get => imageCenterText;
            set {
                imageCenterText = value;
                RaisePropertyChanged(nameof(StreamPageViewModel.ImageCenterText));
                RaisePropertyChanged(nameof(StreamPageViewModel.ImageCenterTextVisibility));
                RaisePropertyChanged(nameof(StreamPageViewModel.ShadeRectangleVisibility));
            }
        }
        public bool StreamIsActive { get => MJPEGStream.IsActive; }
        public BitmapImage Image { get; set; } = new BitmapImage();                  //текущий кадр
        public ObservableCollection<int> FpsCollection { get; set; } = new ObservableCollection<int>();         //список доступных частот кадров трансляций
        public int FpsCollSelectedIndex { get; set; } = 0;

        private readonly List<int> defaultFpsList = new List<int>()         //список доступных частот кадров трансляций по умолчанию
        {
            30, 25, 15, 5, 60
        };

        private async void ShowImage(object sender, MJPEGStream.FrameReadyEventArgs e)
        {
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(e.FrameBuffer);
                    await writer.StoreAsync();
                }
                await this.coreDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Image.SetSource(stream);
                    RaisePropertyChanged(nameof(StreamPageViewModel.ImageSource));
                });
            };
        }

        private async void ShowError(object sender, MJPEGStream.ErrorEventArgs e)
        {
            await this.coreDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                StreamStop(e.Message);
            });                                   //остановка воспроизведения и вывод сообщения о ошибке в центр окна
        }

        public void StreamStart()
        {
            MJPEGStream.Start(PageDataObject.StreamURI
                + PageDataObject.UriParamsList.ElementAt(PageDataObject.ChosenIndex).Split("&fps=")[0]
                + "&fps=" + Convert.ToString(FpsCollection.ElementAt(FpsCollSelectedIndex)));
            ImageCenterText = pauseText;
        }

        public void StreamStop(string reason = pauseText) 
        { 
            MJPEGStream.Stop();
            ImageCenterText = reason;
        }

        public async void StreamRestart()
        {
            await this.coreDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                MJPEGStream.Stop();
            });
            Thread.Sleep(100);
            await this.coreDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                StreamStart();
            });
        }

        public void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is PageData _pdo && !String.IsNullOrEmpty(_pdo.StreamURI))
                PageDataObject = _pdo;
            ServicePointManager.DefaultConnectionLimit = 1;
            FpsCollection = new ObservableCollection<int>();
            //ImageCenterTextVisibility = false;
            try
            {
                int fps = Convert.ToInt32(PageDataObject.UriParamsList.ElementAt(PageDataObject.ChosenIndex).Split("&fps=")[1]);
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
                ShowError(this, new MJPEGStream.ErrorEventArgs() { Message = fpsRespErrorText });
            }
        }

        public void NavigateTo(Type typeOfPage, NavigationTransitionInfo transitionInfo)
        {
            StreamStop();
            NavigationService.Instance.Navigate(typeOfPage, PageDataObject, transitionInfo);
        }
    }
}
