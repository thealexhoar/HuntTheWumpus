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

        public enum GameState { IntroScreen, InGame, GameOver };
        public static GameState currentGameState = GameState.InGame;

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
            gameControl.LoadContent(Content);
            // Load textures for player movement left,right,and stationary
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
            switch (currentGameState)
            {
                case GameState.IntroScreen:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        currentGameState = GameState.InGame;
                    gameControl.UpdateIntro(gameTime);
                    break;
                case GameState.InGame:            
                // Update gameControl (a.k.a. playerPosition and controller input)
                // gameControl.Update also currently calls player.Update, which does the animation and updates the player position
                    gameControl.Update(gameTime);                    
                    if (Input.isKeyPressed(Keys.P))
                        currentGameState = GameState.GameOver;
                    break;
                case GameState.GameOver:
                    break;
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (currentGameState)
            {
                case GameState.IntroScreen:
                    spriteBatch.Begin();
                    gameControl.DrawIntro(spriteBatch);
                    spriteBatch.End();
                    break;
                case GameState.InGame:
                    spriteBatch.Begin();
                    GraphicsDevice.Clear(Color.AliceBlue);

                    gameControl.Draw(spriteBatch);

                    spriteBatch.End();   
                    break;
                case GameState.GameOver:
                    spriteBatch.Begin();
                    gameControl.DrawGameOver(spriteBatch);
                    spriteBatch.End();
                    break;
            }
         
            base.Draw(gameTime);
        }
    }
}
