using System;
using System.IO;
using System.Net;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MJpegStreamViewerProj
{
    class XmlResponse
    {
        //public List<UriPartItem> UriPartList { get; private set; }
        //public List<ChannelDataItem> ChannelidList { get; private set; }
        //public List<QualityDataItem> QualityList { get; private set; }

        public class ChannelDataItem
        {
            public string Name { get; set; }
            public string Id { get; set; }
        }

        public class QualityDataItem
        {
            public int Height { get; set; }
            public int Width { get; set; }
            public int FPS { get; set; }
        }

        public class XMLDataItem
        {
            public ChannelDataItem ChannelData { get; set; }
            public QualityDataItem QualityData { get; set; }
        }

        public static List<XMLDataItem> ReadXMLDataFromDocument(XmlDocument xmlDocument, List<ChannelDataItem> ChannelIdList, List<QualityDataItem> QualityList)
        {
            List<XMLDataItem> XMLDataList = new List<XMLDataItem>();
            XmlNodeList channelsXMLList = xmlDocument.GetElementsByTagName("ChannelInfo");
            XmlNodeList resolutionsXMLList = xmlDocument.GetElementsByTagName("ResolutionInfo");
            foreach (XmlNode channel in channelsXMLList)
            {
                XmlNode channelName = channel.Attributes.GetNamedItem("Name");
                XmlNode channelid = channel.Attributes.GetNamedItem("Id");
                ChannelIdList.Add(new ChannelDataItem { Name = channelName.Value, Id = channelid.Value });
            }
            foreach (XmlNode resolutionInfo in resolutionsXMLList)
            {
                XmlNode Width = resolutionInfo.Attributes.GetNamedItem("Width");
                XmlNode Height = resolutionInfo.Attributes.GetNamedItem("Height");
                XmlNode fpsLimit = resolutionInfo.Attributes.GetNamedItem("FpsLimit");
                QualityList.Add(new QualityDataItem { Height = Convert.ToInt32(Width.Value), Width = Convert.ToInt32(Height.Value), FPS = Convert.ToInt32(fpsLimit.Value) });
            }
            foreach (ChannelDataItem channel in ChannelIdList)
            {
                foreach (QualityDataItem quality in QualityList)
                {
                    XMLDataList.Add(new XMLDataItem { ChannelData = channel, QualityData = quality });
                }
            }
            return XMLDataList;
        }

        public static void WriteXMLDataIntoObservableCollections(List<XmlResponse.XMLDataItem> xmlDataList, ObservableCollection<string> URICollection, ObservableCollection<string> Channels)
        {
            foreach (XmlResponse.XMLDataItem dataItem in xmlDataList)
            {
                int fpsLimit = dataItem.QualityData.FPS;
                string fps;
                if (fpsLimit != 0)
                    fps = "&fps=" + fpsLimit.ToString();
                else
                    fps = "&fps=60";
                string uriLine = $"&channelid={dataItem.ChannelData.Id}&resolutionX={dataItem.QualityData.Width}&resolutionY={dataItem.QualityData.Height}{fps}";
                URICollection.Add(uriLine);
                Channels.Add($"{dataItem.ChannelData.Name} {dataItem.QualityData.Height}x{dataItem.QualityData.Width}");
            }
        }
    }
}
