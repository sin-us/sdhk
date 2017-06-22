using MonoGameWorld.Utilities;
using MonoGameWorld.Inputs.Keyboard;
using MonoGameWorld.Inputs.Mouse;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameWorld.HexGrid;
using MonoGameWorld.Drawables.Plane;
using MonoGameWorld.Drawables.Sphere;
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
        private struct Light
        {
            public enum LightType
            {
                DirectionalLight,
                PointLight,
                SpotLight
            }

            public LightType Type;
            public Vector3 Direction;
            public Vector3 Position;
            public Vector4 Ambient;
            public Vector4 Diffuse;
            public Vector4 Specular;
            public float SpotInnerConeRadians;
            public float SpotOuterConeRadians;
            public float Radius;
        }

        private struct Material
        {
            public Vector4 Ambient;
            public Vector4 Diffuse;
            public Vector4 Specular;
            public float Shininess;
        }

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Camera camera;
        private SpriteFont generalFont;
        private DrawablePlane drawablePlane;
        private DrawableSphere drawableSphere;
        private bool isWireFrame;
        private bool isKeybindingsHintShown;

        private Effect effect;

        private Vector4 globalAmbient;

        private Light light;
        private Material material;

        private Vector3 ld;

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
            camera = new Camera(new Vector3(0, 0, 100.0f), Vector3.Zero, Vector3.UnitY, 45.0f, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0.01f, 10000.0f);

            MouseManager.IsPointerVisible = true;
            this.IsMouseVisible = MouseManager.IsPointerVisible;
            MouseManager.LoadCustomCursor(@"Content\AnimatedCursor.ani");
            Form winForm = (Form)Form.FromHandle(this.Window.Handle);
            winForm.Cursor = MouseManager.CustomCursor;

            isWireFrame = false;
            isKeybindingsHintShown = false;

            drawablePlane = new DrawablePlane(graphics, Content.Load<Texture2D>("Wall"), 50, 50, 1, 1);
            drawablePlane.Position = new Vector3(-25, -25, 0);
            drawablePlane.Rotation = new Vector3(MathHelper.ToRadians(-90), 0, 0);

            ld = Vector3.Zero;

            //drawableSphere = new DrawableSphere(graphics, Content.Load<Texture2D>("Earth8k"), 50.0f, 32);

            // Create a new SpriteBatch, which can be used to draw textures / text
            spriteBatch = new SpriteBatch(GraphicsDevice);

            InputConfigManager.DefaultInitialize();

            globalAmbient = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            light.Type = Light.LightType.SpotLight;
            light.Direction = new Vector3(-1.0f, 0.0f, -0.5f);
            light.Position = Vector3.Zero;
            light.Ambient = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            light.Diffuse = new Vector4(1.0f, 1.0f, 0.95f, 1.0f);
            light.Specular = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            light.SpotInnerConeRadians = MathHelper.ToRadians(20.0f);
            light.SpotOuterConeRadians = MathHelper.ToRadians(80.0f);
            light.Radius = 100.0f;

            material.Ambient = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            material.Diffuse = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            material.Specular = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            material.Shininess = 0.0f;

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

            effect = Content.Load<Effect>("Fx/PerPixelLighting");
            //drawablePlane.Effect = effect;

            effect.Parameters["CameraPosition"].SetValue(Vector3.Zero);

            // Set the shader global ambiance parameters.
            effect.Parameters["GlobalAmbient"].SetValue(globalAmbient);

            // Set the shader lighting parameters.
            effect.Parameters["LightDirection"].SetValue(light.Direction);
            effect.Parameters["LightPosition"].SetValue(light.Position);
            effect.Parameters["LightAmbient"].SetValue(light.Ambient);
            effect.Parameters["LightDiffuse"].SetValue(light.Diffuse);
            effect.Parameters["LightSpecular"].SetValue(light.Specular);
            effect.Parameters["LightSpotInnerCone"].SetValue(light.SpotInnerConeRadians);
            effect.Parameters["LightSpotOuterCone"].SetValue(light.SpotOuterConeRadians);
            effect.Parameters["LightRadius"].SetValue(light.Radius);

            // Set the shader material parameters.
            effect.Parameters["MaterialAmbient"].SetValue(material.Ambient);
            effect.Parameters["MaterialDiffuse"].SetValue(material.Diffuse);
            effect.Parameters["MaterialSpecular"].SetValue(material.Specular);
            effect.Parameters["MaterialShininess"].SetValue(material.Shininess);

            // Bind the texture map to the shader.
            //effect.Parameters["Texture"].SetValue(drawableSphere.SphereTexture);
            effect.Parameters["Texture"].SetValue(drawablePlane.PlaneTexture);

            switch (light.Type)
            {
                case Light.LightType.DirectionalLight:
                    effect.CurrentTechnique = effect.Techniques["PerPixelDirectionalLighting"];
                    break;

                case Light.LightType.PointLight:
                    effect.CurrentTechnique = effect.Techniques["PerPixelPointLighting"];
                    break;

                case Light.LightType.SpotLight:
                    effect.CurrentTechnique = effect.Techniques["PerPixelSpotLighting"];
                    break;

                default:
                    break;
            }

            drawableSphere.Effect = effect;
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

            drawablePlane.Update(camera.Offset);
            //drawableSphere.Rotation += new Vector3(0, 0.001f, 0);
            //drawableSphere.Update(camera.Offset);

            base.Update(gameTime);
        }

        private void HandleCameraInput()
        {
            // Camera handling
            if (InputConfigManager.IsKeyBindingPressed(ActionType.SwitchCameraFreeMode))
            {
                camera.SetFreeCamera();
            }
            /*if (InputConfigManager.IsKeyBindingPressed(ActionType.SwitchCameraThirdPersonMode))
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
            }*/
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
            drawablePlane.Draw(camera.ProjectionMatrix, camera.ViewMatrix);
            //drawableSphere.Draw(camera.ViewMatrix, camera.ProjectionMatrix);

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

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}