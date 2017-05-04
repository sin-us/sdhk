using HexLib;
using GameWorld.Animation2D;
using GameWorld.Particles2D;
using GameWorld.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameWorld.HexGrid;
using System;
using System.Collections.Generic;

namespace GameWorld
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
	{
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private TimeSpan elapsedTime;
		private float locationDelta;
		private float locationEpsilon;

		private SpriteFont generalFont;
		private AnimatedSprite animatedSprite;
		private ParticleEngine2D particleEngine;

        private HexGrid _hexGrid;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			locationDelta = 2.0f;
			locationEpsilon = 4.0f;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here

			// set up font
			generalFont = Content.Load<SpriteFont>("GeneralFont");

			// set up animated sprite
			Texture2D textureAtlas = Content.Load<Texture2D>("TextureAtlasExample");

			Dictionary<string, List<FramePosition>> animationBindings = new Dictionary<string, List<FramePosition>>();
			List<FramePosition> frameList = new List<FramePosition>();

			frameList = new List<FramePosition>();
			frameList.Add(new FramePosition(0, 0));
			frameList.Add(new FramePosition(1, 0));
			frameList.Add(new FramePosition(2, 0));
			frameList.Add(new FramePosition(3, 0));			
			animationBindings.Add("down", frameList);

			frameList = new List<FramePosition>();
			frameList.Add(new FramePosition(0, 1));
			frameList.Add(new FramePosition(1, 1));
			frameList.Add(new FramePosition(2, 1));
			frameList.Add(new FramePosition(3, 1));
			animationBindings.Add("left", frameList);

			frameList = new List<FramePosition>();
			frameList.Add(new FramePosition(0, 2));
			frameList.Add(new FramePosition(1, 2));
			frameList.Add(new FramePosition(2, 2));
			frameList.Add(new FramePosition(3, 2));
			animationBindings.Add("right", frameList);

			frameList = new List<FramePosition>();
			frameList.Add(new FramePosition(0, 3));
			frameList.Add(new FramePosition(1, 3));
			frameList.Add(new FramePosition(2, 3));
			frameList.Add(new FramePosition(3, 3));
			animationBindings.Add("up", frameList);

			animatedSprite = new AnimatedSprite(textureAtlas, 4, 4, animationBindings);
			animatedSprite.AnimationName = "right";
			animatedSprite.Location = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);

			// set up particle engine
			List<Texture2D> textureList = new List<Texture2D>();
			textureList.Add(Content.Load<Texture2D>("Sparkle"));

			particleEngine = new ParticleEngine2D(textureList, new Vector2(400, 240));
			particleEngine.ParticlesPerEmition = 3;
			particleEngine.VelocityMultipliers = new Vector2(0.7f, 0.7f);
			particleEngine.AngularVelocityMultiplier = 0.1f;
			particleEngine.ColorPicker = PickColor;
			particleEngine.SizeMultiplier = 0.5f;
			particleEngine.SizeVelocityMultiplier = 0.0f;
			particleEngine.TimeToLiveBase = 20;
			particleEngine.TimeToLiveRandomRange = 50;


            _hexGrid = new HexGrid(Content.Load<Texture2D>("HexGridTileset"), 50, 100, 87);
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			// TODO: Add your update logic here

			// update frame rate
			FrameRateCounter.Update(gameTime);

			// get mouse position
			Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

			// update animated sprite
			elapsedTime += gameTime.ElapsedGameTime;

			if (elapsedTime > TimeSpan.FromSeconds(0.2))
			{
				elapsedTime -= TimeSpan.FromSeconds(0.2);
				animatedSprite.Update();
			}

			if (Math.Abs(animatedSprite.Location.X - mousePosition.X) > locationEpsilon)
			{
				if (animatedSprite.Location.X < mousePosition.X)
				{
					animatedSprite.Location = new Vector2(animatedSprite.Location.X + locationDelta, animatedSprite.Location.Y);
					animatedSprite.AnimationName = "right";
				}
				else if (animatedSprite.Location.X > mousePosition.X)
				{
					animatedSprite.Location = new Vector2(animatedSprite.Location.X - locationDelta, animatedSprite.Location.Y);
					animatedSprite.AnimationName = "left";
				}
			}
			else if (Math.Abs(animatedSprite.Location.Y - mousePosition.Y) > locationEpsilon)
			{
			   if (animatedSprite.Location.Y < mousePosition.Y)
				{
					animatedSprite.Location = new Vector2(animatedSprite.Location.X, animatedSprite.Location.Y + locationDelta);
					animatedSprite.AnimationName = "down";
				}
				else if (animatedSprite.Location.Y > mousePosition.Y)
				{
					animatedSprite.Location = new Vector2(animatedSprite.Location.X, animatedSprite.Location.Y - locationDelta);
					animatedSprite.AnimationName = "up";
				}
			}

			// update particles engine
			particleEngine.EmitterLocation = mousePosition;
			particleEngine.Update();

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			// Show FPS
			++FrameRateCounter.FrameCounter;

            // Hex Grid
            spriteBatch.Begin();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    _hexGrid.RenderTile(new OffsetPoint(i, j), 0, 0, spriteBatch, Color.White, BlendState.AlphaBlend);
                }
            }
            spriteBatch.End();


            spriteBatch.Begin();
			spriteBatch.DrawString(generalFont, "FPS: " + FrameRateCounter.FrameRate.ToString(), new Vector2(10, 10), Color.White);
			spriteBatch.End();

            // show animated sprite
            animatedSprite.Draw(spriteBatch, Color.White, BlendState.AlphaBlend);

			// show particles
			particleEngine.Draw(spriteBatch, BlendState.Additive);

			base.Draw(gameTime);
		}

		private Color PickColor()
		{
			Random random = new Random();

			float randRG = (float)random.NextDouble();
			float randB = (float)random.NextDouble();

			return new Color(randRG, randRG, randB / 2.0f);
		}
	}
}
