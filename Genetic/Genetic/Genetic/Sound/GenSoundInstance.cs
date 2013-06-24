using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Genetic.Sound
{
    /// <summary>
    /// A playable sound that represents one sound instance.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenSoundInstance : GenBasic
    {
        /// <summary>
        /// A reference to the <c>GenSound</c> object that this sound instance may be handled by.
        /// </summary>
        protected GenSound _soundParent;

        /// <summary>
        /// A sound effect loaded from a sound file.
        /// </summary>
        protected SoundEffect _sound;

        /// <summary>
        /// A playable sound effect instance that is created from a loaded sound effect.
        /// </summary>
        public SoundEffectInstance SoundInstance;

        /// <summary>
        /// The volume of the sound instance, a value from 0.0 to 1.0.
        /// </summary>
        protected float _volume;

        /// <summary>
        /// A value used to adjust the initial sound volume during distance fades.
        /// </summary>
        protected float _volumeDistance;

        /// <summary>
        /// The game object the sound will follow.
        /// </summary>
        public GenObject Follow;

        /// <summary>
        /// The position of the sound relative to the camera.
        /// </summary>
        protected Vector2 _position;

        /// <summary>
        /// Determines if the sound should fade as it moves out of the camera view.
        /// </summary>
        public bool DistanceFading;

        /// <summary>
        /// The total distance, in pixels, that the sound must be from an edge of the camera to fade completely.
        /// </summary>
        public float DistanceFadingLength;

        /// <summary>
        /// The x and y distances of the sound from the edges of the camera.
        /// This is used to calculate the x-axis and y-axis fade values to get an accurate sound fade.
        /// </summary>
        protected Vector2 _cameraDistance;

        protected GenTimer _fadeTimer;

        /// <summary>
        /// Gets if the sound effect instance is currently playing.
        /// </summary>
        public bool IsPlaying
        {
            get { return (SoundInstance.State == SoundState.Playing); }
        }

        /// <summary>
        /// Gets if the sound effect instance is currently paused.
        /// </summary>
        public bool IsPaused
        {
            get { return (SoundInstance.State == SoundState.Paused); }
        }

        /// <summary>
        /// Gets if the sound effect instance is currently stopped.
        /// </summary>
        public bool IsStopped
        {
            get { return (SoundInstance.State == SoundState.Stopped); }
        }

        /// <summary>
        /// Gets or sets if the sound effect instance is looping.
        /// </summary>
        public bool IsLooped
        {
            get { return SoundInstance.IsLooped; }

            set { SoundInstance.IsLooped = value; }
        }

        /// <summary>
        /// Gets or sets the volume of the sound effect instance, a value from 0.0 to 1.0.
        /// </summary>
        public float Volume
        {
            get { return _volume; }

            set { _volume = MathHelper.Clamp(value, 0f, 1f); }
        }

        /// <summary>
        /// Gets or sets the pitch of the sound effect instance, a value from -1.0 to 1.0.
        /// </summary>
        public float Pitch
        {
            get { return SoundInstance.Pitch; }

            set { SoundInstance.Pitch = MathHelper.Clamp(value, -1f, 1f); }
        }

        /// <summary>
        /// Gets or sets the pan of the sound effect instance, a value from -1.0 (full left) to 1.0 (full right).
        /// A value of 0 is centered.
        /// </summary>
        public float Pan
        {
            get { return SoundInstance.Pan; }

            set { SoundInstance.Pan = MathHelper.Clamp(value, -1f, 1f); }
        }

        /// <summary>
        /// Gets the duration of the sound effect.
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                if (_sound != null)
                    return _sound.Duration;
                else
                    return _soundParent.Duration;
            }
        }

        /// <summary>
        /// Creates a playable sound instance.
        /// </summary>
        /// <param name="sound">The loaded <c>SoundEffect</c> to use.</param>
        /// <param name="volume">The volume of the sound, a value from 0.0 to 1.0.</param>
        /// <param name="looping">Determines if the sound is played on a loop.</param>
        public GenSoundInstance(SoundEffect sound, float volume = 1, bool looping = false)
        {
            LoadSound(sound, volume, looping);

            Follow = null;
            DistanceFading = true;
            DistanceFadingLength = 400f;
            _fadeTimer = new GenTimer(0f, null, true);
        }

        /// <summary>
        /// Creates a playable sound instance handled by a <c>GenSound</c> object.
        /// </summary>
        /// <param name="sound">The <c>GenSound</c> object that this sound instance is handled by.</param>
        public GenSoundInstance(GenSound sound)
        {
            _soundParent = sound;
            _sound = _soundParent.Sound;

            SoundInstance = _soundParent.Sound.CreateInstance();
            SoundInstance.IsLooped = _soundParent.IsLooped;
            SoundInstance.Volume = _soundParent.Volume;
            SoundInstance.Pitch = _soundParent.Pitch;
            SoundInstance.Pan = _soundParent.Pan;
            SetFollow(_soundParent.Follow, _soundParent.DistanceFading);
            DistanceFadingLength = _soundParent.DistanceFadingLength;
            _fadeTimer = new GenTimer(0f, null, true);

            // Call UpdateSound initially to correct any volume and pan values.
            UpdateSound();
        }

        /// <summary>
        /// Updates the sound instance and handles sound fading.
        /// </summary>
        public override void Update()
        {
            if (IsPlaying)
                UpdateSound();

            if (_fadeTimer.IsRunning)
            {
                _fadeTimer.Update();

                // Stop the sound if the fade timer has finished during its last update.
                if (!_fadeTimer.IsRunning)
                    Stop();

                SoundInstance.Volume *= _fadeTimer.Remaining / _fadeTimer.Duration;
            }
        }

        /// <summary>
        /// Updates the sound instance volume and pan values.
        /// </summary>
        public void UpdateSound()
        {
            // Adjust the sound pan value according to the game object's position relative to the camera.
            if (Follow != null)
            {
                _position.X = Follow.X + (Follow.Width * 0.5f) - GenG.State.Camera.CameraView.X;
                _position.Y = Follow.Y + (Follow.Height * 0.5f) - GenG.State.Camera.CameraView.Y;

                SoundInstance.Pan = MathHelper.Clamp((_position.X / GenG.State.Camera.CameraView.Width) * 2 - 1, -1, 1);

                // Calculate the distance fade values on both the x-axis and y-axis.
                if (_position.X < 0)
                    _cameraDistance.X = _position.X * -1;
                else if (_position.X > GenG.State.Camera.CameraView.Width)
                    _cameraDistance.X = _position.X - GenG.State.Camera.CameraView.Width;
                else
                    _cameraDistance.X = 0;

                if (_position.Y < 0)
                    _cameraDistance.Y = _position.Y * -1;
                else if (_position.Y > GenG.State.Camera.CameraView.Height)
                    _cameraDistance.Y = _position.Y - GenG.State.Camera.CameraView.Height;
                else
                    _cameraDistance.Y = 0;

                // Adjust the volume based on the distance of the sound from the camera edges.
                if ((_cameraDistance.X == 0) && (_cameraDistance.Y == 0))
                    _volumeDistance = 1;
                else
                    _volumeDistance = MathHelper.Clamp((DistanceFadingLength - _cameraDistance.Length()) / DistanceFadingLength, 0f, 1f);

                // Set the volume of the sound effect instance.
                if (_volumeDistance < 1)
                {
                    if (_soundParent == null)
                        SoundInstance.Volume = _volume * _volumeDistance * GenG.Volume;
                    else
                        SoundInstance.Volume = _soundParent.Volume * _volumeDistance * GenG.Volume;
                }
                else
                {
                    if (_soundParent == null)
                        SoundInstance.Volume = _volume * GenG.Volume;
                    else
                        SoundInstance.Volume = _soundParent.Volume * GenG.Volume;
                }
            }
            else
            {
                if (_soundParent == null)
                    SoundInstance.Volume = _volume * GenG.Volume;
                else
                    SoundInstance.Volume = _soundParent.Volume * GenG.Volume;
            }
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
        /// Plays the sound instance.
        /// </summary>
        /// <param name="forceReset">A flag used to determine if the sound instance should be reset before playing.</param>
        public void Play(bool forceReset = true)
        {
            if (forceReset)
                Stop();

            // Call UpdateSound on the sound instance once before playing it to refresh its volume and pan values.
            UpdateSound();
            SoundInstance.Play();
        }

        /// <summary>
        /// Stops the currently playing sound instance.
        /// </summary>
        /// <param name="immediate">Determines if the sound instance should stop immediately, or break out of the loop and play the rest of the sound instance.</param>
        public void Stop(bool immediate = true)
        {
            SoundInstance.Stop(immediate);
        }

        /// <summary>
        /// Pauses the currently playing sound instance.
        /// </summary>
        public void Pause()
        {
            SoundInstance.Pause();
        }

        /// <summary>
        /// Resumes playback of the sound instance from the paused location.
        /// </summary>
        public void Resume()
        {
            SoundInstance.Resume();
        }

        /// <summary>
        /// Fades the sound instance out to a volume of 0 within the given duration.
        /// </summary>
        /// <param name="duration">The duration of the sound fade, in seconds.</param>
        /// <param name="callback">The method that will invoke after the sound fade has completed.</param>
        public void FadeOut(float duration = 1f, Action callback = null)
        {
            if (!_fadeTimer.IsRunning)
            {
                _fadeTimer.Duration = duration;
                _fadeTimer.Callback = callback;
                _fadeTimer.Start(true);
            }
        }

        /// <summary>
        /// Sets the sound instance to follow a game object.
        /// The sound instance pan value will change according to the game object's position relative to the camera.
        /// </summary>
        /// <param name="follow">The game object the sound instance will follow.</param>
        /// <param name="distanceFading">Determines if the sound instance should fade as the object moves out of the camera view.</param>
        public void SetFollow(GenObject follow, bool distanceFading = false)
        {
                Follow = follow;
                DistanceFading = distanceFading;
        }

        /// <summary>
        /// Reset the sound instance.
        /// </summary>
        public override void Reset()
        {
            Stop();
        }
    }
}
