namespace Genetic.Geometry
{
    public class GenAABB
    {
        /// <summary>
        /// The x position of the top left corner of the bounding box.
        /// </summary>
        protected float _x;

        /// <summary>
        /// The y position of the top left corner of the bounding box.
        /// </summary>
        protected float _y;

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
        /// The position of the left edge of the bounding box along the x-axis.
        /// </summary>
        protected float _left;

        /// <summary>
        /// The position of the right edge of the bounding box along the x-axis.
        /// </summary>
        protected float _right;

        /// <summary>
        /// The position of the top edge of the bounding box along the y-axis.
        /// </summary>
        protected float _top;

        /// <summary>
        /// The position of the bottom edge of the bounding box along the y-axis.
        /// </summary>
        protected float _bottom;

        /// <summary>
        /// Gets or sets the x position of the top left corner of the bounding box.
        /// </summary>
        public float X
        {
            get { return _x; }

            set
            {
                _x = value;
                _midpointX = _x + _halfWidth;
                _left = _x;
                _right = _x + _width;
            }
        }

        /// <summary>
        /// Gets or sets the y position of the top left corner of the bounding box.
        /// </summary>
        public float Y
        {
            get { return _y; }

            set
            {
                _y = value;
                _midpointY = value + _halfHeight;
                _top = _y;
                _bottom = _y + _height;
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
                _halfWidth = _width * 0.5f;
                _midpointX = _x + _halfWidth;
                _right = _x + _width;
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
                _halfHeight = _height * 0.5f;
                _midpointY = _y + _halfHeight;
                _bottom = _y + _height;
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
        /// Gets the position of the left edge of the bounding box along the x-axis.
        /// </summary>
        public float Left
        {
            get { return _left; }
        }

        /// <summary>
        /// Gets the position of the right edge of the bounding box along the x-axis.
        /// </summary>
        public float Right
        {
            get { return _right; }
        }

        /// <summary>
        /// Gets the position of the top edge of the bounding box along the y-axis.
        /// </summary>
        public float Top
        {
            get { return _top; }
        }

        /// <summary>
        /// Gets the position of the bottom edge of the bounding box along the y-axis.
        /// </summary>
        public float Bottom
        {
            get { return _bottom; }
        }

        public GenAABB(float x, float y, float width, float height)
        {
            // Set the width and height values first to get the half-width and half-height values.
            Width = width;
            Height = height;

            X = x;
            Y = y;
        }

        /// <summary>
        /// Checks if the bounding box intersects with another bounding box.
        /// </summary>
        /// <param name="box">The bounding box to check for an intersection.</param>
        /// <returns>True if an intersection occurs, false if not.</returns>
        public bool Intersects(GenAABB box)
        {
            return ((_left < box.Right) && (_right > box.Left) && (_top < box.Bottom) && (_bottom > box.Top));
        }
    }
}