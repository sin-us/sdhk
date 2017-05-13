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
using GameWorld.Shared;

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
        private BasicEffect sphereEffect;
        private HexGrid _hexGrid;
        private HexSphere _hexSphere;
        private bool isWireFrame;
        private Vector3 modelPosition;

        private ControlPanelListener _controlPanelListener;
        private string _customText = string.Empty;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.SynchronizeWithVerticalRetrace = true;
            this.IsFixedTimeStep = true;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
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

            isWireFrame = false;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            effect = new BasicEffect(graphics.GraphicsDevice);
            sphereEffect = new BasicEffect(graphics.GraphicsDevice);
            sphereEffect.VertexColorEnabled = true;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // set up font
            generalFont = Content.Load<SpriteFont>("GeneralFont");

            model = Content.Load<Model>("lava_cube");

            _hexGrid = new HexGrid(Content.Load<Texture2D>("HexGridTileset"), 50, 100, 87);
            _hexSphere = new HexSphere(6);

            _controlPanelListener = ControlPanelListener.Create();
            _controlPanelListener.OnSetText += val => _customText = val;
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
            // TODO: Add your update logic here

            FrameRateCounter.Update(gameTime);
            KeyboardManager.Update();
            MouseManager.Update();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                this.Exit();
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                this.Exit();
            }

            // Switch to/from wireframe mode
            if (KeyboardManager.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.P))
            {
                isWireFrame = !isWireFrame;
            }

            RasterizerState rasterizerState = new RasterizerState();
            if (isWireFrame == true)
            {                
                rasterizerState.FillMode = FillMode.WireFrame;
            }
            else
            {
                rasterizerState.FillMode = FillMode.Solid;
            }
            GraphicsDevice.RasterizerState = rasterizerState;

            // Framerate handling
            if (KeyboardManager.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.R))
            {
                graphics.SynchronizeWithVerticalRetrace = !graphics.SynchronizeWithVerticalRetrace;
                this.IsFixedTimeStep = !this.IsFixedTimeStep;
                graphics.ApplyChanges();
            }

            // Fullscreen handling
            if (KeyboardManager.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.T))
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }

            // Camera handling
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
            {
                camera.MoveRelativeZ(-camera.MovementVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
            {
                camera.MoveRelativeZ(camera.MovementVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
            {
                camera.MoveRelativeX(-camera.MovementVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
            {
                camera.MoveRelativeX(camera.MovementVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
            {
                camera.MoveRelativeY(-camera.MovementVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                camera.MoveRelativeY(camera.MovementVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Q))
            {
                camera.RotateRelativeZ(-camera.RotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E))
            {
                camera.RotateRelativeZ(camera.RotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                camera.RotateRelativeX(-camera.RotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                camera.RotateRelativeX(camera.RotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                camera.RotateRelativeY(-camera.RotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (KeyboardManager.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                camera.RotateRelativeY(camera.RotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
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

            DrawModel(model, modelWorld, camera.ViewMatrix, camera.ProjectionMatrix);
            // Show FPS
            ++FrameRateCounter.FrameCounter;
            // Draw any meshes before the text in order for it to be on the top

            spriteBatch.Begin();
            spriteBatch.DrawString(generalFont, "FPS: " + FrameRateCounter.FrameRate.ToString(), new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(generalFont, "W/A/S/D: forward/left/backwards/right\n" + 
                                                "Ctrl/Space: down/up\n" +
                                                "Q/E: tilt left/right\n" +
                                                "Arrows: look along X/Y axis\n" +
                                                "P: toggle wireframe mode\n" +
                                                "R: toggle fixed framerate\n" +
                                                "T: toggle fullscreen\n" +
                                                "Esc: close app\n", new Vector2(10, 30), Color.White);


            // Hex Sphere
            sphereEffect.World = modelWorld;
            sphereEffect.View = camera.ViewMatrix;
            sphereEffect.Projection = camera.ProjectionMatrix;
            _hexSphere.Draw(graphics, sphereEffect);


            spriteBatch.DrawString(generalFont, _customText, new Vector2(10, 190), Color.White);
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