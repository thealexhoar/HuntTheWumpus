using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusGame
{
    public class GameControl
    {
        public GameControl()
        {
            isInitialized = true;
        }

        public bool isInitialized { get; set; }

        public void GameStart()
        {

        }

        public void Update()
        {

        }

        public void Move()
        {

        }

        /// <summary>
        /// Send info to and from map
        /// </summary>
        public void Bats()
        {

        }

        /// <summary>
        /// Send from Map to Trivia
        /// Recieve Trivia questions
        /// </summary>
        public void BottomlessPit()
        {

        }

        /// <summary>
        /// Send from Map to Trivia
        /// </summary>
        public void Wumpus()
        {

        }

        /// <summary>
        /// Initialize the highscore, remove the other objects from lists
        /// </summary>
        public void GameOver()
        {

        }

        /// <summary>
        /// Call Trivia and get Player gold amount
        /// </summary>
        public void GetArrow()
        {

        }

        /// <summary>
        /// Send to room, get rid of one arrow in player class
        /// </summary>
        public void ShootArrow()
        {
        }
    }
}
