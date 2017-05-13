using System;
using System.Collections.Generic;

using VectorD = MathNet.Numerics.LinearAlgebra.Double.DenseVector;

namespace GameWorld.Gen
{
    public class Perlin3D
    {
        private const int permutationTableSize = 1024;

        //permutation table
        private byte[] permutationTable;

        //pseudorandom hash modifiers
        private int mX;
        private int mY;
        private int mZ;

        // gradients' set
        private static List<VectorD> gradientSet;

        public Perlin3D(int? seed = null)
        {
            permutationTable = new byte[permutationTableSize];
            gradientSet = new List<VectorD>();

            // fill the gradients' set
            for (int x = -1; x <= 1; ++x) // from -1 to 1
            {
                for (int y = -1; y <= 1; ++y)
                {
                    for (int z = -1; z <= 1; ++z)
                    {
                        if ((x != 0) || (y != 0) || (z != 0))
                        {
                            gradientSet.Add(new VectorD(new double[] { x, y, z }));
                        }
                    }
                }
            }

            Random rand = (seed == null) ? new Random() : new Random((int)seed);

            rand.NextBytes(permutationTable);

            mX = rand.Next();
            mY = rand.Next();
            mZ = rand.Next();
        }

        private VectorD getGradient(int x, int y, int z)
        {
            // pick random cell in permutation table (cells 0 to 'permutationTableSize')
            int index = ((x * mX) ^ (y * mY) + z * mZ + (mX * mY * mZ)) & (permutationTableSize - 1);
            // pick random cell in gradientSet vector
            index = permutationTable[index] & (gradientSet.Count - 1);

            // return the content of the picked cell
            return gradientSet[index];
        }

        private static float FastPow(float value, uint pow)
        {
            float powOfValue = 1;

            for (uint i = 0; i < pow; ++i)
            {
                powOfValue *= value;
            }

            return powOfValue;
        }

        private static double BlendingCurve(double d)
        {
            return (d * d * d * (d * (d * 6.0 - 15.0) + 10.0));
        }

        private static double interpolation(double a, double b, double t)
        {
            return ((1.0 - t) * a + t * b);
        }

        public double get3DNoiseValue(double x, double y, double z)
        {
            // find unit grid cell containing point
            int floorX = (int)Math.Floor(x);
            int floorY = (int)Math.Floor(y);
            int floorZ = (int)Math.Floor(z);

            // get relative XYZ coordinates of point in cell
            double relX = x - floorX;
            double relY = y - floorY;
            double relZ = z - floorZ;

            //gradients of cube vertices
            VectorD g000 = getGradient(floorX, floorY, floorZ);
            VectorD g001 = getGradient(floorX, floorY, floorZ + 1);
            VectorD g010 = getGradient(floorX, floorY + 1, floorZ);
            VectorD g011 = getGradient(floorX, floorY + 1, floorZ + 1);
            VectorD g100 = getGradient(floorX + 1, floorY, floorZ);
            VectorD g101 = getGradient(floorX + 1, floorY, floorZ + 1);
            VectorD g110 = getGradient(floorX + 1, floorY + 1, floorZ);
            VectorD g111 = getGradient(floorX + 1, floorY + 1, floorZ + 1);

            // noise contribution from each of the eight corner
            double n000 = g000 * new VectorD(new[] { relX, relY, relZ });
            double n100 = g100 * new VectorD(new[] { relX - 1, relY, relZ });
            double n010 = g010 * new VectorD(new[] { relX, relY - 1, relZ });
            double n110 = g110 * new VectorD(new[] { relX - 1, relY - 1, relZ });
            double n001 = g001 * new VectorD(new[] { relX, relY, relZ - 1 });
            double n101 = g101 * new VectorD(new[] { relX - 1, relY, relZ - 1 });
            double n011 = g011 * new VectorD(new[] { relX, relY - 1, relZ - 1 });
            double n111 = g111 * new VectorD(new[] { relX - 1, relY - 1, relZ - 1 });

            // compute the fade curve value for each x, y, z
            double u = BlendingCurve(relX);
            double v = BlendingCurve(relY);
            double w = BlendingCurve(relZ);

            // interpolate along x the contribution from each of the corners
            double nx00 = interpolation(n000, n100, u);
            double nx01 = interpolation(n001, n101, u);
            double nx10 = interpolation(n010, n110, u);
            double nx11 = interpolation(n011, n111, u);

            // interpolate the four results along y
            double nxy0 = interpolation(nx00, nx10, v);
            double nxy1 = interpolation(nx01, nx11, v);

            // interpolate the two last results along z
            double nxyz = interpolation(nxy0, nxy1, w);

            return nxyz;
        }

        public double getMultioctave3DNoiseValue(double x, double y, double z, uint startOctaveNumber, uint octaveCount)
        {
            double res = 0;
            for (uint i = startOctaveNumber; i < (startOctaveNumber + octaveCount); ++i)
            {
                var powOf2 = FastPow(2, i);

                res += (powOf2 * get3DNoiseValue(x / powOf2, y / powOf2, z / powOf2));
            }
            return res;
        }

        public double getMultioctave3DNoiseValueFromSphere(int x, int y, int z, uint startOctaveNumber, uint octaveCount, uint radius)
        {
            // convert to sphere coordinates
            double d = FastPow(x, 2) + FastPow(y, 2) + FastPow(z, 2);

            d = Math.Sqrt(d);

            double zd = z / d;

            double theta = Math.Acos(zd);
            double phi = Math.Atan2(y, x);

            double s_x = radius * Math.Sin(theta) * Math.Cos(phi);
            double s_y = radius * Math.Sin(theta) * Math.Sin(phi);
            double s_z = radius * Math.Cos(theta);

            return getMultioctave3DNoiseValue(s_x, s_y, s_z, startOctaveNumber, octaveCount);
        }
    }
}