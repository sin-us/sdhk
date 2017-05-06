using System;

namespace MonoGameWorld.Inputs.Mouse
{
    [Flags]
    public enum ButtonStatusFlags : byte
    {
        None = 0,
        Down = 1,
        Pressed = 2,
        Released = 4
    }
}