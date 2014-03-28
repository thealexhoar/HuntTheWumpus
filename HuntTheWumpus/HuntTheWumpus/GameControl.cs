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

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

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

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// 
        /// Updates game objects
        /// Sends objects to be Drawn to GUI which then draws them
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            // Update all the game objects
            // Send these updates to GUI to be drawn

            base.Update(gameTime);
        }

        /// <summary>
        /// Get # of gold from Player.
        /// if gold > # of questions needed:
        /// Sends Trivia the # of questions it needs.
        /// Trivia sends back x amount of questions in 3 string variables.
        /// </summary>
        public static void GetTrivia(int questions)
        {
            // Get # of gold from player
            int gold = 0;
            if (gold > questions)
            {
                // Ask Trivia for 3 questions

                // Subtract the amount of gold used
                gold -= questions;

                // Send new # of gold back to Player class
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
            GetTrivia(6);
            bool wumpusDefeated = true;

            if (wumpusDefeated)
            {
                // Wumpus runs away
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
