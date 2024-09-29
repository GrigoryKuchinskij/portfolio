using System;
using System.Collections.Generic;
using System.Text;
using static PolygonConsoleApp.GeometricObjects;

namespace PolygonConsoleApp
{
    internal class GeometricConversions
    {
        public static Point[] ConvertToPoint(List<string[]> pointsList)
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

        public static Segment[] ConvertToSegment(List<string[]> segmentsStringsList)
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
                        Point1 = new Point { X = XBegSeg, Y = YBegSeg },
                        Point2 = new Point { X = XEndSeg, Y = YEndSeg }
                    });
                }
            }
            return segmentsList.ToArray();
        }
    }
}
