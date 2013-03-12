using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Genetic
{
    public class GenControl : GenBasic
    {
        public enum Movement { Instant, Accelerates };

        public enum Stopping { Instant, Deccelerates, Never };

        public enum Rotation { Instant, Accelerates };

        public enum RotationStopping { Instant, Accelerates, Never };

        /// <summary>
        /// The object that is controlled.
        /// </summary>
        public GenObject controlObject;

        /// <summary>
        /// How acceleration or velocity should be handled as the object moves.
        /// </summary>
        public Movement movementType;

        /// <summary>
        /// How acceleration or velocity should be handled as the object stops.
        /// </summary>
        public Stopping stoppingType;

        /// <summary>
        /// The index number of the player controlling the object, a value of 1 through 4.
        /// </summary>
        public int player;

        /// <summary>
        /// The keyboard controls for movement direction and jumping.
        /// </summary>
        protected Keys[] _keyboardControls = new Keys[5];

        /// <summary>
        /// The game pad controls for movement direction and jumping.
        /// </summary>
        protected Buttons[] _gamePadControls = new Buttons[5];

        /// <summary>
        /// The speed of the object moving or accelerating left.
        /// The speed determines the object's acceleration or velocity depending on the movement type.
        /// </summary>
        public int movementSpeedLeft = 0;

        /// <summary>
        /// The speed of the object moving or accelerating right.
        /// The speed determines the object's acceleration or velocity depending on the movement type.
        /// </summary>
        public int movementSpeedRight = 0;

        /// <summary>
        /// The speed of the object moving or accelerating up.
        /// The speed determines the object's acceleration or velocity depending on the movement type.
        /// </summary>
        public int movementSpeedUp = 0;

        /// <summary>
        /// The speed of the object moving or accelerating down.
        /// The speed determines the object's acceleration or velocity depending on the movement type.
        /// </summary>
        public int movementSpeedDown = 0;

        /// <summary>
        /// How much horizontal and vertical gravitational force affects the object.
        /// </summary>
        public Vector2 gravity = Vector2.Zero;

        /// <summary>
        /// Sets up a control scheme for moving a given object.
        /// </summary>
        /// <param name="controlObject">The object that is controlled.</param>
        /// <param name="movementType">How acceleration or velocity should be handled as the object moves.</param>
        /// <param name="stoppingType">How acceleration or velocity should be handled as the object stops.</param>
        /// <param name="player">The index number of the player controlling the object, a value of 1 through 4.</param>
        public GenControl(GenObject controlObject, Movement movementType, Stopping stoppingType, int player = 1)
        {
            this.controlObject = controlObject;
            this.movementType = movementType;
            this.stoppingType = stoppingType;
            this.player = player;

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
            controlObject.acceleration.X = 0;
            controlObject.acceleration.Y = 0;

            if (movementType == Movement.Instant)
            {
                if (GenG.Keyboards.IsPressed(_keyboardControls[0]) || GenG.GamePads.IsPressed(_gamePadControls[0]))
                    controlObject.velocity.X = movementSpeedLeft;
                else if (GenG.Keyboards.IsPressed(_keyboardControls[1]) || GenG.GamePads.IsPressed(_gamePadControls[1]))
                    controlObject.velocity.X = movementSpeedRight;
                else if (gravity.X == 0)
                    controlObject.velocity.X = 0;

                if (GenG.Keyboards.IsPressed(_keyboardControls[2]) || GenG.GamePads.IsPressed(_gamePadControls[2]))
                    controlObject.velocity.Y = movementSpeedUp;
                else if (GenG.Keyboards.IsPressed(_keyboardControls[3]) || GenG.GamePads.IsPressed(_gamePadControls[3]))
                    controlObject.velocity.Y = movementSpeedDown;
                else if (gravity.Y == 0)
                    controlObject.velocity.Y = 0;
            }
            else
            {
                if (GenG.Keyboards.IsPressed(_keyboardControls[0]) || GenG.GamePads.IsPressed(_gamePadControls[0]))
                    controlObject.acceleration.X = movementSpeedLeft;
                else if (GenG.Keyboards.IsPressed(_keyboardControls[1]) || GenG.GamePads.IsPressed(_gamePadControls[1]))
                    controlObject.acceleration.X = movementSpeedRight;
                else
                    controlObject.acceleration.X = 0;

                if (GenG.Keyboards.IsPressed(_keyboardControls[2]) || GenG.GamePads.IsPressed(_gamePadControls[2]))
                    controlObject.acceleration.Y = movementSpeedUp;
                else if (GenG.Keyboards.IsPressed(_keyboardControls[3]) || GenG.GamePads.IsPressed(_gamePadControls[3]))
                    controlObject.acceleration.Y = movementSpeedDown;
                else
                    controlObject.acceleration.Y = 0;
            }


            if (GenG.Keyboards.JustPressed(_keyboardControls[4]) || GenG.GamePads.JustPressed(_gamePadControls[4]))
                controlObject.velocity.Y = -400;
            
            controlObject.acceleration.X += gravity.X;
            controlObject.acceleration.Y += gravity.Y;
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
            movementSpeedLeft = (int)-xSpeed;
            movementSpeedRight = (int)xSpeed;
            movementSpeedUp = (int)-ySpeed;
            movementSpeedDown = (int)ySpeed;

            controlObject.maxVelocity.X = xSpeedMax;
            controlObject.maxVelocity.Y = xSpeedMax;

            controlObject.deceleration.X = xDeceleration;
            controlObject.deceleration.Y = yDeceleration;
        }
    }
}