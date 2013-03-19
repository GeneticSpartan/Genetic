using System;

using Microsoft.Xna.Framework;

using Genetic.Geometry;

namespace Genetic
{
    public class GenObject : GenBasic
    {
        public enum Direction
        {
            None = 0,
            Left = 0x0001,
            Right = 0x0010,
            Up = 0x0100,
            Down = 0x1000,
            Any = Direction.Left | Direction.Right | Direction.Up | Direction.Down
        }

        /// <summary>
        /// The x and y position of the object.
        /// </summary>
        protected Vector2 _position;

        /// <summary>
        /// The bounding box of the object relative to the position.
        /// </summary>
        protected GenAABB _boundingBox;

        /// <summary>
        /// The bounding rectangle of the object relative to the position.
        /// Used during draw calls.
        /// </summary>
        protected Rectangle _positionRect;

        /// <summary>
        /// The bounding rectangle of the object relative to the position in the camera.
        /// </summary>
        //protected Rectangle _screenPositionRect;

        /// <summary>
        /// Determines if the object is affected by collisions.
        /// </summary>
        public bool Immovable = false;

        /// <summary>
        /// The mass of the object used when calculating collision response against another object.
        /// </summary>
        public float Mass = 1f;

        /// <summary>
        /// The x and y velocity of the object.
        /// </summary>
        public Vector2 Velocity;

        /// <summary>
        /// The x and y acceleration of the object.
        /// </summary>
        public Vector2 Acceleration = Vector2.Zero;

        /// <summary>
        /// The x and y deceleration of the object.
        /// </summary>
        public Vector2 Deceleration = Vector2.Zero;

        /// <summary>
        /// The maximum x and y velocities of the object.
        /// </summary>
        public Vector2 MaxVelocity;

        /// <summary>
        /// The direction that the object is facing.
        /// </summary>
        protected Direction _facing = Direction.None;

        /// <summary>
        /// A bit field of flags giving the directions that the object is colliding in during the previous update.
        /// </summary>
        public Direction WasTouching = Direction.None;

        /// <summary>
        /// A bit field of flags giving the current directions that the object is colliding in.
        /// </summary>
        public Direction Touching = Direction.None;

        /// <summary>
        /// Gets or sets the x position the object.
        /// </summary>
        public float X
        {
            get { return _position.X; }

            set
            {
                _position.X = value;
                _boundingBox.X = value;
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
                _boundingBox.Y = value;
                _positionRect.Y = (int)value;
            }
        }

        /// <summary>
        /// Gets the bounding box of the object relative to the position.
        /// </summary>
        public GenAABB BoundingBox
        {
            get { return _boundingBox; }
        }

        /// <summary>
        /// Gets the bounding rectangle of the object relative to the position.
        /// Used for draw calls.
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
        public float Width
        {
            get { return _boundingBox.Width; }

            set
            {
                _boundingBox.Width = value;
                _positionRect.Width = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the height the object.
        /// </summary>
        public float Height
        {
            get { return _boundingBox.Height; }

            set
            {
                _boundingBox.Height = value;
                _positionRect.Height = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the direction that the object is facing.
        /// </summary>
        public Direction Facing
        {
            get { return _facing; }

            set { _facing = value; }
        }

        public GenObject(float x = 0, float y = 0, float width = 0, float height = 0)
        {
            _position = new Vector2(x, y);
            _boundingBox = new GenAABB(x, y, width, height);
            _positionRect = new Rectangle((int)_position.X, (int)_position.Y, (int)width, (int)height);
            //_screenPositionRect = Rectangle.Empty;

            //_screenPositionRect.X = (int)((_positionRect.X + GenG.camera.ScrollX) * GenG.camera.Zoom);
            //_screenPositionRect.Y = (int)((_positionRect.Y + GenG.camera.ScrollY) * GenG.camera.Zoom);
            //_screenPositionRect.Width = (int)(_positionRect.Width * GenG.camera.Zoom);
            //_screenPositionRect.Height = (int)(_positionRect.Height * GenG.camera.Zoom);

            Velocity = Vector2.Zero;
            MaxVelocity = Vector2.Zero;
        }

        /// <summary>
        /// Override this method to add additional update logic.
        /// </summary>
        public override void Update()
        {
            if (Acceleration.X != 0)
                Velocity.X += Acceleration.X * GenG.PhysicsTimeStep;
            else if (Deceleration.X != 0)
            {
                if (Velocity.X > 0)
                {
                    Velocity.X -= Deceleration.X * GenG.PhysicsTimeStep;

                    if (Velocity.X < 0)
                        Velocity.X = 0;
                }
                else if (Velocity.X < 0)
                {
                    Velocity.X += Deceleration.X * GenG.PhysicsTimeStep;

                    if (Velocity.X > 0)
                        Velocity.X = 0;
                }
            }

            if (Acceleration.Y != 0)
                Velocity.Y += Acceleration.Y * GenG.PhysicsTimeStep;
            else if (Deceleration.Y != 0)
            {
                if (Velocity.Y > 0)
                {
                    Velocity.Y -= Deceleration.Y * GenG.PhysicsTimeStep;

                    if (Velocity.Y < 0)
                        Velocity.Y = 0;
                }
                else if (Velocity.Y < 0)
                {
                    Velocity.Y += Deceleration.Y * GenG.PhysicsTimeStep;

                    if (Velocity.Y > 0)
                        Velocity.Y = 0;
                }
            }

            // Limit the object's velocity to the maximum velocity.
            if (MaxVelocity.X != 0)
                Velocity.X = MathHelper.Clamp(Velocity.X, -MaxVelocity.X, MaxVelocity.X);

            if (MaxVelocity.Y != 0)
                Velocity.Y = MathHelper.Clamp(Velocity.Y, -MaxVelocity.Y, MaxVelocity.Y);

            // Move the object based on its velocity.
            X += Velocity.X * GenG.PhysicsTimeStep;
            Y += Velocity.Y * GenG.PhysicsTimeStep;

            //_screenPositionRect.X = (int)((_positionRect.X + GenG.camera.ScrollX) * GenG.camera.Zoom);
            //_screenPositionRect.Y = (int)((_positionRect.Y + GenG.camera.ScrollY) * GenG.camera.Zoom);
            //_screenPositionRect.Width = (int)(_positionRect.Width * GenG.camera.Zoom);
            //_screenPositionRect.Height = (int)(_positionRect.Height * GenG.camera.Zoom);
        }

        /// <summary>
        /// Override this method to add additional post-update logic.
        /// </summary>
        public override void PostUpdate()
        {
            // Reset the bit fields for collision flags.
            WasTouching = Touching;
            Touching = Direction.None;
        }

        public override void Draw()
        {
            if (GenG.IsDebug)
            {
                GenG.SpriteBatch.Draw(GenG.Pixel, _positionRect, _positionRect, Color.Lime * 0.5f);
                //GenG.DrawLine(_positionRect.Left, _positionRect.Top, _positionRect.Right, _positionRect.Top, Color.Lime);
                //GenG.DrawLine(_positionRect.Right, _positionRect.Top, _positionRect.Right, _positionRect.Bottom, Color.Lime);
                //GenG.DrawLine(_positionRect.Left, _positionRect.Bottom - 1, _positionRect.Right, _positionRect.Bottom - 1, Color.Lime);
                //GenG.DrawLine(_positionRect.Left + 1, _positionRect.Top, _positionRect.Left + 1, _positionRect.Bottom, Color.Lime);
            }
        }

        /// <summary>
        /// Checks if the object is colliding in the given direction.
        /// </summary>
        /// <param name="direction">The direction to check for collision.</param>
        /// <returns>True if there is a collision in the given direction, false if not.</returns>
        public bool IsTouching(Direction direction)
        {
            return (Touching & direction) > Direction.None;
        }

        /// <summary>
        /// Checks if the object was colliding in the given direction during the previous update.
        /// </summary>
        /// <param name="direction">The direction to check for collision.</param>
        /// <returns>True if there was a collision in the given direction, false if not.</returns>
        public bool WasTouched(Direction direction)
        {
            return (WasTouching & direction) > Direction.None;
        }

        /// <summary>
        /// Checks if the object just collided in the given direction.
        /// </summary>
        /// <param name="direction">The direction to check for collision.</param>
        /// <returns>True if there is a collision in the given direction, false if not.</returns>
        public bool JustTouched(Direction direction)
        {
            return (((WasTouching & direction) == Direction.None) && ((Touching & direction) > Direction.None));
        }
    }
}