using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static PolygonConsoleApp.GeometricObjects;

namespace PolygonConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string polygonPointsFilePath = @"polygonPoints.csv";
            string segmentsFilePath = @"segments.csv";
            bool noComments = false;                     //Выдать результат сразу
            string startMessage = "\r\n Приложение рассчитывает сумму длин частей отрезков, находящихся внутри многоугольника. " +
                "\r\n Координаты отрезков и углов многоугольника задаются в файлах \"segments.csv\" и \"polygonPoints.csv\", находящихся в папке с программой." +
                "\r\n Вы также можете самостоятельно задать пути к файлам передав их в качестве аргументов. " +
                "\r\n Пример:> ./PolygonConsoleApp.exe s=\"C:\\Temp\\Координаты_отрезков.csv\" p=\"C:\\Temp\\Координаты_углов_многоугольника.csv\"" +
                "\r\n Приложение может работать с выпуклыми и невыпуклыми многоугольниками без самопересечений."+
                "\r\n \r\n [ Enter ] для расчета | [ H ] для дополнительной информации | [ X ] для выхода\r\n>>";
            string helpMessage = "\r\n Координаты отрезков задаются построчно (по ум. в файле \"segments.csv\")." +
                "\r\n Формат: {X начала отрезка};{Y начала отрезка};{X конца отрезка};{Y конца отрезка} " +
                "\r\n Пример строки: 62,076745;79,945621;64,819002;84,763976" +
                "\r\n Координаты углов многоугольника задаются построчно (по ум. в файле \"polygonPoints.csv\"). Формат: {X точки};{Y точки}" +
                "\r\n Пример строки: 62,76284217;79,61893362" +
                "\r\n Углы многоугольника описываются последовательно, вдоль его границы.\r\n>>";

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
                    case "-c":                      //Аргумент для запуска программы без доп. комментариев
                        noComments = true;
                        break;
                }
            }

            if (!noComments) Console.Write(startMessage);

            ConsoleKey key = ConsoleKey.Enter;

            while (true)
            {
                if (!noComments)
                {
                    key = Console.ReadKey().Key;
                }
                switch (key)
                {
                    case ConsoleKey.H:
                        Console.Clear();
                        Console.Write(startMessage + helpMessage);
                        break;
                    case ConsoleKey.X:
                        Environment.Exit(0);
                        break;
                    case ConsoleKey.Enter:
                        try
                        {
                            List<string[]> polygonPointsList = ReadCSVFile(polygonPointsFilePath);
                            List<string[]> segmentsList = ReadCSVFile(segmentsFilePath);
                            Point[] polyPoints = ConvertToPoint(polygonPointsList);
                            Segment[] lines = ConvertToSegment(segmentsList);
                            double summ = CalcGeometricIntersections.CalcSegmentsSummInPoly(polyPoints, lines);
                            Console.Clear();
                            Console.WriteLine(startMessage + summ);
                            if (noComments)
                                Environment.Exit(0);
                        }
                        catch
                        {
                            if (noComments)
                            { Console.WriteLine("ER_READCSV"); Environment.Exit(-1); }
                            else
                            {
                                Console.WriteLine("Ошибка распознавания *.csv файлов!" +
                              "\r\n Нажмите любую клавишу для выхода из приложения. "); Console.ReadKey(); Environment.Exit(-1);
                            }
                        }
                        GC.Collect();
                        break;
                    default:
                        Console.Clear();
                        Console.Write(startMessage);
                        break;
                }
            }
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

        private static Point[] ConvertToPoint(List<string[]> pointsList)
        {
            Point[] pointsArray = new Point[pointsList.Count];
            for (int i = 0; i < pointsList.Count; i++)
            {
                if (pointsList[i].Length == 2)
                {
                    pointsArray[i] = new Point { X = Convert.ToDouble(pointsList[i][0]), Y = Convert.ToDouble(pointsList[i][1]) };
                }
            }
            return pointsArray;
        }

        private static Segment[] ConvertToSegment(List<string[]> segmentsStringsList)
        {
            List<Segment> segmentsList = new List<Segment>();
            for (int i = 0; i < segmentsStringsList.Count; i++)
            {
                if (segmentsStringsList[i].Length == 4)
                {
                    double XBegSeg = Convert.ToDouble((string)segmentsStringsList[i][0]);
                    double YBegSeg = Convert.ToDouble((string)segmentsStringsList[i][1]);
                    double XEndSeg = Convert.ToDouble((string)segmentsStringsList[i][2]);
                    double YEndSeg = Convert.ToDouble((string)segmentsStringsList[i][3]);
                    if (XBegSeg > XEndSeg)                                                                  //если X координата первой точки больше X второй 
                    {
                        (XBegSeg, XEndSeg) = (XEndSeg, XBegSeg);
                        (YBegSeg, YEndSeg) = (YEndSeg, YBegSeg);
                    }   //обмен координат точек отрезка
                    else if (XBegSeg == XEndSeg && YBegSeg > YEndSeg)                                       //если X координаты равны, а Y координата первой точки больше Y второй 
                    {
                        (YBegSeg, YEndSeg) = (YEndSeg, YBegSeg);
                    }
                    segmentsList.Add(new Segment
                    {
                        P1 = new Point { X = XBegSeg, Y = YBegSeg },
                        P2 = new Point { X = XEndSeg, Y = YEndSeg }
                    });
                }            
            }
            return segmentsList.ToArray();
        }
    }
}
