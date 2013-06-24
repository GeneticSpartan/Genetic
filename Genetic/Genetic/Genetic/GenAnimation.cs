using System;

using Microsoft.Xna.Framework;

namespace Genetic
{
    /// <summary>
    /// Manages a single tilesheet frame-by-frame animation used by a <c>GenSprite</c> object.
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
        /// The amount of space, in pixels, between each of the animation frames.
        /// </summary>
        public int FrameBuffer;

        /// <summary>
        /// The currently displayed animation frame.
        /// </summary>
        protected int _currentFrame;

        /// <summary>
        /// The source rectangle used to draw the current animation frame.
        /// </summary>
        protected Rectangle _frameRect;

        /// <summary>
        /// The total number of frames of the animation.
        /// </summary>
        protected int _frameCount;

        protected GenTimer _frameTimer;

        /// <summary>
        /// The animation frames that will invoke the callback method when displayed.
        /// </summary>
        protected bool[] _callbackFrames;

        /// <summary>
        /// The function that will be invoked when the animation callback frames are displayed.
        /// </summary>
        public Action FrameCallback;

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
        /// <param name="frameBuffer">The amount of space, in pixels, between each of the animation frames.</param>
        public GenAnimation(GenSprite sprite, int frameWidth, int frameHeight, int[] frames = null, int fps = 12, bool isLooping = true, int frameBuffer = 0)
        {
            _sprite = sprite;
            _frameRect = new Rectangle(0, 0, frameWidth, frameHeight);
            FrameBuffer = frameBuffer;
            _currentFrame = 0;
            Frames = (frames == null) ? new int[0] : frames;
            _frameTimer = new GenTimer(0f, UpdateFrame, true);
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

        public void UpdateFrame()
        {
            _currentFrame += (int)(_frameTimer.Elapsed / _frameTimer.Duration);

            if (_currentFrame >= _frameCount)
            {
                if (IsLooping)
                    _currentFrame %= _frameCount;
                else
                {
                    _currentFrame = _frameCount - 1;
                    _frameTimer.Stop();
                }
            }

            if (_frames.Length != 0)
                _frameRect.X = (_frameRect.Width + FrameBuffer) * _frames[_currentFrame] + FrameBuffer;
            else
                _frameRect.X = (_frameRect.Width + FrameBuffer) * _currentFrame + FrameBuffer;

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
                _frameCount = _sprite.Texture.Width / ((_frameRect.Width + FrameBuffer) + FrameBuffer + FrameBuffer);
            else
                _frameCount = _frames.Length;
        }

        /// <summary>
        /// Sets a callback method that will invoke when the given animation frames are displayed.
        /// Useful for playing sounds at specific frames in an animation, like foot steps.
        /// </summary>
        /// <param name="callbackFrames">An array of animation frame numbers used to invoke the callback method. All animations start from frame 0.</param>
        /// <param name="callback">The function to invoke at each animation frame.</param>
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