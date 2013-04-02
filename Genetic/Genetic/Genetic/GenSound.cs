using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Genetic
{
    public class GenSound : GenBasic
    {
        /// <summary>
        /// The sound effect that is loaded from a sound file.
        /// </summary>
        protected SoundEffect _sound;

        /// <summary>
        /// A playable sound effect instance that is created from the loaded sound effect.
        /// </summary>
        protected SoundEffectInstance _soundInstance;

        /// <summary>
        /// The volume of the sound effect instance, a value from 0 to 1.
        /// </summary>
        protected float _volume;

        /// <summary>
        /// The game object the sound will follow.
        /// </summary>
        protected GenObject _follow = null;

        /// <summary>
        /// The position of the sound relative to the camera.
        /// </summary>
        protected Vector2 _position;

        /// <summary>
        /// Determines if the sound should fade as it moves out of the camera view.
        /// </summary>
        public bool DistanceFading = false;

        /// <summary>
        /// The total distance, in pixels, that the sound must be from an edge of the camera to fade completely.
        /// </summary>
        public float DistanceFadingLength = 400f;

        /// <summary>
        /// The x and y distances of the sound from the edges of the camera.
        /// This is used to calculate the x-axis and y-axis fade values to get an accurate sound fade.
        /// </summary>
        protected Vector2 _cameraDistance;

        /// <summary>
        /// A value used to adjust the initial sound volume during fades.
        /// </summary>
        protected float _volumeAdjust = 1;

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
        /// Gets or sets the volume of the sound effect instance, a value from 0 to 1.
        /// </summary>
        public float Volume
        {
            get { return _volume; }

            set
            {
                _volume = MathHelper.Clamp(value, 0, 1);
            }
        }

        /// <summary>
        /// Gets or sets the pitch of the sound effect instance, a value from -1 to 1.
        /// </summary>
        public float Pitch
        {
            get { return _soundInstance.Pitch; }

            set
            {
                _soundInstance.Pitch = MathHelper.Clamp(value, -1, 1);
            }
        }

        /// <summary>
        /// Gets or sets the pan of the sound effect instance, a value from -1 (full left) to 1 (full right).
        /// A value of 0 is centered.
        /// </summary>
        public float Pan
        {
            get { return _soundInstance.Pan; }

            set
            {
                _soundInstance.Pan = MathHelper.Clamp(value, -1, 1);
            }
        }

        /// <summary>
        /// Creates a playable sound.
        /// </summary>
        /// <param name="soundFile">The sound file to load.</param>
        /// <param name="volume">The volume of the sound, a value from 0 to 1.</param>
        /// <param name="looping">Determines if the sound is played on a loop.</param>
        public GenSound(string soundFile = null, float volume = 1, bool looping = false)
        {
            if (soundFile != null)
            {
                LoadSound(soundFile, volume, looping);
            }
            else
            {
                _sound = null;
                _soundInstance = null;
            }
        }

        public override void Update()
        {
            if (IsPlaying == true)
            {
                // Adjust the sound pan value according to the game object's position relative to the camera.
                if (_follow != null && IsPlaying == true)
                {
                    _position.X = _follow.X + (_follow.Width / 2) - GenG.Camera.CameraView.X;
                    _position.Y = _follow.Y + (_follow.Height / 2) - GenG.Camera.CameraView.Y;

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
                    if ((_volumeAdjust < 1) || (_soundInstance.Volume != _volume))
                        _soundInstance.Volume = _volume * _volumeAdjust;
                }
                else if (_soundInstance.Volume != _volume)
                    _soundInstance.Volume = _volume;
            }
        }

        /// <summary>
        /// Loads a sound file.
        /// </summary>
        /// <param name="soundFile">The sound file to load.</param>
        /// <param name="volume">The volume of the sound, a value from 0 to 1.</param>
        /// <param name="looping">Determines if the sound is played on a loop.</param>
        public void LoadSound(string soundFile, float volume = 1, bool looping = false)
        {
            _sound = GenG.Content.Load<SoundEffect>(soundFile);
            _soundInstance = _sound.CreateInstance();
            Volume = volume;
            IsLooped = looping;
        }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        public void Play()
        {
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