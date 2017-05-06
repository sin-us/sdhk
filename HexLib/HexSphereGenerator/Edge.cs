namespace HexLib.HexSphereGenerator
{
    public class Edge
    {
        public int Id { get; }
        public Tile[] Tiles { get; } = new Tile[2];
        public Corner[] Corners { get; } = new Corner[2];

        public Edge(int id)
        {
            Id = id;
        }
    }
}
