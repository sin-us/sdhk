using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameWorld.Animation2D
{
    public struct FramePosition
    {
        public int Row;
        public int Column;

        public FramePosition(int column, int row)
        {
            Row = row;
            Column = column;
        }
    };

    public class AnimatedSprite
    {
        private Texture2D texture;
        private int rows;
        private int columns;
        private int currentFrame;
        private int totalFrames;
        private int frameWidth;
        private int frameHeight;
        private string animationName;
        private List<FramePosition> frameList;
        private Dictionary<String, List<FramePosition>> animationBindings; // assuming that frames in atlas numerated top to bottom / left to right

        public Vector2 Location { get; set; }
        public String AnimationName
        {
            get
            {
                return animationName;
            }

            set
            {
                if ((value != animationName) || (IsRestartAnimation == true))
                {
                    if (animationBindings.TryGetValue(value, out frameList) == true)
                    {
                        totalFrames = frameList.Count;
                        currentFrame = 0;
                        animationName = value;
                    }
                }
            }
        }
        public bool IsRestartAnimation { get; set; } // states if animation should be restarted if the new name is the same as old (true), or just continue playing(false)

        public AnimatedSprite(Texture2D texture, int columns, int rows, Dictionary<string, List<FramePosition>> animationBindings)
        {
            this.texture = texture;
            this.rows = rows;
            this.columns = columns;
            currentFrame = 0;
            totalFrames = 0;
            frameWidth = texture.Width / columns;
            frameHeight = texture.Height / rows;
            animationName = "";
            frameList = new List<FramePosition>();
            this.animationBindings = animationBindings;

            Location = new Vector2(0.0f, 0.0f);
            IsRestartAnimation = false;
        }

        public void Update()
        {
            ++currentFrame;
            if (currentFrame == totalFrames)
                currentFrame = 0;
        }

        public void Draw(SpriteBatch spriteBatch, Color tintColor, BlendState state, SpriteSortMode mode = SpriteSortMode.Deferred)
        {
            int row = frameList[currentFrame].Row;
            int column = frameList[currentFrame].Column;

            Rectangle sourceRectangle = new Rectangle(frameWidth * column, frameHeight * row, frameWidth, frameHeight);
            Rectangle destinationRectangle = new Rectangle((int)Location.X - frameWidth / 2, (int)Location.Y - frameHeight / 2, frameWidth, frameHeight);

            spriteBatch.Begin(mode, state);
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, tintColor);
            spriteBatch.End();
        }
    }
}