using System;

namespace Genetic
{
    /// <summary>
    /// The delegate for <c>GenCollideEvent</c>.
    /// </summary>
    /// <param name="e">The properties that are returned by the collision event.</param>
    public delegate void CollideEvent (GenCollideEvent e);

    /// <summary>
    /// An event that is triggered when a collision occurs between two game objects.
    /// Provides the two game objects involved in the collision, along with each of their colliding directions.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenCollideEvent : EventArgs
    {
        private GenObject _objectA;
        private GenObject _objectB;

        /// <summary>
        /// A bit field of flags giving the directions that the first object is colliding in.
        /// </summary>
        private GenObject.Direction _touchingA;

        /// <summary>
        /// A bit field of flags giving the directions that the second object is colliding in.
        /// </summary>
        private GenObject.Direction _touchingB;

        /// <summary>
        /// Gets the first object involved in the overlap.
        /// </summary>
        public GenObject ObjectA
        {
            get { return _objectA; }
        }

        /// <summary>
        /// Gets the second object involved in the overlap.
        /// </summary>
        public GenObject ObjectB
        {
            get { return _objectB; }
        }

        /// <summary>
        /// Gets a bit field of flags giving the direction that the first object is colliding in.
        /// </summary>
        public GenObject.Direction TouchingA
        {
            get { return _touchingA; }
        }

        /// <summary>
        /// Gets a bit field of flags giving the direction that the second object is colliding in.
        /// </summary>
        public GenObject.Direction TouchingB
        {
            get { return _touchingB; }
        }

        /// <summary>
        /// An event that is triggered when a collision occurs between two game objects.
        /// </summary>
        /// <param name="objectA">The first object involved in the collision.</param>
        /// <param name="objectB">The second object involved in the collision.</param>
        /// <param name="touchingA">The direction that the first object is colliding in.</param>
        /// <param name="touchingB">The direction that the second object is colliding in.</param>
        public GenCollideEvent(GenObject objectA, GenObject objectB, GenObject.Direction touchingA, GenObject.Direction touchingB)
        {
            _objectA = objectA;
            _objectB = objectB;
            _touchingA = touchingA;
            _touchingB = touchingB;

            // TODO: Return the force of the collision as well.
        }
    }
}