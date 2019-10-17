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
            _AddressStrings.SetFuzzyInputStr("614000, �������� ����, �. �����, ��. �����, �. 15, ��. 4");
            var result = _AddressStrings.GetStandartAddr();
            output.WriteLine(result);
            string city, street, house, apartm, state, housing, district, village, postindx;
            _AddressStrings.GetInputValues(out postindx, out state, out city, out street, out house, out housing, out apartm, out district, out village);
            output.WriteLine(city + "|" + street + "|" + house + "|" + apartm + "|" + state + "|" + housing + "|" + district + "|" + village + "|" + postindx);
            Assert.True(result!="");
        }



        [Theory]
        [InlineData("614000,�������� ����,����� �����,����� �����,��� 15,������ 3,�������� 4")]
        [InlineData("�������� ��,����������,����������,���15,������3,��������4")]
        [InlineData("������������� �������,��������� ��������,�������,�15,����3,��4")]
        [InlineData("������������� ���,������� ��������,��. �����,�. 15,�3,��. 4")]
        [InlineData("������������� ���.,��.�����,�. ������ ��������,�.15,� 3,��.4")]
        [InlineData("������������� ��� ,�����.������ ��������,�����.�����,���.15,� 3,��.4")]
        [InlineData("���������� ���� (������),���������,�������,�15,����3,��4")]
        [InlineData("����������.���� (������),�������,��. �����,�. 15,�3,��. 4")]
        [InlineData("���� ���� (������),��.�����,�. ������,�. 15,� 3,��.4")]
        [InlineData("����.���� (������),�����.������,����������,���.15,� 3,��4")]
        [InlineData("�.���� (������),��.�����,�.������,�. 15,� 3,��.4")]
        [InlineData("� ���� (������),�����.������,����� �����,���.15,� 3,��4")]
        [InlineData("���������� �����,�.�����,������������� �������.,�.15")]
        [InlineData("������������� �������. ,���������� �����,������,�15")]
        [InlineData("������������� �������.,���������� �����,�15,������")]
        [InlineData("������������� �������,���������� �����,�.15,������")]
        [InlineData("������������� �������. ,���������� �����,�. 15,�.�����")]
        [InlineData("������������� �������. ,����������  �����,�.  15,��������")]
        [InlineData("������������� �������,���������� �����,� 15,���.�����")]
        [InlineData("�������������  �������,���������� �����,�  15,������������")]
        [InlineData("61400,������������� �������.,���������� �����,�.  15,�������.�����")]
        [InlineData("������������� �������. ,���������� �����,���  15,������")]
        [InlineData("������������� �������. ,���������� �����,���15,���������")]
        [InlineData("������������� �������. ,���������� �����,���.15,�.�����")]
        [InlineData("������������� �������. ,���������� �����,�.15,����.�����")]
        [InlineData("������������� �������. ,���������� �����,  � 15,������������,�� ������")]
        [InlineData("������������� �������. ,���������� �����,��3-� �����,������,�15")]
        [InlineData("�������� ��,��� �����,�� �����,� 15,���� 3,�� 4")]
        [InlineData("�������� ��.,���. �����,��. �����,�. 15,����. 3,��. 4")]
        [InlineData("�15,������������� �������. ,���������� �����")] //��������� ������
        [InlineData("��������� ��������,�������,�15,����3,��4")]
        [InlineData("������������� ���,�������,�15,����3,��4")]
        [InlineData("�������� ��,���15,������3,��������4")] //��������� ������
        [InlineData("�������� ����.,����������,����������,�15,������3,��������4")] 
        [InlineData("�������� ����.,����������,����������,�15,������,������3,��������4")] //��������� ������
        [InlineData("������������� �������. ,���������� �����,������")] //��������� ������
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
