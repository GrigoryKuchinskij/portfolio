using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MJpegStreamViewerWPF
{
    public class PageData
    {
        public string? StreamURI { get; set; }
        public string? URIForConfigRequest { get; set; }
        public int SelectedIndex { get; set; }
        public ObservableCollection<string>? ChannelsList { get; set; }
        public ObservableCollection<string>? UriParamsList { get; set; }
    }
}
