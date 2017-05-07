namespace HexLib.HexSphereGenerator
{
    public class Tile
    {
        public int Id { get; }
        public int EdgeCount { get; }
        public Vector3 V { get; set; }
        public Tile[] Tiles { get; }
        public Corner[] Corners { get; }
        public Edge[] Edges { get; }

        public Tile(int id, int edge_count)
        {
            Id = id;
            EdgeCount = edge_count;

            Tiles = new Tile[edge_count];
            Corners = new Corner[edge_count];
            Edges = new Edge[edge_count];
        }


        public int GetTilePosition(Tile n)
        {
            for (int i = 0; i < EdgeCount; i++)
                if (Tiles[i] == n)
                    return i;
            return -1;
        }

        public int GetCornerPosition(Corner c)
        {
            for (int i = 0; i < EdgeCount; i++)
                if (Corners[i] == c)
                    return i;
            return -1;
        }

        public int GetEdgePosition(Edge e)
        {
            for (int i = 0; i < EdgeCount; i++)
                if (Edges[i] == e)
                    return i;
            return -1;
        }
    }
}
