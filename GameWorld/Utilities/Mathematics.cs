using System;
using Microsoft.Xna.Framework;

namespace MonoGameWorld.Utilities
{
    public static class Mathematics
    {
        public const float ZeroTolerance = 1e-6f; // Value a 8x higher than 1.19209290E-07F

        public static bool IsZero(float a)
        {
            return Math.Abs(a) < ZeroTolerance;
        }

        public static bool IsOne(float a)
        {
            return IsZero(a - 1.0f);
        }

        public static bool IsQuaternionNormalized(Quaternion quaternion)
        {
            return IsOne((quaternion.X * quaternion.X) + (quaternion.Y * quaternion.Y) + (quaternion.Z * quaternion.Z) + (quaternion.W * quaternion.W));
        }
    }
}
