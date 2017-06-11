using GameWorld.Gen;
using Microsoft.Xna.Framework;
using MonoGameWorld.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace MonoGameWorld.HexGrid
{
    public class CustomTile : HexSphereTile
    {
        public double Height { get; set; }

        public Color Color { get; set; }

        public bool IsWater { get; set; }

        public float Brightness
        {
            get
            {
                float brightness = Vector3.Dot(Vector3.TransformNormal(TileCenterPosition.ToVector3(), ParentSphere.WorldMatrix), ParentSphere.LightDirection);
                return brightness < 0.0f ? 0.0f : brightness;
            }
        }

        // TODO: Add restrictions. Should be only set in constructor
        public CustomHexSphere ParentSphere { get; set; }

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
