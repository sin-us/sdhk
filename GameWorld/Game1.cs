using MonoGameWorld.Utilities;
using MonoGameWorld.Inputs.Keyboard;
using MonoGameWorld.Inputs.Mouse;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameWorld.HexGrid;
using MonoGameWorld.Drawables.Plane;
using System.Windows.Forms;
using MonoGameWorld.Camera;
using GameWorld.Shared;
using MonoGameWorld.Configurations.Input;
using System.Linq;

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
        private HexGrid _hexGrid;
        private DrawableHexSphere _hexSphere;
        private int _hexSphereIntersectionChecksCount;
        private DrawablePlane drawablePlane;
        private bool isWireFrame;
        private bool isKeybindingsHintShown;

        private ControlPanelListener _controlPanelListener;
        private string _customText = string.Empty;

        private Effect effect;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
                SynchronizeWithVerticalRetrace = true,
                GraphicsProfile = GraphicsProfile.HiDef
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
            // zfar is decreased intentionally, because on large values Ray Vector becomes NaN
            camera = new Camera(new Vector3(0, 100.0f, 100.0f), Vector3.Zero, Vector3.UnitY, 45.0f, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0.01f, 10000.0f);

            MouseManager.IsPointerVisible = true;
            this.IsMouseVisible = MouseManager.IsPointerVisible;
            MouseManager.LoadCustomCursor(@"Content\AnimatedCursor.ani");
            Form winForm = (Form)Form.FromHandle(this.Window.Handle);
            winForm.Cursor = MouseManager.CustomCursor;

            isWireFrame = false;
            isKeybindingsHintShown = false;

            //_hexSphere = new DrawableHexSphere(graphics, 7, 50);
            //_hexSphere.Effect.VertexColorEnabled = true;
            drawablePlane = new DrawablePlane(graphics, Content.Load<Texture2D>("Wall"), 50, 50, 1, 1);
            drawablePlane.Position = new Vector3(-25, 0, 0);
            drawablePlane.Rotation = new Vector3(MathHelper.ToRadians(-90), 0, 0);

            // Create a new SpriteBatch, which can be used to draw textures / text
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
            // set up font
            generalFont = Content.Load<SpriteFont>("GeneralFont");

            effect = Content.Load<Effect>("Fx/Ambient");
            effect.Parameters["AmbientColor"].SetValue(Color.Green.ToVector4());
            effect.Parameters["AmbientIntensity"].SetValue(0.1f);
            drawablePlane.Effect = effect;

            //_hexGrid = new HexGrid(Content.Load<Texture2D>("HexGridTileset"), 50, 100, 87);

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

            // Toggle keybindings hint
            if (InputConfigManager.IsKeyBindingPressed(ActionType.ToggleKeybindingsHint))
            {
                isKeybindingsHintShown = !isKeybindingsHintShown;
            }

            // Toggle wireframe mode
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

            HandleCameraInput();

            //Ray mouseRay = Mathematics.CalculateRay(MouseManager.MouseStatus.Position.ToVector2(), camera.ViewMatrix, camera.ProjectionMatrix, _hexSphere.Effect.World, GraphicsDevice.Viewport);
            //_hexSphere.CheckIntersection(ref mouseRay, out _hexSphereIntersectionChecksCount);
            
            //_hexSphere.Update(gameTime, camera.Offset);
            drawablePlane.Update(camera.Offset);

            base.Update(gameTime);
        }

        private void HandleCameraInput()
        {
            // Camera handling
            if (InputConfigManager.IsKeyBindingPressed(ActionType.SwitchCameraFreeMode))
            {
                camera.SetFreeCamera();
            }
            if (InputConfigManager.IsKeyBindingPressed(ActionType.SwitchCameraThirdPersonMode))
            {
                camera.SetThirdPersonCamera(_hexSphere.Position, _hexSphere.AxisRotationQuaternion, new Vector3(0, 0, 0), CameraType.ThirdPersonFree, new Vector3(300.0f, 0, 0), 200);
            }
            if (InputConfigManager.IsKeyBindingPressed(ActionType.SwitchCameraThirdPersonAltMode))
            {
                camera.SetThirdPersonCamera(_hexSphere.Position, _hexSphere.AxisRotationQuaternion, new Vector3(0, 0, 0), CameraType.ThirdPersonFreeAlt, null, 200);
            }
            if (InputConfigManager.IsKeyBindingPressed(ActionType.SwitchCameraThirdPersonLockedMode))
            {
                camera.SetThirdPersonCamera(_hexSphere.Position, _hexSphere.AxisRotationQuaternion, new Vector3(0, 0, 0), CameraType.ThirdPersonLocked, null, 200);
            }
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
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            // Draw any meshes before the text in order for it to be on the top

            // Hex Sphere
            //_hexSphere.Draw(camera.ProjectionMatrix, camera.ViewMatrix);
            drawablePlane.Draw(camera.ProjectionMatrix, camera.ViewMatrix);

            string hintString = "";

            if (isKeybindingsHintShown)
            {
                foreach (var binding in InputConfigManager.Bindings.Bindings)
                {
                    hintString += binding.Description + ": ";

                    if (binding.KeyBindingInfo.Key != null)
                    {
                        hintString += binding.KeyBindingInfo.Key.ToString();
                    }

                    hintString += "\n";
                }
            }
            else
            {
                hintString = InputConfigManager.Bindings.GetBindingByAction(ActionType.ToggleKeybindingsHint).Description + ": " +
                             InputConfigManager.Bindings.GetBindingByAction(ActionType.ToggleKeybindingsHint).KeyBindingInfo.Key.ToString();
            }

            // Show FPS
            ++FrameRateCounter.FrameCounter;
            spriteBatch.Begin();
            spriteBatch.DrawString(generalFont, "FPS: " + FrameRateCounter.FrameRate.ToString(), new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(generalFont, "Camera type: " + camera.CameraType.ToString(), new Vector2(10, 30), Color.White);
            spriteBatch.DrawString(generalFont, hintString, new Vector2(10, 50), Color.White);
            spriteBatch.DrawString(generalFont, _customText, new Vector2(50, 10), Color.White);

            //spriteBatch.DrawString(generalFont, $"IntersectChecks: {_hexSphereIntersectionChecksCount}", new Vector2(10, 70), Color.White);
            //spriteBatch.DrawString(generalFont, $"Selected tile height: {_hexSphere.SelectedTile?.Height}", new Vector2(10, 90), Color.White);
            //spriteBatch.DrawString(generalFont, $"Selected tile brightness: {_hexSphere.SelectedTile?.Brightness}", new Vector2(10, 110), Color.White);
            //spriteBatch.DrawString(generalFont, $"Selected tile Long/Lat: {_hexSphere.SelectedTile?.Longitude} / {_hexSphere.SelectedTile?.Latitude}", new Vector2(10, 130), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}