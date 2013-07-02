using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genetic
{
    /// <summary>
    /// A single game state that represents the current state of the game when running.
    /// Only one state can be running at any one time.
    /// Storing a reference to the state can be useful for returning to that state later, without generating a new state.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenState : GenGroup
    {
        /// <summary>
        /// A list of all cameras managed by the state.
        /// </summary>
        public List<GenCamera> Cameras;

        /// <summary>
        /// The initial camera created during the creation of the state.
        /// </summary>
        public GenCamera Camera;

        /// <summary>
        /// The background color of the game window.
        /// </summary>
        public Color BgColor;

        /// <summary>
        /// A flag used to determine if the state has been created.
        /// Useful for preventing <c>Create</c> from being called more than once.
        /// </summary>
        protected bool _loaded;

        /// <summary>
        /// Gets a flag used to determine if the state has been created.
        /// Useful for preventing <c>Create</c> from being called more than once.
        /// </summary>
        public bool Loaded
        {
            get { return _loaded; }
        }

        /// <summary>
        /// Initializes the state.
        /// </summary>
        public GenState()
        {
            Cameras = new List<GenCamera>();

            // Set the default background color of the game window.
            BgColor = Color.CornflowerBlue;
        }

        /// <summary>
        /// Called once when the state is initially created.
        /// This is where initial game elements should be set up.
        /// Override this method to add additional creation logic.
        /// </summary>
        public virtual void Create()
        {
            // Add an initial camera that is the size of the game screen.
            AddCamera(new GenCamera(0, 0, GenG.Game.Width, GenG.Game.Height, GenG.Game.Zoom));

            _loaded = true;
        }

        /// <summary>
        /// Called once when the state starts running.
        /// Override this method to add additional start logic.
        /// </summary>
        public virtual void Start()
        {

        }

        /// <summary>
        /// Calls <c>PreUpdate</c> on this state's group if the game is not paused.
        /// Override this method to add additional pre-update logic.
        /// </summary>
        public override void PreUpdate()
        {
            if (!GenG.Paused)
                base.PreUpdate();
        }

        /// <summary>
        /// Calls <c>Update</c> on this state's group if the game is not paused.
        /// Override this method to add additional update logic.
        /// </summary>
        public override void Update()
        {
            if (!GenG.Paused)
                base.Update();
        }

        /// <summary>
        /// Calls <c>PostUpdate</c> on this state's group if the game is not paused.
        /// Calls <c>PostUpdate</c> on each game camera.
        /// Override this method to add additional post-update logic.
        /// </summary>
        public override void PostUpdate()
        {
            if (!GenG.Paused)
                base.PostUpdate();

            foreach (GenCamera camera in Cameras)
            {
                if (camera.Exists && camera.Active)
                    camera.PostUpdate();
            }
        }

        /// <summary>
        /// Adds a new camera to the state's cameras list.
        /// </summary>
        /// <param name="camera">The <c>GenCamera</c> to add.</param>
        /// <returns>The newly added <c>GenCamera</c>.</returns>
        public GenCamera AddCamera(GenCamera camera)
        {
            Cameras.Add(camera);

            // Assign the new camera to the initial camera if it has not already been assigned.
            if (Camera == null)
                Camera = camera;

            return camera;
        }

        /// <summary>
        /// Removes a camera from the cameras list.
        /// </summary>
        /// <param name="camera">The camera to remove.</param>
        /// <returns>The camera that was removed. Null if the camera does not exist in the cameras list.</returns>
        public GenCamera RemoveCamera(GenCamera camera)
        {
            if (Cameras.Remove(camera))
            {
                // If the camera being removed is the same as the initial camera, assign the initial camera to the first camera in the cameras list.
                if (Camera == camera)
                    Camera = Cameras[0];

                return camera;
            }

            return null;
        }
    }
}