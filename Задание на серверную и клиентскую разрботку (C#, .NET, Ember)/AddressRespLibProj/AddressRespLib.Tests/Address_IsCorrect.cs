using System;
using Xunit;
using Xunit.Abstractions;
using AddressRespLib;

namespace AddressRespLib.UnitTests.AddrObjects
{
    public class Address_IsCorrect
    {
        private readonly AddressStrings _AddressStrings;

        public Address_IsCorrect(ITestOutputHelper output)
        {
            _AddressStrings = new AddressStrings();
            this.output = output;
        }

        private readonly ITestOutputHelper output;

        [Fact]
        public void IsCorrect_InputStandAddr_ReturnTrue()
        {
            _AddressStrings.SetFuzzyInputStr("614000, Пермский край, г. Пермь, ул. Синяя, д. 15, кв. 4");
            var result = _AddressStrings.GetStandartAddr();
            output.WriteLine(result);
            string city, street, house, apartm, state, housing, district, village, postindx;
            _AddressStrings.GetInputValues(out postindx, out state, out city, out street, out house, out housing, out apartm, out district, out village);
            output.WriteLine(city + "|" + street + "|" + house + "|" + apartm + "|" + state + "|" + housing + "|" + district + "|" + village + "|" + postindx);
            Assert.True(result!="");
        }



        [Theory]
        [InlineData("614000,Пермский край,город Пермь,улица Синяя,дом 15,корпус 3,квартира 4")]
        [InlineData("Пермский кр,городПермь,улицаСиняя,дом15,корпус3,квартира4")]
        [InlineData("Нижегородская область,горНижний Новгород,улСиняя,д15,корп3,кв4")]
        [InlineData("Нижегородская обл,гНижний Новгород,ул. Синяя,д. 15,к3,кв. 4")]
        [InlineData("Нижегородская обл.,ул.Синяя,г. Нижний Новгород,д.15,к 3,кв.4")]
        [InlineData("Нижегородская обл ,город.Нижний Новгород,улица.Синяя,дом.15,к 3,кв.4")]
        [InlineData("Республика Саха (Якутия),горЯкутск,улСиняя,д15,корп3,кв4")]
        [InlineData("республика.Саха (Якутия),гЯкутск,ул. Синяя,д. 15,к3,кв. 4")]
        [InlineData("Респ Саха (Якутия),ул.Синяя,г. Якутск,д. 15,к 3,кв.4")]
        [InlineData("Респ.Саха (Якутия),город.Якутск,улицаСиняя,дом.15,к 3,кв4")]
        [InlineData("р.Саха (Якутия),ул.Синяя,г.Якутск,д. 15,к 3,кв.4")]
        [InlineData("р Саха (Якутия),город.Якутск,улица Синяя,дом.15,к 3,кв4")]
        [InlineData("Городецкий район,д.Синяя,Нижегородская область.,д.15")]
        [InlineData("Нижегородская область. ,Городецкий район,дСиняя,д15")]
        [InlineData("Нижегородская область.,Городецкий район,д15,дСиняя")]
        [InlineData("Нижегородская область,Городецкий район,д.15,пСиний")]
        [InlineData("Нижегородская область. ,Городецкий район,д. 15,п.Синий")]
        [InlineData("Нижегородская область. ,Городецкий  район,д.  15,посСиний")]
        [InlineData("Нижегородская область,Городецкий район,д 15,пос.Синий")]
        [InlineData("Нижегородская  область,Городецкий район,д  15,поселокСиний")]
        [InlineData("61400,Нижегородская область.,Городецкий район,д.  15,поселок.Синий")]
        [InlineData("Нижегородская область. ,Городецкий район,дом  15,сСинее")]
        [InlineData("Нижегородская область. ,Городецкий район,дом15,селоСинее")]
        [InlineData("Нижегородская область. ,Городецкий район,дом.15,с.Синее")]
        [InlineData("Нижегородская область. ,Городецкий район,д.15,село.Синее")]
        [InlineData("Нижегородская область. ,Городецкий район,  д 15,деревняСиняя,ул Нижняя")]
        [InlineData("Нижегородская область. ,Городецкий район,ул3-я линия,дСиняя,д15")]
        [InlineData("Пермский кр,гор Пермь,ул Синяя,д 15,корп 3,кв 4")]
        [InlineData("Пермский кр.,гор. Пермь,ул. Синяя,д. 15,корп. 3,кв. 4")]
        [InlineData("д15,Нижегородская область. ,Городецкий район")] //ошибочная строка
        [InlineData("горНижний Новгород,улСиняя,д15,корп3,кв4")]
        [InlineData("Нижегородская обл,улСиняя,д15,корп3,кв4")]
        [InlineData("Пермский кр,дом15,корпус3,квартира4")] //ошибочная строка
        [InlineData("Пермский край.,городПермь,улицаСиняя,д15,корпус3,квартира4")] 
        [InlineData("Пермский край.,городПермь,улицаСиняя,д15,дСиняя,корпус3,квартира4")] //ошибочная строка
        [InlineData("Нижегородская область. ,Городецкий район,дСиняя")] //ошибочная строка
        public void IsCorrect_ValuesIsFuzzy_ReturnTrue(string value)
        {
            _AddressStrings.SetFuzzyInputStr(value);
            var result = _AddressStrings.GetStandartAddr();
            output.WriteLine(result);
            string city, street, house, apartm, state, housing, district, village, postindx;
            _AddressStrings.GetInputValues(out postindx, out state, out city, out street, out house, out housing, out apartm, out district, out village);
            output.WriteLine(city + "|" + street + "|" + house + "|" + apartm + "|" + state + "|" + housing + "|" + district + "|" + village + "|" + postindx);
            Assert.True(result != "");
        }
    }
}
