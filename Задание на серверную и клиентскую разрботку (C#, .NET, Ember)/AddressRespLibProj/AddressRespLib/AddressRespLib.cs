using System;

namespace AddressRespLib
{
    public class AddressStrings
    {
        private string city, street, house, apartm, state, housing, district, village, postindx;

        public AddressStrings() : this("", "", "", "", "", "", "", "", "")
        {
        }

        public AddressStrings(string _postindx, string _state, string _city, string _street, string _house, string _housing, string _apartm, string _district, string _village)
        {
            postindx = _postindx;
            state = _state;
            city = _city;
            street = _street;
            house = _house;
            housing = _housing;
            apartm = _apartm;
            district = _district;
            village = _village;
        }

        public void GetInputValues(out string _postindx, out string _state, out string _city, out string _street, out string _house, out string _housing, out string _apartm, out string _district, out string _village)
        {
            _postindx = postindx;
            _state = state;
            _city = city;
            _street = street;
            _house = house;
            _housing = housing;
            _apartm = apartm;
            _district = district;
            _village = village;
        }

        public string GetStandartAddr()
        {
            string str = "";
            if (!String.IsNullOrEmpty(city)|| !String.IsNullOrEmpty(village))
            {
                if (!String.IsNullOrEmpty(city))
                {
                    str = (!String.IsNullOrEmpty(postindx)) ? postindx + ", " : "";
                    str = str + state + ", " + city + ", " + street + ", " + house;
                    str = (!String.IsNullOrEmpty(housing)) ? str + ", " + housing : str;
                    str = (!String.IsNullOrEmpty(apartm)) ? str + ", " + apartm : str;
                }
                if (!String.IsNullOrEmpty(village))
                {
                    str = (!String.IsNullOrEmpty(postindx)) ? postindx + ", " : "";
                    str = str + state;
                    str = (!String.IsNullOrEmpty(district)) ? str + ", " + district  : str;
                    str = str + ", " + village;
                    str = (!String.IsNullOrEmpty(street)) ? str + ", " + street : str;
                    str = str + ", " + house;
                    str = (!String.IsNullOrEmpty(housing)) ? str + ", " + housing : str;
                    str = (!String.IsNullOrEmpty(apartm)) ? str + ", " + apartm : str;
                }
            }
            return str;
        }

        public void SetFuzzyInputStr(String str)
        {
            char[] punct = { ',' };
            string[] splittedStr = str.Split(punct);
            postindx = state = city = street = house = housing = apartm = district = village = "";
            string[] cityMark = { " г", " г.", " гор", " гор.", " город", " город." };
            string[] streetMark = { " ул", " ул.", " улица", " улица." };
            string[] houseMark = { " дом", " дом." };
            string[] dMark = { " д", " д." };
            string[] apartMark = { " кв", " кв.", " квартира", " квартира." };
            string[] stateMark = { " Р ", " Р.", " р ", " р.", " Респ ", " Респ.", " респ ", " респ.", " Республика ", " Республика.", " республика ", " республика.", " кр. ", " кр ", " край ", " край. ", " обл ", " обл. ", " область ", " область. ",  };
            string[] housingMark = { " к", " к.", " кор", " кор.", " корп", " корп.", " корпус", " корпус." }; 
            string distrMark = " район";
            string[] villMark = { " дер", " дер.", " деревня", " деревня.", " п", " п.", " пос", " пос."
                    , " поселок", " поселок.", " посёлок", " посёлок.", " с", " с.", " село", " село.", };

            foreach (string subst in splittedStr)
            {
                string substr = " " + subst + " ";
                foreach (string mrk in cityMark)
                {
                    if (substr.IndexOf(mrk) != -1)
                    {
                        string cityTmp = substr.Replace(mrk, ""); cityTmp = cityTmp.Trim();
                        if (char.IsUpper(Convert.ToChar(cityTmp.Substring(0, 1))))
                        { city = "г. " + cityTmp; break; };
                    };
                }

                foreach (string mrk in streetMark)
                {
                    if (substr.IndexOf(mrk) != -1)
                    {
                        string streetTmp = substr.Replace(mrk, ""); streetTmp = streetTmp.Trim();
                        if (char.IsUpper(Convert.ToChar(streetTmp.Substring(0, 1))) || char.IsDigit(Convert.ToChar(streetTmp.Substring(0, 1))))
                        { street = "ул. " + streetTmp; break; };
                    };
                }

                foreach (string mrk in dMark)
                {
                    if (substr.IndexOf(mrk) != -1)
                    {
                        string dTmp = substr.Replace(mrk, "");
                        dTmp = dTmp.Trim();
                        if (char.IsUpper(Convert.ToChar(dTmp.Substring(0, 1)))) { village = "дер. " + dTmp; break; };
                        if (char.IsDigit(Convert.ToChar(dTmp.Substring(0, 1)))) { house = "д. " + dTmp; break; };
                    };
                }

                foreach (string mrk in houseMark)
                {
                    if (substr.IndexOf(mrk) != -1)
                    {
                        string houseTmp = substr.Replace(mrk, ""); houseTmp = houseTmp.Trim();
                        if (char.IsDigit(Convert.ToChar(houseTmp.Substring(0, 1))))
                        {
                            house = "д. " +  houseTmp; break;
                        };
                    };
                }

                foreach (string mrk in apartMark)
                {
                    if (substr.IndexOf(mrk) != -1)
                    {
                        string apartmTmp = substr.Replace(mrk, ""); apartmTmp = apartmTmp.Trim();
                        if (char.IsDigit(Convert.ToChar(apartmTmp.Substring(0, 1))))
                        {
                            apartm = "кв. " + apartmTmp; break;
                        };
                    };
                }

                for (int Imrk = 0; Imrk < stateMark.Length; Imrk++)
                {
                    if (substr.IndexOf(stateMark[Imrk]) != -1)
                    {
                        string stateTmp = substr.Replace(stateMark[Imrk], ":&"); stateTmp = stateTmp.Trim();
                        if (char.IsUpper(Convert.ToChar(stateTmp.Replace(":&", "").Substring(0, 1))))
                        {
                            if (Imrk >= 16) { stateTmp = stateTmp.Replace(":&", ""); stateTmp = stateTmp + " область"; }
                            else
                            {
                                if (Imrk >= 12) { stateTmp = stateTmp.Replace(":&", ""); stateTmp = stateTmp + " край"; }
                                else
                                { stateTmp = stateTmp.Replace(":&", " Республика "); }
                            }
                            state = stateTmp;
                            break;
                        };
                    };
                }

                foreach (string mrk in housingMark)
                {
                    if (substr.IndexOf(mrk) != -1)
                    {
                        string korpusTmp = substr.Replace(mrk, ""); korpusTmp = korpusTmp.Trim();
                        if (char.IsDigit(Convert.ToChar(korpusTmp.Substring(0, 1))))
                        { housing = "корпус " + korpusTmp; break; };
                    };
                }

                if (substr.IndexOf(distrMark) != -1)
                {
                    string distrTmp = substr.Replace(distrMark, ""); distrTmp = distrTmp.Trim();
                    if (char.IsUpper(Convert.ToChar(distrTmp.Substring(0, 1)))) district = distrTmp + " район";
                };

                for (int Imrk = 0; Imrk < villMark.Length; Imrk++)
                {
                    if (substr.IndexOf(villMark[Imrk]) != -1)
                    {
                        string villTmp = substr.Replace(villMark[Imrk], ""); villTmp = villTmp.Trim();
                        if (char.IsUpper(Convert.ToChar(villTmp.Substring(0, 1))))
                        {
                            if (Imrk <= 3) { villTmp = "дер. " + villTmp; };
                            if (Imrk > 3 && Imrk <= 11) { villTmp = "пос. " + villTmp; };
                            if (Imrk > 11) { villTmp = "с. " + villTmp; };
                            village = villTmp;
                            break;
                        };
                    };
                }
                if (System.Text.RegularExpressions.Regex.IsMatch(substr, @"\d{6}")) { postindx = substr.Trim(); };
            }

            if (state == "" || (city == "" && village == "")) { state = "Пермский край"; city = "Пермь"; district = ""; }
            if ((street == "" && city != "") || house == "" || (village != "" && city != ""))
            {
                postindx = state = city = street = house = housing = apartm = district = village = "";
                //throw new System.ArgumentException("Ошибка распознавания адреса!");
            }
        }
    }
}