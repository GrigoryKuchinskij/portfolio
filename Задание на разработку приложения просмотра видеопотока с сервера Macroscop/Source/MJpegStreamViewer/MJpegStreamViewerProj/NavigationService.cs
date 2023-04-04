using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace MJpegStreamViewerProj
{
    internal sealed class NavigationService
    {
        public void Navigate(Type sourcePage)
        {
            Frame frame = (Frame)Window.Current.Content;
            frame.Navigate(sourcePage);
        }

        public void Navigate(Type sourcePage, object parameter)
        {
            Frame frame = (Frame)Window.Current.Content;
            frame.Navigate(sourcePage, parameter);
        }

        public void Navigate(Type sourcePage, object parameter, NavigationTransitionInfo transitionInfo)
        {
            Frame frame = (Frame)Window.Current.Content;
            frame.Navigate(sourcePage, parameter, transitionInfo);
        }

        public void GoBack()
        {
            Frame frame = (Frame)Window.Current.Content;
            frame.GoBack();
        }

        private NavigationService() { }

        private static readonly Lazy<NavigationService> instance =
            new Lazy<NavigationService>(() => new NavigationService());

        public static NavigationService Instance => instance.Value;
    }
}
