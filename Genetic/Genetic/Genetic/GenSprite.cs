using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genetic
{
    /// <summary>
    /// A drawable game sprite extended from <c>GenObject</c>.
    /// Static and animated textures are supported, and several animations can be stored for future playback.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenSprite : GenObject
    {
        /// <summary>
        /// The texture used when drawing the sprite.
        /// </summary>
        protected Texture2D _texture;

        /// <summary>
        /// The position to draw the sprite relative to the origin.
        /// </summary>
        protected Vector2 _drawPosition;

        /// <summary>
        /// The bounding rectangle used to draw a portion of the sprite texture.
        /// </summary>
        protected Rectangle _sourceRect;

        /// <summary>
        /// The color used to tint the sprite. White means no tint.
        /// </summary>
        protected Color _color;

        /// <summary>
        /// The unmodified color used to tint the sprite. White means no tint.
        /// Useful for saving the sprite's color tint while using other color effects.
        /// </summary>
        protected Color _baseColor;

        /// <summary>
        /// The color alpha value of the sprite, a value from 0.0 to 1.0.
        /// </summary>
        protected float _alpha;

        /// <summary>
        /// The unmodified color alpha value of the sprite when drawn, a value from 0.0 to 1.0.
        /// Useful for saving the sprite's alpha while using other color alpha effects.
        /// </summary>
        protected float _baseAlpha;

        /// <summary>
        /// The combination of the color and alpha values of the sprite used when drawn.
        /// </summary>
        protected Color _drawColor;

        /// <summary>
        /// The amount of pixels in the x-axis and y-axis to offset the sprite texture when drawing.
        /// </summary>
        public Vector2 DrawOffset;

        /// <summary>
        /// The horizontal and vertical scales at which the sprite texture will be drawn.
        /// The sprite will be scaled from the origin.
        /// </summary>
        public Vector2 Scale;

        /// <summary>
        /// A flag used to determine if the sprite will be drawn rotated when the object is rotated.
        /// </summary>
        public bool DrawRotated;

        /// <summary>
        /// A list of animations used by the sprite.
        /// </summary>
        public Dictionary<string, GenAnimation> Animations;

        /// <summary>
        /// The name of the animation that is currently playing.
        /// </summary>
        protected string _currentAnimation;

        /// <summary>
        /// The sprite effect used to draw the sprite flipped horizontally or vertically.
        /// </summary>
        protected SpriteEffects _spriteEffect;

        /// <summary>
        /// The x and y factors by which the camera scroll values affect the sprite's draw position within a camera.
        /// A value of 1 will move the sprite with the camera scroll value exactly.
        /// A value of 0 will prevent the sprite from scrolling with the camera, and is useful for HUD elements.
        /// </summary>
        public Vector2 ScrollFactor;

        /// <summary>
        /// The speed of the sprite flicker.
        /// </summary>
        protected float _flickerIntensity;

        /// <summary>
        /// The color to tint the sprite during a flicker.
        /// </summary>
        protected Color _flickerColor;

        /// <summary>
        /// Determines if the sprite flicker will use a pulsing effect.
        /// </summary>
        protected bool _flickerPulsing;

        /// <summary>
        /// The timer used to manage a sprite flicker.
        /// </summary>
        protected GenTimer _flickerTimer;

        /// <summary>
        /// The timer used to manage a sprite fade.
        /// </summary>
        protected GenTimer _fadeTimer;

        /// <summary>
        /// The direction of the sprite fade. 1 = fade-in, -1 = fade-out.
        /// </summary>
        protected int _fadeDirection;

        /// <summary>
        /// Gets the texture used when drawing the sprite.
        /// </summary>
        public Texture2D Texture
        {
            get { return _texture; }
        }

        /// <summary>
        /// Gets the position to draw the sprite relative to the origin.
        /// </summary>
        public Vector2 DrawPosition
        {
            get { return _drawPosition; }
        }

        /// <summary>
        /// Gets the bounding rectangle used to draw a portion of the sprite texture.
        /// </summary>
        public Rectangle SourceRect
        {
            get { return _sourceRect; }
        }

        /// <summary>
        /// Gets or sets the color used to tint the sprite. White means no tint.
        /// </summary>
        public Color Color
        {
            get { return _color; }

            set
            {
                _color = value;
                _baseColor = _color;
            }
        }

        /// <summary>
        /// Gets or sets the color alpha of the sprite when drawn, a value from 0.0 to 1.0.
        /// </summary>
        public float Alpha
        {
            get { return _alpha; }

            set
            {
                _alpha = value;
                _baseAlpha = _alpha;
            }
        }

        /// <summary>
        /// Gets the name of the animation that is currently playing.
        /// </summary>
        public string CurrentAnimation
        {
            get { return _currentAnimation; }
        }

        /// <summary>
        /// Gets the sprite effect used to draw the sprite flipped horizontally or vertically.
        /// </summary>
        public SpriteEffects SpriteEffect
        {
            get { return _spriteEffect; }
        }

        /// <summary>
        /// Gets or sets the direction that the drawn sprite is facing.
        /// </summary>
        public new Direction Facing
        {
            get { return _facing; }

            set
            {
                _facing = value;

                switch (_facing)
                {
                    case Direction.Left:
                        _spriteEffect = SpriteEffects.FlipHorizontally;
                        break;
                    case Direction.Right:
                        _spriteEffect = SpriteEffects.None;
                        break;
                    case Direction.Up:
                        _spriteEffect = SpriteEffects.None;
                        break;
                    case Direction.Down:
                        _spriteEffect = SpriteEffects.FlipVertically;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets if the sprite is currently flickering.
        /// </summary>
        public bool Flickering
        {
            get { return _flickerTimer.IsRunning; }
        }

        /// <summary>
        /// Gets if the sprite is currently fading in/out.
        /// </summary>
        public bool Fading
        {
            get { return _fadeTimer.IsRunning; }
        }

        /// <param name="x">The x position of the sprite.</param>
        /// <param name="y">The y position of the sprite.</param>
        /// <param name="texture">The sprite texture.</param>
        /// <param name="width">The width of the object.</param>
        /// <param name="height">The height of the object.</param>
        public GenSprite(float x = 0, float y = 0, Texture2D texture = null, int width = 1, int height = 1)
            : base(x, y, width, height)
        {
            _sourceRect = new Rectangle(0, 0, width, height);
            _texture = (texture != null) ? LoadTexture(texture, true) : null;
            Color = Color.White;
            Alpha = 1f;
            _drawColor = _color * _alpha;
            Scale = Vector2.One;
            DrawRotated = true;
            Animations = new Dictionary<string, GenAnimation>();
            _currentAnimation = null;
            _spriteEffect = SpriteEffects.None;
            ScrollFactor = Vector2.One;
            _flickerIntensity = 0f;
            _flickerColor = Color.White;
            _flickerPulsing = false;
            _flickerIntensity = 0f;
            _flickerTimer = new GenTimer(0f, null, true);
            _flickerColor = Color.White;
            _flickerPulsing = false;
            _fadeTimer = new GenTimer(0f, null, true);
            _fadeDirection = 1;
        }

        /// <summary>
        /// Calls <c>Update</c> on this sprite's object.
        /// Calls <c>Update</c> on the current animation, and manages sprite flicker.
        /// Override this method to add additional update logic.
        /// </summary>
        public override void Update()
        {
            base.Update();

            // Update the currently playing animation.
            if (_currentAnimation != null)
                Animations[_currentAnimation].Update();

            _alpha = _baseAlpha;

            // Manage the sprite flicker effect.
            if (_flickerTimer.IsRunning)
            {
                _flickerTimer.Update();

                if (!_flickerTimer.IsRunning)
                {
                    // Set the color and alpha back to their original values if the sprite flicker is no longer running.
                    _color = _baseColor;
                    _alpha = _baseAlpha;
                }
                else
                {
                    _alpha *= GenU.SineWave(0.5f, _flickerIntensity, 0.5f);

                    // If the sprite flicker is not pulsing, round the alpha value to 0 or 1 for a solid flicker effect.
                    if (!_flickerPulsing)
                        _alpha = (float)Math.Round(_alpha);

                    _color = _flickerColor;
                }
            }
            
            // Manage the sprite fade in/out effect.
            if (_fadeTimer.IsRunning)
            {
                _fadeTimer.Update();

                // If the sprite fade is no longer running, Keep the alpha color value within 0 and the base alpha.
                // Otherwise, fade the alpha color value in or out.
                if (!_fadeTimer.IsRunning)
                    _alpha = (_fadeDirection == 1) ? _baseAlpha : 0f;
                else
                    _alpha *= (_fadeDirection == 1) ? (_fadeTimer.Elapsed / _fadeTimer.Duration) : (_fadeTimer.Remaining / _fadeTimer.Duration);                
            }
        }

        /// <summary>
        /// Handles post-update logic for the sprite.
        /// </summary>
        public override void PostUpdate()
        {
            base.PostUpdate();

            // Update the draw color using the current color and alpha values.
            _drawColor = _color * _alpha;
        }

        /// <summary>
        /// Updates the draw position and rotation, and draws the sprite to the camera.
        /// </summary>
        public override void Draw()
        {
            UpdateDrawPosition();

            if (_texture != null)
            {
                GenG.SpriteBatch.Draw(
                    _texture,
                    _drawPosition,
                    (_currentAnimation == null) ? _sourceRect : Animations[_currentAnimation].FrameRect,
                    _drawColor,
                    (DrawRotated) ? _rotation : 0f,
                    _origin,
                    Scale,
                    _spriteEffect,
                    0f);
            }
        }

        /// <summary>
        /// Updates the x and y positions of where the top-left corner of a sprite will be drawn.
        /// Calculates the draw position from the position, origin, offset, camera scroll, and scroll factor values.
        /// </summary>
        /// <returns></returns>
        protected void UpdateDrawPosition()
        {
            _drawPosition.X = _originPosition.X + DrawOffset.X - GenG.CurrentCamera.ScrollX + (GenG.CurrentCamera.ScrollX * ScrollFactor.X);
            _drawPosition.Y = _originPosition.Y + DrawOffset.Y - GenG.CurrentCamera.ScrollY + (GenG.CurrentCamera.ScrollY * ScrollFactor.Y);

            if (GenG.DrawMode == GenG.DrawType.Pixel)
            {
                _drawPosition.X = (int)_drawPosition.X;
                _drawPosition.Y = (int)_drawPosition.Y;
            }
        }

        /// <summary>
        /// Sets the sprite's image to a loaded texture.
        /// Sets the source bounding rectangle according to the texture's dimensions.
        /// </summary>
        /// <param name="texture">The sprite texture.</param>
        /// <param name="centerOrigin">Determines if the sprite origin should be re-centered to the new source rectangle.</param>
        /// <returns>The Texture2D that was given.</returns>
        public Texture2D LoadTexture(Texture2D texture, bool centerOrigin = true)
        {
            _texture = texture;

            SetSourceRect(0, 0, _texture.Width, _texture.Height, centerOrigin);

            return _texture;
        }

        /// <summary>
        /// Creates a solid color texture to use as the sprite's image.
        /// Sets the source rectangle according to the given width and height values.
        /// A value of 0 for either the width or height will cause the source rectangle to use the existing sprite dimensions.
        /// </summary>
        /// <param name="color">The color of the sprite. Defaults to white if set to null.</param>
        /// <param name="width">The width to display the texture at. A value of 0 will use the existing sprite width.</param>
        /// <param name="height">The height to display the texture at. A value of 0 will use the existing sprite height.</param>
        /// <param name="centerOrigin">Determines if the sprite origin should be re-centered to the new source rectangle.</param>
        /// <returns>The newly created Texture2D.</returns>
        public Texture2D MakeTexture(Color? color = null, int width = 0, int height = 0, bool centerOrigin = true)
        {
            _texture = GenU.MakeTexture(color, width, height);

            // Set the source rectangle to the existing dimensions if the given width and height values are 0.
            // This allows the source rectangle to keep the width and height values given when the sprite was created.
            SetSourceRect(
                0, 
                0, 
                (width == 0) ? _sourceRect.Width : width, 
                (height == 0) ? _sourceRect.Height : height, 
                centerOrigin);

            return _texture;
        }

        /// <summary>
        /// Sets the bounding rectangle used to draw a portion of the sprite texture.
        /// </summary>
        /// <param name="x">The x position of the bounding rectangle.</param>
        /// <param name="y">The y position of the bounding rectangle.</param>
        /// <param name="width">The width of the bounding rectangle.</param>
        /// <param name="height">The height of the bounding rectangle.</param>
        /// <param name="centerOrigin">Determines if the sprite origin should be re-centered to the new source rectangle.</param>
        public void SetSourceRect(int x, int y, int width, int height, bool centerOrigin = true)
        {
            _sourceRect.X = x;
            _sourceRect.Y = y;
            _sourceRect.Width = width;
            _sourceRect.Height = height;

            if (centerOrigin)
                CenterOrigin(true);
        }

        /// <summary>
        /// Places the origin at the center of the sprite image or the bounding box.
        /// </summary>
        /// <param name="useSprite">A flag used to set the origin at the center of the sprite image instead of the bounding box.</param>
        public void CenterOrigin(bool useSprite = true)
        {
            if (useSprite)
                SetOrigin(_sourceRect.Width * 0.5f, _sourceRect.Height * 0.5f);
            else
                base.CenterOrigin();
        }

        /// <summary>
        /// Creates a playable animation for the sprite.
        /// </summary>
        /// <param name="name">The name associated with the animation, and will be used to play it.</param>
        /// <param name="frameWidth">The width of each animation frame.</param>
        /// <param name="frameHeight">The height of each animation frame.</param>
        /// <param name="frames">The sequence of frame numbers of the animation. A value of null will play all frames of the animation texture.</param>
        /// <param name="fps">The speed, in frames per second, of the animation.</param>
        /// <param name="isLooped">Determines whether the animation is looping or not.</param>
        /// <param name="frameBuffer">The amount of space, in pixels, between each of the animation frames.</param>
        /// <returns>The name associated with the animation.</returns>
        public string AddAnimation(string name, int frameWidth, int frameHeight, int[] frames = null, int fps = 12, bool isLooped = true, int frameBuffer = 0)
        {
            Animations.Add(name, new GenAnimation(this, frameWidth, frameHeight, frames, fps, isLooped, frameBuffer));

            return name;
        }

        /// <summary>
        /// Plays a sprite animation.
        /// </summary>
        /// <param name="name">The name associated with the animation.</param>
        /// <param name="forceReset">Determines whether the animation should start playing from the first frame, or continue from the last played frame.</param>
        public void Play(string name, bool forceReset = true)
        {
            if (Animations.ContainsKey(name))
            {
                _currentAnimation = name;
                Animations[name].Play(forceReset);
            }
        }

        /// <summary>
        /// Give the sprite a flicker effect.
        /// </summary>
        /// <param name="intensity">The speed of the sprite flicker.</param>
        /// <param name="duration">The duration of the sprite flicker, in seconds.</param>
        /// <param name="color">The color to tint the sprite during the flicker. A value of null will use the sprite's current color.</param>
        /// <param name="pulsing">Determines if the sprite flicker will use a pulsing effect.</param>
        /// <param name="callback">The method that will be invoked after the sprite flicker has finished.</param>
        public void Flicker(float intensity = 40f, float duration = 1f, Color? color = null, bool pulsing = false, Action callback = null)
        {
            // Apply the flicker if the sprite is not already flickering.
            if (!_flickerTimer.IsRunning)
            {
                _flickerIntensity = intensity;
                _flickerPulsing = pulsing;

                // Set the flicker color to the sprite's current color if the given value is null.
                _flickerColor = color.HasValue ? color.Value : _color;

                _flickerTimer.Duration = duration;
                _flickerTimer.Callback = callback;
                _flickerTimer.Start(true);
            }
        }

        /// <summary>
        /// Fades the sprite in from an alpha color value of 0 over the given duration.
        /// </summary>
        /// <param name="duration">The duration of the sprite fade-in, in seconds.</param>
        /// <param name="callback">The method that will invoke when the sprite fade-in has completed.</param>
        public void FadeIn(float duration = 1f, Action callback = null)
        {
            Fade(1, duration, callback);
        }

        /// <summary>
        /// Fades the sprite out to an alpha color value of 0 over the given duration.
        /// </summary>
        /// <param name="duration">The duration of the sprite fade-out, in seconds.</param>
        /// <param name="callback">The method that will invoke when the sprite fade-out has completed.</param>
        public void FadeOut(float duration = 1f, Action callback = null)
        {
            Fade(-1, duration, callback);
        }

        /// <summary>
        /// Fades the sprite in or out by adjusting its alpha color value over the given duration.
        /// </summary>
        /// <param name="fadeDirection">The direction of the sprite fade. 1 = fade-in, -1 = fade-out.</param>
        /// <param name="duration">The duration of the sprite fade, in seconds.</param>
        /// <param name="callback">The method that will invoke when the sprite fade has completed.</param>
        protected void Fade(int fadeDirection, float duration, Action callback)
        {
            if (!_fadeTimer.IsRunning)
            {
                _fadeDirection = fadeDirection;

                _fadeTimer.Duration = duration;
                _fadeTimer.Callback = callback;
                _fadeTimer.Start(true);
            }
        }
    }
}