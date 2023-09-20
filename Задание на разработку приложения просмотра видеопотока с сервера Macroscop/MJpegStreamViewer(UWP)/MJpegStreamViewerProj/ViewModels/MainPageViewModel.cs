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
                PageDataObject.ExtendedOptionsIsOn = !PageDataObject.ExtendedOptionsIsOn;
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
                RaisePropertyChanged(nameof(PageDataObject));
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
            private set 
            {
                _model.PageDataObject = value;
                RaisePropertyChanged(nameof(PageDataObject));
            }
        }

        public int SelectedIndex
        {
            get => _model.PageDataObject.SelectedIndex;
            set
            {
                _model.PageDataObject.SelectedIndex = value;
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
                RaisePropertyChanged(nameof(PageDataObject));
            }
        }

        private string centerText;
        public string CenterText
        {
            get => centerText;
            private set
            {
                centerText = value;
                RaisePropertyChanged(nameof(CenterText));
            }
        }

        private bool shadeIsOn = false;
        public bool ShadeIsOn
        {
            get => shadeIsOn;
            private set
            {
                shadeIsOn = value;
                RaisePropertyChanged(nameof(ShadeIsOn));
            }
        }
        private bool centerTextIsOn = false;
        public bool CenterTextIsOn
        {
            get => centerTextIsOn;
            private set
            {
                centerTextIsOn = value;
                RaisePropertyChanged(nameof(CenterTextIsOn));
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

        private bool getChannelsButtonIsOn = true;
        public bool GetChannelsButtonIsOn
        {
            get => getChannelsButtonIsOn;
            private set
            {
                getChannelsButtonIsOn = value;
                RaisePropertyChanged(nameof(GetChannelsButtonIsOn));
            }
        }

        public void OnNavigatedTo(NavigationEventArgs e)
        {
            _model.OnNavigatedTo(e);
            RaisePropertyChanged(nameof(PageDataObject));
            GetChannels.Execute();
        }

        public DelegateCommand ShowExtendedOptions { get; set; }
        public DelegateCommand GetChannels { get; set; }

        public ICommand NavigateToStreamPage =>
            new DelegateCommand(() => _model.NavigateTo(typeof(StreamPage), new ContinuumNavigationTransitionInfo()));
    }
}
