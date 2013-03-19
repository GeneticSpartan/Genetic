using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Genetic.Geometry;

namespace Genetic
{
    public static class GenU
    {
        /// <summary>
        /// Used to generate random numbers.
        /// </summary>
        public static Random Random;

        public static void Initialize()
        {
            Random = new Random();
        }

        /// <summary>
        /// Generates a random color.
        /// </summary>
        public static Color randomColor(int minValue = 0, int maxValue = 255)
        {
            int r = Random.Next(minValue, maxValue);
            int g = Random.Next(minValue, maxValue);
            int b = Random.Next(minValue, maxValue);

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
        /// Gets a bounding box containing the object at its current and predicted positions relative to its velocity.
        /// </summary>
        /// <param name="gameObject">The object to check.</param>
        /// <returns>A bounding box containing the object at its current and predicted positions relative to its velocity.</returns>
        public static GenAABB GetMoveBounds(GenObject gameObject)
        {
            float right = gameObject.X + gameObject.Width;
            float bottom = gameObject.Y + gameObject.Height;

            // Get the x and y distances that the object will move relative to its velocity.
            float distanceX = gameObject.Velocity.X * GenG.PhysicsTimeStep;
            float distanceY = gameObject.Velocity.Y * GenG.PhysicsTimeStep;

            float minLeft = Math.Min(gameObject.X, gameObject.X + distanceX);
            float minTop = Math.Min(gameObject.Y, gameObject.Y + distanceY);
            float maxRight = Math.Max(right, right + distanceX);
            float maxBottom = Math.Max(bottom, bottom + distanceX);

            return new GenAABB(minLeft, minTop, maxRight - minLeft, maxBottom - minTop);
        }
    }
}