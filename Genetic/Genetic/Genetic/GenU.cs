using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Genetic.Geometry;

namespace Genetic
{
    /// <summary>
    /// Contains various useful static helper methods.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public static class GenU
    {
        /// <summary>
        /// A random number object used to generate pseudo-random values.
        /// </summary>
        private static Random _random;

        /// <summary>
        /// Initializes any necessary resources used by this class.
        /// </summary>
        public static void Initialize()
        {
            SetRandomSeed(GenG.GlobalSeed);
        }

        /// <summary>
        /// Sets the pseudo-random number seed.
        /// Useful for achieving deterministic random number values.
        /// </summary>
        /// <param name="seed">The random number seed.</param>
        public static void SetRandomSeed(int seed)
        {
            _random = new Random(GenG.GlobalSeed);
        }

        /// <summary>
        /// Generates a pseudo-random number between 0.0 and 1.0.
        /// The result will never equal 1.0.
        /// </summary>
        /// <returns>A random number between 0.0 to 1.0.</returns>
        public static float Random()
        {
            return (float)_random.NextDouble();
        }

        /// <summary>
        /// Generates a random number between a minimum and maximum value.
        /// The result will never equal the maximum value.
        /// </summary>
        /// <param name="min">The minimum value of the number.</param>
        /// <param name="max">The maximum value of the number.</param>
        /// <returns>A random number between the minimum and maximum values given.</returns>
        public static int Random(int min, int max)
        {
            return (int)(Random() * (max - min) + min);
        }

        /// <summary>
        /// Generates a random color using the minimum and maximum RGB values given.
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

            // Keep the width and height values to a minimum of 1.
            width = Math.Max(width, 1);
            height = Math.Max(height, 1);

            Texture2D texture = new Texture2D(GenG.GraphicsDevice, width, height);

            Color[] colorData = new Color[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                    colorData[x + y * width] = color.Value;
            }

            texture.SetData<Color>(colorData);

            return texture;
        }

        /// <summary>
        /// Creates a copy of an existing texture.
        /// </summary>
        /// <param name="texture">The original texture to copy.</param>
        /// <returns>The new texture copy.</returns>
        public static Texture2D CopyTexture(Texture2D texture)
        {
            // Create a new texture with the same dimensions as the original texture.
            Texture2D textureCopy = new Texture2D(GenG.GraphicsDevice, texture.Width, texture.Height);

            // Get the pixel color data of the original texture.
            Color[] colorData = new Color[texture.Width * texture.Height];
            textureCopy.GetData<Color>(colorData);

            // Set the pixel color data of the original texture to the new texture.
            textureCopy.SetData<Color>(colorData);

            return textureCopy;
        }

        /// <summary>
        /// Retrieves the current position of a sine wave relative to the total elapsed time.
        /// Call this during each update to simulate a sine wave motion.
        /// </summary>
        /// <param name="start">The starting position that the wave will be relative to, usually the center point of the wave.</param>
        /// <param name="rate">The rate at which the wave will fluctuate.</param>
        /// <param name="intensity">The intensity or size of the wave.</param>
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
        /// <param name="intensity">The intensity or size of the wave.</param>
        /// <returns>The current position of the wave relative to the total elapsed time.</returns>
        public static float CosineWave(float start, float rate, float intensity)
        {
            return start + (float)Math.Cos(GenG.ElapsedTime * rate) * intensity;
        }

        /// <summary>
        /// Creates a unit vector from the specified vector.
        /// If the specified vector is a zero vector, the vector is just returned to avoid NaN values from Vector2.Normalize.
        /// </summary>
        /// <param name="vector">The vector to normalize.</param>
        /// <returns>The normalized vector.</returns>
        public static Vector2 NormalizeVector2(Vector2 vector)
        {
            return (vector == Vector2.Zero) ? vector : Vector2.Normalize(vector);
        }
    }
}