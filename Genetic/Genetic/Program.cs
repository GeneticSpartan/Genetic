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
            using (GenGame game = new GenGame(1920, 1080, new MenuState(), GenG.DrawType.Pixel, 3f, false))
            {
                game.Run();
            }
        }
    }
#endif
}