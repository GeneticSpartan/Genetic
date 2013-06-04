using System;

namespace Genetic
{
    public delegate void CollideEvent (GenCollideEvent e);

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
        /// Gets a bit field of flags giving the directions that the first object is colliding in.
        /// </summary>
        public GenObject.Direction TouchingA
        {
            get { return _touchingA; }
        }

        /// <summary>
        /// Gets a bit field of flags giving the directions that the second object is colliding in.
        /// </summary>
        public GenObject.Direction TouchingB
        {
            get { return _touchingB; }
        }

        public GenCollideEvent(GenObject objectA, GenObject objectB, GenObject.Direction touchingA, GenObject.Direction touchingB)
        {
            _objectA = objectA;
            _objectB = objectB;
            _touchingA = touchingA;
            _touchingB = touchingB;
        }
    }
}