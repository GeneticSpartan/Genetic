using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genetic.Gui
{
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
        /// The color tint of the progress bar sprite as the progress bar value gets closer to the minimum value.
        /// </summary>
        public Color MinColor;

        /// <summary>
        /// The color tint of the progress bar sprite as the progress bar value gets closer to the maxinum value.
        /// </summary>
        public Color MaxColor;

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

        public GenProgressBar(float x = 0, float y = 0, Texture2D texture = null, int width = 100, int height = 10)
            : base(x, y, texture, width, height)
        {
            _progressRect = new Rectangle(0, 0, width, height);
            MinValue = 0;
            MaxValue = 100;
            _value = 100;
            MinColor = Color.White;
            MaxColor = Color.White;
            MinCallback = null;
            MaxCallback = null;
        }

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

            // Interpolate the progress bar color relative to the current progress bar value.
            _color = Color.Lerp(MinColor, MaxColor, lerp);
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
