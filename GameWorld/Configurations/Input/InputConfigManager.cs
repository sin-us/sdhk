using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using MonoGameWorld.Inputs.Keyboard;
using MonoGameWorld.Inputs.Mouse;

namespace MonoGameWorld.Configurations.Input
{
    public static class InputConfigManager
    {
        public static CombinationList Combinations { get; private set; } = new CombinationList();

        public static void DefaultInitialize()
        {
            List<KeyBinding> keyBindingList;

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.W) };
            AddCombination("Camera move forward", ActionType.CameraMoveForward, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.S) };
            AddCombination("Camera move backward", ActionType.CameraMoveBackward, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.A) };
            AddCombination("Camera move left", ActionType.CameraMoveLeft, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.D) };
            AddCombination("Camera move right", ActionType.CameraMoveRight, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.Space) };
            AddCombination("Camera move up", ActionType.CameraMoveUp, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.LeftControl) };
            AddCombination("Camera move down", ActionType.CameraMoveDown, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.Q) };
            AddCombination("Camera tilt left", ActionType.CameraTiltLeft, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.E) };
            AddCombination("Camera tilt right", ActionType.CameraTiltRight, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.Left) };
            AddCombination("Camera rotate left", ActionType.CameraRotateLeft, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.Right) };
            AddCombination("Camera rotate right", ActionType.CameraRotateRight, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.Up) };
            AddCombination("Camera rotate up", ActionType.CameraRotateUp, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.Down) };
            AddCombination("Camera rotate down", ActionType.CameraRotateDown, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.P) };
            AddCombination("Toggle wireframe", ActionType.ToggleWireframe, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.T) };
            AddCombination("Toggle fullscreen", ActionType.ToggleFullscreen, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.R) };
            AddCombination("Toggle fixed framerate", ActionType.ToggleFixedFramerate, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.Escape) };
            AddCombination("Exit", ActionType.Exit, keyBindingList);
        }

        private static void AddCombination(string description, ActionType actionType, List<KeyBinding> keyBindingList)
        {
            Combinations.Combinations.Add(new Combination(description, actionType, keyBindingList));
        }

        public static bool TryAssignCombination(string description, List<KeyBinding> keyBindingList)
        {
            Combination combination = Combinations.GetCombinationByBinding(keyBindingList);

            if ((combination != null) && (combination.Description != description))
            {
                return false;
            }
            else
            {
                ForceAssignCombination(description, keyBindingList);
            }

            return true;
        }

        public static void ForceAssignCombination(string description, List<KeyBinding> keyBindingList)
        {
            Combination newCombination = Combinations.GetCombinationByDescription(description);
            Combination oldCombination = Combinations.GetCombinationByBinding(keyBindingList);

            if (newCombination != null)
            {
                newCombination.Binding = keyBindingList;

                if (oldCombination != null)
                {
                    oldCombination.Binding = new List<KeyBinding>();
                }
            }
        }

        public static string GetActionDescription(List<KeyBinding> keyBindingList)
        {
            Combination combination = Combinations.GetCombinationByBinding(keyBindingList);

            if (combination != null)
            {
                return combination.Description;
            }

            return string.Empty;
        }

        public static bool IsKeyCombinationDown(ActionType actionType)
        {
            List<KeyBinding> keyBindingList = Combinations.GetCombinationByAction(actionType).Binding;
            bool result = true;

            foreach (KeyBinding keyBinding in keyBindingList)
            {
                if (keyBinding.Type == ControllerType.Keyboard)
                {
                    Keys keyboardKey = (Keys)keyBinding.Key;
                    if (!KeyboardManager.IsKeyDown(keyboardKey))
                    {
                        result = false;
                        break;
                    }
                }
                else if (keyBinding.Type == ControllerType.Mouse)
                {
                    ButtonStatus mouseButton = (ButtonStatus)keyBinding.Key;
                    if (!mouseButton.IsDown)
                    {
                        result = false;
                        break;
                    }
                }
                else if (keyBinding.Type == ControllerType.Pad)
                {
                    //@TODO implement
                }
            }

            return result;
        }

        public static bool IsKeyCombinationPressed(ActionType actionType)
        {
            List<KeyBinding> keyBindingList = Combinations.GetCombinationByAction(actionType).Binding;

            bool isAllKeysDown = true;
            bool isAnyKeyPressed = false;

            foreach (KeyBinding keyBinding in keyBindingList)
            {
                if (keyBinding.Type == ControllerType.Keyboard)
                {
                    Keys keyboardKey = (Keys)keyBinding.Key;
                    if (KeyboardManager.IsKeyDown(keyboardKey))
                    {
                        isAnyKeyPressed = KeyboardManager.IsKeyPressed(keyboardKey);
                    }
                    else
                    {
                        isAllKeysDown = false;
                        break;
                    }
                }
                else if (keyBinding.Type == ControllerType.Mouse)
                {
                    ButtonStatus mouseButton = (ButtonStatus)keyBinding.Key;
                    if (mouseButton.IsDown)
                    {
                        isAnyKeyPressed = mouseButton.IsPressed;
                    }
                    else
                    {
                        isAllKeysDown = false;
                        break;
                    }
                }
                else if (keyBinding.Type == ControllerType.Pad)
                {
                    //@TODO implement
                }
            }

            return (isAllKeysDown && isAnyKeyPressed);
        }
    }
}