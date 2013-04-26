using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace Genetic
{
    public class GenState : GenGroup
    {
        public GenState()
        {
            
        }

        /// <summary>
        /// Override this method to add additional creation logic.
        /// </summary>
        public virtual void Create()
        {
            
        }

        /// <summary>
        /// Override this method to add additional update logic.
        /// </summary>
        public override void Update()
        {
            if (!GenG.Paused)
                base.Update();
        }

        /// <summary>
        /// Override this method to add additional post-update logic.
        /// </summary>
        public override void PostUpdate()
        {
            foreach (GenCamera camera in GenG.Cameras)
            {
                if (camera.Exists && camera.Active)
                    camera.PostUpdate();
            }
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}