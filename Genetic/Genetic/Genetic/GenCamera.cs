using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genetic
{
    public class GenCamera : GenBasic
    {
        /// <summary>
        /// The style that a camera will use to follow a target.
        /// </summary>
        public enum FollowType
        {
            /// <summary>
            /// The camera follows the target exactly.
            /// </summary>
            LockOn,

            /// <summary>
            /// The camera follows the target exactly along the x-axis.
            /// </summary>
            LockOnHorizontal,

            /// <summary>
            /// The camera follows the target exactly along the y-axis.
            /// </summary>
            LockOnVertical,

            /// <summary>
            /// The camera follows a point in front of the target, depending on the target's facing direction.
            /// </summary>
            Leading,

            /// <summary>
            /// The camera follows a point in front of the target along the x-axis, depending on the target's facing direction.
            /// </summary>
            LeadingHorizontal,

            /// <summary>
            /// The camera follows a point in front of the target along the y-axis, depending on the target's facing direction.
            /// </summary>
            LeadingVertical,
        }

        public enum ShakeDirection { Horizontal, Vertical, Both };

        /// <summary>
        /// The bounding rectangle of the camera.
        /// </summary>
        protected Rectangle _cameraRect;

        /// <summary>
        /// The bounding camera view according to the camera bounding rectangle, scroll positions, and zoom level.
        /// </summary>
        protected CameraView _cameraView = new CameraView();

        /// <summary>
        /// The x and y position of the top-left corner of the camera.
        /// </summary>
        protected Vector2 _position;

        /// <summary>
        /// The render target texture used to apply post-process effects after the camera is drawn.
        /// </summary>
        public RenderTarget2D RenderTarget;

        /// <summary>
        /// The texture used to draw camera effects such as background color, flash, and fade.
        /// </summary>
        protected Texture2D _fxTexture;

        /// <summary>
        /// The alpha used when drawing a camera effect.
        /// </summary>
        protected float _fxAlpha;

        /// <summary>
        /// The color used to tint the camera. White means no tint.
        /// </summary>
        public Color Color = Color.White;

        /// <summary>
        /// The background color of the camera.
        /// </summary>
        protected Color? _bgColor;

        /// <summary>
        /// The x and y values used for scrolling the camera view.
        /// </summary>
        protected Vector2 _scroll = Vector2.Zero;

        /// <summary>
        /// The rotation of the camera in radians.
        /// </summary>
        public float Rotation = 0f;

        /// <summary>
        /// The position that the camera will rotate around.
        /// </summary>
        public Vector2 Origin;

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
        public FollowType CameraFollowType = FollowType.LockOn;

        /// <summary>
        /// A list of game objects that the camera will follow.
        /// </summary>
        protected List<GenObject> _followTargets = new List<GenObject>();

        /// <summary>
        /// The x and y positions of the point that the camera will follow, determined by the follow targets.
        /// </summary>
        protected Vector2 _followPosition = Vector2.Zero;

        /// <summary>
        /// The x and y distances from the follow target used in the leading follow style.
        /// </summary>
        public Vector2 FollowLeading = new Vector2(50, 30);

        /// <summary>
        /// Controls the smoothness of the camera as it follows a target, a value from 0 to 1.
        /// A value of 1 will cause the camera to follow a target exactly, and any other value closer to 0 means smoother/slower camera movement.
        /// </summary>
        protected float _followStrength = 1f;

        /// <summary>
        /// The minimum amount of camera zoom possible when following multiple targets.
        /// </summary>
        public float MinZoom = 1f;

        /// <summary>
        /// The maximum amount of camera zoom possible when following multiple targets.
        /// </summary>
        public float MaxZoom = 4f;

        /// <summary>
        /// The x and y position offsets used to apply camera shake.
        /// </summary>
        protected Vector2 _shakeOffset = Vector2.Zero;

        /// <summary>
        /// Determines if the camera is currently shaking.
        /// </summary>
        protected bool _shaking = false;

        /// <summary>
        /// The current intensity of the camera shake.
        /// </summary>
        protected float _shakeIntensity = 0f;

        /// <summary>
        /// The current duration of the camera shake.
        /// </summary>
        protected float _shakeDuration = 0f;

        /// <summary>
        /// Determines whether the camera shake intensity decreases over time.
        /// </summary>
        protected bool _shakeDecreasing = false;

        /// <summary>
        /// The amount of time since the camera shake started, in seconds.
        /// </summary>
        protected float _shakeTimer = 0f;

        /// <summary>
        /// The callback function that will invoke after the camera shake has finished.
        /// </summary>
        protected Action _shakeCallback;

        /// <summary>
        /// The direction of the camera shake.
        /// </summary>
        protected ShakeDirection _shakeDirection;

        /// <summary>
        /// Determines if the camera is currently flashing.
        /// </summary>
        protected bool _flashing = false;

        /// <summary>
        /// The current intensity, or starting opacity, of the camera flash.
        /// </summary>
        protected float _flashIntensity = 0f;

        /// <summary>
        /// The current color of the camera flash.
        /// </summary>
        protected Color _flashColor;

        /// <summary>
        /// The current duration of the camera flash.
        /// </summary>
        protected float _flashDuration = 0f;

        /// <summary>
        /// The amount of time since the camera flash started, in seconds.
        /// </summary>
        protected float _flashTimer = 0f;

        /// <summary>
        /// The callback function that will invoke after the camera flash has finished.
        /// </summary>
        protected Action _flashCallback;

        /// <summary>
        /// Determines if the camera is currently fading.
        /// </summary>
        protected bool _fading = false;

        /// <summary>
        /// The current color of the camera fade.
        /// </summary>
        protected Color _fadeColor;

        /// <summary>
        /// The current duration of the camera fade.
        /// </summary>
        protected float _fadeDuration = 0f;

        /// <summary>
        /// The amount of time since the camera fade started, in seconds.
        /// </summary>
        protected float _fadeTimer = 0f;

        /// <summary>
        /// The callback function that will invoke after the camera fade has finished.
        /// </summary>
        protected Action _fadeCallback;

        /// <summary>
        /// Gets the bounding camera view according to the camera bounding rectangle, scroll positions, and zoom level.
        /// </summary>
        public CameraView CameraView
        {
            get { return _cameraView; }
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
        /// The transform matrix associated with the camera viewport.
        /// </summary>
        public Matrix Transform
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the background color of the camera.
        /// </summary>
        public Color BgColor
        {
            get { return _bgColor.HasValue ? _bgColor.Value : Color.Transparent; }

            set { _bgColor = value; }
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
            get { return _shaking; }
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
        public GenCamera(int x, int y, int width, int height, float zoom)
        {
            _cameraRect = new Rectangle(0, 0, width, height);
            _position = new Vector2(x, y);
            RenderTarget = new RenderTarget2D(GenG.GraphicsDevice, width, height);
            _fxTexture = new Texture2D(GenG.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _fxTexture.SetData<Color>(new[] { Color.White });
            Origin = new Vector2(width * 0.5f, height * 0.5f);
            _drawPosition = new Vector2(x + Origin.X, y + Origin.Y);
            _initialZoom = zoom;
            Zoom = _initialZoom;
            Transform = Matrix.Identity;
        }

        public override void PostUpdate()
        {
            if (_followTargets.Count > 0)
            {
                if (_followTargets.Count > 1)
                {
                    // Set the initial minimum and maximum x and y values based on the first follow target.
                    float followXMin = _followTargets[0].BoundingBox.Left;
                    float followXMax = _followTargets[0].BoundingBox.Right;
                    float followYMin = _followTargets[0].BoundingBox.Top;
                    float followYMax = _followTargets[0].BoundingBox.Bottom;

                    // Loop through the remaining follow targets and adjust the minimum and maximum x and y values accordingly.
                    for (int i = 1; i < _followTargets.Count; i++)
                    {
                        if (_followTargets[i].BoundingBox.Left < followXMin)
                            followXMin = _followTargets[i].BoundingBox.Left;

                        if (_followTargets[i].BoundingBox.Right > followXMax)
                            followXMax = _followTargets[i].BoundingBox.Right;

                        if (_followTargets[i].BoundingBox.Top < followYMin)
                            followYMin = _followTargets[i].BoundingBox.Top;

                        if (_followTargets[i].BoundingBox.Bottom > followYMax)
                            followYMax = _followTargets[i].BoundingBox.Bottom;
                    }

                    // Set the follow target to the center point between the minimum and maximum x and y values of all combined follow targets.
                    _followPosition.X += ((followXMin + followXMax) * 0.5f - _followPosition.X) * _followStrength;
                    _followPosition.Y += ((followYMin + followYMax) * 0.5f - _followPosition.Y) * _followStrength;

                    float distanceX = Math.Abs(followXMax - followXMin) * 2;
                    float distanceY = Math.Abs(followYMax - followYMin) * 2;

                    // Zoom the camera in or out, complying with the minimum and maximum zoom values, and attempt to keep all follow targets within the camera view.
                    Zoom += (MathHelper.Clamp(MathHelper.Min(_cameraRect.Width / distanceX, _cameraRect.Height / distanceY), MinZoom, MaxZoom) - Zoom) * _followStrength;
                }
                else
                {
                    float followTargetX = 0;
                    float followTargetY = 0;

                    if (GenG.DrawMode == GenG.DrawType.Pixel)
                    {
                        followTargetX = (int)_followTargets[0].X;
                        followTargetY = (int)_followTargets[0].Y;
                    }
                    else if (GenG.DrawMode == GenG.DrawType.Smooth)
                    {
                        followTargetX = _followTargets[0].X;
                        followTargetY = _followTargets[0].Y;
                    }

                    if ((CameraFollowType == FollowType.LockOn) || (CameraFollowType == FollowType.LockOnHorizontal))
                        _followPosition.X += (followTargetX + _followTargets[0].BoundingBox.HalfWidth - _followPosition.X) * _followStrength;

                    if ((CameraFollowType == FollowType.LockOn) || (CameraFollowType == FollowType.LockOnVertical))
                        _followPosition.Y += (followTargetY + _followTargets[0].BoundingBox.HalfHeight - _followPosition.Y) * _followStrength;

                    if ((CameraFollowType == FollowType.Leading) || (CameraFollowType == FollowType.LeadingHorizontal))
                    {
                        if (_followTargets[0].Facing == GenObject.Direction.Left)
                            _followPosition.X += (followTargetX - FollowLeading.X - _followPosition.X) * _followStrength;
                        else if (_followTargets[0].Facing == GenObject.Direction.Right)
                            _followPosition.X += (followTargetX + FollowLeading.X - _followPosition.X) * _followStrength;
                        else
                            _followPosition.X += (followTargetX - _followPosition.X) * _followStrength;
                    }

                    if ((CameraFollowType == FollowType.Leading) || (CameraFollowType == FollowType.LeadingVertical))
                    {
                        if (_followTargets[0].Facing == GenObject.Direction.Up)
                            _followPosition.Y += (followTargetY - FollowLeading.Y - _followPosition.Y) * _followStrength;
                        else if (_followTargets[0].Facing == GenObject.Direction.Down)
                            _followPosition.Y += (followTargetY + FollowLeading.Y - _followPosition.Y) * _followStrength;
                        else
                            _followPosition.Y += (followTargetY - _followPosition.Y) * _followStrength;
                    }
                }

                ScrollX = -_followPosition.X + _cameraView.Width * 0.5f;
                ScrollY = -_followPosition.Y + _cameraView.Height * 0.5f;

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

            if (_shaking)
            {
                if (_shakeTimer < _shakeDuration)
                {
                    if ((_shakeDirection == ShakeDirection.Both) || (_shakeDirection == ShakeDirection.Horizontal))
                        _shakeOffset.X = (((float)GenU.Random() * _shakeIntensity * 2) - _shakeIntensity);
                    if ((_shakeDirection == ShakeDirection.Both) || (_shakeDirection == ShakeDirection.Vertical))
                        _shakeOffset.Y = (((float)GenU.Random() * _shakeIntensity * 2) - _shakeIntensity);

                    if (_shakeDecreasing)
                    {
                        float shakeFade = (_shakeDuration - _shakeTimer) / _shakeDuration;

                        _shakeOffset.X *= shakeFade;
                        _shakeOffset.Y *= shakeFade;
                    }

                    _shakeTimer += GenG.PhysicsTimeStep;
                }
                else
                {
                    _shaking = false;

                    if (_shakeCallback != null)
                        _shakeCallback.Invoke();

                    _shakeOffset = Vector2.Zero;
                }
            }

            // Add camera shake.
            ScrollX += _shakeOffset.X;
            ScrollY += _shakeOffset.Y;

            // Create the camera transform.
            if (GenG.DrawMode == GenG.DrawType.Pixel)
                Transform = Matrix.CreateTranslation((int)_scroll.X, (int)_scroll.Y, 0f);
            else if (GenG.DrawMode == GenG.DrawType.Smooth)
                Transform = Matrix.CreateTranslation(_scroll.X, _scroll.Y, 0f) * Matrix.CreateScale(_zoom);
        }

        /// <summary>
        /// Draws the camera background color.
        /// </summary>
        public void DrawBg()
        {
            if (_bgColor != null)
                GenG.SpriteBatch.Draw(_fxTexture, _cameraRect, BgColor);
        }

        /// <summary>
        /// Draws the camera flash and fade effects.
        /// </summary>
        public void DrawFx()
        {
            if (_flashing)
            {
                if (_flashTimer < _flashDuration)
                {
                    _fxAlpha = ((_flashDuration - _flashTimer) / _flashDuration) * _flashIntensity;

                    GenG.SpriteBatch.Draw(_fxTexture, _cameraRect, _flashColor * _fxAlpha);

                    if (!GenG.Paused)
                        _flashTimer += GenG.PhysicsTimeStep;
                }
                else
                {
                    _flashing = false;

                    if (_flashCallback != null)
                        _flashCallback.Invoke();
                }
            }

            if (_fading)
            {
                if (_fadeTimer < _fadeDuration)
                {
                    _fxAlpha = (_fadeTimer / _fadeDuration);

                    GenG.SpriteBatch.Draw(_fxTexture, _cameraRect, _fadeColor * _fxAlpha);

                    if (!GenG.Paused)
                        _fadeTimer += GenG.PhysicsTimeStep;

                    if (_fadeTimer >= _fadeDuration)
                    {
                        _fading = false;

                        if (_fadeCallback != null)
                            _fadeCallback.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// Sets the essential components that function together as the camera view area.
        /// The camera view area must not extend outside the bounds of the game window.
        /// Do not set the camera view often, since a new render target object must be created.
        /// </summary>
        /// <param name="x">The x position of the top-left corner of the camera view.</param>
        /// <param name="y">The y position of the top-left corner of the camera view.</param>
        /// <param name="width">The width of the camera view.</param>
        /// <param name="height">The height of the camera view.</param>
        public void SetCameraView(int x, int y, int width, int height)
        {
            _cameraRect.Width = width;
            _cameraRect.Height = height;

            _position.X = x;
            _position.Y = y;

            if (GenG.DrawMode == GenG.DrawType.Smooth)
            {
                Origin.X = width * 0.5f;
                Origin.Y = height * 0.5f;

                _drawPosition.X = x + Origin.X;
                _drawPosition.Y = y + Origin.Y;
            }

            // Create a new render target object to comply with the new size settings.
            RenderTarget = new RenderTarget2D(GenG.GraphicsDevice, width, height);
        }

        /// <summary>
        /// Adds a game object as a target that the camera will follow.
        /// Adding multiple targets will cause the camera to follow a point within the center of all targets.
        /// </summary>
        /// <param name="gameObject">The game object to set as a target.</param>
        public void AddTarget(GenObject gameObject = null)
        {
            _followTargets.Add(gameObject);
        }

        /// <summary>
        /// Removes a game object from the follow targets list.
        /// </summary>
        /// <param name="gameObject">The game object to remove from the follow targets list.</param>
        public void RemoveTarget(GenObject gameObject)
        {
            _followTargets.Remove(gameObject);
        }

        /// <summary>
        /// Give the camera a shaking effect.
        /// </summary>
        /// <param name="intensity">The amount of camera shake.</param>
        /// <param name="duration">The duration of the camera shake, in seconds.</param>
        /// <param name="decreasing">Whether the shake intensity will decrease over time or not.</param>
        /// <param name="callback">The method that will be invoked after the camera shake has finished.</param>
        /// <param name="direction">The direction of the camera shake.</param>
        public void Shake(float intensity = 5f, float duration = 1f, bool decreasing = false, Action callback = null, ShakeDirection direction = ShakeDirection.Both)
        {
            // Apply the shake if the camera is not already shaking.
            if (!_shaking)
            {
                _shakeIntensity = intensity;
                _shakeDuration = duration;
                _shakeDecreasing = decreasing;
                _shakeCallback = callback;
                _shakeDirection = direction;
                _shakeTimer = 0f;

                _shaking = true;
            }
        }

        /// <summary>
        /// Give the camera a flash effect.
        /// </summary>
        /// <param name="intensity">The intensity, or starting opacity, of the camera flash.</param>
        /// <param name="duration">The duration of the camera flash, in seconds.</param>
        /// <param name="color">The color of the camera flash. Use null to default to white.</param>
        /// <param name="callback">The method that will be invoked after the camera flash has finished.</param>
        public void Flash(float intensity = 1f, float duration = 1f, Color? color = null, Action callback = null)
        {
            // Give the camera flash a default color of white if no other color was passed.
            color = color.HasValue ? color.Value : Color.White;

            // Apply the flash if the camera is not already flashing.
            if (!_flashing)
            {
                _flashIntensity = intensity;
                _flashDuration = duration;
                _flashColor = color.Value;
                _flashCallback = callback;
                _flashTimer = 0f;

                _flashing = true;
            }
        }

        /// <summary>
        /// Give the camera a fade effect.
        /// </summary>
        /// <param name="duration">The duration of the camera fade, in seconds.</param>
        /// <param name="color">The color of the camera fade. Use null to default to black.</param>
        /// <param name="callback">The method that will be invoked after the camera fade has finished.</param>
        public void Fade(float duration = 1f, Color? color = null, Action callback = null)
        {
            // Give the camera flash a default color of white if no other color was passed.
            color = color.HasValue ? color.Value : Color.Black;

            // Apply the flash if the camera is not already flashing.
            if (!_fading)
            {
                _fadeDuration = duration;
                _fadeColor = color.Value;
                _fadeCallback = callback;
                _fadeTimer = 0f;

                _fading = true;
            }
        }

        /// <summary>
        /// Calculates the camera view values relative to the current scroll and zoom values.
        /// </summary>
        protected void RefreshCameraView()
        {
            // Adjust the camera view position.
            _cameraView.X = -_scroll.X;
            _cameraView.Y = -_scroll.Y;

            // Adjust the camera view dimensions.
            _cameraView.Width = _cameraRect.Width / _zoom;
            _cameraView.Height = _cameraRect.Height / _zoom;

            if (GenG.DrawMode == GenG.DrawType.Pixel)
            {
                // Adjust the origin and draw position for drawing the camera render target image in the correct position.
                Origin.X = (_cameraRect.Width * 0.5f) / _zoom;
                Origin.Y = (_cameraRect.Height * 0.5f) / _zoom;

                _drawPosition.X = (_cameraRect.X + Origin.X) * _zoom + _position.X;
                _drawPosition.Y = (_cameraRect.Y + Origin.Y) * _zoom + _position.Y;
            }
        }

        /// <summary>
        /// Resets the camera.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _bgColor = null;
            _scroll = Vector2.Zero;
            Rotation = 0f;
            _zoom = _initialZoom;
            _followPosition = Vector2.Zero;
            _followTargets.Clear();

            // Reset the camera effects.
            _shaking = false;
            _shakeCallback = null;
            _flashing = false;
            _flashCallback = null;
            _fading = false;
            _fadeCallback = null;
        }
    }

    public struct CameraView
    {
        /// <summary>
        /// The position of the top-left corner of the camera view.
        /// </summary>
        private Vector2 _position;

        /// <summary>
        /// The width of the camera view;
        /// </summary>
        private float _width;

        /// <summary>
        /// The height of the camera view;
        /// </summary>
        private float _height;

        /// <summary>
        /// Gets or sets the x position of the top-left corner of the camera view.
        /// </summary>
        public float X
        {
            get { return _position.X; }

            set
            {
                _position.X = value;
                Left = _position.X;
            }
        }

        /// <summary>
        /// Gets or sets the y position of the top-left corner of the camera view.
        /// </summary>
        public float Y
        {
            get { return _position.Y; }

            set
            {
                _position.Y = value;
                Top = _position.Y;
            }
        }

        /// <summary>
        /// Gets or sets the width of the camera view.
        /// </summary>
        public float Width
        {
            get { return _width; }

            set
            {
                _width = value;
                Right = _position.X + _width;
            }
        }

        /// <summary>
        /// Gets or sets the height of the camera view.
        /// </summary>
        public float Height
        {
            get { return _height; }

            set
            {
                _height = value;
                Bottom = _position.Y + _height;
            }
        }

        /// <summary>
        /// Gets or sets the position of the left edge of the camera view.
        /// </summary>
        public float Left { get; set; }

        /// <summary>
        /// Gets or sets the position of the right edge of the camera view.
        /// </summary>
        public float Right { get; set; }

        /// <summary>
        /// Gets or sets the position of the top edge of the camera view.
        /// </summary>
        public float Top { get; set; }

        /// <summary>
        /// Gets or sets the position of the bottom edge of the camera view.
        /// </summary>
        public float Bottom { get; set; }
    }
}