using System;
using System.Collections.Generic;
using System.Xml;

namespace HuntTheWumpus
{
    /// <summary>
    /// Manages the storage and sorting of scores.
    /// </summary>
	class ScoreHandler
	{
        /// <summary>
        /// Represents the list of highscores from the file
        /// </summary>
        public List<Score> HighScores = new List<Score>();

        /// <summary>
        /// Represents a single score
        /// </summary>
        public class Score : XmlNode, IComparable
        {
            /// <summary>
            /// Name of the user
            /// </summary>
            public string name = "";

            /// <summary>
            /// Points recieved from questions/money
            /// </summary>
            public ulong Points = 0;

            /// <summary>
            /// Number of turns the user has taken
            /// </summary>
            public ulong Turns = 0;

            /// <summary>
            /// Seconds taken during the game
            /// </summary>
            public ulong Time = 0;
        }

		public ScoreHandler(Score score)
		{
		}
	}
}
