using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGameWorld.HexGrid
{
    public class HexSphere
    {
        private HexLib.HexSphereGenerator.Grid _sphereGrid;

        public HexSphere(int size)
        {
            _sphereGrid = HexLib.HexSphereGenerator.Grid.CreateSizeNGrid(size);
        }

        public void Draw(GraphicsDeviceManager graphics, BasicEffect effect)
        {
            // The assignment of effect.View and effect.Projection
            // are nearly identical to the code in the Model drawing code.
            var cameraPosition = new Vector3(0, 40, 20);
            var cameraLookAtVector = Vector3.Zero;
            var cameraUpVector = Vector3.UnitZ;

            effect.View = Matrix.CreateLookAt(cameraPosition, cameraLookAtVector, cameraUpVector);

            float aspectRatio = graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
            float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
            float nearClipPlane = 1;
            float farClipPlane = 200;

            effect.Projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClipPlane, farClipPlane);

            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[12];

            foreach (var t in _sphereGrid.Tiles)
            {
                for (int j = 0; j < t.Corners.Length - 2; ++j)
                {
                    vertices[j * 3].Position = new Vector3((float)t.Corners[0].V.X * 15, (float)t.Corners[0].V.Y * 15, (float)t.Corners[0].V.Z * 15);
                    vertices[j * 3 + 1].Position = new Vector3((float)t.Corners[j + 1].V.X * 15, (float)t.Corners[j + 1].V.Y * 15, (float)t.Corners[j + 1].V.Z * 15);
                    vertices[j * 3 + 2].Position = new Vector3((float)t.Corners[j + 2].V.X * 15, (float)t.Corners[j + 2].V.Y * 15, (float)t.Corners[j + 2].V.Z * 15);

                    vertices[j * 3].Color = t.Id % 5 == 0 ? Color.Green : Color.Blue;
                    vertices[j * 3 + 1].Color = t.Id % 5 == 0 ? Color.Green : Color.Blue;
                    vertices[j * 3 + 2].Color = t.Id % 5 == 0 ? Color.Green : Color.Blue;
                }

                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    effect.VertexColorEnabled = true;

                    graphics.GraphicsDevice.DrawUserPrimitives(
                        // We’ll be rendering two trinalges
                        PrimitiveType.TriangleList,
                        // The array of verts that we want to render
                        vertices,
                        // The offset, which is 0 since we want to start 
                        // at the beginning of the floorVerts array
                        0,
                        // The number of triangles to draw
                        t.Corners.Length - 2);
                }
            }
        }
    }
}
