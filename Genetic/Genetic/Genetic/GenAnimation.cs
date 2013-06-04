using System;

using Microsoft.Xna.Framework;

namespace Genetic
{
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
        /// The amount of seconds that each animation frame will last for.
        /// </summary>
        protected float _frameTime;

        /// <summary>
        /// The currently displayed animation frame.
        /// </summary>
        protected int _currentFrame = 0;

        /// <summary>
        /// The source rectangle used to draw the current animation frame.
        /// </summary>
        protected Rectangle _frameRect;

        /// <summary>
        /// The total number of frames of the animation.
        /// </summary>
        protected int _frameCount;

        /// <summary>
        /// A counter used to keep track of how much time has passed since the last frame update.
        /// </summary>
        protected float _timer = 0;

        /// <summary>
        /// Determines whether the animation is currently playing or not.
        /// </summary>
        public bool IsPlaying = true;

        /// <summary>
        /// Determines whether the animation is looping or not.
        /// </summary>
        public bool IsLooped;

        /// <summary>
        /// The animation frames that will invoke the callback function when displayed.
        /// </summary>
        protected bool[] _callbackFrames;

        /// <summary>
        /// The function that will be invoked when the animation callback frames are displayed.
        /// </summary>
        public Action Callback;

        /// <summary>
        /// Gets or sets the speed, in frames per second, of the animation.
        /// </summary>
        public float Fps
        {
            get { return (int)(1 / _frameTime); }

            set { _frameTime = 1 / value; }
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

        /// <summary>
        /// Creates a playable animation for a sprite.
        /// </summary>
        /// <param name="sprite">A reference to the sprite that will use this animation.</param>
        /// <param name="frameWidth">The width of each animation frame.</param>
        /// <param name="frameHeight">The height of each animation frame.</param>
        /// <param name="frames">The sequence of frame numbers of the animation. A value of null will play all frames of the animation texture.</param>
        /// <param name="fps">The speed, in frames per second, of the animation.</param>
        /// <param name="isLooped">Determines whether the animation is looping or not.</param>
        /// <param name="frameBuffer">The amount of space, in pixels, between each of the animation frames.</param>
        public GenAnimation(GenSprite sprite, int frameWidth, int frameHeight, int[] frames = null, int fps = 12, bool isLooped = true, int frameBuffer = 0)
        {
            _sprite = sprite;
            _frameRect = new Rectangle(0, 0, frameWidth, frameHeight);
            FrameBuffer = frameBuffer;
            Frames = (frames == null) ? new int[0] : frames;
            Fps = fps;
            IsLooped = isLooped;
            Callback = null;
        }

        /// <summary>
        /// Updates the current animation frame.
        /// </summary>
        public void Update()
        {
            if (IsPlaying)
            {
                if (_currentFrame >= _frameCount)
                {
                    if (IsLooped)
                        _currentFrame %= _frameCount;
                    else
                    {
                        _currentFrame = _frameCount - 1;
                        IsPlaying = false;
                    }
                }

                if (_frames.Length != 0)
                    _frameRect.X = (_frameRect.Width + FrameBuffer) * _frames[_currentFrame] + FrameBuffer;
                else
                    _frameRect.X = (_frameRect.Width + FrameBuffer) * _currentFrame + FrameBuffer;

                _timer += GenG.TimeStep;

                // Adjust the current frame after each frame time has passed.
                if (_timer > _frameTime)
                {
                    _currentFrame += (int)(_timer / _frameTime);
                    _timer %= _frameTime;

                    if (Callback != null)
                    {
                        if ((_currentFrame < _callbackFrames.Length) && _callbackFrames[_currentFrame])
                            Callback.Invoke();
                    }
                }
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
        /// Sets a callback function that will invoke when the given animation frames are displayed.
        /// Useful for playing sounds at specific frames in an animation, like foot steps.
        /// </summary>
        /// <param name="frames">An array of animation frame numbers used to invoke the callback function. All animations start from frame 0.</param>
        /// <param name="callback">The function to invoke at each animation frame.</param>
        public void SetCallback(int[] callbackFrames, Action callback)
        {
            _callbackFrames = new bool[_frames.Length];

            for (int i = 0; i < callbackFrames.Length; i++)
            {
                if (i < _callbackFrames.Length)
                    _callbackFrames[callbackFrames[i]] = true;
            }

            Callback = callback;
        }

        /// <summary>
        /// Resets the animation to start playing from the fist frame.
        /// </summary>
        public void Reset()
        {
            _currentFrame = 0;
            _timer = 0;
        }
    }
}