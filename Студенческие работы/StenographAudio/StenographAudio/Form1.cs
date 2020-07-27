/*___________________________________________________________________________________________________________________________________________________*/
//С помощью данного стеганографического приложения можно внедрять/извлекать скрытое содержимое (архивы zip) из звуковых файлов (с расширением *.wav).//
//Программа использует библиотеку NAudio для Visual Studio.                                                                                          //
/*___________________________________________________________________________________________________________________________________________________*/
using System;
using System.Collections.Generic;
//using System.ComponentModel;
using System.Windows.Forms;
using NAudio.Wave;
using System.IO;
using System.Collections;
using System.Threading;

namespace StenographAudio
{
    public partial class Form1 : Form
    {
        long progressEncrypt = 0;
        long percentEncrypt = 1;        
        long WFRLengthForCrPrBar = 100;
        int progressDecrypt = 0;
        int WFRLengthForDcrPrBar = 100;
        bool timerIsTiking = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private delegate void delegateForInvoke();                //для лучшей работы программы процесс кодирования происходит в отдельном потоке, для него создается делегат

        public void Crypt(string SoundFileSearchPath, string InputFileNameForHiding, string OutputFilePath, int Bits)
        {
            try
            {
                var WFRwaveFileInput = new WaveFileReader(SoundFileSearchPath);                 //создается новый объект waveFileReader на основе считываемого wave файла
                var SR = new StreamReader(InputFileNameForHiding);                              //создается новый объект streamReader на основе считываемого zip файла                
                var WFRLength = WFRwaveFileInput.Length;                                        //определение длинны входного wave файла
                var WFW = new WaveFileWriter(OutputFilePath, WFRwaveFileInput.WaveFormat);      //Создание нового файла формата wave (выходного)
                percentEncrypt = (WFRLength / 100);
                WFRLengthForCrPrBar = WFRLength;

                //Считываем скрываемый zip файл в байтовый лист
                var BR = new BinaryReader(SR.BaseStream);                                       //создание нового объекта binaryReader для побайтового считывания zip файла из streamReader
                var BL = new List<byte>();
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
                                                                                                //выдать сообщение "Недостаточный размер файла для кодирования"
                    throw new Exception("Недостаточный размер файла для кодирования " + Bits.ToString() + " битами : " + (WFRLength * Bits / 8) + " < " + Convert.ToUInt64(BL.Count));

                var InputZipFileBits = new BitArray(BL.ToArray());                              //Преобразование листа байтов из входного zip файла в массив битов
                BL = new List<byte>();
                BL.Clear();                                                                     //очищение листа байтов

                
                int icurW;                                                                  //Считанный байт из входного wav файла
                int IPosZ = 0;                                                              //Текущая позиция во входном zip файле
                int Mask = (~0 << Bits);                                                    //Маска для обнуления начальных бит (сдвиг влево на указанное число Bits)
                
                // Запись в выходной файл
                byte[] AudioFile = new byte[WFRLength];                                         //создание массива байтов 
                WFRwaveFileInput.Read(AudioFile, 0, (int)WFRLength);                            //считывание исходного wave файла в массив                 
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
                    progressEncrypt = k;
                }
                WFW.Close();                                                                    //закрытие waveFileWriter
                WFW.Dispose();                                                                  //освобождение ресурсов от waveFileWriter
                WFRwaveFileInput.Close();                                                       //закрытие waveFileReader
                WFRwaveFileInput.Dispose();                                                     //освобождение ресурсов от waveFileReader
                InputZipFileBits = new BitArray(0);                                             //очищение массива байтов
            } catch { MessageBox.Show("Проверьте пути к файлам"); };
            progressEncrypt = 0;
            timerIsTiking = false;                                                              //указание таймеру остановится
        }

        public void Decrypt(string EncrFileSearchPath, string OutputFilePath, int Bits)         //Извлечение информации
        {
            try
            {
                var WFR = new WaveFileReader(EncrFileSearchPath);                               //создается новый объект waveFileReader на основе считываемого wave файла
                var SW = new StreamWriter(OutputFilePath);                                      //создается новый объект streamWriter для записываемого zip файла
                var WFRLength = WFR.Length;                                                     //определение длинны входного wave файла
                WFRLengthForDcrPrBar = 99;

                byte[] AudioBytes = new byte[WFRLength];                                        //создаем массив байтов 
                WFR.Read(AudioBytes, 0, (int)WFRLength);                                        //cчитываем wav файл в массив байтов
                WFR.Close();                                                                    //закрываем waveFileReader
                WFR.Dispose();                                                                  //освобождаем ресурсы от waveFileReader
                var BL = new List<byte>(AudioBytes);                                            //копируем массив байтов в лист BL
                AudioBytes = new byte[0];                                                       //очищаем массив

                var BA = new BitArray(BL.Count * 8);                                            //создаем массив битов для выходного zip файла, с расчетом, что весь файл может войти в результат
                BA.SetAll(false);                                                               //устанавливаем массив для выходного zip файла в нули

                int lastNonZero = 0;                                                        //Количество ненулевых бит в результате
                int bitCount = 0;                                                           //Количество битов
                progressDecrypt = WFRLengthForDcrPrBar / 3;                                     

                for (int i = 0; i < BL.Count; i++)                                              //проход по всем байтам листа BL
                {
                    byte CurByte = BL[i];                                                       //считываем текущий байт из листа байтов
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
                }
                BL = new List<byte>();
                BL.Clear();                                                                     //очищаем лист байтов
                byte[] outputBytes = new byte[lastNonZero / 8 + 6];                             //выходной массив
                byte[] ResBytes = new byte[BA.Count / 8 + 6];                                   //массив с нулевыми битами
                BA.CopyTo(ResBytes, 0);                                                         //копируем в массив байт
                BA = new BitArray(0);                                                           //очищаем массив байтов
                progressDecrypt = WFRLengthForDcrPrBar / 2;

                //Запись в выходной массив полученных ненулевых битов
                for (int k = 0; k < lastNonZero / 8 + 6; k++)
                {
                    outputBytes[k] = ResBytes[k];
                }
                ResBytes = new byte[0];                                                         //очищаем массив
                progressDecrypt = WFRLengthForDcrPrBar;
            
                var BW = new BinaryWriter(SW.BaseStream);                                       //создание нового файла формата zip (выходного)
                BW.Write(outputBytes);                                                          //запись расшифрованных данных в выходной файл
                BW.Close();                                                                     //закрываем BinaryWriter
                BW.Dispose();                                                                   //освобождаем ресурсы от waveFileWriter
                outputBytes = new byte[0];                                                      //очищаем массив
                Thread.Sleep(100);                                                              //задержка для обновления таймера
            } catch { MessageBox.Show("Проверьте пути к файлам"); };
            progressDecrypt = 0;
            timerIsTiking = false;                                                              //указание таймеру остановится
        }

       
        //private void Form1_HelpButtonClicked(object sender, CancelEventArgs e)                          //кнопка открытия окна "О авторе"
        //{
        //    e.Cancel = true; // кнопка больше не переводит курсор в режим помощи на лкм
        //    var form2 = new Form2();
        //    form2.ShowDialog();
        //}

        private void SoundFileSearchBtn_Click(object sender, EventArgs e)                               //кнопка обзора пути к "Исходному звуковому файлу"
        {
            openFileDialog.Filter = "wave files (*.wav)|*.wav|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SoundFileSearchTxBx.Text = openFileDialog.FileName;
            }
        }

        private void FileForHidingSearchBtn_Click(object sender, EventArgs e)                           //кнопка обзора пути к "Скрываемому файлу"
        {
            openFileDialog.Filter = "ZIP files (*.zip)|*.zip|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileForHidingSearchTxBx.Text = openFileDialog.FileName;
            }
        }

        private void DestForEncrFileBtn_Click(object sender, EventArgs e)                               //кнопка обзора пути к сохраняемому "Выходному (закодированному) аудио файлу"
        {
            saveFileDialog.Filter = "wave files (*.wav)|*.wav|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                DestForEncrFileTxBx.Text = saveFileDialog.FileName;
            }
        }

        private void EncrFileSearchBtn_Click(object sender, EventArgs e)                                //кнопка обзора пути к "Исходному (закодированному) звуковому файлу"
        {
            openFileDialog.Filter = "wave files (*.wav)|*.wav|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                EncrFileSearchTxBx.Text = openFileDialog.FileName;
            }
        }

        private void DestForDecrFileBtn_Click(object sender, EventArgs e)                               //кнопка обзора пути к сохраняемому "Выходному (раскодированному) файлу"
        {
            saveFileDialog.Filter = "ZIP files (*.zip)|*.zip|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                DestForDecrFileTxBx.Text = saveFileDialog.FileName;
            }
        }

        private void EncryptBtn_Click(object sender, EventArgs e)                                       //Кнопка "Зашифровать"
        {
            DecryptBtn.Enabled = false; EncryptBtn.Enabled = false;
            timerIsTiking = true;                                                                       //указание таймеру продолжать работать
            timer.Start();                                                                              //запуск таймера
            new delegateForInvoke(delegate () {                                                         //запуск потока для выполнения метода Crypt без зависания основной программы
                    Crypt(SoundFileSearchTxBx.Text, FileForHidingSearchTxBx.Text, DestForEncrFileTxBx.Text, Convert.ToInt16(TxBxEncryptBit.Text)); 
                    GC.Collect();                                                                       //принудительная "уборка мусора"
            }).BeginInvoke(null, null);
        }

        private void DecryptBtn_Click(object sender, EventArgs e)                                       //Кнопка "Расшифровать"
        {            
            DecryptBtn.Enabled = false; EncryptBtn.Enabled = false;
            timerIsTiking = true;                                                                       //указание таймеру продолжать работать
            timer.Start();                                                                              //запуск таймера
            new delegateForInvoke(delegate () {                                                         //запуск потока для выполнения метода Decrypt без зависания основной программы
                    Decrypt(EncrFileSearchTxBx.Text, DestForDecrFileTxBx.Text, Convert.ToInt16(TxBxDecryptBit.Text)); 
                    GC.Collect();                                                                       //принудительная "уборка мусора"
            }).BeginInvoke(null, null);
        }

        private void Timer_Tick_1(object sender, EventArgs e)                                           //таймер для обновления ProgressBar (позволяет не замедлять процесс шифровки / дешифровки)
        {
            try
            {
                progrBarEncr.Maximum = (int)(WFRLengthForCrPrBar / percentEncrypt);
                progrBarDecr.Maximum = WFRLengthForDcrPrBar;
                progrBarEncr.Value = (int)(progressEncrypt / percentEncrypt);
                progrBarDecr.Value = progressDecrypt;
            }
            catch
            {
                progrBarEncr.Maximum = 100;
                progrBarDecr.Maximum = 100;
                progrBarEncr.Value = 0;
                progrBarDecr.Value = 0;
            };
            if (timerIsTiking == false) { DecryptBtn.Enabled = true; EncryptBtn.Enabled = true; timer.Stop(); }
        }
    }
}
