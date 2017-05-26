namespace GameWorld.Gen.HexSphereGenerator
{
    internal class Edge
    {
        public int Id { get; }
        public HexSphereTile[] Tiles { get; } = new HexSphereTile[2];
        public HexSphereTileCorner[] Corners { get; } = new HexSphereTileCorner[2];

        public Edge(int id)
        {
            Id = id;
        }
    }
}
