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
            public Point P1 { get; set; }
            public Point P2 { get; set; }
        }
    }
}
