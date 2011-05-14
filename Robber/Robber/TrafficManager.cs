using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GWNorthEngine.Engine;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Scripting;
namespace Robber {
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class TrafficManager : BaseRenderer {

		private Display gameDisplay;
		private Display activeDisplay;

		public TrafficManager() {
			BaseRendererParams baseParms = new BaseRendererParams();
			baseParms.MouseVisible = true;
			baseParms.WindowsText = "Rob, The Dumbass Robber";
#if DEBUG
			baseParms.RunningMode = RunningMode.Debug;
#else
			baseParms.RunningMode = RunningMode.Release;
#endif
			base.initialize(baseParms);
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent() {
			ScriptManager.getInstance().LogFile = "Log.log";
			string MAP_INFORMATION =  Directory.GetCurrentDirectory() +  "\\Scripts\\Map1";
			this.gameDisplay = new GameDisplay(GraphicsDevice, Content, MAP_INFORMATION);
			this.activeDisplay = this.gameDisplay;
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent() {
			this.activeDisplay.dispose();
			base.UnloadContent();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime) {
#if DEBUG
			base.Window.Title = "Rob, The Dumb Ass Robber...FPS: " + FrameRate.getInstance().calculateFrameRate(gameTime) + "    X:" + Mouse.GetState().X + " Y:" + Mouse.GetState().Y;
#endif
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
				this.Exit();
			}

			float elapsed = gameTime.ElapsedGameTime.Milliseconds;
			this.activeDisplay.update(elapsed);
			

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.Yellow);

			base.spriteBatch.Begin();
			this.activeDisplay.render(base.spriteBatch);
			base.spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
