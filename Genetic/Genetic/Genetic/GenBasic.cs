namespace Genetic
{
    /// <summary>
    /// A basic game object which most other game objects will extend from.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public abstract class GenBasic
    {
        /// <summary>
        /// Determines if Update and Draw should be called by a state or group.
        /// </summary>
        public bool Exists;

        /// <summary>
        /// Determines if Update should be called by a state or group.
        /// </summary>
        public bool Active;

        /// <summary>
        /// Determines if Draw should be called by a state or group.
        /// </summary>
        public bool Visible;

        /// <summary>
        /// Initializes the basic object.
        /// </summary>
        public GenBasic()
        {
            Exists = true;
            Active = true;
            Visible = true;
        }

        /// <summary>
        /// Called once before <c>Update</c> during each game loop.
        /// Override this method to add pre-update logic.
        /// </summary>
        public virtual void PreUpdate()
        {

        }

        /// <summary>
        /// Called during each game loop.
        /// Override this method to add update logic.
        /// </summary>
        public virtual void Update()
        {

        }

        /// <summary>
        /// Called once after <c>Update</c> during each game loop.
        /// Override this method to add post-update logic.
        /// </summary>
        public virtual void PostUpdate()
        {

        }

        /// <summary>
        /// Override this method to add draw logic.
        /// </summary>
        public virtual void Draw()
        {

        }

        /// <summary>
        /// Override this method to add debug drawing logic.
        /// </summary>
        public virtual void DrawDebug()
        {

        }

        /// <summary>
        /// Sets Exists and Active to false to "kill" the object.
        /// </summary>
        public virtual void Kill()
        {
            Exists = false;
            Active = false;
        }

        /// <summary>
        /// Sets Exists and Active to true to "revive" the object.
        /// </summary>
        public virtual void Revive()
        {
            Exists = true;
            Active = true;
        }

        /// <summary>
        /// Override this method to add reset logic.
        /// </summary>
        public virtual void Reset()
        {
            Exists = true;
            Active = true;
            Visible = true;
        }
    }
}