using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameWorld.Particles2D
{
	public class ParticleEngine2D
	{
		private Random random;
		private List<Particle2D> particles;
		private List<Texture2D> textures;

		public Vector2 EmitterLocation { get; set; }
		public int ParticlesPerEmition { get; set; }
		public Vector2 VelocityMultipliers { get; set; }
		public float AngularVelocityMultiplier { get; set; }
		public delegate Color ColorPickerHandler();
		public ColorPickerHandler ColorPicker { get; set; }
		public Color Color { get; set; }
		public float SizeMultiplier { get; set; }
		public float SizeVelocityMultiplier { get; set; }
		public int TimeToLiveBase { get; set; }
		public int TimeToLiveRandomRange { get; set; }

		public ParticleEngine2D(List<Texture2D> textures, Vector2 location)
		{
			EmitterLocation = location;
			this.textures = textures;
			particles = new List<Particle2D>();
			random = new Random();

			ParticlesPerEmition = 10;
			VelocityMultipliers = new Vector2(1.0f, 1.0f); //0.7
			AngularVelocityMultiplier = 1.0f; //0.1
			ColorPicker = null;
			Color = Color.White;
			SizeMultiplier = 1.0f; //0.5
			SizeVelocityMultiplier = 0.0f;
			TimeToLiveBase = 20;
			TimeToLiveRandomRange = 40;
		}

		private Particle2D GenerateNewParticle()
		{
			Texture2D texture = textures[random.Next(textures.Count)];
			Vector2 position = EmitterLocation;
			Vector2 velocity = new Vector2(
					  VelocityMultipliers.X * (float)(random.NextDouble() * 2 - 1),
					  VelocityMultipliers.Y * (float)(random.NextDouble() * 2 - 1));
			float angle = 0;
			float angularVelocity = AngularVelocityMultiplier * (float)(random.NextDouble() * 2 - 1);

			if(ColorPicker != null)
			{
				Color = ColorPicker();
			}

			Color color = Color;

			float size = SizeMultiplier * (float)random.NextDouble();
			float sizeVelocity = SizeVelocityMultiplier * (float)random.NextDouble();
			int ttl = TimeToLiveBase + random.Next(TimeToLiveRandomRange);

			return new Particle2D(texture, position, velocity, angle, angularVelocity, color, size, sizeVelocity, ttl);
		}

		public void Update()
		{
			for (int i = 0; i < ParticlesPerEmition; ++i)
			{
				particles.Add(GenerateNewParticle());
			}

			for (int i = 0; i < particles.Count; ++i)
			{
				particles[i].Update();
				if (particles[i].TTL <= 0)
				{
					particles.RemoveAt(i);
					--i;
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch, BlendState state, SpriteSortMode mode = SpriteSortMode.Deferred)
		{
			spriteBatch.Begin(mode, state);
			foreach(Particle2D particle in particles)
			{
				particle.Draw(spriteBatch);
			}
			spriteBatch.End();
		}
	}
}
