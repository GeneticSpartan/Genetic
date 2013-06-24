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
        /// </summary>
        protected GenObject _follow;

        /// <summary>
        /// Determines if each sound instance should fade as they move out of the camera view.
        /// </summary>
        protected bool _distanceFading;

        /// <summary>
        /// The total distance, in pixels, that each sound instance must be from an edge of the camera to fade completely.
        /// </summary>
        protected float _distanceFadingLength;

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
        /// Gets the game object that each sound instance is following.
        /// </summary>
        public GenObject Follow
        {
            get { return _follow; }
        }

        /// <summary>
        /// Gets or sets if each sound instance should fade as they move out of the camera view.
        /// </summary>
        public bool DistanceFading
        {
            get { return _distanceFading; }

            set
            {
                _distanceFading = value;

                foreach (GenSoundInstance soundInstance in _soundInstances)
                    soundInstance.DistanceFading = _distanceFading;
            }
        }

        /// <summary>
        /// The total distance, in pixels, that the sound must be from an edge of the camera to fade completely.
        /// </summary>
        public float DistanceFadingLength
        {
            get { return _distanceFadingLength; }

            set
            {
                _distanceFadingLength = value;

                foreach (GenSoundInstance soundInstance in _soundInstances)
                    soundInstance.DistanceFadingLength = _distanceFadingLength;
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
        /// Creates a playable sound.
        /// </summary>
        /// <param name="sound">The loaded <c>SoundEffect</c> to use.</param>
        /// <param name="volume">The volume of the sound, a value from 0.0 to 1.0.</param>
        /// <param name="looping">Determines if the sound is played on a loop.</param>
        public GenSound(SoundEffect sound, float volume = 1, bool looping = false)
        {
            _soundInstances = new List<GenSoundInstance>();

            LoadSound(sound, volume, looping);

            _follow = null;
            _distanceFading = true;
            _distanceFadingLength = 400f;
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
        /// <param name="looping">Determines if the sound is played on a loop.</param>
        public void LoadSound(SoundEffect sound, float volume = 1, bool looping = false)
        {
            _sound = sound;
            Volume = volume;
            IsLooped = looping;
        }

        /// <summary>
        /// Plays a sound instance.
        /// </summary>
        public void Play()
        {
            // Call Play on an existing sound instance if it is stopped.
            // Otherwise, create a new sound instance.
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
        /// <param name="immediate">Determines if each sound instance should stop immediately, or break out of the loop and play the rest of each sound instance.</param>
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
        /// Sets each sound instance to follow a game object.
        /// Each sound instance pan value will change according to the game object's position relative to the camera.
        /// </summary>
        /// <param name="follow">The game object that each sound instance will follow.</param>
        /// <param name="distanceFading">Determines if each sound instance should fade as the object moves out of the camera view.</param>
        public void SetFollow(GenObject follow, bool distanceFading = true)
        {
            foreach (GenSoundInstance soundInstance in _soundInstances)
            {
                soundInstance.Follow = follow;
                soundInstance.DistanceFading = distanceFading;
            }
        }

        /// <summary>
        /// Reset each sound instance.
        /// </summary>
        public override void Reset()
        {
            Stop();
        }

        /// <summary>
        /// Clears the sound instances list off all existing sound instances.
        /// Useful for cleaning up sound instance memory.
        /// </summary>
        public void Clear()
        {
            _soundInstances.Clear();
        }
    }
}
