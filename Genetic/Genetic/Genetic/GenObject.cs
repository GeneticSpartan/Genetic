using System;

using Microsoft.Xna.Framework;

using Genetic.Geometry;
using Genetic.Path;

namespace Genetic
{
    public class GenObject : GenBasic
    {
        // Bit field flags representing up, down, left, and right directions.
        public enum Direction
        {
            None = 0,
            Left = 0x0001,
            Right = 0x0010,
            Up = 0x0100,
            Down = 0x1000,
            Any = Direction.Left | Direction.Right | Direction.Up | Direction.Down
        }

        // A bit field of flags determining the type of transformations that can be connected with a parent object.
        public enum ParentType
        {
            // The object will not be transformed by a parent.
            None = 0,

            // The object will move relative to its origin and the parent's origin position.
            Position = 0x001,

            // The object's Rotation value will change relative to a parent's Rotation value.
            Rotation = 0x010,

            // The object will move and rotate around the origin point of a parent, relative to the parent's rotation.
            // The object's Rotation value will not change.
            Origin = 0x100,

            // The object will move and rotate around the origin point of a parent, relative to the parent's rotation.
            // The object's Rotation value will change relative to the parent's Rotation value.
            OriginRotation = ParentType.Rotation | ParentType.Origin
        }

        /// <summary>
        /// The x and y positions of the object.
        /// </summary>
        protected Vector2 _position;

        /// <summary>
        /// The x and y positions of the object during the previous update.
        /// </summary>
        protected Vector2 _oldPosition;

        /// <summary>
        /// The x and y positions of the center point of the object.
        /// </summary>
        protected Vector2 _centerPosition;

        /// <summary>
        /// The x and y positions to draw debug objects.
        /// </summary>
        protected Vector2 _debugDrawPosition;

        /// <summary>
        /// The origin, relative to the position, used as an anchor point of the object.
        /// Using decimal values may cause sprites to shift between pixels in the pixel draw mode, giving undesired results.
        /// </summary>
        protected Vector2 _origin;

        /// <summary>
        /// The x and y positions of the origin of the object.
        /// Useful for rotating objects around this point that are parented to this object.
        /// </summary>
        protected Vector2 _originPosition;

        /// <summary>
        /// The bounding box of the object relative to the position.
        /// </summary>
        protected GenAABB _boundingBox;

        /// <summary>
        /// The bounding rectangle of the object.
        /// Used during draw debug calls.
        /// </summary>
        protected Rectangle _boundingRect;

        /// <summary>
        /// The rotation of the object in radians.
        /// </summary>
        protected float _rotation;

        /// <summary>
        /// The unmodified rotation value of the object in radians.
        /// Useful for saving the object's rotation while its Rotation value may be affected by a parent object.
        /// </summary>
        protected float _baseRotation;

        /// <summary>
        /// The speed of the object's rotation in degrees per second.
        /// </summary>
        public float RotationSpeed;

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
        /// A flag used to determine if the object is affected by collisions.
        /// </summary>
        public bool Immovable;

        /// <summary>
        /// A flag used to determine if the object can collide.
        /// </summary>
        public bool Solid;

        /// <summary>
        /// The mass of the object used when calculating collision response against another object.
        /// </summary>
        public float Mass = 1f;

        /// <summary>
        /// The x and y velocities of the object.
        /// </summary>
        public Vector2 Velocity;

        /// <summary>
        /// The x and y velocities of the object during the previous update.
        /// </summary>
        public Vector2 OldVelocity;

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
        /// The object that this object will be parented to.
        /// </summary>
        public GenObject Parent;

        // The type of transformations that are connected with a parent object.
        public ParentType ParentMode;

        /// <summary>
        /// The x and y position offsets relative to the parent object's position.
        /// </summary>
        public Vector2 ParentOffset;

        /// <summary>
        /// A flag used to allow other colliding objects to inherit the x velocity when sitting on this object.
        /// The colliding object will move horizontally along with this object.
        /// </summary>
        public bool IsPlatform = false;

        /// <summary>
        /// The current platform object that the object is interacting with.
        /// </summary>
        public GenObject Platform;

        /// <summary>
        /// The platform object that the object is interacting with during the previous update.
        /// </summary>
        public GenObject OldPlatform;

        /// <summary>
        /// A global container used to store vector calculation results.
        /// Useful for reducing Vector2 allocations.
        /// </summary>
        private static Vector2 _distancesVector = Vector2.Zero;

        /// <summary>
        /// A global container used to store vector calculation results.
        /// Useful for reducing Vector2 allocations.
        /// </summary>
        private static Vector2 _collisionNormalVector = Vector2.Zero;

        #region Path Fields
        /// <summary>
        /// A reference to the current path that the object is following.
        /// </summary>
        public GenPath Path = null;

        /// <summary>
        /// The index number of the current node in the path nodes list to move towards.
        /// </summary>
        public int PathNodeIndex = 0;

        /// <summary>
        /// The direction that the object will move along the path.
        /// Used to change direction in a yoyo path type.
        /// A value of 1 will move clockwise, and a value of -1 will move counterclockwise.
        /// </summary>
        public int PathDirection = 1;

        /// <summary>
        /// The velocity or acceleration of the object as it moves along the path.
        /// </summary>
        public float PathSpeed = 0;

        /// <summary>
        /// Determines the order to move along the path nodes.
        /// </summary>
        public GenPath.Type PathType = GenPath.Type.Clockwise;

        /// <summary>
        /// A bit field of flags used to determine the current movement axis of the object along a path.
        /// Used to constrain movement along a path horizontally, vertically, or both.
        /// </summary>
        public GenMove.Axis PathAxis = GenMove.Axis.Both;

        /// <summary>
        /// Determines whether to set the object's velocity or acceleration to move along the path.
        /// </summary>
        public GenPath.Movement PathMovement = GenPath.Movement.Instant;
        #endregion

        /// <summary>
        /// Gets the x and y positions of the object.
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Gets the x and y positions of the object during the previous update.
        /// </summary>
        public Vector2 OldPosition
        {
            get { return _oldPosition; }
        }

        /// <summary>
        /// Gets the x and y positions of the center point of the object.
        /// </summary>
        public Vector2 CenterPosition
        {
            get { return _centerPosition; }
        }

        /// <summary>
        /// Gets the x and y positions of the origin of the object relative to the object's position.
        /// </summary>
        public Vector2 Origin
        {
            get { return _origin; }
        }

        /// <summary>
        /// Gets the x and y positions of the origin of the object.
        /// </summary>
        public Vector2 OriginPosition
        {
            get { return _originPosition; }
        }

        /// <summary>
        /// Gets or sets the x position of the top-left corner the object.
        /// </summary>
        public float X
        {
            get { return _position.X; }

            set
            {
                _position.X = value;
                _boundingBox.X = value;
                _originPosition.X = _position.X + _origin.X;
            }
        }

        /// <summary>
        /// Gets or sets the y position of the top-left corner the object.
        /// </summary>
        public float Y
        {
            get { return _position.Y; }

            set
            {
                _position.Y = value;
                _boundingBox.Y = value;
                _originPosition.Y = _position.Y + _origin.Y;
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
        /// Gets or sets the width the object.
        /// </summary>
        public float Width
        {
            get { return _boundingBox.Width; }

            set
            {
                _boundingBox.Width = value;
                _boundingRect.Width = (int)value;
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
                _boundingRect.Height = (int)value;
            }
        }

        /// <summary>
        /// Get or sets the rotation of the object in degrees.
        /// </summary>
        public float Rotation
        {
            get { return MathHelper.ToDegrees(_rotation); }

            set
            {
                if ((value > 360) || (value < -360))
                    value %= 360;

                _rotation = MathHelper.ToRadians(value);
                _baseRotation = _rotation;
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
        public GenObject(float x = 0, float y = 0, float width = 1, float height = 1)
        {
            _position = Vector2.Zero;
            _debugDrawPosition = Vector2.Zero;
            _boundingBox = new GenAABB(x, y, width, height);
            _centerPosition = new Vector2(x + _boundingBox.HalfWidth, y + _boundingBox.HalfHeight);
            _origin = new Vector2(_boundingBox.HalfWidth, _boundingBox.HalfHeight);
            _originPosition = Vector2.Zero;

            X = x;
            Y = y;

            _boundingRect = new Rectangle(0, 0, (int)width, (int)height);
            _rotation = 0f;
            RotationSpeed = 0f;
            _moveBounds = new GenAABB(x, y, width, height);
            Immovable = false;
            Solid = true;
            Velocity = Vector2.Zero;
            OldVelocity = Vector2.Zero;
            MaxVelocity = Vector2.Zero;
            Parent = null;
            ParentMode = ParentType.None;
            ParentOffset = Vector2.Zero;
            Platform = null;
            OldPlatform = null;
        }

        /// <summary>
        /// Override this method to add additional pre-update logic.
        /// </summary>
        public override void PreUpdate()
        {
            OldVelocity = Velocity;

            // Reset the bit fields for collision flags.
            WasTouching = Touching;
            Touching = Direction.None;

            OldPlatform = Platform;
            Platform = null;
        }

        /// <summary>
        /// Override this method to add additional update logic.
        /// </summary>
        public override void Update()
        {
            if (Acceleration.X != 0)
                Velocity.X += Acceleration.X * GenG.TimeStep;
            else if (Deceleration.X != 0)
            {
                if (Velocity.X > 0)
                {
                    // In case the object is moving faster than the platform, which may happen when the platform collides, decelerate the object.
                    if ((OldPlatform == null) || (Velocity.X > OldPlatform.Velocity.X))
                        Velocity.X -= Deceleration.X * GenG.TimeStep;

                    if (Velocity.X < 0)
                        Velocity.X = 0;
                }
                else if (Velocity.X < 0)
                {
                    // In case the object is moving faster than the platform, which may happen when the platform collides, decelerate the object.
                    if ((OldPlatform == null) || (Velocity.X < OldPlatform.Velocity.X))
                        Velocity.X += Deceleration.X * GenG.TimeStep;

                    if (Velocity.X > 0)
                        Velocity.X = 0;
                }
            }

            if (Acceleration.Y != 0)
                Velocity.Y += Acceleration.Y * GenG.TimeStep;
            else if (Deceleration.Y != 0)
            {
                if (Velocity.Y > 0)
                {
                    Velocity.Y -= Deceleration.Y * GenG.TimeStep;

                    if (Velocity.Y < 0)
                        Velocity.Y = 0;
                }
                else if (Velocity.Y < 0)
                {
                    Velocity.Y += Deceleration.Y * GenG.TimeStep;

                    if (Velocity.Y > 0)
                        Velocity.Y = 0;
                }
            }

            // Limit the object's velocity to the maximum velocity.
            if (MaxVelocity.X != 0)
                Velocity.X = MathHelper.Clamp(Velocity.X, -MaxVelocity.X, MaxVelocity.X);

            if (MaxVelocity.Y != 0)
                Velocity.Y = MathHelper.Clamp(Velocity.Y, -MaxVelocity.Y, MaxVelocity.Y);

            _oldPosition = _position;

            // Move the object.
            if (Velocity.X != 0)
                X += Velocity.X * GenG.TimeStep;

            if (Velocity.Y != 0)
                Y += Velocity.Y * GenG.TimeStep;

            // Update the center position of the object.
            _centerPosition.X = _position.X + _boundingBox.HalfWidth;
            _centerPosition.Y = _position.Y + _boundingBox.HalfHeight;

            if (Path != null)
                MoveAlongPath();

            Rotation += RotationSpeed * GenG.TimeStep;
        }

        /// <summary>
        /// Override this method to add additional post-update logic.
        /// </summary>
        public override void PostUpdate()
        {
            if (Platform != null)
            {
                // Do not change the object's velocity if its velocity is already greater than the platform's velocity.
                if (((Velocity.X >= 0) && (Velocity.X <= Platform.Velocity.X)) || ((Velocity.X <= 0) && (Velocity.X >= Platform.Velocity.X)))
                {
                    if (Platform.Velocity.X != 0)
                        Velocity.X = Platform.Velocity.X + (Platform.Acceleration.X * GenG.TimeStep);
                }
            }

            // Set the transformations of the object relative to a parent object.
            if ((Parent != null) && (ParentMode != ParentType.None))
            {
                if (ParentMode == ParentType.Position)
                {
                    X = (Parent.OriginPosition.X - Origin.X) + ParentOffset.X;
                    Y = (Parent.OriginPosition.Y - Origin.Y) + ParentOffset.Y;
                }
                else
                {
                    if ((ParentMode & ParentType.Rotation) > ParentType.None)
                        Rotation = Parent.Rotation;

                    // Rotate the object around the parent's origin, using the angle and length of the parent offset vector as a base.
                    if ((ParentMode & ParentType.Origin) > ParentType.None)
                    {
                        GenMove.RotateAroundPoint(this, Parent.OriginPosition, GenMove.VectortoAngle(ParentOffset) + Parent.Rotation, ParentOffset.Length());

                        // Offset the object's position by its origin so that the origin is the point rotating around the parent's origin position.
                        X -= Origin.X;
                        Y -= Origin.Y;
                    }
                }

                // Calculate the velocity of the object based on its new position affected by the parent object.
                Velocity = (_position - _oldPosition) / GenG.TimeStep;
            }
        }

        /// <summary>
        /// Draws a box representing the collision area of the object.
        /// </summary>
        public override void DrawDebug()
        {
            if (GenG.DrawMode == GenG.DrawType.Pixel)
            {
                _debugDrawPosition.X = (int)_position.X;
                _debugDrawPosition.Y = (int)_position.Y;
            }
            else if (GenG.DrawMode == GenG.DrawType.Smooth)
            {
                _debugDrawPosition.X = _position.X;
                _debugDrawPosition.Y = _position.Y;
            }

            GenG.SpriteBatch.Draw(GenG.Pixel, _debugDrawPosition, _boundingRect, ((Immovable) ? Color.Red : Color.Lime) * 0.5f);

            //GenG.DrawLine(_positionRect.Left, _positionRect.Top, _positionRect.Right, _positionRect.Top, ((Immovable) ? Color.Red : Color.Lime) * 0.5f);
            //GenG.DrawLine(_positionRect.Right, _positionRect.Top, _positionRect.Right, _positionRect.Bottom, ((Immovable) ? Color.Red : Color.Lime) * 0.5f);
            //GenG.DrawLine(_positionRect.Left, _positionRect.Bottom - 1, _positionRect.Right, _positionRect.Bottom - 1, ((Immovable) ? Color.Red : Color.Lime) * 0.5f);
            //GenG.DrawLine(_positionRect.Left + 1, _positionRect.Top, _positionRect.Left + 1, _positionRect.Bottom, ((Immovable) ? Color.Red : Color.Lime) * 0.5f);
        }

        /// <summary>
        /// Sets the x and y position of the origin of the object relative to the object's position.
        /// </summary>
        /// <param name="x">The x position of the origin relative to the object's position.</param>
        /// <param name="y">The y position of the origin relative to the object's position.</param>
        public void SetOrigin(float x, float y)
        {
            _origin.X = x;
            _origin.Y = y;

            _originPosition = _position + _origin;
        }

        /// <summary>
        /// Places the origin at the center of the bounding box.
        /// </summary>
        public void CenterOrigin()
        {
            _origin.X = _boundingBox.HalfWidth;
            _origin.Y = _boundingBox.HalfHeight;
        }

        /// <summary>
        /// Sets a given object as the parent of this object, using the given parenting type.
        /// </summary>
        /// <param name="gameObject">The object to set as the parent object.</param>
        /// <param name="parentMode">The type of transformations to connect with the parent object.</param>
        public void SetParent(GenObject gameObject, ParentType parentMode)
        {
            Parent = gameObject;
            ParentMode = parentMode;
        }

        /// <summary>
        /// Gets a bounding box containing the object at its current and predicted positions relative to its velocity.
        /// </summary>
        /// <returns>The bounding box containing the object at its current and predicted positions relative to its velocity.</returns>
        public GenAABB GetMoveBounds()
        {
            // Get the x and y distances that the object will move relative to its velocity.
            _moveDistance.X = Velocity.X * GenG.TimeStep;
            _moveDistance.Y = Velocity.Y * GenG.TimeStep;

            // Calculate the movement bounding box.
            _moveBounds.X = Math.Min(_boundingBox.Left, _boundingBox.Left + _moveDistance.X);
            _moveBounds.Y = Math.Min(_boundingBox.Top, _boundingBox.Top + _moveDistance.Y);
            _moveBounds.Width = Math.Max(_boundingBox.Right, _boundingBox.Right + _moveDistance.X) - _moveBounds.X;
            _moveBounds.Height = Math.Max(_boundingBox.Bottom, _boundingBox.Bottom + _moveDistance.Y) - _moveBounds.Y;

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
        /// <param name="callback">The delegate method that will be invoked if an overlap occurs.</param>
        /// <returns>True if an overlap occurs, false if not.</returns>
        public bool Overlap(GenObject gameObject, CollideEvent callback = null)
        {
            // Check if this object is alive to avoid unwanted overlap checks that may be called by a quadtree.
            if (Exists && Active)
            {
                if (gameObject.Exists && gameObject.Active && GetMoveBounds().Intersects(gameObject.GetMoveBounds()))
                {
                    if (callback != null)
                        callback(new GenCollideEvent(this, gameObject, GenObject.Direction.None, GenObject.Direction.None));

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Applys collision detection and response against another object that may overlap this object.
        /// </summary>
        /// <param name="gameObject">The object to check for a collision.</param>
        /// <param name="callback">The delegate method that will be invoked if a collision occurs.</param>
        /// <param name="penetrate">Determines if the objects are able to penetrate each other for elastic collision response.</param>
        /// <param name="collidableEdges">A bit field of flags determining which edges of the given object are collidable.</param>
        /// <returns>True if a collision occurs, false if not.</returns>
        public bool Collide(GenObject gameObject, CollideEvent callback = null, bool penetrate = true, GenObject.Direction collidableEdges = GenObject.Direction.Any)
        {
            if (!this.Equals(gameObject))
            {
                // Do not check for collisions if either object is not solid, or if both objects are immovable.
                if ((!Solid || !gameObject.Solid) || (Immovable && gameObject.Immovable))
                    return false;

                // If either object is a parent of the other, do not check for a collision.
                //if ((Parent == gameObject) || (gameObject.Parent == this))
                //    return false;

                if (Overlap(gameObject))
                {
                    _distancesVector = GenU.GetDistanceAABB(_boundingBox, gameObject.BoundingBox);

                    if (_distancesVector.X > _distancesVector.Y)
                    {
                        if (_boundingBox.MidpointX > gameObject.BoundingBox.MidpointX)
                        {
                            _collisionNormalVector.X = -1;
                            _collisionNormalVector.Y = 0;
                        }
                        else
                        {
                            _collisionNormalVector.X = 1;
                            _collisionNormalVector.Y = 0;
                        }
                    }
                    else
                    {
                        if (_boundingBox.MidpointY > gameObject.BoundingBox.MidpointY)
                        {
                            _collisionNormalVector.X = 0;
                            _collisionNormalVector.Y = -1;
                        }
                        else
                        {
                            _collisionNormalVector.X = 0;
                            _collisionNormalVector.Y = 1;
                        }
                    }

                    if (((_collisionNormalVector.X == 1) && ((collidableEdges & GenObject.Direction.Left) == GenObject.Direction.Left)) ||
                        ((_collisionNormalVector.X == -1) && ((collidableEdges & GenObject.Direction.Right) == GenObject.Direction.Right)) ||
                        ((_collisionNormalVector.Y == 1) && ((collidableEdges & GenObject.Direction.Up) == GenObject.Direction.Up)) ||
                        ((_collisionNormalVector.Y == -1) && ((collidableEdges & GenObject.Direction.Down) == GenObject.Direction.Down)))
                    {
                        float distance = Math.Max(_distancesVector.X, _distancesVector.Y);
                        float remove = 0f;

                        // Apply a different collision response against tiles for pixel-perfect accuracy.
                        if (gameObject is GenTile)
                        {
                            remove = Vector2.Dot(-Velocity, _collisionNormalVector) + Math.Max(distance, 0) / GenG.TimeStep;

                            if (remove < 0)
                            {
                                if (_collisionNormalVector.X != 0)
                                {
                                    if (_collisionNormalVector.X == -1)
                                        X = gameObject.BoundingBox.Right;
                                    else
                                        X = gameObject.X - _boundingBox.Width;

                                    Velocity.X = 0;
                                }
                                else
                                {
                                    if (_collisionNormalVector.Y == -1)
                                        Y = gameObject.BoundingBox.Bottom;
                                    else
                                        Y = gameObject.Y - _boundingBox.Height;

                                    Velocity.Y = 0;
                                }
                            }
                        }
                        else
                        {
                            float relativeNormalVelocity = Vector2.Dot(gameObject.Velocity - Velocity, _collisionNormalVector);

                            if (penetrate)
                                remove = relativeNormalVelocity + distance / GenG.TimeStep;
                            else
                                remove = relativeNormalVelocity + Math.Max(distance, 0) / GenG.TimeStep;

                            if (remove < 0)
                            {
                                float impulse = remove / (Mass + gameObject.Mass);

                                if (!Immovable)
                                {
                                    Velocity += impulse * _collisionNormalVector * gameObject.Mass;

                                    if (!penetrate)
                                    {
                                        float penetration = Math.Min(distance, 0);

                                        if (_collisionNormalVector.X != 0)
                                            X += penetration * _collisionNormalVector.X;
                                        else
                                            Y += penetration * _collisionNormalVector.Y;
                                    }
                                }

                                if (!gameObject.Immovable)
                                {
                                    gameObject.Velocity -= impulse * _collisionNormalVector * Mass;

                                    if (!penetrate)
                                    {
                                        float penetration = Math.Min(distance, 0);

                                        if (_collisionNormalVector.X != 0)
                                            gameObject.X -= penetration * _collisionNormalVector.X;
                                        else
                                            gameObject.Y -= penetration * _collisionNormalVector.Y;
                                    }
                                }
                            }
                        }

                        if (remove < 0)
                        {
                            // Use a bit field of flags to provide the direction that each object is colliding in during the current collision.
                            Direction touchingA = Direction.None;
                            Direction touchingB = Direction.None;

                            if (_collisionNormalVector.X != 0)
                            {
                                if (_collisionNormalVector.X == 1)
                                {
                                    Touching |= Direction.Right;
                                    gameObject.Touching |= Direction.Left;

                                    touchingA |= Direction.Right;
                                    touchingB |= Direction.Left;
                                }
                                else
                                {
                                    Touching |= Direction.Left;
                                    gameObject.Touching |= Direction.Right;

                                    touchingA |= Direction.Left;
                                    touchingB |= Direction.Right;
                                }
                            }
                            else
                            {
                                if (_collisionNormalVector.Y == 1)
                                {
                                    Touching |= Direction.Down;
                                    gameObject.Touching |= Direction.Up;

                                    touchingA |= Direction.Down;
                                    touchingB |= Direction.Up;

                                    if (gameObject.IsPlatform)
                                    {
                                        if (Acceleration.X == 0)
                                            Platform = gameObject;
                                    }
                                }
                                else
                                {
                                    Touching |= Direction.Up;
                                    gameObject.Touching |= Direction.Down;

                                    touchingA |= Direction.Up;
                                    touchingB |= Direction.Down;

                                    if (IsPlatform)
                                    {
                                        if (gameObject.Acceleration.X == 0)
                                            gameObject.Platform = this;
                                    }
                                }
                            }

                            if (callback != null)
                                callback(new GenCollideEvent(this, gameObject, touchingA, touchingB));

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

        /// <summary>
        /// Sets the velocity and acceleration of the object to 0.
        /// </summary>
        /// <param name="stopAcceleration">A flag used to set the acceleration to 0.</param>
        public void StopMoving(bool stopAcceleration = true)
        {
            Velocity.X = 0;
            Velocity.Y = 0;

            if (stopAcceleration)
            {
                Acceleration.X = 0;
                Acceleration.Y = 0;
            }
        }

        /// <summary>
        /// Sets the path for the object to follow.
        /// </summary>
        /// <param name="path">The path to follow.</param>
        /// <param name="speed">The velocity or acceleration of the object as it moves along the path.</param>
        /// <param name="type">The path movement type.</param>
        /// <param name="axis">The allowed movement axis of the object.</param>
        /// <param name="accelerates">A flag used to set the object's acceleration or velocity as it moves along the path.</param>
        /// <returns>The path that was set.</returns>
        public GenPath SetPath(
            GenPath path,
            float speed,
            GenPath.Type type = GenPath.Type.Clockwise,
            GenMove.Axis axis = GenMove.Axis.Both,
            GenPath.Movement movement = GenPath.Movement.Instant)
        {
            Path = path;
            PathSpeed = speed;
            PathType = type;
            PathAxis = axis;
            PathMovement = movement;

            // Set the initial path node index relative to the path movement type.
            switch (PathType)
            {
                case GenPath.Type.Counterclockwise:
                    PathNodeIndex = Path.Nodes.Count - 1;
                    break;
                case GenPath.Type.Random:
                    PathNodeIndex = GenU.Random(0, Path.Nodes.Count);
                    break;
                default:
                    PathNodeIndex = 0;
                    break;
            }

            return path;
        }

        /// <summary>
        /// Moves the object along a path relative to the path movement type.
        /// </summary>
        public void MoveAlongPath()
        {
            if (PathSpeed != 0)
            {
                // Get the next movement node in the path if the object reaches the current node.
                if (GenMove.CanReachPoint(this, Path.Nodes[PathNodeIndex].Position, PathSpeed, Path.Nodes[PathNodeIndex].Radius, PathAxis))
                {
                    if (Path.Nodes[PathNodeIndex].Callback != null)
                        Path.Nodes[PathNodeIndex].Callback.Invoke();

                    // Check if the path is null in case the callback function set the path to null.
                    if (Path != null)
                    {
                        switch (PathType)
                        {
                            case GenPath.Type.Clockwise:
                                {
                                    if (PathNodeIndex < (Path.Nodes.Count - 1))
                                        PathNodeIndex++;
                                    else
                                        PathNodeIndex = 0;

                                    break;
                                }
                            case GenPath.Type.Counterclockwise:
                                {
                                    if (PathNodeIndex > 0)
                                        PathNodeIndex--;
                                    else
                                        PathNodeIndex = Path.Nodes.Count - 1;

                                    break;
                                }
                            case GenPath.Type.Random:
                                {
                                    PathNodeIndex = GenU.Random(0, Path.Nodes.Count);

                                    break;
                                }
                            case GenPath.Type.Yoyo:
                                {
                                    if (PathDirection > 0) // Move along the path clockwise.
                                    {
                                        // If the object has not reached the end of the path, increment the path node index.
                                        // Otherwise, reverse the path movement direction and decrement the path node index.
                                        if (PathNodeIndex < (Path.Nodes.Count - 1))
                                            PathNodeIndex++;
                                        else
                                        {
                                            PathDirection = -1;
                                            PathNodeIndex--;
                                        }
                                    }
                                    else if (PathDirection < 0) // Move along the path counterclockwise.
                                    {
                                        // If the object has not reached the beginning of the path, decrement the path node index.
                                        // Otherwise, reverse the path movement direction and increment the path node index.
                                        if (PathNodeIndex > 0)
                                            PathNodeIndex--;
                                        else
                                        {
                                            PathDirection = 1;
                                            PathNodeIndex++;
                                        }
                                    }

                                    break;
                                }
                        }
                    }
                }

                // Check if the path is null in case the callback function set the path to null.
                if (Path != null)
                {
                    // Move the object to the next node.
                    switch (PathMovement)
                    {
                        case GenPath.Movement.Instant:
                            GenMove.MoveToPoint(this, Path.Nodes[PathNodeIndex].Position, PathSpeed, PathAxis, true);
                            break;
                        case GenPath.Movement.Accelerates:
                            GenMove.AccelerateToPoint(this, Path.Nodes[PathNodeIndex].Position, PathSpeed, 0, PathAxis, true);
                            break;
                    }
                }
            }
        }
    }
}