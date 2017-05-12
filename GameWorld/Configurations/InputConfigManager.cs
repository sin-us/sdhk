using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using MonoGameWorld.Inputs.Keyboard;
using MonoGameWorld.Inputs.Mouse;

namespace MonoGameWorld.Configurations
{
    public static class InputConfigManager
    {
        private static Dictionary<string, ActionType> descriptionToActionBinding = new Dictionary<string, ActionType>();
        private static Dictionary<ActionType, List<KeyBinding>> actionToKeyBindings = new Dictionary<ActionType, List<KeyBinding>>();

        public static Dictionary<string, ActionType> DescriptionToActionBinding { get { return descriptionToActionBinding; } }
        public static Dictionary<ActionType, List<KeyBinding>> ActionToKeyBindings { get { return actionToKeyBindings; } }

        public static void DefaultInitialization()
        {
            List<KeyBinding> keyBindingList;

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.W) };
            AddDescriptionActionKeyBinding("Camera move forward", ActionType.CameraMoveForward, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.S) };
            AddDescriptionActionKeyBinding("Camera move backward", ActionType.CameraMoveBackward, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.A) };
            AddDescriptionActionKeyBinding("Camera move left", ActionType.CameraMoveLeft, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.D) };
            AddDescriptionActionKeyBinding("Camera move right", ActionType.CameraMoveRight, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.Space) };
            AddDescriptionActionKeyBinding("Camera move up", ActionType.CameraMoveUp, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.LeftControl) };
            AddDescriptionActionKeyBinding("Camera move down", ActionType.CameraMoveDown, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.Q) };
            AddDescriptionActionKeyBinding("Camera tilt left", ActionType.CameraTiltLeft, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.E) };
            AddDescriptionActionKeyBinding("Camera tilt right", ActionType.CameraTiltRight, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.Left) };
            AddDescriptionActionKeyBinding("Camera rotate left", ActionType.CameraRotateLeft, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.Right) };
            AddDescriptionActionKeyBinding("Camera rotate right", ActionType.CameraRotateRight, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.Up) };
            AddDescriptionActionKeyBinding("Camera rotate up", ActionType.CameraRotateUp, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.Down) };
            AddDescriptionActionKeyBinding("Camera rotate down", ActionType.CameraRotateDown, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.P) };
            AddDescriptionActionKeyBinding("Toggle wireframe", ActionType.ToggleWireframe, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.T) };
            AddDescriptionActionKeyBinding("Toggle fullscreen", ActionType.ToggleFullscreen, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.R) };
            AddDescriptionActionKeyBinding("Toggle fixed framerate", ActionType.ToggleFixedFramerate, keyBindingList);

            keyBindingList = new List<KeyBinding>() { new KeyBinding(ControllerType.Keyboard, Keys.Escape) };
            AddDescriptionActionKeyBinding("Exit", ActionType.Exit, keyBindingList);
        }

        private static void AddDescriptionActionKeyBinding(string description, ActionType actionType, List<KeyBinding> keyBindingList)
        {
            descriptionToActionBinding.Add(description, actionType);
            actionToKeyBindings.Add(actionType, keyBindingList);
        }

        public static string AssignKeyToAction(string description, List<KeyBinding> keyBindingList, bool isForceAssign)
        {
            ActionType actionType = descriptionToActionBinding[description];

            if (isForceAssign == false)
            {
                foreach (var actionToKeyPair in actionToKeyBindings)
                {
                    if ((actionToKeyPair.Value == keyBindingList) && (actionToKeyPair.Key != actionType))
                    {
                        foreach (var descriptionToActionPair in descriptionToActionBinding)
                        {
                            if (descriptionToActionPair.Value == actionToKeyPair.Key)
                            {
                                return descriptionToActionPair.Key;
                            }
                        }
                    }
                }
            }
            else
            {
                actionToKeyBindings[actionType] = keyBindingList;
            }

            return "";
        }

        public static bool IsActionKeysDown(ActionType actionType)
        {
            List<KeyBinding> keyBindingList = actionToKeyBindings[actionType];
            bool result = true;

            foreach (KeyBinding keyBinding in keyBindingList)
            {
                if (keyBinding.Type == ControllerType.Keyboard)
                {
                    Keys keyboardKey = (Keys)keyBinding.Key;
                    if (KeyboardManager.IsKeyDown(keyboardKey) == false)
                    {
                        result = false;
                        break;
                    }
                }
                else if (keyBinding.Type == ControllerType.Mouse)
                {
                    ButtonStatus mouseButton = (ButtonStatus)keyBinding.Key;
                    if (mouseButton.IsDown == false)
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

        public static bool IsActionKeysPressed(ActionType actionType)
        {
            List<KeyBinding> keyBindingList = actionToKeyBindings[actionType];

            bool isAllKeysDown = true;
            bool isAnyKeyPressed = false;
            bool result = false;

            foreach (KeyBinding keyBinding in keyBindingList)
            {
                if (keyBinding.Type == ControllerType.Keyboard)
                {
                    Keys keyboardKey = (Keys)keyBinding.Key;
                    if (KeyboardManager.IsKeyDown(keyboardKey) == true)
                    {
                        if (KeyboardManager.IsKeyPressed(keyboardKey) == true)
                        {
                            isAnyKeyPressed = true;
                        }
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
                    if (mouseButton.IsDown == true)
                    {
                        if (mouseButton.IsPressed == true)
                        {
                            isAnyKeyPressed = true;
                        }
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

            result = (isAllKeysDown && isAnyKeyPressed);
            return result;
        }
    }
}