using System;
using System.Net;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
using MJpegStreamViewerProj.ViewModels;
using System.Collections;
using MJpegStreamViewerProj;



namespace MJpegStreamViewerProj
{
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel viewModel;
        public MainPage()
        {
            this.InitializeComponent();
            DataContext = new MainPageViewModel();
            viewModel = (MainPageViewModel)DataContext;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) 
        {            
            viewModel.OnNavigatedTo(e);
        }
    }
}

