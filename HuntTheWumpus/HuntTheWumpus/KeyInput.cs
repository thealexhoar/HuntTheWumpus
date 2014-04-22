using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HuntTheWumpus{
    public static partial class Input {
        /// <summary>
        /// Used with MouseInput to get input from HID
        /// </summary>
        private static KeyboardState oldState;
        private static KeyboardState currentState=Keyboard.GetState();

        public static void Update() {
            oldState = currentState;
            currentState = Keyboard.GetState();
            UpdateMouse();
        }
        public static bool isKeyPressed(Keys k) {
            //returns true if the key is down this tick, not last tick
            if (currentState[k] == KeyState.Down && oldState[k] == KeyState.Up) { return true; }
            return false;
        }
        public static bool isKeyDown(Keys k) {
            //returns true if the key is down this tick and last tick
            if (currentState[k] == KeyState.Down && oldState[k] == KeyState.Down) { return true; }
            return false;
        }
        public static bool isKeyUp(Keys k) {
            //returns true if the key is up this tick and last tick
            if (currentState[k] == KeyState.Up && oldState[k] == KeyState.Up) { return true; }
            return false;
        }
        public static bool isKeyReleased(Keys k) {
            //returns true if the key is up this tick and not last tick
            if (currentState[k] == KeyState.Up && oldState[k] == KeyState.Down) { return true; }
            return false;
        }
    }
}
