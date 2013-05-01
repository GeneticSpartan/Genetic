using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genetic.Particles
{
    public class GenParticle : GenSprite
    {
        /// <summary>
        /// The amount of time, in seconds, that the particle will last after being emitted.
        /// </summary>
        public float Lifetime;

        /// <summary>
        /// The amount of time, in seconds, that has elapsed since the particle was emitted.
        /// </summary>
        protected float _lifeTimer;

        /// <summary>
        /// The starting color of the particle when it is emitted.
        /// </summary>
        public Color StartColor;

        /// <summary>
        /// The ending color of the particle when it reaches the end of its lifetime.
        /// </summary>
        public Color EndColor;

        /// <summary>
        /// The starting color alpha of the particle when it is emitted.
        /// </summary>
        public float StartAlpha;

        /// <summary>
        /// The ending color alpha of the particle when it reaches the end of its lifetime.
        /// </summary>
        public float EndAlpha;

        /// <summary>
        /// The starting scale of the particle sprite when it is emitted.
        /// </summary>
        public float StartScale;

        /// <summary>
        /// The ending scale of the particle sprite when it reaches the end of its lifetime.
        /// </summary>
        public float EndScale;

        /// <summary>
        /// A particle used by a particle emitter.
        /// A particle is a GenSprite object, but has additional features useful to particles.
        /// </summary>
        /// <param name="x">The x position of the top-left corner of the particle.</param>
        /// <param name="y">The y position of the top-left corner of the particle.</param>
        /// <param name="texture">The texture to use as the particle's texture.</param>
        /// <param name="width">The width of the particle.</param>
        /// <param name="height">The height of the particle.</param>
        /// <param name="lifetime">The amount of time, in seconds, that the particle will last after being emitted.</param>
        public GenParticle(float x = 0, float y = 0, Texture2D texture = null, int width = 1, int height = 1, float lifetime = 3f)
            : base(x, y, null, width, height)
        {
            if (texture != null)
                LoadTexture(texture);

            Lifetime = lifetime;
            _lifeTimer = 0f;
            StartColor = Color.White;
            EndColor = Color.White;
            StartAlpha = 1f;
            EndAlpha = 1f;
            StartScale = 1f;
            EndScale = 1f;
        }

        /// <summary>
        /// Updates the particle.
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (_lifeTimer < Lifetime)
            {
                float lerp = _lifeTimer / Lifetime;

                // Interpolate the color and alpha.
                _color = Color.Lerp(StartColor, EndColor, lerp) * MathHelper.Lerp(StartAlpha, EndAlpha, lerp);

                // Interpolate the sprite scale.
                Scale.X = MathHelper.Lerp(StartScale, EndScale, lerp);
                Scale.Y = Scale.X;

                _lifeTimer += GenG.TimeStep;
            }
            else
                Kill();
        }

        /// <summary>
        /// Resets the particle to be used by an emitter.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _lifeTimer = 0f;
        }
    }
}
