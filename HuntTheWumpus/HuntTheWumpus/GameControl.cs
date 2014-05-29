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
    public enum State 
    {
        MOVING,
        SWITCHING,
        TRANSPORTED,
        QUESTIONING
    }

    public class GameControl : Microsoft.Xna.Framework.GameComponent
    {
        public List<Sprite> spriteList = new List<Sprite>();
        // Add a SpriteFont object to display text
        SpriteFont consolas;
        Vector2 fontPos;
        Trivia.Question[] questions;
        int triviaResults = 0;
        int triviaResultsNeeded = 0;
        bool triviaSucceeded;
        int triviaCount, triviaMax;
        Func<bool> triviaResolve;
        public Button[] buttons;

        public static Point[] vertices = { new Point(4, 256), new Point(130, 38), new Point(380, 38), new Point(509, 256), new Point(380, 474), new Point(130, 474)};
        public static List<RoomImage> roomImages;
        public static List<Sprite> displaySprites;
        public static Game game;
        public static Cave cave;
        public static Player player = new Player(game);

        ScoreHandler scoreHandler;

        public State state = State.MOVING;
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
        Texture2D HUD;
        public Texture2D arrow;
        public int roomSwitch;
        public Vector2 transportDelta;

        public byte currentSelectionBox = 0;

        public SpriteManager spriteManager;

        bool oldKeyboardState;
        bool currentKeyboardState;

        bool oldKeyboardStateUp;
        bool currentKeyboardStateUp;

        public static Trivia trivia = new Trivia();

        // Variables for adjacent rooms and their contents
        bool wumpus = false;
        bool pit = false;
        bool bat = false;

        public string arrowCount;
        public string hint;
        public string triviaString, answer1, answer2, answer3, answer4;

        public static bool wumpusDefeated;

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
            spriteManager = new SpriteManager(game, player);
            roomImages = new List<RoomImage>();
            displaySprites = new List<Sprite>();
            cave = new Cave("test.cave");
            Vector2 _position = new Vector2();
            cave.moveWumpus(1, 1);
            cave.Rooms[0, 0].hasBats = true;

            buttons = new Button[4];
            buttons[0] = new Button(new Vector2(5, 620), 0, game);
            buttons[1] = new Button(new Vector2(5, 655), 1, game);
            buttons[2] = new Button(new Vector2(5, 690), 2, game);
            buttons[3] = new Button(new Vector2(5, 725), 3, game);

                      

            for (int x = 0; x < cave.Width; x++) {
                for (int y = 0; y < cave.Height; y++) {
                    _position.X = (x * 380);
                    _position.Y = (y * 438) - ((x % 2) * 219);
                    cave.Rooms[x,y].image = new RoomImage(_position,"Images/hex","Images/hex2",game);
                    cave.Rooms[x, y].image.setExits(cave.Rooms[x, y].Exits);
                    roomImages.Add(cave.Rooms[x, y].image);
                    bool w,b,p;
                    cave.GetAdjacent(x,y,out w, out b, out p);
                    Console.WriteLine(w);
                    if(w){
                       cave.Rooms[x, y].image.nearWumpus = true;
                    }
                    if (cave.Rooms[x, y].hasBats) {
                        cave.Rooms[x, y].image.bat = true;
                        cave.Rooms[x, y].image.resolved = false;
                    }
                    if (cave.Rooms[x, y].hasPit) {
                        cave.Rooms[x, y].image.pit = true;
                        cave.Rooms[x, y].image.resolved = false;
                    }
                }
            }
            hint = "";
            cave.movePlayer(0, 0);
            cave.Rooms[cave.locationPlayer.X, cave.locationPlayer.Y].image.revealed = true;
            cave.Rooms[cave.locationPlayer.X, cave.locationPlayer.Y].image.currentRoom = true;
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

            hint = (Input.mousePos().X.ToString() + ", " + Input.mousePos().Y.ToString());
            #region Moving State
            if (state == State.MOVING) {
                player.speed.X = 0;
                player.speed.Y = 0;

                if (Input.isKeyPressed(Keys.Q))
                {
                    SetTrivia(trivia.CreateQuestionArray(5),2);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Left) || Input.isKeyDown(Keys.A))
                    player.speed.X -= 3;

                if (Input.isKeyDown(Keys.Right) || Input.isKeyDown(Keys.D))
                    player.speed.X += 3;
                
                if (Input.isKeyDown(Keys.Up) || Input.isKeyDown(Keys.W))
                    player.speed.Y -= 3;

                if (Input.isKeyDown(Keys.Down) || Input.isKeyDown(Keys.S))
                    player.speed.Y += 3;



                if (Input.isKeyDown(Keys.R))
                    EncounterWumpus();

                // Update all the game objects
                // Send these updates to GUI to be drawn
                player.position += player.speed;

                //Glad you asked. It resolves collisions with walls in a 95% reliable way
                //If collisions are with a doorway in the current room, there can be code to switch rooms


                Point lastpoint;
                Point thispoint;

                thispoint = vertices[5];
                for (int i = 0; i < 6; i++) {
                    lastpoint = new Point(thispoint.X, thispoint.Y);
                    thispoint = vertices[i];

                    if (player.checkCollision(lastpoint, thispoint)) {
                        if (cave.Rooms[cave.locationPlayer.X, cave.locationPlayer.Y].image.edgeDraws[i] == true)
                            player.resolveCollision(lastpoint, thispoint);
                        else if (cave.Rooms[cave.locationPlayer.X, cave.locationPlayer.Y].image.edgeDraws[i] != true) {
                            state = State.SWITCHING;
                            moveVector = new Vector2(vertices[i].X - vertices[(i + 2) % 6].X, vertices[i].Y - vertices[(i + 2) % 6].Y) * -1;
                            moveCounter = 0;
                            roomSwitch = i;
                            Console.WriteLine(i);
                            cave.locationPlayer.image.currentRoom = false;
                        }
                    }
                }

            }
            #endregion
            #region Switching State
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
                                        dy = cave.Height - 1;
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
                                    if (dy > cave.Height -1) {
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
                        cave.locationPlayer.image.currentRoom = true;
                        if (cave.locationPlayer.hasPit) {
                            EncounterPit();
                        }
                        if (cave.locationPlayer.hasBats) {
                            EncounterBats();
                        }
                        if (cave.locationPlayer.hasWumpus) {
                            EncounterWumpus();
                        }
                    }

                    player.position += (moveVector / 60);
                    foreach (RoomImage r in roomImages) {
                        r.Position += (moveVector / 30);
                    }
            }

            #endregion
            #region Transport State
            if (state == State.TRANSPORTED) {
                moveCounter++;
                if (moveCounter <= 30) {
                    foreach (RoomImage r in roomImages) {
                        r.Position += (transportDelta / 30);
                    }
                }
                else {
                    cave.locationPlayer.image.revealed = true;
                    state = State.MOVING;
                }
            }
            #endregion
            #region Question State
            if (state == State.QUESTIONING) {
                for (int i = 0; i < 4; i++) {
                    if (buttons[i].pressed) {
                        Console.WriteLine(buttons[i].answerKey);
                        if (buttons[i].answerKey == questions[triviaCount].Answer) {
                            triviaResults++;
                        }
                        if (!continueTrivia()) {
                            triviaSucceeded = (triviaResults >= triviaResultsNeeded);
                            state = State.MOVING;
                            triviaResolve();
                        }
                    }
                }
            }
            #endregion
            player.Update(gameTime);
            foreach (RoomImage r in roomImages) {
                r.Update();
            }
            for (int i = 0; i < 4; i++) {
                buttons[i].Update();
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
            arrowCount = "Arrows: " + player.arrows;
            if (wumpus)
                hint += "\nYou hear heavy breathing and rustling nearby";
            if (bat)
                hint += "\nYou hear wings flapping nearby";
            if (pit)
                hint += "\nYou feel a gust of wind";
            
            spriteBatch.Draw(background, new Vector2(), Color.White);

            foreach (RoomImage i in roomImages) {
                i.Draw(spriteBatch);
            }
            spriteBatch.Draw(HUD, new Vector2(), Color.White);
            if (state == State.QUESTIONING) {
                for (int i = 0; i < 4; i++) {
                    buttons[i].Draw(spriteBatch);
                }
                //spriteBatch.DrawString(consolas, triviaString, new Vector2(29, 619), Color.Gainsboro);
                spriteBatch.DrawString(consolas, triviaString, new Vector2(30, 600), Color.Gold);
                spriteBatch.DrawString(consolas, answer1, new Vector2(45, 625), Color.Gold);
                spriteBatch.DrawString(consolas, answer2, new Vector2(45, 660), Color.Gold);
                spriteBatch.DrawString(consolas, answer3, new Vector2(45, 695), Color.Gold);
                spriteBatch.DrawString(consolas, answer4, new Vector2(45, 730), Color.Gold);
                
            }
            spriteBatch.DrawString(consolas, hint, new Vector2(910,50), Color.Gold);
            spriteBatch.DrawString(consolas, arrowCount, new Vector2(30,580), Color.Gold);
            spriteBatch.DrawString(consolas, "Coins: " + Player.gold, new Vector2(150, 580), Color.Gold);
            player.Draw(spriteBatch);            
            foreach (Sprite x in spriteList)
            {
                x.Draw(spriteBatch);
                Console.WriteLine("Arrows are drawn");
            }
        }



        public void LoadContent(ContentManager content)
        {
            for (int i = 0; i < 4; i++) {
                buttons[i].LoadContent(content);
            }
            selectionImage1 = content.Load<Texture2D>(@"Textures/selection1");
            selectionImage2 = content.Load<Texture2D>(@"Textures/selection2");
            selectionImage3 = content.Load<Texture2D>(@"Textures/selection3");
            selectionImage = selectionImage1;
            consolas = content.Load<SpriteFont>(@"Consolas");
            introImage = content.Load<Texture2D>(@"Textures/Menu");
            highscoreImage = content.Load<Texture2D>(@"Images/Highscores");
            arrow = content.Load<Texture2D>(@"Images/ArrowSprite");
            background = content.Load<Texture2D>(@"Images/SpaceBackground");
            HUD = content.Load<Texture2D>(@"Images/HUD");
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

                Console.WriteLine("Questions Recieved");
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
        public void EncounterPit()
        {
            triviaResolve = ResolvePit;
            SetTrivia(trivia.CreateQuestionArray(3), 2);
        }

        public bool ResolvePit() {
            if (triviaSucceeded) {

            }
            else {
                Game1.currentGameState = Game1.GameState.GameOver;
            }
            return true;
        }
        /// <summary>
        /// Run this if user enters room with bats.
        /// Sets the current room value to a random room in the map range.
        /// </summary>
        public void EncounterBats()
        {
            ResolveBats();
        }

        public bool ResolveBats() {
            Random rnd = new Random();
            int rx = rnd.Next(6);
            int ry = rnd.Next(5);
            while (rx == cave.locationPlayer.X && ry == cave.locationPlayer.Y) {
                rx = rnd.Next(6);
                ry = rnd.Next(5);
            }
            Vector2 delta;
            delta = cave.locationPlayer.image.Position - cave.Rooms[rx, ry].image.Position;
            cave.locationPlayer.image.currentRoom = false;
            cave.movePlayer(rx, ry);
            cave.locationPlayer.image.currentRoom = true;
            state = State.TRANSPORTED;
            transportDelta = delta;
            moveCounter = 0;
            return true;
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
        public void EncounterWumpus()
        {
            triviaResolve = ResolveWumpus;
            SetTrivia(trivia.CreateQuestionArray(6), 5);
        }
        public bool ResolveWumpus() {
            if (triviaSucceeded) {

            }
            else {
                Game1.currentGameState = Game1.GameState.GameOver;
            }
            return true;
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

        public void SetTrivia(Trivia.Question[] q,int score) {
            questions = q;
            triviaCount = 0;
            triviaMax = q.Length;
            state = State.QUESTIONING;
            triviaResults = 0;
            triviaResultsNeeded = score;
            triviaString = questions[0].QuestionText;
            answer1 = questions[0].Choices[0];
            answer2 = questions[0].Choices[1];
            answer3 = questions[0].Choices[2];
            answer4 = questions[0].Choices[3];
        }

        public bool continueTrivia() {
            triviaCount++;
            if (triviaCount == triviaMax) { return false; }
            else {
                triviaString = questions[triviaCount].QuestionText;
                answer1 = questions[triviaCount].Choices[0];
                answer2 = questions[triviaCount].Choices[1];
                answer3 = questions[triviaCount].Choices[2];
                answer4 = questions[triviaCount].Choices[3];
                return true;
            }

        }


        public void UpdateIntro(GameTime gameTime)
        {
            trivia.RandomizeQuestions();
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
                x.Draw(spriteBatch);

            spriteBatch.Draw(introImage, new Vector2(), Color.White);
            spriteBatch.Draw(selectionImage, new Vector2(), Color.White);
        }

        public void DrawGameOver(SpriteBatch spriteBatch)
        {
            foreach(ScoreHandler.Score x in scoreHandler.HighScores)
                spriteBatch.DrawString(consolas, "Name: " + Convert.ToString(x), new Vector2(50, 50), Color.Gold);
            
            spriteBatch.Draw(highscoreImage, new Rectangle(0, 0, 819, 460), Color.White);
        }

        public void ShootArrow(Player player)
        {
            Vector2 speed = new Vector2(3, 3);
            Vector2 position = player.position;

            Point frameSize = new Point(50, 20);

            Sprite sprite = new Sprite(Game1.gameControl.arrow,
                    position, new Point(10, 3), 10, new Point(0, 0),
                    new Point(1, 1), new Vector2(5, 0));
            spriteList.Add(sprite);
        }
    }
}