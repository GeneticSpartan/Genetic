using System;

namespace Genetic
{
    /// <summary>
    /// The delegate for callback methods provided to collision checks.
    /// </summary>
    /// <param name="e">The event arguments that are returned by a collision.</param>
    public delegate void CollideEvent (GenCollideEvent e);

    /// <summary>
    /// The event arguments that are returned when a collision occurs between two game objects.
    /// Provides the two game objects involved in the collision, along with each of their colliding directions.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenCollideEvent : EventArgs
    {
        /// <summary>
        /// The first object involved in the collision.
        /// </summary>
        public GenObject ObjectA;

        /// <summary>
        /// The second object involved in the collision.
        /// </summary>
        public GenObject ObjectB;

        /// <summary>
        /// A bit field of flags giving the directions that the first object is colliding in.
        /// </summary>
        public GenObject.Direction TouchingA;

        /// <summary>
        /// A bit field of flags giving the directions that the second object is colliding in.
        /// </summary>
        public GenObject.Direction TouchingB;

        /// <summary>
        /// Assigns the colliding objects and their touching directions.
        /// </summary>
        /// <param name="objectA">The first object involved in the collision.</param>
        /// <param name="objectB">The second object involved in the collision.</param>
        /// <param name="touchingA">The direction that the first object is colliding in.</param>
        /// <param name="touchingB">The direction that the second object is colliding in.</param>
        public GenCollideEvent(GenObject objectA, GenObject objectB, GenObject.Direction touchingA, GenObject.Direction touchingB)
        {
            ObjectA = objectA;
            ObjectB = objectB;
            TouchingA = touchingA;
            TouchingB = touchingB;

            // TODO: Return the force of the collision as well.
        }
    }
}