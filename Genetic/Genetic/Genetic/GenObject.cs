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
        /// The x and y distances the object has moved between the current and previous updates relative to its velocity.
        /// Used by GetMoveBounds() to calculate the movement bounding box of the object.
        /// Call GetMoveBounds() to refresh the movement distance.
        /// </summary>
        protected Vector2 _moveDistance = Vector2.Zero;

        /// <summary>
        /// The bounding box containing the object at its current and predicted positions relative to its velocity.
        /// </summary>
        protected GenAABB _moveBounds;

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
        /// Gets the bounding box containing the object at its current and predicted positions relative to its velocity.
        /// Useful for checking if the object may collide with another object during the next update.
        /// Call GetMoveBounds() first to refresh the movement bounding box.
        /// </summary>
        public GenAABB MoveBounds
        {
            get { return _moveBounds; }
        }

        /// <summary>
        /// Gets or sets the direction that the object is facing.
        /// </summary>
        public Direction Facing
        {
            get { return _facing; }

            set { _facing = value; }
        }

        /// <summary>
        /// A physical object that can be moved around in screen space.
        /// </summary>
        /// <param name="x">The x position of the top-left corner of the object.</param>
        /// <param name="y">The y position of the top-left corner of the object.</param>
        /// <param name="width">The width of the object.</param>
        /// <param name="height">The height of the object.</param>
        public GenObject(float x = 0, float y = 0, float width = 0, float height = 0)
        {
            _position = new Vector2(x, y);
            _boundingBox = new GenAABB(x, y, width, height);
            _positionRect = new Rectangle((int)_position.X, (int)_position.Y, (int)width, (int)height);
            _moveBounds = new GenAABB(x, y, width, height);

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

            // Move the object.
            X += Velocity.X * GenG.PhysicsTimeStep;
            Y += Velocity.Y * GenG.PhysicsTimeStep;
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

        /// <summary>
        /// Draws a box representing the collision area of the object.
        /// </summary>
        public override void DrawDebug()
        {
            GenG.SpriteBatch.Draw(GenG.Pixel, _positionRect, _positionRect, ((Immovable) ? Color.Red : Color.Lime) * 0.5f);

            //GenG.DrawLine(_positionRect.Left, _positionRect.Top, _positionRect.Right, _positionRect.Top, ((Immovable) ? Color.Red : Color.Lime) * 0.5f);
            //GenG.DrawLine(_positionRect.Right, _positionRect.Top, _positionRect.Right, _positionRect.Bottom, ((Immovable) ? Color.Red : Color.Lime) * 0.5f);
            //GenG.DrawLine(_positionRect.Left, _positionRect.Bottom - 1, _positionRect.Right, _positionRect.Bottom - 1, ((Immovable) ? Color.Red : Color.Lime) * 0.5f);
            //GenG.DrawLine(_positionRect.Left + 1, _positionRect.Top, _positionRect.Left + 1, _positionRect.Bottom, ((Immovable) ? Color.Red : Color.Lime) * 0.5f);
        }

        /// <summary>
        /// Gets a bounding box containing the object at its current and predicted positions relative to its velocity.
        /// </summary>
        /// <returns>The bounding box containing the object at its current and predicted positions relative to its velocity.</returns>
        public GenAABB GetMoveBounds()
        {
            // Get the x and y distances that the object will move relative to its velocity.
            _moveDistance.X = Velocity.X * GenG.PhysicsTimeStep;
            _moveDistance.Y = Velocity.Y * GenG.PhysicsTimeStep;

            // Calculate the movement bounding box.
            float minLeft = Math.Min(_boundingBox.Left, _boundingBox.Left + _moveDistance.X);
            float minTop = Math.Min(_boundingBox.Top, _boundingBox.Top + _moveDistance.Y);
            float maxRight = Math.Max(_boundingBox.Right, _boundingBox.Right + _moveDistance.X);
            float maxBottom = Math.Max(_boundingBox.Bottom, _boundingBox.Bottom + _moveDistance.Y);

            _moveBounds.X = minLeft;
            _moveBounds.Y = minTop;
            _moveBounds.Width = maxRight - minLeft;
            _moveBounds.Height = maxBottom - minTop;

            return _moveBounds;
        }

        /// <summary>
        /// Checks if the object overlaps the given camera's view area.
        /// </summary>
        /// <param name="camera">The camera to check.</param>
        /// <returns>True if the object overlaps the camera's view area, false if not.</returns>
        public bool IsOnScreen(GenCamera camera)
        {
            return ((_boundingBox.Left < camera.CameraView.Right) && (_boundingBox.Right > camera.CameraView.Left) && (_boundingBox.Top < camera.CameraView.Bottom) && (_boundingBox.Bottom > camera.CameraView.Top));
        }

        /// <summary>
        /// Checks for overlap between the movements bounds of this object and a given object.
        /// </summary>
        /// <param name="gameObject">The object to check for an overlap.</param>
        /// <returns>True if an overlap occurs, false if not.</returns>
        public bool Overlap(GenObject gameObject)
        {
            return GetMoveBounds().Intersects(gameObject.GetMoveBounds());
        }

        /// <summary>
        /// Applys collision detection and response against another object that may overlap this object.
        /// </summary>
        /// <param name="gameObject">The object to check for a collision.</param>
        /// <param name="penetrate">Determines if the objects are able to penetrate each other for elastic collision response.</param>
        /// <param name="collidableEdges">A bit field of flags determining which edges of the given object are collidable.</param>
        /// <returns>True is a collision occurs, false if not.</returns>
        public bool Collide(GenObject gameObject, bool penetrate = true, GenObject.Direction collidableEdges = GenObject.Direction.Any)
        {
            if (!this.Equals(gameObject))
            {
                if (Immovable && gameObject.Immovable)
                    return false;

                if (Overlap(gameObject))
                {
                    Vector2 distances = GenU.GetDistanceAABB(_boundingBox, gameObject.BoundingBox);
                    Vector2 collisionNormal;

                    if (distances.X > distances.Y)
                        collisionNormal = (_boundingBox.MidpointX > gameObject.BoundingBox.MidpointX) ? new Vector2(-1, 0) : new Vector2(1, 0);
                    else
                        collisionNormal = (_boundingBox.MidpointY > gameObject.BoundingBox.MidpointY) ? new Vector2(0, -1) : new Vector2(0, 1);

                    if (((collisionNormal.X == 1) && ((collidableEdges & GenObject.Direction.Left) == GenObject.Direction.Left)) ||
                        ((collisionNormal.X == -1) && ((collidableEdges & GenObject.Direction.Right) == GenObject.Direction.Right)) ||
                        ((collisionNormal.Y == 1) && ((collidableEdges & GenObject.Direction.Up) == GenObject.Direction.Up)) ||
                        ((collisionNormal.Y == -1) && ((collidableEdges & GenObject.Direction.Down) == GenObject.Direction.Down)))
                    {
                        float distance = Math.Max(distances.X, distances.Y);
                        float remove;

                        // Apply a different collision response against tiles for pixel-perfect accuracy.
                        if (gameObject is GenTile)
                        {
                            remove = Vector2.Dot(-Velocity, collisionNormal) + Math.Max(distance, 0) / GenG.PhysicsTimeStep;

                            if (remove < 0)
                            {
                                if (collisionNormal.X != 0)
                                {
                                    if (collisionNormal.X == -1)
                                        X = gameObject.BoundingBox.Right;
                                    else
                                        X = gameObject.X - _boundingBox.Width;

                                    Velocity.X = 0;
                                }
                                else
                                {
                                    if (collisionNormal.Y == -1)
                                        Y = gameObject.BoundingBox.Bottom;
                                    else
                                        Y = gameObject.Y - _boundingBox.Height;

                                    Velocity.Y = 0;
                                }
                            }
                        }
                        else
                        {
                            float relativeNormalVelocity = Vector2.Dot(gameObject.Velocity - Velocity, collisionNormal);

                            if (penetrate)
                                remove = relativeNormalVelocity + distance / GenG.PhysicsTimeStep;
                            else
                                remove = relativeNormalVelocity + Math.Max(distance, 0) / GenG.PhysicsTimeStep;

                            if (remove < 0)
                            {
                                float impulse = remove / (Mass + gameObject.Mass);

                                if (!Immovable)
                                {
                                    Velocity += impulse * collisionNormal * gameObject.Mass;

                                    if (!penetrate)
                                    {
                                        float penetration = Math.Min(distance, 0);

                                        if (collisionNormal.X != 0)
                                            X += penetration * collisionNormal.X;
                                        else
                                            Y += penetration * collisionNormal.Y;
                                    }
                                }

                                if (!gameObject.Immovable)
                                {
                                    gameObject.Velocity -= impulse * collisionNormal * Mass;

                                    if (!penetrate)
                                    {
                                        float penetration = Math.Min(distance, 0);

                                        if (collisionNormal.X != 0)
                                            gameObject.X -= penetration * collisionNormal.X;
                                        else
                                            gameObject.Y -= penetration * collisionNormal.Y;
                                    }
                                }
                            }
                        }

                        if (remove < 0)
                        {
                            if (collisionNormal.X != 0)
                            {
                                if (collisionNormal.X == 1)
                                {
                                    Touching |= Direction.Right;
                                    gameObject.Touching |= Direction.Left;
                                }
                                else
                                {
                                    Touching |= Direction.Left;
                                    gameObject.Touching |= Direction.Right;
                                }
                            }
                            else
                            {
                                if (collisionNormal.Y == 1)
                                {
                                    Touching |= Direction.Down;
                                    gameObject.Touching |= Direction.Up;
                                }
                                else
                                {
                                    Touching |= Direction.Up;
                                    gameObject.Touching |= Direction.Down;
                                }
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
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