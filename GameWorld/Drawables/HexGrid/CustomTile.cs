using GameWorld.Gen;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MonoGameWorld.HexGrid
{
    public class CustomTile : HexSphereTile
    {
        public double Height { get; set; }

        public Color Color { get; set; }

        public bool IsWater { get; set; }

        public BoundingBox BoundingBox { get; set; }

        public int[] VerticeIndices { get; set; }

        public int[] TopHexVerticeIndices { get; set; }

        public IEnumerable<CustomTile> Neighbours
        {
            get
            {
                return Tiles.Cast<CustomTile>();
            }
        }
    }

    public class CustomTileCorner : HexSphereTileCorner
    {

    }
}
