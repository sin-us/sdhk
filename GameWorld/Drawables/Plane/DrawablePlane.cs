using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using SimplePlane = GameWorld.Gen.PlaneGenerator.Plane;

namespace MonoGameWorld.Drawables.Plane
{
    public class DrawablePlane
    {
        private GraphicsDeviceManager graphics;
        private Vector3 rotation;
        private VertexPositionColorTexture[] vertices;
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;

        public SimplePlane Plane { get; set; }
        public Vector3 Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                RotationQuaternion = Quaternion.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
                RotationQuaternion.Normalize();
            }
        }
        public Vector3 Position { get; set; }
        public Quaternion RotationQuaternion { get; private set; }
        public Matrix World { get; private set; }
        public Texture2D PlaneTexture { get; set; }
        public Effect Effect { get; set; }
        
        public DrawablePlane(GraphicsDeviceManager graphics, Texture2D planeTexture, int verticesWidth, int verticesHeight, float widthStep, float heightStep)
        {
            this.graphics = graphics;
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            PlaneTexture = planeTexture;
            Plane = new SimplePlane(verticesWidth, verticesHeight, widthStep, heightStep);

            FillVertices();

            vertexBuffer = new VertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionColorTexture), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData(vertices);

            indexBuffer = new IndexBuffer(graphics.GraphicsDevice, typeof(int), Plane.Indeces.Count, BufferUsage.None);
            indexBuffer.SetData<int>(Plane.Indeces.ToArray());
        }

        private void FillVertices()
        {
            var vertexList = new List<VertexPositionColorTexture>();

            for (int i = 0; i < Plane.Vertices.Count; ++i)
            {
                Vector3 position = new Vector3((float)Plane.Vertices[i].X, (float)Plane.Vertices[i].Y, (float)Plane.Vertices[i].Z);
                Color color = Color.Green;
                Vector2 textureCoordinates = new Vector2((float)Plane.Vertices[i].TextureX, (float)Plane.Vertices[i].TextureY);
                vertexList.Add(new VertexPositionColorTexture { Position = position, Color = color, TextureCoordinate = textureCoordinates });
            }

            vertices = vertexList.ToArray();            
        }

        public void Update(Vector3 cameraOffset)
        {
            World = Matrix.CreateFromQuaternion(RotationQuaternion) * Matrix.CreateTranslation(Position - cameraOffset);
        }

        public void Draw(Matrix projection, Matrix view)
        {
            Effect.Parameters["World"].SetValue(World);
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);

            graphics.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            graphics.GraphicsDevice.Indices = indexBuffer;

            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Plane.Indeces.Count / 3);
            }
        }
    }
}
