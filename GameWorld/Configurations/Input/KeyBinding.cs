using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameWorld.Configurations.Input
{
    public enum ActionType
    {
        None,
        CameraMoveForward,
        CameraMoveBackward,
        CameraMoveLeft,
        CameraMoveRight,
        CameraMoveUp,
        CameraMoveDown,
        CameraTiltLeft,
        CameraTiltRight,
        CameraRotateLeft,
        CameraRotateRight,
        CameraRotateUp,
        CameraRotateDown,
        ToggleWireframe,
        ToggleFullscreen,
        ToggleFixedFramerate,
        Exit,
    }

    public enum ControllerType
    {
        Keyboard,
        Mouse,
        Pad,
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