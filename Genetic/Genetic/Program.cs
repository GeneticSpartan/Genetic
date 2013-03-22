using System;

namespace Genetic
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GenGame game = new GenGame(1280, 720, new MenuState(), 4))
            {
                game.Run();
            }
        }
    }
#endif
}

