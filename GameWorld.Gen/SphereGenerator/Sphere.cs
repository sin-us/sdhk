using System;
using System.Collections.Generic;
using System.Numerics;

namespace GameWorld.Gen.SphereGenerator
{
    public class VertexData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double TextureX { get; set; }
        public double TextureY { get; set; }
        public double NormalX { get; set; }
        public double NormalY { get; set; }
        public double NormalZ { get; set; }

        public VertexData(double x, double y, double z, double textureX, double textureY, double normalX, double normalY, double normalZ)
        {
            X = x;
            Y = y;
            Z = z;
            TextureX = textureX;
            TextureY = textureY;
            NormalX = normalX;
            NormalY = normalY;
            NormalZ = normalZ;
        }
    }

    public class Sphere
    {
        public List<VertexData> Vertices { get; private set; }
        public List<int> Indices { get; private set; }

        public Sphere(float diameter = 1.0f, int tessellation = 16, bool toRightHanded = true)
        {
            Vertices = new List<VertexData>();
            Indices = new List<int>();

            if (tessellation < 3) tessellation = 3;

            int verticalSegments = tessellation;
            int horizontalSegments = tessellation * 2;

            float radius = diameter / 2;

            // Create rings of vertices at progressively higher latitudes.
            for (int i = 0; i <= verticalSegments; i++)
            {
                float v = 1.0f - (float)i / verticalSegments;

                var latitude = (float)((i * Math.PI / verticalSegments) - Math.PI / 2.0);
                var dy = (float)Math.Sin(latitude);
                var dxz = (float)Math.Cos(latitude);

                // Create a single ring of vertices at this latitude.
                for (int j = 0; j <= horizontalSegments; j++)
                {
                    float u = (float)j / horizontalSegments;

                    var longitude = (float)(j * 2.0 * Math.PI / horizontalSegments);
                    var dx = (float)Math.Sin(longitude);
                    var dz = (float)Math.Cos(longitude);

                    dx *= dxz;
                    dz *= dxz;

                    double textureX = 1.0f - u;
                    double textureY = v;

                    Vertices.Add(new VertexData(dx * radius, dy * radius, dz * radius, textureX, textureY, dx, dy, dz));
                }
            }

            // Fill the index buffer with triangles joining each pair of latitude rings.
            int stride = horizontalSegments + 1;

            for (int i = 0; i < verticalSegments; i++)
            {
                for (int j = 0; j <= horizontalSegments; j++)
                {
                    int nextI = i + 1;
                    int nextJ = (j + 1) % stride;

                    Indices.Add(i * stride + j);
                    Indices.Add(i * stride + nextJ);
                    Indices.Add(nextI * stride + j);

                    Indices.Add(i * stride + nextJ);
                    Indices.Add(nextI * stride + nextJ);
                    Indices.Add(nextI * stride + j);
                }
            }

            if (toRightHanded)
            {
                ReverseWinding();
            }

            //CalculateNormals();
        }

        private void ReverseWinding()
        {
            for (int i = 0; i < Indices.Count; i += 3)
            {
                int Y = Indices[i + 1];
                int Z = Indices[i + 2];
                Indices[i + 1] = Z;
                Indices[i + 2] = Y;
            }
        }

        private void CalculateNormals()
        {
            Vector3[] normalsNew = new Vector3[Vertices.Count];

            for (int i = 0; i < Indices.Count; i += 3)
            {
                Vector3 vi = new Vector3((float)Vertices[Indices[i]].X, (float)Vertices[Indices[i]].Y, (float)Vertices[Indices[i]].Z);
                Vector3 vi1 = new Vector3((float)Vertices[Indices[i + 1]].X, (float)Vertices[Indices[i + 1]].Y, (float)Vertices[Indices[i + 1]].Z);
                Vector3 vi2 = new Vector3((float)Vertices[Indices[i + 2]].X, (float)Vertices[Indices[i + 2]].Y, (float)Vertices[Indices[i + 2]].Z);

                var v0 = vi1 - vi;
                var v1 = vi2 - vi;
                var n = Vector3.Cross(v0, v1);

                normalsNew[Indices[i]] += n;
                normalsNew[Indices[i + 1]] += n;
                normalsNew[Indices[i + 2]] += n;
            }

            for (int i = 0; i < normalsNew.Length; ++i)
            {
                Vector3 normal = Vector3.Normalize(normalsNew[i]);

                Vertices[i].NormalX = normal.X;
                Vertices[i].NormalY = normal.Y;
                Vertices[i].NormalZ = normal.Z;
            }
        }
    }
}