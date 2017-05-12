using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameWorld.Configurations
{
    public enum ActionType
    {
        CameraMoveForward = 0,
        CameraMoveBackward = 1,
        CameraMoveLeft = 2,
        CameraMoveRight = 3,
        CameraMoveUp = 4,
        CameraMoveDown = 5,
        CameraTiltLeft = 6,
        CameraTiltRight = 7,
        CameraRotateLeft = 8,
        CameraRotateRight = 9,
        CameraRotateUp = 10,
        CameraRotateDown = 11,
        ToggleWireframe = 12,
        ToggleFullscreen = 13,
        ToggleFixedFramerate = 14,
        Exit = 15
    }

    public enum ControllerType
    {
        Keyboard = 0,
        Mouse = 1,
        Pad = 2
    }

    public class KeyBinding
    {
        public ControllerType Type { get; set; }
        public object Key { get; set; }

        public KeyBinding(ControllerType type, object key)
        {
            Type = type;
            Key = key;
        }
    }
}