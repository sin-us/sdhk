using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameWorld.Drawables.VertexTypes;
using System.Collections.Generic;
using SimpleSphere = GameWorld.Gen.SphereGenerator.Sphere;

namespace MonoGameWorld.Drawables.Sphere
{
    public class DrawableSphere
    {
        private GraphicsDeviceManager graphics;
        private Vector3 rotation;
        private VertexPositionColorNormalTexture[] vertices;
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;

        public SimpleSphere Sphere { get; set; }
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
        public Texture2D SphereTexture { get; set; }
        public Effect Effect { get; set; }

        public DrawableSphere(GraphicsDeviceManager graphics, Texture2D sphereTexture, float diameter, int tessellation)
        {
            this.graphics = graphics;
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            SphereTexture = sphereTexture;
            Sphere = new SimpleSphere(diameter, tessellation);

            FillVertices();

            vertexBuffer = new VertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionColorNormalTexture), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData(vertices);

            indexBuffer = new IndexBuffer(graphics.GraphicsDevice, typeof(int), Sphere.Indices.Count, BufferUsage.None);
            indexBuffer.SetData<int>(Sphere.Indices.ToArray());
        }

        private void FillVertices()
        {
            var vertexList = new List<VertexPositionColorNormalTexture>();

            for (int i = 0; i < Sphere.Vertices.Count; ++i)
            {
                Vector3 position = new Vector3((float)Sphere.Vertices[i].X, (float)Sphere.Vertices[i].Y, (float)Sphere.Vertices[i].Z);
                Color color = Color.Green;
                Vector3 normal = new Vector3((float)Sphere.Vertices[i].NormalX, (float)Sphere.Vertices[i].NormalY, (float)Sphere.Vertices[i].NormalZ);
                Vector2 textureCoordinates = new Vector2((float)Sphere.Vertices[i].TextureX, (float)Sphere.Vertices[i].TextureY);
                vertexList.Add(new VertexPositionColorNormalTexture { Position = position, Color = color, Normal = normal, TextureCoordinates = textureCoordinates });
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

            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(World));
            Effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);

            Vector3 viewVector = view.Forward;
            Effect.Parameters["ViewVector"].SetValue(viewVector);

            Effect.Parameters["ModelTexture"].SetValue(SphereTexture);

            graphics.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            graphics.GraphicsDevice.Indices = indexBuffer;

            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Sphere.Indices.Count / 3);
            }
        }
    }
}
