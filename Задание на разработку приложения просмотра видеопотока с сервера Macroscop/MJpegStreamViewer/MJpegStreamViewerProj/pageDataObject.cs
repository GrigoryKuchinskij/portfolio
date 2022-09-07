using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJpegStreamViewerProj
{
    class pageDataObject
    {
        public string serverUriPart { get; set; }
        public int chosenIndex { get; set; }
        public bool extOptions { get; set; }
        public List<string> channelStringList { get; set; }
        public List<string> uriParamsStringList { get; set; }
        public pageDataObject() { }
    }
}
