namespace PolygonConsoleApp
{
    class GeometricObjects
    {
        public class Point
        {
            public double X { get; set; }
            public double Y { get; set; }
        }

        public class Segment
        {
            public Point Point1 { get; set; }
            public Point Point2 { get; set; }
        }
    }
}
