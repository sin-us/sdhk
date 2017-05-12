using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameWorld.Inputs.Mouse
{
    public class MouseStatus
    {
        public ButtonStatus LeftButton { get; private set; }
        public ButtonStatus MiddleButton { get; private set; }
        public ButtonStatus RightButton { get; private set; }
        public ButtonStatus XButton1 { get; private set; }
        public ButtonStatus XButton2 { get; private set; }
        public Point Position { get; private set; }
        public int WheelCumulativeValue { get; private set; }
        public int DeltaX { get; private set; }
        public int DeltaY { get; private set; }
        public int WheelDelta { get; private set; }

        public MouseStatus()
        {
            LeftButton = new ButtonStatus();
            MiddleButton = new ButtonStatus();
            RightButton = new ButtonStatus();
            XButton1 = new ButtonStatus();
            XButton2 = new ButtonStatus();
            Position = new Point();
        }

        public void FromXnaMouseState(MouseState mouseState)
        {
            DeltaX = mouseState.X - Position.X;
            DeltaY = mouseState.Y - Position.Y;
            WheelDelta = mouseState.ScrollWheelValue - WheelCumulativeValue;

            Position = mouseState.Position;
            WheelCumulativeValue = mouseState.ScrollWheelValue;

            LeftButton.IsDown = (mouseState.LeftButton == ButtonState.Pressed);
            MiddleButton.IsDown = (mouseState.MiddleButton == ButtonState.Pressed);
            RightButton.IsDown = (mouseState.RightButton == ButtonState.Pressed);
            XButton1.IsDown = (mouseState.XButton1 == ButtonState.Pressed);
            XButton2.IsDown = (mouseState.XButton2 == ButtonState.Pressed);
        }
    }
}