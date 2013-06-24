using Microsoft.Xna.Framework;

namespace Genetic.Geometry
{
    /// <summary>
    /// Represents a basic axis-aligned bounding box.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenAABBBasic
    {
        /// <summary>
        /// The minimum x and y values of the bounding box, representing the top-left corner.
        /// </summary>
        protected Vector2 _min;

        /// <summary>
        /// The maximum x and y values of the bounding box, representing the bottom-right corner.
        /// </summary>
        protected Vector2 _max;

        /// <summary>
        /// The width of the bounding box.
        /// </summary>
        protected float _width;

        /// <summary>
        /// The height of the bounding box.
        /// </summary>
        protected float _height;

        /// <summary>
        /// Gets or sets the x position of the top-left corner of the bounding box.
        /// </summary>
        public float X
        {
            get { return _min.X; }

            set
            {
                _min.X = value;
                _max.X = _min.X + _width;
            }
        }

        /// <summary>
        /// Gets or sets the y position of the top-left corner of the bounding box.
        /// </summary>
        public float Y
        {
            get { return _min.Y; }

            set
            {
                _min.Y = value;
                _max.Y = _min.Y + _height;
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
                _max.X = _min.X + _width;
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
                _max.Y = _min.Y + _height;
            }
        }

        /// <summary>
        /// Gets the position of the left edge of the bounding box along the x-axis.
        /// </summary>
        public float Left
        {
            get { return _min.X; }
        }

        /// <summary>
        /// Gets the position of the right edge of the bounding box along the x-axis.
        /// </summary>
        public float Right
        {
            get { return _max.X; }
        }

        /// <summary>
        /// Gets the position of the top edge of the bounding box along the y-axis.
        /// </summary>
        public float Top
        {
            get { return _min.Y; }
        }

        /// <summary>
        /// Gets the position of the bottom edge of the bounding box along the y-axis.
        /// </summary>
        public float Bottom
        {
            get { return _max.Y; }
        }

        /// <summary>
        /// Creates a basic axis-aligned bounding box.
        /// </summary>
        /// <param name="x">The x position of the top-left corner of the bounding box.</param>
        /// <param name="y">The y position of the top-left corner of the bounding box.</param>
        /// <param name="width">The width of the bounding box.</param>
        /// <param name="height">The height of the bounding box.</param>
        public GenAABBBasic(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Checks if the bounding box intersects with a given point.
        /// </summary>
        /// <param name="point">The point to check for an intersection.</param>
        /// <returns>True if an intersection occurs, false if not.</returns>
        public bool Intersects(Vector2 point)
        {
            if ((point.X < _min.X) || (point.X > _max.X))
                return false;

            if ((point.Y < _min.Y) || (point.Y > _max.Y))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if the bounding box intersects with another bounding box.
        /// </summary>
        /// <param name="box">The bounding box to check for an intersection.</param>
        /// <returns>True if an intersection occurs, false if not.</returns>
        public bool Intersects(GenAABBBasic box)
        {
            if ((box.Right < _min.X) || (box.Left > _max.X))
                return false;

            if ((box.Bottom < _min.Y) || (box.Top > _max.Y))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if the bounding box entirely contains another bounding box.
        /// </summary>
        /// <param name="box">The bounding box to check against.</param>
        /// <returns>True if the bounding box is entirely contained, false if not.</returns>
        public bool Contains(GenAABBBasic box)
        {
            if ((box.Left <= Left) || (box.Right >= Right))
                return false;

            if ((box.Top <= Top) || (box.Bottom >= Bottom))
                return false;

            return true;
        }
    }
}