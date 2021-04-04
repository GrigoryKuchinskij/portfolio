using System;
using System.Collections.Generic;
using System.Linq;
using static PolygonConsoleApp.GeometricObjects;

namespace PolygonConsoleApp
{
    class GeomShapesToCheckForIntersec
    {
        private Point[] _polyPoints;
        private Segment[] _lines;

        public GeomShapesToCheckForIntersec(Point[] polyPoints, Segment[] lines)
        {
            _polyPoints = polyPoints;
            _lines = lines;
        }

        //private static double dbs(double a) { return (a < 0.0) ? -a : a; }

        public static bool CheckIntersectionOfTwoLineSegments(Segment L1, Segment L2, out Segment pointsIntersection) //метод, проверяющий пересекаются ли 2 отрезка [p1, p2] и [pA, pB]
        {                                                                                                       //и возвращающий истинность пересечения и точку (или отрезок) пересечения
            pointsIntersection = new Segment();
            if (L1 == null || L2 == null) return false;
            double Xi, Yi,                                                  // - координаты точки пересечения двух прямых
               A1, A2, b1, b2;
            if (L1.P2.X < L1.P1.X)                                          //расставление точек по порядку, т.е. чтобы было p1.X <= p2.X
            {
                Point tmp = L1.P1;
                L1.P1 = L1.P2;
                L1.P2 = tmp;
            }
            if (L2.P2.X < L2.P1.X)
            {
                Point tmp = L2.P1;
                L2.P1 = L2.P2;
                L2.P2 = tmp;
            }
            double maxYL1 = Math.Max(L1.P1.Y, L1.P2.Y);
            double minYL1 = Math.Min(L1.P1.Y, L1.P2.Y);
            double maxYL2 = Math.Max(L2.P1.Y, L2.P2.Y);
            double minYL2 = Math.Min(L2.P1.Y, L2.P2.Y);
            double maxXL1 = Math.Max(L1.P1.X, L1.P2.X);
            double minXL1 = Math.Min(L1.P1.X, L1.P2.X);
            double maxXL2 = Math.Max(L2.P1.X, L2.P2.X);
            double minXL2 = Math.Min(L2.P1.X, L2.P2.X);

            if (maxXL1 < minXL2) { return false; }                          //проверим существование потенциального интервала для точки пересечения отрезков
            if (maxYL1 < minYL2) { return false; }

            if ((L1.P1.X - L1.P2.X == 0) && (L2.P1.X - L2.P2.X == 0))       //если оба отрезка вертикальные
            {
                if (L1.P1.X == L2.P1.X)                                     //если они лежат на одном X
                {
                    if (maxYL1 >= minYL2 && minYL1 <= maxYL2)               //проверим пересекаются ли они, т.е. есть ли у них общий Y
                    {
                        double maxYi = Math.Min(maxYL1, maxYL2);
                        double minYi = Math.Max(minYL1, minYL2);
                        if (maxYi == minYi) pointsIntersection = new Segment { P1 = new Point { X = L1.P1.X, Y = maxYi } };
                        else pointsIntersection = new Segment { P1 = new Point { X = L1.P1.X, Y = maxYi }, P2 = new Point { X = L1.P1.X, Y = minYi } };
                        return true;
                    };
                }
                return false;
            }

            if ((L1.P1.Y - L1.P2.Y == 0) && (L2.P1.Y - L2.P2.Y == 0))       //если оба отрезка горизонтальные
            {
                if (L1.P1.Y == L2.P1.Y)                                     //если они лежат на одном Y
                {
                    if (maxXL1 >= minXL2 && minXL1 <= maxXL2)               //проверим пересекаются ли они, т.е. есть ли у них общий Y
                    {
                        double maxXi = Math.Min(maxXL1, maxXL2);
                        double minXi = Math.Max(minXL1, minXL2);
                        if (maxXi == minXi) pointsIntersection = new Segment { P1 = new Point { X = minXi, Y = L1.P1.Y } };
                        else pointsIntersection = new Segment { P1 = new Point { X = minXi, Y = L1.P1.Y }, P2 = new Point { X = maxXi, Y = L1.P1.Y } };
                        return true;
                    };
                }
                return false;
            }

            //коэффициенты уравнений, содержащих отрезки
            //f1(x) = A1*x + b1 = y
            //f2(x) = A2*x + b2 = y

            if (L1.P1.X - L1.P2.X == 0)                                       //если первый отрезок вертикальный
            {
                //найдём Xi, Yi - точки пересечения двух прямых
                Xi = L1.P1.X;
                A2 = (L2.P1.Y - L2.P2.Y) / (L2.P1.X - L2.P2.X);
                b2 = L2.P1.Y - A2 * L2.P1.X;
                Yi = A2 * Xi + b2;

                if (L2.P1.X <= Xi && L2.P2.X >= Xi && Math.Min(L1.P1.Y, L1.P2.Y) <= Yi && Math.Max(L1.P1.Y, L1.P2.Y) >= Yi)
                {
                    pointsIntersection = new Segment { P1 = new Point { X = Xi, Y = Yi } };
                    return true;
                }
                return false;
            }

            if (L2.P1.X - L2.P2.X == 0)                                       //если второй отрезок вертикальный
            {
                //найдём Xi, Yi - точки пересечения двух прямых
                Xi = L2.P1.X;
                A1 = (L1.P1.Y - L1.P2.Y) / (L1.P1.X - L1.P2.X);
                b1 = L1.P1.Y - A1 * L1.P1.X;
                Yi = A1 * Xi + b1;

                if (L1.P1.X <= Xi && L1.P2.X >= Xi && Math.Min(L2.P1.Y, L2.P2.Y) <= Yi && Math.Max(L2.P1.Y, L2.P2.Y) >= Yi)
                {
                    pointsIntersection = new Segment { P1 = new Point { X = Xi, Y = Yi } };
                    return true;
                }
                return false;
            }

            //оба отрезка невертикальные и негоризонтальные
            A1 = (L1.P1.Y - L1.P2.Y) / (L1.P1.X - L1.P2.X);
            A2 = (L2.P1.Y - L2.P2.Y) / (L2.P1.X - L2.P2.X);

            if (A1 == A2)                                                           //отрезки параллельны
            {
                double A1_A = (L1.P1.Y - L2.P1.Y) / (L1.P1.X - L2.P1.X);
                if (A1 == A1_A)                                                     //отрезки накладываются
                {
                    if (L1.P1.Y > L1.P2.Y)                                                //если наклон отрезка => \ 
                    {
                        pointsIntersection = new Segment { 
                            P1 = new Point 
                            { 
                                X = Math.Max(minXL1, minXL2), Y = Math.Min(maxYL1, maxYL2) 
                            },
                            P2 = new Point 
                            { 
                                X = Math.Min(maxXL1, maxXL2), Y = Math.Max(minYL1, minYL2) 
                            } };
                        return true;
                    }
                    if (L1.P1.Y < L1.P2.Y)                                                //если наклон отрезка => /
                    {
                        pointsIntersection = new Segment { 
                            P1 = new Point 
                            { 
                                X = Math.Max(minXL1, minXL2), Y = Math.Max(minYL1, minYL2) 
                            }, 
                            P2 = new Point 
                            { 
                                X = Math.Min(maxXL1, maxXL2), Y = Math.Min(maxYL1, maxYL2) 
                            } };
                        return true;
                    }
                }
                return false;
            }

            b1 = L1.P1.Y - A1 * L1.P1.X;
            b2 = L2.P1.Y - A2 * L2.P1.X;

            //Xi - абсцисса точки пересечения двух прямых
            Xi = (b2 - b1) / (A1 - A2);
            if ((Xi < Math.Max(L1.P1.X, L2.P1.X)) || (Xi > Math.Min(L1.P2.X, L2.P2.X)))
            { return false; }
            else                                                                    //если произвольные непараллельные отрезки
            {
                double f11 = L1.P2.Y - L1.P1.Y;
                double f21 = L1.P1.X - L1.P2.X;
                double f31 = -L1.P1.X * (L1.P2.Y - L1.P1.Y) + L1.P1.Y * (L1.P2.X - L1.P1.X);

                double f12 = L2.P2.Y - L2.P1.Y;
                double f22 = L2.P1.X - L2.P2.X;
                double f32 = -L2.P1.X * (L2.P2.Y - L2.P1.Y) + L2.P1.Y * (L2.P2.X - L2.P1.X);

                double d = f11 * f22 - f21 * f12;

                pointsIntersection = new Segment { P1 = new Point { X = (-f31 * f22 + f21 * f32) / d, Y = (-f11 * f32 + f31 * f12) / d } };
                return true;
            };
        }

        public static int PointInPoly(Point point, Point[] poly)                                // метод принимает проверяемую точку и координаты вершин многоугольника
        {                                                                                       // и возвращает значение нахождения проверяемой точки в многоугольнике
            if (point == null || poly == null) return -1;
            bool onEdge = false;
            int i, j = poly.Length - 1;
            bool oddNodes = false;
            for (i = 0; i < poly.Length; i++)                                                   //прохождение по всем граням
            {
                if (Math.Round(point.Y, 8) == Math.Round(poly[i].Y, 8) && Math.Round(point.X, 8) == Math.Round(poly[i].X, 8)) { onEdge = true; break; }
                if ((poly[i].Y < point.Y && poly[j].Y >= point.Y                                // обе вершины выше и ниже проверяемой точки
                    || poly[j].Y < point.Y && poly[i].Y >= point.Y)
                        && (poly[i].X <= point.X || poly[j].X <= point.X))
                {
                    if (poly[i].X + (point.Y - poly[i].Y) / (poly[j].Y - poly[i].Y) * (poly[j].X - poly[i].X) < point.X)
                    {
                        oddNodes = !oddNodes;
                    }
                };
                if (!(Math.Max(poly[i].Y, poly[j].Y) < point.Y || Math.Min(poly[i].Y, poly[j].Y) > point.Y || Math.Max(poly[i].X, poly[j].X) < point.X || Math.Min(poly[i].X, poly[j].X) > point.X))
                {
                    double AL = (poly[j].Y - poly[i].Y) / (poly[j].X - poly[i].X);
                    double AL_P = (point.Y - poly[i].Y) / (point.X - poly[i].X);
                    if (Math.Round(AL, 8) == Math.Round(AL_P, 8))                               //точка лежит на отрезке или вершине
                    //if ((point.Y == ((poly[j].Y - poly[i].Y) / 2) + poly[i].Y                                              
                    //        && point.X == ((poly[j].X - poly[i].X) / 2) + poly[i].X)
                    //    || point.Y == poly[i].Y && point.X == poly[i].X)
                    { onEdge = true; break; }
                }
                j = i;
            }
            if (onEdge == true) return 0;       // точка на грани фигуры
            else if (!oddNodes) return -1;      // точка вне фигуры            
            else return 1;                      // точка внутри фигуры
        }

        private void sortPointsOfLineByXY(ref Point[] points)
        {
            bool hadSomethingToSort = true;
            while (hadSomethingToSort)
            {
                for (int i = 0; i < points.Length; i++)
                    for (int j = 0; j < points.Length - 1; j++)
                        if (points[j].X > points[j + 1].X
                            || (points[j].X == points[j + 1].X && points[j].Y > points[j + 1].Y))
                        {
                            Point t = points[j + 1];
                            points[j + 1] = points[j];
                            points[j] = t;
                            hadSomethingToSort = true;
                        }
                        else hadSomethingToSort = false;
            }
        }

        private Point[] splitSegmentsIntoPoints(Segment[] segments)
        {
            List<Point> pointsL = new List<Point>();
            for (int i = 0; i < segments.Length; i++)
            {
                pointsL.Add(segments[i].P1);
                if (segments[i].P2 != null)
                    pointsL.Add(segments[i].P2);
            }
            return pointsL.ToArray();
        }

        private Point[] removeDuplicatePoints(Point[] points)
        {
            if (points == null) return null;
            List<Point> pointsL = new List<Point>();
            int prevI = 0;
            pointsL.Add(points[prevI]);
            for (int i = 1; i < points.Count(); i++)
            {
                if (!(points[prevI].X == points[i].X && points[prevI].Y == points[i].Y))
                    pointsL.Add(points[i]);
                prevI = i;
            }
            return pointsL.ToArray();
        }

        public double calcSegmentsSummInPoly()                                              //метод расчета суммы длинн частей отрезков внутри многоугольника 
        {
            double summLength = 0;
            if (_polyPoints == null || _polyPoints.Length == 0) return 0;
            for (int i = 0; i < _lines.Count(); i++)                                        //проход в цикле по отрезкам
            {
                List<Segment> listOfSegmentsAndPointsOnCurrentLine = new List<Segment>();   //точки и отрезки лежащие на текущем отрезке перекающиеся с многоугольником
                listOfSegmentsAndPointsOnCurrentLine.Add(new Segment { P1 = _lines[i].P1 });//начальная точка отрезка
                int p1i = _polyPoints.Count() - 1;                                          //предыдущая точка многоугольника
                for (int p2i = 0; p2i < _polyPoints.Count(); p2i++)                         //проход в цикле по граням многоугольника
                {
                    Segment pointsIntersection;
                    bool intersecIsEx = CheckIntersectionOfTwoLineSegments(                 //проверка пересечения отрезка и грани
                        _lines[i],
                        new Segment
                        {
                            P1 = _polyPoints[p1i],
                            P2 = _polyPoints[p2i]
                        },
                        out pointsIntersection
                    );
                    if (intersecIsEx)
                    {                                                                       //если есть пересечение
                        listOfSegmentsAndPointsOnCurrentLine.Add(pointsIntersection);       //добавить элемент в список точек пересечений
                    };
                    p1i = p2i;
                }
                listOfSegmentsAndPointsOnCurrentLine.Add(new Segment { P1 = _lines[i].P2 });//конечная точка отрезка
                //сортировка точек и очистка от повторений
                Segment[] msvOfSegmentsAndPointsOnCurrentLine = listOfSegmentsAndPointsOnCurrentLine.ToArray();
                Point[] unclearedMsvOfPoints = splitSegmentsIntoPoints(msvOfSegmentsAndPointsOnCurrentLine);
                sortPointsOfLineByXY(ref unclearedMsvOfPoints);
                Point[] msvOfPointsOnCurrentLine = removeDuplicatePoints(unclearedMsvOfPoints);

                int pOLCount = msvOfPointsOnCurrentLine.Count();
                if (pOLCount < 1) continue;

                //проверка нахождения сегментов отрезка в многоугольнике
                int tempPointIsInPoly = PointInPoly(msvOfPointsOnCurrentLine[0], _polyPoints);
                for (int firstIndex = 0; firstIndex < pOLCount - 1; firstIndex++)
                {
                    int secondIndex = firstIndex + 1;

                    int fPointIsInPoly = tempPointIsInPoly;
                    //int fPointIsInPoly = PointInPoly(msvOfPointsOnCurrentLine[firstIndex], _polyPoints);
                    int sPointIsInPoly = PointInPoly(msvOfPointsOnCurrentLine[secondIndex], _polyPoints);
                    tempPointIsInPoly = sPointIsInPoly;

                    if (fPointIsInPoly == -1 || sPointIsInPoly == -1)                                       //если один из концов сегмента отрезка снаружи
                        continue;
                    if (fPointIsInPoly == 1 || sPointIsInPoly == 1)                                         //если один из концов сегмента отрезка внутри
                    {
                        double leng = Math.Sqrt(Math.Pow((msvOfPointsOnCurrentLine[secondIndex].X - msvOfPointsOnCurrentLine[firstIndex].X), 2)
                                + Math.Pow((msvOfPointsOnCurrentLine[secondIndex].Y - msvOfPointsOnCurrentLine[firstIndex].Y), 2));
                        summLength += leng;
                        continue;
                    }
                    //если оба конца сегмента на грани 
                    //расчет средней точки фрагмента отрезка
                    double mX = msvOfPointsOnCurrentLine[firstIndex].X + ((msvOfPointsOnCurrentLine[secondIndex].X - msvOfPointsOnCurrentLine[firstIndex].X) / 2);
                    double mY = msvOfPointsOnCurrentLine[firstIndex].Y + ((msvOfPointsOnCurrentLine[secondIndex].Y - msvOfPointsOnCurrentLine[firstIndex].Y) / 2);
                    int mPointIsInPoly = PointInPoly(new Point { X = mX, Y = mY }, _polyPoints);

                    if (mPointIsInPoly == 0 || mPointIsInPoly == 1)                                         //если средняя точка сегмента на грани или внутри
                    {
                        double leng = Math.Sqrt(Math.Pow((msvOfPointsOnCurrentLine[secondIndex].X - msvOfPointsOnCurrentLine[firstIndex].X), 2)
                                + Math.Pow((msvOfPointsOnCurrentLine[secondIndex].Y - msvOfPointsOnCurrentLine[firstIndex].Y), 2));
                        summLength += leng;
                        continue;
                    }
                }
            };
            return summLength;
        }
    }
}
