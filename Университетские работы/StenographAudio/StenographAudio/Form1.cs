using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using NAudio.Wave;
using System.IO;
using System.Collections;
using System.Threading;

namespace StenographAudio
{
    public partial class Form1 : Form
    {
        int progressEncrypt=0;
        int progressDecrypt=0;
        int WFRLengthForCrPrBar = 100;
        int WFRLengthForDcrPrBar = 100;
        bool timerIsTiking = false;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private delegate void delegateForInvoke();              //для лучшей работы программы процесс кодирования происходит в отдельном потоке

        public UInt64 GetMaxSizeForCrypt(int WFRLength, int Bits) //Расчет количества бит скрываемой информации помещаемой в выбранный WAV файл
        {
            return Convert.ToUInt64((WFRLength * Bits / 8));
        }

        public void Crypt(string SoundFileSearchPath, string InputFileNameForHiding, string OutputFilePath, int Bits)
        {
            try
            {
                var WFRwaveFileInput = new WaveFileReader(SoundFileSearchPath);                                     //получение информации о входных файлах
                var SR = new StreamReader(InputFileNameForHiding);
                var WFW = new WaveFileWriter(OutputFilePath, WFRwaveFileInput.WaveFormat); //Создание нового файла формата wave (выходного)
                var WFRLength = (int)WFRwaveFileInput.Length;
                WFRLengthForCrPrBar = WFRLength;

                var BR = new BinaryReader(SR.BaseStream);//Считываем скрываемый файл в байтовый лист
                var BL = new List<byte>();
                byte cur;
                while (BR.BaseStream.Length != BL.Count)
                {
                    cur = BR.ReadByte();
                    BL.Add(cur);
                }
                BR.Close();
                SR.Close();
                //Закрытие файла
                //Проверка того, что в контейнер уместиться весь файл
                if (GetMaxSizeForCrypt(WFRLength, Bits) < Convert.ToUInt64(BL.Count)) throw new Exception("Не достаточный размер файла для кодирования " + Bits.ToString() + " битами : " + GetMaxSizeForCrypt(WFRLength, Bits) + " < " + Convert.ToUInt64(BL.Count));
                var InputFileBits = new BitArray(BL.ToArray());                                        //Преобразуем входной файл в массив битов
                BL = new List<byte>();
                BL.Clear();


                int icur;                                                                                   // Считанный бит из входного файла
                int IPos = 0;                                                                               // Текущая позиция во входном файле
                int Mask = (~0 << Bits);                                                                    // Маска для обнуления начальных бит
                                                                                                            // Запись в выходной файл
                byte[] AudioFile = new byte[WFRLength];
                WFRwaveFileInput.Read(AudioFile, 0, WFRLength);
                for (int k = 0; k < WFRLength; k++)
                {
                    icur = AudioFile[k];                                                                    // определяем нужное количество бит
                    int CurAnd = 0;
                    if (IPos < InputFileBits.Count)
                    {
                        for (int i = 0; i < Bits; i++)
                        {
                            CurAnd = (CurAnd << 1) | (InputFileBits[IPos] ? 1 : 0);
                            IPos++;
                        }
                    }
                    icur = (icur & Mask | CurAnd);
                    WFW.WriteByte((byte)icur);
                    progressEncrypt = k;
                }
                WFW.Close();
                WFW.Dispose();
                WFRwaveFileInput.Close();
                WFRwaveFileInput.Dispose();
                InputFileBits = new BitArray(0);
            } catch { MessageBox.Show("Проверьте пути к файлам"); };
            progressEncrypt = 0;
            timerIsTiking = false;
        }

        public void Decrypt(string EncrFileSearchPath, string OutputFilePath, int Bits)              //Извлечение информации
        {
            try
            {
                var WFR = new WaveFileReader(EncrFileSearchPath);                                      //получение информации из входного аудио-файла
                var SW = new StreamWriter(OutputFilePath);
                var WFRLength = (int)WFR.Length;
                WFRLengthForDcrPrBar = WFRLength;
                byte[] AudioBytes = new byte[WFRLength];                                         
                WFR.Read(AudioBytes, 0, WFRLength);                                                 // Считываем звуковой файл в массив байтов
                WFR.Close();
                WFR.Dispose();
                var BL = new List<byte>(AudioBytes);                                                        // Создание листа битов, с расчетом, что весь файл может войти в результат
                AudioBytes = new byte[0];
                var BA = new BitArray(BL.Count * 8);

                BA.SetAll(false);                                                                           // Устанавливаем в нули
                int LastNonZero = 0;                                                                        // Количество ненулевых бит в результате
                int BitCount = 0;                                                                           // Количество битов
                progressDecrypt = WFRLengthForDcrPrBar / 3;
                //Проход по всем байтам файла
                for (int i = 0; i < BL.Count; i++)
                {
                    byte CurByte = BL[i];                                                                   // Текущий байт
                    byte CurMask = (byte)(1 << (Bits - 1));                                                 // Текущая маска, по которой будем слева направо выделять требуемый бит
                    for (int j = 0; j < Bits; j++)                                                          // Считываем все биты маски
                    {
                        bool CurBit = (CurByte & CurMask) != 0;                                             // Получаем бит
                        if (CurBit) LastNonZero = BitCount;
                        BA.Set(BitCount, CurBit);                                                           // Устанавливаем бит
                        BitCount++;                                                                         // Увеличиваем количество ненулевых
                        CurMask = (byte)(CurMask >> 1);                                                     // Сдвигаем маску
                    }
                }
                BL = new List<byte>();
                BL.Clear();
                byte[] outputBytes = new byte[LastNonZero / 8 + 6];                                         //выходной массив
                byte[] ResBytes = new byte[BA.Count / 8 + 6];                                               //массив с нулевыми битами
                BA.CopyTo(ResBytes, 0);                                                                     // Копируем в массив байт
                BA = new BitArray(0);
                progressDecrypt = WFRLengthForDcrPrBar / 2;
                //Запись в выходной массив полученных ненулевых битов
                for (int k = 0; k < LastNonZero / 8 + 6; k++)
                {
                    outputBytes[k] = ResBytes[k];
                }
                progressDecrypt = WFRLengthForDcrPrBar-1;
            
                var BW = new BinaryWriter(SW.BaseStream);
                BW.Write(outputBytes);
                BW.Close();
                BW.Dispose();            
                outputBytes = new byte[0];
                Thread.Sleep(100);
            } catch { MessageBox.Show("Проверьте пути к файлам"); };
            progressDecrypt = 0;
            timerIsTiking = false;            
        }

       
        private void Form1_HelpButtonClicked(object sender, CancelEventArgs e)                          //кнопка открытия окна "О авторе"
        {
            var form2 = new Form2();
            form2.ShowDialog();
        }

        private void SoundFileSearchBtn_Click(object sender, EventArgs e)                               //кнопка обзора пути к "Исходному звуковому файлу"
        {
            openFileDialog1.Filter = "wave files (*.wav)|*.wav|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SoundFileSearchTxBx.Text = openFileDialog1.FileName;
            }
        }

        private void FileForHidingSearchBtn_Click(object sender, EventArgs e)                           //кнопка обзора пути к "Скрываемому файлу"
        {
            openFileDialog1.Filter = "ZIP files (*.zip)|*.zip|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileForHidingSearchTxBx.Text = openFileDialog1.FileName;
            }
        }

        private void DestForEncrFileBtn_Click(object sender, EventArgs e)                               //кнопка обзора пути к сохраняемому "Выходному (закодированному) аудио файлу"
        {
            saveFileDialog1.Filter = "wave files (*.wav)|*.wav|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DestForEncrFileTxBx.Text = saveFileDialog1.FileName;
            }
        }

        private void EncrFileSearchBtn_Click(object sender, EventArgs e)                                //кнопка обзора пути к "Исходному (закодированному) звуковому файлу"
        {
            openFileDialog1.Filter = "wave files (*.wav)|*.wav|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                EncrFileSearchTxBx.Text = openFileDialog1.FileName;
            }
        }

        private void DestForDecrFileBtn_Click(object sender, EventArgs e)                               //кнопка обзора пути к сохраняемому "Выходному (раскодированному) файлу"
        {
            saveFileDialog1.Filter = "ZIP files (*.zip)|*.zip|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DestForDecrFileTxBx.Text = saveFileDialog1.FileName;
            }
        }

        private void EncryptBtn_Click(object sender, EventArgs e)                                       //Кнопка "Зашифровать"
        {
                DecryptBtn.Enabled = false; EncryptBtn.Enabled = false;
                timerIsTiking = true;
                timer1.Start();
                new delegateForInvoke(delegate () { Crypt(SoundFileSearchTxBx.Text, FileForHidingSearchTxBx.Text, DestForEncrFileTxBx.Text, Convert.ToInt16(TxBxEncryptBit.Text)); GC.Collect(); }).BeginInvoke(null, null);
        }

        private void DecryptBtn_Click(object sender, EventArgs e)                                       //Кнопка "Расшифровать"
        {            
                DecryptBtn.Enabled = false; EncryptBtn.Enabled = false;
                timerIsTiking = true;
                timer1.Start();
                new delegateForInvoke(delegate () { Decrypt(EncrFileSearchTxBx.Text, DestForDecrFileTxBx.Text, Convert.ToInt16(TxBxDecryptBit.Text)); GC.Collect(); }).BeginInvoke(null, null);
        }

        private void Timer1_Tick_1(object sender, EventArgs e)
        {
            try
            {
                progrBarEncr.Maximum = WFRLengthForCrPrBar;
                progrBarDecr.Maximum = WFRLengthForDcrPrBar;
                progrBarEncr.Value = progressEncrypt;
                progrBarDecr.Value = progressDecrypt;
            }
            catch
            {
                progrBarEncr.Maximum = 100;
                progrBarDecr.Maximum = 100;
                progrBarEncr.Value = 0;
                progrBarDecr.Value = 0;
            };
            if (timerIsTiking == false) { timer1.Stop(); DecryptBtn.Enabled = true; EncryptBtn.Enabled = true; }
        }
    }
}
