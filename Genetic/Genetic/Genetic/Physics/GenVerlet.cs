using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Genetic.Physics
{
    public class GenVerlet : GenGroup
    {
        /// <summary>
        /// A list of link constraints.
        /// </summary>
        public List<GenLink> Links;

        /// <summary>
        /// A group used for managing objects that can be connected together using link constraints.
        /// Each link constraint pushes or pulls the two connected objects to a specified distance from each other.
        /// Useful for simulating ropes, cloth, or other elastic bodies.
        /// </summary>
        public GenVerlet()
        {
            Links = new List<GenLink>();
        }

        /// <summary>
        /// Determines if lines are drawn between object points for each link constraint.
        /// </summary>
        public bool DrawLines = true;

        /// <summary>
        /// The color of the lines drawn between object points for each link constraint.
        /// </summary>
        public Color LineColor = Color.White;

        /// <summary>
        /// The thickness of the lines drawn between object points for each link constraint, in pixels.
        /// </summary>
        public float LineThickness = 1f;

        public override void Update()
        {
            base.Update();

            foreach (GenLink link in Links)
                link.Update();
        }

        public override void Draw()
        {
            base.Draw();

            if (DrawLines)
            {
                foreach (GenLink link in Links)
                    GenG.DrawLine(link.PointA.X + link.OffsetA.X, link.PointA.Y + link.OffsetA.Y, link.PointB.X + link.OffsetB.X, link.PointB.Y + link.OffsetB.Y, LineColor, LineThickness);
            }
        }

        /// <summary>
        /// Creates a grid of points, each connected by a link constraint to their adjacent neighboring points.
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
                    GenObject point = (GenObject)Add(new GenSprite(startX + x * distance, startY + y * distance, null, 0, 0));

                    // Make a link to the left point.
                    if (x != 0)
                        MakeLink(point, (GenObject)Members[x + (columns * y) - 1]);

                    // Make a link to the above point.
                    if (y != 0)
                        MakeLink(point, (GenObject)Members[x + columns * (y - 1)]);
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
            Links.Add(new GenLink(pointA, pointB));
        }

        /// <summary>
        /// Sets the mass of every object in the verlet group.
        /// </summary>
        /// <param name="mass">The mass value to set each object.</param>
        public void SetMass(float mass)
        {
            foreach (GenObject member in Members)
                member.Mass = mass;
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
        /// </summary>
        /// <param name="stiffness">The stiffness value to set each link constraint.</param>
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
            foreach (GenObject member in Members)
            {
                member.Acceleration.X = gravityX;
                member.Acceleration.Y = gravityY;
            }
        }
    }

    public class GenLink
    {
        /// <summary>
        /// The value that the distance between the two points will attempt to match.
        /// </summary>
        public float RestingDistance;

        /// <summary>
        /// The stiffness of the link constraint between the two points.
        /// </summary>
        public float Stiffness = 1f;

        /// <summary>
        /// The distance between the two points needed to break the link.
        /// </summary>
        public float TearDistance = 0f;

        /// <summary>
        /// The first point in the link.
        /// </summary>
        public GenObject PointA;

        /// <summary>
        /// The second point in the link.
        /// </summary>
        public GenObject PointB;

        /// <summary>
        /// The x and y position of the point to link on the first object relative to its top-left corner position.
        /// </summary>
        public Vector2 OffsetA;

        /// <summary>
        /// The x and y position of the point to link on the second object relative to its top-left corner position.
        /// </summary>
        public Vector2 OffsetB;

        /// <summary>
        /// A constraint used to link two objects together, and push or pull them until they are a specified distance apart.
        /// Each link point is offset to the center point of its connected object by default.
        /// </summary>
        /// <param name="pointA">The first object to use as a connection point.</param>
        /// <param name="pointB">The second object to use as a connection point.</param>
        public GenLink(GenObject pointA, GenObject pointB)
        {
            PointA = pointA;
            PointB = pointB;

            // Set the link point offsets to the center of the connected objects.
            OffsetA = new Vector2(PointA.Width / 2, PointA.Height / 2);
            OffsetB = new Vector2(PointB.Width / 2, PointB.Height / 2);

            RestingDistance = Vector2.Distance(PointA.Position, PointB.Position);
        }

        public void Update()
        {
            // Calculate the distances between the points.
            float differenceX = (PointA.X + OffsetA.X) - (PointB.X + OffsetB.X);
            float differenceY = (PointA.Y + OffsetA.Y) - (PointB.Y + OffsetB.Y);
            float distance = (float)Math.Sqrt(differenceX * differenceX + differenceY * differenceY);

            //if ((TearDistance != 0) && (distance > TearDistance))
                // Remove link.

            // Calculate the difference scalar.
            float difference = (RestingDistance - distance) / distance;

            // Inverse the mass values.
            float inverseMassA = 1 / PointA.Mass;
            float inverseMassB = 1 / PointB.Mass;
            float scalarPointA = (inverseMassA / (inverseMassA + inverseMassB)) * Stiffness;
            float scalarPointB = Stiffness - scalarPointA;

            // Push or pull the point objects based on their distance and mass.
            if (!PointA.Immovable && PointA.Exists && PointA.Active)
            {
                PointA.X += differenceX * scalarPointA * difference;
                PointA.Y += differenceY * scalarPointA * difference;
            }

            if (!PointB.Immovable && PointB.Exists && PointB.Active)
            {
                PointB.X -= differenceX * scalarPointB * difference;
                PointB.Y -= differenceY * scalarPointB * difference;
            }

            // Set the velocity of each point object to account for the distance they have been moved.
            PointA.Velocity = (PointA.Position - PointA.OldPosition) / GenG.PhysicsTimeStep;
            PointB.Velocity = (PointB.Position - PointB.OldPosition) / GenG.PhysicsTimeStep;
        }
    }
}
