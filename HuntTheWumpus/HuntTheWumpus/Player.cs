using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuntTheWumpus
{
    public class Player
    {
        public Player() 
        {
            this.isInitialized = true; 
        }

        public bool isInitialized
        {
            get;

            set;
        }
    }
}
