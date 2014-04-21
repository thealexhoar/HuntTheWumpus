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


namespace HuntTheWumpus
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    /// 
    public class Player : GameComponent
    {
        // Position and speed variables which are used in the GameControl class
        public Vector2 position = new Vector2(0,0);
        public Vector2 speed = new Vector2(0,0);

        Point playerFrameSize = new Point(30, 50);
        Point playerCurrentFrame = new Point(0, 0);
        Point playerSheetSize = new Point(4, 1);
        Rectangle playerHitBox = new Rectangle(0, 0, 20, 25);
        Texture2D playerTextureLeft;
        Texture2D playerTextureRight;
        Texture2D playerTextureStanding;
        // Animation variables
        int timeSinceLastFrame = 0;
        const int millisecondsPerFrame = 50;

        

        public int arrows = 5;
        public static int gold = 5;

        public bool isInitialized
        {
            get;

            set;
        }

        public Player(Game game)
            : base(game)
        {
            this.isInitialized = true;
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        public void LoadContent(ContentManager content)
        {
            playerTextureLeft = content.Load<Texture2D>(@"Textures/MasterChief_WalkLeft");
            playerTextureRight = content.Load<Texture2D>(@"Textures/MasterChief_WalkRight");
            playerTextureStanding = content.Load<Texture2D>(@"Textures/MasterChief_Standing");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            // Animate the player (switch to next frame every 50 milliseconds)
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame -= millisecondsPerFrame;
                ++playerCurrentFrame.X;
                if (playerCurrentFrame.X >= playerSheetSize.X)
                {
                    playerCurrentFrame.X = 0;
                    ++playerCurrentFrame.Y;
                    if (playerCurrentFrame.Y >= playerSheetSize.Y)
                        playerCurrentFrame.Y = 0;
                }
            }
            base.Update(gameTime);
        }

        public bool checkLineCollision(Point l1p1, Point l1p2) {
            //l1p1 = line 1 point 1
            //l1p2 = line 1 point 2

            if (checkTwoLines(l1p1, l1p2, new Point(playerHitBox.X, playerHitBox.Y), new Point(playerHitBox.X + playerHitBox.Width, playerHitBox.Y)) ||
                checkTwoLines(l1p1, l1p2, new Point(playerHitBox.X + playerHitBox.Width, playerHitBox.Y), new Point(playerHitBox.X + playerHitBox.Width, playerHitBox.Y + playerHitBox.Height)) ||
                checkTwoLines(l1p1, l1p2, new Point(playerHitBox.X + playerHitBox.Width, playerHitBox.Y + playerHitBox.Height), new Point(playerHitBox.X, playerHitBox.Y + playerHitBox.Height)) ||
                checkTwoLines(l1p1, l1p2, new Point(playerHitBox.X, playerHitBox.Y + playerHitBox.Height), new Point(playerHitBox.X, playerHitBox.Y))
                ) {
                return true;
            }
            else {
                return false;
            }

        }

        private bool checkTwoLines(Point l1p1, Point l1p2, Point l2p1, Point l2p2) {
            int slope = (l1p2.Y - l1p1.Y) / (l1p2.X - l1p2.X);

            if ((l2p1.Y > l1p1.Y + slope * (l2p1.X - l1p1.X)) != (l2p2.Y > l1p1.Y + slope * (l2p2.X - l1p1.X))) {
                return true;
            }
            else {
                return false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (speed.X < 0)
            {
                spriteBatch.Draw(playerTextureLeft, position,
                    new Rectangle(playerCurrentFrame.X * playerFrameSize.X,
                        playerCurrentFrame.Y * playerFrameSize.Y,
                        playerFrameSize.X,
                        playerFrameSize.Y),
                        Color.White, 0, Vector2.Zero,
                1, SpriteEffects.None, 1);
            }

           // If the player is moving Right, play the right movement animation
            else if (speed.X > 0)
            {
                spriteBatch.Draw(playerTextureRight, position,
                    new Rectangle(90 - (playerCurrentFrame.X * playerFrameSize.X),
                        playerCurrentFrame.Y * playerFrameSize.Y,
                        playerFrameSize.X,
                        playerFrameSize.Y),
                        Color.White, 0, Vector2.Zero,
                1, SpriteEffects.None, 1);
            }

            // And if the player isn't moving, play the stationary animation
            else
            {
                spriteBatch.Draw(playerTextureStanding, position,
                    new Rectangle(0, 0, 30, 50),
                    Color.White, 0, Vector2.Zero,
                    1, SpriteEffects.None, 1);
            }
        }
    }
}