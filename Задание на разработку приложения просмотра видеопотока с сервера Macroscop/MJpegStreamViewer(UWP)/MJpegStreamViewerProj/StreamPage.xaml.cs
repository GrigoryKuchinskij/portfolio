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
using System.Collections.ObjectModel;
using MJpegStreamViewerProj.ViewModels;
using System.Collections;
using MJpegStreamViewerProj;

namespace MJpegStreamViewerProj
{
    public sealed partial class StreamPage : Page
    {
        private StreamPageViewModel viewModel;
        public StreamPage()
        {
            this.InitializeComponent();
            DataContext = new StreamPageViewModel();
            viewModel = (StreamPageViewModel)DataContext;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            viewModel.OnNavigatedTo(e);
        }
    }
}

