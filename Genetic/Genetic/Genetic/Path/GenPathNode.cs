using System;

using Microsoft.Xna.Framework;

namespace Genetic.Path
{
    public class GenPathNode
    {
        /// <summary>
        /// The x and y positions of the center point of the node.
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        /// <summary>
        /// The radius of the node extending from the center point position.
        /// Useful for creating a search area for an object reaching the node.
        /// </summary>
        protected float _radius;

        /// <summary>
        /// The callback function that will invoke when the node is reached.
        /// </summary>
        public Action Callback;

        public float Radius
        {
            get { return _radius; }

            set { _radius = Math.Max(0, value); }
        }

        /// <summary>
        /// A movement node used in constructing the points of a path.
        /// </summary>
        /// <param name="x">The x position of the center point of the node.</param>
        /// <param name="y">The y position of the center point of the node.</param>
        /// <param name="radius">The radius of the node extending from the center point position.</param>
        /// <param name="callback">The function that will invoke when the node is reached.</param>
        public GenPathNode(float x, float y, float radius = 0, Action callback = null)
        {
            Position.X = x;
            Position.Y = y;
            _radius = radius;
            Callback = callback;
        }
    }
}
