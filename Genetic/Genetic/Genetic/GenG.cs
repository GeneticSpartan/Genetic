using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Genetic.Geometry;
using Genetic.Input;
using Genetic.Physics;
using Genetic.Sound;

namespace Genetic
{
    /// <summary>
    /// A global manager for most necessary game resources.
    /// Manages all game states and cameras.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public static class GenG
    {
        /// <summary>
        /// The mode that determines how game objects should be drawn to the screen.
        /// </summary>
        public enum DrawType
        {
            /// <summary>
            /// Draws using the point clamp sampler state to achieve sharp texture edges, ideal for pixel art.
            /// </summary>
            Pixel,

            /// <summary>
            /// Draws using the linear clamp sampler state to achieve smooth texture edges.
            /// </summary>
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
        /// The global default spritefont used for drawing debug strings.
        /// </summary>
        public static SpriteFont DebugFont;

        /// <summary>
        /// The current <c>GenState</c> being run by the game.
        /// </summary>
        private static GenState _state;

        /// <summary>
        /// The next state that will be loaded.
        /// </summary>
        private static GenState _requestedState;

        /// <summary>
        /// The <c>GenState</c> that will run while a requested state is being loaded.
        /// </summary>
        private static GenState _loadState;

        /// <summary>
        /// A <c>BackgroundWorker</c> used to a load a requested state while a load state is running.
        /// </summary>
        private static BackgroundWorker _stateLoader;

        /// <summary>
        /// A flag used to determine if a requested state has finished being created.
        /// </summary>
        private static bool _stateLoaded;

        /// <summary>
        /// A flag used to determine if a loaded requested state is permitted to start on the next update.
        /// </summary>
        private static bool _canStartState;

        /// <summary>
        /// The amount of seconds that must elapse between each update.
        /// </summary>
        private static float _timeStep;

        /// <summary>
        /// The inverse of the time step, used for optimizing physics simulations.
        /// </summary>
        private static float _inverseTimeStep;

        /// <summary>
        /// The amount to scale the physics time factor by. A default value of 1.0 means there is no change.
        /// </summary>
        public static float TimeScale;

        /// <summary>
        /// The amount of seconds that have elapsed since the last update relative to the time scale.
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
        /// A counter used to keep track of the number of frames drawn per second.
        /// </summary>
        private static int fpsCounter;

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
        /// The screen effect manager.
        /// </summary>
        private static GenScreenEffect _screenEffect;
#if WINDOWS
        /// <summary>
        /// An array containing multiple keyboard inputs used for checking key presses and releases.
        /// </summary>
        private static GenKeyboard[] _keyboards;

        /// <summary>
        /// The mouse input used for checking button presses and releases.
        /// </summary>
        private static GenMouse _mouse;
#endif
        /// <summary>
        /// An array containing multiple game pad inputs used for checking button presses and releases.
        /// </summary>
        private static GenGamePad[] _gamePads;

        #region Volume Fields
        /// <summary>
        /// The global volume for all sounds, a value from 0.0 to 1.0.
        /// </summary>
        private static float _volume;

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
        /// A flag used to determine if debug mode can be enabled.
        /// </summary>
        public static bool AllowDebug;

        /// <summary>
        /// Determines whether debug mode is on or off.
        /// </summary>
        public static bool IsDebug;

        /// <summary>
        /// A flag used to determine if the debug info, such as framerate and memory usage, should be drawn to the screen.
        /// Useful for forcing the display of debug info without needing to be in debug mode.
        /// </summary>
        public static bool ShowDebugInfo;

        /// <summary>
        /// A string used to display debug information.
        /// </summary>
        private static string _debugInfo;

        /// <summary>
        /// The position to draw debug information.
        /// </summary>
        private static Vector2 _debugInfoPosition;

        /// <summary>
        /// The background box of the debug info display.
        /// </summary>
        private static GenSprite _debugInfoBox;

        /// <summary>
        /// A stopwatch used to measure the execution time of update and draw loops.
        /// </summary>
        private static Stopwatch _debugStopwatch;

        /// <summary>
        /// Holds the accumulation of time used to execute update loops.
        /// </summary>
        private static float _updateTime;

        /// <summary>
        /// Holds the accumulation of time used to execute draw loops.
        /// </summary>
        private static float _drawTime;

        /// <summary>
        /// Gets the current <c>GenState</c> being run by the game.
        /// </summary>
        public static GenState State
        {
            get { return _state; }
        }

        /// <summary>
        /// Gets a flag used to determine if a requested state has finished being created.
        /// </summary>
        public static bool StateLoaded
        {
            get { return _stateLoaded; }
        }

        /// <summary>
        /// Gets amount of seconds that must elapse between each update.
        /// </summary>
        public static float TimeStep
        {
            get { return _timeStep; }
        }

        /// <summary>
        /// Gets the inverse of the time step, used for physics simulation optimization.
        /// </summary>
        public static float InverseTimeStep
        {
            get { return _inverseTimeStep; }
        }

        /// <summary>
        /// Gets the amount of seconds that have elapsed since the last update relative to the time scale.
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
#if WINDOWS
        /// <summary>
        /// Gets an array containing the multiple keyboard inputs used for checking key presses and releases.
        /// </summary>
        public static GenKeyboard[] Keyboards
        {
            get { return _keyboards; }
        }

        /// <summary>
        /// Gets the mouse input used for checking button presses and releases.
        /// </summary>
        public static GenMouse Mouse
        {
            get { return _mouse; }
        }
#endif
        /// <summary>
        /// Gets an array containing the multiple game pad inputs used for checking button presses and releases.
        /// </summary>
        public static GenGamePad[] GamePads
        {
            get { return _gamePads; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the mouse cursor should be visible.
        /// </summary>
        public static bool IsMouseVisible
        {
            get { return Game.IsMouseVisible; }

            set { Game.IsMouseVisible = value; }
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

            set
            {
                _volume = MathHelper.Clamp(value, 0.0f, 1.0f);
                SoundEffect.MasterVolume = _volume;
            }
        }

        /// <summary>
        /// Initializes the necessary game resources.
        /// </summary>
        /// <param name="game">The main type for the game.</param>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="content">The content manager used for loading game assets.</param>
        /// <param name="spriteBatch">The sprite batch used for drawing game objects.</param>
        public static void Initialize(GenGame game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spriteBatch)
        {
            GlobalSeed = Environment.TickCount;
            Game = game;
            GraphicsDevice = graphicsDevice;
            Content = content;
            SpriteBatch = spriteBatch;

            // Load default assets.
            Pixel = GenU.MakeTexture(Color.White, 1, 1);
            Font = LoadContent<SpriteFont>("cellphone");
            DebugFont = LoadContent<SpriteFont>("genetic_debug");
            DebugFont.LineSpacing = 10;

            _state = null;
            _stateLoader = null;
            _stateLoaded = false;
            _canStartState = false;

            _timeStep = 1f / 60f;
            _inverseTimeStep = 1f / _timeStep;
            TimeScale = 1f;

            WorldBounds = new Rectangle(0, 0, Game.Width, Game.Height);
            _screenEffect = new GenScreenEffect(GraphicsDevice.Viewport.Bounds);

            _updateTimer = 0f;
            fpsCounter = 0;
            _fpsStopwatch = new Stopwatch();
            _fpsStopwatch.Start();

            Paused = false;
#if WINDOWS
            // Create the input managers.
            _keyboards = new GenKeyboard[4];
            _keyboards[0] = new GenKeyboard(PlayerIndex.One);
            _keyboards[1] = new GenKeyboard(PlayerIndex.Two);
            _keyboards[2] = new GenKeyboard(PlayerIndex.Three);
            _keyboards[3] = new GenKeyboard(PlayerIndex.Four);

            _mouse = new GenMouse();
#endif
            _gamePads = new GenGamePad[4];
            _gamePads[0] = new GenGamePad(PlayerIndex.One);
            _gamePads[1] = new GenGamePad(PlayerIndex.Two);
            _gamePads[2] = new GenGamePad(PlayerIndex.Three);
            _gamePads[3] = new GenGamePad(PlayerIndex.Four);

            _volume = 1f;
            AllowDebug = true;
            IsDebug = false;
            ShowDebugInfo = false;
            _debugInfo = String.Empty;
            _debugInfoPosition = new Vector2(TitleSafeArea.X + 16f, TitleSafeArea.Y + 16f);
            _debugInfoBox = new GenSprite(_debugInfoPosition.X - 8f, _debugInfoPosition.Y - 8f, Pixel, 1, 1);
            _debugInfoBox.Color = Color.Black;
            _debugInfoBox.Alpha = 0.5f;
            _debugInfoBox.PostUpdate(); // Call PostUpdate once on the debug info box to update its draw color.
            _debugStopwatch = new Stopwatch();

            // Create the volume control display.
            _volumeDisplay = new GenGroup();

            _volumeBox = new GenSprite((Game.Width / 2) - 46, 0, null, 92, 54);
            _volumeBox.MakeTexture(Color.Black * 0.5f, 92, 54, false);
            _volumeBox.SetOrigin(0f, 0f);
            _volumeDisplay.Add(_volumeBox);

            _volumeBars = new GenGroup();
            _volumeDisplay.Add(_volumeBars);

            for (int i = 0; i < 10; i++)
            {
                _volumeBars.Add(new GenSprite(0f, 0f, null, 4, i * 2 + 2));
                (_volumeBars.Members[i] as GenSprite).MakeTexture(Color.White);
                (_volumeBars.Members[i] as GenObject).SetOrigin(0f, 0f);
                (_volumeBars.Members[i] as GenObject).ParentOffset.X = i * 8 + 8;
                (_volumeBars.Members[i] as GenObject).ParentOffset.Y = 18 - i * 2 + 8;
                (_volumeBars.Members[i] as GenObject).SetParent(_volumeBox, GenObject.ParentType.Position);
            }

            _volumeText = new GenText("VOLUME", 0f, 0f, 0, 0);
            _volumeText.SetOrigin(0f, 0f);
            _volumeText.SetParent(_volumeBox, GenObject.ParentType.Position);
            _volumeText.ParentOffset.X = 7;
            _volumeText.ParentOffset.Y = 32;
            _volumeDisplay.Add(_volumeText);

            _volumeBeep = new GenSound(LoadContent<SoundEffect>("beep"));
            _volumeDisplay.Add(_volumeBeep);

            _volumeDisplayTimer = new GenTimer(1f, HideVolumeDisplay);
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
            _debugStopwatch.Reset();
            _debugStopwatch.Start();

            _scaleTimeStep = (float)gameTime.ElapsedGameTime.TotalSeconds * TimeScale;
            _elapsedTime += _scaleTimeStep;

            if (_requestedState != null)
            {
                if ((_loadState == null) || _canStartState)
                {
                    LoadState(_requestedState);

                    _requestedState = null;
                    _loadState = null;

                    _stateLoaded = false;
                    _canStartState = false;

                    // Collect any remaining garbage after loading the state.
                    GC.Collect();
                }
                else if ((_stateLoader == null) && !_stateLoaded)
                {
                    _stateLoader = new BackgroundWorker();
                    _stateLoader.DoWork += new DoWorkEventHandler(StateLoader_DoWork);
                    _stateLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(StateLoader_RunWorkerCompleted);
                    _stateLoader.RunWorkerAsync();

                    LoadState(_loadState);
                }
            }

            while (_updateTimer >= _timeStep)
            {
#if WINDOWS
                // Update the keyboards.
                _keyboards[(int)PlayerIndex.One].Update();
                _keyboards[(int)PlayerIndex.Two].Update();
                _keyboards[(int)PlayerIndex.Three].Update();
                _keyboards[(int)PlayerIndex.Four].Update();

                // Update the mouse.
                _mouse.Update();
#endif
                // Update the game pads.
                _gamePads[(int)PlayerIndex.One].Update();
                _gamePads[(int)PlayerIndex.Two].Update();
                _gamePads[(int)PlayerIndex.Three].Update();
                _gamePads[(int)PlayerIndex.Four].Update();

                // Update music.
                GenMusic.Update();
#if WINDOWS
                // Volume down control.
                if (_keyboards[(int)PlayerIndex.One].JustPressed(Keys.OemMinus))
                {
                    Volume -= 0.1f;
                    ShowVolumeDisplay();
                }

                // Volume up control.
                if (_keyboards[(int)PlayerIndex.One].JustPressed(Keys.OemPlus))
                {
                    Volume += 0.1f;
                    ShowVolumeDisplay();
                }

                if (_keyboards[(int)PlayerIndex.One].JustPressed(Keys.OemTilde))
                    IsDebug = !IsDebug;
#endif                
                _state.PreUpdate();
                _state.Update();
                _state.PostUpdate();

                _volumeDisplay.Update();
                _volumeDisplay.PostUpdate();

                _updateTimer -= _timeStep;
            }

            _updateTimer += _scaleTimeStep;

            _debugStopwatch.Stop();
            _updateTime += _debugStopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Starts loading the currently requested state by calling its Create method.
        /// </summary>
        static void StateLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            _requestedState.Create();
        }

        /// <summary>
        /// Completes the state loader after the currently requested state has been loaded.
        /// </summary>
        static void StateLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _stateLoaded = true;

            _stateLoader.DoWork -= StateLoader_DoWork;
            _stateLoader.RunWorkerCompleted -= StateLoader_RunWorkerCompleted;
            _stateLoader = null;
        }

        /// <summary>
        /// Called by the game object every frame to draw the game components.
        /// </summary>
        public static void Draw()
        {
            _debugStopwatch.Reset();
            _debugStopwatch.Start();

            // Draw the game objects within each camera.
            foreach (GenCamera camera in State.Cameras)
            {
                if (camera.Exists && camera.Visible)
                    camera.DrawObjects();
            }

            // If in the pixel draw mode, draw each camera render target to a separate pixel render target.
            if (DrawMode == DrawType.Pixel)
            {
                foreach (GenCamera camera in State.Cameras)
                {
                    if (camera.Exists && camera.Visible)
                        camera.DrawPixelRenderTarget();
                }
            }

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(_state.BgColor);

            // Draw the final camera render targets.
            foreach (GenCamera camera in State.Cameras)
            {
                if (camera.Exists && camera.Visible)
                    camera.DrawFinal();
            }

            // Draw the screen effects.
            if (DrawMode == DrawType.Pixel)
                SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.Identity);
            else
                SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, Matrix.Identity);

            _screenEffect.Draw();

            SpriteBatch.End();

            // Draw any final game visuals.
            SpriteBatch.Begin();
            _volumeDisplay.Draw(null);

            // Draw the debug info.
            if (ShowDebugInfo || (AllowDebug && IsDebug))
            {
                SpriteBatch.Draw(_debugInfoBox.Texture, _debugInfoBox.Position, _debugInfoBox.SourceRect, _debugInfoBox.Color * _debugInfoBox.Alpha);
                SpriteBatch.DrawString(DebugFont, _debugInfo, _debugInfoPosition, Color.White);
            }

            SpriteBatch.End();

            _debugStopwatch.Stop();
            _drawTime += _debugStopwatch.ElapsedMilliseconds;

            // Update the debug info at set intervals.
            if (_fpsStopwatch.ElapsedMilliseconds >= 1000)
            {
                _fpsStopwatch.Reset();
                _fpsStopwatch.Start();

                if (ShowDebugInfo || (AllowDebug && IsDebug))
                {
                    _debugInfo =
                        "Genetic v1.0" +
                        "\n\nFPS: " + fpsCounter +
                        "\nMemory: " + Math.Round((GC.GetTotalMemory(false) / 1024f) / 1024f, 2) + " MB" +
                        "\nUpdate: " + Math.Round(_updateTime / fpsCounter, 2) + " ms" +
                        "\nDraw: " + Math.Round(_drawTime / fpsCounter, 2) + " ms";

                    // Resize the background box to fit the debug info text.
                    Vector2 debugInfoMeasure = DebugFont.MeasureString(_debugInfo);
                    _debugInfoBox.SetSourceRect(0, 0, (int)debugInfoMeasure.X + 16, (int)debugInfoMeasure.Y + 16, false);
                }

                fpsCounter = 0;
                _updateTime = 0f;
                _drawTime = 0f;
            }
            else
                fpsCounter++;
        }

        private static void LoadState(GenState state)
        {
            // Call Create on the current state if it has not already been created previously.
            if (!state.Loaded)
                state.Create();

            _state = state;
            _state.Start();
        }

        public static bool StartLoadedState()
        {
            return _canStartState = _stateLoaded;
        }

        /// <summary>
        /// A simplified method for loading assets that have been processed by the Content Pipeline.
        /// </summary>
        /// <typeparam name="T">The <c>Type</c> of the content to load. Example: Texture2D, SoundEffect, SpriteFont, etc.</typeparam>
        /// <param name="assetName">The asset's name, relative to the loader root directory, and not including the .xnb file extension.</param>
        /// <returns>The content that was loaded.</returns>
        public static T LoadContent<T>(string assetName)
        {
            return Content.Load<T>(assetName);
        }

        /// <summary>
        /// Adds an object to the members list of the current state.
        /// Useful for adding objects to the current state within other object classes.
        /// </summary>
        /// <param name="basic">The object to add.</param>
        /// <returns>The object added to the members list of the current state.</returns>
        public static GenBasic Add(GenBasic basic)
        {
            return _state.Add(basic);
        }

        /// <summary>
        /// Removes an object from the members list of the current state.
        /// Useful for removing objects from the current state within other object classes.
        /// </summary>
        /// <param name="basic">The object to remove.</param>
        /// <returns>The object removed from the members list of the current state.</returns>
        public static GenBasic Remove(GenBasic basic)
        {
            return _state.Remove(basic);
        }

        /// <summary>
        /// Resets the current state by loading a new instance of the state.
        /// </summary>
        /// <param name="loadingState">The game state to run while the current state is being reset.</param>
        public static void ResetState(GenState loadingState = null)
        {
            GenState newState = (GenState)Activator.CreateInstance(_state.GetType());

            SwitchState(newState, loadingState);
        }

        /// <summary>
        /// Sets the requested game state that will be loaded.
        /// </summary>
        /// <param name="newState">The new game state to load.</param>
        /// <param name="loadingState">The game state to run while the new state is loading.</param>
        public static void SwitchState(GenState newState, GenState loadingState = null)
        {
            _requestedState = newState;
            _loadState = loadingState;
        }

        /// <summary>
        /// Draws a line between the two given points.
        /// </summary>
        /// <param name="xA">The x position of the starting point of the line.</param>
        /// <param name="yA">The y position of the starting point of the line.</param>
        /// <param name="xB">The x position of the ending point of the line.</param>
        /// <param name="yB">The y position of the ending point of the line.</param>
        /// <param name="color">The color of the line. Defaults to white if set to null.</param>
        /// <param name="thickness">The thickness of the line, in pixels.</param>
        public static void DrawLine(float xA, float yA, float xB, float yB, Color? color = null, float thickness = 1)
        {
            color = color.HasValue ? color.Value : Color.White;

            Vector2 pointA = new Vector2(xA, yA);
            Vector2 pointB = new Vector2(xB, yB);

            float angle = (float)Math.Atan2(yB - yA, xB - xA);
            float length = (pointB - pointA).Length();

            Vector2 scale = new Vector2(length, thickness);

            SpriteBatch.Draw(Pixel, pointA, null, color.Value, angle, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draws a line between the two given points.
        /// </summary>
        /// <param name="pointA">The position of the starting point of the line.</param>
        /// <param name="pointB">The position of the ending point of the line.</param>
        /// <param name="color">The color of the line. Defaults to white if set to null.</param>
        /// <param name="thickness">The thickness of the line, in pixels.</param>
        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color? color = null, float thickness = 1)
        {
            color = color.HasValue ? color.Value : Color.White;

            float angle = (float)Math.Atan2(pointB.Y - pointA.Y, pointB.X - pointA.X);
            float length = (pointB - pointA).Length();

            SpriteBatch.Draw(Pixel, pointA, null, color.Value, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
        }

        /// <summary>
        /// Checks for overlap between two objects, groups of objects, or tilemaps.
        /// Quadtree data structures are used by groups for optimized overlap checks.
        /// </summary>
        /// <param name="objectOrGroupA">The first object, group, or tilemap to check for overlap.</param>
        /// <param name="objectOrGroupB">The second object, group, or tilemap to check for overlap.</param>
        /// <param name="callback">The delegate method that will be invoked if an overlap occurs.</param>
        /// <param name="separate">A flag used to determine if objects should collide with each other.</param>
        /// <param name="collidableEdges">A bit field of flags determining which edges of the second object or group of objects are collidable.</param>
        /// <returns>True if an overlap occurs, false if not.</returns>
        public static bool Overlap(GenBasic objectOrGroupA, GenBasic objectOrGroupB, CollideEvent callback = null, bool separate = false, GenObject.Direction collidableEdges = GenObject.Direction.Any)
        {
            bool overlap = false;

            if (objectOrGroupA is GenObject)
            {
                if (objectOrGroupB is GenObject)
                {
                    if (separate)
                        overlap = GenCollide.Collide(objectOrGroupA as GenObject, objectOrGroupB as GenObject, callback, collidableEdges);
                    else
                        overlap = GenCollide.Overlap(objectOrGroupA as GenObject, objectOrGroupB as GenObject, callback);
                }
                else if (objectOrGroupB is GenGroup)
                {
                    // If the second group does not currently have a quadtree used for overlap and collision checks, create one.
                    if ((objectOrGroupB as GenGroup).Quadtree == null)
                        (objectOrGroupB as GenGroup).MakeQuadtree();

                    if ((objectOrGroupB as GenGroup).Quadtree.Overlap(objectOrGroupA as GenObject, callback, separate, collidableEdges))
                        overlap = true;
                }
            }
            else if (objectOrGroupA is GenGroup)
            {
                // If the first group does not currently have a quadtree used for overlap and collision checks, create one.
                if ((objectOrGroupA as GenGroup).Quadtree == null)
                    (objectOrGroupA as GenGroup).MakeQuadtree();

                if (objectOrGroupB is GenObject)
                {
                    if ((objectOrGroupA as GenGroup).Quadtree.Overlap(objectOrGroupB as GenObject, callback, separate, collidableEdges))
                        overlap = true;
                }
                else if (objectOrGroupB is GenGroup)
                {
                    // If the second group does not currently have a quadtree used for overlap and collision checks, create one.
                    if ((objectOrGroupB as GenGroup).Quadtree == null)
                        (objectOrGroupB as GenGroup).MakeQuadtree();

                    // If the first and second groups are not the same, check for overlaps and collisions against game objects between the two groups.
                    // Otherwise, do overlap and collision checks among the group quadtree's own objects.
                    if (objectOrGroupA != objectOrGroupB)
                    {
                        // If the first group's quadtree contains the least game objects, call Overlap against the second group for each object in the first group.
                        // Otherwise, call Overlap against the first group for each object in the second group.
                        // This can reduce the amount of overlap and collision checks by iterating through the smallest group.
                        if ((objectOrGroupA as GenGroup).Quadtree.Objects.Count <= (objectOrGroupB as GenGroup).Quadtree.Objects.Count)
                        {
                            foreach (GenObject gameObject in (objectOrGroupA as GenGroup).Quadtree.Objects)
                            {
                                if (Overlap(gameObject, objectOrGroupB as GenGroup, callback, separate, collidableEdges))
                                    overlap = true;
                            }
                        }
                        else
                        {
                            foreach (GenObject gameObject in (objectOrGroupB as GenGroup).Quadtree.Objects)
                            {
                                if (Overlap(gameObject, objectOrGroupA as GenGroup, callback, separate, collidableEdges))
                                    overlap = true;
                            }
                        }
                    }
                    else
                    {
                        // Since the first and second group are the same, do overlap and collision checks among the group quadtree's own objects.
                        if ((objectOrGroupA as GenGroup).Quadtree.OverlapSelf(callback, separate, collidableEdges))
                            overlap = true;
                    }
                }
            }

            return overlap;
        }

        /// <summary>
        /// Applies collision detection and response between two objects, groups of objects, or tilemaps that may overlap.
        /// Quadtree data structures are used by groups for optimized collision checks.
        /// </summary>
        /// <param name="objectOrGroupA">The first object, group, or tilemap to check for collisions.</param>
        /// <param name="objectOrGroupB">The second object, group, or tilemap to check for collisions.</param>
        /// <param name="callback">The delegate method that will be invoked if a collision occurs.</param>
        /// <param name="collidableEdges">A bit field of flags determining which edges of the second object or group of objects are collidable.</param>
        /// <returns>True if a collision occurs, false if not.</returns>
        public static bool Collide(GenBasic objectOrGroupA, GenBasic objectOrGroupB, CollideEvent callback = null, GenObject.Direction collidableEdges = GenObject.Direction.Any)
        {
            if (objectOrGroupA is GenTilemap)
                return (objectOrGroupA as GenTilemap).Collide(objectOrGroupB, callback);
            else if (objectOrGroupB is GenTilemap)
                return (objectOrGroupB as GenTilemap).Collide(objectOrGroupA, callback);

            return Overlap(objectOrGroupA, objectOrGroupB, callback, true, collidableEdges);
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
                (_volumeBars.Members[i] as GenSprite).Color = (i < Math.Round(_volume * 10)) ? Color.White : Color.White * 0.5f;

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