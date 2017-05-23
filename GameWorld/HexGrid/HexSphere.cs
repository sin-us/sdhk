using GameWorld.Gen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGameWorld.HexGrid
{
    public class HexSphere
    {
        private HexLib.HexSphereGenerator.Grid _sphereGrid;
        private Perlin3D _perlin;

        private double _minNoise = double.MaxValue;
        private double _maxNoise = double.MinValue;

        private const int K = 15;
        private const int D = 30;

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

        private Color GetTileColor(HexLib.HexSphereGenerator.Tile t)
        {
            var val = _perlin.getMultioctave3DNoiseValue(t.V.X * 50, t.V.Y * 50, t.V.Z * 50, 1, 6);
            val = (val - _minNoise) / (_maxNoise - _minNoise);

            float fVal = (float)val;

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

            return color;
        }

        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, SpriteBatch spriteBatch, SpriteFont generalFont)
        {
            var vertices = new List<VertexPositionColorTexture>();

            foreach (var t in _sphereGrid.Tiles)
            {
                var color = GetTileColor(t);

                for (int j = 0; j < t.Corners.Length - 2; ++j)
                {
                    var v1 = new VertexPositionColorTexture();
                    var v2 = new VertexPositionColorTexture();
                    var v3 = new VertexPositionColorTexture();


                    v1.Position = new Vector3((float)t.Corners[j + 2].V.X * K, (float)t.Corners[j + 2].V.Y * K, (float)t.Corners[j + 2].V.Z * K);
                    v2.Position = new Vector3((float)t.Corners[j + 1].V.X * K, (float)t.Corners[j + 1].V.Y * K, (float)t.Corners[j + 1].V.Z * K);
                    v3.Position = new Vector3((float)t.Corners[0].V.X * K, (float)t.Corners[0].V.Y * K, (float)t.Corners[0].V.Z * K);

                    v1.Color = color;
                    v2.Color = color;
                    v3.Color = color;

                    vertices.Add(v1);
                    vertices.Add(v2);
                    vertices.Add(v3);
                }

                
            }

            foreach (var t in _sphereGrid.Tiles)
            {
                var color = GetTileColor(t);
                var val = _perlin.getMultioctave3DNoiseValue(t.V.X * 50, t.V.Y * 50, t.V.Z * 50, 1, 6);
                val = (val - _minNoise) / (_maxNoise - _minNoise);

                float fVal = (float)val;

                for (int i = 0; i < t.Corners.Length; ++i)
                {
                    var a1 = new Vector3((float)t.Corners[i].V.X * K, (float)t.Corners[i].V.Y * K, (float)t.Corners[i].V.Z * K);
                    var a2 = new Vector3((float)t.Corners[(i + 1) % t.Corners.Length].V.X * K, (float)t.Corners[(i + 1) % t.Corners.Length].V.Y * K, (float)t.Corners[(i + 1) % t.Corners.Length].V.Z * K);

                    var b1 = new Vector3((float)t.Corners[i].V.X * (K + fVal * D), (float)t.Corners[i].V.Y * (K + fVal * D), (float)t.Corners[i].V.Z * (K + fVal * D));
                    var b2 = new Vector3((float)t.Corners[(i + 1) % t.Corners.Length].V.X * (K + fVal * D), (float)t.Corners[(i + 1) % t.Corners.Length].V.Y * (K + fVal * D), (float)t.Corners[(i + 1) % t.Corners.Length].V.Z * (K + fVal * D));

                    var v1 = new VertexPositionColorTexture();
                    var v2 = new VertexPositionColorTexture();
                    var v3 = new VertexPositionColorTexture();
                    var v4 = new VertexPositionColorTexture();
                    var v5 = new VertexPositionColorTexture();
                    var v6 = new VertexPositionColorTexture();


                    v1.Position = b1;
                    v2.Position = a2;
                    v3.Position = a1;

                    v4.Position = b1;
                    v5.Position = b2;
                    v6.Position = a2;

                    v1.Color = color;
                    v2.Color = color;
                    v3.Color = color;

                    v4.Color = color;
                    v5.Color = color;
                    v6.Color = color;

                    vertices.Add(v1);
                    vertices.Add(v2);
                    vertices.Add(v3);
                    vertices.Add(v4);
                    vertices.Add(v5);
                    vertices.Add(v6);
                }
            }

            foreach (var t in _sphereGrid.Tiles)
            {
                var color = GetTileColor(t);
                var val = _perlin.getMultioctave3DNoiseValue(t.V.X * 50, t.V.Y * 50, t.V.Z * 50, 1, 6);
                val = (val - _minNoise) / (_maxNoise - _minNoise);

                float fVal = (float)val;

                for (int j = 0; j < t.Corners.Length - 2; ++j)
                {
                    var rv1 = new VertexPositionColorTexture();
                    var rv2 = new VertexPositionColorTexture();
                    var rv3 = new VertexPositionColorTexture();

                    rv1.Position = new Vector3((float)t.Corners[j + 2].V.X * (K + fVal * D), (float)t.Corners[j + 2].V.Y * (K + fVal * D), (float)t.Corners[j + 2].V.Z * (K + fVal * D));
                    rv2.Position = new Vector3((float)t.Corners[j + 1].V.X * (K + fVal * D), (float)t.Corners[j + 1].V.Y * (K + fVal * D), (float)t.Corners[j + 1].V.Z * (K + fVal * D));
                    rv3.Position = new Vector3((float)t.Corners[0].V.X * (K + fVal * D), (float)t.Corners[0].V.Y * (K + fVal * D), (float)t.Corners[0].V.Z * (K + fVal * D));

                    rv1.Color = color;
                    rv2.Color = color;
                    rv3.Color = color;

                    vertices.Add(rv1);
                    vertices.Add(rv2);
                    vertices.Add(rv3);
                }
            }

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count / 3);
            }
        }
    }
}
