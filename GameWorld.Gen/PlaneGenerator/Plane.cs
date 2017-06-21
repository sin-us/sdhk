using System.Collections.Generic;

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
        public List<int> Indices { get; private set; }
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
            Indices = new List<int>();

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
                    Indices.Add(i);
                    Indices.Add(i + widthInVertices);
                    Indices.Add(i + 1);

                    // second triangle in pair
                    Indices.Add(i + 1);
                    Indices.Add(i + widthInVertices);
                    Indices.Add(i + widthInVertices + 1);
                }
            }
        }
    }
}
