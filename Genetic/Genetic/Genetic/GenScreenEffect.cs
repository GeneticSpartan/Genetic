using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genetic
{
    public class GenScreenEffect
    {
        /// <summary>
        /// The rectangle area to draw screen effects.
        /// </summary>
        public Rectangle EffectRectangle;

        /// <summary>
        /// The texture used to draw screen effects such as flash and fade.
        /// </summary>
        protected Texture2D _fxTexture;

        /// <summary>
        /// The alpha used when drawing a screen effect.
        /// </summary>
        protected float _fxAlpha;

        /// <summary>
        /// Determines if the screen is currently flashing.
        /// </summary>
        protected bool _flashing;

        /// <summary>
        /// The current intensity, or starting opacity, of the screen flash.
        /// </summary>
        protected float _flashIntensity;

        /// <summary>
        /// The current color of the screen flash.
        /// </summary>
        protected Color _flashColor;

        /// <summary>
        /// The current duration of the screen flash.
        /// </summary>
        protected float _flashDuration;

        /// <summary>
        /// The amount of time since the screen flash started, in seconds.
        /// </summary>
        protected float _flashTimer;

        /// <summary>
        /// The callback function that will invoke after the screen flash has finished.
        /// </summary>
        protected Action _flashCallback;

        /// <summary>
        /// Determines if the screen is currently fading.
        /// </summary>
        protected bool _fading;

        /// <summary>
        /// The current color of the screen fade.
        /// </summary>
        protected Color _fadeColor;

        /// <summary>
        /// The current duration of the screen fade.
        /// </summary>
        protected float _fadeDuration;

        /// <summary>
        /// The amount of time since the screen fade started, in seconds.
        /// </summary>
        protected float _fadeTimer;

        /// <summary>
        /// The callback function that will invoke after the screen fade has finished.
        /// </summary>
        protected Action _fadeCallback;

        /// <summary>
        /// A screen effect manager for creating effects such as screen flashes and fades.
        /// </summary>
        /// <param name="effectRectangle">The rectangle area to draw screen effects.</param>
        public GenScreenEffect(Rectangle effectRectangle)
        {
            EffectRectangle = effectRectangle;
            _fxTexture = new Texture2D(GenG.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _fxTexture.SetData<Color>(new[] { Color.White });
            _flashing = false;
            _flashIntensity = 0f;
            _flashDuration = 0f;
            _flashTimer = 0f;
            _flashCallback = null;
            _fading = false;
            _fadeDuration = 0f;
            _fadeTimer = 0f;
            _fadeCallback = null;
        }

        /// <summary>
        /// Draws the screen effects.
        /// </summary>
        public void Draw()
        {
            if (_flashing)
            {
                if (_flashTimer < _flashDuration)
                {
                    _fxAlpha = ((_flashDuration - _flashTimer) / _flashDuration) * _flashIntensity;

                    GenG.SpriteBatch.Draw(_fxTexture, EffectRectangle, _flashColor * _fxAlpha);

                    if (!GenG.Paused)
                        _flashTimer += GenG.ScaleTimeStep;
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

                    GenG.SpriteBatch.Draw(_fxTexture, EffectRectangle, _fadeColor * _fxAlpha);

                    if (!GenG.Paused)
                        _fadeTimer += GenG.ScaleTimeStep;

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
        /// Give the screen a flash effect.
        /// </summary>
        /// <param name="intensity">The intensity, or starting opacity, of the screen flash.</param>
        /// <param name="duration">The duration of the screen flash, in seconds.</param>
        /// <param name="color">The color of the screen flash. Use null to default to white.</param>
        /// <param name="forceReset">A flag used to determine if the flash will reset any current screen flash.</param>
        /// <param name="callback">The method that will be invoked after the screen flash has finished.</param>
        public void Flash(float intensity = 1f, float duration = 1f, Color? color = null, bool forceReset = false, Action callback = null)
        {
            // Give the screen flash a default color of white if no other color was passed.
            color = color.HasValue ? color.Value : Color.White;

            // Apply the flash if the screen is not already flashing, unless force reset is true.
            if (forceReset || !_flashing)
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
        /// Give the screen a fade effect.
        /// </summary>
        /// <param name="duration">The duration of the screen fade, in seconds.</param>
        /// <param name="color">The color of the screen fade. Use null to default to black.</param>
        /// <param name="callback">The method that will be invoked after the screen fade has finished.</param>
        public void Fade(float duration = 1f, Color? color = null, Action callback = null)
        {
            // Give the screen flash a default color of white if no other color was passed.
            color = color.HasValue ? color.Value : Color.Black;

            // Apply the flash if the screen is not already flashing.
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
        /// Resets the screen effects.
        /// </summary>
        public void Reset()
        {
            _flashing = false;
            _flashCallback = null;
            _fading = false;
            _fadeCallback = null;
        }
    }
}
