using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Genetic.Sound
{
    public class GenSound : GenBasic
    {
        /// <summary>
        /// The sound effect that is loaded from a sound file.
        /// </summary>
        protected SoundEffect _sound;

        /// <summary>
        /// Creates a playable sound.
        /// </summary>
        /// <param name="soundFile">The sound file to load.</param>
        public GenSound(string soundFile)
        {
            LoadSound(soundFile);
        }

        /// <summary>
        /// Loads a sound file.
        /// </summary>
        /// <param name="soundFile">The sound file to load.</param>
        public void LoadSound(string soundFile)
        {
            _sound = GenG.Content.Load<SoundEffect>(soundFile);
        }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        public void Play()
        {
            _sound.Play();
        }
    }
}