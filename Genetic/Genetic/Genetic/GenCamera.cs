using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using Genetic.Geometry;

namespace Genetic
{
    /// <summary>
    /// A 2-dimensional camera used to display an area of the game world to the screen.
    /// Game objects within the camera view area are drawn to a render target texture, which is then drawn to the screen.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenCamera : GenBasic
    {
        /// <summary>
        /// A bit field of flags used to determine the style that a camera will use to follow a target.
        /// </summary>
        public enum FollowType
        {
            /// <summary>
            /// The camera follows the target exactly along the x-axis.
            /// </summary>
            LockOnHorizontal = 0x0001,

            /// <summary>
            /// The camera follows the target exactly along the y-axis.
            /// </summary>
            LockOnVertical = 0x0010,

            /// <summary>
            /// The camera follows the target exactly.
            /// </summary>
            LockOn = LockOnHorizontal | LockOnVertical,

            /// <summary>
            /// The camera follows a point in front of the target along the x-axis, depending on the target's facing direction.
            /// </summary>
            LeadingHorizontal = 0x0100,

            /// <summary>
            /// The camera follows a point in front of the target along the y-axis, depending on the target's facing direction.
            /// </summary>
            LeadingVertical = 0x1000,

            /// <summary>
            /// The camera follows a point in front of the target, depending on the target's facing direction.
            /// </summary>
            Leading = LeadingHorizontal | LeadingVertical,
        }

        /// <summary>
        /// A bit field of flags used to determine the direction that a camera will shake.
        /// </summary>
        public enum ShakeDirection
        {
            /// <summary>
            /// A camera shake will move along the x-axis.
            /// </summary>
            Horizontal = 0x01,

            /// <summary>
            /// A camera shake will move along the y-axis.
            /// </summary>
            Vertical = 0x10,

            /// <summary>
            /// A camera shake will move along both the x-axis and y-axis simultaneously.
            /// </summary>
            Both = Horizontal | Vertical
        };

        /// <summary>
        /// The bounding rectangle of the camera.
        /// </summary>
        protected Rectangle _cameraRect;

        /// <summary>
        /// The bounding camera view according to the camera bounding rectangle, scroll positions, and zoom level.
        /// </summary>
        protected GenAABB _cameraView;

        /// <summary>
        /// The center position of the camera view.
        /// </summary>
        protected Vector2 _centerPosition;

        /// <summary>
        /// The up facing vector of the camera relative to the camera rotation.
        /// </summary>
        protected Vector2 _up;

        /// <summary>
        /// The render target texture used to apply post-process effects after the camera is drawn.
        /// </summary>
        public RenderTarget2D RenderTarget;

        /// <summary>
        /// An extra render target texture needed when drawing in the pixel draw mode.
        /// </summary>
        public RenderTarget2D PixelRenderTarget;

        /// <summary>
        /// The texture used to draw the camera's background color.
        /// </summary>
        protected Texture2D _backgroundTexture;

        /// <summary>
        /// The alpha used when drawing a camera effect.
        /// </summary>
        protected float _fxAlpha;

        /// <summary>
        /// The color used to tint the camera. White means no tint.
        /// </summary>
        public Color Color;

        /// <summary>
        /// The background color of the camera.
        /// </summary>
        public Color BgColor;

        /// <summary>
        /// The x and y values used for scrolling the camera view.
        /// </summary>
        protected Vector2 _scroll;

        /// <summary>
        /// The x and y values used for scrolling the camera view during the previous update.
        /// </summary>
        protected Vector2 _oldScroll;

        /// <summary>
        /// The x and y velocities calculated from the camera scroll value change between each update.
        /// </summary>
        protected Vector2 _velocity;

        /// <summary>
        /// The rotation of the camera in radians.
        /// </summary>
        protected float _rotation;

        /// <summary>
        /// The position that the camera will rotate around.
        /// </summary>
        public Vector2 Origin;

        /// <summary>
        /// The scale at which to draw the camera render target texture.
        /// </summary>
        public Vector2 Scale;

        /// <summary>
        /// The position used to draw the camera.
        /// Includes the camera position and origin values to place the camera correctly.
        /// </summary>
        protected Vector2 _drawPosition;

        /// <summary>
        /// The initial scale to draw objects when the camera is created.
        /// </summary>
        protected float _initialZoom;

        /// <summary>
        /// The scale to draw objects in the camera.
        /// </summary>
        protected float _zoom;

        /// <summary>
        /// The style that the camera will use to follow a target.
        /// </summary>
        public FollowType CameraFollowType;

        /// <summary>
        /// A list of game objects that the camera will follow.
        /// </summary>
        protected List<GenObject> _followTargets;

        /// <summary>
        /// The x and y positions of the point that the camera will follow, determined by the follow targets.
        /// </summary>
        public Vector2 FollowPosition;

        /// <summary>
        /// The x and y distances from the follow target used in the leading follow style.
        /// </summary>
        public Vector2 FollowLeading;

        /// <summary>
        /// Controls the smoothness of the camera as it follows a target, a value from 0 to 1.
        /// A value of 1 will cause the camera to follow a target exactly, and any other value closer to 0 means smoother/slower camera movement.
        /// </summary>
        protected float _followStrength;

        /// <summary>
        /// The minimum amount of camera zoom possible when following multiple targets.
        /// </summary>
        public float MinZoom;

        /// <summary>
        /// The maximum amount of camera zoom possible when following multiple targets.
        /// </summary>
        public float MaxZoom;

        /// <summary>
        /// The x and y position offsets used to apply camera shake.
        /// </summary>
        protected Vector2 _shakeOffset;

        /// <summary>
        /// The current intensity of the camera shake.
        /// </summary>
        protected float _shakeIntensity;

        /// <summary>
        /// Determines whether the camera shake intensity decreases over time.
        /// </summary>
        protected bool _shakeDecreasing;

        /// <summary>
        /// The direction of the camera shake.
        /// </summary>
        protected ShakeDirection _shakeDirection;

        /// <summary>
        /// A timer used to manage camera shakes.
        /// </summary>
        protected GenTimer _shakeTimer;

        /// <summary>
        /// The sprite effect used to draw the camera flipped horizontally or vertically.
        /// </summary>
        public SpriteEffects SpriteEffect;

        /// <summary>
        /// The camera effect manager.
        /// </summary>
        protected GenScreenEffect _cameraEffect;

        /// <summary>
        /// The blend state that will be used when the camera render target texture is drawn.
        /// </summary>
        public BlendState BlendState;

        /// <summary>
        /// Gets the bounding rectangle of the camera.
        /// </summary>
        public Rectangle CameraRect
        {
            get { return _cameraRect; }
        }

        /// <summary>
        /// Gets the bounding camera view according to the camera bounding rectangle, scroll positions, and zoom level.
        /// </summary>
        public GenAABB CameraView
        {
            get { return _cameraView; }
        }

        /// <summary>
        /// Gets the width of the camera bounding rectangle.
        /// </summary>
        public int Width
        {
            get { return _cameraRect.Width; }
        }

        /// <summary>
        /// Gets the height of the camera bounding rectangle.
        /// </summary>
        public int Height
        {
            get { return _cameraRect.Height; }
        }

        /// <summary>
        /// Gets the center position of the camera view.
        /// </summary>
        public Vector2 CenterPosition
        {
            get { return _centerPosition; }
        }

        /// <summary>
        /// Gets the up facing vector of the camera relative to the camera rotation.
        /// </summary>
        public Vector2 Up
        {
            get { return _up; }
        }

        /// <summary>
        /// Gets the position used to draw the camera.
        /// Includes the camera position and origin values to place the camera correctly.
        /// </summary>
        public Vector2 DrawPosition
        {
            get { return _drawPosition; }
        }

        /// <summary>
        /// Gets or sets the x value used for scrolling the camera view.
        /// </summary>
        public float ScrollX
        {
            get { return _scroll.X; }

            set
            {
                _scroll.X = value;

                RefreshCameraView();
            }
        }

        /// <summary>
        /// Gets or sets the y value used for scrolling the camera view.
        /// </summary>
        public float ScrollY
        {
            get { return _scroll.Y; }

            set
            {
                _scroll.Y = value;

                RefreshCameraView();
            }
        }

        /// <summary>
        /// Gets the x and y velocities calculated from the camera scroll value change between each update.
        /// </summary>
        public Vector2 Velocity
        {
            get { return _velocity; }
        }

        /// <summary>
        /// Get or sets the rotation of the camera in degrees.
        /// </summary>
        public float Rotation
        {
            get { return MathHelper.ToDegrees(_rotation); }

            set
            {
                if ((value > 360) || (value < -360))
                    value %= 360;

                _rotation = MathHelper.ToRadians(value);
            }
        }

        /// <summary>
        /// Get or sets the scale at which to draw objects in the camera.
        /// </summary>
        public float Zoom
        {
            get { return _zoom; }

            set
            {
                _zoom = value;
                
                RefreshCameraView();
            }
        }

        /// <summary>
        /// The transform matrix associated with the camera view.
        /// </summary>
        public Matrix Transform
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the smoothness of the camera as it follows a target, a value from 0 to 1.
        /// A value of 1 will cause the camera to follow a target exactly, and any other value closer to 0 means smoother/slower camera movement.
        /// </summary>
        public float FollowStrength
        {
            get { return _followStrength; }

            set { _followStrength = MathHelper.Clamp(value, 0, 1); }
        }

        /// <summary>
        /// Gets whether the camera is shaking or not.
        /// </summary>
        public bool Shaking
        {
            get { return _shakeTimer.IsRunning; }
        }

        /// <summary>
        /// A camera occupies a space on the game screen to draw game objects.
        /// An error will occur if the camera is positioned outside of the game window.
        /// </summary>
        /// <param name="x">The x position of the camera.</param>
        /// <param name="y">The y position of the camera.</param>
        /// <param name="width">The width of the camera.</param>
        /// <param name="height">The height of the camera.</param>
        /// <param name="zoom">The scale at which to draw objects in the camera.</param>
        public GenCamera(float x, float y, int width, int height, float zoom)
        {
            _cameraRect = new Rectangle(0, 0, width, height);
            _cameraView = new GenAABB(0f, 0f, width, height);
            _centerPosition = new Vector2(_cameraView.MidpointX, _cameraView.MidpointY);
            _up = new Vector2(0f, -1f);
            RenderTarget = new RenderTarget2D(GenG.GraphicsDevice, width, height);
            PixelRenderTarget = new RenderTarget2D(GenG.GraphicsDevice, width, height);
            _backgroundTexture = GenU.MakeTexture(Color.White, 1, 1);
            Color = Color.White;
            BgColor = Color.Transparent;
            _scroll = Vector2.Zero;
            _oldScroll = _scroll;
            _velocity = Vector2.Zero;
            _rotation = 0f;
            Origin = new Vector2(width * 0.5f, height * 0.5f);
            Scale = Vector2.One;
            _drawPosition = new Vector2(x + Origin.X, y + Origin.Y);
            _initialZoom = zoom;
            Zoom = _initialZoom;
            CameraFollowType = FollowType.LockOn;
            _followTargets = new List<GenObject>();
            FollowPosition = Vector2.Zero;
            FollowLeading = new Vector2(50, 30);
            _followStrength = 1f;
            MinZoom = _initialZoom;
            MaxZoom = 4f;
            _shakeOffset = Vector2.Zero;
            _shakeIntensity = 0f;
            _shakeDecreasing = false;
            _shakeTimer = new GenTimer(0f, null);
            SpriteEffect = SpriteEffects.None;
            _cameraEffect = new GenScreenEffect(_cameraRect);
            Transform = Matrix.Identity;

            // Set up the default camera blend state as alpha blend.
            BlendState = new BlendState();
            BlendState.ColorSourceBlend = Blend.One;
            BlendState.ColorDestinationBlend = Blend.InverseSourceAlpha;
            BlendState.ColorBlendFunction = BlendFunction.Add;
            BlendState.AlphaSourceBlend = Blend.One;
            BlendState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
            BlendState.AlphaBlendFunction = BlendFunction.Add;
        }

        /// <summary>
        /// Handles any post-update logic for the camera.
        /// </summary>
        public override void PostUpdate()
        {
            if (_followTargets.Count > 0)
            {
                if (_followTargets.Count == 1)
                {
                    float followTargetX = _followTargets[0].CenterPosition.X;
                    float followTargetY = _followTargets[0].CenterPosition.Y;

                    if (GenG.DrawMode == GenG.DrawType.Pixel)
                    {
                        followTargetX = (int)followTargetX;
                        followTargetY = (int)followTargetY;
                    }

                    if ((CameraFollowType & FollowType.LockOn) > 0)
                    {
                        // Get the horizontal lock-on position of the camera follow target.
                        if ((CameraFollowType & FollowType.LockOnHorizontal) == FollowType.LockOnHorizontal)
                        {
                            FollowPosition.X += (followTargetX - FollowPosition.X) * _followStrength;
                        }

                        // Get the vertical lock-on position of the camera follow target.
                        if ((CameraFollowType & FollowType.LockOnVertical) == FollowType.LockOnVertical)
                        {
                            FollowPosition.Y += (followTargetY - FollowPosition.Y) * _followStrength;
                        }
                    }
                    else if ((CameraFollowType & FollowType.Leading) > 0)
                    {
                        // Get the horizontal leading position of the camera follow target.
                        if ((CameraFollowType & FollowType.LeadingHorizontal) == FollowType.LeadingHorizontal)
                        {
                            switch (_followTargets[0].Facing)
                            {
                                case GenObject.Direction.Left:
                                    FollowPosition.X += (followTargetX - FollowLeading.X - FollowPosition.X) * _followStrength;
                                    break;
                                case GenObject.Direction.Right:
                                    FollowPosition.X += (followTargetX + FollowLeading.X - FollowPosition.X) * _followStrength;
                                    break;
                                default:
                                    FollowPosition.X += (followTargetX - FollowPosition.X) * _followStrength;
                                    break;
                            }
                        }

                        // Get the vertical leading position of the camera follow target.
                        if ((CameraFollowType & FollowType.LeadingVertical) == FollowType.LeadingVertical)
                        {
                            switch (_followTargets[0].Facing)
                            {
                                case GenObject.Direction.Up:
                                    FollowPosition.Y += (followTargetY - FollowLeading.Y - FollowPosition.Y) * _followStrength;
                                    break;
                                case GenObject.Direction.Down:
                                    FollowPosition.Y += (followTargetY + FollowLeading.Y - FollowPosition.Y) * _followStrength;
                                    break;
                                default:
                                    FollowPosition.Y += (followTargetY - FollowPosition.Y) * _followStrength;
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    // Set the initial minimum and maximum x and y values based on the first follow target.
                    float followXMin = _followTargets[0].Bounds.Left;
                    float followXMax = _followTargets[0].Bounds.Right;
                    float followYMin = _followTargets[0].Bounds.Top;
                    float followYMax = _followTargets[0].Bounds.Bottom;

                    // Loop through the remaining follow targets and adjust the minimum and maximum x and y values accordingly.
                    for (int i = 1; i < _followTargets.Count; i++)
                    {
                        followXMin = Math.Min(followXMin, _followTargets[i].Bounds.Left);
                        followXMax = Math.Max(followXMax, _followTargets[i].Bounds.Right);
                        followYMin = Math.Min(followYMin, _followTargets[i].Bounds.Top);
                        followYMax = Math.Max(followYMax, _followTargets[i].Bounds.Bottom);
                    }

                    // Set the follow target to the center point between the minimum and maximum x and y values of all combined follow targets.
                    FollowPosition.X += ((followXMin + followXMax) * 0.5f - FollowPosition.X) * _followStrength;
                    FollowPosition.Y += ((followYMin + followYMax) * 0.5f - FollowPosition.Y) * _followStrength;

                    float distanceX = Math.Abs(followXMax - followXMin) * 2;
                    float distanceY = Math.Abs(followYMax - followYMin) * 2;

                    // Zoom the camera in or out, not exceeding the minimum and maximum zoom values.
                    // Attempt to keep all follow targets within the camera view.
                    Zoom += (MathHelper.Clamp(MathHelper.Min(_cameraRect.Width / distanceX, _cameraRect.Height / distanceY), MinZoom, MaxZoom) - Zoom) * _followStrength;
                }

                // Get the camera scroll values during the previous update.
                _oldScroll = _scroll;

                ScrollX = -FollowPosition.X + _cameraView.Width * 0.5f;
                ScrollY = -FollowPosition.Y + _cameraView.Height * 0.5f;

                // Prevent the camera view from moving outside of the world bounds.
                if (_scroll.X > -GenG.WorldBounds.Left)
                    ScrollX = -GenG.WorldBounds.Left;
                else if (_scroll.X < -GenG.WorldBounds.Right + _cameraView.Width)
                    ScrollX = -GenG.WorldBounds.Right + _cameraView.Width;

                if (_scroll.Y > -GenG.WorldBounds.Top)
                    ScrollY = -GenG.WorldBounds.Top;
                else if (_scroll.Y < -GenG.WorldBounds.Bottom + _cameraView.Height)
                    ScrollY = -GenG.WorldBounds.Bottom + _cameraView.Height;
            }

            if (_shakeTimer.IsRunning)
            {
                _shakeTimer.Update();

                if (!_shakeTimer.IsRunning)
                    _shakeOffset = Vector2.Zero;
                else
                {
                    if ((_shakeDirection & ShakeDirection.Horizontal) == ShakeDirection.Horizontal)
                        _shakeOffset.X = (((float)GenU.Random() * _shakeIntensity * 2) - _shakeIntensity);

                    if ((_shakeDirection & ShakeDirection.Vertical) == ShakeDirection.Vertical)
                        _shakeOffset.Y = (((float)GenU.Random() * _shakeIntensity * 2) - _shakeIntensity);

                    if (_shakeDecreasing)
                    {
                        float shakeDecrease = _shakeTimer.Remaining / _shakeTimer.Duration;

                        _shakeOffset.X *= shakeDecrease;
                        _shakeOffset.Y *= shakeDecrease;
                    }
                }
            }

            // Add camera shake.
            ScrollX += _shakeOffset.X;
            ScrollY += _shakeOffset.Y;

            // Create the camera transform.
            if (GenG.DrawMode == GenG.DrawType.Pixel)
            {
                if (_rotation == 0)
                    Transform = Matrix.CreateTranslation((int)(_scroll.X), (int)(_scroll.Y), 0f);
                else
                {
                    Transform =
                    Matrix.CreateTranslation((int)(_scroll.X) - (Origin.X / _zoom), (int)(_scroll.Y) - (Origin.Y / _zoom), 0f) *
                    Matrix.CreateRotationZ(_rotation) *
                    Matrix.CreateTranslation(Origin.X / _zoom, Origin.Y / _zoom, 0f);
                }
            }
            else
            {
                if (_rotation == 0)
                    Transform = Matrix.CreateTranslation(_scroll.X, _scroll.Y, 0f) * Matrix.CreateScale(_zoom);
                else
                {
                    Transform =
                    Matrix.CreateTranslation(_scroll.X - (Origin.X / _zoom), _scroll.Y - (Origin.Y / _zoom), 0f) *
                    Matrix.CreateRotationZ(_rotation) *
                    Matrix.CreateTranslation(Origin.X / _zoom, Origin.Y / _zoom, 0f) *
                    Matrix.CreateScale(_zoom);
                }
            }

            // Set the camera up vector using the camera rotation.
            _up = GenMove.AngleToVector(MathHelper.ToDegrees(_rotation));

            // Set the velocity of the camera using the difference between the current and previous camera scroll values.
            _velocity = _scroll - _oldScroll;
        }

        /// <summary>
        /// Draws game objects within the camera to the camera render target.
        /// </summary>
        public void DrawObjects()
        {
            GenG.GraphicsDevice.SetRenderTarget(RenderTarget);

            // Clear the back buffer to the camera background color.
            GenG.GraphicsDevice.Clear(BgColor);

            if (GenG.DrawMode == GenG.DrawType.Pixel)
                GenG.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Transform);
            else
                GenG.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, Transform);

            GenG.State.Draw(this);
            GenG.SpriteBatch.End();

            // Draw the camera effects.
            GenG.SpriteBatch.Begin();
            DrawFx();
            GenG.SpriteBatch.End();
        }

        /// <summary>
        /// Draws the camera effects.
        /// </summary>
        public void DrawFx()
        {
            _cameraEffect.Draw();
        }

        /// <summary>
        /// Draws the camera render target using the point clamp sampler state.
        /// Used when in the pixel draw mode.
        /// </summary>
        public void DrawPixelRenderTarget()
        {
            GenG.GraphicsDevice.SetRenderTarget(PixelRenderTarget);

            // Clear the back buffer to transparent.
            GenG.GraphicsDevice.Clear(Color.Transparent);

            GenG.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(_zoom));
            GenG.SpriteBatch.Draw(RenderTarget, Vector2.Zero, Color.White);
            GenG.SpriteBatch.End();
        }

        /// <summary>
        /// Draws the final camera render target.
        /// <c>SetEffect</c> and <c>ApplyEffect</c> are called to set and apply any shader effects before drawing the render target texture.
        /// </summary>
        public void DrawFinal()
        {
            SetEffect();

            // Begin drawing the camera render target texture.
            if (GenG.DrawMode == GenG.DrawType.Pixel)
                GenG.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState, SamplerState.PointClamp, null, null, null, Matrix.Identity);
            else
                GenG.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState, SamplerState.LinearClamp, null, null, null, Matrix.Identity);

            ApplyEffect();

            // Draw the camera render target texture.
            if (GenG.DrawMode == GenG.DrawType.Pixel)
                GenG.SpriteBatch.Draw(PixelRenderTarget, _drawPosition, null, Color, 0f, Origin, Scale, SpriteEffect, 0);
            else
                GenG.SpriteBatch.Draw(RenderTarget, _drawPosition, null, Color, 0f, Origin, Scale, SpriteEffect, 0);

            GenG.SpriteBatch.End();
        }

        /// <summary>
        /// Sets the essential components that function together as the camera view area.
        /// Do not set the camera view often, since new render target objects must be created.
        /// </summary>
        /// <param name="x">The x position of the top-left corner of the camera view.</param>
        /// <param name="y">The y position of the top-left corner of the camera view.</param>
        /// <param name="width">The width of the camera view.</param>
        /// <param name="height">The height of the camera view.</param>
        public void SetCameraView(float x, float y, int width, int height)
        {
            // Prevent the width and height from being set to values less than 1.
            // The render target must have width and height values of 1 or greater.
            width = (int)MathHelper.Max(1, width);
            height = (int)MathHelper.Max(1, height);

            _cameraRect.Width = width;
            _cameraRect.Height = height;

            _cameraEffect.EffectRectangle = _cameraRect;

            Origin.X = width * 0.5f;
            Origin.Y = height * 0.5f;

            _drawPosition.X = x + Origin.X;
            _drawPosition.Y = y + Origin.Y;

            // Create new render target objects to comply with the new size settings.
            RenderTarget = new RenderTarget2D(GenG.GraphicsDevice, width, height);
            PixelRenderTarget = new RenderTarget2D(GenG.GraphicsDevice, width, height);
        }

        /// <summary>
        /// Adds an object or group of objects as targets that the camera will follow.
        /// Adding multiple targets will cause the camera to follow a point within the center of all targets.
        /// Adding the same object twice is useful for zooming in on the target.
        /// </summary>
        /// <param name="objectOrGroup">The object or group of objects to set as a target.</param>
        public void AddTarget(GenBasic objectOrGroup = null)
        {
            if (objectOrGroup is GenObject)
                _followTargets.Add(objectOrGroup as GenObject);
            else if (objectOrGroup is GenGroup)
            {
                foreach (GenBasic basic in (objectOrGroup as GenGroup).Members)
                    AddTarget(basic);
            }
        }

        /// <summary>
        /// Removes an object or group of objects from the follow targets list.
        /// </summary>
        /// <param name="objectOrGroup">The object or group of objects to remove from the follow targets list.</param>
        public void RemoveTarget(GenBasic objectOrGroup)
        {
            if (objectOrGroup is GenObject)
            {
                _followTargets.Remove(objectOrGroup as GenObject);
            }
            else if (objectOrGroup is GenGroup)
            {
                foreach (GenBasic basic in (objectOrGroup as GenGroup).Members)
                    RemoveTarget(basic);
            }
        }

        /// <summary>
        /// Clears the follow targets list of all objects.
        /// </summary>
        public void ClearTargets()
        {
            _followTargets.Clear();
        }

        /// <summary>
        /// Give the camera a shaking effect.
        /// </summary>
        /// <param name="intensity">The amount of camera shake.</param>
        /// <param name="duration">The duration of the camera shake, in seconds.</param>
        /// <param name="decreasing">Whether the shake intensity will decrease over time or not.</param>
        /// <param name="forceReset">A flag used to determine if the shake will reset any current camera shake.</param>
        /// <param name="callback">The method that will be invoked after the camera shake has finished.</param>
        /// <param name="direction">The direction of the camera shake.</param>
        public void Shake(float intensity = 5f, float duration = 1f, bool decreasing = false, bool forceReset = false, Action callback = null, ShakeDirection direction = ShakeDirection.Both)
        {
            // Apply the shake if the camera is not already shaking, unless force reset is true.
            if (!_shakeTimer.IsRunning || forceReset)
            {
                _shakeIntensity = intensity;
                _shakeDecreasing = decreasing;
                _shakeDirection = direction;

                _shakeTimer.Duration = duration;
                _shakeTimer.Callback = callback;
                _shakeTimer.Start(true);
            }
        }

        /// <summary>
        /// Give the camera a flash effect.
        /// </summary>
        /// <param name="intensity">The intensity, or starting opacity, of the camera flash.</param>
        /// <param name="duration">The duration of the camera flash, in seconds.</param>
        /// <param name="color">The color of the camera flash. Use null to default to white.</param>
        /// <param name="forceReset">A flag used to determine if the flash will reset any current camera flash.</param>
        /// <param name="callback">The method that will be invoked after the camera flash has finished.</param>
        public void Flash(float intensity = 1f, float duration = 1f, Color? color = null, bool forceReset = false, Action callback = null)
        {
            _cameraEffect.Flash(intensity, duration, color, forceReset, callback);
        }

        /// <summary>
        /// Give the camera a fade effect.
        /// </summary>
        /// <param name="duration">The duration of the camera fade, in seconds.</param>
        /// <param name="color">The color of the camera fade. Use null to default to black.</param>
        /// <param name="callback">The method that will be invoked after the camera fade has finished.</param>
        public void Fade(float duration = 1f, Color? color = null, Action callback = null)
        {
            _cameraEffect.Fade(duration, color, callback);
        }

        /// <summary>
        /// Calculates the camera view values relative to the current scroll and zoom values.
        /// </summary>
        protected void RefreshCameraView()
        {
            // Adjust the position of the camera view.
            _cameraView.X = -_scroll.X;
            _cameraView.Y = -_scroll.Y;

            // Adjust the camera view dimensions.
            _cameraView.Width = _cameraRect.Width / _zoom;
            _cameraView.Height = _cameraRect.Height / _zoom;

            // Adjust the center position of the camera view.
            _centerPosition.X = _cameraView.MidpointX;
            _centerPosition.Y = _cameraView.MidpointY;
        }

        public virtual void SetEffect()
        {
            
        }

        public virtual void ApplyEffect()
        {
            
        }

        /// <summary>
        /// Resets the camera.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            BgColor = Color.Transparent;
            _scroll = Vector2.Zero;
            _rotation = 0f;
            _zoom = GenG.Game.Zoom;
            FollowPosition = Vector2.Zero;
            _followTargets.Clear();

            // Reset the camera effects.
            _shakeOffset = Vector2.Zero;
            _shakeTimer.Reset();
            _cameraEffect.Reset();
        }
    }
}