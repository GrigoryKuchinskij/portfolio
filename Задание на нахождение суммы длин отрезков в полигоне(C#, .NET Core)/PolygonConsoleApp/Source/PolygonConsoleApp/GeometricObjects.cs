namespace PolygonConsoleApp
{
    class GeometricObjects
    {
        public class Point
        {
            private double x, y;
            public double X
            {
                get { return x; }
                set { x = value; }
            }
            public double Y
            {
                get { return y; }
                set { y = value; }
            }
        }

        public class Segment
        {
            private Point p1, p2;
            public Point P1
            {
                get { return p1; }
                set { p1 = value; }
            }
            public Point P2
            {
                get { return p2; }
                set { p2 = value; }
            }
        }
    }
}
