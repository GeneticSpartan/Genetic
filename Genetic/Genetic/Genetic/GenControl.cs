using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Genetic
{
    public class GenControl : GenBasic
    {
        public enum Movement { Instant, Accelerates };

        public enum Stopping { Instant, Deccelerates };

        protected enum State { Idle, Moving, Jumping, Falling };

        /// <summary>
        /// The object that is controlled.
        /// </summary>
        public GenObject ControlObject;

        /// <summary>
        /// How acceleration or velocity should be handled as the object moves.
        /// </summary>
        public Movement MovementType;

        /// <summary>
        /// How acceleration or velocity should be handled as the object stops.
        /// </summary>
        public Stopping StoppingType;

        /// <summary>
        /// The index number of the player controlling the object, a value of 1 through 4.
        /// </summary>
        public int Player;

        /// <summary>
        /// The keyboard controls for movement direction and jumping.
        /// </summary>
        protected Keys[] _keyboardControls = new Keys[5];

        /// <summary>
        /// The game pad controls for movement direction and jumping.
        /// </summary>
        protected Buttons[] _gamePadControls = new Buttons[5];

        /// <summary>
        /// The speed of the object moving or accelerating horizontally.
        /// The speed determines the object's acceleration or velocity depending on the movement type.
        /// </summary>
        public uint MovementSpeedX = 0;

        /// <summary>
        /// The speed of the object moving or accelerating vertically.
        /// The speed determines the object's acceleration or velocity depending on the movement type.
        /// </summary>
        public uint MovementSpeedY = 0;

        /// <summary>
        /// The vertical speed of the object used during a jump.
        /// </summary>
        public uint JumpSpeed = 0;

        /// <summary>
        /// How much horizontal and vertical gravitational force affects the object.
        /// </summary>
        public Vector2 Gravity = Vector2.Zero;

        /// <summary>
        /// A flag used to determine when the object is in the air and not touching the ground.
        /// </summary>
        protected bool _inAir = true;

        /// <summary>
        /// A flag to determine if the object is being controlled to move along the x-axis.
        /// </summary>
        protected bool _movingX = false;

        /// <summary>
        /// A flag to determine if the object is being controlled to move along the y-axis.
        /// </summary>
        protected bool _movingY = false;

        /// <summary>
        /// The current state of the object, either idle, moving, jumping, or falling.
        /// </summary>
        protected State _state = State.Idle;

        /// <summary>
        /// The name associated with the idle animation, and will be used to play it.
        /// Useful for displaying an animation when the object is not moving.
        /// A value of null will not display an animation.
        /// </summary>
        public string IdleAnimation = null;

        /// <summary>
        /// The name associated with the movement animation, and will be used to play it.
        /// Useful for displaying a walking/running animation.
        /// A value of null will not display an animation.
        /// </summary>
        public string MoveAnimation = null;

        /// <summary>
        /// The name associated with the jump animation, and will be used to play it.
        /// Useful for displaying an animation when jumping is activated.
        /// A value of null will not display an animation.
        /// </summary>
        public string JumpAnimation = null;

        /// <summary>
        /// The name associated with the fall animation, and will be used to play it.
        /// Useful for displaying an animation when the object falls.
        /// A value of null will not display an animation.
        /// </summary>
        public string FallAnimation = null;

        /// <summary>
        /// Sets up a control scheme for moving a given object.
        /// </summary>
        /// <param name="controlObject">The object that is controlled.</param>
        /// <param name="movementType">How acceleration or velocity should be handled as the object moves.</param>
        /// <param name="stoppingType">How acceleration or velocity should be handled as the object stops.</param>
        /// <param name="player">The index number of the player controlling the object, a value of 1 through 4.</param>
        public GenControl(GenObject controlObject, Movement movementType, Stopping stoppingType, int player = 1)
        {
            ControlObject = controlObject;
            MovementType = movementType;
            StoppingType = stoppingType;
            Player = player;

            // Set the default movement direction keyboard controls.
            SetDirectionControls(Keys.Left, Keys.Right, Keys.Up, Keys.Down);

            // Set the defualt jumping keyboard control.
            SetJumpControl(Keys.Space);

            // Set the default movement direction game pad controls.
            SetDirectionControls(Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown);

            // Set the defualt jumping game pad control.
            SetJumpControl(Buttons.A);
        }

        public override void Update()
        {
            ControlObject.Acceleration.X = 0;
            ControlObject.Acceleration.Y = 0;

            if (ControlObject.IsTouching(GenObject.Direction.Down))
                _inAir = false;
            else
                _inAir = true;

            if (MovementType == Movement.Instant)
            {
                if (MovementSpeedX != 0)
                {
                    if (GenG.Keyboards.IsPressed(_keyboardControls[0]) || GenG.GamePads.IsPressed(_gamePadControls[0]))
                    {
                        ControlObject.Velocity.X = -MovementSpeedX;
                        ((GenSprite)ControlObject).Facing = GenObject.Direction.Left;

                        _movingX = true;
                        SetState(State.Moving);
                    }
                    else if (GenG.Keyboards.IsPressed(_keyboardControls[1]) || GenG.GamePads.IsPressed(_gamePadControls[1]))
                    {
                        ControlObject.Velocity.X = MovementSpeedX;
                        ((GenSprite)ControlObject).Facing = GenObject.Direction.Right;

                        _movingX = true;
                        SetState(State.Moving);
                    }
                    else if (StoppingType == Stopping.Instant)
                    {
                        ControlObject.Velocity.X = 0f;

                        _movingX = false;
                    }
                }

                if (MovementSpeedY != 0)
                {
                    if (GenG.Keyboards.IsPressed(_keyboardControls[2]) || GenG.GamePads.IsPressed(_gamePadControls[2]))
                    {
                        ControlObject.Velocity.Y = -MovementSpeedY;

                        _movingY = true;
                        SetState(State.Moving);
                    }
                    else if (GenG.Keyboards.IsPressed(_keyboardControls[3]) || GenG.GamePads.IsPressed(_gamePadControls[3]))
                    {
                        ControlObject.Velocity.Y = MovementSpeedY;

                        _movingY = true;
                        SetState(State.Moving);
                    }
                    else if (StoppingType == Stopping.Instant)
                    {
                        ControlObject.Velocity.Y = 0f;

                        _movingY = false;
                    }
                }
            }
            else
            {
                if (MovementSpeedX != 0)
                {
                    if (GenG.Keyboards.IsPressed(_keyboardControls[0]) || GenG.GamePads.IsPressed(_gamePadControls[0]))
                    {
                        ControlObject.Acceleration.X = -MovementSpeedX;
                        ((GenSprite)ControlObject).Facing = GenObject.Direction.Left;

                        _movingX = true;
                        SetState(State.Moving);
                    }
                    else if (GenG.Keyboards.IsPressed(_keyboardControls[1]) || GenG.GamePads.IsPressed(_gamePadControls[1]))
                    {
                        ControlObject.Acceleration.X = MovementSpeedX;
                        ((GenSprite)ControlObject).Facing = GenObject.Direction.Right;

                        _movingX = true;
                        SetState(State.Moving);
                    }
                    else
                    {
                        ControlObject.Acceleration.X = 0;

                        _movingX = false;
                    }
                }

                if (MovementSpeedY != 0)
                {
                    if (GenG.Keyboards.IsPressed(_keyboardControls[2]) || GenG.GamePads.IsPressed(_gamePadControls[2]))
                    {
                        ControlObject.Acceleration.Y = -MovementSpeedY;

                        _movingY = true;
                        SetState(State.Moving);
                    }
                    else if (GenG.Keyboards.IsPressed(_keyboardControls[3]) || GenG.GamePads.IsPressed(_gamePadControls[3]))
                    {
                        ControlObject.Acceleration.Y = MovementSpeedY;

                        _movingY = true;
                        SetState(State.Moving);
                    }
                    else
                    {
                        ControlObject.Acceleration.Y = 0;

                        _movingY = false;
                    }
                }
            }

            if (!_inAir && (GenG.Keyboards.JustPressed(_keyboardControls[4]) || GenG.GamePads.JustPressed(_gamePadControls[4])))
            {
                ControlObject.Velocity.Y -= JumpSpeed;

                _inAir = true;
                SetState(State.Jumping);
            }

            if (_inAir && (ControlObject.Velocity.Y > 0))
                SetState(State.Falling);

            // If the object is touching the ground and not being controlled to move, set its state to idle.
            if (!_inAir)
            {
                if (MovementSpeedX != 0 && MovementSpeedY != 0)
                {
                    if (!_movingX && !_movingY)
                        SetState(State.Idle);
                }
                else if (MovementSpeedX != 0)
                {
                    if (!_movingX)
                        SetState(State.Idle);
                }
                else if (MovementSpeedY != 0)
                {
                    if (!_movingY)
                        SetState(State.Idle);
                }
            }
            
            ControlObject.Acceleration.X += Gravity.X;
            ControlObject.Acceleration.Y += Gravity.Y;
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
        /// Sets the state of the object to determine which animation to display, either idle, moving, jumping, or falling.
        /// </summary>
        /// <param name="state">The state of the object to set.</param>
        protected void SetState(State state)
        {
            // Check if the object is not already in the given state to avoid displaying the same animation.
            if (_state != state)
            {
                switch (state)
                {
                    case State.Idle:
                        if (IdleAnimation != null)
                            ((GenSprite)ControlObject).Play(IdleAnimation);

                        _state = state;

                        break;
                    case State.Moving:
                        if (MoveAnimation != null)
                            ((GenSprite)ControlObject).Play(MoveAnimation);

                        _state = state;

                        break;
                    case State.Jumping:
                        if (JumpAnimation != null)
                            ((GenSprite)ControlObject).Play(JumpAnimation);

                        _state = state;

                        break;
                    case State.Falling:
                        if (FallAnimation != null)
                            ((GenSprite)ControlObject).Play(FallAnimation);

                        _state = state;

                        break;
                }
            }
        }
    }
}