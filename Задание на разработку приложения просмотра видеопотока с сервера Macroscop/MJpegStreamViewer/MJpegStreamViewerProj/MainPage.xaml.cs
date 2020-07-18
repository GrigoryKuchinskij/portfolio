using System;
using System.Net;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace MJpegStreamViewerProj
{
    public sealed partial class MainPage : Page
    {
        private const string defaultUriForConfigRequest = "http://demo.macroscop.com:8080/configex?login=root";
        private List<string> uriList = new List<string>();
        private List<string> channelList = new List<string>();
        private readonly string confErrorMsg = "Ошибка запроса конфигурации. ";
        private readonly string streamUriInpErrorMsg = "Ошибка ввода URI запроса видеопотока.";

        //public BitmapImage BitmapImage; //{ get; set; }
        public MainPage()
        {
            this.InitializeComponent();            
        }

        protected override void OnNavigatedTo(NavigationEventArgs _) 
        {
            uriList = new List<string>();
            channelList = new List<string>();
            //DataContext = this;
            requestUriTBox.Text = defaultUriForConfigRequest;
            getChannelsBtn.IsEnabled = true;
            serverPartUriTBox.Visibility = Visibility.Collapsed;
            //shadeListBoxGrid.Opacity = 0.5d;
            shadeListBoxGrid.Visibility = Visibility.Collapsed;
            overListBoxCenterText.Visibility = Visibility.Collapsed;
            ListProgressRing.IsActive = false;
            getChannels(); 
        }

        private void getChannelsBtn_Click(object sender, RoutedEventArgs e) => getChannels();

        private void getChannels() 
        {
            channelsListBox.ItemsSource = null;
            if (requestUriTBox.Text.Trim() != "" && (requestUriTBox.Text.Trim().IndexOf("http://") == 0 || requestUriTBox.Text.Trim().IndexOf("https://") == 0))
            {
                getChannelsBtn.IsEnabled = false;
                ListProgressRing.IsActive = true;
                serverPartUriTBox.Visibility = Visibility.Visible;
                serverPartUriTBox.Text = requestUriTBox.Text.Replace("configex", "mobile");
                var request = (HttpWebRequest)HttpWebRequest.Create(requestUriTBox.Text);
                request.BeginGetResponse(OnGetXmlResponse, request);
            }
            else
            {
                ListProgressRing.IsActive = false;
                getChannelsBtn.IsEnabled = true;
                channelList.Clear();
                ShowTemporarilyMessageAsync(confErrorMsg, 4000);
            }
        }

        private void OnGetXmlResponse(IAsyncResult asyncResult)
        {
            channelList.Clear();
            uriList.Clear();
            var request = (HttpWebRequest)asyncResult.AsyncState;
            try
            {
                var response = (HttpWebResponse)request.EndGetResponse(asyncResult);
                var stream = response.GetResponseStream();
                var responseList = XmlResponseClass.XmlResponse(stream);
                stream.Close();
                response.Close();
                foreach (string responseLine in responseList)
                {
                    var values = responseLine.Split("%#");
                    var fpsLimit = Convert.ToInt32(values[2]);
                    string fps;
                    if (fpsLimit != 0) fps = "&fps=" + fpsLimit.ToString(); else fps = "&fps=60";
                    var uriLine = values[1] + fps;
                    uriList.Add(uriLine);
                    var quality = values[1].Split("&resolutionY=")[1].Split("&")[0] + "p";
                    var channelStr = values[0] + " " + quality + " " + values[1].Replace("&"," ");
                    channelList.Add(channelStr);
                }                
            }
            catch (Exception ex)
            {
                ShowTemporarilyMessageAsync(confErrorMsg + ex.Message, 4000);
                channelList.Clear();
                uriList.Clear();
            }
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                { 
                    channelsListBox.ItemsSource = channelList;
                    ListProgressRing.IsActive = false;
                    getChannelsBtn.IsEnabled = true;
                    shadeListBoxGrid.Visibility = Visibility.Collapsed;
                    getChannelsBtn.Focus(FocusState.Programmatic);
                });
            request.Abort();            
        }

        private void channelsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            if(serverPartUriTBox.Text.Trim() != "" && (serverPartUriTBox.Text.Trim().IndexOf("http://") == 0 || serverPartUriTBox.Text.Trim().IndexOf("https://") == 0)) 
            {
                receivedDataObject recDataObj = new receivedDataObject();
                recDataObj.channelStringList = channelList;
                recDataObj.uriParamsStringList = uriList;
                recDataObj.serverUriPart = serverPartUriTBox.Text;
                recDataObj.chosenIndex = channelsListBox.SelectedIndex;
                //if (Frame.CanGoForward)
                //    Frame.GoForward();
                //else
                Frame.Navigate(typeof(StreamPage), recDataObj);                
            }
            else
            {
                ShowTemporarilyMessageAsync(streamUriInpErrorMsg, 4000);
                channelsListBox.SelectedIndex = -1;
            }            
        }

        private async void ShowTemporarilyMessageAsync(string message, int millisecondsTimeout) 
        {
            await Task.Run(() =>
            {
                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    shadeListBoxGrid.Visibility = Visibility.Visible;
                    overListBoxCenterText.Visibility = Visibility.Visible;
                    overListBoxCenterText.Text = message;
                });
                Thread.Sleep(millisecondsTimeout);
                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    shadeListBoxGrid.Visibility = Visibility.Collapsed;
                    overListBoxCenterText.Visibility = Visibility.Collapsed;
                    overListBoxCenterText.Text = "";
                });
            });
        }

        private void requestUriTBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            getChannelsBtn.IsEnabled = false;
            //channelsListBox.IsEnabled = false;
            shadeListBoxGrid.Visibility = Visibility.Visible;
        }
        private void requestUriTBox_LostFocus(object sender, RoutedEventArgs e)
        {
            getChannelsBtn.IsEnabled = true;
            //channelsListBox.IsEnabled = true;
            shadeListBoxGrid.Visibility = Visibility.Collapsed;
        }

        private void serverPartUriTBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            //channelsListBox.IsEnabled = false;
            shadeListBoxGrid.Visibility = Visibility.Visible;
        }

        private void serverPartUriTBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //channelsListBox.IsEnabled = true;
            shadeListBoxGrid.Visibility = Visibility.Collapsed;
        }
    }
}

