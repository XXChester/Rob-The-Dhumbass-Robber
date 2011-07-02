using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GWNorthEngine.Engine;
using GWNorthEngine.Engine.Params;
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
		private Display mapSelectionMenu;
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
#if WINDOWS
#if DEBUG
			ScriptManager.getInstance().LogFile = "Log.log";
#endif
#endif
			ResourceManager.getInstance().init(GraphicsDevice, Content);
			this.mainMenu = new MainMenu(Content);
			this.inGameMenu = new InGameMenu(Content);
			this.gameDisplay = new GameDisplay(Content);
			this.mapSelectionMenu = new MapSeletMenu(Content);
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent() {
			this.gameDisplay.dispose();
			this.mainMenu.dispose();
			this.inGameMenu.dispose();
			this.mapSelectionMenu.dispose();
			this.activeDisplay.dispose();
			SoundManager.getInstance().dispose();
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
				 if (StateManager.getInstance().CurrentGameState == StateManager.GameState.MainMenu) {
					if (Keyboard.GetState().IsKeyDown(Keys.Escape) && this.previousKeyBoardState.IsKeyUp(Keys.Escape)) {
						this.Exit();
					}
				 } else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Exit) {
					 this.Exit();
				 }
			
			// Transition code
			if (StateManager.getInstance().CurrentGameState == StateManager.GameState.MainMenu) {
				if (StateManager.getInstance().PreviousGameState == StateManager.GameState.MainMenu &&
					StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
						this.activeDisplay = this.mainMenu;
				} else if (StateManager.getInstance().PreviousGameState == StateManager.GameState.InGameMenu &&
					StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
					this.activeDisplay = this.mainMenu;
				} else if (StateManager.getInstance().PreviousGameState == StateManager.GameState.InGameMenu &&
					StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
					this.activeDisplay = this.inGameMenu;
				} else if (StateManager.getInstance().PreviousGameState == StateManager.GameState.MapSelection &&
					StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
						this.activeDisplay = this.mapSelectionMenu;
				} else {
					this.activeDisplay = this.mainMenu;
				}
			} else  if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Waiting || 
				StateManager.getInstance().CurrentGameState == StateManager.GameState.Active) {
				if (StateManager.getInstance().PreviousGameState == StateManager.GameState.MapSelection &&
					StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
						this.activeDisplay = this.gameDisplay;
				} else if (StateManager.getInstance().PreviousGameState == StateManager.GameState.MapSelection &&
					StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
						this.activeDisplay = this.mapSelectionMenu;
				} else if (StateManager.getInstance().PreviousGameState == StateManager.GameState.Active &&
					StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
					this.activeDisplay = this.mainMenu;
				} else if (StateManager.getInstance().PreviousGameState == StateManager.GameState.Active &&
					StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
					this.activeDisplay = this.gameDisplay;
				} else if (StateManager.getInstance().PreviousGameState == StateManager.GameState.InGameMenu &&
					StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
					this.activeDisplay = gameDisplay;
				} else if (StateManager.getInstance().PreviousGameState == StateManager.GameState.InGameMenu &&
					StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
					this.activeDisplay = this.inGameMenu;
				} else {
					this.activeDisplay = this.gameDisplay;
				}
			} else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.InGameMenu) {
				if (StateManager.getInstance().PreviousGameState == StateManager.GameState.Active &&
					StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
					this.activeDisplay = this.gameDisplay;
				} else if (StateManager.getInstance().PreviousGameState == StateManager.GameState.Active &&
					StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
					this.activeDisplay = this.inGameMenu;
				} else if (StateManager.getInstance().PreviousGameState == StateManager.GameState.Waiting &&
					StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
					this.activeDisplay = this.gameDisplay;
				} else if (StateManager.getInstance().PreviousGameState == StateManager.GameState.Waiting &&
					StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
					this.activeDisplay = this.inGameMenu;
				} else if (StateManager.getInstance().PreviousGameState == StateManager.GameState.GameOver &&
					StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
					this.activeDisplay = this.gameDisplay;
				} else if (StateManager.getInstance().PreviousGameState == StateManager.GameState.GameOver &&
					StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
					this.activeDisplay = this.gameDisplay;
				} else {
					this.activeDisplay = this.inGameMenu;
				}
			} else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.MapSelection) {
				if (StateManager.getInstance().PreviousGameState == StateManager.GameState.MainMenu &&
					StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
					this.activeDisplay = this.mainMenu;
				} else {
					this.activeDisplay = this.mapSelectionMenu;
				}
			} else {
				if (StateManager.getInstance().CurrentGameState == StateManager.GameState.InitGame) {
					this.activeDisplay = this.mapSelectionMenu;
					((GameDisplay)this.gameDisplay).reset();
					StateManager.getInstance().CurrentGameState = StateManager.GameState.Waiting;
					StateManager.getInstance().PreviousGameState = StateManager.GameState.MapSelection;
				} else {
					this.activeDisplay = this.gameDisplay;
				}
			}

			float elapsed = gameTime.ElapsedGameTime.Milliseconds;
			this.activeDisplay.update(elapsed);
			this.previousKeyBoardState = Keyboard.GetState();
			this.previousMouseState = Mouse.GetState();
			SoundManager.getInstance().update();
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

