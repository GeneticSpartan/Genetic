using System;

using Microsoft.Xna.Framework;

using Genetic.Geometry;
using Genetic.Path;
using Genetic.Physics;

namespace Genetic
{
    /// <summary>
    /// A base game object.
    /// Provides a size, origin, rotation, and basic physics movement.
    /// A <c>GenObject</c> can be parented to another <c>GenObject</c> to inherit its position or rotation.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenObject : GenBasic
    {
        /// <summary>
        /// A bit field flags representing up, down, left, and right directions.
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// a bit field representing no direction.
            /// </summary>
            None = 0,

            /// <summary>
            /// a bit field representing the left direction.
            /// </summary>
            Left = 0x0001,

            /// <summary>
            /// a bit field representing the right direction.
            /// </summary>
            Right = 0x0010,

            /// <summary>
            /// a bit field representing the up direction.
            /// </summary>
            Up = 0x0100,

            /// <summary>
            /// a bit field representing the down direction.
            /// </summary>
            Down = 0x1000,

            /// <summary>
            /// a bit field representing every direction.
            /// </summary>
            Any = Direction.Left | Direction.Right | Direction.Up | Direction.Down
        }

        /// <summary>
        /// A bit field of flags determining the type of transformations that can be connected with a parent object.
        /// </summary>
        public enum ParentType
        {
            /// <summary>
            /// The object's transformations will not be affected by a parent.
            /// </summary>
            None = 0,

            /// <summary>
            /// The object will move relative to its origin and the parent's origin position.
            /// </summary>
            Position = 0x001,

            /// <summary>
            /// The object's rotation will change relative to a parent's rotation.
            /// </summary>
            Rotation = 0x010,

            /// <summary>
            /// The object will move and rotate around the origin point of a parent, relative to the parent's rotation.
            /// The object's rotation will not change.
            /// </summary>
            Origin = 0x100,

            /// <summary>
            /// The object will move and rotate around the origin point of a parent, relative to the parent's rotation.
            /// The object's rotation will change relative to the parent's rotation.
            /// </summary>
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
        /// A flag used to determine if the object has moved since the last update by comparing its current and previous positions.
        /// </summary>
        protected bool _hasMoved;

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
        protected GenAABB _bounds;

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
        /// Useful for calculating the movement bounding box of the object.
        /// </summary>
        protected Vector2 _moveDistance;

        /// <summary>
        /// A basic bounding box containing the object at its current and predicted positions relative to its velocity.
        /// </summary>
        protected GenAABBBasic _moveBounds;

        /// <summary>
        /// A flag used to determine if the object is affected by collisions.
        /// </summary>
        public bool Immovable;

        /// <summary>
        /// A flag used to determine if the object can collide.
        /// </summary>
        public bool Solid;

        /// <summary>
        /// The mass of the object.
        /// </summary>
        protected float _mass;

        /// <summary>
        /// The inverse of the object's mass used when calculating collision response against another object.
        /// </summary>
        protected float _inverseMass;

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
        public Vector2 Acceleration;

        /// <summary>
        /// The x and y deceleration of the object.
        /// </summary>
        public Vector2 Deceleration;

        /// <summary>
        /// The maximum x and y velocities of the object.
        /// </summary>
        public Vector2 MaxVelocity;

        /// <summary>
        /// The direction that the object is facing.
        /// </summary>
        protected Direction _facing;

        /// <summary>
        /// A bit field of flags giving the directions that the object is colliding in during the previous update.
        /// </summary>
        public Direction OldTouching;

        /// <summary>
        /// A bit field of flags giving the current directions that the object is colliding in.
        /// </summary>
        public Direction Touching;

        /// <summary>
        /// The <c>GenObject</c> that this <c>GenObject</c> will be parented to.
        /// </summary>
        public GenObject Parent;

        /// <summary>
        /// The type of transformations that are connected with a parent object.
        /// </summary>
        public ParentType ParentMode;

        /// <summary>
        /// The x and y position offsets relative to the parent object's origin position.
        /// </summary>
        public Vector2 ParentOffset;

        /// <summary>
        /// A flag used to allow other colliding objects to inherit the x velocity when sitting on this object.
        /// The colliding object will move horizontally along with this object.
        /// </summary>
        public bool IsPlatform;

        /// <summary>
        /// The current platform object that the object is interacting with.
        /// </summary>
        public GenObject Platform;

        /// <summary>
        /// The platform object that the object is interacting with during the previous update.
        /// </summary>
        public GenObject OldPlatform;

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
        /// Gets if the object has moved since the last update by comparing its current and previous positions.
        /// </summary>
        public bool HasMoved
        {
            get { return _hasMoved; }
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
                _bounds.X = value;
                _centerPosition.X = _position.X + _bounds.HalfWidth;
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
                _bounds.Y = value;
                _centerPosition.Y = _position.Y + _bounds.HalfHeight;
                _originPosition.Y = _position.Y + _origin.Y;
            }
        }

        /// <summary>
        /// Gets the bounding box of the object relative to the position.
        /// </summary>
        public GenAABB Bounds
        {
            get { return _bounds; }
        }

        /// <summary>
        /// Gets or sets the width the object.
        /// </summary>
        public float Width
        {
            get { return _bounds.Width; }

            set
            {
                _bounds.Width = value;
                _boundingRect.Width = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the height the object.
        /// </summary>
        public float Height
        {
            get { return _bounds.Height; }

            set
            {
                _bounds.Height = value;
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
        /// Gets the bounding box surrounding the object at its current and predicted positions relative to its velocity.
        /// Useful for checking if the object may collide with another object during the next update.
        /// </summary>
        public GenAABBBasic MoveBounds
        {
            get { return _moveBounds; }
        }

        /// <summary>
        /// Gets or sets the mass of the object.
        /// Setting this property will also set the inverse mass value used when calculating collision response against another object.
        /// </summary>
        public float Mass
        {
            get { return _mass; }

            set
            {
                _mass = Math.Max(0f, value);
                _inverseMass = (_mass == 0f) ? 0f : 1f / _mass;
            }
        }

        /// <summary>
        /// Gets the inverse of the object's mass used when calculating collision response against another object.
        /// Set the object's <c>Mass</c> property to set the inverse mass.
        /// </summary>
        public float InverseMass
        {
            get { return _inverseMass; }
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
            _bounds = new GenAABB(x, y, width, height);
            _centerPosition = new Vector2(x + _bounds.HalfWidth, y + _bounds.HalfHeight);
            _origin = new Vector2(_bounds.HalfWidth, _bounds.HalfHeight);

            X = x;
            Y = y;

            _boundingRect = new Rectangle(0, 0, (int)width, (int)height);
            _rotation = 0f;
            RotationSpeed = 0f;
            _moveBounds = new GenAABBBasic(x, y, width, height);
            Immovable = false;
            Solid = true;
            Mass = 1f;
            _facing = Direction.None;
            OldTouching = Direction.None;
            Touching = Direction.None;
            Parent = null;
            ParentMode = ParentType.None;
            IsPlatform = false;
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
            OldTouching = Touching;
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
                    Velocity.Y = Math.Max(Velocity.Y - (Deceleration.Y * GenG.TimeStep), 0f);
                else if (Velocity.Y < 0)
                    Velocity.Y = Math.Min(Velocity.Y + (Deceleration.Y * GenG.TimeStep), 0f);
            }

            // Limit the object's velocity to the maximum velocity.
            if (MaxVelocity.X != 0)
                Velocity.X = MathHelper.Clamp(Velocity.X, -MaxVelocity.X, MaxVelocity.X);

            if (MaxVelocity.Y != 0)
                Velocity.Y = MathHelper.Clamp(Velocity.Y, -MaxVelocity.Y, MaxVelocity.Y);

            // Assign the position of the object during the previous update before moving the object based on its current velocity.
            _oldPosition = _position;

            // Get the x and y distances that the object will move relative to its velocity.
            // Update the x and y positions of the object relative to the move distances.
            if (Velocity.X != 0)
                X += _moveDistance.X = Velocity.X * GenG.TimeStep;

            if (Velocity.Y != 0)
                Y += _moveDistance.Y = Velocity.Y * GenG.TimeStep;

            // Check if the object has moved since the last update.
            _hasMoved = (_position == _oldPosition) ? false : true;

            if (Path != null)
                MoveAlongPath();

            // Calculate the movement bounding box if the object has moved.
            // Otherwise set the movement bounds to the object's current bounding box.
            if (_hasMoved)
            {
                _moveBounds.X = Math.Min(_bounds.Left, _bounds.Left + _moveDistance.X);
                _moveBounds.Y = Math.Min(_bounds.Top, _bounds.Top + _moveDistance.Y);
                _moveBounds.Width = Math.Max(_bounds.Right, _bounds.Right + _moveDistance.X) - _moveBounds.X;
                _moveBounds.Height = Math.Max(_bounds.Bottom, _bounds.Bottom + _moveDistance.Y) - _moveBounds.Y;
            }
            else
            {
                _moveBounds.X = _bounds.Left;
                _moveBounds.Y = _bounds.Top;
                _moveBounds.Width = _bounds.Right;
                _moveBounds.Height = _bounds.Bottom;
            }

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
                    if ((ParentMode & ParentType.Rotation) == ParentType.Rotation)
                        Rotation = Parent.Rotation;

                    // Rotate the object around the parent's origin, using the angle and length of the parent offset vector as a base.
                    if ((ParentMode & ParentType.Origin) == ParentType.Origin)
                    {
                        GenMove.RotateAroundPoint(this, Parent.OriginPosition, GenMove.VectortoAngle(ParentOffset) + Parent.Rotation, ParentOffset.Length());

                        // Offset the object's position by its origin so that the origin is the point rotating around the parent's origin position.
                        X -= Origin.X;
                        Y -= Origin.Y;
                    }
                }

                // Calculate the velocity of the object based on its new position affected by the parent object.
                Velocity = (_position - _oldPosition) * GenG.InverseTimeStep;
            }
        }

        /// <summary>
        /// Draws a box representing the collision area of the object.
        /// </summary>
        public override void DrawDebug()
        {
            // If the bounding box does not intersect with the camera's view, do not draw the debug object.
            if (!_bounds.Intersects(GenG.CurrentCamera.CameraView))
                return;

            _debugDrawPosition = _position;

            if (GenG.DrawMode == GenG.DrawType.Pixel)
            {
                _debugDrawPosition.X = (int)_debugDrawPosition.X;
                _debugDrawPosition.Y = (int)_debugDrawPosition.Y;
            }

            GenG.SpriteBatch.Draw(GenG.Pixel, _debugDrawPosition, _boundingRect, ((Immovable) ? Color.Red : Color.Lime) * 0.5f);
        }

        /// <summary>
        /// Sets the x and y positions of the origin of the object relative to the object's position.
        /// </summary>
        /// <param name="x">The x position of the origin relative to the object's position.</param>
        /// <param name="y">The y position of the origin relative to the object's position.</param>
        public void SetOrigin(float x, float y)
        {
            _origin.X = x;
            _origin.Y = y;

            // Update the origin position relative to the object's position.
            _originPosition = _position + _origin;
        }

        /// <summary>
        /// Places the origin at the center of the object's bounding box.
        /// </summary>
        public void CenterOrigin()
        {
            SetOrigin(_bounds.HalfWidth, _bounds.HalfHeight);
        }

        /// <summary>
        /// Sets a given object as the parent of this object, using the specified parenting type.
        /// </summary>
        /// <param name="gameObject">The object to set as the parent object.</param>
        /// <param name="parentMode">The type of transformations to connect with the parent object.</param>
        public void SetParent(GenObject gameObject, ParentType parentMode)
        {
            Parent = gameObject;
            ParentMode = parentMode;
        }

        /// <summary>
        /// Checks if the object is colliding in the given direction.
        /// </summary>
        /// <param name="direction">The direction to check for collision.</param>
        /// <returns>True if there is a collision in the given direction, false if not.</returns>
        public bool IsTouched(Direction direction)
        {
            return (Touching & direction) == direction;
        }

        /// <summary>
        /// Checks if the object was colliding in the given direction during the previous update.
        /// </summary>
        /// <param name="direction">The direction to check for collision.</param>
        /// <returns>True if there was a collision in the given direction, false if not.</returns>
        public bool WasTouched(Direction direction)
        {
            return (OldTouching & direction) == direction;
        }

        /// <summary>
        /// Checks if the object just collided in the given direction.
        /// </summary>
        /// <param name="direction">The direction to check for collision.</param>
        /// <returns>True if there is a collision in the given direction, false if not.</returns>
        public bool JustTouched(Direction direction)
        {
            return (WasTouched(Direction.None) && IsTouched(direction));
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
        /// <param name="movement">Determines whether to set the object's velocity or acceleration as it moves along the path.</param>
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
        /// Moves the object along a path according to the path movement type.
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

                    // Check if the path is null in case the callback method set the path to null.
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

                // Check if the path is null in case the callback method set the path to null.
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

        public void RefreshMoveBounds()
        {
            // Get the x and y distances that the object will move relative to its velocity.
            _moveDistance.X = Velocity.X * GenG.TimeStep;
            _moveDistance.Y = Velocity.Y * GenG.TimeStep;

            // Calculate the movement bounding box.
            _moveBounds.X = Math.Min(_bounds.Left, _bounds.Left + _moveDistance.X);
            _moveBounds.Y = Math.Min(_bounds.Top, _bounds.Top + _moveDistance.Y);
            _moveBounds.Width = Math.Max(_bounds.Right, _bounds.Right + _moveDistance.X) - _moveBounds.X;
            _moveBounds.Height = Math.Max(_bounds.Bottom, _bounds.Bottom + _moveDistance.Y) - _moveBounds.Y;
        }
    }
}