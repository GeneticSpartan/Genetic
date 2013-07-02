using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Genetic.Geometry;

namespace Genetic.Particles
{
    /// <summary>
    /// A particle emitter that emits and manages a group of <c>GenParticle</c> objects.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenEmitter : GenGroup
    {
        /// <summary>
        /// The x and y positions of the top-left corner of the emitter.
        /// </summary>
        protected Vector2 _position;

        /// <summary>
        /// The bounding box within which particles will emit.
        /// </summary>
        protected GenAABB _bounds;

        /// <summary>
        /// The bounding rectangle of the object.
        /// Used during draw debug calls.
        /// </summary>
        protected Rectangle _boundingRect;

        /// <summary>
        /// The minimum x speed allowed for particles as they are emitted.
        /// </summary>
        public int MinParticleSpeedX;

        /// <summary>
        /// The maximum x speed allowed for particles as they are emitted.
        /// </summary>
        public int MaxParticleSpeedX;

        /// <summary>
        /// The minimum y speed allowed for particles as they are emitted.
        /// </summary>
        public int MinParticleSpeedY;

        /// <summary>
        /// The maximum y speed allowed for particles as they are emitted.
        /// </summary>
        public int MaxParticleSpeedY;

        /// <summary>
        /// The minimum rotation allowed for particles as they are emitted.
        /// </summary>
        public int MinRotation;

        /// <summary>
        /// The maximum rotation allowed for particles as they are emitted.
        /// </summary>
        public int MaxRotation;

        /// <summary>
        /// The minimum rotation speed allowed for particles as they are emitted.
        /// </summary>
        public int MinRotationSpeed;

        /// <summary>
        /// The maximum rotation speed allowed for particles as they are emitted.
        /// </summary>
        public int MaxRotationSpeed;

        /// <summary>
        /// A list of colors used to tint the particles over the span of their lifetimes.
        /// The order of the colors proceeds from the birth to the death of a particle.
        /// </summary>
        public List<Color> Colors;

        /// <summary>
        /// The starting color alpha of a particle sprite when it is emitted.
        /// </summary>
        public float StartAlpha;

        /// <summary>
        /// The ending color alpha of a particle sprite when it reaches the end of its lifetime.
        /// </summary>
        public float EndAlpha;

        /// <summary>
        /// The starting scale of a particle sprite when it is emitted.
        /// </summary>
        public float StartScale;

        /// <summary>
        /// The ending scale of a particle sprite when it reaches the end of its lifetime.
        /// </summary>
        public float EndScale;

        /// <summary>
        /// A flag used to determine if all of the particles should emit at once.
        /// </summary>
        public bool Explode;

        /// <summary>
        /// The amount of particles to emit at one time.
        /// A value of 0 will emit all of the particles at once.
        /// </summary>
        public int EmitQuantity;

        /// <summary>
        /// A timer used to manage the emission of particles at a set frequency.
        /// </summary>
        public GenTimer _emitTimer;

        /// <summary>
        /// A flag used to determine if the emitted particles should inherit the velocity of the emitter's parent object.
        /// </summary>
        public bool InheritVelocity;

        /// <summary>
        /// The particle that the emitter is currently controlling.
        /// </summary>
        protected GenParticle _currentParticle;

        /// <summary>
        /// The object that the emitter will be parented to.
        /// The position of the emitter will move with the origin position of the parent object.
        /// </summary>
        public GenObject Parent;

        /// <summary>
        /// The x and y position offsets relative to the parent object's origin position.
        /// </summary>
        public Vector2 ParentOffset;

        /// <summary>
        /// Gets the x and y positions of the top-left corner of the emitter.
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Gets or sets the x position of the top-left corner of the emitter.
        /// </summary>
        public float X
        {
            get { return _position.X; }

            set
            {
                _position.X = value;
                _bounds.X = value;
            }
        }

        /// <summary>
        /// Gets or sets the x position of the top-left corner of the emitter.
        /// </summary>
        public float Y
        {
            get { return _position.Y; }

            set
            {
                _position.Y = value;
                _bounds.Y = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the bounding box within which particles will emit.
        /// </summary>
        public float Width
        {
            get { return _bounds.Width; }

            set
            {
                _bounds.Width = value;
                _boundingRect.Width = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the bounding box within which particles will emit.
        /// </summary>
        public float Height
        {
            get { return _bounds.Height; }

            set
            {
                _bounds.Height = value;
                _boundingRect.Height = (int)value;
            }
        }

        public float EmitFrequency
        {
            get { return _emitTimer.Duration; }

            set { _emitTimer.Duration = value; }
        }

        /// <summary>
        /// Gets if the emitter is currently emitting particles.
        /// </summary>
        public bool IsRunning
        {
            get { return _emitTimer.IsRunning; }
        }

        /// <summary>
        /// A particle emitter that emits particles from a point, 
        /// </summary>
        /// <param name="x">The x position of the top-left corner of the emitter.</param>
        /// <param name="y">The y position of the top-left corner of the emitter.</param>
        public GenEmitter(float x, float y)
        {
            _position = new Vector2(x, y);
            _bounds = new GenAABB(x, y, 0f, 0f);
            _boundingRect = Rectangle.Empty;
            MinParticleSpeedX = -100;
            MaxParticleSpeedX = 100;
            MinParticleSpeedY = -100;
            MaxParticleSpeedY = 100;
            MinRotation = 0;
            MaxRotation = 0;
            MinRotationSpeed = 0;
            MaxRotationSpeed = 0;
            Colors = new List<Color>();
            StartAlpha = 1f;
            EndAlpha = 1f;
            StartScale = 1f;
            EndScale = 1f;
            Explode = true;
            EmitQuantity = 10;
            _emitTimer = new GenTimer(0.1f, EmitParticles);
            _emitTimer.IsLooping = true;
            InheritVelocity = false;
            Parent = null;
            ParentOffset = Vector2.Zero;
        }

        /// <summary>
        /// Updates the emitter timer, and emits particles at each frequency interval.
        /// </summary>
        public override void Update()
        {
            _emitTimer.Update();

            base.Update();

            float lerp;
            int lastColorIndex = Colors.Count - 1;
            int currentColorIndex;
            float colorLerp;

            foreach (GenParticle particle in Members)
            {
                lerp = particle.LifeTimer.Elapsed / particle.LifeTimer.Duration;

                if (!particle.Flickering)
                {
                    if (Colors.Count > 0)
                    {
                        // Interpolate the particle's color over the span of it's lifetime.
                        currentColorIndex = (int)(lerp * lastColorIndex);
                        colorLerp = (lerp - ((1f / lastColorIndex) * currentColorIndex)) / (1f / lastColorIndex);

                        if (currentColorIndex < lastColorIndex)
                            particle.Color = Color.Lerp(Colors[currentColorIndex], Colors[currentColorIndex + 1], colorLerp);
                        else
                            particle.Color = Colors[lastColorIndex];
                    }

                    // Interpolate the sprite alpha.
                    particle.Alpha = (StartAlpha == EndAlpha) ? StartAlpha : MathHelper.Lerp(StartAlpha, EndAlpha, lerp);
                }

                // Interpolate the sprite scale.
                particle.Scale.X = particle.Scale.Y = (StartScale == EndScale) ? StartScale : MathHelper.Lerp(StartScale, EndScale, lerp);
            }
        }

        /// <summary>
        /// Handles any post-update logic for the emitter.
        /// </summary>
        public override void PostUpdate()
        {
            base.PostUpdate();

            MoveToParent();
        }

        /// <summary>
        /// Draws a box that represents the bounding box of the emitter in debug mode.
        /// </summary>
        /// <param name="camera">The camera used to draw.</param>
        public override void DrawDebug(GenCamera camera)
        {
            if ((camera != null) && !CanDraw(camera))
                return;

            GenG.SpriteBatch.Draw(
                GenG.Pixel, 
                _position, 
                _boundingRect, 
                Color.BlueViolet * 0.5f);
        }

        /// <summary>
        /// Sets the position of the emitter to the parent's center position.
        /// </summary>
        protected void MoveToParent()
        {
            if (Parent != null)
            {
                X = Parent.OriginPosition.X + ParentOffset.X;
                Y = Parent.OriginPosition.Y + ParentOffset.Y;
            }
        }

        /// <summary>
        /// Emits a single particle, as long as one is available in the emitter group.
        /// </summary>
        /// <returns>True if a particle was emitted, false if not.</returns>
        public bool EmitParticle()
        {
            _currentParticle = GetFirstAvailable() as GenParticle;

            if (_currentParticle != null)
            {
                // Give the particle a random x and y position, keeping the center of the particle's bounding box within the bounding box of the emitter.
                _currentParticle.X = GenU.Random((int)(_bounds.Left - _currentParticle.Bounds.HalfWidth), (int)(_bounds.Right + _currentParticle.Bounds.HalfWidth + 1));
                _currentParticle.Y = GenU.Random((int)(_bounds.Top - _currentParticle.Bounds.HalfHeight), (int)(_bounds.Bottom + _currentParticle.Bounds.HalfHeight + 1));

                _currentParticle.Velocity.X = GenU.Random(MinParticleSpeedX, MaxParticleSpeedX + 1);
                _currentParticle.Velocity.Y = GenU.Random(MinParticleSpeedY, MaxParticleSpeedY + 1);

                if ((Parent != null) && InheritVelocity)
                    _currentParticle.Velocity += Parent.Velocity;

                _currentParticle.Rotation = GenU.Random(MinRotation, MaxRotation + 1);
                _currentParticle.RotationSpeed = GenU.Random(MinRotationSpeed, MaxRotationSpeed + 1);
                _currentParticle.Color = (Colors.Count > 0) ? Colors[0] : Color.White;
                _currentParticle.Alpha = StartAlpha;
                _currentParticle.Scale.X = _currentParticle.Scale.Y = StartScale;
                _currentParticle.Reset();

                return true;
            }

            return false;
        }

        public void EmitParticles()
        {
            if (Explode)
                EmitParticles(Members.Count);
            else
                EmitParticles(EmitQuantity);
        }

        /// <summary>
        /// Emits a number of particles relative to the given emit quantity, as long as they are available in the emitter group.
        /// </summary>
        public void EmitParticles(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                if (!EmitParticle())
                    return;
            }
        }

        /// <summary>
        /// Adds a particle to the emitter group to be used be the emitter.
        /// </summary>
        /// <param name="particle">The particle to add to the emitter group.</param>
        /// <returns>The particle that was added to the emitter group.</returns>
        public GenParticle Add(GenParticle particle)
        {
            particle.Kill();
            base.Add(particle);

            return particle;
        }

        /// <summary>
        /// Makes a number of particles using the given texture, and adds them to the emitter group.
        /// </summary>
        /// <param name="texture">The texture to assign to the particles.</param>
        /// <param name="width">The width of the particles.</param>
        /// <param name="height">The height of the particles.</param>
        /// <param name="count">The amount of particles to make.</param>
        public void MakeParticles(Texture2D texture, int width, int height, int count)
        {
            for (int i = 0; i < count; i++)
                Add(new GenParticle(0f, 0f, texture, width, height));
        }

        /// <summary>
        /// Sets the minimum and maximum x speeds allowed for particles as they are emitted.
        /// </summary>
        /// <param name="min">The minimum x velocity or acceleration allowed for particles.</param>
        /// <param name="max">The maximum x velocity or acceleration allowed for particles.</param>
        public void SetXSpeed(int min, int max)
        {
            MinParticleSpeedX = min;
            MaxParticleSpeedX = max;
        }

        /// <summary>
        /// Sets the minimum and maximum y speeds allowed for particles as they are emitted.
        /// </summary>
        /// <param name="min">The minimum y velocity or acceleration allowed for particles.</param>
        /// <param name="max">The maximum y velocity or acceleration allowed for particles.</param>
        public void SetYSpeed(int min, int max)
        {
            MinParticleSpeedY = min;
            MaxParticleSpeedY = max;
        }

        /// <summary>
        /// Sets the deceleration speed of each particle in the emitter group.
        /// </summary>
        /// <param name="decelerationX">The x deceleration speed of a particle.</param>
        /// <param name="decelerationY">The y deceleration speed of a particle.</param>
        public void SetDeceleration(float decelerationX, float decelerationY)
        {
            foreach (GenParticle particle in Members)
            {
                particle.Deceleration.X = decelerationX;
                particle.Deceleration.Y = decelerationY;
            }
        }

        /// <summary>
        /// Sets the minimum and maximum rotation allowed for particles as they are emitted.
        /// </summary>
        /// <param name="min">The minimum rotation allowed for particles.</param>
        /// <param name="max">The maximum rotation allowed for particles.</param>
        public void SetRotation(int min, int max)
        {
            MinRotation = min;
            MaxRotation = max;
        }

        /// <summary>
        /// Sets the minimum and maximum rotation speeds allowed for particles as they are emitted.
        /// </summary>
        /// <param name="min">The minimum rotation speed allowed for particles.</param>
        /// <param name="max">The maximum rotation speed allowed for particles.</param>
        public void SetRotationSpeed(int min, int max)
        {
            MinRotationSpeed = min;
            MaxRotationSpeed = max;
        }

        /// <summary>
        /// Sets the lifetime of each particle in the emitter group.
        /// The lifetime is the amount of time, in seconds, that the particle will last after being emitted.
        /// </summary>
        /// <param name="seconds">The lifetime of a particle, in seconds.</param>
        public void SetLifetime(float seconds)
        {
            foreach (GenParticle particle in Members)
                particle.LifeTimer.Duration = seconds;
        }

        /// <summary>
        /// Sets the starting and ending color alpha values of the particles.
        /// Each particle will interpolate its color alpha from the starting to the ending alpha values over the span of its lifetime.
        /// </summary>
        /// <param name="startAlpha">The starting alpha value of a particle.</param>
        /// <param name="endAlpha">The ending alpha value of a particle.</param>
        public void SetAlpha(float startAlpha, float endAlpha)
        {
            StartAlpha = startAlpha;
            EndAlpha = endAlpha;
        }


        /// <summary>
        /// Sets the starting and ending scale values of the particles.
        /// Each particle will interpolate its scale from the starting to the ending scale values over the span of its lifetime.
        /// </summary>
        /// <param name="startScale">The starting scale value of a particle.</param>
        /// <param name="endScale">The ending scale value of a particle.</param>
        public void SetScale(float startScale, float endScale)
        {
            StartScale = startScale;
            EndScale = endScale;
        }

        /// <summary>
        /// Sets the x and y accelerations of each particle in the emitter group.
        /// </summary>
        /// <param name="gravityX">The x acceleration of a particle.</param>
        /// <param name="gravityY">The y acceleration of a particle.</param>
        public void SetGravity(float gravityX, float gravityY)
        {
            foreach (GenParticle particle in Members)
            {
                particle.Acceleration.X = gravityX;
                particle.Acceleration.Y = gravityY;
            }
        }

        /// <summary>
        /// Sets the camera scroll factor of each particle in the emitter group.
        /// </summary>
        /// <param name="x">The horizontal scroll factor.</param>
        /// <param name="y">The vertical scroll factor.</param>
        public void SetScrollFactor(float x, float y)
        {
            foreach (GenParticle particle in Members)
            {
                particle.ScrollFactor.X = x;
                particle.ScrollFactor.Y = y;
            }
        }

        /// <summary>
        /// Starts the emitter.
        /// </summary>
        /// <param name="explode">A flag used to emit every available particle, ignoring the emit quantity.</param>
        public void Start(bool explode = false)
        {
            _emitTimer.Start(true);

            // Move the emitter relative to its parent before emitting particles.
            MoveToParent();

            Explode = explode;
            EmitParticles();
        }

        /// <summary>
        /// Stops the emitter from emitting more particles.
        /// </summary>
        public void Stop()
        {
            _emitTimer.Stop();
        }
    }
}
