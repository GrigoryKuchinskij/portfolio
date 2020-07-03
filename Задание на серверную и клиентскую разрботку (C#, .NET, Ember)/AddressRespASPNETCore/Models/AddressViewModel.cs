using System;
using System.Collections.Generic;
using System.Linq;

namespace AddressRespASPNETCore.Models
{
    public class AddressViewModel
    {
        public string fullAddress { get; set; }
        public string postIndex { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string house { get; set; }
        public string housing { get; set; }
        public string apartment { get; set; }
        public string district { get; set; }
        public string village { get; set; }
        //public List<string> addressList { get; set; }
        //public string StandartOutput { get; set; }
    }
}