using System;
using System.IO;

namespace HuntTheWumpus
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                using (Game1 game = new Game1())
                {
                    game.Run();
                }
            }
            catch (Exception e)
            {
                using (var file = new StreamWriter(".crashlog", true))
                {
                    file.WriteLine(DateTime.Now.ToString() + ": " + e.ToString());
                }
            }
        }
    }
}
