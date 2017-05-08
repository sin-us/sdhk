using HexLib;
using MonoGameWorld.Utilities;
using MonoGameWorld.Inputs.Keyboard;
using MonoGameWorld.Inputs.Mouse;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameWorld.HexGrid;
using System;
using System.Windows.Forms;
using MonoGameWorld.Camera;

namespace GameWorld
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Camera camera;
        private SpriteFont generalFont;
        private Model model;
        private Matrix modelWorld;        
        private BasicEffect effect;
        private HexGrid _hexGrid;
        private HexSphere _hexSphere;

        private Vector3 modelPosition;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
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
            modelPosition = new Vector3(0, 2.0f, 0);
            camera = new Camera(new Vector3(0, 0, 10.0f), Vector3.Forward, Vector3.UnitY, 45.0f, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0.01f, 100000000.0f);

            MouseManager.IsPointerVisible = true;
            this.IsMouseVisible = MouseManager.IsPointerVisible;
            MouseManager.LoadCustomCursor(@"Content\AnimatedCursor.ani");
            Form winForm = (Form)Form.FromHandle(this.Window.Handle);
            winForm.Cursor = MouseManager.CustomCursor;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            effect = new BasicEffect(graphics.GraphicsDevice);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // set up font
            generalFont = Content.Load<SpriteFont>("GeneralFont");

            model = Content.Load<Model>("lava_cube");

            _hexGrid = new HexGrid(Content.Load<Texture2D>("HexGridTileset"), 50, 100, 87);
            _hexSphere = new HexSphere(5);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				this.Exit();

            // TODO: Add your update logic here

            // update frame rate
            FrameRateCounter.Update(gameTime);
            KeyboardManager.Update();
            MouseManager.Update();

            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
            {
                camera.MoveRelativeZ(-0.1f);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
            {
                camera.MoveRelativeZ(0.1f);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
            {
                camera.MoveRelativeX(-0.1f);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
            {
                camera.MoveRelativeX(0.1f);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
            {
                camera.MoveRelativeY(-0.1f);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                camera.MoveRelativeY(0.1f);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Q))
            {
                camera.RotateRelativeZ(-1.0f);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E))
            {
                camera.RotateRelativeZ(1.0f);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                camera.RotateRelativeX(-1.0f);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                camera.RotateRelativeX(1.0f);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                camera.RotateRelativeY(-1.0f);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                camera.RotateRelativeY(1.0f);
            }

            camera.Update();
            modelWorld = Matrix.CreateTranslation(modelPosition - camera.Offset);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            // TODO: Add your drawing code here

            // Draw any meshes before the text in order for it to be on the top
            DrawModel(model, modelWorld, camera.ViewMatrix, camera.ProjectionMatrix);

            // Show FPS
            ++FrameRateCounter.FrameCounter;
            spriteBatch.Begin();
            spriteBatch.DrawString(generalFont, "FPS: " + FrameRateCounter.FrameRate.ToString(), new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(generalFont, "W / A / S / D: forward / left / backwards / right\nCtrl / Space: down / up\nQ / E: tilt left / right\nArrows: look along X / Y axis", new Vector2(10, 30), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.PreferPerPixelLighting = true;
                    effect.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
                    effect.DirectionalLight0.Direction = new Vector3(1.0f, 0, 0);
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.7f, 0.7f, 0.7f);
                    effect.FogEnabled = true;
                    effect.FogColor = Color.DarkGray.ToVector3();
                    effect.FogStart = 8.0f;
                    effect.FogEnd = 50.0f;
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }
    }
}