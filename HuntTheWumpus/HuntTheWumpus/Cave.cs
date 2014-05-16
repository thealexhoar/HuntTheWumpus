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
                        ;
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
                        ;
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
                         ;
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
                         ;
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

        public List<Room> locationsPits;


        /// <summary>
        /// Constructor that loads .cave file
        /// </summary>
        /// <param name="filename">Cave file</param>
        /// <param name="gen">Generate location features?</param>
        public Cave(string filename, bool map = false)
        {
            using (FileStream fStream = new FileStream(@"Content\Caves\" + filename, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader strmReader = new StreamReader(fStream))
                {
                    // first two lines of the file are cave width and height
                    this.Width = ushort.Parse(strmReader.ReadLine());
                    this.Height = ushort.Parse(strmReader.ReadLine());

                    this.Rooms = new Room[Width, Height];

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

                    this.Filename = filename;
                    locationPlayer = Rooms[0, 0];

                    // working on this, will make a cave manually
                    /*
                    if (map)
                    {
                        // need to place features in rooms
                        // order: player, wumpus, bats, pits
                        bool placedPlayer = false, placedWumpus = false, placedBats = false, placedPits = false;

                        // place player
                        locationPlayer = this.Rooms[rand.Next(0, this.Width), rand.Next(0, this.Height)];
                        locationPlayer.hasPlayer = true;
                        locationPlayer.hasThing = true;
                        placedPlayer = true;

                        // place wumpus
                        while (!placedWumpus)
                        {
                            // generate x and y
                            ushort x = (ushort)rand.Next(0, this.Width), y = (ushort)rand.Next(0, this.Height);

                            // if cave is big enough, space player and wumpus out
                            if (this.Width >= 3 && this.Height >= 3)
                            {
                                // https://github.com/thealexhoar/HuntTheWumpus/blob/master/this.png
                                // if generated x/y are adjacent to player, get a new set
                                if ( (y == locationPlayer.Y - 1 && Math.Abs(x - locationPlayer.X) < 2) || (y == locationPlayer.Y && Math.Abs(x - locationPlayer.X) < 2) || (y == locationPlayer.Y + 1 && x == locationPlayer.X) )
                                    continue;
                            }
                            // otherwise it's gucci, assign
                            else
                            {
                                locationWumpus = this.Rooms[x, y];
                                locationWumpus.hasWumpus = true;
                                locationWumpus.hasThing = true;
                                placedWumpus = true;
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
                        // shouldn't be adjacent to any other obstacles
                        while (!placedPits)
                    }
                    */
                }
            }
        }
        
        // shitty
#if DEBUG
        public string _GetStatusString()
        {
            string str = "";

            str += "Cave status:\n\n";
            str += "Width = " + this.Width.ToString() + "\n";
            str += "Height = " + this.Height.ToString() + "\n";
            str += "________________________________\n\n";

            for (ushort y = 0; y < this.Height; ++y)
            {
                for (ushort x = 0; x < this.Width; ++x)
                {
                    str += "Room " + x.ToString() + "x" + y.ToString() + ": \n";

                    for (ushort s = 0; s < 3; ++s)
                    {
                        str += "\t" + Rooms[x, y].Exits[s].ToString() + "\n";
                    }
                }
            }

            return str;
        }
#endif
    
        public static Cave GetDebugCave()
        {
            Cave c = new Cave("test.cave", false);

            c.locationPlayer = c.Rooms[0, 0]; 
            c.Rooms[0, 0].hasPlayer = true;
            c.Rooms[0, 0].hasThing = true;

            c.locationWumpus = c.Rooms[2, 2];
            c.Rooms[2, 2].hasPlayer = true;
            c.Rooms[2, 2].hasThing = true;

            c.locationBats = c.Rooms[0, 2];
            c.Rooms[0, 2].hasBats = true;
            c.Rooms[0, 2].hasThing = true;

            c.locationsPits.Add(c.Rooms[1, 1]);
            c.Rooms[1, 1].hasPit = true;
            c.Rooms[1, 1].hasThing = true;

            return c;
        }

        public void GetHints(out bool wumpus, out bool bats, out bool pit)
        {
            ushort x = locationPlayer.X, y = locationPlayer.Y;
            Room.Edge e = locationPlayer.GetEdge();

            bool w = false, b = false, p = false;

            // if touching left, check hex to the right
            if ( (e & Room.Edge.LEFT) > 0)
            {
                w = w || Rooms[locationPlayer.X + 1, locationPlayer.Y].hasWumpus;
                b = b || Rooms[locationPlayer.X + 1, locationPlayer.Y].hasBats;
                p = p || Rooms[locationPlayer.X + 1, locationPlayer.Y].hasPit;
            }
            // if touching right, check hex to the left
            if ( (e & Room.Edge.RIGHT) > 0)
            {
                w = w || Rooms[locationPlayer.X - 1, locationPlayer.Y].hasWumpus;
                b = b || Rooms[locationPlayer.X - 1, locationPlayer.Y].hasBats;
                p = p || Rooms[locationPlayer.X - 1, locationPlayer.Y].hasPit;
            }
            // if touching top, check hex to the bottom
            if ( (e & Room.Edge.TOP) > 0)
            {
                w = w || Rooms[locationPlayer.X, locationPlayer.Y + 1].hasWumpus;
                b = b || Rooms[locationPlayer.X, locationPlayer.Y + 1].hasBats;
                p = p || Rooms[locationPlayer.X, locationPlayer.Y + 1].hasPit;
            }
            // if touching bottom, check hex to the top
            if ( (e & Room.Edge.BOTTOM) > 0)
            {
                w = w || Rooms[locationPlayer.X, locationPlayer.Y - 1].hasWumpus;
                b = b || Rooms[locationPlayer.X, locationPlayer.Y - 1].hasBats;
                p = p || Rooms[locationPlayer.X, locationPlayer.Y - 1].hasPit;
            }
            // if touching left and top, check bottom-right
            if ( (e & (Room.Edge.LEFT | Room.Edge.TOP)) > 0)
            {
                w = w || Rooms[locationPlayer.X + 1, locationPlayer.Y + 1].hasWumpus;
                b = b || Rooms[locationPlayer.X + 1, locationPlayer.Y + 1].hasBats;
                p = p || Rooms[locationPlayer.X + 1, locationPlayer.Y + 1].hasPit;
            }
            // if touching right and top, check bottom-left
            if ((e & (Room.Edge.RIGHT | Room.Edge.TOP)) > 0)
            {
                w = w || Rooms[locationPlayer.X - 1, locationPlayer.Y + 1].hasWumpus;
                b = b || Rooms[locationPlayer.X - 1, locationPlayer.Y + 1].hasBats;
                p = p || Rooms[locationPlayer.X - 1, locationPlayer.Y + 1].hasPit;
            }
            // if touching left and bottom, check top-right
            if ((e & (Room.Edge.LEFT | Room.Edge.BOTTOM)) > 0)
            {
                w = w || Rooms[locationPlayer.X + 1, locationPlayer.Y - 1].hasWumpus;
                b = b || Rooms[locationPlayer.X + 1, locationPlayer.Y - 1].hasBats;
                p = p || Rooms[locationPlayer.X + 1, locationPlayer.Y - 1].hasPit;
            }
            // if touching right and bottom, check top-left
            if ((e & (Room.Edge.RIGHT | Room.Edge.BOTTOM)) > 0)
            {
                w = w || Rooms[locationPlayer.X - 1, locationPlayer.Y - 1].hasWumpus;
                b = b || Rooms[locationPlayer.X - 1, locationPlayer.Y - 1].hasBats;
                p = p || Rooms[locationPlayer.X - 1, locationPlayer.Y - 1].hasPit;
            }

            wumpus = w;
            bats = b;
            pit = p;
        }
    }
}
