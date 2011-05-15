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
		private Display mainMenu;
		private Display inGameMenu;
		private Display activeDisplay;
		private KeyboardState previousKeyBoardState;
		private MouseState previousMouseState;

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
			ResourceManager.getInstance().init(GraphicsDevice, Content);
			string MAP_INFORMATION =  Directory.GetCurrentDirectory() +  "\\Scripts\\Map1";
			this.mainMenu = new MainMenu(Content);
			this.inGameMenu = new InGameMenu(Content);
			this.gameDisplay = new GameDisplay(GraphicsDevice, Content, MAP_INFORMATION);
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent() {
			this.mainMenu.dispose();
			this.inGameMenu.dispose();
			this.gameDisplay.dispose();
			this.activeDisplay.dispose();
			ResourceManager.getInstance().dispose();
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
			if (StateManager.getInstance().CurrentGameState != StateManager.GameState.InitReturnToMain) {
				if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Active) {
					if (Keyboard.GetState().IsKeyDown(Keys.Escape) && this.previousKeyBoardState.IsKeyUp(Keys.Escape)) {
						StateManager.getInstance().CurrentGameState = StateManager.GameState.InGameMenu;
					}
				} else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.MainMenu) {
					if (Keyboard.GetState().IsKeyDown(Keys.Escape) && this.previousKeyBoardState.IsKeyUp(Keys.Escape)) {
						this.Exit();
					}
				} else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.InGameMenu) {
					if (Keyboard.GetState().IsKeyDown(Keys.Escape) && this.previousKeyBoardState.IsKeyUp(Keys.Escape)) {
						StateManager.getInstance().CurrentGameState = StateManager.GameState.Active;
					}
				} else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Exit) {
					this.Exit();
				}
			}

			if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Active || StateManager.getInstance().CurrentGameState == StateManager.GameState.GameOver) {
				this.activeDisplay = this.gameDisplay;
			} else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.MainMenu) {
				this.activeDisplay = this.mainMenu;
			} else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.InGameMenu) {
				this.activeDisplay = this.inGameMenu;
			} else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.InitGame) {
				this.activeDisplay = this.gameDisplay;
				((GameDisplay)this.activeDisplay).reset(true);
				StateManager.getInstance().CurrentGameState = StateManager.GameState.Active;
			} else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.InitReturnToMain) {
				StateManager.getInstance().CurrentGameState = StateManager.GameState.MainMenu;
				this.activeDisplay = this.mainMenu;
			}

			float elapsed = gameTime.ElapsedGameTime.Milliseconds;
			this.activeDisplay.update(elapsed);
			this.previousKeyBoardState = Keyboard.GetState();
			this.previousMouseState = Mouse.GetState();
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.Black);

			base.spriteBatch.Begin();
			this.activeDisplay.render(base.spriteBatch);
			base.spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
