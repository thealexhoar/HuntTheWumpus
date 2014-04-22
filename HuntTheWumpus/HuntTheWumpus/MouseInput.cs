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

        private static MouseState mouseState;
        private static MouseState oldMouseState;
        private static void UpdateMouse() {
            oldMouseState = mouseState;
            mouseState = Mouse.GetState();
        }

        public static Vector2 mousePos() {
            return new Vector2(mouseState.X, mouseState.Y);
        }

        public static bool isLeftMouseDown() {
            if (mouseState.LeftButton.Equals(ButtonState.Pressed)) {
                return true;
            }
            return false;
        }

        public static bool isRightMouseDown() {
            if (mouseState.RightButton.Equals(ButtonState.Pressed)) {
                return true;
            }
            return false;
        }
        public static bool isLeftMousePressed() {
            if (mouseState.LeftButton.Equals(ButtonState.Pressed) && oldMouseState.LeftButton.Equals(ButtonState.Released)) {
                return true;
            }
            return false;
        }

        public static bool isLeftMouseReleased() {
            if (mouseState.LeftButton.Equals(ButtonState.Released) && oldMouseState.LeftButton.Equals(ButtonState.Pressed)) {
                return true;
            }
            return false;
        }

        public static Vector2 getPos() {
            return new Vector2((float)Mouse.GetState().X, (float)Mouse.GetState().Y);
        }
    }
}
