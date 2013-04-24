using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genetic
{
    public class GenText : GenSprite
    {
        public enum TextAlign { LEFT, RIGHT, CENTER };

        /// <summary>
        /// The text that is displayed.
        /// </summary>
        protected string _text;

        /// <summary>
        /// The sprite font used to draw the text.
        /// </summary>
        private static SpriteFont _font = GenG.Content.Load<SpriteFont>("Nokia");

        /// <summary>
        /// The width and height of the text string using the sprite font.
        /// </summary>
        private Vector2 _textMeasure;

        /// <summary>
        /// The horizontal alignment of the text relative to the bounding rectangle.
        /// </summary>
        public TextAlign TextAlignment = TextAlign.LEFT;

        /// <summary>
        /// The origin of the text in relation to the origin of the game object.
        /// </summary>
        protected Vector2 _textOrigin = Vector2.Zero;

        /// <summary>
        /// The scale that the font sprite will be drawn at.
        /// Setting the font size will adjust this number automatically.
        /// </summary>
        protected float _fontScale = 1f;

        /// <summary>
        /// Determines whether the text shadow should be drawn.
        /// </summary>
        public bool HasShadow = false;

        /// <summary>
        /// The color of the text shadow.
        /// </summary>
        public Color ShadowColor = Color.Black;

        /// <summary>
        /// The position to draw the text shadow relative to the text position.
        /// Default is 2 pixels lower on the y-axis.
        /// </summary>
        public Vector2 ShadowPosition = new Vector2(0, 2);

        /// <summary>
        /// Gets or sets the font size of the text.
        /// Calculates the font scale from the given font size relative to the default font size of 12.
        /// </summary>
        public float FontSize
        {
            get { return 12 * _fontScale; }

            set { _fontScale = value / 12; }
        }

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
                _textMeasure = _font.MeasureString(_text);
            }
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
            if (text == null)
                Text = String.Empty;
            else
                Text = text;
        }

        public override void Update()
        {
            base.Update();

            // Calculate the text origin relative to the game object origin to adjust for alignment settings.
            if (TextAlignment == TextAlign.RIGHT)
                _textOrigin.X = (Origin.X - Width) * (1 / _fontScale) + _textMeasure.X;
            else if (TextAlignment == TextAlign.CENTER)
                _textOrigin.X = Origin.X - ((Width * 0.5f) - (_textMeasure.X * 0.5f));
            else
                _textOrigin.X = Origin.X * (1 / _fontScale);

            _textOrigin.Y = Origin.Y * (1 / _fontScale);
        }

        /// <summary>
        /// Draws the sprite to the camera.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            // Draw the text shadow.
            if (HasShadow)
                GenG.SpriteBatch.DrawString(_font, _text, _drawPosition + ShadowPosition, ShadowColor, _rotation, _textOrigin, _fontScale, SpriteEffects.None, 0);

            GenG.SpriteBatch.DrawString(_font, _text, _drawPosition, Color, _rotation, _textOrigin, _fontScale, SpriteEffects.None, 0);
        }
    }
}