using System;

using Microsoft.Xna.Framework;

namespace Genetic.Physics
{
    /// <summary>
    /// Provides methods to check for an overlap or collision between two game objects.
    /// Handles collision response between two colliding axis-aligned bounding box objects.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    class GenCollide
    {
        /// <summary>
        /// A global container used to store vector calculation results for the distance between two bounding boxes.
        /// Useful for reducing Vector2 allocations.
        /// </summary>
        private static Vector2 _distance;

        /// <summary>
        /// A global container used to store vector calculation results for the normal direction vector of a collision.
        /// Useful for reducing Vector2 allocations.
        /// </summary>
        private static Vector2 _collisionNormal;

        protected static GenCollideEvent _collideEventArgs = new GenCollideEvent(null, null, GenObject.Direction.None, GenObject.Direction.None);

        /// <summary>
        /// Checks for overlap between the movements bounds of two objects.
        /// </summary>
        /// <param name="objectA">The first object to check for an overlap.</param>
        /// <param name="objectB">The second object to check for an overlap.</param>
        /// <param name="callback">The delegate method that will be invoked if an overlap occurs.</param>
        /// <returns>True if an overlap occurs, false if not.</returns>
        public static bool Overlap(GenObject objectA, GenObject objectB, CollideEvent callback)
        {
            // Check if both objects are alive to avoid unwanted overlap checks that may be called by a quadtree.
            if ((objectA.Exists && objectA.Active) && (objectB.Exists && objectB.Active))
            {
                if (objectA.MoveBounds.Intersects(objectB.MoveBounds))
                {
                    if (callback != null)
                    {
                        _collideEventArgs.ObjectA = objectA;
                        _collideEventArgs.ObjectB = objectB;
                        _collideEventArgs.TouchingA = GenObject.Direction.None;
                        _collideEventArgs.TouchingB = GenObject.Direction.None;

                        callback(_collideEventArgs);
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Applies collision detection and response against two objects that may overlap.
        /// </summary>
        /// <param name="objectA">The first object to check for a collision.</param>
        /// <param name="objectB">The second object to check for a collision.</param>
        /// <param name="callback">The delegate method that will be invoked if a collision occurs.</param>
        /// <param name="collidableEdges">A bit field of flags determining which edges of the second object are collidable.</param>
        /// <returns>True if a collision occurs, false if not.</returns>
        public static bool Collide(GenObject objectA, GenObject objectB, CollideEvent callback, GenObject.Direction collidableEdges = GenObject.Direction.Any)
        {
            // Do not check for collisions if the objects are the same.
            if (objectA == objectB)
                return false;

            // Do not check for collisions if either object is not solid, or if both objects are immovable.
            if ((!objectA.Solid || !objectB.Solid) || (objectA.Immovable && objectB.Immovable))
                return false;

            // If the movement bounding boxes of each object are overlapping, check for a collision.
            if (Overlap(objectA, objectB, callback))
            {
                // Get the normal direction of the collision from bounding box A to B along the least penetrating axis.
                // Return no collision if the surface normal is projected towards a non-collidable edge of bounding box B.
                _distance = objectA.Bounds.GetDistanceAABB(objectB.Bounds);

                // The greatest absolute distance value between the two bounding boxes is along the axis of least penetration.
                if (_distance.X > _distance.Y)
                {
                    if (objectA.Bounds.MidpointX > objectB.Bounds.MidpointX)
                    {
                        // The collision normal is pointing from the left edge of bounding box A.
                        _collisionNormal.X = -1f;
                        _collisionNormal.Y = 0f;

                        if ((collidableEdges & GenObject.Direction.Right) == 0)
                            return false;
                    }
                    else
                    {
                        // The collision normal is pointing from the right edge of bounding box A.
                        _collisionNormal.X = 1f;
                        _collisionNormal.Y = 0f;

                        if ((collidableEdges & GenObject.Direction.Left) == 0)
                            return false;
                    }
                }
                else
                {
                    if (objectA.Bounds.MidpointY > objectB.Bounds.MidpointY)
                    {
                        // The collision normal is pointing from the top edge of bounding box A.
                        _collisionNormal.X = 0f;
                        _collisionNormal.Y = -1f;

                        if ((collidableEdges & GenObject.Direction.Down) == 0)
                            return false;
                    }
                    else
                    {
                        // The collision normal is pointing from the bottom edge of bounding box A.
                        _collisionNormal.X = 0f;
                        _collisionNormal.Y = 1f;

                        if ((collidableEdges & GenObject.Direction.Up) == 0)
                            return false;
                    }
                }

                float distance = Math.Max(_distance.X, _distance.Y);
                float remove;

                // Apply an alternative collision response against tiles for pixel-perfect accuracy.
                if (objectB is GenTile)
                {
                    // Get the amount of normal velocity to remove from the object so that it just touches along the collision surface of the tile.
                    remove = Vector2.Dot(-objectA.Velocity, _collisionNormal) + Math.Max(distance, 0f) * GenG.InverseTimeStep;

                    // If the amount of velocity to remove is positive, the object will separate itself from the tile.
                    if (remove >= 0)
                        return false;

                    if (_collisionNormal.X != 0)
                    {
                        objectA.X = (_collisionNormal.X == 1) ? objectB.X - objectA.Bounds.Width : objectB.Bounds.Right;
                        objectA.Velocity.X = 0f;
                    }
                    else
                    {
                        objectA.Y = (_collisionNormal.Y == 1) ? objectB.Y - objectA.Bounds.Height : objectB.Bounds.Bottom;
                        objectA.Velocity.Y = 0f;
                    }
                }
                else
                {
                    // Get the relative velocity along the collision normal.
                    float relativeNormalVelocity = Vector2.Dot(objectB.Velocity - objectA.Velocity, _collisionNormal);

                    // Get the amount of relative normal velocity to remove so that the bounding boxes are just touching along the collision surface.
                    remove = relativeNormalVelocity + distance * GenG.InverseTimeStep;

                    // If the amount of velocity to remove is positive, the objects will separate themselves.
                    if (remove >= 0)
                        return false;

                    float impulse = remove / (objectA.InverseMass + objectB.InverseMass);

                    if (!objectA.Immovable)
                        objectA.Velocity += impulse * _collisionNormal * objectA.InverseMass;

                    if (!objectB.Immovable)
                        objectB.Velocity -= impulse * _collisionNormal * objectB.InverseMass;
                }

                // Use a bit field of flags to provide the direction that each object is colliding in during the current collision.
                GenObject.Direction touchingA = GenObject.Direction.None;
                GenObject.Direction touchingB = GenObject.Direction.None;

                if (_collisionNormal.X != 0)
                {
                    if (_collisionNormal.X == 1)
                    {
                        objectA.Touching |= GenObject.Direction.Right;
                        objectB.Touching |= GenObject.Direction.Left;

                        touchingA |= GenObject.Direction.Right;
                        touchingB |= GenObject.Direction.Left;
                    }
                    else
                    {
                        objectA.Touching |= GenObject.Direction.Left;
                        objectB.Touching |= GenObject.Direction.Right;

                        touchingA |= GenObject.Direction.Left;
                        touchingB |= GenObject.Direction.Right;
                    }
                }
                else
                {
                    if (_collisionNormal.Y == 1)
                    {
                        objectA.Touching |= GenObject.Direction.Down;
                        objectB.Touching |= GenObject.Direction.Up;

                        touchingA |= GenObject.Direction.Down;
                        touchingB |= GenObject.Direction.Up;

                        if (objectB.IsPlatform)
                        {
                            if (objectA.Acceleration.X == 0)
                                objectA.Platform = objectB;
                        }
                    }
                    else
                    {
                        objectA.Touching |= GenObject.Direction.Up;
                        objectB.Touching |= GenObject.Direction.Down;

                        touchingA |= GenObject.Direction.Up;
                        touchingB |= GenObject.Direction.Down;

                        if (objectA.IsPlatform)
                        {
                            if (objectB.Acceleration.X == 0)
                                objectB.Platform = objectA;
                        }
                    }
                }

                if (callback != null)
                {
                    _collideEventArgs.ObjectA = objectA;
                    _collideEventArgs.ObjectB = objectB;
                    _collideEventArgs.TouchingA = touchingA;
                    _collideEventArgs.TouchingB = touchingB;

                    callback(_collideEventArgs);
                }

                return true;
            }

            return false;
        }
    }
}
