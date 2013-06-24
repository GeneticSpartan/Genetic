using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genetic.Gui
{
    /// <summary>
    /// A graphical progress bar meter.
    /// Extends from <c>GenSprite</c>, so the progress bar can utilize features of a <c>GenSprite</c> such as animations.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenProgressBar : GenSprite
    {
        /// <summary>
        /// The rectangle used to draw a portion of the progress bar sprite relative to its current value.
        /// </summary>
        protected Rectangle _progressRect;

        /// <summary>
        /// The minimum value of the progress bar.
        /// </summary>
        public float MinValue;

        /// <summary>
        /// The maximum value of the progress bar.
        /// </summary>
        public float MaxValue;

        /// <summary>
        /// The current value of the progress bar.
        /// </summary>
        protected float _value;

        /// <summary>
        /// A list of colors used to tint the progress bar relative to its current value.
        /// The order of the colors proceeds from min to max.
        /// </summary>
        public List<Color> Colors;

        /// <summary>
        /// A flag used to determine if the progress bar colors should interpolate as its value changes.
        /// </summary>
        public bool BlendColors;

        /// <summary>
        /// The method that will invoke when the progress bar value reaches the minimum value.
        /// </summary>
        public Action MinCallback;

        /// <summary>
        /// The method that will invoke when the progress bar value reaches the maximum value.
        /// </summary>
        public Action MaxCallback;

        /// <summary>
        /// Gets or sets the current value of the progress bar.
        /// Invokes the respective callback method if the current value of the progress bar reaches the minimum or maximum values.
        /// </summary>
        public float Value
        {
            get { return _value; }

            set
            {
                // If the current value of the progress bar reaches the minimum or maximum values, invoke the respective callback method.
                if ((MinCallback != null) && (value <= MinValue) && (_value > MinValue))
                    MinCallback.Invoke();
                else if ((MaxCallback != null) && (value >= MaxValue) && (_value < MaxValue))
                    MaxCallback.Invoke();

                _value = MathHelper.Clamp(value, MinValue, MaxValue);
            }
        }

        /// <summary>
        /// A graphical progress bar meter.
        /// </summary>
        /// <param name="x">The x position of the top-left corner of the progress bar.</param>
        /// <param name="y">The y position of the top-left corner of the progress bar.</param>
        /// <param name="texture">The texture used as the progress bar fill.</param>
        /// <param name="width">The width of the progress bar.</param>
        /// <param name="height">The height of the progress bar.</param>
        public GenProgressBar(float x = 0, float y = 0, Texture2D texture = null, int width = 100, int height = 10)
            : base(x, y, texture, width, height)
        {
            _progressRect = new Rectangle(0, 0, width, height);
            MinValue = 0;
            MaxValue = 100;
            _value = 100;
            Colors = new List<Color>();
            BlendColors = true;
            MinCallback = null;
            MaxCallback = null;
        }

        /// <summary>
        /// Updates the progress bar display using the current progress value.
        /// </summary>
        public override void Update()
        {
            base.Update();

            float lerp = _value / (MaxValue - MinValue);

            if (_currentAnimation == null)
            {
                _progressRect.X = _sourceRect.X;
                _progressRect.Y = _sourceRect.Y;
                _progressRect.Width = (int)(_sourceRect.Width * lerp);
                _progressRect.Height = _sourceRect.Height;
            }
            else
            {
                _progressRect.X = Animations[_currentAnimation].FrameRect.X;
                _progressRect.Y = Animations[_currentAnimation].FrameRect.Y;
                _progressRect.Width = (int)(Animations[_currentAnimation].FrameRect.Width * lerp);
                _progressRect.Height = Animations[_currentAnimation].FrameRect.Height;
            }

            int lastColorIndex = Colors.Count - 1;

            if (Colors.Count > 0)
            {
                if (BlendColors)
                {
                    // Interpolate the progress bar colors relative to the current progress bar value.
                    int currentColorIndex = (int)(lerp * lastColorIndex);
                    float colorLerp = (lerp - ((1f / lastColorIndex) * currentColorIndex)) / (1f / lastColorIndex);

                    if (currentColorIndex < lastColorIndex)
                        _color = Color.Lerp(Colors[currentColorIndex], Colors[currentColorIndex + 1], colorLerp);
                    else
                        _color = Colors[lastColorIndex];
                }
                else
                {
                    // Set the progress bar color relative to the current progress bar value.
                    _color = Colors[Math.Min((int)(lerp * Colors.Count), lastColorIndex)];
                }
            }
        }

        /// <summary>
        /// Draws the progress bar.
        /// </summary>
        public override void Draw()
        {
            UpdateDrawPosition();

            if (_texture != null)
                GenG.SpriteBatch.Draw(_texture, _drawPosition, _progressRect, _color, _rotation, _origin, Scale, _spriteEffect, 0);
        }
    }
}
