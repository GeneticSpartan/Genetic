using System;

using Microsoft.Xna.Framework;

namespace Genetic
{
    /// <summary>
    /// Manages a single frame-by-frame tilesheet animation used by a <c>GenSprite</c> object.
    /// The frames of a single animation must be aligned horizontally.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenAnimation
    {
        /// <summary>
        /// A reference to the sprite that uses this animation.
        /// </summary>
        protected GenSprite _sprite;

        /// <summary>
        /// The sequence of frame numbers of the animation.
        /// A value of null will play all frames of the animation texture.
        /// </summary>
        protected int[] _frames;

        /// <summary>
        /// The amount of space, in pixels, surrounding each animation frame.
        /// </summary>
        protected int _frameSpacing;

        /// <summary>
        /// The currently displayed animation frame.
        /// </summary>
        protected int _currentFrame;

        /// <summary>
        /// The position of the top-left corner of the animation on a tilesheet texture, considering frame spacing.
        /// Useful for textures that may contain other animations/assets.
        /// </summary>
        protected Vector2 _position;

        /// <summary>
        /// The source rectangle used to draw the current animation frame.
        /// </summary>
        protected Rectangle _frameRect;

        /// <summary>
        /// The total number of frames of the animation.
        /// </summary>
        protected int _frameCount;

        /// <summary>
        /// A timer used to manage animation frame updates.
        /// </summary>
        protected GenTimer _frameTimer;

        /// <summary>
        /// The animation frames that will invoke the callback method when displayed.
        /// </summary>
        protected bool[] _callbackFrames;

        /// <summary>
        /// The function that will be invoked when the animation callback frames are displayed.
        /// </summary>
        public Action FrameCallback;

        /// <summary>
        /// Gets if the animation is currently playing.
        /// </summary>
        public bool IsPlaying
        {
            get { return _frameTimer.IsRunning; }
        }

        /// <summary>
        /// Gets or sets the speed, in frames per second, of the animation.
        /// </summary>
        public float Fps
        {
            get { return (int)(1 / _frameTimer.Duration); }

            set { _frameTimer.Duration = 1 / value; }
        }

        /// <summary>
        /// Gets or sets the sequence of frame numbers of the animation.
        /// A value of null will play all frames of the animation texture.
        /// </summary>
        public int[] Frames
        {
            get { return _frames; }

            set
            {
                _frames = value;
                _callbackFrames = new bool[_frames.Length];

                RefreshFrameCount();
            }
        }

        /// <summary>
        /// Gets or sets the amount of space, in pixels, surrounding each animation frame.
        /// </summary>
        public int FrameSpacing
        {
            get { return _frameSpacing; }

            set
            {
                _frameSpacing = value;
                _frameRect.Y = _frameSpacing;
            }
        }

        /// <summary>
        /// Gets the source rectangle used to draw the current animation frame.
        /// </summary>
        public Rectangle FrameRect
        {
            get { return _frameRect; }
        }

        public bool IsLooping
        {
            get { return _frameTimer.IsLooping; }

            set { _frameTimer.IsLooping = value; }
        }

        /// <summary>
        /// Creates a playable animation for a sprite.
        /// </summary>
        /// <param name="sprite">A reference to the sprite that will use this animation.</param>
        /// <param name="frameWidth">The width of each animation frame.</param>
        /// <param name="frameHeight">The height of each animation frame.</param>
        /// <param name="frames">The sequence of frame numbers of the animation. A value of null will play all frames of the animation texture.</param>
        /// <param name="fps">The speed, in frames per second, of the animation.</param>
        /// <param name="isLooping">Determines whether the animation is looping or not.</param>
        /// <param name="frameSpacing">The amount of space, in pixels, between each of the animation frames.</param>
        public GenAnimation(GenSprite sprite, int frameWidth, int frameHeight, int[] frames = null, int fps = 12, bool isLooping = true, int frameSpacing = 0)
        {
            _sprite = sprite;
            Frames = (frames == null) ? new int[0] : frames;
            _frameSpacing = frameSpacing;
            _currentFrame = 0;
            _position = Vector2.Zero;
            _frameRect = new Rectangle(0, 0, frameWidth, frameHeight);
            _frameTimer = new GenTimer(0f, UpdateFrame);
            Fps = fps;
            IsLooping = isLooping;
            FrameCallback = null;
        }

        /// <summary>
        /// Updates the current animation frame.
        /// </summary>
        public void Update()
        {
            _frameTimer.Update();
        }

        /// <summary>
        /// Updates the current animation with the appropriate frame.
        /// </summary>
        public void UpdateFrame()
        {
            _currentFrame += (int)(_frameTimer.Elapsed / _frameTimer.Duration);

            if (_currentFrame >= _frameCount)
            {
                if (IsLooping)
                    _currentFrame %= _frameCount;
                else
                {
                    // Stop the animation on the last frame.
                    _currentFrame = _frameCount - 1;
                    _frameTimer.Stop();
                }
            }

            if (_frames.Length != 0)
                _frameRect.X = (_frameRect.Width + _frameSpacing) * _frames[_currentFrame] + _frameSpacing;
            else
                _frameRect.X = (_frameRect.Width + _frameSpacing) * _currentFrame + _frameSpacing;

            _frameRect.Y = _frameSpacing;

            // If the current animation frame is set as a callback frame, invoke the callback method.
            if (FrameCallback != null)
            {
                if ((_currentFrame < _callbackFrames.Length) && _callbackFrames[_currentFrame])
                    FrameCallback.Invoke();
            }
        }

        /// <summary>
        /// Adjusts the frame count using the sprite texture or frames array.
        /// </summary>
        protected void RefreshFrameCount()
        {
            if (_frames.Length == 0)
                _frameCount = _sprite.Texture.Width / (_frameRect.Width + (_frameSpacing * 3));
            else
                _frameCount = _frames.Length;
        }

        /// <summary>
        /// Sets the position of the top-left corner of the animation on a tilesheet texture, considering frame spacing.
        /// Useful for textures that may contain other animations/assets.
        /// </summary>
        /// <param name="x">The x position of the top-left corner of the animation.</param>
        /// <param name="y">The y position of the top-left corner of the animation.</param>
        public void SetPosition(int x, int y)
        {
            _position.X = x;
            _position.Y = y;
        }

        /// <summary>
        /// Sets a callback method that will invoke when the specified animation frames are displayed.
        /// An example would be to play sounds at specific frames in an animation, like foot steps.
        /// </summary>
        /// <param name="callbackFrames">An array of animation frame numbers used to invoke the callback method. All animations start from frame 0.</param>
        /// <param name="callback">The method to invoke at each animation frame.</param>
        public void SetCallback(int[] callbackFrames, Action callback)
        {
            _callbackFrames = new bool[_frames.Length];

            for (int i = 0; i < callbackFrames.Length; i++)
            {
                if (i < _callbackFrames.Length)
                    _callbackFrames[callbackFrames[i]] = true;
            }

            FrameCallback = callback;
        }

        /// <summary>
        /// Starts playing the animation.
        /// </summary>
        /// <param name="forceReset">A flag used to determine if the animation should play from the starting frame.</param>
        public void Play(bool forceReset = true)
        {
            if (forceReset)
                Reset();

            _frameTimer.Start(forceReset);
            UpdateFrame();
        }

        /// <summary>
        /// Resets the animation to start playing from the fist frame.
        /// </summary>
        public void Reset()
        {
            _currentFrame = 0;
            _frameTimer.Stop();
        }
    }
}