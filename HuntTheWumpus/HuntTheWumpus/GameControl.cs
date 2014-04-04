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
    public class GameControl : Microsoft.Xna.Framework.GameComponent
    {
        public static Game game;
        public Player player = new Player(game);

        // GameControl class
        public GameControl(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            GUIStubb graphicsInterface = new GUIStubb();
            Trivia trivia = new Trivia();

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// 
        /// Updates game objects
        /// Sends objects to be drawn to GUI which then draws them
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            player.speed.X = 0;
            player.speed.Y = 0;
            // Create a KeyboardState and Player object to be used when checking for key presses
            KeyboardState keyboardState = Keyboard.GetState();

            // Check for keyboard input
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                player.speed.X -= 3;
                player.position.X += player.speed.X;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                player.speed.X += 3;
                player.position.X += player.speed.X;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                player.speed.Y -= 3;
                player.position.Y += player.speed.Y;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                player.speed.Y += 3;
                player.position.Y += player.speed.Y;
            }

            // if user presses buy arrows, get 3 questions from Trivia
            if (keyboardState.IsKeyDown(Keys.B))
            {
                GetTrivia(3);
            }


            // Console.WriteLine the position to check that the position is in fact changing
            Console.WriteLine(player.position);

            // Update all the game objects
            // Send these updates to GUI to be drawn

            player.Update(gameTime);

            base.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            player.Draw(spriteBatch);
        }

        public void LoadContent(ContentManager content)
        {
            player.LoadContent(content);
        }

        /// <summary>
        /// Get # of gold from Player.
        /// if gold > # of questions needed:
        /// Sends Trivia the # of questions it needs.
        /// Trivia sends back x amount of questions in 3 string variables.
        /// </summary>
        public static void GetTrivia(int questions)
        {
            if (Player.gold > questions)
            {
                // Ask Trivia for 3 questions
                Trivia.TriviaGenerator(questions);

                Console.WriteLine("Questions Recieved");
                // Subtract the amount of gold used
                Player.gold -= questions;

                // Send new # of gold back to Player class
            }
            else
            {
                Console.WriteLine("Not enough gold");
            }
        }

        /// <summary>
        /// Run this if the user falls into a room which contains a bottomless pit.
        /// It gets and asks trivia.
        /// If user answered correctly, reset his position to the previous room he was in.
        /// </summary>
        public static void EncounterBottomlessPit()
        {
            int prevRoom = 1;
            int currentRoom = 7;
            bool triviaCorrect = true;

            GetTrivia(3);

            // Insert trivia class method which checks if user answered correctly
            // Check if trivia answered correctly

            if (triviaCorrect)
            {
                currentRoom = prevRoom;
            }
        }

        /// <summary>
        /// Run this if user enters room with bats.
        /// Sets the current room value to a random room in the map range.
        /// </summary>
        public static void EncounterBats()
        {
            Random rnd = new Random();

            // Get the current room position from map
            int currentRoom = 7;

            // Send the player to a random room in the map
            currentRoom = rnd.Next(1, 31);                  // THERE IS ONE PROBLEM WITH THIS. If the player is randomly dropped into a room with a pit, or the wumpus, however unlikely, the player will not like that.
        }

        /// <summary>
        /// If player goes into the same room as the Wumpus, call this method
        /// "Fight" with the Wumpus using trivia questions.
        /// If you win trivia against wumpus, it escapes
        /// Otherwise, you die.
        /// Get info from all objects
        /// Remove them all from the Update List
        /// Initialize highscore
        /// </summary>
        public static void EncounterWumpus()
        {
            GetTrivia(6); // if answered correctly wumpusDefeated = true
            bool wumpusDefeated = true;

            if (wumpusDefeated)
            {
                // Wumpus runs away
                // Define who owns WumpusRun()
            }

            else
            {
                // Get all information from objects (gold, arrows, time, etc.)
                // Remove all objects from update list except GUI
                // Initialize highscore object, send it previously gathered information
            }

        }

        /// <summary>
        /// If user presses the buyArrow key (to be determined) call this method
        /// Gets 3 trivia questions
        /// If answered correctly, arrows are added to the arrowCount
        /// </summary>
        public static void BuyArrow()
        {
            GetTrivia(3);

            /* if (triviaCorrect)
             * {
             *      arrows += 1;
             * } */
        }

        public static void ShootWumpus()
        {
            bool didHit = true;

            if (didHit)
            {
                // Gather info from all objects
                // Remove all objects from update list except GUI
                // Initialize highscore
            }
            else
            {
                // Wumpus runs away
            }
        }
    }
}
