namespace Genetic
{
    public class GenBasic
    {
        /// <summary>
        /// Determines if Update and Draw should be called by a state or group.
        /// </summary>
        public bool Exists = true;

        /// <summary>
        /// Determines if Update should be called by a state or group.
        /// </summary>
        public bool Active = true;

        /// <summary>
        /// Determines if Draw should be called by a state or group.
        /// </summary>
        public bool Visible = true;

        public GenBasic()
        {

        }

        /// <summary>
        /// Override this method to add update logic.
        /// </summary>
        public virtual void Update()
        {

        }

        /// <summary>
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