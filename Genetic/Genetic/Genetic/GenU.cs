using System;

using Microsoft.Xna.Framework;

namespace Genetic
{
    public static class GenU
    {
        /// <summary>
        /// Used to generate random numbers.
        /// </summary>
        public static Random random;

        public static void Initialize()
        {
            random = new Random();
        }

        /// <summary>
        /// Generates a random color.
        /// </summary>
        public static Color randomColor(int minValue = 0, int maxValue = 255)
        {
            int r = random.Next(minValue, maxValue);
            int g = random.Next(minValue, maxValue);
            int b = random.Next(minValue, maxValue);

            return new Color(r, g, b);
        }
    }
}