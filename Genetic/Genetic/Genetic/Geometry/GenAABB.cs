using System;

using Microsoft.Xna.Framework;

namespace Genetic.Geometry
{
    /// <summary>
    /// Represents an axis-aligned bounding box.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenAABB
    {
        /// <summary>
        /// The minimum x value of the bounding box, representing the left edge.
        /// </summary>
        protected float _minX;

        /// <summary>
        /// The maximum x value of the bounding box, representing the right edge.
        /// </summary>
        protected float _maxX;

        /// <summary>
        /// The minimum y value of the bounding box, representing the top edge.
        /// </summary>
        protected float _minY;

        /// <summary>
        /// The maximum y value of the bounding box, representing the bottom edge.
        /// </summary>
        protected float _maxY;

        /// <summary>
        /// The width of the bounding box.
        /// </summary>
        protected float _width;

        /// <summary>
        /// The height of the bounding box.
        /// </summary>
        protected float _height;

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
        /// Gets or sets the x position of the top-left corner of the bounding box.
        /// </summary>
        public float X
        {
            get { return _minX; }

            set
            {
                _minX = value;
                _maxX = _minX + _width;
                _midpointX = _minX + _halfWidth;
            }
        }

        /// <summary>
        /// Gets or sets the y position of the top-left corner of the bounding box.
        /// </summary>
        public float Y
        {
            get { return _minY; }

            set
            {
                _minY = value;
                _maxY = _minY + _height;
                _midpointY = _minY + _halfHeight;
            }
        }

        /// <summary>
        /// Gets or sets the width of the bounding box.
        /// </summary>
        public float Width
        {
            get { return _width; }

            set
            {
                _width = value;
                _maxX = _minX + _width;
                _halfWidth = _width * 0.5f;
                _midpointX = _minX + _halfWidth;
            }
        }

        /// <summary>
        /// Gets or sets the height of the bounding box.
        /// </summary>
        public float Height
        {
            get { return _height; }

            set
            {
                _height = value;
                _maxY = _minY + _height;
                _halfHeight = _height * 0.5f;
                _midpointY = _minY + _halfHeight;
            }
        }

        /// <summary>
        /// Gets the position of the left edge of the bounding box along the x-axis.
        /// </summary>
        public float Left
        {
            get { return _minX; }
        }

        /// <summary>
        /// Gets the position of the right edge of the bounding box along the x-axis.
        /// </summary>
        public float Right
        {
            get { return _maxX; }
        }

        /// <summary>
        /// Gets the position of the top edge of the bounding box along the y-axis.
        /// </summary>
        public float Top
        {
            get { return _minY; }
        }

        /// <summary>
        /// Gets the position of the bottom edge of the bounding box along the y-axis.
        /// </summary>
        public float Bottom
        {
            get { return _maxY; }
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
        {
            _minX = x;
            _minY = y;
            _width = width;
            _height = height;
            _maxX = _minX + _width;
            _maxY = _minY + height;
            _halfWidth = _width * 0.5f;
            _halfHeight = _height * 0.5f;
            _midpointX = _minX + _halfWidth;
            _midpointY = _minY + _halfHeight;
        }

        /// <summary>
        /// Checks if the bounding box intersects with a given point.
        /// </summary>
        /// <param name="point">The point to check for an intersection.</param>
        /// <returns>True if an intersection occurs, false if not.</returns>
        public bool Intersects(Vector2 point)
        {
            if ((point.X < _minX) || (point.X > _maxX))
                return false;

            if ((point.Y < _minY) || (point.Y > _maxY))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if the bounding box intersects with another bounding box.
        /// </summary>
        /// <param name="box">The bounding box to check for an intersection.</param>
        /// <returns>True if an intersection occurs, false if not.</returns>
        public bool Intersects(GenAABB box)
        {
            if ((box._maxX < _minX) || (box._minX > _maxX))
                return false;

            if ((box._maxY < _minY) || (box._minY > _maxY))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if the bounding box entirely contains another bounding box.
        /// </summary>
        /// <param name="box">The bounding box to check against.</param>
        /// <returns>True if the bounding box is entirely contained, false if not.</returns>
        public bool Contains(GenAABB box)
        {
            if ((box._minX <= _minX) || (box._maxX >= _maxX))
                return false;

            if ((box._minY <= _minY) || (box._maxY >= _maxY))
                return false;

            return true;
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
                    Vector2 intersection = Vector2.Zero;

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
            Vector2 distance = Vector2.Zero;

            distance.X = Math.Abs(_midpointX - box.MidpointX) - (_halfWidth + box.HalfWidth);
            distance.Y = Math.Abs(_midpointY - box.MidpointY) - (_halfHeight + box.HalfHeight);

            return distance;
        }
    }
}