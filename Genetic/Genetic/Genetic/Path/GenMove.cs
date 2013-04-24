using Microsoft.Xna.Framework;

namespace Genetic.Path
{
    public static class GenMove
    {
        /// <summary>
        /// A global container used to store vector calculation results.
        /// Useful for reducing Vector2 allocations.
        /// </summary>
        public static Vector2 Vector = Vector2.Zero;

        /// <summary>
        /// Sets the x and y velocities of an object or group of objects to move towards a given point.
        /// </summary>
        /// <param name="objectOrGroup">The object or group of objects to move.</param>
        /// <param name="point">The point to move towards.</param>
        /// <param name="speed">The velocity to move the object at.</param>
        public static void MoveToPoint(GenBasic objectOrGroup, Vector2 point, float speed)
        {
            if (speed != 0)
            {
                if (objectOrGroup is GenObject)
                {
                    // Get the x and y distances between the object and the point.
                    Vector.X = point.X - ((GenObject)objectOrGroup).Position.X;
                    Vector.Y = point.Y - ((GenObject)objectOrGroup).Position.Y;

                    // Normalize the distance vector to calculate the horizontal and vertical speeds.
                    Vector = Vector2.Normalize(Vector);

                    ((GenObject)objectOrGroup).Velocity = Vector * speed;
                }
                else if (objectOrGroup is GenGroup)
                {
                    foreach (GenBasic basic in ((GenGroup)objectOrGroup).Members)
                        MoveToPoint(basic, point, speed);
                }
            }
        }

        /// <summary>
        /// Sets the x and y acceleration of an object or group of objects to move towards a given point.
        /// </summary>
        /// <param name="objectOrGroup">The object or group of objects to move.</param>
        /// <param name="point">The point to move towards.</param>
        /// <param name="speed">The acceleration to move the object at.</param>
        public static void AccelerateToPoint(GenBasic objectOrGroup, Vector2 point, float speed)
        {
            if (speed != 0)
            {
                if (objectOrGroup is GenObject)
                {
                    // Get the x and y distances between the object and the point.
                    Vector.X = point.X - ((GenObject)objectOrGroup).Position.X;
                    Vector.Y = point.Y - ((GenObject)objectOrGroup).Position.Y;

                    // Normalize the distance vector to calculate the horizontal and vertical speeds.
                    Vector = Vector2.Normalize(Vector);

                    ((GenObject)objectOrGroup).Acceleration = Vector * speed;
                }
                else if (objectOrGroup is GenGroup)
                {
                    foreach (GenBasic basic in ((GenGroup)objectOrGroup).Members)
                        AccelerateToPoint(basic, point, speed);
                }
            }
        }

        /// <summary>
        /// Determines if an object will reach a specified point on the next update.
        /// Uses the current position and movement direction of the object, and a given speed.
        /// The current velocity of the object will be used if the given speed is 0.
        /// </summary>
        /// <param name="gameObject">The moving object to check.</param>
        /// <param name="point">The point needed to reach.</param>
        /// <param name="speed">The velocity to move the object at. A value of 0 will use the current velocity of the object.</param>
        /// <param name="radius">The radius extending from the point position to use as a search area.</param>
        /// <returns>True if the object will reach the point, false if not.</returns>
        public static bool CanReachPoint(GenObject gameObject, Vector2 point, float speed = 0, float radius = 0)
        {
            // Calculate the minimum distance that the object will need to move to reach the point.
            float minDistance = Vector2.Distance(gameObject.Position, point);

            // Calculate the next position of the object using its current direction.
            // Use the current velocity of the object if speed is 0.
            if (speed == 0)
                Vector = Vector2.Add(gameObject.Position, gameObject.Velocity * GenG.PhysicsTimeStep);
            else
                Vector = Vector2.Add(gameObject.Position, Vector2.Normalize(gameObject.Velocity) * speed * GenG.PhysicsTimeStep);

            // Calculate the distance that the object will move.
            float moveDistance = Vector2.Distance(gameObject.Position, Vector);

            return (moveDistance + radius) >= minDistance ;
        }
    }
}
