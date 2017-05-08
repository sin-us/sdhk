using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace MonoGameWorld.Inputs.Keyboard
{
    public static class KeyboardManager
    {
        private static List<KeyStatus> keysDown = new List<KeyStatus>();

        public static List<KeyStatus> KeysDown => keysDown;

        public static bool IsKeyDown(Keys key)
        {
            for (int i = 0; i < keysDown.Count; ++i)
            {
                if (keysDown[i].Key == key)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsKeyUp(Keys key)
        {
            for (int i = 0; i < keysDown.Count; ++i)
            {
                if (keysDown[i].Key == key)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsKeyPressed(Keys key)
        {
            for (int i = 0; i < keysDown.Count; ++i)
            {
                if ((keysDown[i].Key == key) && (keysDown[i].IsPressed == true))
                {
                    return true;
                }
            }

            return false;
        }

        public static void Update()
        {
            Keys[] pressedKeys = Microsoft.Xna.Framework.Input.Keyboard.GetState().GetPressedKeys();

            bool isPressed = false;
            List<KeyStatus> newKeyStatusList = new List<KeyStatus>();

            foreach (Keys key in pressedKeys)
            {
                isPressed = true;

                foreach (KeyStatus oldKeyStatus in keysDown)
                {
                    if (key == oldKeyStatus.Key)
                    {
                        isPressed = false;
                        break;
                    }
                }

                newKeyStatusList.Add(new KeyStatus(key, isPressed));
            }

            keysDown = newKeyStatusList;
        }
    }
}