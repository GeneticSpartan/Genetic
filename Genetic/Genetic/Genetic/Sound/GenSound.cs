using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Genetic.Sound
{
    /// <summary>
    /// A playable sound that creates and manages multiple sound instances.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenSound : GenBasic
    {
        /// <summary>
        /// A <c>SoundEffect</c> loaded from a sound file.
        /// </summary>
        protected SoundEffect _sound;

        /// <summary>
        /// A list of playable sound instances created from this sound.
        /// </summary>
        protected List<GenSoundInstance> _soundInstances;

        /// <summary>
        /// A flag used to determine if each sound instance has been set to looping.
        /// </summary>
        protected bool _isLooped;

        /// <summary>
        /// The volume that was set for each sound instance, a value from 0.0 to 1.0.
        /// </summary>
        protected float _volume;

        /// <summary>
        /// The pitch that was set for each sound instance.
        /// </summary>
        protected float _pitch;

        /// <summary>
        /// The pan that was set for each sound instance.
        /// </summary>
        protected float _pan;

        /// <summary>
        /// The game object that each sound instance is following.
        /// Each sound instance will inherit the position and velocity of the follow object.
        /// </summary>
        protected GenObject _follow;

        /// <summary>
        /// The value that adjusts the effect of distance calculations on 2D positional sounds.
        /// </summary>
        protected static float _distanceScale = 2000f;

        /// <summary>
        /// The inverse of the distance scale, used to reduce division calculations.
        /// </summary>
        protected static float _inverseDistanceScale = 1f / _distanceScale;

        /// <summary>
        /// The scaling of the source and receiver velocities used to adjust the influence of a doppler pitch shift.
        /// </summary>
        public static float DopplerFactor = 0.1f;

        /// <summary>
        /// Gets the sound effect loaded from a sound effect file.
        /// </summary>
        public SoundEffect Sound
        {
            get { return _sound; }
        }

        /// <summary>
        /// Gets if any sound instance is currently playing.
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                foreach (GenSoundInstance soundInstance in _soundInstances)
                {
                    if (soundInstance.IsPlaying)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets if all sound instances are currently paused.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                foreach (GenSoundInstance soundInstance in _soundInstances)
                {
                    if (soundInstance.IsPlaying || soundInstance.IsStopped)
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets if all sound instances are currently stopped.
        /// </summary>
        public bool IsStopped
        {
            get
            {
                foreach (GenSoundInstance soundInstance in _soundInstances)
                {
                    if (soundInstance.IsPlaying || soundInstance.IsPaused)
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets if any sound instance is looping, or sets each sound instance to looping or not.
        /// </summary>
        public bool IsLooped
        {
            get { return _isLooped; }

            set
            {
                _isLooped = value;

                foreach (GenSoundInstance soundInstance in _soundInstances)
                    soundInstance.SoundInstance.IsLooped = _isLooped;
            }
        }

        /// <summary>
        /// Gets or sets the volume of each sound instance, a value from 0.0 to 1.0.
        /// </summary>
        public float Volume
        {
            get { return _volume; }

            set { _volume = MathHelper.Clamp(value, 0f, 1f); }
        }

        /// <summary>
        /// Gets or sets the pitch of each sound instance, a value from -1.0 to 1.0.
        /// </summary>
        public float Pitch
        {
            get { return _pitch; }

            set
            {
                _pitch = MathHelper.Clamp(value, -1f, 1f);

                foreach (GenSoundInstance soundInstance in _soundInstances)
                    soundInstance.SoundInstance.Pitch = _pitch;
            }
        }

        /// <summary>
        /// Gets or sets the pan of each sound instance, a value from -1.0 (full left) to 1.0 (full right).
        /// A value of 0 is centered.
        /// </summary>
        public float Pan
        {
            get { return _pan; }

            set
            {
                _pan = MathHelper.Clamp(value, -1f, 1f);

                foreach (GenSoundInstance soundInstance in _soundInstances)
                    soundInstance.SoundInstance.Pan = _pan;
            }
        }

        /// <summary>
        /// Gets the duration of the sound effect.
        /// </summary>
        public TimeSpan Duration
        {
            get { return _sound.Duration; }
        }

        /// <summary>
        /// Gets or sets the game object that each sound instance is following.
        /// Each sound instance will inherit the position and velocity of the follow object.
        /// </summary>
        public GenObject Follow
        {
            get { return _follow; }

            set
            {
                _follow = value;

                foreach (GenSoundInstance soundInstance in _soundInstances)
                    soundInstance.Follow = _follow;
            }
        }

        /// <summary>
        /// Gets or sets the value that adjusts the effect of distance calculations on 2D positional sounds.
        /// </summary>
        public static float DistanceScale
        {
            get { return _distanceScale; }

            set
            {
                _distanceScale = value;
                _inverseDistanceScale = 1f / _distanceScale;
            }
        }

        /// <summary>
        /// Gets the inverse of the distance scale, used to reduce division calculations.
        /// </summary>
        public static float InverseDistanceScale
        {
            get { return _inverseDistanceScale; }
        }

        /// <summary>
        /// Creates a playable sound.
        /// </summary>
        /// <param name="sound">The loaded <c>SoundEffect</c> to use.</param>
        /// <param name="volume">The volume of the sound, a value from 0.0 to 1.0.</param>
        /// <param name="isLooping">A flag used to determine if the sound is played on a loop.</param>
        public GenSound(SoundEffect sound, float volume = 1f, bool isLooping = false)
        {
            _soundInstances = new List<GenSoundInstance>();

            LoadSound(sound, volume, isLooping);
        }

        /// <summary>
        /// Calls <c>Update</c> on each currently playing sound instance in the sound instances list.
        /// </summary>
        public override void Update()
        {
            foreach (GenSoundInstance soundInstance in _soundInstances)
                    soundInstance.Update();
        }

        /// <summary>
        /// Loads a sound effect.
        /// </summary>
        /// <param name="sound">The sound effect to load.</param>
        /// <param name="volume">The volume of the sound, a value from 0.0 to 1.0.</param>
        /// <param name="isLooping">A flag used to determine if the sound is played on a loop.</param>
        public void LoadSound(SoundEffect sound, float volume = 1f, bool isLooping = false)
        {
            _sound = sound;
            Volume = volume;
            IsLooped = isLooping;
        }

        /// <summary>
        /// Plays a sound instance.
        /// </summary>
        public virtual void Play()
        {
            // Attempt to call Play on an existing sound instance if it is stopped.
            // Otherwise, create and add a new sound instance to play.
            foreach (GenSoundInstance soundInstance in _soundInstances)
            {
                if (soundInstance.IsStopped)
                {
                    soundInstance.Play(false);
                    return;
                }
            }

            GenSoundInstance newSoundInstance = new GenSoundInstance(this);
            _soundInstances.Add(newSoundInstance);
            newSoundInstance.Play();
        }

        /// <summary>
        /// Stops each currently playing sound instance.
        /// </summary>
        /// <param name="immediate">A flag used to determine if each sound instance should stop immediately, or play through the remaining sound.</param>
        public void Stop(bool immediate = true)
        {
            foreach (GenSoundInstance soundInstance in _soundInstances)
                soundInstance.SoundInstance.Stop(immediate);
        }

        /// <summary>
        /// Pauses each currently playing sound instance.
        /// </summary>
        public void Pause()
        {
            foreach (GenSoundInstance soundInstance in _soundInstances)
                soundInstance.SoundInstance.Pause();
        }

        /// <summary>
        /// Resumes playback of each sound instance from their paused locations.
        /// </summary>
        public void Resume()
        {
            foreach (GenSoundInstance soundInstance in _soundInstances)
                soundInstance.SoundInstance.Resume();
        }

        /// <summary>
        /// Fades each sound instance out to a volume of 0 within the given duration.
        /// </summary>
        /// <param name="duration">The duration of each sound fade, in seconds.</param>
        /// <param name="stopSound">A flag used to determine if each sound instance will stop playing after each sound fade has completed.</param>
        /// <param name="callback">The method that will invoke after each sound fade has completed.</param>
        public void FadeOut(float duration = 1f, bool stopSound = true, Action callback = null)
        {
            foreach (GenSoundInstance soundInstance in _soundInstances)
                soundInstance.FadeOut(duration, callback);
        }

        /// <summary>
        /// Reset each sound instance.
        /// </summary>
        public override void Reset()
        {
            Stop();
        }

        /// <summary>
        /// Clears the sound instances list off all existing <c>GenSoundInstance</c> objects.
        /// Useful for cleaning up sound instance memory.
        /// </summary>
        public void Clear()
        {
            _soundInstances.Clear();
        }

        /// <summary>
        /// Gets a doppler pitch shift using the relative velocities of a source and receiver of a sound.
        /// The doppler pitch shift is used to adjust the pitch of a sound, a value between -1.0 and 1.0.
        /// A source and receiver moving towards each other results in a higher pitch (positive), or a lower pitch (negative) as they move away.
        /// </summary>
        /// <param name="sourceVelocity">The velocity vector of the source of the sound.</param>
        /// <param name="receiverVelocity">The velocity vector of the receiver of the sound.</param>
        /// <param name="distance">A distance vector from the emitter to the receiver.</param>
        /// <param name="dopplerFactor">The scaling of the source and receiver velocities used to adjust the influence of the doppler pitch shift.</param>
        /// <returns>The sound pitch value resulting from the doppler shift.</returns>
        public static float GetDopplerShift(Vector2 sourceVelocity, Vector2 receiverVelocity, Vector2 distance, float dopplerFactor = 1f)
        {
            float relativeSourceVelocity = Vector2.Dot(distance, sourceVelocity) / distance.Length();
            float relativeReceiverVelocity = Vector2.Dot(distance, receiverVelocity) / distance.Length();

            relativeSourceVelocity = Math.Min(relativeSourceVelocity, SoundEffect.SpeedOfSound / dopplerFactor);
            relativeReceiverVelocity = Math.Min(relativeReceiverVelocity, SoundEffect.SpeedOfSound / dopplerFactor);

            float dopplerShift = (SoundEffect.SpeedOfSound - dopplerFactor * relativeSourceVelocity) / (SoundEffect.SpeedOfSound - dopplerFactor * relativeReceiverVelocity);

            return MathHelper.Clamp(dopplerShift - 1, -1f, 1f);
        }
    }
}
