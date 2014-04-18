using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HuntTheWumpus{
    public static partial class Input {
        /// <summary>
        /// Used with KeyInput to get input from HID
        /// </summary>
        /// <returns></returns>
        public static bool isLeftMouseDown() {
            if (Mouse.GetState().LeftButton.Equals(ButtonState.Pressed)) {
                return true;
            }
            return false;
        }

        public static bool isRightMouseDown() {
            if (Mouse.GetState().RightButton.Equals(ButtonState.Pressed)) {
                return true;
            }
            return false;
        }

        public static Vector2 getPos() {
            return new Vector2((float)Mouse.GetState().X, (float)Mouse.GetState().Y);
        }
    }
}
