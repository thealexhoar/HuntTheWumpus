using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text;

namespace ScoreServer
{
    /// <summary>
    /// Contains the server for storing high scores
    /// </summary>
    class Program
    {
        /// <summary>
        /// List of highscores in memory
        /// </summary>
        static List<Score> HighScores = new List<Score>();

        /// <summary>
        /// Listener for UDP requests
        /// </summary>
        static UdpClient udp = new UdpClient(10000);

        /// <summary>
        /// IP Address for current clients
        /// </summary>
        static IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 10000);

        /// <summary>
        /// Container for unproccessed data
        /// </summary>
        static byte[] byteArray;

        /// <summary>
        /// A string representing complete data
        /// </summary>
        static string recievedData;

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        static void Main()
        {
            if (File.Exists(".scores"))
            {
                using (var file = new StreamReader(".scores"))
                {
                    var text = file.ReadToEnd();
                    Score.Deserialize(text, out HighScores);
                }
            }

            try
            {
                while (true)
                {
                    byteArray = udp.Receive(ref groupEP);
                    recievedData = Encoding.ASCII.GetString(byteArray, 0, byteArray.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            using (var file = new StreamWriter(".scores"))
            {
                foreach (var score in HighScores)
                    file.Write(score.Serialize());
            }

            udp.Close();
        }
    }
}
