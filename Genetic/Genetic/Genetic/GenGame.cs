using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Genetic
{
    /// <summary>
    /// This is the main type for your game
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
        /// The initial scale at which to draw objects in the camera.
        /// </summary>
        protected float _zoom;

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
        /// <param name="zoom">The scale of the initial camera.</param>
        /// <param name="fps">The amount of frames that the game will run each second.</param>
        public GenGame(int width, int height, GenState initialState, float zoom = 1)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //IsFixedTimeStep = false;
            //graphics.SynchronizeWithVerticalRetrace = false;
            
            // Set the width and height of the game window.
            _width = width;
            _height = height;
            graphics.PreferredBackBufferWidth = _width;
            graphics.PreferredBackBufferHeight = _height;

            _zoom = zoom;

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

            GenG.Initialize(this, GraphicsDevice, Content, spriteBatch);
            GenU.Initialize();

            GenG.Camera = GenG.AddCamera(new GenCamera(0, 0, _width, _height, _zoom));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            GenG.Update();
            
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
