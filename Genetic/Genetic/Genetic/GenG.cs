using System;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Genetic.Geometry;
using Genetic.Input;
using Genetic.Sound;

namespace Genetic
{
    public static class GenG
    {
        private static string fileSize;

        public enum DrawType
        {
            // Draws using the point clamp sampler state to achieve sharp texture edges, ideal for pixel art.
            Pixel,

            // Draws using the linear clamp sampler state to achieve smooth texture edges.
            Smooth
        }

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
        /// A global seed value used for random number generation.
        /// Useful for achieving deterministic random values.
        /// </summary>
        public static int GlobalSeed;

        /// <summary>
        /// The draw type used for rendering either sharp or smooth textures.
        /// </summary>
        public static DrawType DrawMode;

        /// <summary>
        /// A 1 x 1 pixel texture used for drawing lines.
        /// </summary>
        public static Texture2D Pixel;

        /// <summary>
        /// The global default spritefont used for drawing strings.
        /// </summary>
        public static SpriteFont Font;

        /// <summary>
        /// An effect shader used to apply post-processing effects to the render target texture.
        /// </summary>
        private static Effect _effect;

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
        public static Color BackgroundColor;

        /// <summary>
        /// The amount of seconds needed between each update.
        /// </summary>
        private static float _timeStep = 1f / 60f;

        /// <summary>
        /// The amount to scale the physics time factor by. A default value of 1.0 means there is no change.
        /// </summary>
        public static float TimeScale;

        /// <summary>
        /// The amount of seconds that have elapsed between each update relative to the time scale.
        /// Used for calculating physics relative to the time scale.
        /// </summary>
        private static float _scaleTimeStep;

        /// <summary>
        /// The total amount of seconds that have elapsed relative to the time scale.
        /// </summary>
        private static float _elapsedTime;

        /// <summary>
        /// The amount of time, in seconds, since the last update call.
        /// Used to call Update the correct amount of times relative to the time step and time scale.
        /// </summary>
        private static float _updateTimer;

        /// <summary>
        /// A flag used to determine if the current frame rate whould be drawn to the screen.
        /// </summary>
        public static bool ShowFps;

        // A counter used to keep track of the number of frames drawn per second.
        private static int fpsCounter;

        // A string used to display the current number of frames drawn per second.
        private static string currentFps;

        /// <summary>
        /// A stopwatch used to calculate the game's frame rate.
        /// </summary>
        private static Stopwatch _fpsStopwatch;

        /// <summary>
        /// A flag used to pause the game.
        /// </summary>
        public static bool Paused;

        /// <summary>
        /// Represents the bounding rectangle of the world space.
        /// Used to keep camera views within these bounds when following a target.
        /// </summary>
        public static Rectangle WorldBounds;

        /// <summary>
        /// A list of all current cameras.
        /// </summary>
        public static List<GenCamera> Cameras;

        /// <summary>
        /// The initial camera created at the start of the game.
        /// </summary>
        public static GenCamera Camera;

        /// <summary>
        /// The camera that is currently being drawn to.
        /// </summary>
        public static GenCamera CurrentCamera;

        /// <summary>
        /// The screen effect manager.
        /// </summary>
        private static GenScreenEffect _screenEffect;

        /// <summary>
        /// A dictionary containing multiple keyboard inputs used for checking key presses and releases.
        /// </summary>
        private static Dictionary<PlayerIndex, GenKeyboard> _keyboards;

        /// <summary>
        /// A dictionary containing multiple game pad inputs used for checking button presses and releases.
        /// </summary>
        private static Dictionary<PlayerIndex, GenGamePad> _gamePads;

        /// <summary>
        /// The mouse input used for checking button presses and releases.
        /// </summary>
        private static GenMouse _mouse;

        #region Volume Fields
        /// <summary>
        /// The global volume for all sounds, a value from 0.0 to 1.0.
        /// </summary>
        private static float _volume = 1f;

        /// <summary>
        /// A group that controls the volume control display components.
        /// </summary>
        private static GenGroup _volumeDisplay;

        /// <summary>
        /// The background box of the volume control display.
        /// </summary>
        private static GenSprite _volumeBox;

        /// <summary>
        /// The sound level bars of the volume control display.
        /// </summary>
        private static GenGroup _volumeBars;

        /// <summary>
        /// The volume label text of the volume control display.
        /// </summary>
        private static GenText _volumeText;

        /// <summary>
        /// The beep sound of the volume control display.
        /// Used to hear the current volume level.
        /// </summary>
        private static GenSound _volumeBeep;

        /// <summary>
        /// A timer used to hide the volume control display after a period of time.
        /// </summary>
        private static GenTimer _volumeDisplayTimer;
        #endregion

        /// <summary>
        /// Determines whether debug mode is on or off.
        /// </summary>
        public static bool IsDebug = false;

        /// <summary>
        /// Gets the amount of seconds that have elapsed between each update.
        /// </summary>
        public static float TimeStep
        {
            get { return _timeStep; }
        }

        /// <summary>
        /// Gets the amount of seconds that have elapsed between each update relative to the time scale.
        /// </summary>
        public static float ScaleTimeStep
        {
            get { return _scaleTimeStep; }
        }

        /// <summary>
        /// Gets the total amount of seconds that have elapsed relative to the time scale.
        /// </summary>
        public static float ElapsedTime
        {
            get { return _elapsedTime; }
        }

        /// <summary>
        /// Gets a dictionary containing the multiple keyboard inputs used for checking key presses and releases.
        /// </summary>
        public static Dictionary<PlayerIndex, GenKeyboard> Keyboards
        {
            get { return _keyboards; }
        }

        /// <summary>
        /// Gets a dictionary containing the multiple game pad inputs used for checking button presses and releases.
        /// </summary>
        public static Dictionary<PlayerIndex, GenGamePad> GamePads
        {
            get { return _gamePads; }
        }

        /// <summary>
        /// Gets the mouse input used for checking button presses and releases.
        /// </summary>
        public static GenMouse Mouse
        {
            get { return _mouse; }
        }

        /// <summary>
        /// Gets the title safe area of the current viewport.
        /// </summary>
        public static Rectangle TitleSafeArea
        {
            get { return GraphicsDevice.Viewport.TitleSafeArea; }
        }

        /// <summary>
        /// Gets or sets the global volume for all sounds, a value from 0.0 to 1.0.
        /// </summary>
        public static float Volume
        {
            get { return _volume; }

            set { _volume = MathHelper.Clamp(value, 0.0f, 1.0f); }
        }

        public static void Initialize(GenGame game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spriteBatch)
        {
            GlobalSeed = Environment.TickCount;

            Pixel = new Texture2D(graphicsDevice, 1, 1);
            Pixel.SetData<Color>(new Color[] { Color.White });
            Font = content.Load<SpriteFont>("Nokia");

            Game = game;
            GraphicsDevice = graphicsDevice;
            Content = content;
            SpriteBatch = spriteBatch;
            _effect = Content.Load<Effect>("grayscale");
            _defaultViewport = GraphicsDevice.Viewport;
            BackgroundColor = Color.CornflowerBlue;
            TimeScale = 1.0f;
            WorldBounds = new Rectangle(0, 0, Game.Width, Game.Height);
            Cameras = new List<GenCamera>();
            _screenEffect = new GenScreenEffect(GraphicsDevice.Viewport.Bounds);
            _updateTimer = 0f;
            ShowFps = false;
            fpsCounter = 0;
            currentFps = "";
            _fpsStopwatch = new Stopwatch();
            _fpsStopwatch.Start();
            Paused = false;

            _keyboards = new Dictionary<PlayerIndex, GenKeyboard>();
            _keyboards.Add(PlayerIndex.One, new GenKeyboard(PlayerIndex.One));
            _keyboards.Add(PlayerIndex.Two, new GenKeyboard(PlayerIndex.Two));
            _keyboards.Add(PlayerIndex.Three, new GenKeyboard(PlayerIndex.Three));
            _keyboards.Add(PlayerIndex.Four, new GenKeyboard(PlayerIndex.Four));

            _gamePads = new Dictionary<PlayerIndex, GenGamePad>();
            _gamePads.Add(PlayerIndex.One, new GenGamePad(PlayerIndex.One));
            _gamePads.Add(PlayerIndex.Two, new GenGamePad(PlayerIndex.Two));
            _gamePads.Add(PlayerIndex.Three, new GenGamePad(PlayerIndex.Three));
            _gamePads.Add(PlayerIndex.Four, new GenGamePad(PlayerIndex.Four));

            _mouse = new GenMouse();

            // Create the volume control display.
            _volumeDisplay = new GenGroup();

            _volumeBox = new GenSprite((Game.Width / 2) - 46, 0);
            _volumeBox.MakeTexture(Color.Black * 0.5f, 92, 56);
            _volumeDisplay.Add(_volumeBox);

            _volumeBars = new GenGroup();
            _volumeDisplay.Add(_volumeBars);

            for (int i = 0; i < 10; i++)
            {
                _volumeBars.Add(new GenSprite(_volumeBox.X + (i * 8 + 8), 20 - i * 2 + 8, null, 4, i * 2 + 2));
                ((GenSprite)_volumeBars.Members[i]).MakeTexture(Color.White);
            }

            _volumeText = new GenText("VOLUME", _volumeBox.X + 8, 32, 0, 0);
            _volumeDisplay.Add(_volumeText);

            _volumeBeep = new GenSound("beep");
            _volumeDisplay.Add(_volumeBeep);

            _volumeDisplayTimer = new GenTimer(1f, HideVolumeDisplay, false);
            _volumeDisplay.Add(_volumeDisplayTimer);

            _volumeDisplay.PreUpdate();

            // Hide the volume control display initially.
            HideVolumeDisplay();
        }

        /// <summary>
        /// Called by the game object every frame to update the game components.
        /// </summary>
        public static void Update(GameTime gameTime)
        {
            _scaleTimeStep = TimeScale * _timeStep;
            _elapsedTime += _scaleTimeStep;

            if (_requestedState != null)
            {
                // Reset the cameras.
                Cameras.Clear();
                Camera.Reset();
                AddCamera(Camera);

                // Reset the members of the previous state.
                if (_state != null && _state.Members.Count > 0)
                {
                    foreach (GenBasic member in _state.Members)
                        member.Reset();
                }

                // Create a new state from the requested state.
                _state = _requestedState;
                _state.Create();
                _requestedState = null;
            }

            while (_updateTimer >= _timeStep)
            {
                // Update the keyboards.
                _keyboards[PlayerIndex.One].Update();
                _keyboards[PlayerIndex.Two].Update();
                _keyboards[PlayerIndex.Three].Update();
                _keyboards[PlayerIndex.Four].Update();

                // Update the game pads.
                _gamePads[PlayerIndex.One].Update();
                _gamePads[PlayerIndex.Two].Update();
                _gamePads[PlayerIndex.Three].Update();
                _gamePads[PlayerIndex.Four].Update();

                // Update the mouse.
                _mouse.Update();

                // Volume down control.
                if (_keyboards[PlayerIndex.One].JustPressed(Keys.OemMinus))
                {
                    Volume -= 0.1f;
                    ShowVolumeDisplay();
                }

                // Volume up control.
                if (_keyboards[PlayerIndex.One].JustPressed(Keys.OemPlus))
                {
                    Volume += 0.1f;
                    ShowVolumeDisplay();
                }

                if (_keyboards[PlayerIndex.One].JustPressed(Keys.OemTilde))
                    IsDebug = !IsDebug;

                _state.PreUpdate();
                _state.Update();
                _state.PostUpdate();

                _volumeDisplay.Update();

                _updateTimer -= _timeStep;
            }

            _updateTimer += _scaleTimeStep;
        }

        /// <summary>
        /// Called by the game object every frame to draw the game components.
        /// </summary>
        public static void Draw()
        {
            foreach (GenCamera camera in Cameras)
            {
                if (camera.Exists && camera.Visible)
                {
                    CurrentCamera = camera;

                    GraphicsDevice.SetRenderTarget(CurrentCamera.RenderTarget);

                    // Clear the back buffer to a transparent color so that each camera render target will have a transparent background.
                    GraphicsDevice.Clear(Color.Transparent);

                    // Draw the camera background color.
                    if (CurrentCamera.BgColor != null)
                    {
                        SpriteBatch.Begin();
                        CurrentCamera.DrawBg();
                        SpriteBatch.End();
                    }

                    if (DrawMode == DrawType.Pixel)
                        SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, CurrentCamera.Transform);
                    else if (DrawMode == DrawType.Smooth)
                        SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, CurrentCamera.Transform);

                    _state.Draw();

                    SpriteBatch.End();

                    // Draw the camera effects.
                    SpriteBatch.Begin();
                    camera.DrawFx();
                    SpriteBatch.End();
                }
            }

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(BackgroundColor);

            // Draw the camera render targets with any attatched effects.
            foreach (GenCamera camera in Cameras)
            {
                if (camera.Exists && camera.Visible)
                {
                    _effect.Parameters["timer"].SetValue(_elapsedTime);
                    _effect.CurrentTechnique = _effect.Techniques["Grayscale"];

                    // Draw the render target texture.
                    if (DrawMode == DrawType.Pixel)
                        SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.Identity);
                    else if (DrawMode == DrawType.Smooth)
                        SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                    _effect.CurrentTechnique.Passes[0].Apply();

                    // Draw the render target texture.
                    if (DrawMode == DrawType.Pixel)
                        GenG.SpriteBatch.Draw(camera.RenderTarget, camera.DrawPosition, null, camera.Color, camera.Rotation, camera.Origin, camera.Zoom * camera.Scale, SpriteEffects.None, 0);
                    else if (DrawMode == DrawType.Smooth)
                        GenG.SpriteBatch.Draw(camera.RenderTarget, camera.DrawPosition, null, camera.Color, camera.Rotation, camera.Origin, camera.Scale, SpriteEffects.None, 0);

                    SpriteBatch.End();
                }
            }

            // Call Draw on any overlay objects after the cameras have been drawn.
            if (DrawMode == DrawType.Pixel)
                SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.Identity);
            else if (DrawMode == DrawType.Smooth)
                SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            _state.DrawOverlay();
            _screenEffect.Draw();

            SpriteBatch.End();

            if (ShowFps || IsDebug)
            {
                if (_fpsStopwatch.ElapsedMilliseconds >= 1000)
                {
                    _fpsStopwatch.Reset();
                    _fpsStopwatch.Start();
                    fileSize = GC.GetTotalMemory(false).ToString();
                    currentFps = fpsCounter.ToString() + " - GC:" + fileSize + " bytes";
                    fpsCounter = 0;
                }
                else
                    fpsCounter++;
            }

            SpriteBatch.Begin();
            _volumeDisplay.Draw();

            // Draw the frame rate.
            if (ShowFps || IsDebug)
                SpriteBatch.DrawString(Font, currentFps, new Vector2(16, 16), Color.White);

            SpriteBatch.End();
        }

        /// <summary>
        /// Adds an object to the members list of the current state.
        /// Useful for adding objects to the current state within other object classes.
        /// </summary>
        /// <param name="basic">The object to add.</param>
        /// <param name="overlay">A flag used to determine if the object should be drawn to the screen directly, ignoring cameras.</param>
        /// <returns>The object added to the members list of the current state.</returns>
        public static GenBasic Add(GenBasic basic, bool overlay = false)
        {
            return _state.Add(basic, overlay);
        }

        /// <summary>
        /// Removes an object from the members list of the current state.
        /// Useful for removing objects from the current state within other object classes.
        /// </summary>
        /// <param name="basic">The object to remove.</param>
        /// <param name="overlay">A flag used to determine if the object being removed is an overlay object.</param>
        /// <returns>The object removed from the members list of the current state.</returns>
        public static GenBasic Remove(GenBasic basic, bool overlay = false)
        {
            return _state.Remove(basic, overlay);
        }

        /// <summary>
        /// Adds a new camera to the cameras list.
        /// </summary>
        /// <param name="newCamera">The new camera to add.</param>
        /// <returns>The newly added camera.</returns>
        public static GenCamera AddCamera(GenCamera newCamera)
        {
            Cameras.Add(newCamera);

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
        /// <param name="xA">The x position of the starting point.</param>
        /// <param name="yA">The y position of the starting point.</param>
        /// <param name="xB">The x position of the ending point.</param>
        /// <param name="yB">The y position of the ending point.</param>
        /// <param name="color">The color of the line. Defaults to white if set to null.</param>
        /// <param name="thickness">The thickness of the line, in pixels.</param>
        public static void DrawLine(float xA, float yA, float xB, float yB, Color? color = null, float thickness = 1)
        {
            color = color.HasValue ? color.Value : Color.White;

            Vector2 point1 = new Vector2(xA, yA);
            Vector2 point2 = new Vector2(xB, yB);

            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = (point2 - point1).Length();

            SpriteBatch.Draw(Pixel, point1, null, color.Value, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
        }

        /// <summary>
        /// Checks for overlap between two objects, groups of objects, or tilemaps.
        /// </summary>
        /// <param name="objectOrGroupA">The first object, group, or tilemap to check for overlap.</param>
        /// <param name="objectOrGroupB">The second object, group, or tilemap to check for overlap.</param>
        /// <param name="callback">The delegate method that will be invoked if an overlap occurs.</param>
        /// <param name="separate">Determines if objects should collide with each other.</param>
        /// <param name="penetrate">Determines if the objects are able to penetrate each other for soft collision response.</param>
        /// <param name="collidableEdges">A bit field of flags determining which edges of the second object or group of objects are collidable.</param>
        /// <returns>True if an overlap occurs, false if not.</returns>
        public static bool Overlap(GenBasic objectOrGroupA, GenBasic objectOrGroupB, CollideEvent callback = null, bool separate = false, bool penetrate = true, GenObject.Direction collidableEdges = GenObject.Direction.Any)
        {
            bool overlap = false;

            if (objectOrGroupA is GenObject)
            {
                if (objectOrGroupB is GenObject)
                {
                    if (separate)
                        overlap = ((GenObject)objectOrGroupA).Collide((GenObject)objectOrGroupB, callback, penetrate, collidableEdges);
                    else
                        overlap = ((GenObject)objectOrGroupA).Overlap((GenObject)objectOrGroupB, callback);
                }
                else if (objectOrGroupB is GenGroup)
                {
                    if (((GenGroup)objectOrGroupB).Quadtree != null)
                    {
                        if (((GenGroup)objectOrGroupB).Quadtree.Overlap((GenObject)objectOrGroupA, callback, separate, penetrate, collidableEdges) && !overlap)
                            overlap = true;
                    }
                    else
                        throw new Exception(String.Format("{0} must have a quadtree containing its members before being used in Overlap or Collide. Set useQuadtree to true in its constructor to fix this.", objectOrGroupB));
                }
            }
            else if (objectOrGroupA is GenGroup)
            {
                for (int i = 0; i < ((GenGroup)objectOrGroupA).Members.Count; i++)
                {
                    if (objectOrGroupB is GenObject)
                    {
                        if (separate)
                        {
                            if (((GenObject)((GenGroup)objectOrGroupA).Members[i]).Collide((GenObject)objectOrGroupB, callback, penetrate, collidableEdges) && !overlap)
                                overlap = true;
                        }
                        else
                        {
                            if (((GenObject)((GenGroup)objectOrGroupA).Members[i]).Overlap((GenObject)objectOrGroupB, callback) && !overlap)
                                overlap = true;
                        }
                    }
                    else if (objectOrGroupB is GenGroup)
                    {
                        if (((GenGroup)objectOrGroupB).Quadtree != null)
                        {
                            if (((GenGroup)objectOrGroupB).Quadtree.Overlap((GenObject)((GenGroup)objectOrGroupA).Members[i], callback, separate, penetrate, collidableEdges) && !overlap)
                                overlap = true;
                        }
                    }
                }
            }

            return overlap;
        }

        /// <summary>
        /// Applys collision detection and response between two objects, groups of objects, or tilemap that may overlap.
        /// </summary>
        /// <param name="objectOrGroupA">The first object, group, or tilemap to check for collisions.</param>
        /// <param name="objectOrGroupB">The second object, group, or tilemap to check for collisions.</param>
        /// <param name="callback">The delegate method that will be invoked if a collision occurs.</param>
        /// <param name="penetrate">Determines if the objects are able to penetrate each other for soft collision response.</param>
        /// <param name="collidableEdges">A bit field of flags determining which edges of the second object or group of objects are collidable.</param>
        /// <returns>True if a collision occurs, false if not.</returns>
        public static bool Collide(GenBasic objectOrGroupA, GenBasic objectOrGroupB, CollideEvent callback = null, bool penetrate = true, GenObject.Direction collidableEdges = GenObject.Direction.Any)
        {
            if (objectOrGroupA is GenTilemap)
                return ((GenTilemap)objectOrGroupA).Collide(objectOrGroupB, callback);
            else if (objectOrGroupB is GenTilemap)
                return ((GenTilemap)objectOrGroupB).Collide(objectOrGroupA, callback);

            return Overlap(objectOrGroupA, objectOrGroupB, callback, true, penetrate, collidableEdges);
        }

        /// <summary>
        /// Give the screen a flash effect.
        /// </summary>
        /// <param name="intensity">The intensity, or starting opacity, of the screen flash.</param>
        /// <param name="duration">The duration of the screen flash, in seconds.</param>
        /// <param name="color">The color of the screen flash. Use null to default to white.</param>
        /// <param name="forceReset">A flag used to determine if the flash will reset any current screen flash.</param>
        /// <param name="callback">The method that will be invoked after the screen flash has finished.</param>
        public static void Flash(float intensity = 1f, float duration = 1f, Color? color = null, bool forceReset = false, Action callback = null)
        {
            _screenEffect.Flash(intensity, duration, color, forceReset, callback);
        }

        /// <summary>
        /// Give the screen a fade effect.
        /// </summary>
        /// <param name="duration">The duration of the screen fade, in seconds.</param>
        /// <param name="color">The color of the screen fade. Use null to default to black.</param>
        /// <param name="callback">The method that will be invoked after the screen fade has finished.</param>
        public static void Fade(float duration = 1f, Color? color = null, Action callback = null)
        {
            _screenEffect.Fade(duration, color, callback);
        }

        /// <summary>
        /// Shows the volume control display.
        /// </summary>
        private static void ShowVolumeDisplay()
        {
            // Set the alpha values of the volume bars.
            for (int i = 0; i < 10; i++)
                ((GenSprite)_volumeBars.Members[i]).Color = (i < Math.Round(_volume * 10)) ? Color.White : Color.White * 0.5f;

            _volumeDisplay.Revive();
            _volumeDisplayTimer.Start();

            // Play the beep sound to hear the current volume level.
            _volumeBeep.Play();
        }

        /// <summary>
        /// Hides the volume control display.
        /// </summary>
        private static void HideVolumeDisplay()
        {
            _volumeDisplay.Kill();
        }
    }
}