namespace MonoGameWorld.Inputs.Mouse
{
    public class ButtonStatus
    {
        private bool isDown;

        public bool IsPressed { get; set; }
        public bool IsDown
        {
            get
            {
                return isDown;
            }

            set
            {
                if (value == true)
                {
                    if (isDown == false)
                    {
                        IsPressed = true;
                    }
                    else
                    {
                        IsPressed = false;
                    }
                }
                else
                {
                    IsPressed = false;
                }

                isDown = value;
            }
        }
    }
}