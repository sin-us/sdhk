using MonoGameWorld.Utilities;
using MonoGameWorld.Inputs.Keyboard;
using MonoGameWorld.Inputs.Mouse;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameWorld.HexGrid;
using System.Windows.Forms;
using MonoGameWorld.Camera;
using GameWorld.Shared;
using MonoGameWorld.Configurations.Input;

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
        private BasicEffect sphereEffect;
        private Vector3 spherePosition;
        private HexGrid _hexGrid;
        private HexSphere _hexSphere;
        private bool isWireFrame;

        private Model model;
        private Matrix modelWorld;
        private Vector3 modelPosition;

        private ControlPanelListener _controlPanelListener;
        private string _customText = string.Empty;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
                SynchronizeWithVerticalRetrace = true
            };
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
            camera = new Camera(new Vector3(0, 0, 10.0f), Vector3.Forward, Vector3.UnitY, 45.0f, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0.01f, 100000000.0f);

            MouseManager.IsPointerVisible = true;
            this.IsMouseVisible = MouseManager.IsPointerVisible;
            MouseManager.LoadCustomCursor(@"Content\AnimatedCursor.ani");
            Form winForm = (Form)Form.FromHandle(this.Window.Handle);
            winForm.Cursor = MouseManager.CustomCursor;

            isWireFrame = false;

            spherePosition = new Vector3(0, 0, 0);
            sphereEffect = new BasicEffect(graphics.GraphicsDevice);
            sphereEffect.View = camera.ViewMatrix;
            sphereEffect.Projection = camera.ProjectionMatrix;
            sphereEffect.World = Matrix.CreateTranslation(spherePosition - camera.Offset);

            modelPosition = new Vector3(0, 0, 0);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            InputConfigManager.DefaultInitialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            model = Content.Load<Model>("lava_cube");

            // set up font
            generalFont = Content.Load<SpriteFont>("GeneralFont");

            _hexGrid = new HexGrid(Content.Load<Texture2D>("HexGridTileset"), 50, 100, 87);
            _hexSphere = new HexSphere(5);

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
            if (InputConfigManager.IsKeyBindingPressed(ActionType.Exit))
            {
                this.Exit();
            }

            // Switch to/from wireframe mode
            if (InputConfigManager.IsKeyBindingPressed(ActionType.ToggleWireframe))
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
            if (InputConfigManager.IsKeyBindingPressed(ActionType.ToggleFixedFramerate))
            {
                graphics.SynchronizeWithVerticalRetrace = !graphics.SynchronizeWithVerticalRetrace;
                this.IsFixedTimeStep = !this.IsFixedTimeStep;
                graphics.ApplyChanges();
            }

            // Fullscreen handling
            if (InputConfigManager.IsKeyBindingPressed(ActionType.ToggleFullscreen))
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }

            // Camera handling
            if (InputConfigManager.IsKeyBindingDown(ActionType.CameraMoveForward))
            {
                camera.MoveRelativeZ(-camera.MovementVelocity * FrameRateCounter.FrameTime);
            }
            if (InputConfigManager.IsKeyBindingDown(ActionType.CameraMoveBackward))
            {
                camera.MoveRelativeZ(camera.MovementVelocity * FrameRateCounter.FrameTime);
            }
            if (InputConfigManager.IsKeyBindingDown(ActionType.CameraMoveLeft))
            {
                camera.MoveRelativeX(-camera.MovementVelocity * FrameRateCounter.FrameTime);
            }
            if (InputConfigManager.IsKeyBindingDown(ActionType.CameraMoveRight))
            {
                camera.MoveRelativeX(camera.MovementVelocity * FrameRateCounter.FrameTime);
            }
            if (InputConfigManager.IsKeyBindingDown(ActionType.CameraMoveDown))
            {
                camera.MoveRelativeY(-camera.MovementVelocity * FrameRateCounter.FrameTime);
            }
            if (InputConfigManager.IsKeyBindingDown(ActionType.CameraMoveUp))
            {
                camera.MoveRelativeY(camera.MovementVelocity * FrameRateCounter.FrameTime);
            }
            if (InputConfigManager.IsKeyBindingDown(ActionType.CameraTiltLeft))
            {
                camera.RotateRelativeZ(-camera.RotationVelocity * FrameRateCounter.FrameTime);
            }
            if (InputConfigManager.IsKeyBindingDown(ActionType.CameraTiltRight))
            {
                camera.RotateRelativeZ(camera.RotationVelocity * FrameRateCounter.FrameTime);
            }
            if (InputConfigManager.IsKeyBindingDown(ActionType.CameraRotateUp))
            {
                camera.RotateRelativeX(-camera.RotationVelocity * FrameRateCounter.FrameTime);
            }
            if (InputConfigManager.IsKeyBindingDown(ActionType.CameraRotateDown))
            {
                camera.RotateRelativeX(camera.RotationVelocity * FrameRateCounter.FrameTime);
            }
            if (InputConfigManager.IsKeyBindingDown(ActionType.CameraRotateLeft))
            {
                camera.RotateRelativeY(-camera.RotationVelocity * FrameRateCounter.FrameTime);
            }
            if (InputConfigManager.IsKeyBindingDown(ActionType.CameraRotateRight))
            {
                camera.RotateRelativeY(camera.RotationVelocity * FrameRateCounter.FrameTime);
            }

            camera.Update();
            sphereEffect.World = Matrix.CreateTranslation(spherePosition - camera.Offset);
            sphereEffect.View = camera.ViewMatrix;
            sphereEffect.Projection = camera.ProjectionMatrix;
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

            // Draw any meshes before the text in order for it to be on the top
            DrawModel(model, modelWorld, camera.ViewMatrix, camera.ProjectionMatrix);

            string hintString = "";

            foreach (var binding in InputConfigManager.Bindings.Bindings)
            {
                hintString += binding.Description + ": ";

                if (binding.KeyBindingInfo.Key != null)
                {
                    hintString += binding.KeyBindingInfo.Key.ToString();
                }

                hintString += "\n";
            }

            // Show FPS
            ++FrameRateCounter.FrameCounter;
            spriteBatch.Begin();
            spriteBatch.DrawString(generalFont, "FPS: " + FrameRateCounter.FrameRate.ToString(), new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(generalFont, hintString, new Vector2(10, 30), Color.White);
            spriteBatch.DrawString(generalFont, _customText, new Vector2(50, 10), Color.White);
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