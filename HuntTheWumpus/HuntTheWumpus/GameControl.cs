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
    /// This is a game component that implements IUpdateable
    /// </summary>
    /// 
    enum State 
    {
        MOVING,
        SWITCHING
    }

    public class GameControl : Microsoft.Xna.Framework.GameComponent
    {    
        // Add a SpriteFont object to display text
        SpriteFont consolas;
        Vector2 fontPos;

        public static Point[] vertices = { new Point(4, 256), new Point(130, 38), new Point(380, 38), new Point(509, 256), new Point(380, 474), new Point(130, 474)};
        public static List<RoomImage> roomImages;
        public static List<Sprite> displaySprites;
        public static Game game;
        public static Cave cave;
        public static Player player = new Player(game);

        public string triviaString;

        ScoreHandler scoreHandler;

        State state = State.MOVING;
        Vector2 moveVector;
        int moveCounter;
        ScoreHandler.Score score = new ScoreHandler.Score();

        Texture2D background;
        Texture2D introImage;
        Texture2D highscoreImage;
        Texture2D selectionImage1;
        Texture2D selectionImage2;
        Texture2D selectionImage3;
        Texture2D selectionImage;
        public Texture2D arrow;
        public int roomSwitch;

        public byte currentSelectionBox = 0;
        Vector2 selectionImagePos;

        public SpriteManager spriteManager;

        bool oldKeyboardState;
        bool currentKeyboardState;

        bool oldKeyboardStateUp;
        bool currentKeyboardStateUp;

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

            scoreHandler = new ScoreHandler(score);
            Console.WriteLine("ScoreHandler: " + scoreHandler.HighScores);

            GUIStubb graphicsInterface = new GUIStubb();
            Trivia trivia = new Trivia();
            spriteManager = new SpriteManager(game, player);
            roomImages = new List<RoomImage>();
            displaySprites = new List<Sprite>();
            cave = new Cave("test.cave");
            //cave = new Cave("test.cave")
            //cave._GetStatusString();
            //creates new images and asigns them to the rooms and room render/draw list
            Vector2 _position = new Vector2();
            for (int x = 0; x < cave.Width; x++) {
                for (int y = 0; y < cave.Height; y++) {
                    _position.X = (x * 380);
                    _position.Y = (y * 438) - ((x % 2) * 219);
                    cave.Rooms[x,y].image = new RoomImage(_position,"Images/hex","Images/hex2",game);
                    cave.Rooms[x, y].image.setExits(cave.Rooms[x, y].Exits);
                    roomImages.Add(cave.Rooms[x, y].image);
                }
            }

            cave.Rooms[cave.locationPlayer.X, cave.locationPlayer.Y].image.revealed = true;
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// 
        /// Updates game objects
        /// Sends objects to be drawn to GUI which then draws them
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime) {
            // TODO: Add your update code here
            if (state == State.MOVING) {
                player.speed.X = 0;
                player.speed.Y = 0;

                // Check for keyboard input
                Input.Update();
                //if (Input.isKeyDown(Keys.T))
                    //triviaString = GetTrivia(3);

                if (Input.isKeyDown(Keys.Left) || Input.isKeyDown(Keys.A))
                    player.speed.X -= 3;

                if (Input.isKeyDown(Keys.Right) || Input.isKeyDown(Keys.D))
                    player.speed.X += 3;
                
                if (Input.isKeyDown(Keys.Up) || Input.isKeyDown(Keys.W))
                    player.speed.Y -= 3;

                if (Input.isKeyDown(Keys.Down) || Input.isKeyDown(Keys.S))
                    player.speed.Y += 3;
                

                // if user presses buy arrows, get 3 questions from Trivia
                if (Input.isKeyDown(Keys.B)) {
                    BuyArrow();
                }

                if (Input.isKeyPressed(Keys.F)) 
                {
                    if (player.arrows <= 0)
                    {

                    }
                    else
                    {
                        player.arrows -= 1;
                        ShootWumpus();
                        spriteManager.SpawnArrow(player);
                    }
                        
                }

                if (Input.isKeyDown(Keys.R)) {
                    EncounterWumpus();
                }
                // Update all the game objects
                // Send these updates to GUI to be drawn

                player.position += player.speed;

                // WHAT DOES THIS CODE DO???? 
                //Glad you asked. It resolves collisions with walls in a 95% reliable way
                //If collisions are with a doorway in the current room, there can be code to switch rooms


                Point lastpoint;
                Point thispoint;

                thispoint = vertices[5];
                for (int i = 0; i < 6; i++) {
                    lastpoint = new Point(thispoint.X, thispoint.Y);
                    thispoint = vertices[i];

                    if (player.checkCollision(lastpoint, thispoint)) {
                        if (cave.Rooms[cave.locationPlayer.X, cave.locationPlayer.Y].image.edgeDraws[i] == true) {
                            player.resolveCollision(lastpoint, thispoint);
                        }
                        else if (cave.Rooms[cave.locationPlayer.X, cave.locationPlayer.Y].image.edgeDraws[i] != true) {
                            state = State.SWITCHING;
                            moveVector = new Vector2(vertices[i].X - vertices[(i + 2) % 6].X, vertices[i].Y - vertices[(i + 2) % 6].Y) * -1;
                            moveCounter = 0;
                            roomSwitch = i;
                            Console.WriteLine(i);
                        }
                    }

                }
            }
                
            else if (state == State.SWITCHING) {
                    moveCounter++;
                    if (moveCounter >= 30) {
                        state = State.MOVING;
                        moveCounter = 0;
                        int dy = cave.locationPlayer.Y;
                        int dx = cave.locationPlayer.X;
                        switch (roomSwitch) {
                            case 0:                            
                                if (cave.locationPlayer.X % 2 == 0) {
                                    dx = cave.locationPlayer.X - 1;
                                    if(dx < 0){
                                        dx = cave.Width - 1;
                                    }
                                    dy = cave.locationPlayer.Y + 1;
                                    if (dy > cave.Height - 1) {
                                        dy = 0;
                                    }
                                }
                                else {
                                    dx = cave.locationPlayer.X - 1;
                                    if (dx < 0) {
                                        dx = cave.Width - 1;
                                    }
                                    dy = cave.locationPlayer.Y;
                                }
                                break;
                            case 1:

                                if (cave.locationPlayer.X % 2 == 1) {
                                    dx = cave.locationPlayer.X - 1;
                                    if (dx < 0) {
                                        dx = cave.Width - 1;
                                    }
                                    dy = cave.locationPlayer.Y - 1;
                                    if (dy < 0) {
                                        dy = cave.Height - 1;
                                    }
                                }
                                else {
                                    dx = cave.locationPlayer.X - 1;
                                    if (dx < 0) {
                                        dx = cave.Width - 1;
                                    }
                                    dy = cave.locationPlayer.Y;
                                }
                                break;
                            case 2:

                                dx = cave.locationPlayer.X;
                                dy = cave.locationPlayer.Y - 1;
                                if (dy < 0) {
                                    dy = cave.Height-1;
                                }
                                break;
                            case 3:
                                
                                if (cave.locationPlayer.X % 2 == 1) {
                                    dx = cave.locationPlayer.X + 1;
                                    if (dx > cave.Width-1) {
                                        dx = 0;
                                    }
                                    dy = cave.locationPlayer.Y - 1;
                                    if (dy < 0) {
                                        dy = cave.Width - 1;
                                    }
                                }
                                else {
                                    dx = cave.locationPlayer.X + 1;
                                    if (dx > cave.Width-1) {
                                        dx = 0;
                                    }
                                    dy = cave.locationPlayer.Y;
                                }
                                break;
                            case 4:
                                if (cave.locationPlayer.X % 2 == 0) {
                                    dx = cave.locationPlayer.X + 1;
                                    if (dx > cave.Width-1) {
                                        dx = 0;
                                    }
                                    dy = cave.locationPlayer.Y + 1;
                                    if (dy > cave.Width -1) {
                                        dy = 0;
                                    }
                                }
                                else {
                                    dx = cave.locationPlayer.X + 1;
                                    if (dx > cave.Width-1) {
                                        dx = 0;
                                    }
                                    dy = cave.locationPlayer.Y;
                                }
                                break;
                            case 5:
                                dx = cave.locationPlayer.X;
                                dy = cave.locationPlayer.Y + 1;
                                if (dy > cave.Height-1) {
                                    dy = 0;
                                }
                                break;
                        }

                        cave.movePlayer(dx, dy);
                        if (!cave.locationPlayer.image.revealed) {
                            player.addGold((int)cave.locationPlayer.Gold);
                        }
                        cave.locationPlayer.image.revealed = true;
                        



                    }

                    player.position += (moveVector / 60);
                    foreach (RoomImage r in roomImages) {
                        r.Position += (moveVector / 30);
                    }
            }

            player.Update(gameTime);
            foreach (RoomImage r in roomImages) {
                r.Update();
            }


            foreach (RoomImage r in roomImages) {
                if (r.Position.X < -380 * 3) {
                    r.Position.X += 6 * 380;
                }
                else if (r.Position.X > 380 * 3) {
                    r.Position.X -= 6 * 380;
                }
                if (r.Position.Y < -438 * 2) {
                    r.Position.Y += 438 * 5;
                }
                else if (r.Position.Y > 438 * 3) {
                    r.Position.Y -= 438 * 5;
                }
            }

                base.Update(gameTime);
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            string output = "Arrows: " + player.arrows;
            string hint = " ";
            string triviaString = " ";
            spriteBatch.Draw(background, new Vector2(), Color.White);
            foreach (RoomImage i in roomImages) {
                i.Draw(spriteBatch);
            }
            // Draw the string in trivia at position 300, 600 in golden consolas font
            spriteBatch.DrawString(consolas, triviaString, new Vector2(300, 600), Color.Gold);
            // Draw the string in hint at position 900,10 in golden consolas font
            spriteBatch.DrawString(consolas, hint, new Vector2(900, 50), Color.Gold);
            // Draw the string in output at position 10,10 in golden consolas font
            spriteBatch.DrawString(consolas, output, fontPos, Color.Gold);
            // Draw the coins# at position (10,30) in golden consolas font
            spriteBatch.DrawString(consolas, "Coins: " + Player.gold, new Vector2(10, 30), Color.Gold);
            player.Draw(spriteBatch);
        }



        public void LoadContent(ContentManager content)
        {
            selectionImage1 = content.Load<Texture2D>(@"Textures/selection1");
            selectionImage2 = content.Load<Texture2D>(@"Textures/selection2");
            selectionImage3 = content.Load<Texture2D>(@"Textures/selection3");
            selectionImage = selectionImage1;
            consolas = content.Load<SpriteFont>(@"Consolas");
            introImage = content.Load<Texture2D>(@"Textures/Menu");
            highscoreImage = content.Load<Texture2D>(@"Images/Highscores");
            arrow = content.Load<Texture2D>(@"Images/ArrowSprite");
            background = content.Load<Texture2D>(@"Images/SpaceBackground");
            player.LoadContent(content);
            foreach (RoomImage i in roomImages) {
                i.LoadContent(content);
            }

            fontPos = new Vector2(10, 10);
            Sprite selectionBox = new Sprite(selectionImage, new Vector2(10,10), new Point(10,10),0,new Point(0,0),new Point(0,0),new Vector2 (0,0));            
            displaySprites.Add(selectionBox);
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
            if (answeredCorrectly && Player.gold > 0)
            {
                player.arrows += 1;
                Player.gold -= 1;
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
                // Wumpus runs away
            }
        }

        public void GetHint()
        {

        }

        public void UpdateIntro(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                currentKeyboardState = Keyboard.GetState().IsKeyDown(Keys.Down);
                if (currentKeyboardState == oldKeyboardState)
                {

                }
                else if (currentKeyboardState != oldKeyboardState)
                {
                    if (currentSelectionBox < 3)
                        currentSelectionBox += 1;
                    if (currentSelectionBox >= 3)
                        currentSelectionBox = 0;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                currentKeyboardStateUp = Keyboard.GetState().IsKeyDown(Keys.Up);
                if (currentKeyboardStateUp == oldKeyboardStateUp)
                {

                }
                else if (currentKeyboardStateUp != oldKeyboardStateUp)
                {
                    if (currentSelectionBox < 3 || currentSelectionBox >= 0)
                        currentSelectionBox -= 1;
                    if (currentSelectionBox > 3)
                        currentSelectionBox = 2;
                    Console.WriteLine(currentSelectionBox);
                }
            }   
            switch (currentSelectionBox)
                {
                    case (0):
                    selectionImage = selectionImage1;
                        break;
                    case (1):
                        selectionImage = selectionImage2;
                        break;
                    case (2):
                        selectionImage = selectionImage3;
                        break;
                }
            oldKeyboardState = Keyboard.GetState().IsKeyDown(Keys.Down);
            oldKeyboardStateUp = Keyboard.GetState().IsKeyDown(Keys.Up);
            base.Update(gameTime);
        }

        

        public void DrawIntro(SpriteBatch spriteBatch)
        {            
            foreach (Sprite x in displaySprites)
            {
                x.Draw(spriteBatch);
            }

            spriteBatch.Draw(introImage, new Vector2(), Color.White);
            spriteBatch.Draw(selectionImage, new Vector2(), Color.White);
        }

        public void DrawGameOver(SpriteBatch spriteBatch)
        {
            foreach(ScoreHandler.Score x in scoreHandler.HighScores)
            {
                spriteBatch.DrawString(consolas, "Name: " + Convert.ToString(x), new Vector2(50, 50), Color.Gold);
            }
            
            spriteBatch.Draw(highscoreImage, new Rectangle(0, 0, 819, 460), Color.White);
        }
    }
}
