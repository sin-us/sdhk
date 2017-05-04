using System;

namespace HexLib
{
    public struct PointI
    {
        public int X { get; }
        public int Y { get; }

        public PointI(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public struct PointD
    {
        public double X { get; }
        public double Y { get; }

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public struct OffsetPoint
    {
        public int Q { get; }
        public int R { get; }

        public OffsetPoint(int q, int r)
        {
            Q = q;
            R = r;
        }

        public AxialPoint ToAxial()
        {
            return new AxialPoint(Q, R - (Q - (Q & 1)) / 2);
        }
    }

    public struct AxialPoint
    {
        public int X { get; }
        public int Z { get; }

        public AxialPoint(int x, int z)
        {
            X = x;
            Z = z;
        }

        public OffsetPoint ToOffset()
        {
            return new OffsetPoint(X, Z + (X - (X & 1)) / 2);
        }

        public static AxialPoint FromDouble(double x, double z)
        {
            return CubePoint.FromDouble(x, -x - z, z).ToAxial();
        }
}

    public struct CubePoint
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        public CubePoint(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public AxialPoint ToAxial()
        {
            return new AxialPoint(X, Z);
        }

        public static CubePoint FromDouble(double x, double y, double z)
        {
            double rx = Math.Round(x);
            double ry = Math.Round(y);
            double rz = Math.Round(z);

            double x_diff = Math.Abs(rx - x);
            double y_diff = Math.Abs(ry - y);
            double z_diff = Math.Abs(rz - z);

            if (x_diff > y_diff && x_diff > z_diff)
            {
                rx = -ry - rz;
            }
            else if (y_diff > z_diff)
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }

            return new CubePoint((int)rx, (int)ry, (int)rz);
        }
    }

    public class HexUtils
    {
        public static AxialPoint PixelToHex(PointI p, int hexSize)
        {
            double q = p.X * 2.0 / 3.0 / hexSize;
            double r = (-p.X / 3.0 + Math.Sqrt(3) / 3.0 * p.Y) / hexSize;

            return AxialPoint.FromDouble(q, r);
        }

        public static PointI HexToPixel(OffsetPoint hex, int hexSize)
        {
            int x = (int)(hexSize * 3.0 / 2.0 * hex.Q);
            int y = (int)(hexSize * Math.Sqrt(3) * (hex.R + 0.5 * (hex.Q & 1)));

            return new PointI(x, y);
        }
    }
}
