using GameWorld.Gen;
using Microsoft.Xna.Framework;

namespace MonoGameWorld.HexGrid
{
    public class CustomTile : HexSphereTile
    {
        public double Height { get; set; }

        public Color Color { get; set; }

        public bool IsWater { get; set; }
    }

    public class CustomTileCorner : HexSphereTileCorner
    {

    }
}
