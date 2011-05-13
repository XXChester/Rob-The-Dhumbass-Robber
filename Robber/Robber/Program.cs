using System;

namespace Robber {
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TrafficManager game = new TrafficManager())
            {
                game.Run();
            }
        }
    }
#endif
}

