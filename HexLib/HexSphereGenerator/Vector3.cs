using System;

namespace HexLib.HexSphereGenerator
{
    public class Vector3
    {
        public double X;
        public double Y;
        public double Z;

        public Vector3() { }

        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3 GetNormal()
        {
            double d = 1.0f / GetLength();
	        return this * d;
        }

        public double GetLength()
        {
	        return Math.Sqrt(GetSquaredLength());
        }

        public double GetSquaredLength()
        {
            return X * X + Y * Y + Z * Z;
        }

        public static Vector3 operator+ (Vector3 v1, Vector3 v2)
        {
	        return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3 operator- (Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3 operator* (Vector3 v, double c)
        {
            return new Vector3(v.X * c, v.Y * c, v.Z * c);
        }
    }
}
