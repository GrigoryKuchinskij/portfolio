using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
using Prism.Commands;
using System.Net;
using MJpegStreamViewerProj.ViewModels;
using System.Collections.ObjectModel;
using System.IO;
using Windows.UI.Core;
using System.Xml;
using static MJpegStreamViewerProj.XmlResponse;

namespace MJpegStreamViewerProj.Models
{
    public class MainPageModel : BindableBase
    {
        public PageData PageDataObject { get; set; } = new PageData
        {
            SelectedIndex = -1,
            ExtendedOptionsIsOn = false,
            ChannelsList = new ObservableCollection<string>(),
            UriParamsList = new ObservableCollection<string>()
        };

        public MainPageModel()
        {
            //PageDataObject.StreamURI = uriForConfigRequest.Replace("configex", "mobile");
        }

        private const string confErrorMsg = "Ошибка запроса конфигурации. ";
        private const string streamUriInpErrorMsg = "Ошибка ввода URL запроса видеопотока.";
        private const string requestUriInpErrorMsg = "Ошибка ввода URL запроса доступных каналов.";
                
        private string uriForConfigRequest = "http://demo.macroscop.com:8080/configex?login=root";
        public string UriForConfigRequest { 
            get => uriForConfigRequest; 
            set 
            {
                uriForConfigRequest = value;
                PageDataObject.StreamURI = value.Replace("configex", "mobile");
            } 
        }

        public async Task GetChannels()
        {
            PageDataObject.ChannelsList = new ObservableCollection<string>();
            PageDataObject.UriParamsList = new ObservableCollection<string>();
            try
            {
                if (UriForConfigRequest.Trim() != "" && (UriForConfigRequest.Trim().IndexOf("http://") == 0 || UriForConfigRequest.Trim().IndexOf("https://") == 0))
                {
                    List<XmlResponse.XMLDataItem> responseList = new List<XmlResponse.XMLDataItem>();
                    await Task.Run(() =>
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UriForConfigRequest);
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        System.IO.Stream stream = response.GetResponseStream();
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(stream);
                        stream.Close();
                        List<ChannelDataItem> ChannelIdList = new List<ChannelDataItem>();
                        List<QualityDataItem> QualityList = new List<QualityDataItem>();
                        responseList = XmlResponse.ReadXMLDataFromDocument(xmlDoc, ChannelIdList, QualityList);

                    });
                    XmlResponse.WriteXMLDataIntoObservableCollections(responseList, PageDataObject.UriParamsList, PageDataObject.ChannelsList);
                }
                else
                {
                    UriFormatException Exception = new UriFormatException(requestUriInpErrorMsg);
                    throw Exception;
                }
            }
            catch (UriFormatException ex) { throw ex; }
            catch
            {
                PageDataObject.ChannelsList.Clear();
                PageDataObject.UriParamsList.Clear();
                Exception Exception = new Exception(confErrorMsg);
                throw Exception;
            }
        }

        public void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is PageData pdo && !String.IsNullOrEmpty(pdo.StreamURI))
                PageDataObject = pdo;
            else
                PageDataObject.StreamURI = UriForConfigRequest.Replace("configex", "mobile");
        }

        public void NavigateTo(Type typeOfPage, NavigationTransitionInfo transitionInfo)
        {
            if (PageDataObject.StreamURI.Trim() != "" && (PageDataObject.StreamURI.Trim().IndexOf("http://") == 0 || PageDataObject.StreamURI.Trim().IndexOf("https://") == 0))
            {
                NavigationService.Instance.Navigate(typeOfPage, PageDataObject, transitionInfo);
            }
            else
            {
                PageDataObject.SelectedIndex = -1;
                Exception Exception = new Exception(streamUriInpErrorMsg);
                throw Exception;
            }
        }
    }
}
