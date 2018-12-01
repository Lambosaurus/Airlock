using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Net;

using Airlock.Util;
using Airlock.Server;
using Airlock.Client;
using Airlock.Render;
using System;
using NetCode;

namespace Airlock
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class AirlockGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        AirlockServer Server;
        AirlockClient Client;
        Camera WorldCamera;
        Point Resolution;
        AirlockSettings Settings;

        private double LastUpdateTime = 0.0f;

        public AirlockGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Settings = new AirlockSettings();
            Resolution = new Point(Settings.ResolutionX, Settings.ResolutionY);
            graphics.PreferredBackBufferWidth = Resolution.X;
            graphics.PreferredBackBufferHeight = Resolution.Y;
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
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Drawing.Load(Content);
            
            Server = new AirlockServer(11002);
            Client = new AirlockClient(IPAddress.Parse("127.0.0.1"), 11002, 11003);
            WorldCamera = new Camera(spriteBatch, Resolution.ToVector2());
            
            NetTime.Realtime = false;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            NetTime.Advance(1000.0 / 60.0);
            double now = NetTime.Seconds();
            float elapsed = (float)(now - LastUpdateTime);
            LastUpdateTime = now;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Server.Close();
                Client.Close();
                Exit();
            }
            
            Server.Update(elapsed);
            Client.Update(elapsed);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            WorldCamera.Batch.Begin(samplerState: SamplerState.PointClamp);
            Client.Render(WorldCamera);
            WorldCamera.Batch.End();

            base.Draw(gameTime);
        }
    }
}
