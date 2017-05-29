using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using MonoGameWorld.Inputs.Keyboard;
using MonoGameWorld.Inputs.Mouse;

namespace MonoGameWorld.Configurations.Input
{
    public static class InputConfigManager
    {
        public static BindingList Bindings { get; private set; } = new BindingList();

        public static void DefaultInitialize()
        {
            AddCombination("Toggle keybindings hint", ActionType.ToggleKeybindingsHint, new KeyBindingInfo(ControllerType.Keyboard, Keys.F1));
            AddCombination("Camera move forward", ActionType.CameraMoveForward, new KeyBindingInfo(ControllerType.Keyboard, Keys.W));
            AddCombination("Camera move backward", ActionType.CameraMoveBackward, new KeyBindingInfo(ControllerType.Keyboard, Keys.S));
            AddCombination("Camera move left", ActionType.CameraMoveLeft, new KeyBindingInfo(ControllerType.Keyboard, Keys.A));
            AddCombination("Camera move right", ActionType.CameraMoveRight, new KeyBindingInfo(ControllerType.Keyboard, Keys.D));
            AddCombination("Camera move up", ActionType.CameraMoveUp, new KeyBindingInfo(ControllerType.Keyboard, Keys.Space));
            AddCombination("Camera move down", ActionType.CameraMoveDown, new KeyBindingInfo(ControllerType.Keyboard, Keys.LeftControl));
            AddCombination("Camera tilt left", ActionType.CameraTiltLeft, new KeyBindingInfo(ControllerType.Keyboard, Keys.Q));
            AddCombination("Camera tilt right", ActionType.CameraTiltRight, new KeyBindingInfo(ControllerType.Keyboard, Keys.E));
            AddCombination("Camera rotate left", ActionType.CameraRotateLeft, new KeyBindingInfo(ControllerType.Keyboard, Keys.Left));
            AddCombination("Camera rotate right", ActionType.CameraRotateRight, new KeyBindingInfo(ControllerType.Keyboard, Keys.Right));
            AddCombination("Camera rotate up", ActionType.CameraRotateUp, new KeyBindingInfo(ControllerType.Keyboard, Keys.Up));
            AddCombination("Camera rotate down", ActionType.CameraRotateDown, new KeyBindingInfo(ControllerType.Keyboard, Keys.Down));
            AddCombination("Toggle wireframe", ActionType.ToggleWireframe, new KeyBindingInfo(ControllerType.Keyboard, Keys.P));
            AddCombination("Toggle fullscreen", ActionType.ToggleFullscreen, new KeyBindingInfo(ControllerType.Keyboard, Keys.T));
            AddCombination("Toggle fixed framerate", ActionType.ToggleFixedFramerate, new KeyBindingInfo(ControllerType.Keyboard, Keys.R));
            AddCombination("Exit", ActionType.Exit, new KeyBindingInfo(ControllerType.Keyboard, Keys.Escape));
        }

        private static void AddCombination(string description, ActionType actionType, KeyBindingInfo keyBinding)
        {
            Bindings.Bindings.Add(new Binding(description, actionType, keyBinding));
        }

        public static bool TryAssignCombination(string description, KeyBindingInfo keyBindingInfo)
        {
            Binding binding = Bindings.GetBindingByKeyBindingInfo(keyBindingInfo);

            if ((binding != null) && (binding.Description != description))
            {
                return false;
            }
            else
            {
                ForceAssignCombination(description, keyBindingInfo);
            }

            return true;
        }

        public static void ForceAssignCombination(string description, KeyBindingInfo keyBindingInfo)
        {
            Binding newBinding = Bindings.GetBindingByDescription(description);

            if (newBinding != null)
            {
                Binding oldBinding = Bindings.GetBindingByKeyBindingInfo(keyBindingInfo);

                if (oldBinding != null)
                {
                    oldBinding.KeyBindingInfo = new KeyBindingInfo();
                }

                newBinding.KeyBindingInfo = keyBindingInfo;
            }
        }

        public static string GetActionDescription(KeyBindingInfo keyBindingInfo)
        {
            Binding binding = Bindings.GetBindingByKeyBindingInfo(keyBindingInfo);

            if (binding != null)
            {
                return binding.Description;
            }

            return string.Empty;
        }

        public static bool IsKeyBindingDown(ActionType actionType)
        {
            KeyBindingInfo keyBindingInfo = Bindings.GetBindingByAction(actionType).KeyBindingInfo;
            bool result = false;

            if (keyBindingInfo.Type == ControllerType.Keyboard)
            {
                Keys keyboardKey = (Keys)keyBindingInfo.Key;
                result = KeyboardManager.IsKeyDown(keyboardKey);
            }
            else if (keyBindingInfo.Type == ControllerType.Mouse)
            {
                ButtonStatus mouseButton = (ButtonStatus)keyBindingInfo.Key;
                result = mouseButton.IsDown;
            }
            else if (keyBindingInfo.Type == ControllerType.Pad)
            {
                //@TODO implement
            }

            return result;
        }

        public static bool IsKeyBindingPressed(ActionType actionType)
        {
            KeyBindingInfo keyBindingInfo = Bindings.GetBindingByAction(actionType).KeyBindingInfo;
            bool result = false;
            
            if (keyBindingInfo.Type == ControllerType.Keyboard)
            {
                Keys keyboardKey = (Keys)keyBindingInfo.Key;
                result = KeyboardManager.IsKeyPressed(keyboardKey);
            }
            else if (keyBindingInfo.Type == ControllerType.Mouse)
            {
                ButtonStatus mouseButton = (ButtonStatus)keyBindingInfo.Key;
                result = mouseButton.IsPressed;
            }
            else if (keyBindingInfo.Type == ControllerType.Pad)
            {
                //@TODO implement
            }

            return result;
        }
    }
}