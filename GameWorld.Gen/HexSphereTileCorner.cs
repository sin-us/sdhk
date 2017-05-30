using GameWorld.Gen.HexSphereGenerator;
using VectorD = MathNet.Numerics.LinearAlgebra.Double.DenseVector;

namespace GameWorld.Gen
{
    public abstract class HexSphereTileCorner
    {
        public int Id { get; internal set; }
        public VectorD V { get; internal set; }
        public HexSphereTile[] Tiles { get; } = new HexSphereTile[3];
        public HexSphereTileCorner[] Corners { get; } = new HexSphereTileCorner[3];

        public float X => (float)V[0];
        public float Y => (float)V[1];
        public float Z => (float)V[2];

        internal Edge[] Edges { get; } = new Edge[3];

        internal int GetTilePosition(HexSphereTile t)
        {
            for (int i = 0; i < 3; i++)
                if (Tiles[i] == t)
                    return i;
            return -1;
        }

        internal int GetCornerPosition(HexSphereTileCorner n)
        {
            for (int i = 0; i < 3; i++)
                if (Corners[i] == n)
                    return i;
            return -1;
        }

        internal int GetEdgePosition(Edge e)
        {
            for (int i = 0; i < 3; i++)
                if (Edges[i] == e)
                    return i;
            return -1;
        }
    }
}
