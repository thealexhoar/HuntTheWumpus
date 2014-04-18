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
            /// <summary>
            /// Exits start at the top left, increase in number clockwise
            /// </summary>
            public bool[] Exits = new bool[6];

            public ushort X { get; private set; }
            public ushort Y { get; private set; }

            public ushort CaveW { get; private set; }
            public ushort CaveH { get; private set; }

            public uint Gold { get; private set; }

            /// <summary>
            /// Room exits start at top left and increase in number clockwise
            /// Cave sould do generation of exits
            /// </summary>
            /// <param name="x">Room x position in cave, increases left to right</param>
            /// <param name="y">Room y position in cave, increases up to down</param>
            public Room(bool exit0, bool exit1, bool exit2, bool exit3, bool exit4, bool exit5, ushort x, ushort y, ushort caveW, ushort caveH)
            {
                this.Exits[0] = exit0;
                this.Exits[1] = exit1;
                this.Exits[2] = exit2;
                this.Exits[3] = exit3;
                this.Exits[4] = exit4;
                this.Exits[5] = exit5;

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
            FileStream fStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            StreamReader strmReader = new StreamReader(fStream);
            
            // first two lines of the file are width and height
            this.Width = ushort.Parse(strmReader.ReadLine());
            this.Height = ushort.Parse(strmReader.ReadLine());

            // after that, each line is a 6-long string of 0s or 1s - exits or walls, respectively, for each room of the cave (going clockwise from top-left)
            // rooms represented going sideways, then down
            for (uint i = 0; i < (uint)(this.Width * this.Height); ++i)
            {
                string line = strmReader.ReadLine();

                for (ushort s = 0; s < 6; ++s)
                    this.Rooms[i%this.Width, i - i%this.Width].Exits[s] = (line[s] == 0) ? (true) : (false);
            }    
            // write your own caves nerd

            this.Filename = filename;
        }

        /// <summary>
        /// debug function
        /// </summary>
        public void _PrintStatus()
        {
        #if DEBUG
            Console.WriteLine("Cave status:\n");
            Console.WriteLine("Width = " + this.Width.ToString());
            Console.WriteLine("Height = " + this.Height.ToString());

            for (uint i = 0; i < (uint)(this.Width * this.Height); ++i)
            {
                Console.WriteLine("Room " + (i % this.Width).ToString() + "*" + (i - i % this.Width).ToString());
                for (ushort s = 0; s < 6; ++s)
                    Console.WriteLine("\tExit " + s.ToString() + ": " + ((this.Rooms[i % this.Width, i - i % this.Width].Exits[s] == true) ? ("hole") : ("wall")));
            }
        #endif
        }
    }
}
