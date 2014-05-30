using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HuntTheWumpus
{
    /// <summary>
    /// The whole cave
    /// </summary>
    public class Cave
    {
        /// <summary>
        /// One hexagonal room of the cave
        /// </summary>
        public class Room
        {
            // enum representing an exit (door) in the room
            public enum Exit
            {
                NONE,   // just in case, each room should have 3 exits though
                TL,
                TM,
                TR,
                BR,
                BM,
                BL
            }

            /// <summary>
            /// 3 exits per room, can leave some as Exit.NONE if necessary
            /// </summary>
            public Exit[] Exits = new Exit[3];

            public ushort X { get; private set; }
            public ushort Y { get; private set; }

            public ushort CaveW { get; private set; }
            public ushort CaveH { get; private set; }

            public uint Gold { get; private set; }

            // is this needed?
            public RoomImage image;


            private bool wumpus;
            public bool hasWumpus
            {
                get
                {
                    return wumpus;
                }
                set
                {
                    if (this.hasPlayer || this.hasBats || this.hasPit)
                        // throw exception
                        { }
                    else wumpus = value;
                }
            }

            private bool player;
            public bool hasPlayer
            {
                get
                {
                    return player;
                }
                set
                {
                    if (this.hasWumpus || this.hasBats || this.hasPit)
                        // throw exception
                        { }
                    else player = value;
                }
            }

            private bool pit;
            public bool hasPit
            {
                get
                {
                    return pit;
                }
                set
                {
                    if (this.hasWumpus || this.hasPlayer || this.hasBats)
                        // throw exception
                        { }
                    else pit = value;
                }
            }

            private bool bats;
            public bool hasBats
            {
                get
                {
                    return bats;
                }
                set
                {
                    if (this.hasWumpus || this.hasBats || this.hasPit)
                        // throw exception
                        { }
                    else bats = value;
                }
            }

            public bool hasThing;

            /// <summary>
            /// A hexagonal (flat ends on top and bottom) room of the 
            /// cave
            /// </summary>
            public Room(Exit exit0, Exit exit1, Exit exit2, ushort x, ushort y, ushort caveW, ushort caveH)
            {
                this.Exits[0] = exit0;
                this.Exits[1] = exit1;
                this.Exits[2] = exit2;

                this.CaveW = caveW;
                this.CaveH = caveH;

                // tweak min, max values;
                this.Gold = (uint)rand.Next(0, 10);

                this.hasThing = false;
                this.hasPlayer = false;
                this.hasWumpus = false;
                this.hasBats = false;
                this.hasPit = false;
                this.X = x;
                this.Y = y;
            }

            public void killWumpus() {
                this.hasWumpus = false;
                this.image.wumpus = false;
            }

            /// <summary>
            /// Returned by GetEdge function, represents which (if any) edges of the cave this room resides on 
            /// </summary>
            public enum Edge
            {
                NONE = 0,
                TOP = 0x1 << 1,
                LEFT = 0x1 << 2,
                BOTTOM = 0x1 << 3,
                RIGHT = 0x1 << 4
            }

            public Edge GetEdge()
            {
                Edge e = Edge.NONE;

                // is on top?
                if (this.Y == 0)
                    e |= Edge.TOP;
                // is on bottom?
                else if (this.Y == CaveH - 1)
                    e |= Edge.BOTTOM;
                // is on left?
                if (this.X == 0)
                    e |= Edge.LEFT;
                // is on right?
                if (this.X == CaveW - 1)
                    e |= Edge.RIGHT;

                return e;
            }
        }

        // RNG for getting gold values
        private static Random rand = new Random((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);

        public ushort Width { get; private set; }
        public ushort Height { get; private set; }

        public string Filename { get; private set; }

        public Room[,] Rooms;

        public Room locationWumpus { get; private set; }
        public void moveWumpus(ushort x, ushort y)
        {
            // remove wumpus from current location
            locationWumpus.hasWumpus = false;

            // repoint reference and place wumpus
            locationWumpus = this.Rooms[x, y];
            Rooms[x, y].hasWumpus = true;
        }

        public Room locationPlayer { get; private set; }
        public void movePlayer(int x, int y)
        {
            // remove player from current location
            locationPlayer.hasPlayer = false;

            // repoint reference and place player
            locationPlayer = Rooms[x, y];
            Rooms[x, y].hasPlayer = true;
        }

        public Room locationBats { get; private set; }
        public void moveBats(ushort x, ushort y)
        {
            // remove bats from current location
            //locationBats.hasBats = false;

            // repoint reference and place bats
            locationBats = this.Rooms[x, y];
            //Rooms[x, y].hasBats = true;
        }

        public List<Room> locationsPits = new List<Room>();


        /// <summary>
        /// Constructor that loads .cave file
        /// </summary>
        /// <param name="filename">Cave file</param>
        /// <param name="gen">Generate location features?</param>
        public Cave(string filename, bool map = true)
        {
            using (FileStream fStream = new FileStream(@"Content\Caves\" + filename, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader strmReader = new StreamReader(fStream))
                {
                    // first two lines of the file are cave width and height
                    this.Width = ushort.Parse(strmReader.ReadLine());
                    this.Height = ushort.Parse(strmReader.ReadLine());

                    this.Rooms = new Room[Width, Height];

                    this.Filename = filename;

                    // after that, each line is a 3-number string, representing the enum values of the 3 exits for each room 
                    // counter-clockwise, starting top-left
                    // (e.g. "135" means this room has exits on top left, top right, bottom middle)
                    for (ushort y = 0; y < this.Height; ++y)
                    {
                        for (ushort x = 0; x < this.Width; ++x)
                        {
                            string line = strmReader.ReadLine();

                            // initialize rooms
                            Room.Exit exit0 = (Room.Exit)ushort.Parse(line.Substring(0, 1));
                            Room.Exit exit1 = (Room.Exit)ushort.Parse(line.Substring(1, 1));
                            Room.Exit exit2 = (Room.Exit)ushort.Parse(line.Substring(2, 1));

                            this.Rooms[x, y] = new Room(exit0, exit1, exit2, x, y, this.Width, this.Height);
                        }
                    }
                    // write your own caves nerd

                    //locationPlayer = Rooms[0, 0];

                    if (map)
                    {
                        // need to place features in rooms
                        // order: player, wumpus, bats, pits
                        bool placedPlayer = false, placedWumpus = false, placedBats = false, placedPits = false;

                        // place player at 0, 0
                        locationPlayer = this.Rooms[0, 0];
                        locationPlayer.hasPlayer = true;
                        locationPlayer.hasThing = true;
                        placedPlayer = true;

                        // place wumpus
                        while (!placedWumpus)
                        {
                            // generate x and y
                            ushort x = (ushort)rand.Next(0, this.Width), y = (ushort)rand.Next(0, this.Height);

                            if (this.Rooms[x, y].hasThing)
                                continue;

                            // if cave is big enough, space player and wumpus out
                            if (this.Width >= 3 && this.Height >= 3)
                            {
                                int TLx = (x == 0) ? (this.Width - 1) : (x - 1), TLy = (y == 0) ? (this.Height - 1) : (y - 1);
                                int TMx = x, TMy = (y == 0) ? (this.Height - 1) : (y - 1);
                                int TRx = (x == this.Width - 1) ? (0) : (x + 1), TRy = (y == 0) ? (this.Height - 1) : (y - 1);
                                int MLx = (x == 0) ? (this.Width - 1) : (x - 1), MLy = y;
                                int MRx = (x == this.Width - 1) ? (0) : (x + 1), MRy = y;
                                int BMx = x, BMy = (y == this.Height - 1) ? (0) : (y + 1);

                                // https://github.com/thealexhoar/HuntTheWumpus/blob/master/this.png
                                // if generated x/y are adjacent to player, get a new set
                                if (this.Rooms[TLx, TLy].hasPlayer || this.Rooms[TMx, TMy].hasPlayer || this.Rooms[TRx, TRy].hasPlayer || this.Rooms[MLx, MLy].hasPlayer || this.Rooms[MRx, MRy].hasPlayer || this.Rooms[BMx, BMy].hasPlayer)
                                    continue;
                                // otherwise assign
                                else
                                {
                                    locationWumpus = this.Rooms[x, y];
                                    locationWumpus.hasWumpus = true;
                                    locationWumpus.hasThing = true;
                                    placedWumpus = true;
                                    Console.Write(x);
                                    Console.Write(" ");
                                    Console.WriteLine(y);
                                }
                            }
                            // otherwise it's gucci, assign
                            else
                            {
                                locationWumpus = this.Rooms[x, y];
                                locationWumpus.hasWumpus = true;
                                locationWumpus.hasThing = true;
                                placedWumpus = true;
                                Console.Write(x);
                                Console.Write(" ");
                                Console.WriteLine(y);
                            }
                        }

                        // place bats
                        // can go anywhere
                        while (!placedBats)
                        {
                            ushort x = (ushort)rand.Next(0, this.Width), y = (ushort)rand.Next(0, this.Height);

                            if (this.Rooms[x, y].hasThing)
                                continue;
                            else
                            {
                                locationBats = this.Rooms[x, y];
                                locationBats.hasBats = true;
                                locationBats.hasThing = true;
                                placedBats = true;
                            }
                        }

                        // place pits
                        // certain amount per cave
                        // can end up adjacent to other obstacles
                        // place per 2 rows of cave
                        for (ushort i = 0; i < this.Height / 2; ++i)
                        {
                            // lowest section if cave has uneven height
                            // *****
                            // ***** <
                            // *****
                            // ***** <
                            // ***** <- this
                            if ((i == (ushort)(this.Height / 2) - 1) && (this.Height % 2 != 0))
                            {
                                bool based = false;

                                while (!based)
                                {
                                    ushort x = (ushort)rand.Next(0, this.Width), y = (ushort)(this.Height - 1);

                                    if (this.Rooms[x, y].hasThing)
                                        continue;
                                    else
                                    {
                                        locationsPits.Add(this.Rooms[x, y]);
                                        this.Rooms[x, y].hasPit = true;
                                        this.Rooms[x, y].hasThing = true;

                                        based = true;
                                    }
                                }
                            }
                            // ***** }
                            // ***** } - one pit per
                            // ***** ]
                            // ***** ] - one pit per
                            // ...
                            else
                            {
                                bool based = false;

                                while (!based)
                                {
                                    ushort x = (ushort)rand.Next(0, this.Width), y = (ushort)rand.Next(i * 2, i * 2 + 2);

                                    if (this.Rooms[x, y].hasThing)
                                        continue;
                                    else
                                    {
                                        locationsPits.Add(this.Rooms[x, y]);
                                        this.Rooms[x, y].hasPit = true;
                                        this.Rooms[x, y].hasThing = true;

                                        based = true;
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }


#if DEBUG
        public string _GetStatusString()
        {
            string str = "";

            str += "Cave status:\n\n";
            str += "(coordinates x by y)\n";
            str += "Player location = " + locationPlayer.X + "x" + locationPlayer.Y + "\n";
            str += "Wumpus location = " + locationWumpus.X + "x" + locationWumpus.Y + "\n";
            str += "Bats location = " + locationBats.X + "x" + locationBats.Y + "\n";
            str += "Pits locations = \n";
            foreach (var pl in locationsPits)
                str += "\t" + pl.X + "x" + pl.Y + "\n";

            str += "\nWidth = " + this.Width.ToString() + "\n";
            str += "Height = " + this.Height.ToString() + "\n";
            str += "________________________________\n\n";

            for (ushort x = 0; x < this.Width; ++x)
            {
                for (ushort y = 0; y < this.Height; ++y)
                {
                    str += "Room " + x.ToString() + "x" + y.ToString() + ": \n";

                    for (ushort s = 0; s < 3; ++s)
                    {
                        str += "\t" + Rooms[x, y].Exits[s].ToString() + "\n";
                    }
                }
            }

            str += "\n";
            //

            return str;
        }
#endif
        /*
        public static Cave GetCave()
        {
            Cave c = new Cave("test.cave", false);

            c.locationPlayer = c.Rooms[0, 0]; 
            c.Rooms[0, 0].hasPlayer = true;
            c.Rooms[0, 0].hasThing = true;

            c.locationWumpus = c.Rooms[3, 2];
            c.Rooms[3, 2].hasPlayer = true;
            c.Rooms[3, 2].hasThing = true;

            c.locationBats = c.Rooms[4, 2];
            c.Rooms[4, 2].hasBats = true;
            c.Rooms[4, 2].hasThing = true;

            c.locationsPits.Add(c.Rooms[5, 2]);
            c.Rooms[5, 2].hasPit = true;
            c.Rooms[5, 2].hasThing = true;

            c.locationsPits.Add(c.Rooms[1, 3]);
            c.Rooms[1, 3].hasPit = true;
            c.Rooms[1, 3].hasThing = true;

            return c;
        }
        */
        public void GetAdjacent(int x, int y, out bool wumpus, out bool bats, out bool pit) // This function is broken
        {
            /*
            FFUUCCKK the ternary operator
            int TLx = (x==0) ? (this.Width-1) : (x-1), TLy = (y==0) ? (this.Height-1) : (y-1);
            int TMx = x, TMy = (y==0) ? (this.Height-1) : (y-1);
            int TRx = (x==this.Width-1) ? (0) : (x+1), TRy = (y==0) ? (this.Height-1) : (y-1);
            int MLx = (x==0) ? (this.Width-1) : (x-1), MLy = y;
            int MRx = (x==this.Width-1) ? (0) : (x+1), MRy = y;
            int BMx = x, BMy = (y==this.Height-1) ? (0) : (y+1);
            */

            bool w = false, b = false, p = false;
            Room.Edge e = Rooms[x, y].GetEdge();
            Room tmp;

            int xPlusOne, xMinusOne, yPlusOne, yMinusOne;
            if (x == this.Width-1) 
            { 
                xPlusOne = 0; 
            }
            else 
            { 
                xPlusOne = x + 1; 
            }

            if (x == 0) 
            { 
                xMinusOne = 5; 
            }
            else 
            { 
                xMinusOne = x - 1; 
            }

            if (y == this.Height) 
            { 
                yPlusOne = 0; 
            }
            else 
            { 
                yPlusOne = y + 1; 
            }

            if (y == 0) 
            { 
                yMinusOne = 4; 
            }
            else 
            { 
                yMinusOne = y - 1; 
            }

            tmp = Rooms[xMinusOne, y];
            w = w || tmp.hasWumpus;
            b = b || tmp.hasBats;
            p = p || tmp.hasPlayer;

            tmp = Rooms[xPlusOne, y];
            w = w || tmp.hasWumpus;
            b = b || tmp.hasBats;
            p = p || tmp.hasPlayer;

            tmp = Rooms[x, yMinusOne];
            w = w || tmp.hasWumpus;
            b = b || tmp.hasBats;
            p = p || tmp.hasPlayer;

            tmp = Rooms[x, yPlusOne];
            w = w || tmp.hasWumpus;
            b = b || tmp.hasBats;
            p = p || tmp.hasPlayer;

            if (x % 2 == 1) 
            {
                tmp = Rooms[xMinusOne, yMinusOne];
                w = w || tmp.hasWumpus;
                b = b || tmp.hasBats;
                p = p || tmp.hasPlayer;

                
                tmp = Rooms[xPlusOne, yMinusOne];
                w = w || tmp.hasWumpus;
                b = b || tmp.hasBats;
                p = p || tmp.hasPlayer;
            }
            else 
            {
                tmp = Rooms[xMinusOne, yPlusOne];
                w = w || tmp.hasWumpus;
                b = b || tmp.hasBats;
                p = p || tmp.hasPlayer;

                tmp = Rooms[xPlusOne, yPlusOne];
                w = w || tmp.hasWumpus;
                b = b || tmp.hasBats;
                p = p || tmp.hasPlayer;
            }

            wumpus = w;
            bats = b;
            pit = p;
        }
    }
}