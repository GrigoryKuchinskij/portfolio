using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PalindromeCheckServer
{
    class Program
    {
        static readonly string DefaultPort = "8090";
        static int numbOfRequestsLimit = 128;
        static int defaultNumbOfRequests = 4;
        internal static bool IsRunAsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static void Main(string[] args)
        {
            Console.WindowWidth = 125;
            int consoleSizeH = Console.WindowHeight;
            int consoleSizeW = Console.WindowWidth;
            Console.SetBufferSize(consoleSizeW, consoleSizeH);

            Dictionary<string, string> consoleOutputs = new Dictionary<string, string>
            {
                {"os_not_supported","Текущая ос не поддерживается!" },
                {"launch_as_admin", "Запустите от имени администратора!"},
                {"on","вкл." },
                {"off","выкл." },
                {"server_launched_at_addr","Сервер запущен по адресу " },
                {"skip_punctuation","Пропуск знаков припинания в палиндроме: " },
                {"skip_similar", "Пропуск схожих символов и беззвучных в палиндроме: "},
                {"available_number_of_threads", "Доступно ${numbOfRequests} поток(-а/-ов) для обрабатываемых запросов"},
                {"keys_binding", @"[x]-Выход | [*число*]-количество потоков | [s]-Пропуск знаков | [e]-Приравнять ""Е"" к ""Ё"", ""И"" к ""Й"" и отбросить ""Ь"" и ""Ъ"""},
                {"command_not_recognized", "Команда не распознана"},
                {"server_not_launched", "Сервер не запущен!" },
                {"server_stopped","Сервер остановлен!" },
                {"load_header_text","Нагрузка:" },//        Состояние запроса:" },
                {"request_error" ,"Ошибка обработки запроса!"}
            };
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine(consoleOutputs["os_not_supported"]);             //если текущая ос не поддерживается
                Console.ReadKey();
                return;
            }
            if (!IsRunAsAdmin())
            {
                Console.WriteLine(consoleOutputs["launch_as_admin"]);
                Console.ReadKey();
                return;
            }

            HttpListener httpListener = new HttpListener();
            CheckPalindromeServer server = new CheckPalindromeServer(httpListener);
            server.MaxNumbOfRequests = defaultNumbOfRequests;
            string Ip = "+", Port = DefaultPort;
            bool exit = false;
            foreach (string arg in args)
            {
                if (arg.ToLower().StartsWith("addr-"))
                    Ip = arg.Substring(5);
                if (arg.ToLower().StartsWith("port-"))
                    Port = arg.Substring(5);
                if (arg.ToLower().StartsWith("maxreq-") && Convert.ToInt32(arg.Substring(7)) <= numbOfRequestsLimit)
                    server.MaxNumbOfRequests = Convert.ToInt32(arg.Substring(7));
                if (arg == "skip-punctuation") server.SkipPn = true;
                if (arg == "skip-similar") server.SkipSimilar = true;
            }
            string addr = @"http://" + Ip + ":" + Port + "/";
            server.Listener.Prefixes.Add(addr);

            string tempTextInput = "";
            bool inputIsComplete = true;

            server.Notify += (string message) => DrawServerTextLine(message);

            //запускаем север
            server.Listener.Start();
            Task.Run(() => server.StartClientFlow());
            while (!exit)
            {
                if (inputIsComplete)
                    DrawConsole();
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                char inputChar = keyInfo.KeyChar;
                ConsoleKey key = keyInfo.Key;
                switch (key)
                {
                    case ConsoleKey.X:
                        server.Listener.Stop();
                        server.Listener.Abort();
                        exit = true;
                        break;
                    case ConsoleKey.S:
                        server.SkipPn = !server.SkipPn;
                        break;
                    case ConsoleKey.E:
                        server.SkipSimilar = !server.SkipSimilar;
                        break;
                    case ConsoleKey.Backspace:
                        if (tempTextInput.Length != 0)
                            tempTextInput = tempTextInput.Remove(tempTextInput.Length - 1);
                        break;
                    default:
                        try
                        {
                            if (key != ConsoleKey.Enter)
                            {
                                tempTextInput += inputChar;
                                inputIsComplete = false;
                            }
                            else
                            {
                                server.MaxNumbOfRequests = Convert.ToInt32(tempTextInput);
                                tempTextInput = "";
                                inputIsComplete = true;
                            }
                        }
                        catch
                        {
                            Console.WriteLine(consoleOutputs["command_not_recognized"]);
                        };
                        break;
                }
            }

            void DrawConsole()
            {
                Console.Clear();
                Console.SetCursorPosition(consoleSizeW - 40, 0);
                Console.WriteLine(consoleOutputs["load_header_text"]);

                Console.SetCursorPosition(0, 0);
                Console.WriteLine(consoleOutputs["server_launched_at_addr"] + addr);

                if (server.SkipPn) Console.WriteLine(consoleOutputs["skip_punctuation"] + consoleOutputs["on"]);
                else Console.WriteLine(consoleOutputs["skip_punctuation"] + consoleOutputs["off"]);

                if (server.SkipSimilar) Console.WriteLine(consoleOutputs["skip_similar"] + consoleOutputs["on"]);
                else Console.WriteLine(consoleOutputs["skip_similar"] + consoleOutputs["off"]);
                                
                Console.WriteLine(consoleOutputs["available_number_of_threads"].Replace("${numbOfRequests}", $"{server.MaxNumbOfRequests}"));
                Console.WriteLine();
                Console.WriteLine(consoleOutputs["keys_binding"]);
                Console.WriteLine();
                Console.Write(">");
                //Console.SetCursorPosition(0, 7);
            }

            void DrawServerTextLine(string text)
            {
                if (inputIsComplete)
                {
                    inputIsComplete = false;
                    if (consoleOutputs.ContainsKey(text))
                    {
                        Console.WriteLine(consoleOutputs[text]);
                    }
                    else if (text.StartsWith("Load:"))
                    {
                        text = text.Replace("Load:", "  ");
                        Console.SetCursorPosition(consoleSizeW - text.Count() - 1, 0);
                        Console.WriteLine(text);//.Replace("#",new String(' ',15)));
                        Console.SetCursorPosition(1, 7);
                    }
                    inputIsComplete = true;
                }
            }
        }
    }

    class CheckPalindromeServer
    {
        public delegate void MessageHandler(string message);
        public event MessageHandler Notify;

        private bool skipPunctuation = false;
        private bool skipSimilar = false;
        private int maxNumbOfRequests = 2;
        private int currentNumbOfRequests = 0;
        private readonly int sleepDuration = 3;                         //имитация загрузки сервера
        private readonly string[,] xmlPage = {
            { @"<input name=""answer"" value=""",@"""><br>" },
            { @"<input name=""index"" value=""",@""">" }
        };

        public HttpListener Listener { get; private set; }
        public int MaxNumbOfRequests { get => maxNumbOfRequests; set => maxNumbOfRequests = value; }
        public bool SkipSimilar { get => skipSimilar; set => skipSimilar = value; }
        public bool SkipPn { get => skipPunctuation; set => skipPunctuation = value; }

        public CheckPalindromeServer(HttpListener listener) => Listener = listener;

        public void StartClientFlow()
        {
            if (Listener.IsListening == false)
            {
                Notify.Invoke("server_not_launched");
                return;
            };            
            while (Listener.IsListening)                                //пока сервер запущен
            {
                HttpListenerContext context = Listener.GetContext();    //ожидаем входящие запросы

                if (currentNumbOfRequests >= maxNumbOfRequests)             //если превышено количество обрабатываемых запросов
                {
                    ExtractValuesFromContext(ref context, out string reqWord, out string reqID);
                    SendResponseData(context.Response,
                        xmlPage[0, 0] + "wait=" + sleepDuration + xmlPage[0, 1]
                        + xmlPage[1, 0] + reqID + xmlPage[1, 1]);
                    //Notify.Invoke("Load:" + currentNumbOfRequests + @"/" + maxNumbOfRequests);// + "#rejected");
                    continue;
                }
                else
                    Task.Run(() => CheckPalindromeInContext(context));
            }
            currentNumbOfRequests = 0;                                      //при остановке сервера сбросить счетчик
            Notify.Invoke("server_stopped");
        }

        private void ExtractValuesFromContext(ref HttpListenerContext context, out string reqWord, out string reqID)
        {
            reqWord = "";
            reqID = "";
            string reqText = ReadRequestData(context.Request);
            Notify.Invoke("Load:" + currentNumbOfRequests + @"/" + maxNumbOfRequests);// + "#received ");
            string[] reqvals = reqText.Split('&');
            foreach (string reqval in reqvals)
            {
                if (reqval.StartsWith("word="))
                    reqWord = reqval.Replace("word=", "");
                if (reqval.StartsWith("index="))
                    reqID = reqval.Replace("index=", "");
            }
        }

        private void CheckPalindromeInContext(HttpListenerContext context)
        {
            ++currentNumbOfRequests;
            try
            {
                string reqWord;
                string reqID;
                char[] punctuation = { '.', ',', '!', '?', ';', ':', '"', '-' };
                char[] eqLetterE = { '.', ',', '!', '?', ';', ':', '"', '-' };
                ExtractValuesFromContext(ref context, out reqWord, out reqID);
                reqWord = reqWord.ToLower();
                if (skipPunctuation == true)
                {
                    reqWord = new string(reqWord.Select(s => (punctuation.Contains(s) ? ' ' : s)).ToArray());
                    reqWord = reqWord.Replace(" ", "");
                }
                if (skipSimilar == true)
                {
                    reqWord = reqWord.Replace("ё", "е");
                    reqWord = reqWord.Replace("й", "и");
                    reqWord = reqWord.Replace("ь", "");
                    reqWord = reqWord.Replace("ъ", "");
                }
                bool isPal = reqWord.IsPalindrome();
                //формируем ответ сервера:
                Thread.Sleep(sleepDuration * 1000);
                SendResponseData(context.Response,
                    xmlPage[0, 0] + isPal.ToString() + xmlPage[0, 1]
                    + xmlPage[1, 0] + reqID + xmlPage[1, 1]);
            }
            catch
            {
                Notify.Invoke("request_error");
                SendResponseData(context.Response,
                    xmlPage[0, 0] + "Error" + xmlPage[0, 1]
                    + xmlPage[1, 0] + "-1" + xmlPage[1, 1]);
            }
            --currentNumbOfRequests;
            Notify.Invoke("Load:" + currentNumbOfRequests + @"/" + maxNumbOfRequests);// + "#processed");
        }

        private string ReadRequestData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody) return "";
            using (Stream body = request.InputStream)
            {
                using (StreamReader reader = new StreamReader(body))
                {
                    string text = reader.ReadToEnd();
                    text = HttpUtility.UrlDecode(text, Encoding.UTF8);
                    return text;
                }
            }
        }
        private void SendResponseData(HttpListenerResponse response, string data)
        {
            string responseString = data;
            response.ContentType = "text/html; charset=UTF-8";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            using (Stream output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
            }
        }
    }

    public static class StringExtensions
    {
        public static bool IsPalindrome(this string text)
        {
            int Len = text.Count();
            if (Len <= 1) return false;
            int halfLen = (int)Math.Truncate(Len / (double)2);
            char[] directHalf = text.ToCharArray(0, halfLen),
                    reversedHalf = text.ToCharArray(Len - halfLen, halfLen);                //разделить текст на две равные части отбросив символ по середине (если он есть)
            Array.Reverse(reversedHalf);                                                    //перевернуть вторую половину
            return (new string(directHalf).Replace(new string(reversedHalf), "") == "");    //Удалить совпадающие строки первой и второй половины. 
                                                                                            //Если останется пустая строка, то это палиндром.
        }
    }
}
