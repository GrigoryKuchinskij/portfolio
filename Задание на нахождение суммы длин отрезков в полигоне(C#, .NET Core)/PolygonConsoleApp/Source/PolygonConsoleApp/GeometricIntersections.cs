using System;
using System.Collections.Generic;
using System.Linq;
using static PolygonConsoleApp.GeometricObjects;

namespace PolygonConsoleApp
{
    class CalcGeometricIntersections
    {
        public static double CalcSegmentsSummInPoly(Point[] polyPoints, Segment[] lines)    //метод расчета суммы длинн частей отрезков внутри многоугольника 
        {
            double summLength = 0;
            if (polyPoints == null || polyPoints.Length == 0) return 0;
            for (int i = 0; i < lines.Count(); i++)                                         //проход в цикле по отрезкам
            {
                List<Segment> listOfSegmentsAndPointsOnCurrentLine = new List<Segment>();   //точки и отрезки, лежащие на текущем отрезке, который пресекается с гранями многоугольника

                listOfSegmentsAndPointsOnCurrentLine.Add(new Segment { P1 = lines[i].P1 }); //начальная точка отрезка

                int previousPointIndex = polyPoints.Count() - 1;                            //предыдущая точка многоугольника

                for (int pointIndex = 0; pointIndex < polyPoints.Count(); pointIndex++)     //проход в цикле по граням многоугольника
                {
                    Segment pointsIntersection;
                    bool intersectionIsExist = CheckIntersectionOfTwoLineSegments(          //проверка пересечения отрезка и грани
                        lines[i],
                        new Segment
                        {
                            P1 = polyPoints[previousPointIndex],
                            P2 = polyPoints[pointIndex]
                        },
                        out pointsIntersection
                    );
                    if (intersectionIsExist)
                    {                                                                       //если есть пересечение
                        listOfSegmentsAndPointsOnCurrentLine.Add(pointsIntersection);       //добавить элемент в список точек пересечений
                    };
                    previousPointIndex = pointIndex;
                }

                listOfSegmentsAndPointsOnCurrentLine.Add(new Segment { P1 = lines[i].P2 }); //конечная точка отрезка

                //сортировка точек и очистка от повторений
                Point[] unclearedArrayOfPoints = SplitSegmentsIntoPoints(listOfSegmentsAndPointsOnCurrentLine);
                sortPointsOfLineByXY(ref unclearedArrayOfPoints);
                Point[] PointsOnCurrentLine = RemoveDuplicatedPoints(unclearedArrayOfPoints);

                int pOLCount = PointsOnCurrentLine.Count();
                if (pOLCount < 1) continue;

                //проверка нахождения сегментов отрезка в многоугольнике

                PointInPoly(PointsOnCurrentLine[0], polyPoints, out sbyte tempPointIsInPoly);
                for (int firstIndex = 0; firstIndex < pOLCount - 1; firstIndex++)
                {
                    int secondIndex = firstIndex + 1;

                    sbyte firstPointIsInPoly = tempPointIsInPoly;
                    PointInPoly(PointsOnCurrentLine[secondIndex], polyPoints, out sbyte secondPointIsInPoly);
                    tempPointIsInPoly = secondPointIsInPoly;

                    if (firstPointIsInPoly == -1 || secondPointIsInPoly == -1)              //если один из концов сегмента отрезка снаружи
                        continue;
                    if (firstPointIsInPoly == 1 || secondPointIsInPoly == 1)                //если один из концов сегмента отрезка внутри
                    {
                        double leng = Math.Sqrt(Math.Pow((PointsOnCurrentLine[secondIndex].X - PointsOnCurrentLine[firstIndex].X), 2)
                                + Math.Pow((PointsOnCurrentLine[secondIndex].Y - PointsOnCurrentLine[firstIndex].Y), 2));
                        summLength += leng;
                        continue;
                    }
                    //если оба конца сегмента на грани 
                    //расчет средней точки фрагмента отрезка
                    double mX = PointsOnCurrentLine[firstIndex].X + ((PointsOnCurrentLine[secondIndex].X - PointsOnCurrentLine[firstIndex].X) / 2);
                    double mY = PointsOnCurrentLine[firstIndex].Y + ((PointsOnCurrentLine[secondIndex].Y - PointsOnCurrentLine[firstIndex].Y) / 2);
                    bool mPointIsInPoly = PointInPoly(new Point { X = mX, Y = mY }, polyPoints);

                    if (mPointIsInPoly)                                                     //если средняя точка сегмента на грани или внутри
                    {
                        double leng = Math.Sqrt(Math.Pow((PointsOnCurrentLine[secondIndex].X - PointsOnCurrentLine[firstIndex].X), 2)
                                + Math.Pow((PointsOnCurrentLine[secondIndex].Y - PointsOnCurrentLine[firstIndex].Y), 2));
                        summLength += leng;
                        continue;
                    }
                }
            };
            return summLength;
        }

        public static bool PointInPoly(Point point, Point[] poly) => PointInPoly(point, poly, out _);

        public static bool PointInPoly(Point point, Point[] poly, out sbyte state)          // метод принимает проверяемую точку и координаты вершин многоугольника
        {                                                                                   // и возвращает значение нахождения проверяемой точки в многоугольнике
            if (point == null || poly == null)
            {
                state = -1;
                return false;
            }
            bool onEdge = false;
            int i, j = poly.Length - 1;
            bool oddNodes = false;
            for (i = 0; i < poly.Length; i++)                                               //прохождение по всем граням
            {
                if (Math.Round(point.Y, 8) == Math.Round(poly[i].Y, 8) && Math.Round(point.X, 8) == Math.Round(poly[i].X, 8))
                {
                    onEdge = true;
                    break;
                }
                // обе вершины выше и ниже проверяемой точки
                if ((poly[i].Y < point.Y && poly[j].Y >= point.Y || poly[j].Y < point.Y && poly[i].Y >= point.Y)
                    && (poly[i].X <= point.X || poly[j].X <= point.X))
                {
                    if (poly[i].X + (point.Y - poly[i].Y) / (poly[j].Y - poly[i].Y) * (poly[j].X - poly[i].X) < point.X)
                    {
                        oddNodes = !oddNodes;
                    }
                };
                if (!(Math.Max(poly[i].Y, poly[j].Y) < point.Y || Math.Min(poly[i].Y, poly[j].Y) > point.Y || Math.Max(poly[i].X, poly[j].X) < point.X || Math.Min(poly[i].X, poly[j].X) > point.X))
                {
                    double coordinateDifferenceRatio = (poly[j].Y - poly[i].Y) / (poly[j].X - poly[i].X);
                    double CDRatioBetweenPolypointAndPoint = (point.Y - poly[i].Y) / (point.X - poly[i].X);
                    //точка лежит на отрезке или вершине
                    if (Math.Round(coordinateDifferenceRatio, 8) == Math.Round(CDRatioBetweenPolypointAndPoint, 8))
                    {
                        onEdge = true;
                        break;
                    }
                }
                j = i;
            }
            if (onEdge == true)                 // точка на грани фигуры
            {
                state = 0;
                return true;
            }
            else if (!oddNodes)                 // точка вне фигуры
            {
                state = -1;
                return false;
            }
            else                                // точка внутри фигуры
            {
                state = 1;
                return true;
            }
        }

        public static bool CheckIntersectionOfTwoLineSegments(Segment L1, Segment L2, out Segment pointsIntersection) //метод, проверяющий пересекаются ли 2 отрезка [p1, p2] и [pA, pB]
        {                                                                                                             //и возвращающий истинность пересечения и точку (или отрезок) пересечения
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

            //////////////////////////////////////////////////////////////
            //Проверка частных случаев пересечения для исключения ошибок//
            //////////////////////////////////////////////////////////////

            //проверим существование потенциального интервала для точки пересечения отрезков
            if (maxXL1 < minXL2)
                return false;
            if (maxYL1 < minYL2)
                return false;

            //если оба отрезка вертикальные
            if ((L1.P1.X - L1.P2.X == 0) && (L2.P1.X - L2.P2.X == 0))
            {
                if (L1.P1.X != L2.P1.X)                                     //если они не лежат на одном X
                    return false;
                if (maxYL1 >= minYL2 && minYL1 <= maxYL2)               //проверим пересекаются ли они, т.е. есть ли у них общий Y
                {
                    double maxYi = Math.Min(maxYL1, maxYL2);
                    double minYi = Math.Max(minYL1, minYL2);
                    if (maxYi == minYi)
                        pointsIntersection = new Segment { P1 = new Point { X = L1.P1.X, Y = maxYi } };
                    else
                        pointsIntersection = new Segment
                        {
                            P1 = new Point { X = L1.P1.X, Y = maxYi },
                            P2 = new Point { X = L1.P1.X, Y = minYi }
                        };
                    return true;
                };
            }

            //если оба отрезка горизонтальные
            if ((L1.P1.Y - L1.P2.Y == 0) && (L2.P1.Y - L2.P2.Y == 0))
            {
                if (L1.P1.Y != L2.P1.Y)                                     //если они не лежат на одном Y
                    return false;
                if (maxXL1 >= minXL2 && minXL1 <= maxXL2)               //проверим пересекаются ли они, т.е. есть ли у них общий Y
                {
                    double maxXi = Math.Min(maxXL1, maxXL2);
                    double minXi = Math.Max(minXL1, minXL2);
                    if (maxXi == minXi)
                        pointsIntersection = new Segment { P1 = new Point { X = minXi, Y = L1.P1.Y } };
                    else
                        pointsIntersection = new Segment
                        {
                            P1 = new Point { X = minXi, Y = L1.P1.Y },
                            P2 = new Point { X = maxXi, Y = L1.P1.Y }
                        };
                    return true;
                };
            }

            //коэффициенты уравнений, содержащих отрезки
            //f1(x) = A1*x + b1 = y
            //f2(x) = A2*x + b2 = y

            //если только первый отрезок вертикальный
            if (L1.P1.X - L1.P2.X == 0)
            {
                //найдём Xi, Yi - точки пересечения двух прямых
                Xi = L1.P1.X;
                A2 = (L2.P1.Y - L2.P2.Y) / (L2.P1.X - L2.P2.X);
                b2 = L2.P1.Y - A2 * L2.P1.X;
                Yi = A2 * Xi + b2;

                if (L2.P1.X <= Xi 
                    && L2.P2.X >= Xi 
                    && Math.Min(L1.P1.Y, L1.P2.Y) <= Yi 
                    && Math.Max(L1.P1.Y, L1.P2.Y) >= Yi)
                {
                    pointsIntersection = new Segment { P1 = new Point { X = Xi, Y = Yi } };
                    return true;
                }
                return false;
            }

            //если только второй отрезок вертикальный
            if (L2.P1.X - L2.P2.X == 0)
            {
                //найдём Xi, Yi - точки пересечения двух прямых
                Xi = L2.P1.X;
                A1 = (L1.P1.Y - L1.P2.Y) / (L1.P1.X - L1.P2.X);
                b1 = L1.P1.Y - A1 * L1.P1.X;
                Yi = A1 * Xi + b1;

                if (L1.P1.X <= Xi 
                    && L1.P2.X >= Xi 
                    && Math.Min(L2.P1.Y, L2.P2.Y) <= Yi 
                    && Math.Max(L2.P1.Y, L2.P2.Y) >= Yi)
                {
                    pointsIntersection = new Segment { P1 = new Point { X = Xi, Y = Yi } };
                    return true;
                }
                return false;
            }

            //////////////////////////////////////////
            //Проверка остальных случаев пересечения//
            //////////////////////////////////////////

            //оба отрезка невертикальные и негоризонтальные
            A1 = (L1.P1.Y - L1.P2.Y) / (L1.P1.X - L1.P2.X);
            A2 = (L2.P1.Y - L2.P2.Y) / (L2.P1.X - L2.P2.X);

            if (A1 == A2)                                                           //отрезки параллельны
            {
                double A1_A = (L1.P1.Y - L2.P1.Y) / (L1.P1.X - L2.P1.X);
                if (A1 != A1_A)
                    return false;
                if (L1.P1.Y > L1.P2.Y)                                                //если наклон отрезка => \ 
                {
                    pointsIntersection = new Segment
                    {
                        P1 = new Point
                        {
                            X = Math.Max(minXL1, minXL2),
                            Y = Math.Min(maxYL1, maxYL2)
                        },
                        P2 = new Point
                        {
                            X = Math.Min(maxXL1, maxXL2),
                            Y = Math.Max(minYL1, minYL2)
                        }
                    };
                    return true;
                }
                if (L1.P1.Y < L1.P2.Y)                                                //если наклон отрезка => /
                {
                    pointsIntersection = new Segment
                    {
                        P1 = new Point
                        {
                            X = Math.Max(minXL1, minXL2),
                            Y = Math.Max(minYL1, minYL2)
                        },
                        P2 = new Point
                        {
                            X = Math.Min(maxXL1, maxXL2),
                            Y = Math.Min(maxYL1, maxYL2)
                        }
                    };
                    return true;
                }
            }

            b1 = L1.P1.Y - A1 * L1.P1.X;
            b2 = L2.P1.Y - A2 * L2.P1.X;

            //Xi - абсцисса точки пересечения двух прямых
            Xi = (b2 - b1) / (A1 - A2);
            if ((Xi < Math.Max(L1.P1.X, L2.P1.X)) || (Xi > Math.Min(L1.P2.X, L2.P2.X)))
            {
                return false;
            }
            //если произвольные непараллельные отрезки
            double f11 = L1.P2.Y - L1.P1.Y;
            double f21 = L1.P1.X - L1.P2.X;
            double f31 = -L1.P1.X * (L1.P2.Y - L1.P1.Y) + L1.P1.Y * (L1.P2.X - L1.P1.X);

            double f12 = L2.P2.Y - L2.P1.Y;
            double f22 = L2.P1.X - L2.P2.X;
            double f32 = -L2.P1.X * (L2.P2.Y - L2.P1.Y) + L2.P1.Y * (L2.P2.X - L2.P1.X);

            double d = f11 * f22 - f21 * f12;

            pointsIntersection = new Segment { P1 = new Point { X = (-f31 * f22 + f21 * f32) / d, Y = (-f11 * f32 + f31 * f12) / d } };
            return true;
        }

        private static Point[] SplitSegmentsIntoPoints(List<Segment> segments)
        {
            List<Point> pointsL = new List<Point>();
            foreach (Segment seg in segments)
            {
                pointsL.Add(seg.P1);
                if (seg.P2 != null)
                    pointsL.Add(seg.P2);
            }
            return pointsL.ToArray();
        }

        private static Point[] RemoveDuplicatedPoints(Point[] points)
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

        private static void sortPointsOfLineByXY(ref Point[] points)
        {
            Point temp;
            for (int i = 1; i < points.Length; i++)
            {
                temp = points[i];
                var j = i;
                while (j > 0
                    && (temp.X < points[j - 1].X
                        || (temp.X == points[j - 1].X && temp.Y < points[j - 1].Y)
                    ))
                {
                    points[j] = points[j - 1];
                    j--;
                }
                points[j] = temp;
            }
        }
    }
}