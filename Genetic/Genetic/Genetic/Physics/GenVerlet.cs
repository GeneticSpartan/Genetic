using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Genetic.Physics
{
    /// <summary>
    /// Manages a group of points that can be connected together in various arrangments by link constraints using verlet integration physics.
    /// Useful for creating simulations such as ropes, cloth, or collision bodies.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenVerlet : GenGroup
    {
        /// <summary>
        /// A list of link constraints.
        /// </summary>
        public List<GenLink> Links;

        /// <summary>
        /// Determines if lines are drawn between object points for each link constraint.
        /// </summary>
        public bool DrawLines;

        /// <summary>
        /// The color of the lines drawn between object points for each link constraint.
        /// </summary>
        public Color LineColor;

        /// <summary>
        /// The thickness of the lines drawn between object points for each link constraint, in pixels.
        /// </summary>
        public float LineThickness;

        /// <summary>
        /// The number of physics iterations used to solve the verlet integration in one update.
        /// More iterations will cause the simulation to become less springy, which can be useful for rope physics.
        /// </summary>
        public int Iterations;

        /// <summary>
        /// A group used for managing objects that can be connected together using link constraints.
        /// Each link constraint pushes or pulls the two connected objects to a specified distance from each other.
        /// Useful for simulating ropes, cloth, or other elastic bodies.
        /// </summary>
        public GenVerlet()
        {
            Links = new List<GenLink>();
            DrawLines = true;
            LineColor = Color.White;
            LineThickness = 1f;
            Iterations = 1;
        }

        /// <summary>
        /// Calls <c>Update</c> on this verlat chain's group.
        /// Calls <c>Update</c> on each link constraint in the verlet chain for a specified number of iterations.
        /// </summary>
        public override void Update()
        {
            base.Update();

            for (int i = 0; i < Iterations; i++)
            {
                foreach (GenLink link in Links)
                    link.Update();
            }
        }

        /// <summary>
        /// Calls <c>Draw</c> on this verlet chain's group.
        /// Draws lines that represent each link constraint in the verlet chain.
        /// </summary>
        /// <param name="camera">The camera used to draw.</param>
        public override void Draw(GenCamera camera)
        {
            base.Draw(camera);

            if (DrawLines)
            {
                foreach (GenLink link in Links)
                    GenG.DrawLine(link.PointA.Position + link.OffsetA, link.PointB.Position + link.OffsetB, LineColor, LineThickness);
            }
        }

        /// <summary>
        /// Creates a grid of points, each connected by a link constraint to their adjacent neighboring points.
        /// Clears the verlet group members list and links list before creating the grid.
        /// </summary>
        /// <param name="startX">The x position of the top-left corner of the grid.</param>
        /// <param name="startY">The y position of the top-left corner of the grid.</param>
        /// <param name="distance">This distance between each neighboring point.</param>
        /// <param name="rows">The number of points spanning vertically.</param>
        /// <param name="columns">The number of points spanning horizontally.</param>
        public void MakeGrid(float startX, float startY, float distance, int rows, int columns)
        {
            Members.Clear();
            Links.Clear();

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    GenObject point = Add(new GenSprite(startX + x * distance, startY + y * distance, null, 1, 1)) as GenObject;

                    // Make a link to the left point.
                    if (x != 0)
                        MakeLink(Members[x + (columns * y) - 1] as GenObject, point);

                    // Make a link to the above point.
                    if (y != 0)
                        MakeLink(Members[x + columns * (y - 1)] as GenObject, point);
                }
            }
        }

        /// <summary>
        /// Make a link constraint connecting two object points.
        /// </summary>
        /// <param name="pointA">The first object to use as a connection point.</param>
        /// <param name="pointB">The second object to use as a connection point.</param>
        public void MakeLink(GenObject pointA, GenObject pointB)
        {
            Add(pointA);
            Add(pointB);

            Links.Add(new GenLink(pointA, pointB));
        }

        /// <summary>
        /// Sets the mass of every object in the verlet group.
        /// </summary>
        /// <param name="mass">The mass value to set each object.</param>
        public void SetMass(float mass)
        {
            foreach (GenObject gameObject in Members)
                gameObject.Mass = mass;
        }

        /// <summary>
        /// Sets the resting distance of every link constraint in the links list.
        /// </summary>
        /// <param name="restingDistance">The resting distance value to set each link constraint.</param>
        public void SetRestingDistance(float restingDistance)
        {
            foreach (GenLink link in Links)
                link.RestingDistance = restingDistance;
        }

        /// <summary>
        /// Sets the stiffness of every link constraint in the links list.
        /// The higher the stiffness value, the stiffer the constraint.
        /// Keep the value between 0.0 and 1.0 to retain stability.
        /// </summary>
        /// <param name="stiffness">The stiffness value to set each link constraint, a value from 0.0 to 1.0.</param>
        public void SetStiffness(float stiffness)
        {
            foreach (GenLink link in Links)
                link.Stiffness = stiffness;
        }

        /// <summary>
        /// Sets the x and y acceleration of each object in the verlet group.
        /// </summary>
        /// <param name="gravityX">The acceleration value along the x-axis.</param>
        /// <param name="gravityY">The acceleration value along the y-axis.</param>
        public void SetGravity(float gravityX, float gravityY)
        {
            foreach (GenObject gameObject in Members)
            {
                gameObject.Acceleration.X = gravityX;
                gameObject.Acceleration.Y = gravityY;
            }
        }

        /// <summary>
        /// Sets the x and y deceleration of each object in the verlet group.
        /// </summary>
        /// <param name="decelerationX">The deceleration value along the x-axis.</param>
        /// <param name="decelerationY">The deceleration value along the y-axis.</param>
        public void SetDeceleration(float decelerationX, float decelerationY)
        {
            foreach (GenObject gameObject in Members)
            {
                gameObject.Deceleration.X = decelerationX;
                gameObject.Deceleration.Y = decelerationY;
            }
        }
    }

    /// <summary>
    /// A link constraint between two points in a verlet chain.
    /// Uses verlet integration physics to attempt to keep the two points at a specified distance from ecah other.
    /// </summary>
    public class GenLink
    {
        /// <summary>
        /// The value that the distance between the two points will attempt to match.
        /// </summary>
        protected float _restingDistance;

        /// <summary>
        /// The stiffness of the link constraint between the two points.
        /// </summary>
        public float Stiffness;

        /// <summary>
        /// The distance between the two points needed to break the link.
        /// </summary>
        public float TearDistance;

        /// <summary>
        /// The first point in the link.
        /// </summary>
        public GenObject PointA;

        /// <summary>
        /// The second point in the link.
        /// </summary>
        public GenObject PointB;

        /// <summary>
        /// The x and y position of the point to link on the first object relative to its origin position.
        /// </summary>
        public Vector2 OffsetA;

        /// <summary>
        /// The x and y position of the point to link on the second object relative to its origin position.
        /// </summary>
        public Vector2 OffsetB;

        /// <summary>
        /// Gets or sets the value that the distance between the two points will attempt to match.
        /// </summary>
        public float RestingDistance
        {
            get { return _restingDistance; }

            set { _restingDistance = Math.Max(0f, value); }
        }

        /// <summary>
        /// A constraint used to link two objects together, and push or pull them until they are a specified distance apart.
        /// Each link point is offset to the center point of its connected object by default.
        /// </summary>
        /// <param name="pointA">The first object to use as a connection point.</param>
        /// <param name="pointB">The second object to use as a connection point.</param>
        public GenLink(GenObject pointA, GenObject pointB)
        {
            Stiffness = 1f;
            TearDistance = 0f;
            PointA = pointA;
            PointB = pointB;

            _restingDistance = Vector2.Distance(PointA.OriginPosition, PointB.OriginPosition);
        }

        /// <summary>
        /// Applies verlet integration to attempt to keep the two points at a specified distance from each other.
        /// Updates each point's velocity based on the distance each point has been moved.
        /// </summary>
        public void Update()
        {
            // Calculate the distances between the points.
            float differenceX = (PointA.OriginPosition.X + OffsetA.X) - (PointB.OriginPosition.X + OffsetB.X);
            float differenceY = (PointA.OriginPosition.Y + OffsetA.Y) - (PointB.OriginPosition.Y + OffsetB.Y);
            float distance = (float)Math.Sqrt(differenceX * differenceX + differenceY * differenceY);

            //if ((TearDistance != 0) && (distance > TearDistance))
            // Remove link.

            // Calculate the difference scalar.
            float difference = (distance == 0) ? 0 : (_restingDistance - distance) / distance;

            // Inverse the mass values.
            float inverseMassA = (PointA.Mass == 0) ? 0 : 1 / PointA.Mass;
            float inverseMassB = (PointB.Mass == 0) ? 0 : 1 / PointB.Mass;
            float scalarPointA = (inverseMassA / (inverseMassA + inverseMassB)) * Stiffness;
            float scalarPointB = Stiffness - scalarPointA;
            float scalarDifference;

            // Push or pull the point objects based on their distance and mass.
            // Set the velocity of each point object to account for the distance they have been moved.
            if (!PointA.Immovable && PointA.Exists && PointA.Active)
            {
                scalarDifference = scalarPointA * difference;

                PointA.X += differenceX * scalarDifference;
                PointA.Y += differenceY * scalarDifference;
                PointA.Velocity = (PointA.Position - PointA.OldPosition) * GenG.InverseTimeStep;
            }

            if (!PointB.Immovable && PointB.Exists && PointB.Active)
            {
                scalarDifference = scalarPointB * difference;

                PointB.X -= differenceX * scalarDifference;
                PointB.Y -= differenceY * scalarDifference;
                PointB.Velocity = (PointB.Position - PointB.OldPosition) * GenG.InverseTimeStep;
            }

            // TODO: Fix verlet point collisions breaking through objects.
        }
    }
}
