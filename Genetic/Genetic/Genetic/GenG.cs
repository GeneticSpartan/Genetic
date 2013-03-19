﻿using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Genetic.Geometry;

namespace Genetic
{
    public static class GenG
    {
        /// <summary>
        /// A 1 x 1 pixel texture used for drawing lines.
        /// </summary>
        public static Texture2D pixel;

        /// <summary>
        /// Gets a reference to the game object.
        /// </summary>
        public static GenGame Game
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current graphics device.
        /// </summary>
        public static GraphicsDevice GraphicsDevice
        {
            get;
            private set;
        }

        /// <summary>
        /// The content manager used to load game assets.
        /// </summary>
        public static ContentManager Content
        {
            get;
            private set;
        }

        /// <summary>
        /// The sprite batch used to draw visual game assets.
        /// </summary>
        public static SpriteBatch SpriteBatch
        {
            get;
            private set;
        }

        /// <summary>
        /// The default viewport of the graphics device.
        /// </summary>
        private static Viewport _defaultViewport;

        /// <summary>
        /// The current state object of the game.
        /// </summary>
        private static GenState _state = null;

        /// <summary>
        /// The next state that will be loaded.
        /// </summary>
        private static GenState _requestedState;

        /// <summary>
        /// The background color of the game window.
        /// </summary>
        public static Color bgColor;

        /// <summary>
        /// The amount of seconds that have elapsed between each update.
        /// </summary>
        private static float _timeStep = 1f / 60f;

        /// <summary>
        /// The amount to scale the physics time factor by. A default value of 1.0 means there is no change.
        /// </summary>
        public static float timeScale;

        /// <summary>
        /// The amount of seconds that have elapsed between each update relative to the time scale.
        /// Used for calculating physics relative to the time scale.
        /// </summary>
        private static float _physicsTimeStep;

        /// <summary>
        /// Represents the bounding rectangle of the world space.
        /// Used to keep camera views within these bounds when following a target.
        /// </summary>
        public static Rectangle worldBounds;

        /// <summary>
        /// A list of all current cameras.
        /// </summary>
        public static List<GenCamera> cameras;

        /// <summary>
        /// The initial camera created at the start of the game.
        /// </summary>
        public static GenCamera camera;

        /// <summary>
        /// The camera that is currently being drawn to.
        /// </summary>
        public static GenCamera currentCamera;

        /// <summary>
        /// The keyboard input used for checking key presses and releases.
        /// </summary>
        private static GenKeyboard _keyboards;

        /// <summary>
        /// The game pad input used for checking button presses and releases.
        /// </summary>
        private static GenGamePad _gamePads;

        /// <summary>
        /// A quadtree data structure used to partition the screen space for faster object-to-object checking.
        /// </summary>
        public static GenQuadtree quadtree;

        /// <summary>
        /// Determines whether debug mode is on or off.
        /// </summary>
        public static bool isDebug = false;

        /// <summary>
        /// Gets the amount of seconds that have elapsed between each update relative to the time scale.
        /// Used for calculating physics relative to the time scale.
        /// </summary>
        public static float PhysicsTimeStep
        {
            get { return _physicsTimeStep; }
        }

        /// <summary>
        /// Gets the keyboard input used for checking key presses and releases.
        /// </summary>
        public static GenKeyboard Keyboards
        {
            get { return _keyboards; }
        }

        /// <summary>
        /// Gets the game pad input used for checking button presses and releases.
        /// </summary>
        public static GenGamePad GamePads
        {
            get { return _gamePads; }
        }

        public static void Initialize(GenGame game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spriteBatch)
        {
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });

            Game = game;
            GraphicsDevice = graphicsDevice;
            Content = content;
            SpriteBatch = spriteBatch;
            _defaultViewport = GraphicsDevice.Viewport;
            bgColor = Color.CornflowerBlue;
            timeScale = 1.0f;
            worldBounds = new Rectangle(0, 0, Game.Width, Game.Height);
            cameras = new List<GenCamera>();
            _keyboards = new GenKeyboard();
            _gamePads = new GenGamePad();
            quadtree = new GenQuadtree(0, 0, Game.Width, Game.Height);
        }

        /// <summary>
        /// Called by the game object every frame to update the game components.
        /// </summary>
        public static void Update()
        {
            _physicsTimeStep = timeScale * _timeStep;

            // Update the keyboards.
            _keyboards.Update();

            // Update the game pads.
            _gamePads.Update();

            if (_keyboards.JustPressed(Keys.OemTilde))
                isDebug = !isDebug;

            if (_requestedState != null)
            {
                // Reset the cameras.
                cameras.Clear();
                camera.Reset();
                AddCamera(camera);

                // Reset the members of the previous state.
                if (_state != null && _state.members.Count > 0)
                {
                    foreach (GenBasic member in _state.members)
                        member.Reset();
                }

                quadtree.Clear();

                // Create a new state from the requested state.
                _state = _requestedState;
                _state.Create();
                _requestedState = null;
            }

            _state.Update();
        }

        /// <summary>
        /// Called by the game object every frame to draw the game components.
        /// </summary>
        public static void Draw()
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                currentCamera = cameras[i];

                GraphicsDevice.Viewport = currentCamera.Viewport;

                // Draw the camera background color.
                if (currentCamera.BgColor != null)
                {
                    SpriteBatch.Begin();
                    currentCamera.DrawBg();
                    SpriteBatch.End();
                }

                SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, currentCamera.Transform);
                _state.Draw();

                if (isDebug)
                    quadtree.Draw();

                SpriteBatch.End();

                // Draw the camera effects.
                SpriteBatch.Begin();
                currentCamera.DrawFx();
                SpriteBatch.End();
            }
        }

        /// <summary>
        /// Adds a new camera to the cameras list.
        /// </summary>
        /// <param name="newCamera">The new camera to add.</param>
        /// <returns>The newly added camera.</returns>
        public static GenCamera AddCamera(GenCamera newCamera)
        {
            cameras.Add(newCamera);

            return newCamera;
        }

        /// <summary>
        /// Resets the current state.
        /// </summary>
        public static void ResetState()
        {
            GenState newState = (GenState)Activator.CreateInstance(_state.GetType());

            SwitchState(newState);
        }

        /// <summary>
        /// Sets the requested state that will be switched to.
        /// </summary>
        public static void SwitchState(GenState newState)
        {
            _requestedState = newState;
        }

        /// <summary>
        /// Draws a line between the two given points.
        /// </summary>
        /// <param name="x1">The x position of the starting point.</param>
        /// <param name="y1">The y position of the starting point.</param>
        /// <param name="x2">The x position of the ending point.</param>
        /// <param name="y2">The y position of the ending point.</param>
        /// <param name="color">The color of the line. Defaults to white if set to null.</param>
        public static void DrawLine(float x1, float y1, float x2, float y2, Color? color = null)
        {
            color = color.HasValue ? color.Value : Color.White;

            Vector2 point1 = new Vector2(x1, y1);
            Vector2 point2 = new Vector2(x2, y2);

            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = (point2 - point1).Length();

            SpriteBatch.Draw(pixel, point1, null, color.Value, angle, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }

        /// <summary>
        /// Applys collision detection and response between two objects, groups of objects, or tilemap that may overlap.
        /// </summary>
        /// <param name="objectOrGroup1">The first object, group, or tilemap to check for collisions.</param>
        /// <param name="objectOrGroup2">The second object, group, or tilemap to check for collisions.</param>
        /// <param name="penetrate">Determines if the objects are able to penetrate each other for soft collision response.</param>
        public static void Collide(GenBasic objectOrGroup1, GenBasic objectOrGroup2, bool penetrate = true)
        {
            quadtree.Clear();

            if (objectOrGroup1 is GenTilemap)
            {
                ((GenTilemap)objectOrGroup1).Collide(objectOrGroup2);
                return;
            }
            else if (objectOrGroup2 is GenTilemap)
            {
                ((GenTilemap)objectOrGroup2).Collide(objectOrGroup1);
                return;
            }

            // Insert the objects into the quadtree for faster collision checks.
            quadtree.Insert(objectOrGroup1);

            // If the second object or group is the same as the first, do not insert it into the quadtree twice.
            if (!objectOrGroup1.Equals(objectOrGroup2))
                quadtree.Insert(objectOrGroup2);

            List<GenBasic> objects = new List<GenBasic>();

            if (objectOrGroup1 is GenObject)
            {
                if (objectOrGroup2 is GenObject)
                {
                    CollideObjects((GenObject)objectOrGroup1, (GenObject)objectOrGroup2, penetrate);
                }
                else if (objectOrGroup2 is GenGroup)
                {
                    // Retrieve the objects from the quadtree that the first object may collide with.
                    GenG.quadtree.Retrieve(objects, ((GenObject)objectOrGroup1).BoundingBox);

                    for (int i = 0; i < objects.Count; i++)
                        CollideObjects((GenObject)objectOrGroup1, (GenObject)objects[i], penetrate);
                }
            }
            else if (objectOrGroup1 is GenGroup)
            {
                for (int i = 0; i < ((GenGroup)objectOrGroup1).members.Count; i++)
                {
                    if (objectOrGroup2 is GenObject)
                    {
                        CollideObjects((GenObject)((GenGroup)objectOrGroup1).members[i], (GenObject)objectOrGroup2, penetrate);
                    }
                    else if (objectOrGroup2 is GenGroup)
                    {
                        objects.Clear();

                        // Retrieve the objects from the quadtree that the current object may collide with.
                        GenG.quadtree.Retrieve(objects, ((GenObject)((GenGroup)objectOrGroup1).members[i]).BoundingBox);

                        for (int j = 0; j < objects.Count; j++)
                            CollideObjects((GenObject)((GenGroup)objectOrGroup1).members[i], (GenObject)objects[j], penetrate);
                        //for (int j = 0; j < ((GenGroup)objectOrGroup2).members.Count; j++)
                        //    CollideObjects((GenObject)((GenGroup)objectOrGroup1).members[i], (GenObject)((GenGroup)objectOrGroup1).members[j], penetrate);
                    }
                }
            }
        }

        /// <summary>
        /// Applys collision detection and response between two objects that may overlap.
        /// </summary>
        /// <param name="object1">The first object to check for a collision.</param>
        /// <param name="object2">The second object to check for a collision.</param>
        /// <param name="penetrate">Determines if the objects are able to penetrate each other for soft collision response.</param>
        /// <returns>The contact normal represented as a unit vector, or a zero vector if no collision occurs.</returns>
        public static bool CollideObjects(GenObject object1, GenObject object2, bool penetrate = true)
        {
            if (!object1.Equals(object2))
            {
                if (object1.immovable && object2.immovable)
                    return false;

                GenAABB moveBounds1 = GenU.GetMoveBounds(object1);
                GenAABB moveBounds2 = GenU.GetMoveBounds(object2);

                if (moveBounds1.Intersects(moveBounds2))
                {
                    Vector2 distances = GenU.GetDistanceAABB(object1.BoundingBox, object2.BoundingBox);
                    Vector2 normal;

                    if (distances.X > distances.Y)
                        normal = (object1.BoundingBox.MidpointX > object2.BoundingBox.MidpointX) ? new Vector2(-1, 0) : new Vector2(1, 0);
                    else
                        normal = (object1.BoundingBox.MidpointY > object2.BoundingBox.MidpointY) ? new Vector2(0, -1) : new Vector2(0, 1);

                    float distance = Math.Max(distances.X, distances.Y);
                    float separation = 0f;

                    if (!penetrate)
                        separation = Math.Max(distance, 0);

                    float relativeNormalVelocity = Vector2.Dot(object2.velocity - object1.velocity, normal);
                    float remove;

                    if (penetrate)
                        remove = relativeNormalVelocity + distance / (GenG._timeStep / GenG.timeScale);
                    else
                        remove = relativeNormalVelocity + separation / (GenG._timeStep / GenG.timeScale);

                    if (remove < 0)
                    {
                        float impulse = remove / (object1.mass + object2.mass);

                        if (!object1.immovable)
                        {
                            object1.velocity += impulse * normal * object2.mass;

                            if (!penetrate)
                            {
                                float penetration = Math.Min(distance, 0);

                                object1.X += penetration * normal.X;
                                object1.Y += penetration * normal.Y;
                            }
                        }

                        if (!object2.immovable)
                        {
                            object2.velocity -= impulse * normal * object1.mass;

                            if (!penetrate)
                            {
                                float penetration = Math.Min(distance, 0);

                                object2.X -= penetration * normal.X;
                                object2.Y -= penetration * normal.Y;
                            }
                        }

                        return true;
                    }
                }
            }

            return false;

            /* PHYSICS SYSTEM 2
            if (!object1.Equals(object2))
            {
                if (object1.immovable && object2.immovable)
                    return;

                Vector2 intersectionDepth = GenU.GetIntersectDepth(object1.PositionRect, object2.PositionRect);

                if (intersectionDepth != Vector2.Zero)
                {
                    if (Math.Abs(intersectionDepth.X) < Math.Abs(intersectionDepth.Y))
                    {
                        if (!object1.immovable && !object2.immovable)
                        {
                            object1.X += intersectionDepth.X * 0.5f;
                            object2.X -= intersectionDepth.X * 0.5f;

                            float object1VelocityX = (float)Math.Sqrt((object2.velocity.X * object2.velocity.X * object2.mass) / object1.mass) * ((object2.velocity.X > 0) ? 1 : -1);
                            float object2VelocityX = (float)Math.Sqrt((object1.velocity.X * object1.velocity.X * object1.mass) / object2.mass) * ((object1.velocity.X > 0) ? 1 : -1);
                            float averageVelocityX = (object1VelocityX + object2VelocityX) * 0.5f;
                            object1VelocityX -= averageVelocityX;
                            object2VelocityX -= averageVelocityX;

                            object1.velocity.X = averageVelocityX + object1VelocityX; // Multiply object1VelocityX by elasticity of the object later.
                            object2.velocity.X = averageVelocityX + object2VelocityX; // Multiply object1VelocityX by elasticity of the object later.
                        }
                        else if (!object1.immovable)
                        {
                            object1.X += intersectionDepth.X;
                            object1.velocity.X = object2.velocity.X; // Multiply by elasticity of the object later.
                        }
                        else
                        {
                            object2.X -= intersectionDepth.X;
                            object2.velocity.X = object1.velocity.X; // Multiply by elasticity of the object later.
                        }
                    }
                    else
                    {
                        if (!object1.immovable && !object2.immovable)
                        {
                            object1.Y += intersectionDepth.Y * 0.5f;
                            object2.Y -= intersectionDepth.Y * 0.5f;

                            float object1VelocityY = (float)Math.Sqrt((object2.velocity.Y * object2.velocity.Y * object2.mass) / object1.mass) * ((object2.velocity.Y > 0) ? 1 : -1);
                            float object2VelocityY = (float)Math.Sqrt((object1.velocity.Y * object1.velocity.Y * object1.mass) / object2.mass) * ((object1.velocity.Y > 0) ? 1 : -1);
                            float averageVelocityY = (object1VelocityY + object2VelocityY) * 0.5f;
                            object1VelocityY -= averageVelocityY;
                            object2VelocityY -= averageVelocityY;

                            object1.velocity.Y = averageVelocityY + object1VelocityY; // Multiply object1VelocityX by elasticity of the object later.
                            object2.velocity.Y = averageVelocityY + object2VelocityY; // Multiply object1VelocityX by elasticity of the object later.
                        }
                        else if (!object1.immovable)
                        {
                            object1.Y += intersectionDepth.Y;
                            object1.velocity.Y = object2.velocity.Y; // Multiply by elasticity of the object later.
                        }
                        else
                        {
                            object2.Y -= intersectionDepth.Y;
                            object2.velocity.Y = object1.velocity.Y; // Multiply by elasticity of the object later.
                        }
                    }
                }
            }

            /* PHYSICS SYSTEM 1
            if (!object1.Equals(object2))
            {
                Vector2 intersectionDepth = GenU.GetIntersectDepth(object1.PositionRect, object2.PositionRect);

                if (intersectionDepth != Vector2.Zero)
                {
                    float massDifference1 = object1.mass - object2.mass;
                    float massDifference2 = object2.mass - object1.mass;
                    float massSum = object1.mass + object2.mass;

                    if (Math.Abs(intersectionDepth.X) < Math.Abs(intersectionDepth.Y))
                    {
                        float velocityX1 = object1.velocity.X;
                        float velocityX2 = object2.velocity.X;

                        if (!object1.immovable)
                        {
                            object1.X = object1.X + intersectionDepth.X;
                            object1.velocity.X = (velocityX1 * massDifference1 + (2 * object2.mass * velocityX2)) / massSum;
                        }

                        if (!object2.immovable)
                            object2.velocity.X = (velocityX2 * massDifference2 + (2 * object1.mass * velocityX1)) / massSum;
                    }
                    else
                    {
                        float velocityY1 = object1.velocity.Y;
                        float velocityY2 = object2.velocity.Y;

                        if (!object1.immovable)
                        {
                            object1.Y = object1.Y + intersectionDepth.Y;
                            object1.velocity.Y = (velocityY1 * massDifference1 + (2 * object2.mass * velocityY2)) / massSum;
                        }

                        if (!object2.immovable)
                            object2.velocity.Y = (velocityY2 * massDifference2 + (2 * object1.mass * velocityY1)) / massSum;
                    }
                }
            }*/
        }
    }
}