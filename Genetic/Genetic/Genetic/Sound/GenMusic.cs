using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Genetic.Sound
{
    /// <summary>
    /// A manager for playback of background music.
    /// Extends features such as volume fading.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    class GenMusic
    {
        /// <summary>
        /// A <c>Song</c> loaded from a song file.
        /// </summary>
        protected Song _song;

        /// <summary>
        /// The volume of the <c>MediaPlayer</c>, a value from 0.0 to 1.0.
        /// </summary>
        protected static float _volume;

        protected static GenTimer _fadeTimer;

        /// <summary>
        /// Visualization (frequency and sample) data for the currently playing song.
        /// </summary>
        public static VisualizationData VisualData;

        /// <summary>
        /// Gets the duration of the song.
        /// </summary>
        public TimeSpan Duration
        {
            get { return _song.Duration; }
        }

        /// <summary>
        /// Gets the number of times the song has been played.
        /// </summary>
        public int PlayCount
        {
            get { return _song.PlayCount; }
        }

        /// <summary>
        /// Gets or sets the volume of the <c>MediaPlayer</c>, a value from 0.0 to 1.0.
        /// </summary>
        public static float Volume
        {
            get { return _volume; }

            set { _volume = MathHelper.Clamp(value, 0f, 1f); }
        }

        /// <summary>
        /// Gets if a song is currently playing.
        /// </summary>
        public static bool IsPlaying
        {
            get { return MediaPlayer.State == MediaState.Playing; }
        }

        /// <summary>
        /// Gets if the current song is paused.
        /// </summary>
        public static bool IsPaused
        {
            get { return MediaPlayer.State == MediaState.Paused; }
        }

        /// <summary>
        /// Gets if song playback is currently stopped.
        /// </summary>
        public static bool IsStopped
        {
            get { return MediaPlayer.State == MediaState.Stopped; }
        }

        /// <summary>
        /// Gets or sets if the <c>MediaPlayer</c> is set to muted.
        /// </summary>
        public static bool IsMuted
        {
            get { return MediaPlayer.IsMuted; }

            set { MediaPlayer.IsMuted = value; }
        }

        /// <summary>
        /// Gets or sets if the <c>MediaPlayer</c> is set to repeat.
        /// </summary>
        public static bool IsLooping
        {
            get { return MediaPlayer.IsRepeating; }

            set { MediaPlayer.IsRepeating = value; }
        }

        /// <summary>
        /// Gets the play position within the currently playing song.
        /// </summary>
        public static TimeSpan PlayPosition
        {
            get { return MediaPlayer.PlayPosition; }
        }

        /// <summary>
        /// Gets if the game currently has control of song playback.
        /// Song playback is disabled if the gamer is currently playing custom background music.
        /// </summary>
        public static bool GameHasControl
        {
            get { return MediaPlayer.GameHasControl; }
        }

        /// <summary>
        /// Creates a playable song.
        /// </summary>
        /// <param name="song">The loaded <c>Song</c> to use.</param>
        public GenMusic(Song song)
        {
            _song = song;
        }

        /// <summary>
        /// Initializes any necessary resources used by <c>GenMusic</c>.
        /// </summary>
        public static void Initialize()
        {
            _volume = 1f;
            _fadeTimer = new GenTimer(0f, null, true);
            VisualData = new VisualizationData();
            MediaPlayer.IsVisualizationEnabled = true;
        }

        /// <summary>
        /// Updates the volume of <c>MediaPlayer</c>, handles song fading, and gets visualization data for the currently playing song.
        /// </summary>
        public static void Update()
        {
            if (_fadeTimer.IsRunning)
            {
                _fadeTimer.Update();

                // Stop the current song if the fade timer has finished during its last update.
                if (!_fadeTimer.IsRunning)
                    Stop();

                MediaPlayer.Volume = _volume * GenG.Volume * (_fadeTimer.Remaining / _fadeTimer.Duration);
            }
            else if (IsPlaying)
                MediaPlayer.Volume = _volume * GenG.Volume;

            if (IsPlaying)
                MediaPlayer.GetVisualizationData(GenMusic.VisualData);
        }

        /// <summary>
        /// Starts playing the song using <c>MediaPlayer</c>.
        /// </summary>
        public void Play()
        {
            MediaPlayer.Play(_song);
        }

        /// <summary>
        /// Pauses the currently playing song using <c>MediaPlayer</c>.
        /// </summary>
        public static void Pause()
        {
            MediaPlayer.Pause();
        }

        /// <summary>
        /// Resumes the currently paused or stopped song using <c>MediaPlayer</c>.
        /// </summary>
        public static void Resume()
        {
            MediaPlayer.Resume();
        }

        /// <summary>
        /// Stops the currently playing song using <c>MediaPlayer</c>.
        /// </summary>
        public static void Stop()
        {
            MediaPlayer.Stop();
        }

        /// <summary>
        /// Fades the currently playing song out to a volume of 0 within the given duration.
        /// </summary>
        /// <param name="duration">The duration of the song fade, in seconds.</param>
        /// <param name="callback">The callback method that will invoke after the song fade has completed.</param>
        public static void FadeOut(float duration = 1f, Action callback = null)
        {
            if (!_fadeTimer.IsRunning)
            {
                _fadeTimer.Duration = duration;
                _fadeTimer.Callback = callback;
                _fadeTimer.Start(true);
            }
        }
    }
}
