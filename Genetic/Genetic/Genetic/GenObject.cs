using Microsoft.Xna.Framework;

namespace Genetic
{
    public enum Facing { Left, Right, Up, Down };

    public class GenObject : GenBasic
    {
        /// <summary>
        /// The x and y position of the object.
        /// </summary>
        protected Vector2 _position;

        /// <summary>
        /// The bounding rectangle of the object relative to the position.
        /// </summary>
        protected Rectangle _positionRect;

        /// <summary>
        /// The bounding rectangle of the object relative to the position in the camera.
        /// </summary>
        //protected Rectangle _screenPositionRect;

        /// <summary>
        /// The x and y velocity of the object.
        /// </summary>
        public Vector2 velocity;

        /// <summary>
        /// The x and y acceleration of the object.
        /// </summary>
        public Vector2 acceleration;

        /// <summary>
        /// The maximum x and y velocities of the object.
        /// </summary>
        public Vector2 maxVelocity;

        /// <summary>
        /// The direction that the object is facing.
        /// The object is facing right by default.
        /// </summary>
        protected Facing _facing = Facing.Right;

        /// <summary>
        /// Gets or sets the x position the object.
        /// </summary>
        public float X
        {
            get { return _position.X; }

            set
            {
                _position.X = value;
                _positionRect.X = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the y position the object.
        /// </summary>
        public float Y
        {
            get { return _position.Y; }

            set
            {
                _position.Y = value;
                _positionRect.Y = (int)value;
            }
        }

        /// <summary>
        /// Gets the bounding rectangle of the object relative to the position.
        /// </summary>
        public Rectangle PositionRect
        {
            get { return _positionRect; }
        }

        /// <summary>
        /// Gets the bounding rectangle of the object relative to the position in the camera.
        /// </summary>
        //public Rectangle ScreenPositionRect
        //{
        //    get { return _screenPositionRect; }
        //}

        /// <summary>
        /// Gets or sets the width the object.
        /// </summary>
        public int Width
        {
            get { return _positionRect.Width; }

            set { _positionRect.Width = value; }
        }

        /// <summary>
        /// Gets or sets the height the object.
        /// </summary>
        public int Height
        {
            get { return _positionRect.Height; }

            set { _positionRect.Height = value; }
        }

        /// <summary>
        /// Gets or sets the direction that the object is facing.
        /// </summary>
        public Facing Facing
        {
            get { return _facing; }

            set { _facing = value; }
        }

        public GenObject(float x = 0, float y = 0, int width = 0, int height = 0)
        {
            _position = new Vector2(x, y);
            _positionRect = new Rectangle((int)_position.X, (int)_position.Y, width, height);
            //_screenPositionRect = Rectangle.Empty;

            //_screenPositionRect.X = (int)((_positionRect.X + GenG.camera.ScrollX) * GenG.camera.Zoom);
            //_screenPositionRect.Y = (int)((_positionRect.Y + GenG.camera.ScrollY) * GenG.camera.Zoom);
            //_screenPositionRect.Width = (int)(_positionRect.Width * GenG.camera.Zoom);
            //_screenPositionRect.Height = (int)(_positionRect.Height * GenG.camera.Zoom);

            velocity = Vector2.Zero;
            maxVelocity = Vector2.Zero;
        }

        /// <summary>
        /// Override this method to add additional update logic.
        /// </summary>
        public override void Update()
        {
            if (acceleration.X != 0)
                velocity.X += acceleration.X * GenG.timeScale * GenG.deltaTime;

            if (acceleration.Y != 0)
                velocity.Y += acceleration.Y * GenG.timeScale * GenG.deltaTime;

            // Limit the object's velocity to the maximum velocity.
            if ((maxVelocity.X != 0) && (velocity.X > maxVelocity.X))
                velocity.X = maxVelocity.X;

            if ((maxVelocity.Y != 0) && (velocity.Y > maxVelocity.Y))
                velocity.Y = maxVelocity.Y;

            // Move the object based on its velocity.
            X += velocity.X * GenG.timeScale * GenG.deltaTime;
            Y += velocity.Y * GenG.timeScale * GenG.deltaTime;

            //_screenPositionRect.X = (int)((_positionRect.X + GenG.camera.ScrollX) * GenG.camera.Zoom);
            //_screenPositionRect.Y = (int)((_positionRect.Y + GenG.camera.ScrollY) * GenG.camera.Zoom);
            //_screenPositionRect.Width = (int)(_positionRect.Width * GenG.camera.Zoom);
            //_screenPositionRect.Height = (int)(_positionRect.Height * GenG.camera.Zoom);
        }

        public override void Draw()
        {
            if (GenG.isDebug)
            {
                GenG.SpriteBatch.Draw(GenG.pixel, _positionRect, _positionRect, Color.Lime * 0.5f);
                //GenG.DrawLine(_positionRect.Left, _positionRect.Top, _positionRect.Right, _positionRect.Top, Color.Lime);
                //GenG.DrawLine(_positionRect.Right, _positionRect.Top, _positionRect.Right, _positionRect.Bottom, Color.Lime);
                //GenG.DrawLine(_positionRect.Left, _positionRect.Bottom - 1, _positionRect.Right, _positionRect.Bottom - 1, Color.Lime);
                //GenG.DrawLine(_positionRect.Left + 1, _positionRect.Top, _positionRect.Left + 1, _positionRect.Bottom, Color.Lime);
            }
        }
    }
}