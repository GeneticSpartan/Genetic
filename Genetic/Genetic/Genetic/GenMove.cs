using System;

using Microsoft.Xna.Framework;

namespace Genetic
{
    public static class GenMove
    {
        /// <summary>
        /// A bit field of flags determining the allowed movement axis of an object.
        /// Used to constrain movement horizontally, vertically, or both.
        /// </summary>
        public enum Axis 
        { 
            Horizontal = 0x01,
            Vertical = 0x10,
            Both = Axis.Horizontal | Axis.Vertical
        }

        /// <summary>
        /// A global container used to store vector calculation results.
        /// Useful for reducing Vector2 allocations.
        /// </summary>
        private static Vector2 _vector = Vector2.Zero;

        /// <summary>
        /// Gets a normalized vector of the x and y distances from an object to a point.
        /// Useful for calculating the distribution between horizontal and vertical speeds needed to meet an overall speed.
        /// </summary>
        /// <param name="gameObject">The object to start the distance calculation from.</param>
        /// <param name="point">The point to end the distance calculation at.</param>
        /// <param name="axis">A bit field of flags to determine which axis should be included.</param>
        /// <returns>The normalized distance vector.</returns>
        public static Vector2 GetDistanceNormal(GenObject gameObject, Vector2 point, Axis axis = Axis.Both)
        {
            // Get the x and y distances between the object and the point.
            _vector.X = ((axis & Axis.Horizontal) == Axis.Horizontal) ? (point.X - gameObject.Position.X) : 0;
            _vector.Y = ((axis & Axis.Vertical) == Axis.Vertical) ? (point.Y - gameObject.Position.Y) : 0;

            // Return the normalized distance vector.
            return Vector2.Normalize(_vector);
        }

        /// <summary>
        /// Gets a normalized vector of the x and y distances from one point to another point.
        /// Useful for calculating the distribution between horizontal and vertical speeds needed to meet an overall speed.
        /// </summary>
        /// <param name="pointA">The object to start the distance calculation from.</param>
        /// <param name="pointB">The point to end the distance calculation at.</param>
        /// <param name="axis">A bit field of flags to determine which axis should be included.</param>
        /// <returns>The normalized distance vector.</returns>
        public static Vector2 GetDistanceNormal(Vector2 pointA, Vector2 pointB, Axis axis = Axis.Both)
        {
            // Get the x and y distances between the first point and the second point.
            _vector.X = ((axis & Axis.Horizontal) == Axis.Horizontal) ? (pointB.X - pointA.X) : 0;
            _vector.Y = ((axis & Axis.Vertical) == Axis.Vertical) ? (pointB.Y - pointA.Y) : 0;

            // Return the normalized distance vector.
            return Vector2.Normalize(_vector);
        }

        /// <summary>
        /// Gets a normal vector in the direction of the given angle in degrees.
        /// </summary>
        /// <param name="angle">The angle, in degrees, to convert to a vector.</param>
        /// <returns>The normal vector calculated from the given angle.</returns>
        public static Vector2 AngleToVector(float angle)
        {
            angle = MathHelper.ToRadians(angle);

            _vector.X = (float)Math.Sin(angle);
            _vector.Y = -(float)Math.Cos(angle);

            return _vector;
        }

        /// <summary>
        /// Gets an angle, in degrees, from the given vector.
        /// </summary>
        /// <param name="vector">The vector to convert to an angle.</param>
        /// <returns>The angle calculated from the given vector.</returns>
        public static float VectortoAngle(Vector2 vector)
        {
            return MathHelper.ToDegrees((float)Math.Atan2(vector.X, -vector.Y));
        }

        /// <summary>
        /// Gets an angle, in degrees, from a vector calculated from two given points.
        /// The vector is calculated from the first point to the second point.
        /// </summary>
        /// <param name="pointA">The first point used to calculate the vector.</param>
        /// <param name="pointB">The second point used to calculate the vector.</param>
        /// <returns>The angle calculated from the vector.</returns>
        public static float VectortoAngle(Vector2 pointA, Vector2 pointB)
        {
            _vector = pointB - pointA;

            return MathHelper.ToDegrees((float)Math.Atan2(_vector.X, -_vector.Y));
        }

        /// <summary>
        /// Sets the x and y velocities of an object or group of objects to move towards a given point.
        /// </summary>
        /// <param name="objectOrGroup">The object or group of objects to move.</param>
        /// <param name="point">The point to move towards.</param>
        /// <param name="speed">The velocity to move the object at.</param>
        /// <param name="axis">A bit field of flags to determine the allowed movement axis of the object.</param>
        /// <param name="allowImmovable">A flag used to determine if an object set to immovable will be affected.</param>
        public static void MoveToPoint(GenBasic objectOrGroup, Vector2 point, float speed, Axis axis = Axis.Both, bool allowImmovable = false)
        {
            if (speed != 0)
            {
                if (objectOrGroup is GenObject)
                {
                    if (allowImmovable || !((GenObject)objectOrGroup).Immovable)
                    {
                        // Get a normalized distance vector to calculate the horizontal and vertical speeds.
                        _vector = GetDistanceNormal((GenObject)objectOrGroup, point, axis);

                        ((GenObject)objectOrGroup).Velocity = _vector * speed;
                    }
                }
                else if (objectOrGroup is GenGroup)
                {
                    foreach (GenBasic basic in ((GenGroup)objectOrGroup).Members)
                        MoveToPoint(basic, point, speed, axis, allowImmovable);
                }
            }
        }

        /// <summary>
        /// Sets the x and y acceleration of an object or group of objects to move towards a given point.
        /// </summary>
        /// <param name="objectOrGroup">The object or group of objects to move.</param>
        /// <param name="point">The point to move towards.</param>
        /// <param name="speed">The acceleration to move the object at.</param>
        /// <param name="radius">The radius extending from the point. An object will need to be within this area to accelerate. A value of 0 will not use a radius.</param>
        /// <param name="axis">The allowed movement axis of the object.</param>
        /// <param name="allowImmovable">A flag used to determine if an object set to immovable will be affected.</param>
        public static void AccelerateToPoint(GenBasic objectOrGroup, Vector2 point, float speed, float radius = 0, Axis axis = Axis.Both, bool allowImmovable = false)
        {
            if (speed != 0)
            {
                if (objectOrGroup is GenObject)
                {
                    if (allowImmovable || !((GenObject)objectOrGroup).Immovable)
                    {
                        // Get a normalized distance vector to calculate the horizontal and vertical speeds.
                        _vector = GetDistanceNormal((GenObject)objectOrGroup, point, axis);

                        if (radius <= 0)
                            ((GenObject)objectOrGroup).Velocity += _vector * speed * GenG.TimeStep;
                        else
                        {
                            // If the object is within the radius from the point, accelerate the object towards the point.
                            // The closer the object is to the point, the higher its acceleration will be.
                            float accelerationFactor = MathHelper.Clamp(radius - Vector2.Distance(((GenObject)objectOrGroup).CenterPosition, point), 0, 1);

                            ((GenObject)objectOrGroup).Velocity += _vector * speed * accelerationFactor * GenG.TimeStep;
                        }
                    }
                }
                else if (objectOrGroup is GenGroup)
                {
                    foreach (GenBasic basic in ((GenGroup)objectOrGroup).Members)
                        AccelerateToPoint(basic, point, speed, radius, axis, allowImmovable);
                }
            }
        }

        /// <summary>
        /// Sets the x and y velocities of an object to move in a given angle direction.
        /// </summary>
        /// <param name="objectOrGroup">The object or group of objects to move.</param>
        /// <param name="angle">The angle direction, in degrees, to move the object in. A value of 0 would be up, and 90 would be to the right.</param>
        /// <param name="speed">The velocity to move the object at.</param>
        /// <param name="allowImmovable">A flag used to determine if an object set to immovable will be affected.</param>
        public static void MoveAtAngle(GenBasic objectOrGroup, float angle, float speed, bool allowImmovable = false)
        {
            if (speed != 0)
            {
                if (objectOrGroup is GenObject)
                {
                    if (allowImmovable || !((GenObject)objectOrGroup).Immovable)
                    {
                        // Convert the angle to a normal vector to calculate the horizontal and vertical speeds.
                        _vector = AngleToVector(angle);

                        ((GenObject)objectOrGroup).Velocity = _vector * speed;
                    }
                }
                else if (objectOrGroup is GenGroup)
                {
                    foreach (GenBasic basic in ((GenGroup)objectOrGroup).Members)
                        MoveAtAngle(basic, angle, speed, allowImmovable);
                }
            }
        }

        /// <summary>
        /// Sets the x and y acceleration of an object to move in a given angle direction.
        /// </summary>
        /// <param name="objectOrGroup">The object or group of objects to move.</param>
        /// <param name="angle">The angle direction, in degrees, to move the object in. A value of 0 would be up, and 90 would be to the right.</param>
        /// <param name="speed">The acceleration to move the object at.</param>
        /// <param name="allowImmovable">A flag used to determine if an object set to immovable will be affected.</param>
        public static void AccelerateAtAngle(GenBasic objectOrGroup, float angle, float speed, bool allowImmovable = false)
        {
            if (speed != 0)
            {
                if (objectOrGroup is GenObject)
                {
                    if (allowImmovable || !((GenObject)objectOrGroup).Immovable)
                    {
                        // Convert the angle to a normal vector to calculate the horizontal and vertical speeds.
                        _vector = AngleToVector(angle);

                        ((GenObject)objectOrGroup).Acceleration = _vector * speed;
                    }
                }
                else if (objectOrGroup is GenGroup)
                {
                    foreach (GenBasic basic in ((GenGroup)objectOrGroup).Members)
                        AccelerateAtAngle(basic, angle, speed, allowImmovable);
                }
            }
        }

        /// <summary>
        /// Rotates an object around a given point.
        /// </summary>
        /// <param name="gameObject">The object to rotate around the point.</param>
        /// <param name="point">The point to rotate around.</param>
        /// <param name="angle">The angle of rotation, in degrees.</param>
        /// <param name="radius">The distance from the point that the object will be placed.</param>
        public static void RotateAroundPoint(GenObject gameObject, Vector2 point, float angle, float radius)
        {
            _vector = AngleToVector(angle);
            _vector = point + _vector * radius;

            gameObject.X = _vector.X;
            gameObject.Y = _vector.Y;
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
        /// <param name="axis">The movement axis to include in the check.</param>
        /// <returns>True if the object will reach the point, false if not.</returns>
        public static bool CanReachPoint(GenObject gameObject, Vector2 point, float speed = 0, float radius = 0, Axis axis = Axis.Both)
        {
            // Calculate the minimum distance that the object will need to move to reach the point.
            float minDistanceX = ((axis & Axis.Horizontal) == Axis.Horizontal) ? (gameObject.Position.X - point.X) : 0;
            float minDistanceY = ((axis & Axis.Vertical) == Axis.Vertical) ? (gameObject.Position.Y - point.Y) : 0;
            float minDistance = (float)Math.Sqrt(minDistanceX * minDistanceX + minDistanceY * minDistanceY);

            // Calculate the next position of the object using its current direction.
            // Use the current velocity of the object if speed is 0.
            if (speed == 0)
                _vector = Vector2.Add(gameObject.Position, gameObject.Velocity * GenG.TimeStep);
            else
                _vector = Vector2.Add(gameObject.Position, Vector2.Normalize(gameObject.Velocity) * speed * GenG.TimeStep);

            // Calculate the distance that the object will move.
            float moveDistance = Vector2.Distance(gameObject.Position, _vector);

            return (moveDistance + radius) >= minDistance ;
        }
    }
}
