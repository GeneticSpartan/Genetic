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
            return GetIntersectDepth(rect1.X, rect1.Y, rect1.Width, rect1.Height, rect2.X, rect2.Y, rect2.Width, rect2.Height);
        }

        /// <summary>
        /// Gets the x and y intersection depths of two bounding boxes.
        /// </summary>
        /// <param name="x1">The x position of the top left corner of the first bounding box.</param>
        /// <param name="y1">The y position of the top left corner of the first bounding box.</param>
        /// <param name="width1">The width of the first bounding box.</param>
        /// <param name="height1">The height of the first bounding box.</param>
        /// <param name="x2">The x position of the top left corner of the second bounding box.</param>
        /// <param name="y2">The y position of the top left corner of the second bounding box.</param>
        /// <param name="width2">The width of the second bounding box.</param>
        /// <param name="height2">The height of the second bounding box.</param>
        /// <returns>A Vector2 containing the x and y intersection depths, or a Vector2.Zero if no intersection occurs.</returns>
        public static Vector2 GetIntersectDepth(float x1, float y1, float width1, float height1, float x2, float y2, float width2, float height2)
        {
            float halfWidth1 = width1 / 2.0f;
            float halfWidth2 = width2 / 2.0f;
            float centerX1 = x1 + halfWidth1;
            float centerX2 = x2 + halfWidth2;
            float distanceX = centerX1 - centerX2;
            float minDistanceX = halfWidth1 + halfWidth2;

            if (Math.Abs(distanceX) < minDistanceX)
            {
                float halfHeight1 = height1 / 2.0f;
                float halfHeight2 = height2 / 2.0f;
                float centerY1 = y1 + halfHeight1;
                float centerY2 = y2 + halfHeight2;
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