using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Noise
{
    public static class Perlin3D
    {
        private const int permutationTableSize = 1024;

        //permutation table
        private static byte[] permutationTable;

        //pseudorandom hash modifiers
        private static int mX;
        private static int mY;
        private static int mZ;

        // gradients' set
        private static List<Vector3D> gradientSet;

        static Perlin3D()
        {
            permutationTable = new byte[permutationTableSize];
            gradientSet = new List<Vector3D>();

            // fill the gradients' set
            for (int x = -1; x <= 1; ++x) // from -1 to 1
            {
                for (int y = -1; y <= 1; ++y)
                {
                    for (int z = -1; z <= 1; ++z)
                    {
                        if ((x != 0) || (y != 0) || (z != 0))
                        {
                            gradientSet.Add(new Vector3D(x, y, z));
                        }
                    }
                }
            }

            setSeed();
        }

        public static void setSeed(int? seed = null)
        {
            Random rand = ((seed == null) ? new Random() : new Random((int)seed));

            rand.NextBytes(permutationTable);

            mX = rand.Next();
            mY = rand.Next();
            mZ = rand.Next();
        }

        private static Vector3D getGradient(int x, int y, int z)
        {
            // pick random cell in permutation table (cells 0 to 'permutationTableSize')
            int index = (int)((x * mX) ^ (y * mY) + z * mZ + (mX * mY * mZ)) & (permutationTableSize - 1);
            // pick random cell in gradientSet vector
            index = permutationTable[index] & (gradientSet.Count - 1);

            // return the content of the picked cell
            return gradientSet[index];
        }

        private static int fastFloor(double d)
        {
            int res = (int)d;

            if (d < 0)
            {
                if (res != d)
                {
                    --res;
                }
            }

            return res;
        }

        private static double fastPow(double value, uint pow)
        {
            double powOfValue = 1;

            for (uint i = 0; i < pow; ++i)
            {
                powOfValue *= value;
            }

            return powOfValue;
        }

        private static double blendingCurve(double d)
        {
            return (d * d * d * (d * (d * 6.0 - 15.0) + 10.0));
        }

        private static double interpolation(double a, double b, double t)
        {
            return ((1.0 - t) * a + t * b);
        }

        public static double get3DNoiseValue(double x, double y, double z)
        {
            // find unit grid cell containing point
            int floorX = fastFloor(x);
            int floorY = fastFloor(y);
            int floorZ = fastFloor(z);

            // get relative XYZ coordinates of point in cell
            double relX = x - floorX;
            double relY = y - floorY;
            double relZ = z - floorZ;

            //gradients of cube vertices
            Vector3D g000 = getGradient(floorX, floorY, floorZ);
            Vector3D g001 = getGradient(floorX, floorY, floorZ + 1);
            Vector3D g010 = getGradient(floorX, floorY + 1, floorZ);
            Vector3D g011 = getGradient(floorX, floorY + 1, floorZ + 1);
            Vector3D g100 = getGradient(floorX + 1, floorY, floorZ);
            Vector3D g101 = getGradient(floorX + 1, floorY, floorZ + 1);
            Vector3D g110 = getGradient(floorX + 1, floorY + 1, floorZ);
            Vector3D g111 = getGradient(floorX + 1, floorY + 1, floorZ + 1);

            // noise contribution from each of the eight corner
            double n000 = Vector3D.DotProduct(g000, new Vector3D(relX, relY, relZ));
            double n100 = Vector3D.DotProduct(g100, new Vector3D(relX - 1, relY, relZ));
            double n010 = Vector3D.DotProduct(g010, new Vector3D(relX, relY - 1, relZ));
            double n110 = Vector3D.DotProduct(g110, new Vector3D(relX - 1, relY - 1, relZ));
            double n001 = Vector3D.DotProduct(g001, new Vector3D(relX, relY, relZ - 1));
            double n101 = Vector3D.DotProduct(g101, new Vector3D(relX - 1, relY, relZ - 1));
            double n011 = Vector3D.DotProduct(g011, new Vector3D(relX, relY - 1, relZ - 1));
            double n111 = Vector3D.DotProduct(g111, new Vector3D(relX - 1, relY - 1, relZ - 1));

            // compute the fade curve value for each x, y, z
            double u = blendingCurve(relX);
            double v = blendingCurve(relY);
            double w = blendingCurve(relZ);

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

        public static double getMultioctave3DNoiseValue(double x, double y, double z, uint startOctaveNumber, uint octaveCount, double persistence)
        {
            double total = 0;
            double frequency = fastPow(2, startOctaveNumber);
            double amplitude = fastPow(persistence, startOctaveNumber);

            for (uint i = startOctaveNumber; i < (startOctaveNumber + octaveCount); ++i)
            {
                total += (amplitude * get3DNoiseValue(x / frequency, y / frequency, z / frequency));

                frequency *= 2;
                amplitude *= persistence;
            }
            return total;
        }
    }
}