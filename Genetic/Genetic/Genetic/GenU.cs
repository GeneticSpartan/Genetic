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

        /// <summary>
        /// Gets the x and y intersection depths of two rectangles.
        /// </summary>
        /// <param name="rect1">The first rectangle to check.</param>
        /// <param name="rect2">The second rectangle to check.</param>
        /// <returns>A Vector2 containing the x and y intersection depths, or a Vector2.Zero if no intersection occurs.</returns>
        public static Vector2 GetIntersectDepth(Rectangle rect1, Rectangle rect2)
        {
            float halfWidth1 = rect1.Width / 2.0f;
            float halfWidth2 = rect2.Width / 2.0f;
            float centerX1 = rect1.Left + halfWidth1;
            float centerX2 = rect2.Left + halfWidth2;
            float distanceX = centerX1 - centerX2;
            float minDistanceX = halfWidth1 + halfWidth2;

            if (Math.Abs(distanceX) < minDistanceX)
            {
                float halfHeight1 = rect1.Height / 2.0f;
                float halfHeight2 = rect2.Height / 2.0f;
                float centerY1 = rect1.Top + halfHeight1;
                float centerY2 = rect2.Top + halfHeight2;
                float distanceY = centerY1 - centerY2;
                float minDistanceY = halfHeight1 + halfHeight2;

                if (Math.Abs(distanceY) < minDistanceY)
                {
                    Vector2 intersectDepth = new Vector2();

                    intersectDepth.X = (distanceX > 0) ? minDistanceX - distanceX : -minDistanceX - distanceX;
                    intersectDepth.Y = (distanceY > 0) ? minDistanceY - distanceY : -minDistanceY - distanceY;

                    return intersectDepth;
                }
                else
                    return Vector2.Zero;
            }
            else
                return Vector2.Zero;
        }
    }
}