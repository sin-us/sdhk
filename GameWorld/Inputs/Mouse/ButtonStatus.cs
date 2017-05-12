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
                if (value)
                {
                    IsPressed = !isDown;
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