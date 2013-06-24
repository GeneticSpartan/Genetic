using System;

using Microsoft.Xna.Framework;

namespace Genetic.Geometry
{
    /// <summary>
    /// Represents an axis-aligned bounding box.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenAABB : GenAABBBasic
    {
        /// <summary>
        /// Half of the width value of the bounding box.
        /// </summary>
        protected float _halfWidth;

        /// <summary>
        /// Half of the height value of the bounding box.
        /// </summary>
        protected float _halfHeight;

        /// <summary>
        /// The x position of the center of the bounding box.
        /// </summary>
        protected float _midpointX;

        /// <summary>
        /// The y position of the center of the bounding box.
        /// </summary>
        protected float _midpointY;

        /// <summary>
        /// Gets or sets the x position of the top left corner of the bounding box.
        /// </summary>
        new public float X
        {
            get { return _min.X; }

            set
            {
                base.X = value;
                _midpointX = _min.X + _halfWidth;
            }
        }

        /// <summary>
        /// Gets or sets the y position of the top left corner of the bounding box.
        /// </summary>
        new public float Y
        {
            get { return _min.Y; }

            set
            {
                base.Y = value;
                _midpointY = _min.Y + _halfHeight;
            }
        }

        /// <summary>
        /// Gets or sets the width of the bounding box.
        /// </summary>
        new public float Width
        {
            get { return _width; }

            set
            {
                base.Width = value;
                _halfWidth = _width * 0.5f;
                _midpointX = _min.X + _halfWidth;
            }
        }

        /// <summary>
        /// Gets or sets the height of the bounding box.
        /// </summary>
        new public float Height
        {
            get { return _height; }

            set
            {
                base.Height = value;
                _halfHeight = _height * 0.5f;
                _midpointY = _min.Y + _halfHeight;
            }
        }

        /// <summary>
        /// Gets half of the width value of the bounding box.
        /// </summary>
        public float HalfWidth
        {
            get { return _halfWidth; }
        }

        /// <summary>
        /// Gets half of the height value of the bounding box.
        /// </summary>
        public float HalfHeight
        {
            get { return _halfHeight; }
        }

        /// <summary>
        /// Gets the x position of the center of the bounding box.
        /// </summary>
        public float MidpointX
        {
            get { return _midpointX; }
        }

        /// <summary>
        /// Gets the y position of the center of the bounding box.
        /// </summary>
        public float MidpointY
        {
            get { return _midpointY; }
        }

        /// <summary>
        /// Creates an axis-aligned bounding box.
        /// </summary>
        /// <param name="x">The x position of the top-left corner of the bounding box.</param>
        /// <param name="y">The y position of the top-left corner of the bounding box.</param>
        /// <param name="width">The width of the bounding box.</param>
        /// <param name="height">The height of the bounding box.</param>
        public GenAABB(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Gets the x and y intersection depths of two bounding boxes.
        /// </summary>
        /// <param name="box">The bounding box to check against for an intersection.</param>
        /// <returns>A <c>Vector2</c> containing the x and y intersection depths, or a zero vector if no intersection occurs.</returns>
        public Vector2 GetIntersectDepthAABB(GenAABB box)
        {
            // Check for intersection along the x-axis first.
            float distanceX = _midpointX - box.MidpointX;
            float minDistanceX = _halfWidth + box.HalfWidth;

            // If the horizontal distance between the midpoints is less than the sum of the widths, continue on the y-axis.
            if (Math.Abs(distanceX) < minDistanceX)
            {
                float distanceY = _midpointY - box.MidpointY;
                float minDistanceY = _halfHeight + box.HalfHeight;

                // If the vertical distance between the midpoints is less than the sum of the heights, there is an intersection. 
                if (Math.Abs(distanceY) < minDistanceY)
                {
                    Vector2 intersection;

                    // Get the intersection depths.
                    intersection.X = (distanceX > 0) ? minDistanceX - distanceX : -minDistanceX - distanceX;
                    intersection.Y = (distanceY > 0) ? minDistanceY - distanceY : -minDistanceY - distanceY;

                    return intersection;
                }
            }

            // Return a zero vector if there is no intersection.
            return Vector2.Zero;
        }

        /// <summary>
        /// Gets the positive or negative distances between the edges of two bounding boxes along the x-axis and y-axis.
        /// </summary>
        /// <param name="box">The bounding box to get the distance from.</param>
        /// <returns>A <c>Vector2</c> containing the positive or negative distances between the edges of two bounding boxes along the x-axis and y-axis.</returns>
        public Vector2 GetDistanceAABB(GenAABB box)
        {
            Vector2 distance;

            distance.X = Math.Abs(_midpointX - box.MidpointX) - (_halfWidth + box.HalfWidth);
            distance.Y = Math.Abs(_midpointY - box.MidpointY) - (_halfHeight + box.HalfHeight);

            return distance;
        }
    }
}