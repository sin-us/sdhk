using GameWorld.Gen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace MonoGameWorld.HexGrid
{
    public class HexGrid
    {
        public Texture2D HexTexture { get; }

        private int _hexSize;
        public int HexSize
        {
            get { return _hexSize; }
            set
            {
                _hexSize = value;
                HexWidth = HexSize * 2;
                HexHeight = (int)(Math.Sqrt(3) / 2 * HexWidth);
            }
        }

        public int HexWidth { get; private set; }
        public int HexHeight { get; private set; }

        public int HexTextureWidth { get; }
        public int HexTextureHeight { get; }

        public HexGrid(Texture2D texture, int hexSizePixels, int hexTextureWidth, int hexTextureHeight)
        {
            Debug.Assert(hexSizePixels > 0);

            HexTexture = texture;
            HexSize = hexSizePixels;

            HexTextureWidth = hexTextureWidth;
            HexTextureHeight = hexTextureHeight;
        }

        public void RenderTile(OffsetPoint pos, int tilesetRow, int tilesetCol, SpriteBatch spriteBatch, Color tintColor, BlendState state, SpriteSortMode mode = SpriteSortMode.Deferred)
        {
            int viewportLeft = 0;
            int viewportTop = 0;

            PointI p = HexUtils.HexToPixel(pos, HexSize);

            int x = p.X - HexWidth / 2;
            int y = p.Y - HexHeight / 2;

            Rectangle sourceRectangle = new Rectangle(tilesetCol * HexTextureWidth, tilesetRow * HexTextureHeight, HexTextureWidth, HexTextureHeight);
            Rectangle destinationRectangle = new Rectangle(x + viewportLeft, y + viewportTop, HexWidth, HexHeight);

            spriteBatch.Draw(HexTexture, destinationRectangle, sourceRectangle, tintColor);
        }
    }
}
