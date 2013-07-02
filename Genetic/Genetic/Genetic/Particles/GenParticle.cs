using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genetic.Particles
{
    /// <summary>
    /// A game object that makes up a single particle used by a particle emitter.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenParticle : GenSprite
    {
        /// <summary>
        /// A timer used to manage the lifetime of the particle.
        /// </summary>
        public GenTimer LifeTimer;

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
            : base(x, y, texture, width, height)
        {
            // Set the life timer to kill the particle at the end of its life.
            LifeTimer = new GenTimer(lifetime, Kill);
        }

        /// <summary>
        /// Updates the particle.
        /// </summary>
        public override void Update()
        {
            base.Update();

            LifeTimer.Update();
        }

        /// <summary>
        /// Resets the particle to be used by an emitter.
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            LifeTimer.Start(true);
        }
    }
}
