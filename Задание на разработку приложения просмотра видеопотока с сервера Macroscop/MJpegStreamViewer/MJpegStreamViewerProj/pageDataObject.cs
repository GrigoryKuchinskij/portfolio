using System.Collections.Generic;

namespace MJpegStreamViewerProj
{
    internal class PageDataObject
    {
        public string ServerUriPart { get; set; }
        public int ChosenIndex { get; set; }
        public bool ExtOptions { get; set; }
        public List<string> ChannelStringList { get; set; }
        public List<string> UriParamsStringList { get; set; }
        public PageDataObject() { }
    }
}
