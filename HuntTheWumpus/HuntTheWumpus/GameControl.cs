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
        QUESTIONING,
        ADJUSTING,
        SHOOTING
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

        public ScoreHandler scoreHandler;

        bool firingResolved = false;
        bool firing = false;

        List<Sprite> hitList = new List<Sprite>();
        public State state = State.MOVING;
        Vector2 moveVector;
        int moveCounter;
        public ScoreHandler.Score score = new ScoreHandler.Score();

        Texture2D background;
        Texture2D introImage;
        Texture2D highscoreImage;
        Texture2D selectionImage1;
        Texture2D selectionImage2;
        Texture2D selectionImage3;
        Texture2D selectionImage;
        Texture2D HUD;
        Texture2D arrowOverlay;
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

            GUIStubb graphicsInterface = new GUIStubb();
            spriteManager = new SpriteManager(game, player);
            roomImages = new List<RoomImage>();
            displaySprites = new List<Sprite>();
            cave = new Cave("test.cave");
            Vector2 _position = new Vector2();

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
                    if (cave.Rooms[x, y].hasWumpus) {
                        cave.Rooms[x,y].image.wumpus = true;
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

            hint = (cave.locationPlayer.X.ToString() + ", " + cave.locationPlayer.Y.ToString()) ;
            #region Moving State
            if (state == State.MOVING) {
                player.speed.X = 0;
                player.speed.Y = 0;

                
                if (Keyboard.GetState().IsKeyDown(Keys.Left) || Input.isKeyDown(Keys.A))
                    player.speed.X -= 3;

                if (Input.isKeyDown(Keys.Right) || Input.isKeyDown(Keys.D))
                    player.speed.X += 3;
                
                if (Input.isKeyDown(Keys.Up) || Input.isKeyDown(Keys.W))
                    player.speed.Y -= 3;

                if (Input.isKeyDown(Keys.Down) || Input.isKeyDown(Keys.S))
                    player.speed.Y += 3;

                if (Input.isKeyPressed(Keys.F) && player.arrows > 0) {
                    state = State.ADJUSTING;
                    moveCounter = 0;
                    moveVector = new Vector2(240) - player.position;
                }
                if (Input.isKeyPressed(Keys.B)) {
                    BuyArrow();
                }

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
                            score.Turns++;
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
                            score.Points += cave.locationPlayer.Gold * 10;
                        }
                        cave.locationPlayer.image.revealed = true;
                        cave.locationPlayer.image.currentRoom = true;
                        if (cave.locationPlayer.hasPit && cave.locationPlayer.image.resolved == false) {
                            EncounterPit();
                        }
                        if (cave.locationPlayer.hasBats && cave.locationPlayer.image.resolved == false) {
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
                    if (cave.locationPlayer.hasPit && cave.locationPlayer.image.resolved == false) {
                        EncounterPit();
                    }
                    if (cave.locationPlayer.hasBats && cave.locationPlayer.image.resolved == false) {
                        EncounterBats();
                    }
                    if (cave.locationPlayer.hasWumpus) {
                        EncounterWumpus();
                    }
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
                            score.Points += (ulong)(triviaResults * 15);
                            state = State.MOVING;
                            triviaResolve();
                        }
                    }
                }
            }
            #endregion
            #region Adjusting State
            if (state == State.ADJUSTING) {
                moveCounter++;
                player.position += moveVector / 20;
                if (moveCounter == 20) {
                    state = State.SHOOTING;
                }
            }
            #endregion
            #region Shooting State
            if (state == State.SHOOTING) {
                firingResolved = false;
                if (firing == false) {
                    int dx, dy, aimRoom;
                    dx = cave.locationPlayer.X;
                    dy = cave.locationPlayer.Y;
                    aimRoom = -1;
                    if (Input.isKeyPressed(Keys.D1) && 
                        cave.locationPlayer.image.edgeDraws[1] == false) {
                        ShootArrow(new Vector2(-3,-2));
                        aimRoom = 1;
                        player.arrows -= 1;
                        firing = true;
                    }
                    if (Input.isKeyPressed(Keys.D2) &&
                        cave.locationPlayer.image.edgeDraws[2] == false) {
                        ShootArrow(new Vector2(0, -5));
                        aimRoom = 2;
                        player.arrows -= 1;
                        firing = true;
                    }
                    if (Input.isKeyPressed(Keys.D3) &&
                        cave.locationPlayer.image.edgeDraws[3] == false) {
                        ShootArrow(new Vector2(3, -2));
                        aimRoom = 3;
                        player.arrows -= 1;
                        firing = true;
                    }
                    if (Input.isKeyPressed(Keys.D4) &&
                        cave.locationPlayer.image.edgeDraws[4] == false) {
                        ShootArrow(new Vector2(3, 2));
                        aimRoom = 4;
                        player.arrows -= 1;
                        firing = true;
                    }
                    if (Input.isKeyPressed(Keys.D5) &&
                        cave.locationPlayer.image.edgeDraws[5] == false) {
                        ShootArrow(new Vector2(0, 5));
                        aimRoom = 5;
                        player.arrows -= 1;
                        firing = true;
                    }
                    if (Input.isKeyPressed(Keys.D6) &&
                        cave.locationPlayer.image.edgeDraws[0] == false) {
                        ShootArrow(new Vector2(-3, 2));
                        aimRoom = 6;
                        player.arrows -= 1;
                        firing = true;
                    }
                    switch (aimRoom) {
                        case -1:
                            break;
                        case 0:
                            if (cave.locationPlayer.X % 2 == 0) {
                                dx = cave.locationPlayer.X - 1;
                                if (dx < 0) {
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
                                dy = cave.Height - 1;
                            }
                            break;
                        case 3:

                            if (cave.locationPlayer.X % 2 == 1) {
                                dx = cave.locationPlayer.X + 1;
                                if (dx > cave.Width - 1) {
                                    dx = 0;
                                }
                                dy = cave.locationPlayer.Y - 1;
                                if (dy < 0) {
                                    dy = cave.Height - 1;
                                }
                            }
                            else {
                                dx = cave.locationPlayer.X + 1;
                                if (dx > cave.Width - 1) {
                                    dx = 0;
                                }
                                dy = cave.locationPlayer.Y;
                            }
                            break;
                        case 4:
                            if (cave.locationPlayer.X % 2 == 0) {
                                dx = cave.locationPlayer.X + 1;
                                if (dx > cave.Width - 1) {
                                    dx = 0;
                                }
                                dy = cave.locationPlayer.Y + 1;
                                if (dy > cave.Height - 1) {
                                    dy = 0;
                                }
                            }
                            else {
                                dx = cave.locationPlayer.X + 1;
                                if (dx > cave.Width - 1) {
                                    dx = 0;
                                }
                                dy = cave.locationPlayer.Y;
                            }
                            break;
                        case 5:
                            dx = cave.locationPlayer.X;
                            dy = cave.locationPlayer.Y + 1;
                            if (dy > cave.Height - 1) {
                                dy = 0;
                            }
                            break;
                    }
                    if (aimRoom != -1) {
                        if (cave.Rooms[dx, dy].hasWumpus) {
                            //WIN GAME
                            cave.Rooms[dx, dy].killWumpus();

                        }
                    }
                }
                else {
                    if (spriteList.Count > 0) {
                        foreach (Sprite s in spriteList) {
                            s.Update();
                            if ((s.position.X > 740 || s.position.X < 0) || (s.position.Y > 640 || s.position.Y < 0)) {
                                s.kill = true;
                                hitList.Add(s);
                            }
                            
                        }
                        if (hitList.Count > 0) {
                            foreach (Sprite ss in hitList) {
                                spriteList.Remove(ss);
                                firing = false;
                                state = State.MOVING;
                            }
                            hitList = new List<Sprite>();
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
            if (state == State.SHOOTING) {
                foreach (RoomImage i in roomImages) {
                    i.Draw(spriteBatch,true);
                }
            }
            else {
                foreach (RoomImage i in roomImages) {
                    i.Draw(spriteBatch);
                }
            }
            foreach (Sprite x in spriteList) {
                x.Draw(spriteBatch);
                Console.WriteLine("Arrows are drawn");
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
            spriteBatch.DrawString(consolas, "Coins: " + player.gold, new Vector2(150, 580), Color.Gold);
            spriteBatch.DrawString(consolas, "Points: " + score.Points, new Vector2(270, 580), Color.Gold);
            spriteBatch.DrawString(consolas, "Time: " + Game1.clock.Elapsed.Seconds, new Vector2(390, 580), Color.Gold);
            player.Draw(spriteBatch);            
           
            
            
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
            arrowOverlay = content.Load<Texture2D>(@"Images/ArrowOverlay");

            player.LoadContent(content);
            foreach (RoomImage i in roomImages) 
            {
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
        public void GetTrivia(int questions)
        {
            string[] questionsString = new string[questions];
            if (player.gold > questions)
            {
                // Ask Trivia for 3 questions

                Console.WriteLine("Questions Recieved");
                player.gold -= questions;
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
                cave.locationPlayer.image.resolved = true;
            }
            else {
                scoreHandler = new ScoreHandler();
                Game1.currentGameState = Game1.GameState.GameOver;
            }
            return true;
        }
        /// <summary>
        /// Run this if user enters room with bats.
        /// Sets the current room value to a random room in the map range.
        /// </summary>
        public void EncounterBats() {
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
            if (triviaSucceeded)
            {
                score.Time = (ulong)Game1.clock.Elapsed.Seconds + (ulong)Game1.clock.Elapsed.Minutes * 60;
                Game1.clock.Reset();
                scoreHandler = new ScoreHandler(score);
                Game1.currentGameState = Game1.GameState.GameOver;
            }
            else
            {
                scoreHandler = new ScoreHandler();
                Game1.currentGameState = Game1.GameState.GameOver;
            }
            return true;
        }

        /// <summary>
        /// If user presses the buyArrow key (to be determined) call this method
        /// Gets 3 trivia questions
        /// If answered correctly, arrows are added to the arrowCount
        /// </summary>
        public void BuyArrow() {
            if (player.gold >= 20) {
                player.gold -= 20;
                triviaResolve = ResolveBuy;
                SetTrivia(trivia.CreateQuestionArray(3), 2);
            }
        }
        public bool ResolveBuy() {
            player.arrows+=1;
            return true;
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

            spriteBatch.Draw(highscoreImage, new Rectangle(0, 0, 1024, 768), Color.White);

            spriteBatch.DrawString(consolas, "Name", new Vector2(125, 125), Color.Cyan);
            spriteBatch.DrawString(consolas, "Points", new Vector2(350, 125), Color.Cyan);
            spriteBatch.DrawString(consolas, "Time", new Vector2(550, 125), Color.Cyan);
            spriteBatch.DrawString(consolas, "Turns", new Vector2(750, 125), Color.Cyan);

            for (int i = 0; i < scoreHandler.HighScores.Count; i++)
            {
                var tmpScore = scoreHandler.HighScores[i];
                Color tmpColor = Color.Cyan;
                if (tmpScore == score)
                    tmpColor = Color.Gold;
                spriteBatch.DrawString(consolas, tmpScore.Name, new Vector2(125, 175 + i * 50), tmpColor);
                spriteBatch.DrawString(consolas, tmpScore.Points.ToString(), new Vector2(350, 175 + i * 50), tmpColor);
                spriteBatch.DrawString(consolas, tmpScore.Time.ToString(), new Vector2(550, 175 + i * 50), tmpColor);
                spriteBatch.DrawString(consolas, tmpScore.Turns.ToString(), new Vector2(750, 175 + i * 50), tmpColor);
            }
        }

        public void ShootArrow(Vector2 vector)
        {
            Vector2 speed = vector;
            Vector2 position = new Vector2(256);

            Point frameSize = new Point(5, 5);

            Sprite sprite = new Sprite(arrow,position,frameSize,0,new Point(),new Point(),vector);
            spriteList.Add(sprite);
        }
    }
}