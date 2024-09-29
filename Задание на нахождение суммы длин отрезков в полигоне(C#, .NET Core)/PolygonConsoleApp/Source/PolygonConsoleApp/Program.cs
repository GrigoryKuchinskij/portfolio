using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static PolygonConsoleApp.GeometricObjects;

namespace PolygonConsoleApp
{
    class Program
    {
        readonly static string shortHelpMessage = "\r\n Приложение рассчитывает сумму длин частей отрезков, находящихся внутри многоугольника. " +
                "\r\n Координаты отрезков и углов многоугольника задаются в файлах \"segments.csv\" и \"polygonPoints.csv\", находящихся в папке с программой." +
                "\r\n Вы также можете самостоятельно задать пути к файлам передав их в качестве аргументов. " +
                "\r\n Пример:> ./PolygonConsoleApp.exe s=\"C:\\Temp\\Координаты_отрезков.csv\" p=\"C:\\Temp\\Координаты_углов_многоугольника.csv\"" +
                "\r\n Приложение может работать с выпуклыми и невыпуклыми многоугольниками без самопересечений." +
                "\r\n \r\n [ Enter ] для расчета | [ H ] для дополнительной информации | [ X ] для выхода\r\n>>";
        readonly static string extendedHelpMessage = "\r\n Координаты отрезков задаются построчно (по ум. в файле \"segments.csv\")." +
            "\r\n Формат: {X начала отрезка};{Y начала отрезка};{X конца отрезка};{Y конца отрезка} " +
            "\r\n Пример строки: 62,076745;79,945621;64,819002;84,763976" +
            "\r\n Координаты углов многоугольника задаются построчно (по ум. в файле \"polygonPoints.csv\"). Формат: {X точки};{Y точки}" +
            "\r\n Пример строки: 62,76284217;79,61893362" +
            "\r\n Углы многоугольника описываются последовательно, вдоль его границы.\r\n>>";

        static void Main(string[] args)
        {
            string polygonPointsFilePath = @"polygonPoints.csv";
            string segmentsFilePath = @"segments.csv";

            foreach (string arg in args)
            {
                switch (arg.Substring(0, 2))
                {
                    case "s=":
                        segmentsFilePath = arg.Substring(2).Trim(new char[] { ' ', '"' });
                        break;
                    case "p=":
                        polygonPointsFilePath = arg.Substring(2).Trim(new char[] { ' ', '"' });
                        break;
                }
            }

            Console.Write(shortHelpMessage);

            while (true)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.H:
                        Console.Clear();
                        Console.Write(shortHelpMessage + extendedHelpMessage);
                        break;
                    case ConsoleKey.X:
                        Environment.Exit(0);
                        break;
                    case ConsoleKey.Enter:
                        try
                        {
                            Point[] polyPoints = ReadPoints(polygonPointsFilePath);
                            Segment[] segments = ReadSegments(segmentsFilePath);                            
                            double summ = GeometricIntersections.CalcSegmentsLengthSummInPoly(polyPoints, segments);
                            Console.Clear();
                            Console.WriteLine(shortHelpMessage + summ);
                        }
                        catch
                        {
                            Console.WriteLine("Ошибка распознавания *.csv файлов!" +
                                "\r\n Нажмите любую клавишу для выхода из приложения. ");
                            Console.ReadKey();
                            Environment.Exit(-1);
                        }
                        GC.Collect();
                        break;
                    default:
                        Console.Clear();
                        Console.Write(shortHelpMessage);
                        break;
                }
            }
        }

        private static Point[] ReadPoints(string pointsFilePath) 
        {
            List<string[]> pointsList = ReadCSVFile(pointsFilePath);
            Point[] points = GeometricConversions.ConvertToPoint(pointsList);
            return points;
        }

        private static Segment[] ReadSegments(string segmentsFilePath)
        {
            List<string[]> segmentsList = ReadCSVFile(segmentsFilePath);
            Segment[] segments = GeometricConversions.ConvertToSegment(segmentsList);
            return segments;
        }

        private static List<string[]> ReadCSVFile(string pathToCsvFile)
        {
            using (StreamReader reader = new StreamReader(pathToCsvFile, Encoding.Default))
            {
                _ = reader.ReadLine();  //Пропуск заголовка
                List<string[]> CSVList = new List<string[]>();
                while (!reader.EndOfStream)
                {
                    string[] rowValues = reader.ReadLine().Split(';');
                    CSVList.Add(rowValues);
                }
                return CSVList;
            }
        }
    }
}
