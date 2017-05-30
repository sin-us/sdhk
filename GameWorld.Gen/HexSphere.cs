using GameWorld.Gen.HexSphereGenerator;

namespace GameWorld.Gen
{
    public class HexSphere<TTile, TCorner>
        where TTile : HexSphereTile, new()
        where TCorner : HexSphereTileCorner, new()
    {
        private Grid<TTile, TCorner> _grid;

        public TTile[] Tiles { get; }

        public TTile NorthPole { get; private set; }

        public TTile SouthPole { get; private set; }

        public HexSphere(int size)
        {
            _grid = Grid<TTile, TCorner>.CreateSizeNGrid(size);

            Tiles = _grid.Tiles;

            NorthPole = Tiles[0];
            SouthPole = Tiles[0];

            foreach (TTile t in Tiles)
            {
                if (t.Y < SouthPole.Y)
                {
                    SouthPole = t;
                }

                if (t.Y > NorthPole.Y)
                {
                    NorthPole = t;
                }
            }
        }
    }
}
