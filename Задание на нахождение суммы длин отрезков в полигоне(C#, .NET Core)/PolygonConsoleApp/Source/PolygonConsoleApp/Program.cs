using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static PolygonConsoleApp.GeometricObjects;
using static PolygonConsoleApp.GeomShapesToCheckForIntersec;

namespace PolygonConsoleApp
{
    class Program
    {        
        static void Main(string[] args)
        {
            string polygonPointsFilePath = @"polygonPoints.csv";
            string segmentsFilePath = @"segments.csv";

            foreach (string arg in args)
            {
                switch(arg.Substring(0, 2))
                {
                    case "s=":
                        segmentsFilePath = arg.Substring(2).Trim(new char[] { ' ', '"' });
                        break;
                    case "p=":
                        polygonPointsFilePath = arg.Substring(2).Trim(new char[] { ' ', '"' });
                        break;
                }
            }

            List<string[]> polygonPointsList = new List<string[]>();
            List<string[]> segmentsList = new List<string[]>();
            try 
            {
                polygonPointsList = ReadCSVFile(polygonPointsFilePath);
                segmentsList = ReadCSVFile(segmentsFilePath);
            } catch 
            { 
                Console.WriteLine("Ошибка распознавания *.csv файлов!" +
                "\r\n Нажмите Enter для выхода из приложения. "); Console.ReadKey(); Environment.Exit(-1); 
            }
            Point[] polyPoints = ConvertToPoint(polygonPointsList);
            Segment[] lines = ConvertToSegment(segmentsList);            
            Console.Write(
                "\r\n Приложение рассчитывает сумму длин частей отрезков, находящихся внутри многоугольника. " + 
                "\r\n Координаты отрезков и углов многоугольника задаются в файлах \"segments.csv\" и \"polygonPoints.csv\", находящихся в папке с программой." +
                "\r\n Вы также можете самостоятельно задать пути к файлам передав их в качестве аргументов. " +
                "\r\n Пример:> ./PolygonConsoleApp.exe s=\"C:\\Temp\\Координаты_отрезков.csv\" p=\"C:\\Temp\\Координаты_углов_многоугольника.csv\"" +
                "\r\n Приложение может работать с выпуклыми и невыпуклыми многоугольниками без самопересечений." +
                "\r\n \r\n Нажмите Enter для начала расчета, или введите \"H\" и Enter для дополнительной информации. Введите \"X\" и Enter для выхода." +
                "\r\n>>");            
            while (true)
            {
                string inpString = Console.ReadLine().ToLower();
                switch (inpString)
                {
                    case "h":
                        Console.Write("\r\n Координаты отрезков задаются построчно (по ум. в файле \"segments.csv\"). Формат: {X начала отрезка};{Y начала отрезка};{X конца отрезка};{Y конца отрезка} " +
                        "\r\n Пример строки: 62,076745;79,945621;64,819002;84,763976" +
                        "\r\n Координаты углов многоугольника задаются построчно (по ум. в файле \"polygonPoints.csv\"). Формат: {X точки};{Y точки}" +
                        "\r\n Пример строки: 62,76284217;79,61893362" +
                        "\r\n Углы многоугольника описываются последовательно, вдоль его границы." +
                        "\r\n>>");
                        break;
                    case "x":
                        Environment.Exit(0);
                        break;
                    case "":
                        GeomShapesToCheckForIntersec shapesToCheckForIntersec = new GeomShapesToCheckForIntersec(polyPoints, lines);
                        Console.WriteLine(shapesToCheckForIntersec.calcSegmentsSummInPoly());
                        GC.Collect();
                        break;
                }
                Console.Write("\r\n \r\n Нажмите Enter для расчета, или введите \"H\" и Enter для дополнительной информации. Введите \"X\" и Enter для выхода." +
                "\r\n>>");
            }
        }

        private static List<string[]> ReadCSVFile(string pathToCsvFile)
        {
            using (StreamReader sr = new StreamReader(pathToCsvFile, Encoding.Default))
            {
                List<string[]> vs = new List<string[]>();
                while (!sr.EndOfStream)
                {
                    string[] rowValues = sr.ReadLine().Split(';');
                    vs.Add(rowValues);
                }
                return vs;
            }
        }

        private static Point[] ConvertToPoint(List<string[]> _pointsList)
        {
            Point[] _points = new Point[_pointsList.Count];
            for (int i = 0; i < _pointsList.Count; i++)
            {
                try
                {
                    if (_pointsList[i].Length == 2)
                    {
                        _points[i] = new Point { X = Convert.ToDouble(_pointsList[i][0]), Y = Convert.ToDouble(_pointsList[i][1]) };
                    }
                }
                catch { }
            }
            return _points;
        }

        private static Segment[] ConvertToSegment(List<string[]> _segmentsStringsList)
        {
            List<Segment> _segmentsList = new List<Segment>();
            for (int i = 0; i < _segmentsStringsList.Count; i++)
            {
                try
                {
                    if (_segmentsStringsList[i].Length == 4)
                    {
                        double xbegSeg = Convert.ToDouble((string)_segmentsStringsList[i][0]);
                        double ybegSeg = Convert.ToDouble((string)_segmentsStringsList[i][1]);
                        double xendSeg = Convert.ToDouble((string)_segmentsStringsList[i][2]);
                        double yendSeg = Convert.ToDouble((string)_segmentsStringsList[i][3]);
                        if (xbegSeg > xendSeg)                                                                  //если x координата первой точки больше x второй 
                        { (xbegSeg, xendSeg) = (xendSeg, xbegSeg); (ybegSeg, yendSeg) = (yendSeg, ybegSeg); }   //обмен координат точек отрезка
                        else if (xbegSeg == xendSeg && ybegSeg < yendSeg) { (xbegSeg, xendSeg) = (xendSeg, xbegSeg); (ybegSeg, yendSeg) = (yendSeg, ybegSeg); }
                        _segmentsList.Add(new Segment
                        {
                            P1 = new Point { X = xbegSeg, Y = ybegSeg },
                            P2 = new Point { X = xendSeg, Y = yendSeg }
                        });
                    }
                }
                catch { }
            }
            return _segmentsList.ToArray();
        }
    }
}
