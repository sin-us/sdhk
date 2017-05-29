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
        private CameraType cameraType;
        private Quaternion rotation;
        private Vector3 oldOffset;

        public CameraType CameraType
        {
            get { return cameraType; }
            set
            {
                if (value != cameraType)
                {
                    {
                        if ((cameraType == CameraType.ThirdPersonFree) || (cameraType == CameraType.ThirdPersonFreeAlt))
                        {
                            ViewMatrix.Decompose(out Vector3 scale, out rotation, out Vector3 translation);
                        }
                    }

                    cameraType = value;
                }
            }
        }
        public Matrix ViewMatrix { get; set; } = Matrix.Identity;
        public Matrix ProjectionMatrix { get; set; }
        public Vector3 Offset { get; set; }
        public Quaternion Rotation { get => rotation; set => rotation = value; }
        public Vector3 LookAt { get; set; }
        public Vector3 Up { get; set; }
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
            Rotation = Quaternion.Identity;
            Width = width;
            Height = height;
            Fov = fov;
            AspectRatio = (float)width / height;
            ZNear = znear;
            ZFar = zfar;
            CameraType = CameraType.Free;
            MovementVelocity = 10.0f;
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
            Rotation = Quaternion.Identity;
            Width = width;
            Height = height;
            Fov = fov;
            AspectRatio = (float)width / height;
            ZNear = znear;
            ZFar = zfar;
            CameraType = CameraType.Free;
            MovementVelocity = 10.0f;
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
                //@TODO implement for ThirdPerson all types
            }

            GetAxisFromViewMatrix();

            // LookAt could be a change candidate (to Vector3? instead of Vector3) if 3rd person view functionality will be implemented.
            // In this case it will no longer be -ZAxis, but the center of the folowed object.
            LookAt = -ZAxis;
            Up = YAxis;

            LookAt.Normalize();
            Up.Normalize();

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
            if ((CameraType == CameraType.ThirdPersonFree) || (CameraType == CameraType.ThirdPersonFreeAlt) || (CameraType == CameraType.ThirdPersonLocked))
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
                    //@TODO implement
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
                //@TODO implement
            }
        }

        public void SetFreeCamera(Vector3 position, Vector3 lookAt, Vector3 up)
        {
            Offset = position;
            LookAt = lookAt;
            Up = up;
            ViewMatrix = Matrix.CreateLookAt(Vector3.Zero, lookAt, up);

            Quaternion decomposedRotation;
            ViewMatrix.Decompose(out Vector3 scale, out decomposedRotation, out Vector3 translation);
            Rotation = decomposedRotation;
            CameraType = CameraType.Free;
        }

        public void SetFreeCamera()
        {
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

                Quaternion decomposedRotation;
                ViewMatrix.Decompose(out Vector3 scale, out decomposedRotation, out Vector3 translation);
                Rotation = decomposedRotation;
            }
        }
    }
}