﻿using System;
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
    public class Button : GameComponent {
        private Vector2 Position;
        private string textAsset;
        private Texture2D texture;
        private Texture2D texture2;
        private Texture2D texture3;
        private Texture2D graphic;
        public bool pressed = false;
        public bool revealed = true;
        public int answerKey;

        public Button(Vector2 Pos, int answer, Game game)
            :base(game) {
            Position = Pos;
            answerKey = answer;
        }

        public void LoadContent(ContentManager content) {
            texture = content.Load<Texture2D>(@"Textures/Button1");
            texture2 = content.Load<Texture2D>(@"Textures/Button2");
            texture3 = content.Load<Texture2D>(@"Textures/Button3");
            graphic = texture;
        }

        public void Update() {
            //Console.WriteLine(checkMouse());
            if (Input.isLeftMouseDown() && checkMouse()) {
                graphic = texture2;
            }
            else if (checkMouse()) {
                graphic = texture3;
            }
            else {
                graphic = texture;
            }
            if (Input.isLeftMouseReleased() && checkMouse()) {
                pressed = true;
                graphic = texture;
            }
            else {
                pressed = false;    
            }
            
        }

        private bool checkMouse() {
            if (((Input.mousePos().X >= Position.X) && (Input.mousePos().X <= Position.X + texture.Width)) &&
                ((Input.mousePos().Y >= Position.Y) && (Input.mousePos().Y <= Position.Y + texture.Height))) {
                    return true;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(graphic, Position, Color.White);
        }
        
    }
}
