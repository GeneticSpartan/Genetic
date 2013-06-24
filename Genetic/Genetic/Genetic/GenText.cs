using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genetic
{
    /// <summary>
    /// An extended <c>GenSprite</c> used for drawing text to the screen.
    /// Provides features such as text alignment and a drop-shadow.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenText : GenSprite
    {
        /// <summary>
        /// The horizontal alignment of the text.
        /// </summary>
        public enum TextAlign {
            /// <summary>
            /// The body of text is horizontally aligned to the left of the bounding box.
            /// </summary>
            Left,

            /// <summary>
            /// The body of text is horizontally aligned to the right of the bounding box.
            /// </summary>
            Right,

            /// <summary>
            /// The body of text is horizontally aligned to the center of the bounding box.
            /// </summary>
            Center
        };

        /// <summary>
        /// The text that is displayed.
        /// </summary>
        protected string _text;

        /// <summary>
        /// The sprite font used to draw the text.
        /// </summary>
        public SpriteFont Font;

        /// <summary>
        /// The width and height of the text string using the current sprite font.
        /// </summary>
        private Vector2 _textMeasure;

        /// <summary>
        /// The horizontal alignment of the text relative to the bounding rectangle.
        /// </summary>
        public TextAlign TextAlignment;

        /// <summary>
        /// The origin of the text in relation to the origin of the game object.
        /// </summary>
        protected Vector2 _textOrigin;

        /// <summary>
        /// Determines whether the text shadow should be drawn.
        /// </summary>
        public bool HasShadow;

        /// <summary>
        /// The color of the text shadow.
        /// </summary>
        public Color ShadowColor;

        /// <summary>
        /// The position to draw the text shadow relative to the text position.
        /// Default is 2 pixels lower on the y-axis.
        /// </summary>
        public Vector2 ShadowPosition;

        /// <summary>
        /// Gets or sets the text that is displayed.
        /// </summary>
        public string Text
        {
            get { return _text; }

            set
            {
                _text = value;

                // Recalculate the width and height of the text string using the current sprite font.
                _textMeasure = Font.MeasureString(_text);
            }
        }

        /// <summary>
        /// Gets the width and height of the current text string, using the size of the current sprite font.
        /// </summary>
        public Vector2 Measure
        {
            get { return _textMeasure; }
        }

        /// <summary>
        /// Creates a displayable text game object.
        /// </summary>
        /// <param name="text">The text that is displayed.</param>
        /// <param name="x">The x position of the object.</param>
        /// <param name="y">The y position of the object.</param>
        /// <param name="width">The width of the bounding rectangle.</param>
        /// <param name="height">The height of the bounding rectangle.</param>
        public GenText(string text = null, float x = 0, float y = 0, int width = 1, int height = 1)
            : base(x, y, null, width, height)
        {
            // Set the font type of the text to the default font.
            Font = GenG.Font;

            Text = (text == null) ? String.Empty : text;
            TextAlignment = TextAlign.Left;
            HasShadow = false;
            ShadowColor = Color.Black;
            ShadowPosition = new Vector2(0f, 2f);
        }

        /// <summary>
        /// Updates the text origin relative to the its object origin to correct text alignment.
        /// </summary>
        public override void Update()
        {
            base.Update();

            // Calculate the text origin relative to the game object origin to adjust for alignment settings.
            if (TextAlignment == TextAlign.Right)
                _textOrigin.X = (_origin.X - _bounds.Width) / Scale.X + _textMeasure.X;
            else if (TextAlignment == TextAlign.Center)
                _textOrigin.X = _origin.X - (_bounds.HalfWidth - (_textMeasure.X * 0.5f));
            else
                _textOrigin.X = _origin.X / Scale.X;

            _textOrigin.Y = _origin.Y / Scale.Y;
        }

        /// <summary>
        /// Draws the text sprite to the camera.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            // Draw the text shadow.
            if (HasShadow)
            {
                GenG.SpriteBatch.DrawString(
                    Font, 
                    _text, 
                    _drawPosition + ShadowPosition, 
                    ShadowColor * _alpha, 
                    (DrawRotated) ? _rotation : 0f, 
                    _textOrigin, 
                    Scale,
                    SpriteEffects.None, 
                    0f);
            }

            // Draw the text.
            GenG.SpriteBatch.DrawString(
                Font, 
                _text, 
                _drawPosition, 
                _color * _alpha, 
                (DrawRotated) ? _rotation : 0f, 
                _textOrigin, 
                Scale, 
                SpriteEffects.None, 
                0f);
        }
    }
}