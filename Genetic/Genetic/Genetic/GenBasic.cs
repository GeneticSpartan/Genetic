using System.Collections.Generic;

using Microsoft.Xna.Framework;

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
        /// A list of cameras associated with this object.
        /// </summary>
        public List<GenCamera> Cameras;

        /// <summary>
        /// Initializes the basic object.
        /// </summary>
        public GenBasic()
        {
            Exists = true;
            Active = true;
            Visible = true;
            Cameras = null;
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
        /// <param name="camera">The camera used to draw.</param>
        public virtual void Draw(GenCamera camera)
        {
            
        }

        /// <summary>
        /// Override this method to add debug drawing logic.
        /// </summary>
        /// <param name="camera">The camera used to draw.</param>
        public virtual void DrawDebug(GenCamera camera)
        {
            
        }

        /// <summary>
        /// Checks if the object can be drawn to the specified camera.
        /// </summary>
        /// <param name="camera">The camera to check.</param>
        /// <returns>True if the object is associated with the specified camera, false if not.</returns>
        public bool CanDraw(GenCamera camera)
        {
            // If the cameras list has not been initialized, associate the object with each camera in the current state.
            if (Cameras == null)
            {
                Cameras = new List<GenCamera>();

                foreach (GenCamera stateCamera in GenG.State.Cameras)
                    Cameras.Add(stateCamera);
            }

            return Cameras.Contains(camera);
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