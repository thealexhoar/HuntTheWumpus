using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace HuntTheWumpus {
    class Button : GameComponent {
        private Vector2 Position;
        private string _asset1;
        private string _asset2;
        private Texture2D texture;
        private Texture2D texture2;
        private Texture2D graphic;
        public bool pressed = false;

        public Button(Vector2 Pos, string asset1, string asset2, Game game)
            :base(game) {
            Position = Pos;
            _asset1 = asset1;
            _asset2 = asset2;
        }

        public void LoadContent(ContentManager content) {
            texture = content.Load<Texture2D>(_asset1);
            texture2 = content.Load<Texture2D>(_asset2);
            graphic = texture;
        }

        public void Update() { 
            if (Input.isLeftMouseDown() && checkMouse()) {
                graphic = texture2;
            }
            else if (Input.isLeftMouseReleased() && checkMouse()) {
                pressed = true;
                graphic = texture;
            }
            else {
                graphic = texture;
            }
        }

        private bool checkMouse() {
            if (((Input.mousePos().X >= Position.X) && (Input.mousePos().X <= Position.X + texture.Width)) &&
                ((Input.mousePos().Y >= Position.Y) && (Input.mousePos().Y <= Position.Y + texture.Height))) {
                    return true;
            }
            return false;
        }

        private void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(graphic, Position, Color.White);
        }
        
    }
}
