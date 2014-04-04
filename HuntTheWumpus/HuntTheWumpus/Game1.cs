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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static Game game;
        GameControl gameControl = new GameControl(game);

        Texture2D playerTextureLeft;
        Texture2D playerTextureRight;
        Texture2D playerTextureStanding;
        Point playerFrameSize = new Point(30, 50);
        public static Point playerCurrentFrame = new Point(0, 0);
        public static Point playerSheetSize = new Point(4, 1);


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            gameControl.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // Load textures for player movement left,right,and stationary
            playerTextureLeft = Content.Load<Texture2D>(@"Textures/MasterChief_WalkLeft");
            playerTextureRight = Content.Load<Texture2D>(@"Textures/MasterChief_WalkRight");
            playerTextureStanding = Content.Load<Texture2D>(@"Textures/MasterChief_Standing");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // Update gameControl (a.k.a. playerPosition and controller input)
            // gameControl.Update also currently calls player.Update, which does the animation and updates the player position
            gameControl.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            GraphicsDevice.Clear(Color.AliceBlue);

            // If the player is moving Left, play the left movement animation
            if (gameControl.player.speed.X < 0)
            {            
                spriteBatch.Draw(playerTextureLeft, gameControl.player.position,
                    new Rectangle(playerCurrentFrame.X * playerFrameSize.X,
                        playerCurrentFrame.Y * playerFrameSize.Y,
                        playerFrameSize.X,
                        playerFrameSize.Y), 
                        Color.White, 0, Vector2.Zero,
                1, SpriteEffects.None, 1);
            }

            // If the player is moving Right, play the right movement animation
            else if (gameControl.player.speed.X > 0)
            {
                spriteBatch.Draw(playerTextureRight, gameControl.player.position,
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
                spriteBatch.Draw(playerTextureStanding, gameControl.player.position,
                    new Rectangle(0, 0, 30, 50),
                    Color.White, 0, Vector2.Zero,
                    1, SpriteEffects.None,1);
            }


            base.Draw(gameTime);
            spriteBatch.End();
        }
    }
}
