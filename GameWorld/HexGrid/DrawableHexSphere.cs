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

        private const int PerlinCoefficient = 80;

        private double waterHeight = 0.5f;
        private int desiredWaterCoveragePercent = 100;

        private GraphicsDeviceManager graphics;
        private Vector3 rotation;

        public Vector3 Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                RotationQuaternion = Quaternion.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
                RotationQuaternion.Normalize();
            }
        }

        public Vector3 Position { get; set; }
        public Quaternion RotationQuaternion { get; private set; }
        public Matrix World { get; private set; }

        public int Radius { get; private set; }
        public int GroundHeight { get; private set; }

        public BasicEffect Effect { get; set; }

        private VertexPositionColorTexture[] _vertices;

        public DrawableHexSphere(GraphicsDeviceManager graphics, int size, int radius = 30, int groundHeight = 8)
        {
            _sphereGrid = new HexSphere<CustomTile, CustomTileCorner>(size);

            this.graphics = graphics;

            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Effect = new BasicEffect(graphics.GraphicsDevice);

            Radius = radius;
            GroundHeight = groundHeight;

            _perlin = Perlin3D.Instance;

            foreach (var t in _sphereGrid.Tiles)
            {
                var val = _perlin.GetMultioctave3DNoiseValue(t.X * PerlinCoefficient, t.Y * PerlinCoefficient, t.Z * PerlinCoefficient, 1, 5, 1.5);

                _minNoise = Math.Min(_minNoise, val);
                _maxNoise = Math.Max(_maxNoise, val);
            }

            foreach (var t in _sphereGrid.Tiles)
            {
                var val = _perlin.GetMultioctave3DNoiseValue(t.X * PerlinCoefficient, t.Y * PerlinCoefficient, t.Z * PerlinCoefficient, 1, 5, 1.5);
                val = (val - _minNoise) / (_maxNoise - _minNoise);
                float fVal = (float)val;

                t.Height = val;
            }

            waterHeight = GetHeightFromCoveragePercent(desiredWaterCoveragePercent);

            foreach (var t in _sphereGrid.Tiles)//separated water detection from height calculation for water level calculation
            {
                float fVal = (float)t.Height;

                // Ground
                if (t.Height > waterHeight)
                {
                    if (t.Height > 0.75)
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

            _sphereGrid.NorthPole.Color = Color.Red;
            _sphereGrid.SouthPole.Color = Color.Red;

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

        public double GetHeightFromCoveragePercent(int coveragePercent)
        {
            double resultHeight = 0.5f;
            List<double> tiles_heights = new List<double>();

            foreach (var t in _sphereGrid.Tiles)
            {
                tiles_heights.Add(t.Height);
            }

            tiles_heights.Sort();

            if (0 > coveragePercent || 100 < coveragePercent)
            {
                resultHeight = 0.5f;
            }
            else if (0 == coveragePercent)
            {
                resultHeight = 0.0f;
            }
            else if (100 == coveragePercent)
            {
                resultHeight = 1.0f;
            }
            else
            {
                int index = (tiles_heights.Count * coveragePercent / 100) - 1;
                if (0 < index)
                {
                    resultHeight = tiles_heights[index];
                }
                else
                {
                    resultHeight = tiles_heights[0];
                }
            }

            return resultHeight;
        }

        public void Update(Vector3 cameraOffset)
        {
            World = Matrix.CreateFromQuaternion(RotationQuaternion) * Matrix.CreateTranslation(Position - cameraOffset);
        }

        public void Draw(Matrix projection, Matrix view)
        {
            Effect.Projection = projection;
            Effect.View = view;
            Effect.World = World;

            foreach (var pass in Effect.CurrentTechnique.Passes)
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
                else
                {
                    // "Raised" tiles (top)
                    float height = (float)(Radius + waterHeight * GroundHeight);
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
