using GameWorld.Gen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGameWorld.HexGrid
{
    public class HexSphere
    {
        private HexLib.HexSphereGenerator.Grid _sphereGrid;
        private Perlin3D _perlin;

        private double _minNoise = double.MaxValue;
        private double _maxNoise = double.MinValue;

        private const int K = 15;

        public HexSphere(int size)
        {
            _sphereGrid = HexLib.HexSphereGenerator.Grid.CreateSizeNGrid(size);

            _perlin = new Perlin3D(new Random().Next());

            foreach (var t in _sphereGrid.Tiles)
            {
                var val = _perlin.getMultioctave3DNoiseValue(t.V.X * K, t.V.Y * K, t.V.Z * K, 1, 7);

                _minNoise = Math.Min(_minNoise, val);
                _maxNoise = Math.Max(_maxNoise, val);
            }

        }

        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect)
        {
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[12];

            foreach (var t in _sphereGrid.Tiles)
            {
                var val = _perlin.getMultioctave3DNoiseValue(t.V.X * 50, t.V.Y * 50, t.V.Z * 50, 1, 6);
                val = (val - _minNoise) / (_maxNoise - _minNoise);

                Color color;
                // Ground
                if (val > 0.5)
                {
                    if (val > 0.75)
                    {
                        // Mountain
                        color = new Color((float)val, (float)val, (float)val);
                    }
                    else
                    {
                        // Grass
                        color = new Color(0, (float)val, 0);
                    }

                }
                else
                {
                    // Water
                    color = new Color(0, 0, (float)val);
                }

                for (int j = 0; j < t.Corners.Length - 2; ++j)
                {
                    vertices[j * 3].Position = new Vector3((float)t.Corners[0].V.X * K, (float)t.Corners[0].V.Y * K, (float)t.Corners[0].V.Z * K);
                    vertices[j * 3 + 1].Position = new Vector3((float)t.Corners[j + 1].V.X * K, (float)t.Corners[j + 1].V.Y * K, (float)t.Corners[j + 1].V.Z * K);
                    vertices[j * 3 + 2].Position = new Vector3((float)t.Corners[j + 2].V.X * K, (float)t.Corners[j + 2].V.Y * K, (float)t.Corners[j + 2].V.Z * K);

                    vertices[j * 3].Color = color;
                    vertices[j * 3 + 1].Color = color;
                    vertices[j * 3 + 2].Color = color;
                }

                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, t.Corners.Length - 2);
                }
            }
        }
    }
}
