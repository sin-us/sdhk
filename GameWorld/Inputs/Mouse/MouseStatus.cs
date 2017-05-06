using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameWorld.Inputs.Mouse
{
    public class MouseStatus
    {
        private ButtonStatus leftButton;
        private ButtonStatus middleButton;
        private ButtonStatus rightButton;
        private ButtonStatus xButton1;
        private ButtonStatus xButton2;
        private Point position;
        private int wheelCumulativeValue;
        private int deltaX;
        private int deltaY;
        private int wheelDelta;

        public ButtonStatus LeftButton => leftButton;
        public ButtonStatus MiddleButton => middleButton;
        public ButtonStatus RightButton => rightButton;
        public ButtonStatus XButton1 => xButton1;
        public ButtonStatus XButton2 => xButton2;
        public Point Position => position;
        public int WheelCumulativeValue => wheelCumulativeValue;
        public int DeltaX => deltaX;
        public int DeltaY => deltaY;
        public int WheelDelta => wheelDelta;

        public void FromXnaMouseState(MouseState mouseState)
        {
            deltaX = mouseState.X - position.X;
            deltaY = mouseState.Y - position.Y;
            wheelDelta = mouseState.ScrollWheelValue - wheelCumulativeValue;

            position = mouseState.Position;
            wheelCumulativeValue = mouseState.ScrollWheelValue;

            leftButton.IsDown = (mouseState.LeftButton == ButtonState.Pressed);
            middleButton.IsDown = (mouseState.MiddleButton == ButtonState.Pressed);
            rightButton.IsDown = (mouseState.RightButton == ButtonState.Pressed);
            xButton1.IsDown = (mouseState.XButton1 == ButtonState.Pressed);
            xButton2.IsDown = (mouseState.XButton2 == ButtonState.Pressed);
        }
    }
}