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
        /// Stupid constructor, makes an empty cave
        /// </summary>
        /// <param name="Width">Height of the cave in number of hexagonal rooms</param>
        /// <param name="Height">Width of the cave in number of hexagonal rooms</param>
        private Cave(ushort Width, ushort Height)
        {
            this.Width = Width;
            this.Height = Height;

            Rooms = new Room[Width, Height];
        }

        /// <summary>
        /// Constructor that loads .cave file
        /// </summary>
        /// <param name="filename">Cave file</param>
        public Cave(string filename)
        {
            FileStream fStream = new FileStream(@"Content\Caves\" + filename, FileMode.Open, FileAccess.Read);
            StreamReader strmReader = new StreamReader(fStream);
            
            // first two lines of the file are cave width and height
            this.Width = ushort.Parse(strmReader.ReadLine());
            this.Height = ushort.Parse(strmReader.ReadLine());

            Rooms = new Room[Width, Height];

            // after that, each line is a 3-number string, representing the enum values of the 3 exits for each room 
            // going left-to-right, then down
            // (e.g. "135" means this room has exits on top left, top right, bottom middle)
            for (uint y = 0; y < this.Height; ++y)
            {
                for (uint x = 0; x < this.Width; ++x)
                {
                    string line = strmReader.ReadLine();

                    // parse the cave-exit string and assign exits
                    for (ushort s = 0; s < 3; ++s)
                        this.Rooms[x, y].Exits[s] = (Room.Exit)ushort.Parse(line.Substring(s, 1));
                }
            }
            // write your own caves nerd

            this.Filename = filename;

            strmReader.Close();
            fStream.Close();
        }

        /// <summary>
        /// debug function
        /// </summary>
<<<<<<< HEAD
        //public void _PrintStatus()
        //{
        //#if DEBUG
        //    Console.WriteLine("Cave status:\n");
        //    Console.WriteLine("Width = " + this.Width.ToString());
        //    Console.WriteLine("Height = " + this.Height.ToString());

        //    for (uint i = 0; i < (uint)(this.Width * this.Height); ++i)
        //    {
        //        Console.WriteLine("Room " + (i % this.Width).ToString() + "*" + (i - i % this.Width).ToString());
        //        for (ushort s = 0; s < 6; ++s)
        //            Console.WriteLine("\tExit " + s.ToString() + ": " + ((this.Rooms[i % this.Width, i - i % this.Width].Exits[s] == true) ? ("hole") : ("wall")));
        //    }
        //#endif
        //}
=======
        public void _PrintStatus()
        {
            /*
        #if DEBUG
            Console.WriteLine("Cave status:\n");
            Console.WriteLine("Width = " + this.Width.ToString());
            Console.WriteLine("Height = " + this.Height.ToString());
            Console.WriteLine("________________________________");

            for (uint y = 0; y < this.Height; ++y)
            {
                for (uint x = 0; x < this.Width; ++x)
                {
                    Console.WriteLine("Room " + x.ToString() + "x" + y.ToString() + ": ");

                    for (ushort s = 0; s < 3; ++s)
                        Console.WriteLine("\tExit 1: " + this.Rooms[x, y].Exits[s].ToString());
                }
            }
        #endif
             */
        }
>>>>>>> 55ca93942dc1bebe1b0e078bb0c5d0b516f5d33d
    }
}
