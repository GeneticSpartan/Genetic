using System;

namespace Genetic
{
    public delegate void CollideEvent (GenCollideEvent e);

    public class GenCollideEvent : EventArgs
    {
        private GenObject _objectA;
        private GenObject _objectB;

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

        public GenCollideEvent(GenObject objectA, GenObject objectB)
        {
            _objectA = objectA;
            _objectB = objectB;
        }
    }
}