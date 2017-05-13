using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameWorld.Gen
{
    public class PerlinRectangle
    {
        private List<double[,]> rawtile;
        private int size;
        private int pow;
        private const int plainCnt = 6;

        private Perlin3D perlin;

        public PerlinRectangle(int? seed, int pow)
        {
            rawtile = new List<double[,]>();
            size = (int)Math.Pow(2, pow) + 1;
            int half_size = (size - 1) / 2;

            for (int i = 0; i < plainCnt; ++i)
            {
                rawtile.Add(new double[size, size]);
            }

            perlin = new Perlin3D(seed);

            Parallel.For(0, plainCnt, index => { CalculateNoise(half_size, index); });

            normalizeArray();
        }

        private void CalculateNoise(int halfSize, int index)
        {
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    // center of cube is moved to the center of coordinates and its coords transformed to sphere coords
                    /*
                    *    5
                    * 1  2  3  4
                    *    6
                    */
                    switch (index)
                    {
                        case 0:
                            rawtile[0][i, j] = perlin.getMultioctave3DNoiseValueFromSphere(0 - halfSize, halfSize - j, i - halfSize, 1, (uint)pow - 1, (uint)halfSize);
                            break;
                        case 1:
                            rawtile[1][i, j] = perlin.getMultioctave3DNoiseValueFromSphere(j - halfSize, 0 - halfSize, i - halfSize, 1, (uint)pow - 1, (uint)halfSize);
                            break;
                        case 2:
                            rawtile[2][i, j] = perlin.getMultioctave3DNoiseValueFromSphere(halfSize, j - halfSize, i - halfSize, 1, (uint)pow - 1, (uint)halfSize);
                            break;
                        case 3:
                            rawtile[3][i, j] = perlin.getMultioctave3DNoiseValueFromSphere(halfSize - j, halfSize, i - halfSize, 1, (uint)pow - 1, (uint)halfSize);
                            break;
                        case 4:
                            rawtile[4][i, j] = perlin.getMultioctave3DNoiseValueFromSphere(j - halfSize, i - halfSize, halfSize, 1, (uint)pow - 1, (uint)halfSize);
                            break;
                        case 5:
                            rawtile[5][i, j] = perlin.getMultioctave3DNoiseValueFromSphere(j - halfSize, halfSize - i, 0 - halfSize, 1, (uint)pow - 1, (uint)halfSize);
                            break;
                    }
                }
            }
        }

        private double minNoise()
        {
            double min = rawtile[0][0, 0];

            for (int a = 0; a < plainCnt; ++a)
            {
                for (int i = 0; i < size; ++i)
                {
                    for (int j = 0; j < size; ++j)
                    {
                        if (min > rawtile[a][i, j])
                        {
                            min = rawtile[a][i, j];
                        }
                    }
                }
            }

            return min;
        }

        private double maxNoise()
        {
            double max = rawtile[0][0, 0];

            for (int a = 0; a < plainCnt; ++a)
            {
                for (int i = 0; i < size; ++i)
                {
                    for (int j = 0; j < size; ++j)
                    {
                        if (max < rawtile[a][i, j])
                        {
                            max = rawtile[a][i, j];
                        }
                    }
                }
            }

            return max;
        }

        private void normalizeArray()
        {
            double min = minNoise();
            double max = maxNoise();

            for (int a = 0; a < plainCnt; ++a)
            {
                for (int i = 0; i < size; ++i)
                {
                    for (int j = 0; j < size; ++j)
                    {
                        rawtile[a][i, j] = ((rawtile[a][i, j] - min) / (max - min));
                    }
                }
            }
        }
    }
}
