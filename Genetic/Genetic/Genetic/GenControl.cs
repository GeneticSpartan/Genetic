using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Genetic.Input;
using Genetic.Sound;

namespace Genetic
{
    /// <summary>
    /// A manager for various input control schemes to control the movement of a game object.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenControl : GenBasic
    {
        /// <summary>
        /// The type that determines how a control object's movement is controlled.
        /// </summary>
        public enum ControlType
        {
            /// <summary>
            /// A control type common to top-down games.
            /// Uses movement along both the x-axis and y-axis.
            /// </summary>
            TopDown,
            
            /// <summary>
            /// A control type common to platform games.
            /// Uses movement along the x-axis, and gravity to affect the object's position along the y-axis.
            /// An optional jumping ability is also available.
            /// </summary>
            Platformer
        }

        /// <summary>
        /// Determines how acceleration or velocity should be handled as an object moves.
        /// </summary>
        public enum Movement
        {
            /// <summary>
            /// Velocity is set directly to instantly move an object at a given speed.
            /// </summary>
            Instant,

            /// <summary>
            /// Acceleration is set to gradually move an object as acceleration is added to its velocity.
            /// </summary>
            Accelerates
        }

        /// <summary>
        /// Determines how acceleration or velocity should be handled as an object stops.
        /// </summary>
        public enum Stopping
        {
            /// <summary>
            /// Velocity is set to 0 to instantly stop an object.
            /// </summary>
            Instant,

            /// <summary>
            /// Acceleration is set to 0 to allow deceleration to be subtracted from an object's velocity to gradually stop the object.
            /// </summary>
            Decelerates
        }

        /// <summary>
        /// The movement state of a control object.
        /// </summary>
        protected enum State
        {
            /// <summary>
            /// An object is not moving.
            /// </summary>
            Idle,

            /// <summary>
            /// An object is being controlled to move in a specific direction.
            /// </summary>
            Moving,

            /// <summary>
            /// An object is jumping after being controlled to jump.
            /// </summary>
            Jumping,

            /// <summary>
            /// An object is falling after reaching the highest point of a jump, or moving off of another object.
            /// </summary>
            Falling
        }

        /// <summary>
        /// The object that is controlled.
        /// </summary>
        public GenObject ControlObject;

        /// <summary>
        /// The type of control available to the control object.
        /// </summary>
        public ControlType ControlMode;

        /// <summary>
        /// Determines how acceleration or velocity should be handled as the object moves.
        /// </summary>
        public Movement MovementType;

        /// <summary>
        /// Determines how acceleration or velocity should be handled as the object stops.
        /// </summary>
        public Stopping StoppingType;

        /// <summary>
        /// The index number of the player controlling the object.
        /// </summary>
        public PlayerIndex PlayerIndex;

        /// <summary>
        /// The keyboard controls for movement direction and jumping.
        /// </summary>
        protected Keys[] _keyboardControls;

        /// <summary>
        /// The game pad controls for movement direction and jumping.
        /// </summary>
        protected Buttons[] _gamePadControls;

        /// <summary>
        /// A special-case control scheme to use the variable values available to gamepad controls to move at variable speeds.
        /// </summary>
        public GenGamePad.ButtonsSpecial ButtonsSpecial;

        /// <summary>
        /// A flag that determines if user input should be checked.
        /// Useful for creating an AI-controlled object.
        /// </summary>
        public bool UseInput;
#if WINDOWS
        /// <summary>
        /// A flag used to determine if keyboard input should be checked.
        /// </summary>
        public bool UseKeyboard;
#endif
        /// <summary>
        /// A flag used to determine if game pad input should be checked.
        /// </summary>
        public bool UseGamePad;

        /// <summary>
        /// The speed of the object moving or accelerating horizontally.
        /// The speed determines the object's acceleration or velocity depending on the movement type.
        /// </summary>
        public uint MovementSpeedX;

        /// <summary>
        /// The speed of the object moving or accelerating vertically.
        /// The speed determines the object's acceleration or velocity depending on the movement type.
        /// </summary>
        public uint MovementSpeedY;

        /// <summary>
        /// The vertical speed of the object used during a jump.
        /// </summary>
        public int JumpSpeed;

        /// <summary>
        /// A flag used to determine if the jump speed will be calculated against the control object's current velocity when jumping.
        /// </summary>
        public bool JumpInheritVelocity;

        /// <summary>
        /// The allowed number of consecutive jumps.
        /// A value of 0 means unlimited jumps.
        /// </summary>
        public int JumpCount;

        /// <summary>
        /// The number of consecutive jumps that have been made since the object first jumps off of an object.
        /// Moving off of an object also counts as the first jump.
        /// </summary>
        protected int _jumpCounter;

        /// <summary>
        /// How much horizontal and vertical gravitational force affects the object.
        /// </summary>
        public Vector2 Gravity;

        /// <summary>
        /// A flag used to determine when the object is in the air and not touching the ground.
        /// </summary>
        protected bool _inAir;

        /// <summary>
        /// The current state of the control object, either idle, moving, jumping, or falling.
        /// </summary>
        protected State _state;

        /// <summary>
        /// A bit field of flags determining the current movement directions of the control object.
        /// </summary>
        protected GenObject.Direction MoveState;

        /// <summary>
        /// The name associated with the idle animation, and will be used to play it.
        /// Useful for displaying an animation when the object is not moving.
        /// A value of null will not display an animation.
        /// </summary>
        public string IdleAnimation;

        /// <summary>
        /// The name associated with the movement animation, and will be used to play it.
        /// Useful for displaying a walking/running animation.
        /// A value of null will not display an animation.
        /// </summary>
        public string MoveAnimation;

        /// <summary>
        /// The name associated with the jump animation, and will be used to play it.
        /// Useful for displaying an animation when jumping is activated.
        /// A value of null will not display an animation.
        /// </summary>
        public string JumpAnimation;

        /// <summary>
        /// The name associated with the fall animation, and will be used to play it.
        /// Useful for displaying an animation when the object falls.
        /// A value of null will not display an animation.
        /// </summary>
        public string FallAnimation;

        /// <summary>
        /// A flag used to determine if the frames per second of the move animation changes relative to the velocity of the control object.
        /// Useful for changing the speed of a running animation as the control object moves faster or slower.
        /// </summary>
        public bool UseSpeedAnimation;

        /// <summary>
        /// The minimum frames per second of the move animation when UseSpeedAnimation is true.
        /// </summary>
        public float MinAnimationFps;

        /// <summary>
        /// The maximum frames per second of the move animation when UseSpeedAnimation is true.
        /// </summary>
        public float MaxAnimationFps;

        /// <summary>
        /// The method that will be invoked when the control object jumps.
        /// </summary>
        public Action JumpCallback;

        /// <summary>
        /// The method that will be invoked when the control object lands on another object.
        /// </summary>
        public Action LandCallback;

        /// <summary>
        /// Gets the number of consecutive jumps that have been made since the object first jumps off of an object.
        /// Moving off of an object also counts as the first jump.
        /// </summary>
        public int JumpCounter
        {
            get { return _jumpCounter; }
        }

        /// <summary>
        /// Sets up a control scheme for moving a given object.
        /// </summary>
        /// <param name="controlObject">The object that is controlled.</param>
        /// <param name="controlMode">The type of control available to the control object.</param>
        /// <param name="movementType">How acceleration or velocity should be handled as the object moves.</param>
        /// <param name="stoppingType">How acceleration or velocity should be handled as the object stops.</param>
        /// <param name="playerIndex">The index number of the player controlling the object.</param>
        public GenControl(GenObject controlObject, ControlType controlMode = ControlType.TopDown, Movement movementType = Movement.Instant, Stopping stoppingType = Stopping.Instant, PlayerIndex playerIndex = PlayerIndex.One)
        {
            ControlMode = controlMode;
            ControlObject = controlObject;
            MovementType = movementType;
            StoppingType = stoppingType;
            PlayerIndex = playerIndex;
            _keyboardControls = new Keys[5];
            _gamePadControls = new Buttons[5];
            ButtonsSpecial = GenGamePad.ButtonsSpecial.None;
            UseInput = true;
#if WINDOWS
            UseKeyboard = true;
#endif
            UseGamePad = true;
            MovementSpeedX = 0;
            MovementSpeedY = 0;
            JumpSpeed = 0;
            JumpInheritVelocity = false;
            JumpCount = 1;

            // Start the jump counter at 1, since the control object spawns in the air initially.
            _jumpCounter = 1;

            Gravity = Vector2.Zero;
            _inAir = true;_state = State.Idle;
            MoveState = GenObject.Direction.None;
            IdleAnimation = null;
            MoveAnimation = null;
            JumpAnimation = null;
            FallAnimation = null;
            UseSpeedAnimation = false;
            MinAnimationFps = 0f;
            MaxAnimationFps = 12f;
            JumpCallback = null;
            LandCallback = null;

            // Set the default movement direction keyboard controls.
            SetDirectionControls(Keys.Left, Keys.Right, Keys.Up, Keys.Down);

            // Set the default jumping keyboard control.
            SetJumpControl(Keys.Space);

            // Set the default movement direction game pad controls.
            SetDirectionControls(Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown);

            // Set the default jumping game pad control.
            SetJumpControl(Buttons.A);
        }

        /// <summary>
        /// Clears the movement directions of the control object to prepare for the next update.
        /// </summary>
        public override void PreUpdate()
        {
            // Clear the bit field for the control object movement directions.
            MoveState = GenObject.Direction.None;
        }

        /// <summary>
        /// Moves the control object based on the specified control settings and current input.
        /// </summary>
        public override void Update()
        {
            // Reset the x and y accelerations to 0 to allow new acceleration values to be calculated.
            ControlObject.Acceleration.X = 0;
            ControlObject.Acceleration.Y = 0;

            if (UseInput)
            {
                if (MovementSpeedX != 0)
                {
#if WINDOWS
                    if (UseKeyboard)
                    {
                        if (UseKeyboard && GenG.Keyboards[(int)PlayerIndex].IsPressed(_keyboardControls[0]))
                            MoveX(-1f);
                        else if (UseKeyboard && GenG.Keyboards[(int)PlayerIndex].IsPressed(_keyboardControls[1]))
                            MoveX(1f);
                    }
#endif
                    if (UseGamePad)
                    {
                        if (ButtonsSpecial != GenGamePad.ButtonsSpecial.None)
                        {
                            if (ButtonsSpecial == GenGamePad.ButtonsSpecial.ThumbStickLeft)
                            {
                                if (GenG.GamePads[(int)PlayerIndex].ThumbStickLeftX != 0)
                                    MoveX(GenG.GamePads[(int)PlayerIndex].ThumbStickLeftX);
                            }
                            else if (ButtonsSpecial == GenGamePad.ButtonsSpecial.ThumbStickRight)
                            {
                                if (GenG.GamePads[(int)PlayerIndex].ThumbStickRightX != 0)
                                    MoveX(GenG.GamePads[(int)PlayerIndex].ThumbStickRightX);
                            }
                        }
                        else if (GenG.GamePads[(int)PlayerIndex].IsPressed(_gamePadControls[0]))
                            MoveX(-1f);
                        else if (GenG.GamePads[(int)PlayerIndex].IsPressed(_gamePadControls[1]))
                            MoveX(1f);
                    }
                }

                if (MovementSpeedY != 0)
                {
#if WINDOWS
                    if (UseKeyboard)
                    {
                        if (UseKeyboard && GenG.Keyboards[(int)PlayerIndex].IsPressed(_keyboardControls[2]))
                            MoveY(-1f);
                        else if (UseKeyboard && GenG.Keyboards[(int)PlayerIndex].IsPressed(_keyboardControls[3]))
                            MoveY(1f);
                    }
#endif
                    if (UseGamePad)
                    {
                        if (ButtonsSpecial != GenGamePad.ButtonsSpecial.None)
                        {
                            if (ButtonsSpecial == GenGamePad.ButtonsSpecial.ThumbStickLeft)
                            {
                                if (GenG.GamePads[(int)PlayerIndex].ThumbStickLeftY != 0)
                                    MoveY(GenG.GamePads[(int)PlayerIndex].ThumbStickLeftY);
                            }
                            else if (ButtonsSpecial == GenGamePad.ButtonsSpecial.ThumbStickRight)
                            {
                                if (GenG.GamePads[(int)PlayerIndex].ThumbStickRightY != 0)
                                    MoveY(GenG.GamePads[(int)PlayerIndex].ThumbStickRightY);
                            }
                        }
                        else if (GenG.GamePads[(int)PlayerIndex].IsPressed(_gamePadControls[2]))
                            MoveY(-1f);
                        else if (GenG.GamePads[(int)PlayerIndex].IsPressed(_gamePadControls[3]))
                            MoveY(1f);
                    }
                }
#if WINDOWS
                if (UseKeyboard && GenG.Keyboards[(int)PlayerIndex].JustPressed(_keyboardControls[4]))
                    Jump();
#endif
                if (UseGamePad && GenG.GamePads[(int)PlayerIndex].JustPressed(_gamePadControls[4]))
                    Jump();
            }

            if (!IsMovingX())
                StopX();

            if (!IsMovingY())
                StopY();

            if (ControlMode == ControlType.Platformer)
            {
                if (_inAir && (ControlObject.Velocity.Y > 0))
                    SetState(State.Falling);
            }

            // If the object is touching the ground and not being controlled to move, set its state to idle.
            if ((ControlMode == ControlType.TopDown) || !_inAir)
            {
                if (MovementSpeedX != 0 && MovementSpeedY != 0)
                {
                    if (!IsMovingX() && !IsMovingY())
                        SetState(State.Idle);
                }
                else if (MovementSpeedX != 0)
                {
                    if (!IsMovingX())
                        SetState(State.Idle);
                }
                else if (MovementSpeedY != 0)
                {
                    if (!IsMovingY())
                        SetState(State.Idle);
                }
            }
            
            ControlObject.Acceleration.X += Gravity.X;
            ControlObject.Acceleration.Y += Gravity.Y;

            UpdateSpeedAnimation();
        }

        /// <summary>
        /// Checks the "in air" state of the control object, and manages the jump counter.
        /// </summary>
        public override void PostUpdate()
        {
            // Check if the control object just landed on top of another object.
            // If not, check if the control object moved off of another object.
            if (ControlMode == ControlType.Platformer)
            {
                if (ControlObject.IsTouched(GenObject.Direction.Down) && _inAir)
                {
                    _inAir = false;

                    // Reset the jump counter.
                    _jumpCounter = 0;

                    if (LandCallback != null)
                        LandCallback.Invoke();
                }
                else if (!ControlObject.IsTouched(GenObject.Direction.Down) && !_inAir)
                {
                    _inAir = true;

                    // Increment the jump counter if the control object moved off of an object.
                    if (_jumpCounter == 0)
                        _jumpCounter++;
                }
            }
        }

        /// <summary>
        /// Sets the keyboard controls associated with each movement direction.
        /// </summary>
        /// <param name="leftControl">The keyboard control for moving left.</param>
        /// <param name="rightControl">The keyboard control for moving right.</param>
        /// <param name="upControl">The keyboard control for moving up.</param>
        /// <param name="downControl">The keyboard control for moving down.</param>
        public void SetDirectionControls(Keys leftControl, Keys rightControl, Keys upControl, Keys downControl)
        {
            _keyboardControls[0] = leftControl;
            _keyboardControls[1] = rightControl;
            _keyboardControls[2] = upControl;
            _keyboardControls[3] = downControl;
        }

        /// <summary>
        /// Sets the game pad controls associated with each movement direction.
        /// </summary>
        /// <param name="leftControl">The game pad control for moving left.</param>
        /// <param name="rightControl">The game pad control for moving right.</param>
        /// <param name="upControl">The game pad control for moving up.</param>
        /// <param name="downControl">The game pad control for moving down.</param>
        public void SetDirectionControls(Buttons leftControl, Buttons rightControl, Buttons upControl, Buttons downControl)
        {
            _gamePadControls[0] = leftControl;
            _gamePadControls[1] = rightControl;
            _gamePadControls[2] = upControl;
            _gamePadControls[3] = downControl;
        }

        /// <summary>
        /// Sets the keyboard control associated with jumping.
        /// </summary>
        /// <param name="jumpControl">The keyboard control for jumping.</param>
        public void SetJumpControl(Keys jumpControl)
        {
            _keyboardControls[4] = jumpControl;
        }

        /// <summary>
        /// Sets the game pad control associated with jumping.
        /// </summary>
        /// <param name="jumpControl">The game pad control for jumping.</param>
        public void SetJumpControl(Buttons jumpControl)
        {
            _gamePadControls[4] = jumpControl;
        }

        /// <summary>
        /// Sets the speeds of the object moving or accelerating in each direction.
        /// </summary>
        /// <param name="xSpeed">The speed of the object moving or accelerating horizontally.</param>
        /// <param name="ySpeed">The speed of the object moving or accelerating vertically.</param>
        /// <param name="xSpeedMax">The maximum speed of the object moving horizontally.</param>
        /// <param name="ySpeedMax">The maximum speed of the object moving vertically.</param>
        /// <param name="xDeceleration">The deceleration of the object horizontally.</param>
        /// <param name="yDeceleration">The deceleration of the object vertically.</param>
        public void SetMovementSpeed(uint xSpeed, uint ySpeed, uint xSpeedMax = 0, uint ySpeedMax = 0, uint xDeceleration = 0, uint yDeceleration = 0)
        {
            MovementSpeedX = xSpeed;
            MovementSpeedY = ySpeed;

            ControlObject.MaxVelocity.X = xSpeedMax;
            ControlObject.MaxVelocity.Y = ySpeedMax;

            ControlObject.Deceleration.X = xDeceleration;
            ControlObject.Deceleration.Y = yDeceleration;
        }

        /// <summary>
        /// Moves the control object along the x-axis.
        /// </summary>
        /// <param name="speedFactor">The factor determining the amount of the x movement speed to use, a value from -1.0 (left) to 1.0 (right).</param>
        public void MoveX(float speedFactor)
        {
            if (MovementType == Movement.Instant)
                ControlObject.Velocity.X = MathHelper.Clamp(MovementSpeedX, 0f, ControlObject.MaxVelocity.X) * speedFactor;
            else if (MovementType == Movement.Accelerates)
            {
                // If the control object is being moved in the opposite direction it is currently moving, set its x velocity to 0 to prevent delayed movement from acceleration.
                if (((ControlObject.Velocity.X < 0) && (speedFactor > 0)) || ((ControlObject.Velocity.X > 0) && (speedFactor < 0)))
                    ControlObject.Velocity.X = 0f;

                ControlObject.Acceleration.X = MovementSpeedX * speedFactor;
            }

            if (speedFactor < 0)
            {
                MoveState |= GenObject.Direction.Left;
                (ControlObject as GenSprite).Facing = GenObject.Direction.Left;
            }
            else if (speedFactor > 0)
            {
                MoveState |= GenObject.Direction.Right;
                (ControlObject as GenSprite).Facing = GenObject.Direction.Right;
            }

            if ((ControlMode == ControlType.TopDown) || !_inAir)
                SetState(State.Moving);
        }

        /// <summary>
        /// Stops the control object moving on the x-axis.
        /// </summary>
        public void StopX()
        {
            if (StoppingType == Stopping.Instant)
                ControlObject.Velocity.X = 0f;
            else if (StoppingType == Stopping.Decelerates)
                ControlObject.Acceleration.X = 0f;
        }

        /// <summary>
        /// Moves the control object along the y-axis.
        /// </summary>
        /// <param name="speedFactor">The factor determining the amount of the y movement speed to use, a value from -1.0 (up) to 1.0 (down).</param>
        public void MoveY(float speedFactor)
        {
            if (MovementType == Movement.Instant)
                ControlObject.Velocity.Y = MathHelper.Clamp(MovementSpeedY, 0f, ControlObject.MaxVelocity.Y) * speedFactor;
            else if (MovementType == Movement.Accelerates)
                ControlObject.Acceleration.Y = MovementSpeedY * speedFactor;

            if (speedFactor < 0)
                MoveState |= GenObject.Direction.Up;
            else if (speedFactor > 0)
                MoveState |= GenObject.Direction.Down;

            SetState(State.Moving);
        }

        /// <summary>
        /// Stops the control object moving on the y-axis.
        /// </summary>
        public void StopY()
        {
            if (StoppingType == Stopping.Instant)
                ControlObject.Velocity.Y = 0;
            else if (StoppingType == Stopping.Decelerates)
                ControlObject.Acceleration.Y = 0;
        }

        /// <summary>
        /// Initiates a jump for the control object.
        /// </summary>
        public void Jump()
        {
            if (!_inAir || (_jumpCounter < JumpCount) || (JumpCount == 0))
            {
                _jumpCounter++;

                if (JumpInheritVelocity)
                    ControlObject.Velocity.Y -= JumpSpeed;
                else
                    ControlObject.Velocity.Y = -JumpSpeed;

                _inAir = true;
                SetState(State.Jumping);

                // Unset the touching down bit field of the control object after jumping.
                // Prevents the control object's animation from being set back to the move animation during a touching down check in Post Update.
                if (ControlObject.IsTouched(GenObject.Direction.Down))
                    ControlObject.Touching ^= GenObject.Direction.Down;

                if (JumpCallback != null)
                    JumpCallback.Invoke();
            }
        }

        /// <summary>
        /// Gets whether or not the control object is being controlled to move along the x-axis.
        /// </summary>
        /// <returns>True if the control object is being controlled to move along the x-axis, false if not.</returns>
        public bool IsMovingX()
        {
            return (MoveState & (GenObject.Direction.Left | GenObject.Direction.Right)) > GenObject.Direction.None;
        }

        /// <summary>
        /// Gets whether or not the control object is being controlled to move along the y-axis.
        /// </summary>
        /// <returns>True if the control object is being controlled to move along the y-axis, false if not.</returns>
        public bool IsMovingY()
        {
            return (MoveState & (GenObject.Direction.Up | GenObject.Direction.Down)) > GenObject.Direction.None;
        }

        /// <summary>
        /// Updates the frames per second of the move animation relative to the velocity of the control object.
        /// </summary>
        protected void UpdateSpeedAnimation()
        {
            if (UseSpeedAnimation && (MoveAnimation != null))
            {
                if ((MovementSpeedX != 0) && (MovementSpeedY != 0))
                {
                    float lerp = ControlObject.Velocity.Length() / ControlObject.MaxVelocity.Length();

                    (ControlObject as GenSprite).Animations[MoveAnimation].Fps = MathHelper.Lerp(MinAnimationFps, MaxAnimationFps, lerp);
                }
                else if (MovementSpeedX != 0)
                {
                    if (ControlObject.MaxVelocity.X != 0)
                    {
                        float lerp = Math.Abs(ControlObject.Velocity.X) / ControlObject.MaxVelocity.X;

                        (ControlObject as GenSprite).Animations[MoveAnimation].Fps = MathHelper.Lerp(MinAnimationFps, MaxAnimationFps, lerp);
                    }
                }
                else if (MovementSpeedY != 0)
                {
                    if (ControlObject.MaxVelocity.Y != 0)
                    {
                        float lerp = Math.Abs(ControlObject.Velocity.Y) / ControlObject.MaxVelocity.Y;

                        (ControlObject as GenSprite).Animations[MoveAnimation].Fps = MathHelper.Lerp(MinAnimationFps, MaxAnimationFps, lerp);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the state of the object to determine which animation to display, either idle, moving, jumping, or falling.
        /// </summary>
        /// <param name="state">The state of the object to set.</param>
        protected void SetState(State state)
        {
            // Check if the object is not already in the given state to avoid displaying the same animation.
            if ((_state != state) || ((ControlObject as GenSprite).CurrentAnimation == null))
            {
                switch (state)
                {
                    case State.Idle:
                        {
                            if (IdleAnimation != null)
                                (ControlObject as GenSprite).Play(IdleAnimation, true);

                            _state = state;

                            break;
                        }
                    case State.Moving:
                        {
                            if (MoveAnimation != null)
                                (ControlObject as GenSprite).Play(MoveAnimation, true);

                            _state = state;

                            break;
                        }
                    case State.Jumping:
                        {
                            if (JumpAnimation != null)
                                (ControlObject as GenSprite).Play(JumpAnimation, true);

                            _state = state;

                            break;
                        }
                    case State.Falling:
                        {
                            if (FallAnimation != null)
                                (ControlObject as GenSprite).Play(FallAnimation, true);

                            _state = state;

                            break;
                        }
                }
            }
        }
    }
}