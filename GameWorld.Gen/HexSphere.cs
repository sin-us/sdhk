using GameWorld.Gen.HexSphereGenerator;

namespace GameWorld.Gen
{
    public class HexSphere<TTile, TCorner>
        where TTile : HexSphereTile, new()
        where TCorner: HexSphereTileCorner, new()
    {
        private Grid<TTile, TCorner> _grid;
        
        public TTile[] Tiles { get; }
        
        public HexSphere(int size)
        {
            _grid = Grid<TTile, TCorner>.CreateSizeNGrid(size);

            Tiles = _grid.Tiles;
        }
    }
}
