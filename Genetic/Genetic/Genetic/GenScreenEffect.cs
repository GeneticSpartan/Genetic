using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genetic
{
    /// <summary>
    /// A screen effect manager used for flashes and fades.
    /// A <c>Texture2D</c> is used to draw the screen effect.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
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
        /// A timer used to manage the flash effect.
        /// </summary>
        protected GenTimer _flashTimer;

        /// <summary>
        /// The current intensity, or starting opacity, of the screen flash.
        /// </summary>
        protected float _flashIntensity;

        /// <summary>
        /// The current color of the screen flash.
        /// </summary>
        protected Color _flashColor;

        /// <summary>
        /// A timer used to manage the fade effect.
        /// </summary>
        protected GenTimer _fadeTimer;

        /// <summary>
        /// The current color of the screen fade.
        /// </summary>
        protected Color _fadeColor;

        /// <summary>
        /// A screen effect manager for creating effects such as screen flashes and fades.
        /// </summary>
        /// <param name="effectRectangle">The rectangle area to draw screen effects.</param>
        public GenScreenEffect(Rectangle effectRectangle)
        {
            EffectRectangle = effectRectangle;
            _fxTexture = new Texture2D(GenG.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _fxTexture.SetData<Color>(new[] { Color.White });
            _flashTimer = new GenTimer(0f, null);
            _flashIntensity = 0f;
            _fadeTimer = new GenTimer(0f, null);
        }

        /// <summary>
        /// Draws the screen effects.
        /// </summary>
        public void Draw()
        {
            if (_flashTimer.IsRunning)
            {
                _fxAlpha = (_flashTimer.Remaining / _flashTimer.Duration) * _flashIntensity;

                GenG.SpriteBatch.Draw(_fxTexture, EffectRectangle, _flashColor * _fxAlpha);
            }

            if (_fadeTimer.IsRunning)
            {
                _fxAlpha = (_fadeTimer.Elapsed / _fadeTimer.Duration);

                GenG.SpriteBatch.Draw(_fxTexture, EffectRectangle, _fadeColor * _fxAlpha);
            }

            if (!GenG.Paused)
            {
                _flashTimer.Update();
                _fadeTimer.Update();
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
            // Apply the flash if the screen is not already flashing, unless force reset is true.
            if (!_flashTimer.IsRunning || forceReset)
            {
                _flashIntensity = intensity;

                // Give the screen flash a default color of white if no other color was passed.
                _flashColor = color.HasValue ? color.Value : Color.White;

                _flashTimer.Duration = duration;
                _flashTimer.Callback = callback;
                _flashTimer.Start();
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
            // Apply the flash if the screen is not already flashing.
            if (!_fadeTimer.IsRunning)
            {
                // Give the screen flash a default color of white if no other color was passed.
                _fadeColor = color.HasValue ? color.Value : Color.Black;

                _fadeTimer.Duration = duration;
                _fadeTimer.Callback = callback;
                _fadeTimer.Start(true);
            }
        }

        /// <summary>
        /// Resets the screen effects.
        /// </summary>
        public void Reset()
        {
            _flashTimer.Reset();
            _fadeTimer.Reset();
        }
    }
}
