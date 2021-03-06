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
    public class RoomImage : GameComponent {
        public Vector2 Position;
        private string _asset1;
        private string _asset2;
        private Texture2D texture;
        private Texture2D texture2;
        private Texture2D baseEdge;
        private Texture2D[] edges;
        private Texture2D[] arrowDirections;
        private Texture2D graphic;
        private Texture2D corners;
        private Texture2D greenHex;
        private Texture2D pitHex;
        private Texture2D batHex;
        private Texture2D wumpusHex;
        private Cave.Room.Exit[] exits;
        public bool currentRoom = false;
        public bool pit = false;
        public bool bat = false;
        public bool wumpus = false;
        public bool resolved = false;
        public bool nearWumpus = false;
        public bool[] edgeDraws;
        public bool revealed = false;

        public RoomImage(Vector2 Pos, string asset1, string asset2, Game game)
            :base(game) {
            Position = Pos;
            _asset1 = asset1;
            _asset2 = asset2;
            edges = new Texture2D[6];
            arrowDirections = new Texture2D[6];
            edgeDraws = new bool[6];
            for (int i = 0; i < 6; i++) {
                edgeDraws[i] = true;
            }
        }

        public void setExits(Cave.Room.Exit[] exit) {
            exits = exit;
            foreach (Cave.Room.Exit e in exits) {
                switch (e) {
                    case Cave.Room.Exit.BL:
                        edgeDraws[0] = false;
                        break;
                    case Cave.Room.Exit.TL:
                        edgeDraws[1] = false;
                        break;
                    case Cave.Room.Exit.TM:
                        edgeDraws[2] = false;
                        break;
                    case Cave.Room.Exit.TR:
                        edgeDraws[3] = false;
                        break;
                    case Cave.Room.Exit.BR:
                        edgeDraws[4] = false;
                        break;
                    case Cave.Room.Exit.BM:
                        edgeDraws[5] = false;
                        break;
                }
            }
        }

        public void LoadContent(ContentManager content) {
            texture = content.Load<Texture2D>(_asset1);
            texture2 = content.Load<Texture2D>(_asset2);
            baseEdge = content.Load<Texture2D>("images/baseHex");
            edges[0] = content.Load<Texture2D>("images/hexBL");
            edges[1] = content.Load<Texture2D>("images/hexTL");
            edges[2] = content.Load<Texture2D>("images/hexTM");
            edges[3] = content.Load<Texture2D>("images/hexTR");
            edges[4] = content.Load<Texture2D>("images/hexBR");
            edges[5] = content.Load<Texture2D>("images/hexBM");
            arrowDirections[0] = content.Load<Texture2D>("images/ArrowBL");
            arrowDirections[1] = content.Load<Texture2D>("images/ArrowTL");
            arrowDirections[2] = content.Load<Texture2D>("images/ArrowTM");
            arrowDirections[3] = content.Load<Texture2D>("images/ArrowTR");
            arrowDirections[4] = content.Load<Texture2D>("images/ArrowBR");
            arrowDirections[5] = content.Load<Texture2D>("images/ArrowBM");
            greenHex = content.Load<Texture2D>("images/GreenHex");
            batHex = content.Load<Texture2D>("images/BatHex");
            pitHex = content.Load<Texture2D>("images/BottomlessPitTrial");
            wumpusHex = content.Load<Texture2D>("images/WumpusHex");
            graphic = texture;
            corners = content.Load<Texture2D>("images/TopHex");
        }

        public void Update() {
            if (revealed) {
                if (bat) {
                    graphic = batHex;
                }
                else if (pit && !resolved) {
                    graphic = pitHex;
                }
                else if (wumpus) {
                    graphic = wumpusHex;
                }
                else {
                    graphic = texture;
                }
            }
            else {
                graphic = texture2;
            }
        }
        public void Draw(SpriteBatch spriteBatch, bool arrows = false) {
            spriteBatch.Draw(graphic, Position, Color.White);
            if (revealed) {
                spriteBatch.Draw(baseEdge, Position, Color.White);
                spriteBatch.Draw(corners, Position, Color.White);
            }
            if (nearWumpus && revealed) {
                spriteBatch.Draw(greenHex, Position, Color.White);
            }
            for (int i = 0; i < 6; i++) {
                if (revealed) {
                    if (edgeDraws[i]) {
                        spriteBatch.Draw(edges[i], Position, Color.White);
                    }
                    else {
                        if (currentRoom && arrows) {
                            spriteBatch.Draw(arrowDirections[i], Position, Color.White);

                        }
                    }
                }
                
            }
        }
        
    }
}
