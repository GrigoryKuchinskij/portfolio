using MJpegStreamViewerWPF.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Threading;
using Windows.UI.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using System.IO;
using System.Collections;
using System.ServiceModel;
using System.Windows.Input;
using System.Windows.Navigation;
using MJpegStreamViewerWPF.Utilities;
using static MJpegStreamViewerWPF.XmlResponse;
using System.Windows;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MJpegStreamViewerWPF.ViewModels
{
    class MainPageVM : BindableBase
    {
        private readonly MainPageModel _model = new MainPageModel();
        public MainPageVM()
        {
            _model.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };
            Maximize = new DelegateCommand(() => 
            {
                if (App.Current.MainWindow.WindowState is WindowState.Maximized)
                    App.Current.MainWindow.WindowState = WindowState.Normal;
                else
                    App.Current.MainWindow.WindowState = WindowState.Maximized;
            });
            Minimize = new DelegateCommand(() => App.Current.MainWindow.WindowState = WindowState.Minimized);
            Shutdown = new DelegateCommand(() =>
            {
                if (_model.StreamIsActive) try { _model.StreamStop(); } catch { }
                Task.Delay(200);
                Application.Current.Shutdown();
            });
            GetChannels = new DelegateCommand(async () =>
            {
                GetChannelsButtonEnabled = false;
                ShadeChannelsListVisibility = Visibility.Visible;
                ProgressRingIsOn = true;
                try
                {
                    await _model.GetChannels(UriForConfigRequest);
                }
                catch (Exception ex)
                {
                    await ShowTemporarilyMessageAsync(ex.Message);
                }
                RaisePropertyChanged(nameof(PageDataObject));
                ProgressRingIsOn = false;
                ShadeChannelsListVisibility = Visibility.Collapsed;
                GetChannelsButtonEnabled = true;
                GetChannelsBtnText = "Обновить список";
                PlayBtnIsEnabled = true;
                ExtOptionsBtnIsEnabled = true;
            });
            PlayBtnCommand = new DelegateCommand(async () =>
            {
                ExtendedOptionsIsOn = false;
                PlayBtnIsEnabled = false;
                try
                {
                    if (!_model.StreamIsActive)
                        StreamStart();
                    else
                        StreamStop();
                }
                catch (Exception ex)
                {
                    await ShowTemporarilyMessageAsync(ex.Message);
                }
                await Task.Delay(400);
                PlayBtnIsEnabled = true;
                //RaisePropertyChanged(nameof(PlayBtnIsChecked));
            });
            _model.PageDataObject.StreamURI = uriForConfigRequest.Replace("configex", "mobile");
        }

        private const string pauseText = "Пауза";
        
        //private const string restartText = "Переключение канала";

        private async Task ShowTemporarilyMessageAsync(string message)
        {
            ShadeImageVisibility = Visibility.Visible;
            CenterTextVisibility = Visibility.Visible;
            CenterText = message;
            ProgressRingIsOn = false;
            await Task.Delay(4000);
            ShadeImageVisibility = Visibility.Collapsed;
            CenterTextVisibility = Visibility.Collapsed;
            CenterText = "";
        }

        public PageData PageDataObject
        {
            get => _model.PageDataObject;
            set
            {
                _model.PageDataObject = value;
                RaisePropertyChanged(nameof(PageDataObject));
            }
        }

        private string uriForConfigRequest = "http://demo.macroscop.com:8080/configex?login=root";
        public string UriForConfigRequest
        {
            get => uriForConfigRequest;
            set
            {
                uriForConfigRequest = value;
                _model.PageDataObject.StreamURI = value.Replace("configex", "mobile");
                RaisePropertyChanged(nameof(PageDataObject));
                RaisePropertyChanged(nameof(UriForConfigRequest));
            }
        }

        public bool StreamFrameIsOn
        {
            get => !ExtendedOptionsIsOn;
            set
            {
                ExtendedOptionsIsOn = !value;
                RaisePropertyChanged(nameof(StreamFrameIsOn));
            }
        }

        //private Visibility channelsComboBoxVisibility = Visibility.Collapsed;
        public bool ChannelsComboBoxIsOn
        {
            get => !ExtendedOptionsIsOn;
            private set
            {
                extendedOptionsIsOn = !value;
                RaisePropertyChanged(nameof(ChannelsComboBoxIsOn));
            }
        }

        private bool extOptionsBtnIsEnabled = false;
        public bool ExtOptionsBtnIsEnabled 
        { 
            get => extOptionsBtnIsEnabled;
            set 
            { 
                extOptionsBtnIsEnabled = value;
                RaisePropertyChanged(nameof(ExtOptionsBtnIsEnabled));
            }
        }

        private Visibility shadeImageVisibility = Visibility.Collapsed;
        public Visibility ShadeImageVisibility
        {
            get => shadeImageVisibility;
            private set
            {
                shadeImageVisibility = value;
                RaisePropertyChanged(nameof(ShadeImageVisibility));
            }
        }

        private Visibility shadeChannelsListVisibility = Visibility.Collapsed;
        public Visibility ShadeChannelsListVisibility
        {
            get => shadeChannelsListVisibility;
            private set
            {
                shadeChannelsListVisibility = value;
                RaisePropertyChanged(nameof(ShadeChannelsListVisibility));
            }
        }

        public string CenterText
        {
            get => _model.TextMessage;
            private set
            {
                _model.TextMessage = value;
                RaisePropertyChanged(nameof(CenterText));
            }
        }

        private Visibility centerTextVisibility = Visibility.Collapsed;
        public Visibility CenterTextVisibility
        {
            get => centerTextVisibility;
            private set
            {
                centerTextVisibility = value;
                RaisePropertyChanged(nameof(CenterTextVisibility));
            }
        }

        private bool progressRingIsOn = false;
        public bool ProgressRingIsOn
        {
            get => progressRingIsOn;
            private set
            {
                progressRingIsOn = value;
                RaisePropertyChanged(nameof(ProgressRingIsOn));
            }
        }

        private bool getChannelsButtonEnabled = true;
        public bool GetChannelsButtonEnabled
        {
            get => getChannelsButtonEnabled;
            private set
            {
                getChannelsButtonEnabled = value;
                RaisePropertyChanged(nameof(GetChannelsButtonEnabled));
            }
        }

        private string getChannelsBtnText = "Получить список каналов";
        public string GetChannelsBtnText 
        { 
            get => getChannelsBtnText; 
            private set 
            { 
                getChannelsBtnText = value;
                RaisePropertyChanged(nameof(GetChannelsBtnText));
            } 
        }

        private bool extendedOptionsIsOn = true;
        public bool ExtendedOptionsIsOn
        {
            get => extendedOptionsIsOn;
            set
            {
                extendedOptionsIsOn = value;
                RaisePropertyChanged(nameof(ExtendedOptionsIsOn));
                RaisePropertyChanged(nameof(ChannelsComboBoxIsOn));
                RaisePropertyChanged(nameof(StreamFrameIsOn));
            }
        }

        public int FpsCollSelectedIndex
        {
            get => _model.FpsCollSelectedIndex;
            set
            {
                _model.FpsCollSelectedIndex = value;
                if (_model.StreamIsActive)
                    Application.Current.Dispatcher.BeginInvoke(new System.Action(() => StreamRestart()));
                RaisePropertyChanged(nameof(FpsCollSelectedIndex));
            }
        }

        public int ChannelsSelectedIndex
        {
            get => _model.PageDataObject.SelectedIndex;
            set
            {
                _model.PageDataObject.SelectedIndex = value;
                if (_model.StreamIsActive)
                    Application.Current.Dispatcher.BeginInvoke(new System.Action(() => StreamRestart()));
                RaisePropertyChanged(nameof(ChannelsSelectedIndex));
            }
        }

        public ObservableCollection<int> FpsCollection
        {
            get => _model.FpsCollection;
            private set 
            { 
                _model.FpsCollection = value; 
                RaisePropertyChanged(nameof(FpsCollection)); 
            }
        }

        //public bool PlayBtnIsChecked => _model.StreamIsActive;

        private bool playBtnIsEnabled = false;
        public bool PlayBtnIsEnabled 
        { 
            get => playBtnIsEnabled;
            set
            {
                playBtnIsEnabled = value;
                RaisePropertyChanged(nameof(PlayBtnIsEnabled));
            }
            
        }
        public void StreamStart()
        {
            ShadeImageVisibility = Visibility.Collapsed;
            CenterTextVisibility = Visibility.Collapsed;
            GetChannelsButtonEnabled = false;
            _model.StreamStart(_model.PageDataObject.StreamURI
                + _model.PageDataObject.UriParamsList.ElementAt(_model.PageDataObject.SelectedIndex).Split("&fps=")[0]
                + "&fps=" + Convert.ToString(FpsCollection.ElementAt(FpsCollSelectedIndex)));
        }

        public void StreamStop(string reason = pauseText)
        {
            _model.StreamStop();
            ShadeImageVisibility = Visibility.Visible;
            CenterText = reason;
            CenterTextVisibility = Visibility.Visible;
            GetChannelsButtonEnabled = true;
        }

        public async void StreamRestart()
        {
            await _model.StreamRestart(_model.PageDataObject.StreamURI
                + _model.PageDataObject.UriParamsList.ElementAt(_model.PageDataObject.SelectedIndex).Split("&fps=")[0]
                + "&fps=" + Convert.ToString(FpsCollection.ElementAt(FpsCollSelectedIndex)));
        }

        public WriteableBitmap Image => _model.Image;

        public DelegateCommand GetChannels { get; set; }
        public DelegateCommand Shutdown { get; set; }
        public DelegateCommand Maximize { get; set; }
        public DelegateCommand Minimize { get; set; }
        public DelegateCommand PlayBtnCommand { get; set; }
    }
}
