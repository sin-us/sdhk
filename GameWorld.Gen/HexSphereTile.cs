using GameWorld.Gen.HexSphereGenerator;
using VectorD = MathNet.Numerics.LinearAlgebra.Double.DenseVector;


namespace GameWorld.Gen
{
    public abstract class HexSphereTile
    {
        public int Id { get; internal set; }
        public int EdgeCount { get; internal set; }
        public VectorD TileCenterPosition { get; internal set; }
        public HexSphereTile[] Tiles { get; internal set; }
        public HexSphereTileCorner[] Corners { get; internal set; }

        internal Edge[] Edges { get; set; }

        public float X => (float)TileCenterPosition[0];
        public float Y => (float)TileCenterPosition[1];
        public float Z => (float)TileCenterPosition[2];

        internal void Initialize(int id, int edge_count)
        {
            Id = id;
            EdgeCount = edge_count;

            Tiles = new HexSphereTile[edge_count];
            Corners = new HexSphereTileCorner[edge_count];
            Edges = new Edge[edge_count];
        }


        internal int GetTilePosition(HexSphereTile n)
        {
            for (int i = 0; i < EdgeCount; i++)
                if (Tiles[i] == n)
                    return i;
            return -1;
        }

        internal int GetCornerPosition(HexSphereTileCorner c)
        {
            for (int i = 0; i < EdgeCount; i++)
                if (Corners[i] == c)
                    return i;
            return -1;
        }

        internal int GetEdgePosition(Edge e)
        {
            for (int i = 0; i < EdgeCount; i++)
                if (Edges[i] == e)
                    return i;
            return -1;
        }
    }
}
