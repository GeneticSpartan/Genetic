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
            base.Update();

            foreach (GenCamera camera in GenG.Cameras)
                camera.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}