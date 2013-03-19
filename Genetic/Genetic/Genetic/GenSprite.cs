using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genetic
{
    public class GenSprite : GenObject
    {
        /// <summary>
        /// Dictates whether the sprite should be drawn.
        /// </summary>
        public bool Visible = true;

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
        public Color Color;

        /// <summary>
        /// The rotation of the sprite in radians.
        /// </summary>
        protected float _rotation;

        /// <summary>
        /// The speed of the sprite's rotation in degrees per second.
        /// </summary>
        public float RotationSpeed;

        /// <summary>
        /// The origin, relative to the position, that the sprite texture will rotate around.
        /// </summary>
        public Vector2 Origin;

        /// <summary>
        /// A list of animations used by the sprite.
        /// </summary>
        protected Dictionary<string, GenAnimation> _animations;

        /// <summary>
        /// The name of the animation that is currently playing.
        /// </summary>
        protected string _currentAnimation = null;

        /// <summary>
        /// The sprite effect used to draw the sprite flipped horizontally or vertically.
        /// </summary>
        protected SpriteEffects _spriteEffect = SpriteEffects.None;

        /// <summary>
        /// The factor by which the camera scroll values effect the sprite's position.
        /// A default value of 1.0 means that the sprite will move with the camera scroll values exactly.
        /// </summary>
        public float ScrollFactor = 1.0f;

        /// <summary>
        /// Gets the texture used when drawing the sprite.
        /// </summary>
        public Texture2D Texture
        {
            get { return _texture; }
        }

        /// <summary>
        /// Get or sets the rotation of the sprite in degrees.
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
                        _spriteEffect = SpriteEffects.FlipVertically;
                        break;
                    case Direction.Down:
                        _spriteEffect = SpriteEffects.None;
                        break;
                }
            }
        }

        /// <param name="x">The x position of the sprite.</param>
        /// <param name="y">The y position of the sprite.</param>
        /// <param name="textureFile">The sprite texture file to load.</param>
        /// <param name="width">The width of the object.</param>
        /// <param name="height">The height of the object.</param>
        public GenSprite(float x = 0, float y = 0, string textureFile = null, int width = 0, int height = 0)
            : base(x, y, width, height)
        {
            if (textureFile != null)
                LoadTexture(textureFile);
            else
                _texture = null;

            _sourceRect = new Rectangle(0, 0, width, height);
            Color = Color.White;
            _rotation = 0f;
            RotationSpeed = 0f;
            Origin = new Vector2(width / 2, height / 2);
            _animations = new Dictionary<string, GenAnimation>();
        }

        public override void Update()
        {
            base.Update();

            Rotation += RotationSpeed * GenG.PhysicsTimeStep;

            // Update the currently playing animation.
            if (_currentAnimation != null)
                _animations[_currentAnimation].Update();
        }

        /// <summary>
        /// Draws the sprite to the camera.
        /// </summary>
        public override void Draw()
        {
            _drawPosition.X = _positionRect.X + Origin.X - GenG.CurrentCamera.ScrollX + (GenG.CurrentCamera.ScrollX * ScrollFactor);
            _drawPosition.Y = _positionRect.Y + Origin.Y - GenG.CurrentCamera.ScrollY + (GenG.CurrentCamera.ScrollY * ScrollFactor);

            if (Visible && (_texture != null))
            {
                if (_currentAnimation == null)
                    GenG.SpriteBatch.Draw(_texture, _drawPosition, _sourceRect, Color, _rotation, Origin, 1, _spriteEffect, 0);
                else
                    GenG.SpriteBatch.Draw(_texture, _drawPosition, _animations[_currentAnimation].FrameRect, Color, _rotation, Origin, 1, _spriteEffect, 0);
            }

            base.Draw();
        }

        /// <summary>
        /// Loads a texture file to use as the sprite's image.
        /// Sets the source bounding rectangle according to the new texture.
        /// </summary>
        /// <param name="textureFile">The sprite texture file to load.</param>
        /// <param name="centerOrigin">Determines if the sprite origin should be re-centered.</param>
        /// <returns>The Texture2D created from loading the texture file.</returns>
        public Texture2D LoadTexture(string textureFile, bool centerOrigin = true)
        {
            _texture = GenG.Content.Load<Texture2D>(textureFile);

            SetSourceRect(0, 0, _texture.Width, _texture.Height, centerOrigin);

            return _texture;
        }

        /// <summary>
        /// Sets the sprite's image to an existing texture.
        /// Sets the source bounding rectangle according to the texture.
        /// </summary>
        /// <param name="textureFile">The sprite texture.</param>
        /// <param name="centerOrigin">Determines if the sprite origin should be re-centered.</param>
        /// <returns>The Texture2D that was given.</returns>
        public Texture2D LoadTexture(Texture2D texture, bool centerOrigin = true)
        {
            _texture = texture;

            SetSourceRect(0, 0, _texture.Width, _texture.Height, centerOrigin);

            return _texture;
        }

        /// <summary>
        /// Creates a solid color 1 x 1 texture to use as the sprite's image.
        /// Sets the source rectangle according to the given width and height values.
        /// A value of 0 for either the width or height will cause the source rectangle to use the existing sprite dimensions.
        /// </summary>
        /// <param name="color">The color of the sprite. Defaults to white if set to null.</param>
        /// <param name="width">The width to display the texture at. A value of 0 will use the existing sprite width.</param>
        /// <param name="height">The height to display the texture at. A value of 0 will use the existing sprite height.</param>
        /// <param name="centerOrigin">Determines if the sprite origin should be re-centered.</param>
        /// <returns>The newly created Texture2D.</returns>
        public Texture2D MakeTexture(Color? color = null, int width = 0, int height = 0, bool centerOrigin = true)
        {
            color = color.HasValue ? color.Value : Color.White;

            _texture = GenU.MakeTexture(color.Value, width, height);

            // Set the source rectangle to the existing dimensions if the given width and height values are 0.
            // This allows the source rectangle to keep the width and height values given when the sprite was created.
            if ((width == 0) || (height == 0))
                SetSourceRect(0, 0, _sourceRect.Width, _sourceRect.Height, centerOrigin);
            else
                SetSourceRect(0, 0, width, height, centerOrigin);

            return _texture;
        }

        /// <summary>
        /// Sets the bounding rectangle used to draw a portion of the sprite texture.
        /// </summary>
        /// <param name="x">The x position of the bounding rectangle.</param>
        /// <param name="y">The y position of the bounding rectangle.</param>
        /// <param name="width">The width of the bounding rectangle.</param>
        /// <param name="height">The height of the bounding rectangle.</param>
        /// <param name="centerOrigin">Determines if the sprite origin should be re-centered.</param>
        public void SetSourceRect(int x, int y, int width, int height, bool centerOrigin = true)
        {
            _sourceRect.X = x;
            _sourceRect.Y = y;
            _sourceRect.Width = width;
            _sourceRect.Height = height;

            if (centerOrigin)
                CenterOrigin();
        }

        public void CenterOrigin()
        {
            Origin.X = _sourceRect.Width / 2;
            Origin.Y = _sourceRect.Height / 2;
        }

        /// <summary>
        /// Creates a playable animation for the sprite.
        /// </summary>
        /// <param name="name">The name associated with the animation, and will be used to play it.</param>
        /// <param name="frameWidth">The width of each animation frame.</param>
        /// <param name="frameHeight">The height of each animation frame.</param>
        /// <param name="frames">The sequence of frame numbers of the animation. A value of null will play all frames of the animation texture.</param>
        /// <param name="fps">The speed, in frames per second, of the animation.</param>
        /// <param name="frameBuffer">The amount of space, in pixels, between each of the animation frames.</param>
        public void AddAnimation(string name, int frameWidth, int frameHeight, int[] frames = null, int fps = 12, int frameBuffer = 0)
        {
            _animations.Add(name, new GenAnimation(this, frameWidth, frameHeight, frames, fps, frameBuffer));
        }

        /// <summary>
        /// Plays a sprite animation.
        /// </summary>
        /// <param name="name">The name associated with the animation.</param>
        public void Play(string name)
        {
            if (_animations.ContainsKey(name))
                _currentAnimation = name;
        }
    }
}