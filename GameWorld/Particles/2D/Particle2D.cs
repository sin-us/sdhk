﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoGameWorld.Particles2D
{
    public class Particle2D
    {
        public Texture2D Texture { get; set; }          // The texture that will be drawn to represent the particle
        public Vector2 Position { get; set; }        // The current position of the particle        
        public Vector2 Velocity { get; set; }        // The speed of the particle at the current instance
        public float Angle { get; set; }                    // The current angle of rotation of the particle
        public float AngularVelocity { get; set; }  // The speed that the angle is changing
        public Color Color { get; set; }                    // The color of the particle
        public float Size { get; set; }                 // The size of the particle
        public float SizeVelocity { get; set; }     // The speed of size changing of the particle
        public int TTL { get; set; }                        // The 'time to live' of the particle

        public Particle2D(Texture2D texture, Vector2 position, Vector2 velocity, float angle, float angularVelocity, Color color, float size, float sizeVelocity, int ttl)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Color = color;
            Size = size;
            SizeVelocity = sizeVelocity;
            TTL = ttl;
        }

        public void Update()
        {
            --TTL;
            Position += Velocity;
            Angle += AngularVelocity;
            Size += SizeVelocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            spriteBatch.Draw(Texture, Position, sourceRectangle, Color, Angle, origin, Size, SpriteEffects.None, 0f);
        }
    }
}