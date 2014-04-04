using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTheWumpus
{
    /// <summary>
    /// The whole cave
    /// </summary>
    class Cave
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

        public ushort Width { get; private set; } 
        public ushort Height { get; private set; }

        Room[,] Rooms;

        /// <summary>
        /// Cave constructor
        /// </summary>
        /// <param name="Width">Height of the cave in number of hexagonal rooms</param>
        /// <param name="Height">Width of the cave in number of hexagonal rooms</param>
        public Cave(ushort Width, ushort Height)
        {
            this.Width = Width;
            this.Height = Height;

            Rooms = new Room[Width, Height];
        }
    }
}
