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

            /// <summary>
            /// A hexagonal (flat ends on top and bottom) room of the cave
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

        // RNG for getting gold value
        private static Random rand = new Random((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);

        public ushort Width { get; private set; } 
        public ushort Height { get; private set; }

        public string Filename { get; private set; }

        Room[,] Rooms;

        /// <summary>
        /// Constructor that loads .cave file
        /// </summary>
        /// <param name="filename">Cave file</param>
        public Cave(string filename)
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
                    // going left-to-right, then down
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
                }
            }
        }
        
        public void _PrintStatus()
        {
        #if DEBUG
            Console.WriteLine("Cave status:\n");
            Console.WriteLine("Width = " + this.Width.ToString());
            Console.WriteLine("Height = " + this.Height.ToString());
            Console.WriteLine("________________________________");

            for (ushort y = 0; y < this.Height; ++y)
            {
                for (ushort x = 0; x < this.Width; ++x)
                {
                    Console.WriteLine("Room " + x.ToString() + "x" + y.ToString() + ": ");

                    for (ushort s = 0; s < 3; ++s)
                        Console.WriteLine("\tExit 1: " + this.Rooms[x, y].Exits[s].ToString());
                }
            }
        #endif
        }
    }
}
