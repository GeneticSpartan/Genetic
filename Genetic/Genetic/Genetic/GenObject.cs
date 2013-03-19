﻿using System;

using Microsoft.Xna.Framework;

using Genetic.Geometry;

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
        public bool immovable = false;

        /// <summary>
        /// The mass of the object used when calculating collision response against another object.
        /// </summary>
        public float mass = 1f;

        /// <summary>
        /// The x and y velocity of the object.
        /// </summary>
        public Vector2 velocity;

        /// <summary>
        /// The x and y acceleration of the object.
        /// </summary>
        public Vector2 acceleration = Vector2.Zero;

        /// <summary>
        /// The x and y deceleration of the object.
        /// </summary>
        public Vector2 deceleration = Vector2.Zero;

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
        public Facing Facing
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

            velocity = Vector2.Zero;
            maxVelocity = Vector2.Zero;
        }

        /// <summary>
        /// Override this method to add additional update logic.
        /// </summary>
        public override void Update()
        {
            if (acceleration.X != 0)
                velocity.X += acceleration.X * GenG.PhysicsTimeStep;
            else if (deceleration.X != 0)
            {
                if (velocity.X > 0)
                {
                    velocity.X -= deceleration.X * GenG.PhysicsTimeStep;

                    if (velocity.X < 0)
                        velocity.X = 0;
                }
                else if (velocity.X < 0)
                {
                    velocity.X += deceleration.X * GenG.PhysicsTimeStep;

                    if (velocity.X > 0)
                        velocity.X = 0;
                }
            }

            if (acceleration.Y != 0)
                velocity.Y += acceleration.Y * GenG.PhysicsTimeStep;
            else if (deceleration.Y != 0)
            {
                if (velocity.Y > 0)
                {
                    velocity.Y -= deceleration.Y * GenG.PhysicsTimeStep;

                    if (velocity.Y < 0)
                        velocity.Y = 0;
                }
                else if (velocity.Y < 0)
                {
                    velocity.Y += deceleration.Y * GenG.PhysicsTimeStep;

                    if (velocity.Y > 0)
                        velocity.Y = 0;
                }
            }

            // Limit the object's velocity to the maximum velocity.
            if (maxVelocity.X != 0)
                velocity.X = MathHelper.Clamp(velocity.X, -maxVelocity.X, maxVelocity.X);

            if (maxVelocity.Y != 0)
                velocity.Y = MathHelper.Clamp(velocity.Y, -maxVelocity.Y, maxVelocity.Y);

            // Move the object based on its velocity.
            X += velocity.X * GenG.PhysicsTimeStep;
            Y += velocity.Y * GenG.PhysicsTimeStep;

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