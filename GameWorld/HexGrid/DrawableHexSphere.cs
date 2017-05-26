using GameWorld.Gen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonoGameWorld.HexGrid
{
    public class DrawableHexSphere
    {
        private HexSphere<CustomTile, CustomTileCorner> _sphereGrid;
        private Perlin3D _perlin;

        private double _minNoise = double.MaxValue;
        private double _maxNoise = double.MinValue;

        private const int PerlinCoefficient = 100;

        public int Radius { get; private set; }
        public int GroundHeight { get; private set; }

        private VertexPositionColorTexture[] _vertices;

        public DrawableHexSphere(int size, int radius = 30, int groundHeight = 2)
        {
            _sphereGrid = new HexSphere<CustomTile, CustomTileCorner>(size);

            Radius = radius;
            GroundHeight = groundHeight;

            _perlin = Perlin3D.Instance;

            foreach (var t in _sphereGrid.Tiles)
            {
                var val = _perlin.getMultioctave3DNoiseValue(t.X * PerlinCoefficient, t.Y * PerlinCoefficient, t.Z * PerlinCoefficient, 1, 7);

                _minNoise = Math.Min(_minNoise, val);
                _maxNoise = Math.Max(_maxNoise, val);
            }

            foreach (var t in _sphereGrid.Tiles)
            {
                var val = _perlin.getMultioctave3DNoiseValue(t.X * PerlinCoefficient, t.Y * PerlinCoefficient, t.Z * PerlinCoefficient, 1, 7);
                val = (val - _minNoise) / (_maxNoise - _minNoise);
                float fVal = (float)val;

                t.Height = val;

                // Ground
                if (val > 0.5)
                {
                    if (val > 0.75)
                    {
                        // Mountain
                        t.Color = new Color(fVal, fVal, fVal);
                    }
                    else
                    {
                        // Grass
                        t.Color = new Color(0, fVal, 0);
                    }
                }
                else
                {
                    // Water
                    t.IsWater = true;
                    t.Color = new Color(0, 0, fVal);
                }
            }

            InitializeVertices();
        }

        public void SetRadius(int radius)
        {
            Radius = radius;
            InitializeVertices();
        }

        public void SetGroundHeight(int groundHeight)
        {
            GroundHeight = groundHeight;
            InitializeVertices();
        }

        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect, SpriteBatch spriteBatch, SpriteFont generalFont)
        {
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, _vertices.Length / 3);
            }
        }

        private void InitializeVertices()
        {
            var vertices = new List<VertexPositionColorTexture>();

            foreach (var t in _sphereGrid.Tiles)
            {
                // "0-level" tiles (bottom)
                for (int j = 0; j < t.Corners.Length - 2; ++j)
                {
                    vertices.Add(new VertexPositionColorTexture { Position = new Vector3(t.Corners[j + 2].X * Radius, t.Corners[j + 2].Y * Radius, t.Corners[j + 2].Z * Radius), Color = t.Color });
                    vertices.Add(new VertexPositionColorTexture { Position = new Vector3(t.Corners[j + 1].X * Radius, t.Corners[j + 1].Y * Radius, t.Corners[j + 1].Z * Radius), Color = t.Color });
                    vertices.Add(new VertexPositionColorTexture { Position = new Vector3(t.Corners[0].X * Radius, t.Corners[0].Y * Radius, t.Corners[0].Z * Radius), Color = t.Color });
                }

                if (!t.IsWater)
                {
                    // "Raised" tiles (top)
                    float height = (float)(Radius + t.Height * GroundHeight);
                    for (int j = 0; j < t.Corners.Length - 2; ++j)
                    {
                        vertices.Add(new VertexPositionColorTexture { Position = new Vector3(t.Corners[j + 2].X * height, t.Corners[j + 2].Y * height, t.Corners[j + 2].Z * height), Color = t.Color });
                        vertices.Add(new VertexPositionColorTexture { Position = new Vector3(t.Corners[j + 1].X * height, t.Corners[j + 1].Y * height, t.Corners[j + 1].Z * height), Color = t.Color });
                        vertices.Add(new VertexPositionColorTexture { Position = new Vector3(t.Corners[0].X * height, t.Corners[0].Y * height, t.Corners[0].Z * height), Color = t.Color });
                    }

                    // Sides of raised tiles
                    for (int i = 0; i < t.Corners.Length; ++i)
                    {
                        // Two points of "0-level" edge
                        var a1 = new Vector3(t.Corners[i].X * Radius, t.Corners[i].Y * Radius, t.Corners[i].Z * Radius);
                        var a2 = new Vector3(t.Corners[(i + 1) % t.Corners.Length].X * Radius, t.Corners[(i + 1) % t.Corners.Length].Y * Radius, t.Corners[(i + 1) % t.Corners.Length].Z * Radius);

                        // Two points of "height-level" edge
                        var b1 = new Vector3(t.Corners[i].X * height, t.Corners[i].Y * height, t.Corners[i].Z * height);
                        var b2 = new Vector3(t.Corners[(i + 1) % t.Corners.Length].X * height, t.Corners[(i + 1) % t.Corners.Length].Y * height, t.Corners[(i + 1) % t.Corners.Length].Z * height);

                        vertices.Add(new VertexPositionColorTexture { Position = b1, Color = t.Color });
                        vertices.Add(new VertexPositionColorTexture { Position = a2, Color = t.Color });
                        vertices.Add(new VertexPositionColorTexture { Position = a1, Color = t.Color });

                        vertices.Add(new VertexPositionColorTexture { Position = b1, Color = t.Color });
                        vertices.Add(new VertexPositionColorTexture { Position = b2, Color = t.Color });
                        vertices.Add(new VertexPositionColorTexture { Position = a2, Color = t.Color });
                    }
                }


            }

            _vertices = vertices.ToArray();
        }
    }
}
