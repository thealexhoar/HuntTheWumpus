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
    public class Player : Microsoft.Xna.Framework.GameComponent
    {
        // Animation variables
        int timeSinceLastFrame = 0;
        const int millisecondsPerFrame = 50;

        // Position and speed variables which are used in the GameControl class
        public Vector2 position = new Vector2(0,0);
        public Vector2 speed = new Vector2(0,0);

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
                ++Game1.playerCurrentFrame.X;
                if (Game1.playerCurrentFrame.X >= Game1.playerSheetSize.X)
                {
                    Game1.playerCurrentFrame.X = 0;
                    ++Game1.playerCurrentFrame.Y;
                    if (Game1.playerCurrentFrame.Y >= Game1.playerSheetSize.Y)
                        Game1.playerCurrentFrame.Y = 0;
                }
            }
            base.Update(gameTime);
        }
    }
}