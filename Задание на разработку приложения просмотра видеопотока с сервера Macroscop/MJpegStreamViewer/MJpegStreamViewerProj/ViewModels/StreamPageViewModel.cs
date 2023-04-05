using MJpegStreamViewerProj.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace MJpegStreamViewerProj.ViewModels
{
    internal class StreamPageViewModel: BindableBase
    {
        private readonly StreamPageModel _model = new StreamPageModel();
        public StreamPageViewModel()
        {
            _model.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };
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

        public int FpsCollSelectedIndex
        {
            get => _model.FpsCollSelectedIndex;
            set
            {
                _model.FpsCollSelectedIndex = value;
                SelectionChanged();
            }
        }
        public ObservableCollection<int> FpsCollection
        {
            get => _model.FpsCollection;
        }
        public int ChosenChannelIndex
        {
            get => _model.PageDataObject.SelectedIndex;
            set
            {
                _model.PageDataObject.SelectedIndex = value;
                SelectionChanged();
            }
        }

        public bool PlayBtnIsChecked 
        { 
            get => _model.StreamIsActive; 
            set 
            {
                if (value)
                    _model.StreamStart();
                else
                    _model.StreamStop();
            } 
        }
        public bool ImageCenterTextVisibility 
        {
            get => !_model.StreamIsActive;
        }
        public bool ShadeRectangleVisibility
        {
            get => !_model.StreamIsActive;
        }
        public string ImageCenterText 
        { 
            get => _model.ImageCenterText;
        }
        public BitmapSource ImageSource
        {
            get => _model.Image as BitmapSource;
        }

        public void OnNavigatedTo(NavigationEventArgs e)
        {
            _model.OnNavigatedTo(e);
            RaisePropertyChanged(nameof(PageDataObject));
            RaisePropertyChanged(nameof(FpsCollection));
            RaisePropertyChanged(nameof(FpsCollSelectedIndex));
        }

        public void SelectionChanged()
        {
            if (_model.StreamIsActive)
                Task.Run(() => _model.StreamRestart());
        }

        public ICommand NavigateBack =>
            new DelegateCommand(() => _model.NavigateTo(typeof(MainPage),new DrillInNavigationTransitionInfo()));
    }
}
