using System;

using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;

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

        /// <summary>
        /// Creates a new solid color texture.
        /// </summary>
        /// <param name="color">The color of the texture. Defaults to white if set to null.</param>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <returns>The newly created texture.</returns>
        public static Texture2D MakeTexture(Color? color = null, int width = 1, int height = 1)
        {
            color = color.HasValue ? color.Value : Color.White;

            Texture2D texture = new Texture2D(GenG.GraphicsDevice, width, height);

            Color[] colorData = new Color[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    colorData[x + y * width] = color.Value;
                }
            }

            texture.SetData<Color>(colorData);

            return texture;
        }
    }
}