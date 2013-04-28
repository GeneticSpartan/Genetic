using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Genetic.Geometry;

namespace Genetic.Particles
{
    public class GenEmitter : GenGroup
    {
        /// <summary>
        /// The x and y positions of the top-left corner of the emitter.
        /// </summary>
        protected Vector2 _position;

        /// <summary>
        /// The bounding box within which particles will emit.
        /// </summary>
        protected GenAABB _boundingBox;

        /// <summary>
        /// The minimum x speed allowed for particles as they are emitted.
        /// </summary>
        public int MinParticleSpeedX;

        /// <summary>
        /// The minimum x speed allowed for particles as they are emitted.
        /// </summary>
        public int MaxParticleSpeedX;

        /// <summary>
        /// The minimum y speed allowed for particles as they are emitted.
        /// </summary>
        public int MinParticleSpeedY;

        /// <summary>
        /// The minimum y speed allowed for particles as they are emitted.
        /// </summary>
        public int MaxParticleSpeedY;

        /// <summary>
        /// The minimum rotation speed allowed for particles as they are emitted.
        /// </summary>
        public int MinRotationSpeed;

        /// <summary>
        /// The maximum rotation speed allowed for particles as they are emitted.
        /// </summary>
        public int MaxRotationSpeed;

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
        /// The amount of time, in seconds, to wait for the next particle emission.
        /// </summary>
        public float EmitFrequency;

        /// <summary>
        /// The amount of time, in seconds, that has elapsed since the last particle emission.
        /// </summary>
        protected float _emitTimer;

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
        /// The position of the emitter will move with the center point of the parent object.
        /// </summary>
        public GenObject Parent;

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
                _boundingBox.X = value;
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
                _boundingBox.Y = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the bounding box within which particles will emit.
        /// </summary>
        public float Width
        {
            get { return _boundingBox.Width; }

            set { _boundingBox.Width = value; }
        }

        /// <summary>
        /// Gets or sets the width of the bounding box within which particles will emit.
        /// </summary>
        public float Height
        {
            get { return _boundingBox.Height; }

            set { _boundingBox.Height = value; }
        }

        /// <summary>
        /// A particle emitter that emits particles from a point, 
        /// </summary>
        /// <param name="x">The x position of the top-left corner of the emitter.</param>
        /// <param name="y">The y position of the top-left corner of the emitter.</param>
        public GenEmitter(float x = 0, float y = 0)
        {
            _position = new Vector2(x, y);
            _boundingBox = new GenAABB(x, y, 0, 0);
            MinParticleSpeedX = -100;
            MaxParticleSpeedX = 100;
            MinParticleSpeedY = -100;
            MaxParticleSpeedY = 100;
            MinRotationSpeed = 0;
            MaxRotationSpeed = 0;
            Explode = true;
            EmitQuantity = 0;
            EmitFrequency = 0.1f;
            _emitTimer = 0f;
            InheritVelocity = false;
            Parent = null;

            Active = false;
        }

        public override void Update()
        {
            if (_emitTimer >= EmitFrequency)
            {
                if (Explode)
                    EmitParticles(Members.Count);
                else
                    EmitParticles(EmitQuantity);

                _emitTimer -= EmitFrequency;
            }
            else
                _emitTimer += GenG.PhysicsTimeStep;

            base.Update();
        }

        public override void PostUpdate()
        {
            MoveToParent();
        }

        /// <summary>
        /// Sets the position of the emitter to the parent's center position.
        /// </summary>
        protected void MoveToParent()
        {
            if (Parent != null)
            {
                X = Parent.Position.X;
                Y = Parent.Position.Y;
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
                _currentParticle.X = GenU.Random((int)_boundingBox.Left, (int)(_boundingBox.Right + 1));
                _currentParticle.Y = GenU.Random((int)_boundingBox.Top, (int)(_boundingBox.Bottom + 1));
                _currentParticle.Velocity.X = GenU.Random(MinParticleSpeedX, MaxParticleSpeedX + 1);
                _currentParticle.Velocity.Y = GenU.Random(MinParticleSpeedY, MaxParticleSpeedY + 1);

                if ((Parent != null) && InheritVelocity)
                    _currentParticle.Velocity += Parent.Velocity;

                _currentParticle.RotationSpeed = GenU.Random(MinRotationSpeed, MaxRotationSpeed + 1);
                _currentParticle.Reset();

                return true;
            }

            return false;
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
        /// <param name="texture"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="count"></param>
        public void MakeParticles(Texture2D texture, int width, int height, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                GenParticle particle = new GenParticle(0, 0, null, width, height);
                particle.LoadTexture(texture);
                Add(particle);
            }
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
            foreach (GenParticle partical in Members)
                partical.Lifetime = seconds;
        }

        /// <summary>
        /// Sets the starting and ending color tints of each particle in the emitter group.
        /// Each particle will interpolate its color from the starting to the ending color tints over the span of its lifetime.
        /// </summary>
        /// <param name="startAlpha">The starting color tint of a particle.</param>
        /// <param name="endAlpha">The ending color tint of a particle.</param>
        public void SetColor(Color startColor, Color endColor)
        {
            foreach (GenParticle partical in Members)
            {
                partical.StartColor = startColor;
                partical.EndColor = endColor;
            }
        }

        /// <summary>
        /// Sets the starting and ending color alpha values of each particle in the emitter group.
        /// Each particle will interpolate its color alpha from the starting to the ending alpha values over the span of its lifetime.
        /// </summary>
        /// <param name="startAlpha">The starting alpha value of a particle.</param>
        /// <param name="endAlpha">The ending alpha value of a particle.</param>
        public void SetAlpha(float startAlpha, float endAlpha)
        {
            foreach (GenParticle partical in Members)
            {
                partical.StartAlpha = startAlpha;
                partical.EndAlpha = endAlpha;
            }
        }

        /// <summary>
        /// Sets the starting and ending scale values of each particle in the emitter group.
        /// Each particle will interpolate its scale from the starting to the ending scale values over the span of its lifetime.
        /// </summary>
        /// <param name="startScale">The starting scale value of a particle.</param>
        /// <param name="endScale">The ending scale value of a particle.</param>
        public void SetScale(float startScale, float endScale)
        {
            foreach (GenParticle partical in Members)
            {
                partical.StartScale = startScale;
                partical.EndScale = endScale;
            }
        }

        /// <summary>
        /// Sets the x and y accelerations of each particle in the emitter group.
        /// </summary>
        /// <param name="gravityX">The x acceleration of a particle.</param>
        /// <param name="gravityY">The y acceleration of a particle.</param>
        public void SetGravity(float gravityX, float gravityY)
        {
            foreach (GenParticle partical in Members)
            {
                partical.Acceleration.X = gravityX;
                partical.Acceleration.Y = gravityY;
            }
        }

        /// <summary>
        /// Starts the emitter.
        /// </summary>
        /// <param name="explode">A flag used to emit every available particle, ignoring the emit quantity.</param>
        public void Start(bool explode = false)
        {
            Active = true;

            // Move the emitter relative to its parent before emitting particles.
            MoveToParent();

            Explode = explode;

            if (Explode)
                EmitParticles(Members.Count);
            else
                EmitParticles(EmitQuantity);
        }
    }
}
