using Microsoft.Xna.Framework.Input;

namespace MonoGameWorld.Inputs.Keyboard
{
    public class KeyStatus
    {
        public Keys Key { get; set; }
        public bool IsPressed { get; set; }

        public KeyStatus(Keys key, bool isPressed)
        {
            Key = key;
            IsPressed = isPressed;
        }
    }
}