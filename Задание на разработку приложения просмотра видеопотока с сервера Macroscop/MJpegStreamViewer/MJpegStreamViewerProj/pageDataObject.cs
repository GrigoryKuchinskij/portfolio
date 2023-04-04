using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MJpegStreamViewerProj
{
    public class PageData
    {
        public string StreamURI { get; set; }
        public int ChosenIndex { get; set; }
        public bool ExtOptions { get; set; }
        public ObservableCollection<string> ChannelsList { get; set; }
        public ObservableCollection<string> UriParamsList { get; set; }
    }
}
