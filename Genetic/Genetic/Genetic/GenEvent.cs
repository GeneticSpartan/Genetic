using System;

namespace Genetic
{
    public delegate void CollideEvent (GenCollideEvent e);

    public class GenCollideEvent : EventArgs
    {
        private GenObject _object1;
        private GenObject _object2;

        /// <summary>
        /// Gets the first object involved in the overlap.
        /// </summary>
        public GenObject Object1
        {
            get { return _object1; }
        }

        /// <summary>
        /// Gets the second object involved in the overlap.
        /// </summary>
        public GenObject Object2
        {
            get { return _object2; }
        }

        public GenCollideEvent(GenObject object1, GenObject object2)
        {
            _object1 = object1;
            _object2 = object2;
        }
    }
}