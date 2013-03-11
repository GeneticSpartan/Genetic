using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        public static float deltaTime = 1f / 60f;

        /// <summary>
        /// The amount to scale the physics time factor by. A default value of 1.0 means there is no change.
        /// </summary>
        public static float timeScale;

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
        public static GenQuadTree quadTree;

        /// <summary>
        /// Determines whether debug mode is on or off.
        /// </summary>
        public static bool isDebug = false;

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
            quadTree = new GenQuadTree(0, 0, Game.Width, Game.Height);
        }

        /// <summary>
        /// Called by the game object every frame to update the game components.
        /// </summary>
        public static void Update()
        {
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

                quadTree.Clear();

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
            foreach (GenCamera cam in cameras)
            {
                currentCamera = cam;

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
                    quadTree.Draw();

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

        public static void Collide(GenBasic objectOrGroup1, GenBasic objectOrGroup2)
        {
            if (objectOrGroup1 is GenObject)
            {
                if (objectOrGroup2 is GenObject)
                {
                    Vector2 intersectionDepth = GenU.GetIntersectDepth((objectOrGroup1 as GenObject).PositionRect, (objectOrGroup2 as GenObject).PositionRect);

                    if (intersectionDepth != Vector2.Zero)
                    {
                        if (Math.Abs(intersectionDepth.Y) < Math.Abs(intersectionDepth.X))
                        {
                            (objectOrGroup1 as GenObject).Y = (objectOrGroup1 as GenObject).Y + intersectionDepth.Y;
                        }
                        else
                        {
                            (objectOrGroup1 as GenObject).X = (objectOrGroup1 as GenObject).X + intersectionDepth.X;
                        }
                    }
                }
            }
        }
    }
}