using System;
using System.Collections.Generic;
using System.IO;

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
        public class Score
        {
            /// <summary>
            /// Name of the user
            /// </summary>
            public string Name = "";

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

            /// <summary>
            /// Basic constructor
            /// </summary>
            public Score()
            { }

            /// <summary>
            /// Basic constructor for giving all objects
            /// </summary>
            /// <param name="name">Name of the user</param>
            /// <param name="points">Points recieved from questions/money</param>
            /// <param name="turns">Number of turns the user has taken</param>
            /// <param name="time">Seconds taken during the game</param>
            public Score(string name, ulong points, ulong turns, ulong time)
            {
                Name = name;
                Points = points;
                Turns = turns;
                Time = time;
            }

            /// <summary>
            /// Serialization into a unit that will go into the file
            /// </summary>
            /// <returns>The object as a string</returns>
            public string Serialize()
            {
                return Name + ';' + Convert.ToString(Points) + ':' + Convert.ToString(Turns) + '|' + Convert.ToString(Time) + '\n';
            }
        }

        public ScoreHandler(Score score)
        {
            HighScores.Add(score);
        }

        /// <summary>
        /// Takes the contents of a file and converts it into highscores
        /// </summary>
        /// <param name="serial">Contents of the file</param>
        void Deserialize(string serial)
        {
            string temp = "";
            Score score = new Score();

            for (int i = 0; i < serial.Length; i++)
            {
                if (serial[i] == ';')
                {
                    score.Name = temp;
                    temp = "";
                }
                else if (serial[i] == ':')
                {
                    ulong.TryParse(temp, out score.Points);
                    temp = "";
                }
                else if (serial[i] == '|')
                {
                    ulong.TryParse(temp, out score.Turns);
                    temp = "";
                }
                else if (serial[i] == '\n')
                {
                    ulong.TryParse(temp, out score.Time);
                    temp = "";
                    HighScores.Add(score);
                    score = new Score();
                }
                else
                {
                    temp += serial[i];
                }
            }
        }
    }
}
