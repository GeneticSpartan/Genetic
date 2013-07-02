using System;
using System.Collections.Generic;

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
        /// The base volume of the sound instance, a value from 0.0 to 1.0.
        /// </summary>
        protected float _volume;

        /// <summary>
        /// A timer used to manage sound fades.
        /// </summary>
        protected GenTimer _fadeTimer;

        /// <summary>
        /// The game object the sound will follow.
        /// The sound instance will inherit the position and velocity of the follow object.
        /// </summary>
        public GenObject Follow;

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
            get { return (_sound != null) ? _sound.Duration : _soundParent.Duration; }
        }

        /// <summary>
        /// Creates a playable sound instance.
        /// </summary>
        /// <param name="sound">The loaded <c>SoundEffect</c> to use.</param>
        /// <param name="volume">The volume of the sound, a value from 0.0 to 1.0.</param>
        /// <param name="isLooping">A flag used to determine if the sound is played on a loop.</param>
        public GenSoundInstance(SoundEffect sound, float volume = 1f, bool isLooping = false)
        {
            LoadSound(sound, volume, isLooping);

            _fadeTimer = new GenTimer(0f, null);
        }

        /// <summary>
        /// Creates a playable sound instance managed by a <c>GenSound</c> object.
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
            
            
            _fadeTimer = new GenTimer(0f, null);
            Follow = _soundParent.Follow;

            // Call UpdateSound initially to correct the volume value.
            UpdateSound();
        }

        /// <summary>
        /// Updates the sound instance and handles sound fading.
        /// </summary>
        public override void Update()
        {
            if (IsPlaying)
            {
                UpdateSound();
            }

            if (_fadeTimer.IsRunning)
            {
                _fadeTimer.Update();

                // Stop the sound if the fade timer has finished during its last update.
                if (!_fadeTimer.IsRunning)
                {
                    Stop();
                }

                SoundInstance.Volume *= _fadeTimer.Remaining / _fadeTimer.Duration;
            }
        }

        /// <summary>
        /// Updates the sound instance volume, pan, and pitch values.
        /// Applies 2D positional adjustments to the sound instance if attached to a follow object.
        /// </summary>
        public virtual void UpdateSound()
        {
            if (Follow == null)
            {
                SoundInstance.Volume = (_soundParent == null) ? _volume : _soundParent.Volume;
            }
            else
            {
                Apply2D(Follow, Follow.Cameras);
            }
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
            SoundInstance = _sound.CreateInstance();
            Volume = volume;
            IsLooped = isLooping;
        }

        /// <summary>
        /// Plays the sound instance.
        /// </summary>
        /// <param name="forceReset">A flag used to determine if the sound instance should be reset before playing.</param>
        public void Play(bool forceReset = true)
        {
            if (forceReset)
            {
                Stop();
            }

            // Call UpdateSound on the sound instance once before playing it to refresh its volume, pan, and pitch values.
            UpdateSound();
            SoundInstance.Play();
        }

        /// <summary>
        /// Stops the currently playing sound instance.
        /// </summary>
        /// <param name="immediate">A flag used to determine if the sound instance should stop immediately, or play through the remaining sound.</param>
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
        /// Applies 2D positional adjustments to the sound instance using a source and receivers (cameras) of the sound.
        /// The volume, pan, and pitch values are averaged when using multiple cameras depending on their distances from the source.
        /// </summary>
        /// <param name="source">The source of the sound.</param>
        /// <param name="cameras">A list of the receivers (cameras) of the sound.</param>
        public void Apply2D(GenObject source, List<GenCamera> cameras)
        {
            if ((cameras != null) && (cameras.Count > 0))
            {
                Vector2 distance = source.CenterPosition - cameras[0].CenterPosition;

                if (cameras.Count == 1)
                {
                    float distanceFade = MathHelper.Clamp((GenSound.DistanceScale - distance.Length()) / GenSound.DistanceScale, 0f, 1f);

                    SoundInstance.Volume = ((_soundParent == null) ? _volume : _soundParent.Volume) * distanceFade;
                    SoundInstance.Pan = MathHelper.Clamp(distance.X / GenSound.DistanceScale, -1f, 1f) * -cameras[0].Up.Y;
                    SoundInstance.Pitch = GenSound.GetDopplerShift(Follow.Velocity, cameras[0].Velocity, distance, GenSound.DopplerFactor);
                }
                else
                {
                    GenCamera workingCamera = cameras[0];

                    // Get the distance from the sound source to the center of the first camera, and set it as the initial minimum distance.
                    Vector2 distanceMin = distance;
                    float distanceMinLength = distanceMin.Length();

                    // Iterate through each remaining camera, checking for the one closest to the sound source.
                    for (int i = 1; i < cameras.Count; i++)
                    {
                        Vector2 newDistance = source.CenterPosition - cameras[i].CenterPosition;
                        float newDistanceLength = newDistance.Length();

                        // If the current camera is closest to the sound source, set it as the working camera and minimum distance.
                        if (newDistanceLength < distanceMinLength)
                        {
                            workingCamera = cameras[i];
                            distanceMin = newDistance;
                            distanceMinLength = newDistanceLength;
                        }
                    }

                    float cameraFactor = 0f;
                    float cameraFactorSum = 0f;

                    Vector2 distanceSum = Vector2.Zero;
                    Vector2 velocitySum = Vector2.Zero;
                    Vector2 upSum = Vector2.Zero;

                    Vector2 cameraDistance = Vector2.Zero;

                    // Iterate through each camera, calculating their influences on the sound instance depending on their distances from the source.
                    // The closest camera will have the most influence.
                    foreach (GenCamera camera in cameras)
                    {
                        if (camera == workingCamera)
                        {
                            cameraFactorSum += 1f;

                            distanceSum += distanceMin;
                            velocitySum += workingCamera.Velocity;
                            upSum += workingCamera.Up;
                        }
                        else
                        {
                            cameraDistance = source.CenterPosition - camera.CenterPosition;
                            cameraFactor = 1f - (cameraDistance.Length() * GenSound.InverseDistanceScale);

                            if (cameraFactor > 0)
                            {
                                cameraFactorSum += cameraFactor;

                                distanceSum += cameraDistance * cameraFactor;
                                velocitySum += camera.Velocity * cameraFactor;
                                upSum += camera.Up * cameraFactor;
                            }
                        }
                    }

                    // Inverse the camera factor sum to reduce division calculations.
                    float inverseCameraFactorSum = 1f / cameraFactorSum;

                    Vector2 averageDistance = distanceSum * inverseCameraFactorSum;

                    float distanceFade = MathHelper.Clamp((GenSound.DistanceScale - distanceMin.Length()) * GenSound.InverseDistanceScale, 0f, 1f);

                    SoundInstance.Volume = ((_soundParent == null) ? _volume : _soundParent.Volume) * distanceFade;
                    SoundInstance.Pan = MathHelper.Clamp(averageDistance.X * GenSound.InverseDistanceScale, -1f, 1f) * -(upSum * inverseCameraFactorSum).Y;
                    SoundInstance.Pitch = GenSound.GetDopplerShift(Follow.Velocity, (velocitySum * inverseCameraFactorSum), averageDistance, GenSound.DopplerFactor);
                }
            }
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
        /// Resets the sound instance.
        /// </summary>
        public override void Reset()
        {
            Stop();
        }
    }
}
