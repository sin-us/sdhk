using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameWorld.Inputs.Mouse
{
   [Flags]
   public enum ButtonStatusFlags : byte
   {
      None = 0,
      Down = 1,
      Pressed = 2,
      Released = 4
   }
}