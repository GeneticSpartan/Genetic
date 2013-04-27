using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Genetic.Geometry;

namespace Genetic
{
    public static class GenU
    {
        /// <summary>
        /// A random number object used to generate psuedo-random values.
        /// </summary>
        private static Random _random;

        public static void Initialize()
        {
            SetRandomSeed(GenG.GlobalSeed);
        }

        /// <summary>
        /// Sets the psuedo-random number seed.
        /// Useful for achieving deterministic random number values.
        /// </summary>
        /// <param name="seed">The random number seed.</param>
        public static void SetRandomSeed(int seed)
        {
            _random = new Random(GenG.GlobalSeed);
        }

        /// <summary>
        /// Generates a psuedo-random number between 0.0 and 1.0.
        /// The result will never equal 1.0.
        /// </summary>
        /// <returns>A random number between 0.0 to 1.0.</returns>
        public static float Random()
        {
            return (float)_random.NextDouble();
        }

        /// <summary>
        /// Generates a random number between a minumum and maximum value.
        /// The reslut will never equal the maximum value.
        /// </summary>
        /// <param name="min">The minimum value of the number.</param>
        /// <param name="max">The maximum value of the number.</param>
        /// <returns>A random number between the minumum and maximum values given.</returns>
        public static int Random(int min, int max)
        {
            return (int)(Random() * (max - min) + min);
        }

        /// <summary>
        /// Generates a random color using the mimimum and maximum RGB values given.
        /// </summary>
        /// <param name="minValue">The minimum RGB value.</param>
        /// <param name="maxValue">The maximum RGB value.</param>
        /// <returns>A random color.</returns>
        public static Color RandomColor(int minValue = 0, int maxValue = 255)
        {
            int r = Random(minValue, maxValue + 1);
            int g = Random(minValue, maxValue + 1);
            int b = Random(minValue, maxValue + 1);

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

            // Set the width or height value to 1 if either are 0.
            width = (width != 0) ? width : 1;
            height = (height != 0) ? height : 1;

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
        /// Gets the x and y intersection depths of two bounding boxes.
        /// </summary>
        /// <param name="box1">The first bounding box to compare.</param>
        /// <param name="box2">The second bounding box to compare.</param>
        /// <returns>A Vector2 containing the x and y intersection depths, or a Vector2.Zero if no intersection occurs.</returns>
        public static Vector2 GetIntersectDepthAABB(GenAABB box1, GenAABB box2)
        {
            float distanceX = box1.MidpointX - box2.MidpointX;
            float minDistanceX = box1.HalfWidth + box2.HalfWidth;

            if (Math.Abs(distanceX) < minDistanceX)
            {
                float distanceY = box1.MidpointY - box2.MidpointY;
                float minDistanceY = box1.HalfHeight + box2.HalfHeight;

                if (Math.Abs(distanceY) < minDistanceY)
                {
                    Vector2 intersectDepth = new Vector2();

                    intersectDepth.X = (distanceX > 0) ? minDistanceX - distanceX : -minDistanceX - distanceX;
                    intersectDepth.Y = (distanceY > 0) ? minDistanceY - distanceY : -minDistanceY - distanceY;

                    return intersectDepth;
                }
            }

            return Vector2.Zero;
        }

        /// <summary>
        /// Gets the positive or negative distances between the edges of two bounding boxes on the x-axis and y-axis.
        /// </summary>
        /// <param name="box1">The first bounding box to compare.</param>
        /// <param name="box2">The second bounding box to compare.</param>
        /// <returns>A Vector2 containing the positive or negative distances between the edges of two bounding boxes on the x-axis and y-axis.</returns>
        public static Vector2 GetDistanceAABB(GenAABB box1, GenAABB box2)
        {
            float distanceX = Math.Abs(box1.MidpointX - box2.MidpointX) - (box1.HalfWidth + box2.HalfWidth);
            float distanceY = Math.Abs(box1.MidpointY - box2.MidpointY) - (box1.HalfHeight + box2.HalfHeight);

            return new Vector2(distanceX, distanceY);
        }

        /// <summary>
        /// Retrieves the current position of a sine wave relative to the total elapsed time.
        /// Call this during each update to simulate a sine wave motion.
        /// </summary>
        /// <param name="start">The starting position that the wave will be relative to, usually the center point of the wave.</param>
        /// <param name="rate">The rate at which the wave will fluctuate.</param>
        /// <param name="intensity">The instensity or size of the wave.</param>
        /// <returns>The current position of the wave relative to the total elapsed time.</returns>
        public static float SineWave(float start, float rate, float intensity)
        {
            return start + (float)Math.Sin(GenG.ElapsedTime * rate) * intensity;
        }

        /// <summary>
        /// Retrieves the current position of a cosine wave relative to the total elapsed time.
        /// Call this during each update to simulate a cosine wave motion.
        /// </summary>
        /// <param name="start">The starting position that the wave will be relative to, usually the center point of the wave.</param>
        /// <param name="rate">The rate at which the wave will fluctuate.</param>
        /// <param name="intensity">The instensity or size of the wave.</param>
        /// <returns>The current position of the wave relative to the total elapsed time.</returns>
        public static float CosineWave(float start, float rate, float intensity)
        {
            return start + (float)Math.Cos(GenG.ElapsedTime * rate) * intensity;
        }
    }
}