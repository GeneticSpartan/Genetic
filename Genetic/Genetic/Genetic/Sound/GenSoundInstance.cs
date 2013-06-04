using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Genetic.Sound
{
    public class GenSoundInstance : GenSound
    {
        /// <summary>
        /// A playable sound effect instance that is created from the loaded sound effect.
        /// </summary>
        protected SoundEffectInstance _soundInstance;

        /// <summary>
        /// The volume of the sound effect instance, a value from 0.0 to 1.0.
        /// </summary>
        protected float _volume;

        /// <summary>
        /// The game object the sound will follow.
        /// </summary>
        protected GenObject _follow;

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

        /// <summary>
        /// A value used to adjust the initial sound volume during fades.
        /// </summary>
        protected float _volumeAdjust;

        /// <summary>
        /// Determines if the sound is currently fading.
        /// </summary>
        protected bool _fading;

        /// <summary>
        /// The current duration of the sound fade, in seconds.
        /// </summary>
        protected float _fadeDuration;

        /// <summary>
        /// The amount of time since the sound fade started, in seconds.
        /// </summary>
        protected float _fadeTimer;

        /// <summary>
        /// A flag used to determine if the sound will stop playing after the sound fade has completed.
        /// </summary>
        protected bool _fadeStopSound;

        /// <summary>
        /// The callback function that will invoke after the sound fade has completed.
        /// </summary>
        protected Action _fadeCallback;

        /// <summary>
        /// Gets whether the sound effect instance is currently playing.
        /// </summary>
        public bool IsPlaying
        {
            get { return _soundInstance.State == SoundState.Playing; }
        }

        /// <summary>
        /// Gets whether the sound effect instance is currently paused.
        /// </summary>
        public bool IsPaused
        {
            get { return _soundInstance.State == SoundState.Paused; }
        }

        /// <summary>
        /// Gets whether the sound effect instance is currently stopped.
        /// </summary>
        public bool IsStopped
        {
            get { return _soundInstance.State == SoundState.Stopped; }
        }

        /// <summary>
        /// Gets or sets whether the sound effect instance is looping or not.
        /// </summary>
        public bool IsLooped
        {
            get { return _soundInstance.IsLooped; }

            set { _soundInstance.IsLooped = value; }
        }

        /// <summary>
        /// Gets or sets the volume of the sound effect instance, a value from 0.0 to 1.0.
        /// </summary>
        public float Volume
        {
            get { return _volume; }

            set { _volume = MathHelper.Clamp(value, 0, 1); }
        }

        /// <summary>
        /// Gets or sets the pitch of the sound effect instance, a value from -1.0 to 1.0.
        /// </summary>
        public float Pitch
        {
            get { return _soundInstance.Pitch; }

            set { _soundInstance.Pitch = MathHelper.Clamp(value, -1, 1); }
        }

        /// <summary>
        /// Gets or sets the pan of the sound effect instance, a value from -1.0 (full left) to 1.0 (full right).
        /// A value of 0 is centered.
        /// </summary>
        public float Pan
        {
            get { return _soundInstance.Pan; }

            set { _soundInstance.Pan = MathHelper.Clamp(value, -1, 1); }
        }

        /// <summary>
        /// Gets the duration of the sound effect.
        /// </summary>
        public TimeSpan Duration
        {
            get { return _sound.Duration; }
        }

        /// <summary>
        /// Creates a playable sound instance.
        /// </summary>
        /// <param name="soundFile">The sound file to load.</param>
        /// <param name="volume">The volume of the sound, a value from 0.0 to 1.0.</param>
        /// <param name="looping">Determines if the sound is played on a loop.</param>
        public GenSoundInstance(string soundFile, float volume = 1, bool looping = false)
            : base(soundFile)
        {
            LoadSound(soundFile, volume, looping);

            _follow = null;
            DistanceFading = false;
            DistanceFadingLength = 400f;
            _volumeAdjust = 1f;
            _fading = false;
            _fadeDuration = 0f;
            _fadeTimer = 0f;
            _fadeStopSound = true;
            _fadeCallback = null;
        }

        public override void Update()
        {
            if (IsPlaying == true)
            {
                // Adjust the sound pan value according to the game object's position relative to the camera.
                if (_follow != null)
                {
                    _position.X = _follow.X + (_follow.Width * 0.5f) - GenG.Camera.CameraView.X;
                    _position.Y = _follow.Y + (_follow.Height * 0.5f) - GenG.Camera.CameraView.Y;

                    Pan = (_position.X / GenG.Camera.CameraView.Width) * 2 - 1;

                    // Calculate the distance fade values on both the x-axis and y-axis.
                    if (_position.X < 0)
                        _cameraDistance.X = _position.X * -1;
                    else if (_position.X > GenG.Camera.CameraView.Width)
                        _cameraDistance.X = _position.X - GenG.Camera.CameraView.Width;
                    else
                        _cameraDistance.X = 0;

                    if (_position.Y < 0)
                        _cameraDistance.Y = _position.Y * -1;
                    else if (_position.Y > GenG.Camera.CameraView.Height)
                        _cameraDistance.Y = _position.Y - GenG.Camera.CameraView.Height;
                    else
                        _cameraDistance.Y = 0;

                    // Adjust the volume based on the distance of the sound from the camera edges.
                    if ((_cameraDistance.X == 0) && (_cameraDistance.Y == 0))
                        _volumeAdjust = 1;
                    else
                        _volumeAdjust = MathHelper.Clamp((DistanceFadingLength - _cameraDistance.Length()) / DistanceFadingLength, 0, 1);

                    // Set the volume of the sound effect instance.
                    if (_volumeAdjust < 1)
                        _soundInstance.Volume = _volume * _volumeAdjust * GenG.Volume;
                    else
                        _soundInstance.Volume = _volume * GenG.Volume;
                }
                else
                    _soundInstance.Volume = _volume * GenG.Volume;

                if (_fading)
                {
                    if (_fadeTimer < _fadeDuration)
                    {
                        _soundInstance.Volume *= (_fadeDuration - _fadeTimer) / _fadeDuration;

                        _fadeTimer += GenG.TimeStep;
                    }
                    else
                    {
                        _fading = false;

                        if (_fadeStopSound)
                            Stop(true);

                        if (_fadeCallback != null)
                            _fadeCallback.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// Loads a sound file.
        /// </summary>
        /// <param name="soundFile">The sound file to load.</param>
        /// <param name="volume">The volume of the sound, a value from 0.0 to 1.0.</param>
        /// <param name="looping">Determines if the sound is played on a loop.</param>
        public void LoadSound(string soundFile, float volume = 1, bool looping = false)
        {
            //_sound = GenG.Content.Load<SoundEffect>(soundFile);
            _soundInstance = _sound.CreateInstance();
            Volume = volume;
            IsLooped = looping;
        }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        /// <param name="forceReset">A flag used to determine if the sound should be reset before playing.</param>
        public void Play(bool forceReset = true)
        {
            if (forceReset)
                Stop();

            _soundInstance.Play();
        }

        /// <summary>
        /// Stops the currently playing sound.
        /// </summary>
        /// <param name="immediate">Determines if the sound should stop immediately, or break out of the loop and play the rest of the sound.</param>
        public void Stop(bool immediate = true)
        {
            _soundInstance.Stop(immediate);
        }

        /// <summary>
        /// Pauses the currently playing sound.
        /// </summary>
        public void Pause()
        {
            _soundInstance.Pause();
        }

        /// <summary>
        /// Resumes playback of the sound from its paused location.
        /// </summary>
        public void Resume()
        {
            _soundInstance.Resume();
        }

        /// <summary>
        /// Fades the sound out to a volume of 0 within the given duration.
        /// </summary>
        /// <param name="duration">The duration of the sound fade, in seconds.</param>
        /// <param name="stopSound">A flag used to determine if the sound will stop playing after the sound fade has completed.</param>
        /// <param name="callback">The callback function that will invoke after the sound fade has completed.</param>
        public void FadeOut(float duration = 1f, bool stopSound = true, Action callback = null)
        {
            _fadeDuration = duration;
            _fadeCallback = callback;
            _fadeStopSound = stopSound;
            _fadeTimer = 0f;

            _fading = true;
        }

        /// <summary>
        /// Sets the sound to follow a game object.
        /// The sound pan value will change according to the game object's position relative to the camera.
        /// </summary>
        /// <param name="follow">The game object the sound will follow.</param>
        /// <param name="distanceFading">Determines if the sound should fade as the object moves out of the camera view.</param>
        public void SetFollow(GenObject follow, bool distanceFading = false)
        {
            _follow = follow;
            DistanceFading = distanceFading;
        }

        /// <summary>
        /// Reset the sound.
        /// </summary>
        public override void Reset()
        {
            Stop();
        }
    }
}
