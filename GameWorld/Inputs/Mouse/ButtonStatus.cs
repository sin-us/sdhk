namespace MonoGameWorld.Inputs.Mouse
{
    public struct ButtonStatus
    {
        private ButtonStatusFlags buttonStateFlags;

        public bool IsDown
        {
            get { return (buttonStateFlags & ButtonStatusFlags.Down) != 0; }
            set
            {
                if (value)
                {
                    // if first time down - then it is also a Pressed case
                    if (!IsDown)
                    {
                        IsPressed = true;
                    }
                    else
                    {
                        IsPressed = false;
                    }

                    buttonStateFlags |= ButtonStatusFlags.Down;
                }
                else
                {
                    IsPressed = false;
                    buttonStateFlags &= ~ButtonStatusFlags.Down;
                }
            }
        }

        public bool IsPressed
        {
            get { return (buttonStateFlags & ButtonStatusFlags.Pressed) != 0; }
            set
            {
                if (value)
                {
                    buttonStateFlags |= ButtonStatusFlags.Pressed;
                }
                else
                {
                    buttonStateFlags &= ~ButtonStatusFlags.Pressed;
                }
            }
        }
    }
}