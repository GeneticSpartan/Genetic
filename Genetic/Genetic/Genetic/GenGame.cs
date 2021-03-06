using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Genetic.Sound;

namespace Genetic
{
    /// <summary>
    /// This is the main type for your game.
    /// Manages the initialization of required game resources.
    /// <c>GenGame</c> will be used to initialize the game in the main entry point of the program.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// The width of the game window.
        /// </summary>
        protected int _width;

        /// <summary>
        /// The height of the game window.
        /// </summary>
        protected int _height;

        /// <summary>
        /// The initial scale at which to draw objects in a camera.
        /// </summary>
        public float Zoom;

        /// <summary>
        /// Gets the width of the game window.
        /// </summary>
        public int Width
        {
            get { return _width; }
        }

        /// <summary>
        /// Gets the height of the game window.
        /// </summary>
        public int Height
        {
            get { return _height; }
        }

        /// <param name="width">The width of the game window.</param>
        /// <param name="height">The height of the game window.</param>
        /// <param name="initialState">The initial state of the game.</param>
        /// <param name="drawMode">The mode that determines how game objects should be drawn to the screen.</param>
        /// <param name="zoom">The scale of the initial camera. The value must be 1.0 or greater.</param>
        /// <param name="fullScreen">A flag used to set the game to fullscreen.</param>
        public GenGame(int width, int height, GenState initialState, GenG.DrawType drawMode = GenG.DrawType.Smooth, float zoom = 1f, bool fullScreen = false)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = fullScreen;
            //graphics.PreferMultiSampling = false;
            //IsFixedTimeStep = false;
            //graphics.SynchronizeWithVerticalRetrace = false;
            //TargetElapsedTime = new TimeSpan(10000000L / 30L);
            
            // Set the width and height of the game window.
            _width = width;
            _height = height;
            graphics.PreferredBackBufferWidth = _width;
            graphics.PreferredBackBufferHeight = _height;

            GenG.DrawMode = drawMode;

            Zoom = zoom;

            GenG.SwitchState(initialState);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize necessary game resources.
            GenG.Initialize(this, GraphicsDevice, Content, spriteBatch);
            GenU.Initialize();
            GenMusic.Initialize();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            GenG.Update(gameTime);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GenG.Draw();

            base.Draw(gameTime);
        }
    }
}