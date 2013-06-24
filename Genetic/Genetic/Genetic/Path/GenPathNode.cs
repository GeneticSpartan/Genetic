using System;

using Microsoft.Xna.Framework;

namespace Genetic.Path
{
    /// <summary>
    /// Represents a single movement node within a path.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenPathNode
    {
        /// <summary>
        /// The x and y positions of the center point of the node.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The radius of the node extending from the center point position.
        /// Useful for creating a search area for an object reaching the node.
        /// </summary>
        protected float _radius;

        /// <summary>
        /// The callback method that will invoke when the node is reached.
        /// </summary>
        public Action Callback;

        /// <summary>
        /// Gets or sets the radius of the node extending from the center point position.
        /// Useful for creating a search area for an object reaching the node.
        /// </summary>
        public float Radius
        {
            get { return _radius; }

            set { _radius = Math.Max(0f, value); }
        }

        /// <summary>
        /// Creates a movement node used in constructing the points of a path.
        /// </summary>
        /// <param name="x">The x position of the center point of the node.</param>
        /// <param name="y">The y position of the center point of the node.</param>
        /// <param name="radius">The radius of the node extending from the center point position.</param>
        /// <param name="callback">The function that will invoke when the node is reached.</param>
        public GenPathNode(float x, float y, float radius = 0, Action callback = null)
        {
            Position = new Vector2(x, y);
            _radius = radius;
            Callback = callback;
        }
    }
}
