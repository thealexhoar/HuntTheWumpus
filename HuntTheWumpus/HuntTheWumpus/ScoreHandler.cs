using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

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
        /// The list of downloaded highscores
        /// </summary>
        public List<Score> GlobalScores = new List<Score>();

        /// <summary>
        /// Represents whether the loading thread is done
        /// </summary>
        public bool HasLocalLoaded = false;

        /// <summary>
        /// Represents whether download of highscores has finished
        /// </summary>
        public bool HasGlobalLoaded = false;

        /// <summary>
        /// Represents a single score
        /// </summary>
        public class Score : IComparable<Score>
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
            /// Networking client
            /// </summary>
            public UdpClient udp = new UdpClient(10000);

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

            /// <summary>
            /// Allows comparing to take place between different scores
            /// </summary>
            /// <param name="obj">The other score</param>
            /// <returns>Represents whether equal to, greater than, or less than</returns>
            public int CompareTo(Score obj)
            {
                if (Points > obj.Points)
                    return -1;
                else if (Points < obj.Points)
                    return 1;
                else if (Time < obj.Time)
                    return -1;
                else if (Time > obj.Time)
                    return 1;
                else if (Turns < obj.Turns)
                    return -1;
                else if (Turns > obj.Turns)
                    return 1;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Contructor for the score management system. Call once the game has ended
        /// </summary>
        /// <param name="score">The score of the game</param>
        public ScoreHandler(Score score)
        {
            var localThread = new Thread(new ParameterizedThreadStart(LoadScores));
            localThread.Start(score);

            var globalThread = new Thread(new ParameterizedThreadStart(ManageServer));
            globalThread.Start(score);
        }

        /// <summary>
        /// Handles loading from file, sorting, and rewriting the file
        /// </summary>
        /// <param name="score">The score of the game</param>
        void LoadScores(Object obj)
        {
            var score = (Score)obj;
            HighScores.Add(score);

            if (File.Exists(".scores"))
            {
                using (var read = new StreamReader(".scores"))
                {
                    var text = read.ReadToEnd();
                    Deserialize(text);
                }
            }

            HighScores.Sort();
            HighScores.RemoveRange(10, HighScores.Count - 10);

            using (var file = new StreamWriter(".scores", false))
            {
                foreach (var highScore in HighScores)
                    file.Write(highScore.Serialize());
            }

            HasLocalLoaded = true;
        }

        /// <summary>
        /// Takes the contents of a file and converts it into highscores
        /// </summary>
        /// <param name="serial">Contents of the file</param>
        void Deserialize(string serial)                                 // Find what string to put in
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

        /// <summary>
        /// Stub method for connecting to a server, sending score, and downloading new scores
        /// </summary>
        /// <param name="obj">The score value</param>
        void ManageServer(Object obj)
        {
            var score = (Score)obj;
        }
    }
}
