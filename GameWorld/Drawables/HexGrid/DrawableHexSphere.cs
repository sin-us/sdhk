using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameWorld.Drawables;
using MonoGameWorld.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGameWorld.HexGrid
{
    public class DrawableHexSphere : CustomHexSphere
    {
        public Vector3 Position { get; set; }

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
                        _vertices[v].Color = GetColorByHeight(_selectedTile.Height);
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

        private GraphicsDeviceManager _graphics;
        public BasicEffect Effect { get; private set; }

        private VertexPositionColorNormal[] _vertices;
        private IntersectionCheckNode _intersectionChecker;

        public DrawableHexSphere(GraphicsDeviceManager graphics, int size, int radius = 30, int groundHeight = 8) : base(size)
        {
            _graphics = graphics;

            Position = Vector3.Zero;
            AxisRotation = Vector3.Zero;
            Effect = new BasicEffect(graphics.GraphicsDevice);

            Vector3 lightVector = new Vector3(1.0f, 0, 0);
            LightDirection = lightVector;

            Effect.VertexColorEnabled = true;
            Effect.World = WorldMatrix;
            Effect.LightingEnabled = true; 
            Effect.DirectionalLight0.DiffuseColor = new Vector3(0.7f, 0.7f, 0.7f);
            Effect.DirectionalLight0.Direction = lightVector * -1; // Should be inverted because calculations in CustomTile and in BasicEffect are done differently
            Effect.AmbientLightColor = new Vector3(0.05f, 0.05f, 0.05f);

            Radius = radius;
            GroundHeight = groundHeight;

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

        public void Update(GameTime gameTime, Vector3 cameraOffset)
        {
            Update(gameTime);
            WorldMatrix = Matrix.CreateFromQuaternion(AxisRotationQuaternion) * Matrix.CreateTranslation(Position - cameraOffset);
        }

        public void Draw(Matrix projection, Matrix view)
        {
            Effect.Projection = projection;
            Effect.View = view;
            Effect.World = WorldMatrix;

            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, _vertices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
            }
        }

        private Color GetColorByHeight(double height)
        {
            float fVal = (float)height;

            // Ground
            if (height > WaterHeight)
            {
                if (height > 0.75)
                {
                    // Mountain
                    return new Color(fVal, fVal, fVal);
                }
                else
                {
                    // Grass
                    return new Color(0.0f, fVal, 0.0f);
                }
            }
            else
            {
                // Water
                return new Color(0.0f , 0.0f, fVal);
            }
        }

        private VertexPositionColorNormal CreateVertex(Vector3 position, Vector3 norm, double height)
        {
            return new VertexPositionColorNormal(position, norm, GetColorByHeight(height));
        }

        private VertexPositionColorNormal CreateVertex(Vector3 position, CustomTile t)
        {
            Vector3 tileNorm = Vector3.Zero;
            Vector3 tileCenter = new Vector3(t.X, t.Y, t.Z);
            for (int i = 0; i < t.Corners.Length - 1; ++i)
            {
                Vector3 a = new Vector3(t.Corners[i].X, t.Corners[i].Y, t.Corners[i].Z);
                Vector3 b = new Vector3(t.Corners[i + 1].X, t.Corners[i + 1].Y, t.Corners[i + 1].Z);

                tileNorm += ((tileCenter - a) * (tileCenter - b));
            }

            tileNorm.Normalize();

            return new VertexPositionColorNormal(position, tileCenter, GetColorByHeight(t.Height));
        }

        private void InitializeVertices()
        {
            var vertices = new List<VertexPositionColorNormal>();


            foreach (var t in Tiles)
            {
                int startingVerticesLength = vertices.Count;

                // "0-level" tiles (bottom)
                for (int j = 0; j < t.Corners.Length - 2; ++j)
                {
                    Vector3 a = new Vector3(t.Corners[j + 2].X * Radius, t.Corners[j + 2].Y * Radius, t.Corners[j + 2].Z * Radius);
                    Vector3 b = new Vector3(t.Corners[j + 1].X * Radius, t.Corners[j + 1].Y * Radius, t.Corners[j + 1].Z * Radius);
                    Vector3 c = new Vector3(t.Corners[0].X * Radius, t.Corners[0].Y * Radius, t.Corners[0].Z * Radius);

                    Vector3 normA = t.Corners[j + 2].V.ToVector3();
                    Vector3 normB = t.Corners[j + 1].V.ToVector3();
                    Vector3 normC = t.Corners[0].V.ToVector3();

                    vertices.Add(CreateVertex(a, normA, t.Height));
                    vertices.Add(CreateVertex(b, normB, t.Height));
                    vertices.Add(CreateVertex(c, normC, t.Height));
                }

                // "Raised" tiles (top)
                float height = t.IsWater ? (float)(Radius + WaterHeight * GroundHeight) : (float)(Radius + t.Height * GroundHeight);
                for (int j = 0; j < t.Corners.Length - 2; ++j)
                {
                    vertices.Add(CreateVertex(new Vector3(t.Corners[j + 2].X * height, t.Corners[j + 2].Y * height, t.Corners[j + 2].Z * height), t));
                    vertices.Add(CreateVertex(new Vector3(t.Corners[j + 1].X * height, t.Corners[j + 1].Y * height, t.Corners[j + 1].Z * height), t));
                    vertices.Add(CreateVertex(new Vector3(t.Corners[0].X * height, t.Corners[0].Y * height, t.Corners[0].Z * height), t));
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

                    var normal = (a2 - b1) * (a2 - a1);
                    normal = new Vector3(t.X, t.Y, t.Z);
                    normal.Normalize();

                    vertices.Add(CreateVertex(b1, normal, t.Height));
                    vertices.Add(CreateVertex(a2, normal, t.Height));
                    vertices.Add(CreateVertex(a1, normal, t.Height));

                    vertices.Add(CreateVertex(b1, normal, t.Height));
                    vertices.Add(CreateVertex(b2, normal, t.Height));
                    vertices.Add(CreateVertex(a2, normal, t.Height));
                }


                t.VerticeIndices = Enumerable.Repeat(startingVerticesLength, vertices.Count - startingVerticesLength).Select((value, index) => value + index).ToArray();
                t.TopHexVerticeIndices = t.VerticeIndices.Skip((t.Corners.Length - 2) * 3).Take((t.Corners.Length - 2) * 3).ToArray();
                t.BoundingBox = BoundingBox.CreateFromPoints(t.TopHexVerticeIndices.Select(vi => vertices[vi].Position));
            }

            _intersectionChecker = IntersectionCheckNode.CreateFromTiles(Tiles, Radius + GroundHeight);
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
