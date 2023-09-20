using MJpegStreamViewerWPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MJpegStreamViewerWPF
{
    public class MJPEGStream
    {
        private int chunkSize;
        private readonly int maxBufferSize;
        private readonly byte[] JpegFirstBytes = new byte[] { 0xff, 0xd8 };    //первые байты Jpeg
        private readonly byte[] JpegLastBytes = new byte[] { 0xff, 0xd9 };    //последние байты Jpeg
        private HttpWebRequest request;
        private HttpWebResponse response;
        private BinaryReader? binaryReader;
        private Stream stream;
        private Thread? streamThread;

        public event EventHandler<FrameReadyEventArgs> FrameReady;
        public event EventHandler<ErrorEventArgs> Error;

        public MJPEGStream(int chunkSize = 4096, int maxBufferSize = 1024 * 1024 * 10)
        {
            if (chunkSize >= maxBufferSize)
            {
                throw new ArgumentException("chunkSize too big", nameof(chunkSize));
            }
            this.chunkSize = chunkSize;
            this.maxBufferSize = maxBufferSize;
        }

        public void ErrorEventTrigger(EventHandler<ErrorEventArgs> ErrorHandler, Exception ex)
        {
            ErrorHandler?.Invoke(this, new ErrorEventArgs
            {
                Message = ex.Message,
                ErrorCode = ex.GetHashCode(),
            });
        }

        public class FrameReadyEventArgs : EventArgs
        {
            public byte[] FrameBuffer;
        }

        public sealed class ErrorEventArgs : EventArgs
        {
            public string Message { get; set; }
            public int ErrorCode { get; set; }
        }

        public bool IsActive { get; private set; } = false;

        public void Start(string streamURI)
        {

            if (!IsActive && (streamThread is null || !streamThread.IsAlive))
            {
                IsActive = true;
                streamThread = new Thread(() =>
                    {
                        request = (HttpWebRequest)HttpWebRequest.Create(new Uri(streamURI));
                        request.BeginGetResponse(OnGetResponse, request);
                    });
                streamThread.Start();
            }
        }

        public void Stop() => IsActive = false;

        private void OnGetResponse(IAsyncResult asyncResult)                                    //метод запускаемый асинхронно и принимающий Http-ответы
        {
            try
            {
                if (request is null) throw new ArgumentNullException(nameof(request));
                response = (HttpWebResponse)request.EndGetResponse(asyncResult);
                stream = response.GetResponseStream();
                ReadMJpegStream(stream);
                response?.Close();
            }
            catch (Exception ex)
            {
                ErrorEventTrigger(this.Error, ex);
            }
            request?.Abort();
            GC.Collect();                                                                       //принудительная "уборка мусора"
        }

        private void ReadMJpegStream(Stream stream)                                         //метод распознающий поток MJPEG видеоданных
        {
            binaryReader = new BinaryReader(stream);
            int currentPosition = 0;
            byte[] buffer = new byte[chunkSize];                                                //буфер для считываемых кадров
            byte[] currentChunk;                                                                //чанк для считывания потока в буфер
            byte[] FrameBuffer;                                                                 //буфер для кадра
            int collectorCounter = 0;

            while (IsActive)
            {
                try
                {
                    currentChunk = binaryReader.ReadBytes(chunkSize);                               //считывание полученных байт в чанк
                    if (currentChunk.Length < chunkSize) { throw new ObjectDisposedException(nameof(binaryReader)); }
                }
                catch { Stop(); break; }
                if (buffer.Length < currentPosition + currentChunk.Length)
                {
                    if (buffer.Length < maxBufferSize)
                    {
                        Array.Resize(ref buffer, currentPosition + currentChunk.Length);        //изменение размера буфера для добавления нового чанка
                    }
                    else                                                                        //сброс буфера если его размер больше 10MB
                    {
                        currentPosition = 0;
                        Array.Resize(ref buffer, chunkSize);
                    }
                }
                Array.Copy(currentChunk, 0, buffer, currentPosition, currentChunk.Length);      //копирование текущего чанка в буфер начиная с текущей позиции
                currentPosition += currentChunk.Length;                                         //увеличение текущей позиции в буфере на размер чанка
                int startOfImage = buffer.FindPatternPosition(JpegFirstBytes, 0, currentPosition);  //поиск позиции начальных байт JPEG в буфере
                if (startOfImage != -1)                                                         //если нахождение прошло успешно
                {
                    int endOfImage = buffer.FindPatternPosition(JpegLastBytes, lowerLimit: startOfImage, currentPosition);  //нахождение позиции последних битов JPEG кадра в буфере
                    if (endOfImage != -1)                                                       //если нахождение прошло успешно
                    {
                        if (endOfImage > startOfImage)                                          //если начало и конец кадра расчитаны верно
                        {
                            int endOfCurrentImage = endOfImage + JpegLastBytes.Length;          //расчет размера текущего кадра
                            FrameBuffer = new byte[endOfCurrentImage - startOfImage];           //пересоздание массива необходимого, для кадра, размера
                            Array.Copy(buffer, startOfImage, FrameBuffer, 0, FrameBuffer.Length);   //копирование данных из буфера в только-что созданный массив
                            this.FrameReady?.Invoke(this, new FrameReadyEventArgs               //вызов метода запуска события обновления картинки imOfStream
                            {
                                FrameBuffer = FrameBuffer,
                            });
                            int remainingSize = currentPosition - endOfCurrentImage;            //расчет размера буфера для оставшихся кадров
                            Array.Copy(buffer, endOfCurrentImage, buffer, 0, remainingSize);    //копирование байтов буфера, начиная с текущей позиции, в начало буфера
                            currentPosition = remainingSize;                                    //обновление значения теущей позиции в измененном буфере
                            chunkSize = Convert.ToInt32(FrameBuffer.Length * 0.5d);             //пересчет размера чанка чтобы избежать множественных считываний
                        }
                    }
                }
                if (collectorCounter >= 2000)
                { GC.Collect(); collectorCounter = 0; }                                         //принудительная "уборка мусора"
                else
                    collectorCounter++;
            }
            binaryReader?.Dispose();
            binaryReader?.Close();
            binaryReader = null;
            stream?.Dispose();
            stream?.Close();
        }
    }

    internal static class ByteArrayExtensions
    {
        public static int FindPatternPosition(this byte[] buff, byte[] pattern, int lowerLimit = 0, int upperLimit = int.MaxValue)     //метод-расширение для поиска пограничных байт JPEG в буфере начиная с определенной позиции
        {
            int patter_match_counter = 0;
            int i = 0;
            for (i = lowerLimit; i < buff.Length && patter_match_counter < pattern.Length && i < upperLimit; i++)
            {
                if (buff[i] == pattern[patter_match_counter])   //если фрагмент буфера совпадает с пограничными байтами JPEG
                {
                    patter_match_counter++;
                }
                else
                {
                    patter_match_counter = 0;
                }
            }       //по достижении patter_match_counter размера длинны пограничных байт JPEG и изменении счетчика i ...

            if (patter_match_counter == pattern.Length)
            {
                return i - pattern.Length;                      //возвратить значение позиции буфера с совпадающими байтами
            }
            else
            {
                return -1;                                      //возвратить значение -1 при неудачном поиске
            }
        }
    }
}
