using System;
using System.Collections.Generic;

using VectorD = MathNet.Numerics.LinearAlgebra.Double.DenseVector;

namespace GameWorld.Gen
{
    /// <summary>
    /// Perlin3D implementation with static singleton. Static methods are not thread safe
    /// </summary>
    public class Perlin3D
    {
        private static Perlin3D _instance = new Perlin3D();

        public static Perlin3D Instance
        {
            get { return _instance; }
        }

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

            SetSeed(seed);
        }

        public void SetSeed(int? seed = null)
        {
            Random rand = ((seed == null) ? new Random() : new Random((int)seed));

            rand.NextBytes(permutationTable);

            mX = rand.Next();
            mY = rand.Next();
            mZ = rand.Next();
        }

        private VectorD GetGradient(int x, int y, int z)
        {
            // pick random cell in permutation table (cells 0 to 'permutationTableSize')
            int index = ((x * mX) ^ (y * mY) + z * mZ + (mX * mY * mZ)) & (permutationTableSize - 1);
            // pick random cell in gradientSet vector
            index = permutationTable[index] & (gradientSet.Count - 1);

            // return the content of the picked cell
            return gradientSet[index];
        }

        private double FastPow(double value, uint pow)
        {
            double powOfValue = 1;

            for (uint i = 0; i < pow; ++i)
            {
                powOfValue *= value;
            }

            return powOfValue;
        }

        private double BlendingCurve(double d)
        {
            return (d * d * d * (d * (d * 6.0 - 15.0) + 10.0));
        }

        private double Interpolation(double a, double b, double t)
        {
            return ((1.0 - t) * a + t * b);
        }

        public double Get3DNoiseValue(double x, double y, double z)
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
            VectorD g000 = GetGradient(floorX, floorY, floorZ);
            VectorD g001 = GetGradient(floorX, floorY, floorZ + 1);
            VectorD g010 = GetGradient(floorX, floorY + 1, floorZ);
            VectorD g011 = GetGradient(floorX, floorY + 1, floorZ + 1);
            VectorD g100 = GetGradient(floorX + 1, floorY, floorZ);
            VectorD g101 = GetGradient(floorX + 1, floorY, floorZ + 1);
            VectorD g110 = GetGradient(floorX + 1, floorY + 1, floorZ);
            VectorD g111 = GetGradient(floorX + 1, floorY + 1, floorZ + 1);

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
            double nx00 = Interpolation(n000, n100, u);
            double nx01 = Interpolation(n001, n101, u);
            double nx10 = Interpolation(n010, n110, u);
            double nx11 = Interpolation(n011, n111, u);

            // interpolate the four results along y
            double nxy0 = Interpolation(nx00, nx10, v);
            double nxy1 = Interpolation(nx01, nx11, v);

            // interpolate the two last results along z
            double nxyz = Interpolation(nxy0, nxy1, w);

            return nxyz;
        }

        public double GetMultioctave3DNoiseValue(double x, double y, double z, uint startOctaveNumber, uint octaveCount, double persistence)
        {
            double total = 0;
            double frequency = FastPow(2, startOctaveNumber);
            double amplitude = FastPow(persistence, startOctaveNumber);

            for (uint i = startOctaveNumber; i < (startOctaveNumber + octaveCount); ++i)
            {
                total += (amplitude * Get3DNoiseValue(x / frequency, y / frequency, z / frequency));

                frequency *= 2;
                amplitude *= persistence;
            }

            return total;
        }
    }
}