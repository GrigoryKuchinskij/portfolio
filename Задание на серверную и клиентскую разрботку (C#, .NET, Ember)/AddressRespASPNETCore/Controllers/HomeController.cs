using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AddressRespASPNETCore.Models;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace AddressRespASPNETCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult InputPage()
        {
            AddressViewModel avm =new AddressViewModel();
            return View(avm);
        }

        [HttpPost]
        public IActionResult InputPage(AddressViewModel _addrElements)
        {
            string FuzzyInput = _addrElements.fullAddress;
            _addrElements = Input(_addrElements);
            ViewBag.FuzzyInput = FuzzyInput;
            ViewModelToViewBag(_addrElements);
            return View();
        }

        [HttpPost]
        public IActionResult OutputPage(AddressViewModel _addrElements)
        {
            AddressRespLib.AddressStrings aSObj = new AddressRespLib.AddressStrings();
            aSObj.SetFuzzyInputStr(_addrElements.postIndex + "," + _addrElements.state + "," + _addrElements.city + "," + _addrElements.street + "," + _addrElements.house + "," + _addrElements.housing + "," + _addrElements.apartment + "," + _addrElements.district + "," + _addrElements.village);
            ViewBag.StandartOutput = aSObj.GetStandartAddr();
            ViewModelToViewBag(_addrElements);
            return View();
        }

        private AddressViewModel Input(AddressViewModel _addrElements)
        {
            string FuzzyInput = _addrElements.fullAddress;
            AddressRespLib.AddressStrings aSObj = new AddressRespLib.AddressStrings();
            if (!String.IsNullOrEmpty(FuzzyInput))
            {
                aSObj.SetFuzzyInputStr(FuzzyInput);
            }
            else
            {
                aSObj.SetFuzzyInputStr(_addrElements.postIndex + "," + _addrElements.state + "," + _addrElements.city + "," + _addrElements.street + "," + _addrElements.house + "," + _addrElements.housing + "," + _addrElements.apartment + "," + _addrElements.district + "," + _addrElements.village);
            };
            aSObj.GetInputValues(out string _postindx, out string _state, out string _city, out string _street, out string _house, out string _housing, out string _apartm, out string _district, out string _village);
            _addrElements = new AddressViewModel { fullAddress = "", postIndex = _postindx, state = _state, city = _city, street = _street, house = _house, housing = _housing, apartment = _apartm, district = _district, village = _village };

            return _addrElements;
        }

        [HttpPost("Input")]
        public JsonResult InputREST( AddressViewModel avm)
        {
            avm = Input(avm);        
            return Json(avm);
        }

        [HttpPost("Output")]
        public JsonResult OutputREST(AddressViewModel _addrElements)
        {
            AddressRespLib.AddressStrings aSObj = new AddressRespLib.AddressStrings();
            aSObj.SetFuzzyInputStr(_addrElements.postIndex + "," + _addrElements.state + "," + _addrElements.city + "," + _addrElements.street + "," + _addrElements.house + "," + _addrElements.housing + "," + _addrElements.apartment + "," + _addrElements.district + "," + _addrElements.village);
            _addrElements = new AddressViewModel { fullAddress = aSObj.GetStandartAddr(), postIndex = "", state = "", city = "", street = "", house = "", housing = "", apartment = "", district = "", village = "" };
            return Json(_addrElements);
        }

        private void ViewModelToViewBag(AddressViewModel fields)
        {
            ViewBag.PostIndex = fields.postIndex;
            ViewBag.State = fields.state;
            ViewBag.City = fields.city;
            ViewBag.Street = fields.street;
            ViewBag.House = fields.house;
            ViewBag.Housing = fields.housing;
            ViewBag.Apartment = fields.apartment;
            ViewBag.District = fields.district;
            ViewBag.Village = fields.village;
        }
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
