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
        public static GameControl gameControl = new GameControl(game);
        public enum GameState { IntroScreen, InGame, GameOver };
        public static GameState currentGameState = GameState.IntroScreen;

        public static SpriteManager spriteManager;
        Song background;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = true;
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
        }

        protected override void Initialize()
        {
            gameControl.Initialize();
            Components.Add(gameControl.spriteManager);
            IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Song>("iamthewumpus");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(background);

            // TODO: use this.Content to load your game content here
            gameControl.LoadContent(Content);
            // Load textures for player movement left,right,and stationary
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (Input.isKeyPressed(Keys.M))
            {
                if (MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Pause();
                else
                    MediaPlayer.Resume();
            }

            switch (currentGameState)
            {
                case GameState.IntroScreen:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        switch (gameControl.currentSelectionBox)
                        {
                            case(0):
                                currentGameState = GameState.InGame;
                                break;
                            case(1):
                                currentGameState = GameState.GameOver;
                                break;
                            case(2):
                                this.Exit();
                                break;
                        }
                    }
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
                    if (Keyboard.GetState().IsKeyDown(Keys.Q))
                        currentGameState = GameState.IntroScreen;
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
                    GraphicsDevice.Clear(Color.Black);
                    gameControl.DrawIntro(spriteBatch);
                    spriteBatch.End();
                    break;
                case GameState.InGame:
                    spriteBatch.Begin();
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    gameControl.Draw(spriteBatch);

                    spriteBatch.End();   
                    break;
                case GameState.GameOver:
                    spriteBatch.Begin();
                    GraphicsDevice.Clear(Color.Black);
                    gameControl.DrawGameOver(spriteBatch);
                    spriteBatch.End();
                    break;
            }
         
            base.Draw(gameTime);
        }
    }
}
