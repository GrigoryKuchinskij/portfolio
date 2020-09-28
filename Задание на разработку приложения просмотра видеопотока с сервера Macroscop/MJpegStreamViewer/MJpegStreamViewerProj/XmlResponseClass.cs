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

namespace MJpegStreamViewerProj
{
    class XmlResponseClass
    {
        public static List<string> XmlResponse(Stream stream)
        {
            List<string> uriPartList = new List<string>();
            List<string> channelidList = new List<string>();
            List<string> resolutionList = new List<string>();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(stream);
            XmlNodeList channelXMLList = xDoc.GetElementsByTagName("ChannelInfo");
            XmlNodeList resolutionXMLList = xDoc.GetElementsByTagName("ResolutionInfo");
            for (int i = 0; i < channelXMLList.Count; i++)
            {
                XmlNode channel = channelXMLList.Item(i);
                XmlNode channelName = channel.Attributes.GetNamedItem("Name");
                XmlNode channelid = channel.Attributes.GetNamedItem("Id");
                channelidList.Add(channelName.Value.Replace("%#","") + "%#" + "&channelid=" + channelid.Value);
            }
            for (int i = 0; i < resolutionXMLList.Count; i++)
            {
                XmlNode resolutionInfo = resolutionXMLList.Item(i);
                XmlNode resolutionX = resolutionInfo.Attributes.GetNamedItem("Width");
                XmlNode resolutionY = resolutionInfo.Attributes.GetNamedItem("Height");
                XmlNode fpsLimit = resolutionInfo.Attributes.GetNamedItem("FpsLimit");
                resolutionList.Add("&resolutionX=" + resolutionX.Value + "&resolutionY=" + resolutionY.Value + "%#" + fpsLimit.Value);
            }
            foreach (string channelid in channelidList)
            {
                foreach (string resolution in resolutionList)
                {
                    uriPartList.Add(channelid + resolution);
                }
            }
            return uriPartList;
        }
    }
}
