using System;
using Microsoft.Xna.Framework;
using MonoGameWorld.Utilities;

namespace MonoGameWorld.Camera
{
    public enum CameraType
    {
        Free,
        FirstPerson,
        ThirdPersonFree,
        ThirdPersonFreeAlt,
        ThirdPersonLocked,
    }

    class Camera
    {
        private Quaternion rotation;
        private Vector3 oldOffset;
        private Quaternion hostRotation;
        private Vector3 hostPosition;

        public CameraType CameraType { get; private set; }
        public Matrix ViewMatrix { get; set; } = Matrix.Identity;
        public Matrix ProjectionMatrix { get; set; }
        public Vector3 Offset { get; set; }
        public Quaternion Rotation { get => rotation; set => rotation = value; }
        public Vector3? LookAt { get; set; }
        public Vector3 Up { get; set; }
        public Quaternion HostRotation { get => hostRotation; set => hostRotation = value; }
        public Vector3 HostPosition { get => hostPosition; set => hostPosition = value; }
        public float Radius { get; set; }
        public Vector3 XAxis { get; private set; }
        public Vector3 YAxis { get; private set; }
        public Vector3 ZAxis { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Single Fov { get; set; }
        public Single FovY { get; set; }
        public Single ZNear { get; set; }
        public Single ZFar { get; set; }
        public Single AspectRatio { get; set; }
        public float MovementVelocity { get; set; }
        public float RotationVelocity { get; set; }
        public bool IsMoving { get; set; }

        public Camera(Single fov, Int32 width, Int32 height, Single znear, Single zfar)
        {
            Offset = Vector3.Zero;
            oldOffset = Offset;
            LookAt = Vector3.Forward;
            Up = Vector3.UnitY;
            HostRotation = Quaternion.Identity;
            HostPosition = Vector3.Zero;
            Rotation = Quaternion.Identity;
            Width = width;
            Height = height;
            Fov = fov;
            AspectRatio = (float)width / height;
            ZNear = znear;
            ZFar = zfar;
            CameraType = CameraType.Free;
            MovementVelocity = 40.0f;
            RotationVelocity = 50.0f;
            IsMoving = false;

            Initialize();
        }

        public Camera(Vector3 position, Vector3 lookAt, Vector3 up, Single fov, Int32 width, Int32 height, Single znear, Single zfar)
        {
            Offset = position;
            oldOffset = Offset;
            LookAt = lookAt;
            Up = up;
            HostRotation = Quaternion.Identity;
            HostPosition = Vector3.Zero;
            Rotation = Quaternion.Identity;
            Width = width;
            Height = height;
            Fov = fov;
            AspectRatio = (float)width / height;
            ZNear = znear;
            ZFar = zfar;
            CameraType = CameraType.Free;
            MovementVelocity = 40.0f;
            RotationVelocity = 50.0f;
            IsMoving = false;

            Initialize();
        }

        private void Initialize()
        {
            GetAxisFromViewMatrix();
            BuildPerspectiveFov(ZNear, ZFar);
        }

        private void BuildPerspectiveFov(float zNear, float zFar)
        {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(Fov), (float)Width / Height, zNear, zFar);
        }

        private void GetAxisFromViewMatrix()
        {
            XAxis = new Vector3(ViewMatrix.M11, ViewMatrix.M21, ViewMatrix.M31);
            YAxis = new Vector3(ViewMatrix.M12, ViewMatrix.M22, ViewMatrix.M32);
            ZAxis = new Vector3(ViewMatrix.M13, ViewMatrix.M23, ViewMatrix.M33);
        }

        public void Update()
        {
            // check rotation quaternion normalization
            if (!Mathematics.IsQuaternionNormalized(Rotation))
            {
                Rotation.Normalize();
            }

            if (CameraType == CameraType.Free)
            {
                ViewMatrix = Matrix.CreateFromQuaternion(Rotation);
            }
            else if (CameraType == CameraType.FirstPerson)
            {
                //@TODO implement
            }
            else
            {
                Matrix rotationMatrix = Matrix.CreateFromQuaternion(Rotation);
                Offset = HostPosition - (Vector3.Multiply(new Vector3(rotationMatrix.M13, rotationMatrix.M23, rotationMatrix.M33), Radius));

                if (LookAt != null)
                {
                    ViewMatrix = Matrix.CreateLookAt(Vector3.Zero, (Vector3)LookAt - Offset, new Vector3(rotationMatrix.M12, rotationMatrix.M22, rotationMatrix.M32));
                }
                else
                {
                    ViewMatrix = Matrix.CreateLookAt(Vector3.Zero, HostPosition - Offset, new Vector3(rotationMatrix.M12, rotationMatrix.M22, rotationMatrix.M32));
                }
            }

            GetAxisFromViewMatrix();

            CheckIsMoving();
        }

        public void SetAbsoluteRotation(float angleX, float angleY, float angleZ)
        {
            if (CameraType != CameraType.ThirdPersonLocked)
            {
                Rotation = Quaternion.CreateFromYawPitchRoll(angleY, angleX, angleZ);
            }
            else
            {
                Rotation = Quaternion.CreateFromYawPitchRoll(0, angleX, 0);
            }
        }

        public void RotateRelativeX(float angle)
        {
            if (CheckIsThirdPersonType(CameraType))
            {
                angle = -angle;
            }

            Rotation = Quaternion.Multiply(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(angle)), Rotation);
        }

        public void RotateRelativeY(float angle)
        {
            if (CameraType != CameraType.ThirdPersonLocked)
            {
                if ((CameraType == CameraType.ThirdPersonFree) || (CameraType == CameraType.ThirdPersonFreeAlt))
                {
                    angle = -angle;
                }

                if (CameraType == CameraType.ThirdPersonFreeAlt)
                {
                    Rotation = Quaternion.Multiply(Rotation, Quaternion.CreateFromAxisAngle(Matrix.CreateFromQuaternion(HostRotation).Up, MathHelper.ToRadians(angle)));
                }
                else
                {
                    Rotation = Quaternion.Multiply(Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(angle)), Rotation);
                }
            }
        }

        public void RotateRelativeZ(float angle)
        {
            if (CameraType != CameraType.ThirdPersonLocked)
            {
                Rotation = Quaternion.Multiply(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(angle)), Rotation);
            }
        }

        public void SetAbsoluteTranslation(float positionX, float positionY, float positionZ)
        {
            Offset = new Vector3(positionX, positionY, positionZ);
        }

        public void MoveRelativeX(float relativeX)
        {
            if (CameraType == CameraType.Free)
            {
                Offset += Vector3.Multiply(XAxis, relativeX);
            }
        }

        public void MoveRelativeY(float relativeY)
        {
            if (CameraType == CameraType.Free)
            {
                Offset += Vector3.Multiply(YAxis, relativeY);
            }
        }

        public void MoveRelativeZ(float relativeZ)
        {
            if (CameraType == CameraType.Free)
            {
                Offset += Vector3.Multiply(ZAxis, relativeZ);
            }
            else if (CameraType == CameraType.FirstPerson)
            {
                //@TODO implement
            }
            else
            {
                Radius += relativeZ;
            }
        }

        public void SetFreeCamera(Vector3 position, Vector3 lookAt, Vector3 up)
        {
            Offset = position;
            LookAt = lookAt;
            Up = up;
            ViewMatrix = Matrix.CreateLookAt(Vector3.Zero, lookAt, up);

            ViewMatrix.Decompose(out Vector3 scale, out Quaternion decomposedRotation, out Vector3 translation);
            Rotation = decomposedRotation;
            CameraType = CameraType.Free;
        }

        public void SetFreeCamera()
        {
            if (CheckIsThirdPersonType(CameraType))
            {
                ViewMatrix.Decompose(out Vector3 dummyScale, out rotation, out Vector3 dummyTranslation);
            }

            CameraType = CameraType.Free;
        }

        public void SetFreePosition(Vector3 position)
        {
            if (CameraType == CameraType.Free)
            {
                Offset = position;
            }
        }

        public void SetFreeLookAt(Vector3 lookAt)
        {
            if (CameraType == CameraType.Free)
            {
                // convert to offset world presentation (float)
                lookAt -= Offset;
                Vector3 up = new Vector3(ViewMatrix.M12, ViewMatrix.M22, ViewMatrix.M32);

                ViewMatrix = Matrix.CreateLookAt(Vector3.Zero, lookAt, up);

                ViewMatrix.Decompose(out Vector3 scale, out Quaternion decomposedRotation, out Vector3 translation);
                Rotation = decomposedRotation;
            }
        }

        public void SetThirdPersonCamera(Vector3 hostPosition, Quaternion hostRotation, Vector3 initialRelativeRotation, CameraType desiredType, Vector3? lookAt = null, float? radius = null)
        {
            if (!CheckIsThirdPersonType(desiredType))
            {
                return;
            }

            HostPosition = hostPosition;
            HostRotation = hostRotation;
            LookAt = lookAt;
            Rotation = HostRotation;

            if (radius != null)
            {
                Radius = (float)radius;
            }
            else
            {
                //@TODO add check for bounding sphere radius
                //Radius = (Owner.GetDiameter()); // Diameter - how far away
            }

            if (desiredType != CameraType.ThirdPersonLocked)
            {
                Rotation = Quaternion.Multiply(Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(-initialRelativeRotation.X)), Rotation);
                if (CameraType == CameraType.ThirdPersonFree)
                {
                    Rotation = Quaternion.Multiply(Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(initialRelativeRotation.Y)), Rotation);
                }
                else
                {
                    Rotation = Quaternion.Multiply(Quaternion.CreateFromAxisAngle(Matrix.CreateFromQuaternion(Rotation).Up, MathHelper.ToRadians(initialRelativeRotation.Y)), Rotation);
                }
                Rotation = Quaternion.Multiply(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(-initialRelativeRotation.Z)), Rotation);
            }
            else
            {
                Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(-initialRelativeRotation.X));
            }

            CameraType = desiredType;
        }

        public void SetRadius(float radius)
        {
            if (CameraType != CameraType.Free)
            {
                Radius = radius;
            }
        }

        public void SetThirdPersonLookAt(Vector3? lookAt)
        {
            if (CheckIsThirdPersonType(CameraType))
            {
                LookAt = lookAt;
            }
        }

        private void CheckIsMoving()
        {
            if ((oldOffset - Offset) != Vector3.Zero)
            {
                IsMoving = true;
            }
            else
            {
                IsMoving = false;
            }

            oldOffset = Offset;
        }

        private bool CheckIsThirdPersonType(CameraType type)
        {
            if ((type == CameraType.ThirdPersonFree) || (type == CameraType.ThirdPersonFreeAlt) || (type == CameraType.ThirdPersonLocked))
            {
                return true;
            }

            return false;
        }
    }
}