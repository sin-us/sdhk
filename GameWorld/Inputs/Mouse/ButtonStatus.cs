using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

               IsReleased = false;
               buttonStateFlags |= ButtonStatusFlags.Down;
            }
            else
            {
               IsPressed = false;
               IsReleased = true;
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

      public bool IsReleased
      {
         get { return (buttonStateFlags & ButtonStatusFlags.Released) != 0; }
         set
         {
            if (value)
            {
               buttonStateFlags |= ButtonStatusFlags.Released;
            }
            else
            {
               buttonStateFlags &= ~ButtonStatusFlags.Released;
            }
         }
      }
   }
}
