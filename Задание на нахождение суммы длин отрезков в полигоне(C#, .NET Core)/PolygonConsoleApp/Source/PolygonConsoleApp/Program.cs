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
            bool noComments = false;                     //Выдать результат сразу
            string startMes = "\r\n Приложение рассчитывает сумму длин частей отрезков, находящихся внутри многоугольника. " + 
                "\r\n Координаты отрезков и углов многоугольника задаются в файлах \"segments.csv\" и \"polygonPoints.csv\", находящихся в папке с программой." +
                "\r\n Вы также можете самостоятельно задать пути к файлам передав их в качестве аргументов. " +
                "\r\n Пример:> ./PolygonConsoleApp.exe s=\"C:\\Temp\\Координаты_отрезков.csv\" p=\"C:\\Temp\\Координаты_углов_многоугольника.csv\"" +
                "\r\n Приложение может работать с выпуклыми и невыпуклыми многоугольниками без самопересечений.";
            string cycleMes = "\r\n \r\n Нажмите Enter для расчета, или введите \"H\" и Enter для дополнительной информации. Введите \"X\" и Enter для выхода." +
                "\r\n>>";
            string helpMes = "\r\n Координаты отрезков задаются построчно (по ум. в файле \"segments.csv\"). Формат: {X начала отрезка};{Y начала отрезка};{X конца отрезка};{Y конца отрезка} " +
                        "\r\n Пример строки: 62,076745;79,945621;64,819002;84,763976" +
                        "\r\n Координаты углов многоугольника задаются построчно (по ум. в файле \"polygonPoints.csv\"). Формат: {X точки};{Y точки}" +
                        "\r\n Пример строки: 62,76284217;79,61893362" +
                        "\r\n Углы многоугольника описываются последовательно, вдоль его границы." +
                        "\r\n>>";

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
                    case "-c":                      //Аргумент для запуска программы без доп. комментариев
                        noComments = true;
                        break;
                }
            }

            List<string[]> polygonPointsList = new List<string[]>();
            List<string[]> segmentsList = new List<string[]>();
            
            if (!noComments) Console.Write(startMes + cycleMes);

            string inpString = "";

            while (true)
            {                
                if (!noComments) inpString = Console.ReadLine().ToLower();
                switch (inpString)
                {
                    case "h":
                        Console.Write(helpMes);
                        break;
                    case "x":
                        Environment.Exit(0);
                        break;
                    case "":
                        try
                        {
                            polygonPointsList = ReadCSVFile(polygonPointsFilePath);
                            segmentsList = ReadCSVFile(segmentsFilePath);
                        }
                        catch
                        {
                            if (noComments)
                            { Console.WriteLine("ER_READCSV"); Environment.Exit(-1); }
                            else
                            {
                                Console.WriteLine("Ошибка распознавания *.csv файлов!" +
                              "\r\n Нажмите Enter для выхода из приложения. "); Console.ReadKey(); Environment.Exit(-1);
                            }
                        }
                        Point[] polyPoints = ConvertToPoint(polygonPointsList);
                        Segment[] lines = ConvertToSegment(segmentsList);
                        GeomShapesToCheckForIntersec shapesToCheckForIntersec = new GeomShapesToCheckForIntersec(polyPoints, lines);
                        Console.WriteLine(shapesToCheckForIntersec.calcSegmentsSummInPoly());
                        if (noComments) Environment.Exit(0);
                        GC.Collect();
                        break;
                }
                Console.Write(cycleMes);
            }
        }

        private static List<string[]> ReadCSVFile(string pathToCsvFile)
        {
            using (StreamReader sr = new StreamReader(pathToCsvFile, Encoding.Default))
            {
                _ = sr.ReadLine();  //Пропуск заголовка
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
                        double XBegSeg = Convert.ToDouble((string)_segmentsStringsList[i][0]);
                        double YBegSeg = Convert.ToDouble((string)_segmentsStringsList[i][1]);
                        double XEndSeg = Convert.ToDouble((string)_segmentsStringsList[i][2]);
                        double YEndSeg = Convert.ToDouble((string)_segmentsStringsList[i][3]);
                        if (XBegSeg > XEndSeg)                                                                  //если x координата первой точки больше x второй 
                        { (XBegSeg, XEndSeg) = (XEndSeg, XBegSeg); (YBegSeg, YEndSeg) = (YEndSeg, YBegSeg); }   //обмен координат точек отрезка
                        else if (XBegSeg == XEndSeg && YBegSeg < YEndSeg) { (XBegSeg, XEndSeg) = (XEndSeg, XBegSeg); (YBegSeg, YEndSeg) = (YEndSeg, YBegSeg); }
                        _segmentsList.Add(new Segment
                        {
                            P1 = new Point { X = XBegSeg, Y = YBegSeg },
                            P2 = new Point { X = XEndSeg, Y = YEndSeg }
                        });
                    }
                }
                catch { }
            }
            return _segmentsList.ToArray();
        }
    }
}
