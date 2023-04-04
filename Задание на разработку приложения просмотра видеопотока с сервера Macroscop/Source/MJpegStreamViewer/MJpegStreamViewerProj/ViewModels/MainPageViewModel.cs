using MJpegStreamViewerProj.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using System.IO;
using System.Collections;
using System.ServiceModel.Channels;
using System.Windows.Input;

namespace MJpegStreamViewerProj.ViewModels
{
    internal class MainPageViewModel : BindableBase
    {
        private readonly MainPageModel _model = new MainPageModel();
        public MainPageViewModel()
        {
            _model.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };
            ShowExtendedOptions = new DelegateCommand(() =>
            {
                PageDataObject.ExtOptions = !PageDataObject.ExtOptions;
                RaisePropertyChanged(nameof(PageDataObject));
            });
            GetChannels = new DelegateCommand(async () =>
            {
                GetChannelsButtonIsOn = false;
                ShadeIsOn = true;
                ProgressRingIsOn = true;
                try
                {
                    await _model.GetChannels();
                }
                catch (Exception ex)
                {
                    await ShowTemporarilyMessageAsync(ex.Message);
                }
                RaisePropertyChanged(nameof(ChannelsList));
                ProgressRingIsOn = false;
                ShadeIsOn = false;
                GetChannelsButtonIsOn = true;
            });
        }

        private async Task ShowTemporarilyMessageAsync(string message)
        {
            ShadeIsOn = true;
            CenterText = message;
            CenterTextIsOn = true;
            ProgressRingIsOn = false;
            await Task.Delay(4000);
            ShadeIsOn = false;
            CenterTextIsOn = false;
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
        public ObservableCollection<string> ChannelsList
        {
            get => _model.PageDataObject.ChannelsList;
            set
            {
                _model.PageDataObject.ChannelsList = value;
                //RaisePropertyChanged(nameof(ChannelsList));
            }
        }
        public int ChosenIndex
        {
            get => _model.PageDataObject.ChosenIndex;
            set
            {
                _model.PageDataObject.ChosenIndex = value;
                NavigateToStreamPage.Execute(this);
                //RaisePropertyChanged(nameof(ChosenIndex));
            }
        }

        public string UriForConfigRequest
        {
            get => _model.UriForConfigRequest;
            set
            {
                _model.UriForConfigRequest = value;
                RaisePropertyChanged(nameof(UriForConfigRequest));
                RaisePropertyChanged(nameof(PageDataObject.StreamURI));
            }
        }

        //public string UriForStreamRequest
        //{
        //    get => _model.UriForStreamRequest;
        //    set
        //    {
        //        _model.UriForStreamRequest = value;
        //        RaisePropertyChanged(nameof(UriForStreamRequest));
        //    }
        //}
        private string centerText;
        public string CenterText
        {
            get => centerText;
            set
            {
                centerText = value;
                RaisePropertyChanged(nameof(CenterText));
            }
        }


        public void OnNavigatedTo(NavigationEventArgs e)
        {
            _model.OnNavigatedTo(e);
            RaisePropertyChanged(nameof(PageDataObject));
            GetChannels.Execute();
        }

        //public bool ExtendedOptionsIsOn
        //{
        //    get => _model.ExtendedOptionsIsOn;
        //    set
        //    {
        //        _model.ExtendedOptionsIsOn = value;
        //        RaisePropertyChanged(nameof(ExtendedOptionsIsOn));
        //    }
        //}
        private bool shadeIsOn = false;
        public bool ShadeIsOn
        {
            get => shadeIsOn;
            set
            {
                shadeIsOn = value;
                RaisePropertyChanged(nameof(ShadeIsOn));
            }
        }
        private bool centerTextIsOn = false;
        public bool CenterTextIsOn
        {
            get => centerTextIsOn;
            set
            {
                centerTextIsOn = value;
                RaisePropertyChanged(nameof(CenterTextIsOn));
            }
        }
        private bool progressRingIsOn = false;
        public bool ProgressRingIsOn
        {
            get => progressRingIsOn;
            set
            {
                progressRingIsOn = value;
                RaisePropertyChanged(nameof(ProgressRingIsOn));
            }
        }

        private bool getChannelsButtonIsOn = true;
        public bool GetChannelsButtonIsOn
        {
            get => getChannelsButtonIsOn;
            set
            {
                getChannelsButtonIsOn = value;
                RaisePropertyChanged(nameof(GetChannelsButtonIsOn));
            }
        }

        public DelegateCommand ShowExtendedOptions { get; set; }
        public DelegateCommand GetChannels { get; set; }

        public ICommand NavigateToStreamPage =>
            new DelegateCommand(() => _model.NavigateTo(typeof(StreamPage), new ContinuumNavigationTransitionInfo()));
    }
}
