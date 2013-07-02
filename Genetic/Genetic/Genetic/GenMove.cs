using System;

using Microsoft.Xna.Framework;

namespace Genetic
{
    /// <summary>
    /// Provides various helper functions useful for moving game objects.
    /// </summary>
    public static class GenMove
    {
        /// <summary>
        /// A bit field of flags determining the allowed movement axis of an object.
        /// Used to constrain movement horizontally, vertically, or both.
        /// 
        /// Author: Tyler Gregory (GeneticSpartan)
        /// </summary>
        public enum Axis 
        { 
            /// <summary>
            /// A bit field representing a movement constraint along the x-axis.
            /// </summary>
            Horizontal = 0x01,

            /// <summary>
            /// A bit field representing a movement constraint along the y-axis.
            /// </summary>
            Vertical = 0x10,

            /// <summary>
            /// A bit field representing a movement constraint along both the x-axis and y-axis.
            /// </summary>
            Both = Axis.Horizontal | Axis.Vertical
        }

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
            Vector2 distance = Vector2.Zero;

            // Get the x and y distances between the object and the point.
            distance.X = ((axis & Axis.Horizontal) == Axis.Horizontal) ? (point.X - gameObject.Position.X) : 0f;
            distance.Y = ((axis & Axis.Vertical) == Axis.Vertical) ? (point.Y - gameObject.Position.Y) : 0f;

            // Return the normalized distance vector.
            return GenU.NormalizeVector2(distance);
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
            Vector2 distance = Vector2.Zero;

            // Get the x and y distances between the first point and the second point.
            distance.X = ((axis & Axis.Horizontal) == Axis.Horizontal) ? (pointB.X - pointA.X) : 0;
            distance.Y = ((axis & Axis.Vertical) == Axis.Vertical) ? (pointB.Y - pointA.Y) : 0;

            // Return the normalized distance vector.
            return GenU.NormalizeVector2(distance);
        }

        /// <summary>
        /// Gets a normal vector in the direction of the given angle in degrees.
        /// </summary>
        /// <param name="angle">The angle, in degrees, to convert to a vector.</param>
        /// <returns>The normal vector calculated from the given angle.</returns>
        public static Vector2 AngleToVector(float angle)
        {
            Vector2 angleVector = Vector2.Zero;
            angle = MathHelper.ToRadians(angle);

            angleVector.X = (float)Math.Sin(angle);
            angleVector.Y = -(float)Math.Cos(angle);

            return angleVector;
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
            Vector2 distance = pointB - pointA;

            return MathHelper.ToDegrees((float)Math.Atan2(distance.X, -distance.Y));
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
                    if (allowImmovable || !(objectOrGroup as GenObject).Immovable)
                    {
                        // Get a normalized distance vector to calculate the horizontal and vertical speeds.
                        Vector2 distanceNormal = GetDistanceNormal(objectOrGroup as GenObject, point, axis);

                        (objectOrGroup as GenObject).Velocity = distanceNormal * speed;
                    }
                }
                else if (objectOrGroup is GenGroup)
                {
                    foreach (GenBasic basic in (objectOrGroup as GenGroup).Members)
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
                    if (allowImmovable || !(objectOrGroup as GenObject).Immovable)
                    {
                        // Get a normalized distance vector to calculate the horizontal and vertical speeds.
                        Vector2 distanceNormal = GetDistanceNormal(objectOrGroup as GenObject, point, axis);

                        if (radius <= 0)
                            (objectOrGroup as GenObject).Velocity += distanceNormal * speed * GenG.TimeStep;
                        else
                        {
                            // If the object is within the radius from the point, accelerate the object towards the point.
                            // The closer the object is to the point, the higher its acceleration will be.
                            float accelerationFactor = MathHelper.Clamp(radius - Vector2.Distance((objectOrGroup as GenObject).CenterPosition, point), 0, 1);

                            (objectOrGroup as GenObject).Velocity += distanceNormal * speed * accelerationFactor * GenG.TimeStep;
                        }
                    }
                }
                else if (objectOrGroup is GenGroup)
                {
                    foreach (GenBasic basic in (objectOrGroup as GenGroup).Members)
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
                    if (allowImmovable || !(objectOrGroup as GenObject).Immovable)
                    {
                        // Convert the angle to a normal vector to calculate the horizontal and vertical speeds.
                        Vector2 angleVector = AngleToVector(angle);

                        (objectOrGroup as GenObject).Velocity = angleVector * speed;
                    }
                }
                else if (objectOrGroup is GenGroup)
                {
                    foreach (GenBasic basic in (objectOrGroup as GenGroup).Members)
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
                    if (allowImmovable || !(objectOrGroup as GenObject).Immovable)
                    {
                        // Convert the angle to a normal vector to calculate the horizontal and vertical speeds.
                        Vector2 angleVector = AngleToVector(angle);

                        (objectOrGroup as GenObject).Acceleration = angleVector * speed;
                    }
                }
                else if (objectOrGroup is GenGroup)
                {
                    foreach (GenBasic basic in (objectOrGroup as GenGroup).Members)
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
            Vector2 rotationPosition = Vector2.Zero;

            rotationPosition = AngleToVector(angle);
            rotationPosition = point + rotationPosition * radius;

            gameObject.X = rotationPosition.X;
            gameObject.Y = rotationPosition.Y;
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

            Vector2 newPosition = Vector2.Zero;

            // Calculate the next position of the object using its current direction.
            // Use the current velocity of the object if speed is 0.
            if (speed == 0)
                newPosition = Vector2.Add(gameObject.Position, gameObject.Velocity * GenG.TimeStep);
            else
                newPosition = Vector2.Add(gameObject.Position, GenU.NormalizeVector2(gameObject.Velocity) * speed * GenG.TimeStep);

            // Calculate the distance that the object will move.
            float moveDistance = Vector2.Distance(gameObject.Position, newPosition);

            return (moveDistance + radius) >= minDistance ;
        }
    }
}
