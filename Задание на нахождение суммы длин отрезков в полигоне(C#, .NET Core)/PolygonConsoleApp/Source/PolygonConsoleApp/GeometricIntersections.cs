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

                listOfSegmentsAndPointsOnCurrentLine.Add(new Segment { Point1 = lines[i].Point1 }); //начальная точка отрезка

                int previousPointIndex = polyPoints.Count() - 1;                            //предыдущая точка многоугольника

                for (int pointIndex = 0; pointIndex < polyPoints.Count(); pointIndex++)     //проход в цикле по граням многоугольника
                {
                    Segment pointsIntersection;
                    bool intersectionIsExist = CheckIntersectionOfTwoLineSegments(          //проверка пересечения отрезка и грани
                        lines[i],
                        new Segment
                        {
                            Point1 = polyPoints[previousPointIndex],
                            Point2 = polyPoints[pointIndex]
                        },
                        out pointsIntersection
                    );
                    if (intersectionIsExist)
                    {                                                                       //если есть пересечение
                        listOfSegmentsAndPointsOnCurrentLine.Add(pointsIntersection);       //добавить элемент в список точек пересечений
                    };
                    previousPointIndex = pointIndex;
                }

                listOfSegmentsAndPointsOnCurrentLine.Add(new Segment { Point1 = lines[i].Point2 }); //конечная точка отрезка

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
            int j = poly.Length - 1;
            bool oddNodes = false;
            for (int i = 0; i < poly.Length; i++)                                               //прохождение по всем граням
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

        public static bool CheckIntersectionOfTwoLineSegments(Segment LineSegment1, Segment LineSegment2, out Segment pointsIntersection) //метод, проверяющий пересекаются ли 2 отрезка 
        {                                                                                                             //и возвращающий истинность пересечения и точку (или отрезок) пересечения
            pointsIntersection = new Segment();
            if (LineSegment1 == null || LineSegment2 == null) return false;
            double intersectionX, intersectionY,                                                  // - координаты точки пересечения двух прямых
               Angle1, Angle2, b1, b2;
            if (LineSegment1.Point2.X < LineSegment1.Point1.X)                                          //расставление точек по порядку, т.е. чтобы было p1.X <= p2.X
            {
                (LineSegment1.Point2, LineSegment1.Point1) = (LineSegment1.Point1, LineSegment1.Point2);
            }
            if (LineSegment2.Point2.X < LineSegment2.Point1.X)
            {
                (LineSegment2.Point2, LineSegment2.Point1) = (LineSegment2.Point1, LineSegment2.Point2);
            }
            double maxYSegment1 = Math.Max(LineSegment1.Point1.Y, LineSegment1.Point2.Y);
            double minYSegment1 = Math.Min(LineSegment1.Point1.Y, LineSegment1.Point2.Y);
            double maxYSegment2 = Math.Max(LineSegment2.Point1.Y, LineSegment2.Point2.Y);
            double minYSegment2 = Math.Min(LineSegment2.Point1.Y, LineSegment2.Point2.Y);
            double maxXSegment1 = Math.Max(LineSegment1.Point1.X, LineSegment1.Point2.X);
            double minXSegment1 = Math.Min(LineSegment1.Point1.X, LineSegment1.Point2.X);
            double maxXSegment2 = Math.Max(LineSegment2.Point1.X, LineSegment2.Point2.X);
            double minXSegment2 = Math.Min(LineSegment2.Point1.X, LineSegment2.Point2.X);

            //////////////////////////////////////////////////////////////
            //Проверка частных случаев пересечения для исключения ошибок//
            //////////////////////////////////////////////////////////////

            //проверим существование потенциального интервала для точки пересечения отрезков
            if (maxXSegment1 < minXSegment2)
                return false;
            if (maxYSegment1 < minYSegment2)
                return false;

            //если оба отрезка вертикальные
            if ((LineSegment1.Point1.X - LineSegment1.Point2.X == 0) && (LineSegment2.Point1.X - LineSegment2.Point2.X == 0))
            {
                if (LineSegment1.Point1.X != LineSegment2.Point1.X)                                     //если они не лежат на одном X
                    return false;
                if (maxYSegment1 >= minYSegment2 && minYSegment1 <= maxYSegment2)               //проверим пересекаются ли они, т.е. есть ли у них общий Y
                {
                    double maxYSegments = Math.Min(maxYSegment1, maxYSegment2);
                    double minYSegments = Math.Max(minYSegment1, minYSegment2);
                    if (maxYSegments == minYSegments)
                        pointsIntersection = new Segment { Point1 = new Point { X = LineSegment1.Point1.X, Y = maxYSegments } };
                    else
                        pointsIntersection = new Segment
                        {
                            Point1 = new Point { X = LineSegment1.Point1.X, Y = maxYSegments },
                            Point2 = new Point { X = LineSegment1.Point1.X, Y = minYSegments }
                        };
                    return true;
                };
            }

            //если оба отрезка горизонтальные
            if ((LineSegment1.Point1.Y - LineSegment1.Point2.Y == 0) && (LineSegment2.Point1.Y - LineSegment2.Point2.Y == 0))
            {
                if (LineSegment1.Point1.Y != LineSegment2.Point1.Y)                                     //если они не лежат на одном Y
                    return false;
                if (maxXSegment1 >= minXSegment2 && minXSegment1 <= maxXSegment2)               //проверим пересекаются ли они, т.е. есть ли у них общий Y
                {
                    double maxXSegments = Math.Min(maxXSegment1, maxXSegment2);
                    double minXSegments = Math.Max(minXSegment1, minXSegment2);
                    if (maxXSegments == minXSegments)
                        pointsIntersection = new Segment { Point1 = new Point { X = minXSegments, Y = LineSegment1.Point1.Y } };
                    else
                        pointsIntersection = new Segment
                        {
                            Point1 = new Point { X = minXSegments, Y = LineSegment1.Point1.Y },
                            Point2 = new Point { X = maxXSegments, Y = LineSegment1.Point1.Y }
                        };
                    return true;
                };
            }

            //коэффициенты уравнений, содержащих отрезки
            //f1(x) = Angle1*x + b1 = y
            //f2(x) = Angle2*x + b2 = y

            Angle2 = (LineSegment2.Point1.Y - LineSegment2.Point2.Y) / (LineSegment2.Point1.X - LineSegment2.Point2.X);

            //если только первый отрезок вертикальный
            if (LineSegment1.Point1.X - LineSegment1.Point2.X == 0)
            {
                //найдём Xi, Yi - точки пересечения двух прямых
                intersectionX = LineSegment1.Point1.X;
                b2 = LineSegment2.Point1.Y - Angle2 * LineSegment2.Point1.X;
                intersectionY = Angle2 * intersectionX + b2;

                if (LineSegment2.Point1.X <= intersectionX 
                    && LineSegment2.Point2.X >= intersectionX 
                    && Math.Min(LineSegment1.Point1.Y, LineSegment1.Point2.Y) <= intersectionY 
                    && Math.Max(LineSegment1.Point1.Y, LineSegment1.Point2.Y) >= intersectionY)
                {
                    pointsIntersection = new Segment { Point1 = new Point { X = intersectionX, Y = intersectionY } };
                    return true;
                }
                return false;
            }

            Angle1 = (LineSegment1.Point1.Y - LineSegment1.Point2.Y) / (LineSegment1.Point1.X - LineSegment1.Point2.X);

            //если только второй отрезок вертикальный
            if (LineSegment2.Point1.X - LineSegment2.Point2.X == 0)
            {
                //найдём Xi, Yi - точки пересечения двух прямых
                intersectionX = LineSegment2.Point1.X;
                b1 = LineSegment1.Point1.Y - Angle1 * LineSegment1.Point1.X;
                intersectionY = Angle1 * intersectionX + b1;

                if (LineSegment1.Point1.X <= intersectionX 
                    && LineSegment1.Point2.X >= intersectionX 
                    && Math.Min(LineSegment2.Point1.Y, LineSegment2.Point2.Y) <= intersectionY 
                    && Math.Max(LineSegment2.Point1.Y, LineSegment2.Point2.Y) >= intersectionY)
                {
                    pointsIntersection = new Segment { Point1 = new Point { X = intersectionX, Y = intersectionY } };
                    return true;
                }
                return false;
            }

            //////////////////////////////////////////
            //Проверка остальных случаев пересечения//
            //////////////////////////////////////////

            //оба отрезка невертикальные и негоризонтальные
            if (Angle1 == Angle2)                                                           //отрезки параллельны
            {
                double A1_A = (LineSegment1.Point1.Y - LineSegment2.Point1.Y) / (LineSegment1.Point1.X - LineSegment2.Point1.X);
                if (Angle1 != A1_A)
                    return false;
                if (LineSegment1.Point1.Y > LineSegment1.Point2.Y)                                                //если наклон отрезка => \ 
                {
                    pointsIntersection = new Segment
                    {
                        Point1 = new Point
                        {
                            X = Math.Max(minXSegment1, minXSegment2),
                            Y = Math.Min(maxYSegment1, maxYSegment2)
                        },
                        Point2 = new Point
                        {
                            X = Math.Min(maxXSegment1, maxXSegment2),
                            Y = Math.Max(minYSegment1, minYSegment2)
                        }
                    };
                    return true;
                }
                if (LineSegment1.Point1.Y < LineSegment1.Point2.Y)                                                //если наклон отрезка => /
                {
                    pointsIntersection = new Segment
                    {
                        Point1 = new Point
                        {
                            X = Math.Max(minXSegment1, minXSegment2),
                            Y = Math.Max(minYSegment1, minYSegment2)
                        },
                        Point2 = new Point
                        {
                            X = Math.Min(maxXSegment1, maxXSegment2),
                            Y = Math.Min(maxYSegment1, maxYSegment2)
                        }
                    };
                    return true;
                }
            }

            b1 = LineSegment1.Point1.Y - Angle1 * LineSegment1.Point1.X;
            b2 = LineSegment2.Point1.Y - Angle2 * LineSegment2.Point1.X;

            intersectionX = (b2 - b1) / (Angle1 - Angle2);
            if ((intersectionX < Math.Max(LineSegment1.Point1.X, LineSegment2.Point1.X)) || (intersectionX > Math.Min(LineSegment1.Point2.X, LineSegment2.Point2.X)))
            {
                return false;
            }
            //если произвольные непараллельные отрезки
            double f11 = LineSegment1.Point2.Y - LineSegment1.Point1.Y;
            double f21 = LineSegment1.Point1.X - LineSegment1.Point2.X;
            double f31 = -LineSegment1.Point1.X * (LineSegment1.Point2.Y - LineSegment1.Point1.Y) + LineSegment1.Point1.Y * (LineSegment1.Point2.X - LineSegment1.Point1.X);

            double f12 = LineSegment2.Point2.Y - LineSegment2.Point1.Y;
            double f22 = LineSegment2.Point1.X - LineSegment2.Point2.X;
            double f32 = -LineSegment2.Point1.X * (LineSegment2.Point2.Y - LineSegment2.Point1.Y) + LineSegment2.Point1.Y * (LineSegment2.Point2.X - LineSegment2.Point1.X);

            double d = f11 * f22 - f21 * f12;

            pointsIntersection = new Segment { Point1 = new Point { X = (-f31 * f22 + f21 * f32) / d, Y = (-f11 * f32 + f31 * f12) / d } };
            return true;
        }

        private static Point[] SplitSegmentsIntoPoints(List<Segment> segments)
        {
            List<Point> pointsL = new List<Point>();
            foreach (Segment seg in segments)
            {
                pointsL.Add(seg.Point1);
                if (seg.Point2 != null)
                    pointsL.Add(seg.Point2);
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