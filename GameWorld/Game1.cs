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
using GameWorld.ControlPanel;

namespace GameWorld
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private SpriteFont generalFont;
        private Model model;
        private float angle;

        private Matrix world;
        private Matrix view;
        private Matrix projection;

        private BasicEffect effect;

        private HexGrid _hexGrid;
        private HexSphere _hexSphere;

        private ControlPanelListener _controlPanelListener;
        private string _customText = string.Empty;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 800;
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
            world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
            view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 1000f);

            angle = 0;

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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
				this.Exit();

            // TODO: Add your update logic here

            // update frame rate
            FrameRateCounter.Update(gameTime);
            KeyboardManager.Update();
            MouseManager.Update();

            angle += 0.01f;
            world = Matrix.CreateRotationX(angle) * Matrix.CreateRotationY(angle);

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
            DrawModel(model, world, view, projection);

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

            // Hex Sphere
            _hexSphere.Draw(graphics, effect);
            
            Texture2D SimpleTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Int32[] pixel = { 0xFFFFFF }; // White. 0xFF is Red, 0xFF0000 is Blue
            SimpleTexture.SetData<Int32>(pixel, 0, SimpleTexture.Width * SimpleTexture.Height);
            
            spriteBatch.Begin();

            spriteBatch.DrawString(generalFont, "FPS: " + FrameRateCounter.FrameRate.ToString(), new Vector2(10, 0), Color.White);
            spriteBatch.DrawString(generalFont, "Left: " +
                                                         "{D: " + MouseManager.MouseStatus.LeftButton.IsDown + " " +
                                                         "P: " + MouseManager.MouseStatus.LeftButton.IsPressed + " " +
                                                         "R: " + MouseManager.MouseStatus.LeftButton.IsReleased + "}"
                                                          , new Vector2(10, 20), Color.White);
            spriteBatch.DrawString(generalFont, "Middle: " +
                                                         "{D: " + MouseManager.MouseStatus.MiddleButton.IsDown + " " +
                                                         "P: " + MouseManager.MouseStatus.MiddleButton.IsPressed + " " +
                                                         "R: " + MouseManager.MouseStatus.MiddleButton.IsReleased + "}"
                                                          , new Vector2(10, 40), Color.White);
            spriteBatch.DrawString(generalFont, "Right: " +
                                                         "{D: " + MouseManager.MouseStatus.RightButton.IsDown + " " +
                                                         "P: " + MouseManager.MouseStatus.RightButton.IsPressed + " " +
                                                         "R: " + MouseManager.MouseStatus.RightButton.IsReleased + "}"
                                                          , new Vector2(10, 60), Color.White);
            spriteBatch.DrawString(generalFont, "DeltaX: " + MouseManager.MouseStatus.DeltaX, new Vector2(10, 80), Color.White);
            spriteBatch.DrawString(generalFont, "DeltaY: " + MouseManager.MouseStatus.DeltaY, new Vector2(10, 100), Color.White);
            spriteBatch.DrawString(generalFont, "DeltaW: " + MouseManager.MouseStatus.WheelDelta, new Vector2(10, 120), Color.White);
            spriteBatch.DrawString(generalFont, _customText, new Vector2(10, 190), Color.White);

            String keysString = "";
            foreach(KeyStatus k in KeyboardManager.KeysDown)
            {
                keysString += k.Key.ToString() + "(" + (k.IsPressed ? "1" : "0") + ")" + " ";
            }
            spriteBatch.DrawString(generalFont, "Keys: " + keysString, new Vector2(10, 140), Color.White);
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
                    effect.FogEnd = 10.0f;
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }
    }
}