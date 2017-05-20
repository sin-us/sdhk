using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoGameWorld.Utilities
{
    public static class FrameRateCounter
    {
        private static TimeSpan elapsedTime;

        public static int FrameRate { get; set; }
        public static int FrameCounter { get; set; }
        public static float FrameTime { get; private set; }

        static FrameRateCounter()
        {
            elapsedTime = TimeSpan.Zero;

            FrameRate = 0;
            FrameCounter = 0;
            FrameTime = 0.0f;
        }

        public static void Update(GameTime gameTime)
        {
            FrameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                FrameRate = FrameCounter;
                FrameCounter = 0;
            }
        }
    }
}