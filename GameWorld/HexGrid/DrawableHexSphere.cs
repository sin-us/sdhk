using GameWorld.Gen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameWorld.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        private int desiredWaterCoveragePercent = 30;

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

        private CustomTile _selectedTile;
        public CustomTile SelectedTile
        {
            get { return _selectedTile; }
            private set
            {
                if (_selectedTile != null)
                {
                    foreach (var v in _selectedTile.VerticeIndices)
                    {
                        _vertices[v].Color = _selectedTile.Color;
                    }
                }

                _selectedTile = value;

                if (_selectedTile != null)
                {
                    foreach (var v in _selectedTile.VerticeIndices)
                    {
                        _vertices[v].Color = Color.Red;
                    }
                }
            }
        }

        public BasicEffect Effect { get; set; }

        private VertexPositionColorTexture[] _vertices;
        private IntersectionCheckNode _intersectionChecker;

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

        public void CheckIntersection(ref Ray ray, out int intersectChecksCount)
        {
            SelectedTile = null;

            var candidates = _intersectionChecker.GetIntersectCandidates(ref ray, out intersectChecksCount);

            if (candidates.Count > 0)
            {
                foreach (var t in candidates.OrderBy(c => c.Distance).Select(c => c.Tile))
                {
                    bool found = false;
                    for (int i = 0; i < t.TopHexVerticeIndices.Length - 2; ++i)
                    {
                        intersectChecksCount++;

                        if (Mathematics.RayIntersectsTriangle(
                            ref ray, 
                            ref _vertices[ t.TopHexVerticeIndices[i] ].Position, 
                            ref _vertices[ t.TopHexVerticeIndices[i + 1] ].Position,
                            ref _vertices[ t.TopHexVerticeIndices[i + 2] ].Position, 
                            out float distance))
                        {
                            SelectedTile = t;
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        break;
                    }
                }
            }
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

            if (coveragePercent < 0 || coveragePercent > 100)
            {
                resultHeight = 0.5f;
            }
            else if (coveragePercent == 0)
            {
                resultHeight = 0.0f;
            }
            else if (coveragePercent == 100)
            {
                resultHeight = 1.0f;
            }
            else
            {
                int index = (tiles_heights.Count * coveragePercent / 100) - 1;
                if (index > 0)
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
                int startingVerticesLength = vertices.Count;

                // "0-level" tiles (bottom)
                for (int j = 0; j < t.Corners.Length - 2; ++j)
                {
                    vertices.Add(new VertexPositionColorTexture { Position = new Vector3(t.Corners[j + 2].X * Radius, t.Corners[j + 2].Y * Radius, t.Corners[j + 2].Z * Radius), Color = t.Color });
                    vertices.Add(new VertexPositionColorTexture { Position = new Vector3(t.Corners[j + 1].X * Radius, t.Corners[j + 1].Y * Radius, t.Corners[j + 1].Z * Radius), Color = t.Color });
                    vertices.Add(new VertexPositionColorTexture { Position = new Vector3(t.Corners[0].X * Radius, t.Corners[0].Y * Radius, t.Corners[0].Z * Radius), Color = t.Color });
                }

                // TODO: Remove code duplication
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

                t.VerticeIndices = Enumerable.Repeat(startingVerticesLength, vertices.Count - startingVerticesLength).Select((value, index) => value + index).ToArray();
                t.TopHexVerticeIndices = t.VerticeIndices.Skip((t.Corners.Length - 2) * 3).Take((t.Corners.Length - 2) * 3).ToArray();
                t.BoundingBox = BoundingBox.CreateFromPoints(t.TopHexVerticeIndices.Select(vi => vertices[vi].Position));
            }


            _intersectionChecker = IntersectionCheckNode.CreateFromTiles(_sphereGrid.Tiles, Radius + GroundHeight);
            _vertices = vertices.ToArray();
        }

        private class CustomTileIntersection
        {
            public CustomTile Tile { get; }

            public float Distance { get; }

            public CustomTileIntersection(CustomTile tile, float distance)
            {
                Tile = tile;
                Distance = distance;
            }
        }

        private class IntersectionCheckNode
        {
            private const int MaxDepth = 20;

            public IntersectionCheckNode Left { get; private set; }

            public IntersectionCheckNode Right { get; private set; }

            public BoundingBox BoundingBox { get; private set; }

            public List<CustomTile> Tiles { get; private set; }

            public int? GetTotalTilesCount()
            {
                if (Tiles != null)
                {
                    return Tiles?.Count;
                }

                return (Left == null ? 0 : Left.GetTotalTilesCount()) + (Right == null ? 0 : Right.GetTotalTilesCount());
            }

            public List<CustomTileIntersection> GetIntersectCandidates(ref Ray ray, out int intersectChecksCount)
            {
                List<CustomTileIntersection> result = new List<CustomTileIntersection>();
                intersectChecksCount = 0;

                GetIntersectCandidates(ref ray, ref result, ref intersectChecksCount);

                return result;
            }

            private void GetIntersectCandidates(ref Ray ray, ref List<CustomTileIntersection> candidates, ref int intersectChecksCount)
            {
                if (Tiles != null)
                {
                    foreach (var t in Tiles)
                    {
                        float? distance = ray.Intersects(t.BoundingBox);

                        if (distance.HasValue)
                        {
                            candidates.Add(new CustomTileIntersection(t, distance.Value));
                        }
                    }
                }
                else
                {
                    float? distLarger = Left != null ? ray.Intersects(Left.BoundingBox) : null;
                    float? distSmaller = Right != null ? ray.Intersects(Right.BoundingBox) : null;

                    if (distLarger != null && distSmaller != null)
                    {
                        intersectChecksCount += 2;
                        Left.GetIntersectCandidates(ref ray, ref candidates, ref intersectChecksCount);
                        Right.GetIntersectCandidates(ref ray, ref candidates, ref intersectChecksCount);
                    }
                    else if (distLarger != null)
                    {
                        intersectChecksCount++;
                        Left.GetIntersectCandidates(ref ray, ref candidates, ref intersectChecksCount);
                    }
                    else if (distSmaller != null)
                    {
                        intersectChecksCount++;
                        Right.GetIntersectCandidates(ref ray, ref candidates, ref intersectChecksCount);
                    }
                }
            }

            private void AddTile(CustomTile tile, int currentDepth, ref BoundingBox box)
            {
                BoundingBox = box;

                if (currentDepth >= MaxDepth)
                {
                    if (Tiles == null) { Tiles = new List<CustomTile>(); }

                    Tiles.Add(tile);
                }
                else
                {
                    var dividedBoxes = DivideBoundingBox(ref box, currentDepth);

                    var leftBoundingBox = dividedBoxes.Item1;
                    var rightBoundingBox = dividedBoxes.Item2;

                    var leftContainment = leftBoundingBox.Contains(tile.BoundingBox);
                    var rightContainment = rightBoundingBox.Contains(tile.BoundingBox);

                    if (leftContainment == ContainmentType.Contains || leftContainment == ContainmentType.Intersects)
                    {
                        if (Left == null) { Left = new IntersectionCheckNode(); }
                        Left.AddTile(tile, currentDepth + 1, ref leftBoundingBox);
                    }

                    if (rightContainment == ContainmentType.Contains || rightContainment == ContainmentType.Intersects)
                    {
                        if (Right == null) { Right = new IntersectionCheckNode(); }
                        Right.AddTile(tile, currentDepth + 1, ref rightBoundingBox);
                    }
                }
            }

            private static Tuple<BoundingBox, BoundingBox> DivideBoundingBox(ref BoundingBox box, int currentDepth)
            {
                switch (currentDepth % 3)
                {
                    case 0:
                    default:
                        return DivideBoundingBoxByX(ref box);
                    case 1:
                        return DivideBoundingBoxByY(ref box);
                    case 2:
                        return DivideBoundingBoxByZ(ref box);
                }
            }

            private static Tuple<BoundingBox, BoundingBox> DivideBoundingBoxByX(ref BoundingBox box)
            {
                float newSize = (box.Max.X - box.Min.X) / 2.0f;

                return Tuple.Create(
                    new BoundingBox(box.Min, new Vector3(box.Max.X - newSize, box.Max.Y, box.Max.Z)),
                    new BoundingBox(new Vector3(box.Min.X + newSize, box.Min.Y, box.Min.Z), box.Max));
            }

            private static Tuple<BoundingBox, BoundingBox> DivideBoundingBoxByY(ref BoundingBox box)
            {
                float newSize = (box.Max.Y - box.Min.Y) / 2.0f;

                return Tuple.Create(
                    new BoundingBox(box.Min, new Vector3(box.Max.X, box.Max.Y - newSize, box.Max.Z)),
                    new BoundingBox(new Vector3(box.Min.X, box.Min.Y + newSize, box.Min.Z), box.Max));
            }

            private static Tuple<BoundingBox, BoundingBox> DivideBoundingBoxByZ(ref BoundingBox box)
            {
                float newSize = (box.Max.Z - box.Min.Z) / 2.0f;

                return Tuple.Create(
                    new BoundingBox(box.Min, new Vector3(box.Max.X, box.Max.Y, box.Max.Z - newSize)),
                    new BoundingBox(new Vector3(box.Min.X, box.Min.Y, box.Min.Z + newSize), box.Max));
            }

            public static IntersectionCheckNode CreateFromTiles(ICollection<CustomTile> tiles, float maxRadius)
            {
                IntersectionCheckNode result = new IntersectionCheckNode();

                var initialBoundingBox = new BoundingBox(new Vector3(-maxRadius, -maxRadius, -maxRadius), new Vector3(maxRadius, maxRadius, maxRadius));

                foreach (CustomTile t in tiles)
                {
                    result.AddTile(t, 0, ref initialBoundingBox);
                }

                return result;
            }
        }
    }
}
