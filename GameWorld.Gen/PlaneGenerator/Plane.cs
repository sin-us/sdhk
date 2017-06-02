using System.Collections.Generic;
using VectorD = MathNet.Numerics.LinearAlgebra.Double.DenseVector;

namespace GameWorld.Gen.PlaneGenerator
{
    public class VertexData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double TextureX { get; set; }
        public double TextureY { get; set; }

        public VertexData(double x, double y, double z, double textureX, double textureY)
        {
            X = x;
            Y = y;
            Z = z;
            TextureX = textureX;
            TextureY = textureY;
        }
    }

    public class Plane
    {
        public List<VertexData> Vertices { get; private set; }
        public List<int> Indeces { get; private set; }
        public List<VectorD> TextureCoordinates { get; set; }
        public int WidthInVertices { get; set; }
        public int HeightInVertices { get; set; }
        public float WidthStep { get; set; }
        public float HeightStep { get; set; }

        public Plane(int widthInVertices, int heightInVertices, float widthStep, float heightStep)
        {
            WidthInVertices = widthInVertices;
            HeightInVertices = heightInVertices;
            WidthStep = widthStep;
            HeightStep = heightStep;

            Vertices = new List<VertexData>();
            Indeces = new List<int>();

            for (int i = 0; i < (widthInVertices * heightInVertices); ++i)
            {
                int column = i % widthInVertices;
                int row = i / widthInVertices;

                double x = column * widthStep;
                double y = row * heightStep;
                double z = 0;

                double textureX = (double)column / (widthInVertices - 1);
                double textureY = (double)(heightInVertices - 1 - row) / (heightInVertices - 1);

                Vertices.Add(new VertexData(x, y, z, textureX, textureY));

                if ((column < (widthInVertices - 1)) && (i < (widthInVertices * (heightInVertices - 1))))
                {
                    // first triangle in pair
                    Indeces.Add(i);
                    Indeces.Add(i + widthInVertices);
                    Indeces.Add(i + 1);

                    // second triangle in pair
                    Indeces.Add(i + 1);
                    Indeces.Add(i + widthInVertices);
                    Indeces.Add(i + widthInVertices + 1);
                }
            }
        }
    }
}
