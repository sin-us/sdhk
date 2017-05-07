namespace HexLib.HexSphereGenerator
{
    public class Corner
    {
        public int Id { get; }
        public Vector3 V { get; set; }
        public Tile[] Tiles { get; } = new Tile[3];
        public Corner[] Corners { get; } = new Corner[3];
        public Edge[] Edges { get; } = new Edge[3];

        public Corner(int id)
        {
            Id = id;
        }

        public int GetTilePosition(Tile t)
        {
	        for (int i=0; i<3; i++)
		        if (Tiles[i] == t)
			        return i;
	        return -1;
        }

        public int GetCornerPosition(Corner n)
        {
            for (int i = 0; i < 3; i++)
                if (Corners[i] == n)
                    return i;
            return -1;
        }

        public int GetEdgePosition(Edge e)
        {
            for (int i = 0; i < 3; i++)
                if (Edges[i] == e)
                    return i;
            return -1;
        }
    }
}
