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
        public static Point[] vertices = { new Point(130, 3), new Point(380, 3), new Point(509, 221), new Point(380, 374), new Point(130, 374), new Point(4, 206) };
        public static List<RoomImage> roomImages;
        public static Game game;
        public static Cave cave;
        public static Player player = new Player(game);

        Texture2D introImage;
        Texture2D highscoreImage;
        public Texture2D arrow;

        public SpriteManager spriteManager;

        // GameControl class
        public GameControl(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initiali zation it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            GUIStubb graphicsInterface = new GUIStubb();
            Trivia trivia = new Trivia();
            spriteManager = new SpriteManager(game, player);
            roomImages = new List<RoomImage>();
            cave = new Cave("test.cave");
            cave._PrintStatus();
            //creates new images and asigns them to the rooms and room render/draw list
            Vector2 _position = new Vector2();
            for (int x = 0; x < cave.Width; x++) {
                for (int y = 0; y < cave.Height; y++) {
                    _position.X = (x * 380);
                    _position.Y = (y * 438) - ((x % 2) * 219);
                    cave.Rooms[x,y].image = new RoomImage(_position,"Images/hex","Images/hex",game);
                    roomImages.Add(cave.Rooms[x, y].image);
                }

            }

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
            


            // Check for keyboard input
            Input.Update();
            if (Input.isKeyDown(Keys.Left))
            {
                player.speed.X -= 3;
                player.position.X += player.speed.X;
            }
            if (Input.isKeyDown(Keys.Right))
            {
                player.speed.X += 3;
                player.position.X += player.speed.X;
            }
            if (Input.isKeyDown(Keys.Up))
            {
                player.speed.Y -= 3;
                player.position.Y += player.speed.Y;
            }
            if (Input.isKeyDown(Keys.Down))
            {
                player.speed.Y += 3;
                player.position.Y += player.speed.Y;
            }

            // if user presses buy arrows, get 3 questions from Trivia
            if (Input.isKeyDown(Keys.B))
            {
                BuyArrow();
            }

            if (Input.isKeyDown(Keys.S))
            {
                player.arrows -= 1;
                ShootWumpus();
                spriteManager.SpawnArrow(player);
                Console.WriteLine(player.arrows);
            }

            if (Input.isKeyDown(Keys.W))
            {
                EncounterWumpus();
            }

            // Update all the game objects
            // Send these updates to GUI to be drawn

            player.Update(gameTime);
            foreach (RoomImage i in roomImages){
                i.Update();
            }
            base.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            player.Draw(spriteBatch);
            foreach (RoomImage i in roomImages) {
                i.Draw(spriteBatch);
            }
        }



        public void LoadContent(ContentManager content)
        {
            introImage = content.Load<Texture2D>(@"Images/MainMenu");
            highscoreImage = content.Load<Texture2D>(@"Images/Highscores");
            arrow = content.Load<Texture2D>(@"Images/ArrowSprite");
            player.LoadContent(content);
            foreach (RoomImage i in roomImages) {
                i.LoadContent(content);
            }
        }

        /// <summary>
        /// Get # of gold from Player.
        /// if gold > # of questions needed:
        /// Sends Trivia the # of questions it needs.
        /// Trivia sends back x amount of questions in 3 string variables.
        /// </summary>
        public static void GetTrivia(int questions)
        {
            string[] questionsString = new string[questions];
            if (Player.gold > questions)
            {
                // Ask Trivia for 3 questions
                Trivia.TriviaGenerator(questions);

                Console.WriteLine("Questions Recieved");
                // Subtract the amount of gold used
                Player.gold -= questions;
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
            bool wumpusDefeated = false;

            if (wumpusDefeated)                    // Work out with trivia how to check if answered correctly
            {
                // Wumpus runs away                // Send to map that wumpus has run away, or find how other stuff
                // Define who owns WumpusRun()
            }

            else
            {
                Game1.currentGameState = Game1.GameState.GameOver;
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
            bool answeredCorrectly = true;
            if (answeredCorrectly)
            {
                player.arrows += 3;
            }
        }

        public static void ShootWumpus()
        {
            bool didHit = false;
            
            if (didHit)
            {
                Game1.currentGameState = Game1.GameState.GameOver;
            }
            else
            {
                player.arrows -= 1;
                // Wumpus runs away
            }
        }

        public void GetHint()
        {

        }

        public void UpdateIntro(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        

        public void DrawIntro(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(introImage, new Rectangle(0,0,819,460), Color.White);
        }

        public void DrawGameOver(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(highscoreImage, new Rectangle(0, 0, 819, 460), Color.White);
        }

        public Vector2 GetPlayerPosition(Player player)
        {
            return player.position;
        }
    }
}
