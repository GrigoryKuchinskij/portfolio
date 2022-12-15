using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using System.IO;
using Prism.Commands;
using Prism.Mvvm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.Threading;
using Microsoft.Win32;
using System.Windows;
using StenographAudio_WPF_.ViewModels;
using Prism.Services.Dialogs;

namespace StenographAudio_WPF_.Models
{
    class MainWindowModel : BindableBase
    {
        public string InputSoundFilePath { get; set; }
        public string InputFileForHidingPath { get; set; }
        public string OutputEncrFilePath { get; set; }
        public int BitsForCryptCount { get; set; }
        public string InputEncrFilePath { get; set; }
        public string OutputDecrFilePath { get; set; }
        public int BitsForDecryptCount { get; set; }

        //величины для обновления прогресс-баров
        public double ProgressEncrVal { get; private set; } = 0;
        public double ProgressDecrVal { get; private set; } = 0;
        public bool CryptBtnsIsEnabled;

        private delegate void delegateForInvoke();                //для лучшей работы программы процесс кодирования происходит в отдельном потоке, для него создается делегат

        //private readonly Dispatcher _dispatcher;

        public MainWindowModel()
        {
            //_dispatcher = Dispatcher.CurrentDispatcher;
        }

        private void ReadWaveFileInBytes(WaveFileReader WFR, out byte[] AudioFile)  //считывание wave файла в массив   
        {
            long fileLength = WFR.Length;
            AudioFile = new byte[fileLength];
            WFR.Read(AudioFile, 0, (int)fileLength);
            WFR.Close();
            WFR.Dispose();
        }

        public void Encrypt(string SoundFileSearchPath, string InputFileNameForHiding, string OutputFilePath, int Bits)
        {
            try
            {
                StreamReader SR = new StreamReader(InputFileNameForHiding);                    //создается новый объект streamReader на основе считываемого zip файла                
                WaveFileReader WFR = new WaveFileReader(SoundFileSearchPath);     //создается новый объект waveFileReader на основе считываемого wave файла
                ReadWaveFileInBytes(WFR, out byte[] AudioFile);
                long WFRLength = WFR.Length;

                //Считываем скрываемый zip файл в байтовый лист
                BinaryReader BR = new BinaryReader(SR.BaseStream);                              //создание нового объекта binaryReader для побайтового считывания zip файла из streamReader
                List<byte> BL = new List<byte>();
                byte cur;
                while (BR.BaseStream.Length != BL.Count)                                        //пока длины считываемого zip файла и листа байтов не сравняются
                {
                    cur = BR.ReadByte();
                    BL.Add(cur);                                                                //добавлять новый байт в лист
                }
                BR.Close();                                                                     //закрыть binaryreader
                SR.Close();                                                                     //Закрытие файла

                //Проверка того, что в контейнер уместиться весь файл
                if ((long)BL.Count > (WFRLength * Bits / 8))                                    //если размер zip файла превышает допустимое количество бит скрываемой информации (помещаемой в выходной WAV файл)
                {                                                                               //выдать сообщение "Недостаточный размер файла для кодирования"
                    throw new Exception("Недостаточный размер файла для кодирования " + Bits.ToString() + " битами : " + (WFRLength * Bits / 8) + " < " + Convert.ToUInt64(BL.Count));
                }
                BitArray InputZipFileBits = new BitArray(BL.ToArray());                         //Преобразование листа байтов из входного zip файла в массив битов
                BL = new List<byte>();
                BL.Clear();                                                                     //очищение листа байтов

                int icurW;                                                                      //Считанный байт из входного wav файла
                int IPosZ = 0;                                                                  //Текущая позиция во входном zip файле
                int Mask = (~0 << Bits);                                                        //Маска для обнуления начальных бит (сдвиг влево на указанное число Bits)

                // Запись в выходной файл
                WaveFileWriter WFW = new WaveFileWriter(OutputFilePath, WFR.WaveFormat);   //Создание нового файла формата wave (выходного)
                double OnePerc = (WFRLength / 100);
                double percCounter = 0;
                for (long k = 0; k < WFRLength; k++)                                            //до тех пор пока не обработается каждый байт входного wav файла:
                {
                    icurW = AudioFile[k];                                                       //считывание бита из массива байтов входного wav файла
                    int CurAnd = 0;
                    if (IPosZ < InputZipFileBits.Count)                                         //если текущая позиция во входном zip файле меньше числа бит zip файла
                    {
                        for (int i = 0; i < Bits; i++)                                          //цикл прогоняется в зависимости от указанного числа занимаемых младших бит
                        {
                            CurAnd = (CurAnd << 1) | (InputZipFileBits[IPosZ] ? 1 : 0);         //запись младших битов в CurAnd
                            IPosZ++;                                                            //изменение текущей позиции во входном zip файле
                        }
                    }
                    icurW = ((icurW & Mask) | CurAnd);                                          //очищение младших битов байта входного wav файла и замена битами из zip файла
                    WFW.WriteByte((byte)icurW);                                                 //запись полученных байтов в новый wav файл
                    
                    percCounter++;
                    if (percCounter >= OnePerc * 2)
                    {
                        percCounter = 0;
                        ProgressEncrVal = (k * 100) / WFRLength ;
                        RaisePropertyChanged(nameof(MainWindowViewModel.ProgrBarEncrVal));
                    }
                }
                WFW.Close();                                                                    //закрытие waveFileWriter
                WFW.Dispose();                                                                  //освобождение ресурсов от waveFileWriter
                InputZipFileBits = new BitArray(0);                                             //очищение массива байтов
            }
            catch { MessageBox.Show("Проверьте пути к файлам"); };
            ProgressEncrVal = 0;
            RaisePropertyChanged(nameof(MainWindowViewModel.ProgrBarEncrVal));
        }

        public void Decrypt(string EncrFileSearchPath, string OutputFilePath, int Bits)         //Извлечение информации
        {
            try
            {
                WaveFileReader WFR = new WaveFileReader(EncrFileSearchPath);                    //создается новый объект waveFileReader на основе считываемого wave файла     
                ReadWaveFileInBytes(WFR, out byte[] AudioFile);
                long WFRLength = WFR.Length;                                                    //определение длинны входного wave файла

                BitArray BA = new BitArray((int)(WFRLength * 8));                               //создаем массив битов для выходного zip файла, с расчетом, что весь файл может войти в результат
                BA.SetAll(false);                                                               //устанавливаем массив для выходного zip файла в нули

                int lastNonZero = 0;                                                            //Количество ненулевых бит в результате
                int bitCount = 0;                                                               //Количество битов

                double OnePerc = (WFRLength / 100);
                double byteCounter = 0;
                for (int i = 0; i < WFRLength; i++)                                             //проход по всем байтам листа BL
                {
                    byte CurByte = AudioFile[i];                                                //считываем текущий байт из листа байтов
                    byte CurMask = (byte)(1 << (Bits - 1));                                     //текущая маска, по которой будем выделять требуемый бит

                    // Считываем все биты маски
                    for (int j = 0; j < Bits; j++)
                    {
                        bool CurBit = (CurByte & CurMask) != 0;                                 //Получаем бит по маске
                        if (CurBit) lastNonZero = bitCount;                                     //если бит = 1 то lastNonZero присвоить индекс последнего ненулевого младшего бита
                        BA.Set(bitCount, CurBit);                                               //записываем бит (по индексу) в массив битов для выходного zip файла
                        bitCount++;                                                             //увеличиваем индекс 
                        CurMask = (byte)(CurMask >> 1);                                         //сдвигаем маску для более старшего бита (если нужно)
                    }

                    byteCounter++;
                    if (byteCounter >= OnePerc * 2)
                    {
                        byteCounter = 0;
                        ProgressDecrVal = (i * 50) / WFRLength;
                        RaisePropertyChanged(nameof(MainWindowViewModel.ProgrBarDecrVal));
                    }
                }
                int ByteMassLength = (lastNonZero / 8) + 6;                                     //размер выходного массива в байтах с запасом
                byte[] outputBytes = new byte[ByteMassLength];                                  //выходной массив
                byte[] ResBytes = new byte[(BA.Count / 8) + 6];                                 //массив с нулевыми битами
                BA.CopyTo(ResBytes, 0);                                                         //копируем в массив байт
                BA = new BitArray(0);                                                           //очищаем массив байтов
                //Запись в выходной массив полученных ненулевых битов
                for (int k = 0; k < ByteMassLength; k++)
                {
                    outputBytes[k] = ResBytes[k];
                }
                ResBytes = new byte[0];                                                         //очищаем массив
                ProgressDecrVal = 99;
                RaisePropertyChanged(nameof(MainWindowViewModel.ProgrBarDecrVal));
                StreamWriter SW = new StreamWriter(OutputFilePath);                             //создается новый объект streamWriter для записываемого zip файла
                BinaryWriter BW = new BinaryWriter(SW.BaseStream);                              //создание нового файла формата zip (выходного)
                BW.Write(outputBytes);                                                          //запись расшифрованных данных в выходной файл
                BW.Close();                                                                     //закрываем BinaryWriter
                BW.Dispose();                                                                   //освобождаем ресурсы от waveFileWriter
                outputBytes = new byte[0];                                                      //очищаем массив
                Thread.Sleep(100);                                                              //задержка для обновления таймера
            }
            catch { MessageBox.Show("Проверьте пути к файлам"); };
            ProgressDecrVal = 0;
            RaisePropertyChanged(nameof(MainWindowViewModel.ProgrBarDecrVal));
        }

        public void EncryptStart()                                       //Кнопка "Зашифровать"
        {
            CryptBtnsIsEnabled = false;
            RaisePropertyChanged(nameof(MainWindowViewModel.CryptBtnsIsEnabled));
            _ = new delegateForInvoke(delegate () {                                                 //запуск потока для выполнения метода Crypt без зависания основной программы
                Encrypt(InputSoundFilePath, InputFileForHidingPath, OutputEncrFilePath, Convert.ToInt16(BitsForCryptCount));
                CryptBtnsIsEnabled = true;
                RaisePropertyChanged(nameof(MainWindowViewModel.CryptBtnsIsEnabled));
                GC.Collect();                                                                       //принудительная "уборка мусора"
            }).BeginInvoke(null, null);
        }

        public void DecryptStart()                                       //Кнопка "Расшифровать"
        {
            CryptBtnsIsEnabled = false;
            RaisePropertyChanged(nameof(MainWindowViewModel.CryptBtnsIsEnabled));
            _ = new delegateForInvoke(delegate () {                                                 //запуск потока для выполнения метода Decrypt без зависания основной программы
                Decrypt(InputEncrFilePath, OutputDecrFilePath, Convert.ToInt16(BitsForDecryptCount));
                CryptBtnsIsEnabled = true;
                RaisePropertyChanged(nameof(MainWindowViewModel.CryptBtnsIsEnabled));
                GC.Collect();                                                                       //принудительная "уборка мусора"
            }).BeginInvoke(null, null);
        }
    }
}
