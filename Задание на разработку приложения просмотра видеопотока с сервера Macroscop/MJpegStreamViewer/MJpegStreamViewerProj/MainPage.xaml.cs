﻿using System;
using System.Net;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;

namespace MJpegStreamViewerProj
{
    public sealed partial class MainPage : Page
    {
        private const string defaultUriForConfigRequest = "http://demo.macroscop.com:8080/configex?login=root";
        private List<string> uriList = new List<string>();
        private List<string> channelList = new List<string>();
        private readonly string confErrorMsg = "Ошибка запроса конфигурации. ";
        private readonly string streamUriInpErrorMsg = "Ошибка ввода URI запроса видеопотока.";
        private bool ExtendedOptionsIsOn;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) 
        {
            uriList = new List<string>();
            channelList = new List<string>();
            requestUriTBox.Text = defaultUriForConfigRequest;
            getChannelsBtn.IsEnabled = true;
            shadeListBoxGrid.Visibility = Visibility.Collapsed;
            overListBoxCenterText.Visibility = Visibility.Collapsed;
            ListProgressRing.IsActive = false;
            PageDataObject pdo = (PageDataObject)e.Parameter;
            ExtendedOptionsIsOn = pdo.ExtOptions;
            if (ExtendedOptionsIsOn)
                ShowExtOpt();
            else
                HideExtOpt();
            GetChannels();
        }

        private void GetChannelsButtonClick(object sender, RoutedEventArgs e) => GetChannels();

        private void GetChannels()
        {
            channelsListBox.ItemsSource = null;
            if (requestUriTBox.Text.Trim() != "" && (requestUriTBox.Text.Trim().IndexOf("http://") == 0 || requestUriTBox.Text.Trim().IndexOf("https://") == 0))
            {
                getChannelsBtn.IsEnabled = false;
                ListProgressRing.IsActive = true;
                serverPartUriTBox.Text = requestUriTBox.Text.Replace("configex", "mobile");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriTBox.Text);
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
            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult);
                System.IO.Stream stream = response.GetResponseStream();
                
                List<XmlResponse.XMLDataItem> responseList = XmlResponse.ReadXMLFromStream(stream);
                stream.Close();
                response.Close();
                foreach (XmlResponse.XMLDataItem responseItem in responseList)
                {
                    int fpsLimit = responseItem.QualityData.FPS;
                    string fps;
                    if (fpsLimit != 0)
                        fps = "&fps=" + fpsLimit.ToString();
                    else 
                        fps = "&fps=60";
                    string uriLine =$"&channelid={responseItem.ChannelData.Id}&resolutionX={responseItem.QualityData.Width}&resolutionY={responseItem.QualityData.Height}{fps}";
                    uriList.Add(uriLine);
                    channelList.Add($"{responseItem.ChannelData.Name} {responseItem.QualityData.Height}x{responseItem.QualityData.Width}");
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

        private void ChannelsListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(serverPartUriTBox.Text.Trim() != "" && (serverPartUriTBox.Text.Trim().IndexOf("http://") == 0 || serverPartUriTBox.Text.Trim().IndexOf("https://") == 0)) 
            {
                PageDataObject pageDataObj = new PageDataObject();
                pageDataObj.ChannelStringList = channelList;
                pageDataObj.UriParamsStringList = uriList;
                pageDataObj.ServerUriPart = serverPartUriTBox.Text;
                pageDataObj.ChosenIndex = channelsListBox.SelectedIndex;
                pageDataObj.ExtOptions = ExtendedOptionsIsOn;
                Frame.Navigate(typeof(StreamPage), pageDataObj, new ContinuumNavigationTransitionInfo());
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

        private void RequestUriTextBoxChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            getChannelsBtn.IsEnabled = false;
            shadeListBoxGrid.Visibility = Visibility.Visible;
        }
        private void RequestUriTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            getChannelsBtn.IsEnabled = true;
            shadeListBoxGrid.Visibility = Visibility.Collapsed;
        }

        private void ServerPartUriTextBoxChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            shadeListBoxGrid.Visibility = Visibility.Visible;
        }

        private void ServerPartUriTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            shadeListBoxGrid.Visibility = Visibility.Collapsed;
        }

        private void ShowExtendedOptions(object sender, RoutedEventArgs e)
        {
            if (ExtendedOptionsIsOn)
            {
                ExtendedOptionsIsOn = false;
                HideExtOpt();
            }
            else
            {
                ExtendedOptionsIsOn = true;
                ShowExtOpt();
            }
        }

        private void ShowExtOpt()
        {
            requestUriTBox.Visibility = Visibility.Visible;
            serverPartUriTBox.Visibility = Visibility.Visible;
        }

        private void HideExtOpt()
        {
            requestUriTBox.Visibility = Visibility.Collapsed;
            serverPartUriTBox.Visibility = Visibility.Collapsed;
        }
    }
}

