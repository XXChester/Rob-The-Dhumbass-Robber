using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Robber.Interfaces;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
using GWNorthEngine.AI.AStar;
using GWNorthEngine.Scripting;
namespace Robber {
	public class MapSeletMenu : Display {
	#region Class variables
		private List<MapSelection> mapSelections;
		private ColouredButton exitToMainButton;
		private StaticDrawable2D title;
		private SoundEffect outroSfx;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public MapSeletMenu(ContentManager content) {
			ColouredButtonParams buttonParms = new ColouredButtonParams();
			buttonParms.Font = ResourceManager.getInstance().Font;
			buttonParms.Height = 25;
			buttonParms.LinesTexture = ResourceManager.getInstance().ButtonLineTexture;
			buttonParms.MouseOverColour = ResourceManager.MOUSE_OVER_COLOUR;
			buttonParms.RegularColour = ResourceManager.TEXT_COLOUR;
			buttonParms.StartX = 580;
			buttonParms.Width = 205;
			buttonParms.StartY = 557;
			buttonParms.Text = "Exit To Main Menu";
			buttonParms.TextsPosition = new Vector2(590f, buttonParms.StartY - 2);
			this.exitToMainButton = new ColouredButton(buttonParms);

			string[] maps = new string[] { "Map3", "Map2", "Map1"};
			this.mapSelections = new List<MapSelection>(maps.Length);
			for (int i = 0; i < maps.Length; i++) {
				this.mapSelections.Add(new MapSelection(content, maps[i], i, buttonParms.Width, buttonParms.Height, buttonParms.StartX, buttonParms.StartY));
			}

			// title
			StaticDrawable2DParams staticParms = new StaticDrawable2DParams();
			staticParms.Position = new Vector2(0f, -20f);
			staticParms.Texture = ResourceManager.getInstance().TitleTexture;
			this.title = new StaticDrawable2D(staticParms);

			// sound effects
			this.outroSfx = content.Load<SoundEffect>("LetsGo");
		}
		#endregion Constructor

		#region Support methods
		public override void update(float elapsed) {
			base.currentKeyBoardState = Keyboard.GetState();
			base.currentMouseState = Mouse.GetState();
			Vector2 mousePos = new Vector2(base.currentMouseState.X, base.currentMouseState.Y);
			foreach (MapSelection selection in this.mapSelections) {
				selection.update(elapsed);
			}
			this.exitToMainButton.processActorsMovement(mousePos);
			// mouse over sfx
			if (this.exitToMainButton.isActorOver(mousePos)) {
				if (!base.previousMouseOverButton) {
					if (ResourceManager.PLAY_SOUND) {
						ResourceManager.getInstance().MouseOverSfx.Play();
					}
				}
				base.previousMouseOverButton = true;
			} else {
				bool foundInLoop = false;
				foreach (MapSelection selection in this.mapSelections) {
					if (selection.PreviewButton.isActorOver(mousePos)) {
						if (!base.previousMouseOverButton) {
							if (ResourceManager.PLAY_SOUND) {
								ResourceManager.getInstance().MouseOverSfx.Play();
							}
						}
						foundInLoop = true;
						base.previousMouseOverButton = true;
						break;
					}
				}
				if (!foundInLoop) {
					base.previousMouseOverButton = false;
				}
			}
			if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.None) {
				if (base.currentMouseState.LeftButton == ButtonState.Pressed && base.prevousMouseState.LeftButton == ButtonState.Released) {
					if (this.exitToMainButton.isActorOver(mousePos)) {
						StateManager.getInstance().CurrentGameState = StateManager.GameState.MainMenu;
						StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionOut;
					} else {
						foreach (MapSelection selection in this.mapSelections) {
							if (selection.PreviewButton.isActorOver(mousePos)) {
								// Need a call back to load the correct map here
								StateManager.getInstance().CurrentGameState = StateManager.GameState.InitGame;
								StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionOut;
								StateManager.getInstance().MapInformation = selection.MapName;
								if (ResourceManager.PLAY_SOUND) {
									this.outroSfx.Play();
								}
								break;
							}
						}
					}
				}
			} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
				this.title.LightColour = base.fadeIn(Color.White);
				this.exitToMainButton.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));

				if (this.exitToMainButton.isActorOver(mousePos)) {
					this.exitToMainButton.updateColours(base.fadeIn(ResourceManager.MOUSE_OVER_COLOUR));
				}
				foreach (MapSelection selection in this.mapSelections) {
					if (selection.PreviewButton.isActorOver(mousePos)) {
						selection.updateColours(base.fadeIn(ResourceManager.MOUSE_OVER_COLOUR), base.fadeIn(Color.White));
					} else {
						selection.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR), base.fadeIn(Color.White));
					}

				}
			} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
				this.title.LightColour = base.fadeOut(Color.White);
				this.exitToMainButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
				if (this.exitToMainButton.isActorOver(mousePos)) {
					this.exitToMainButton.updateColours(base.fadeOut(ResourceManager.MOUSE_OVER_COLOUR));
				}
				foreach (MapSelection selection in this.mapSelections) {
					if (selection.PreviewButton.isActorOver(mousePos)) {
						selection.updateColours(base.fadeOut(ResourceManager.MOUSE_OVER_COLOUR), base.fadeOut(Color.White));
					} else {
						selection.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR), base.fadeOut(Color.White));
					}
				}

			}
			// if our transition time is up change our state
			if (base.transitionTimeElapsed()) {
				if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
					StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.None;
				} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
					StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionIn;
				}
			}
			if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.None) {
				if (Keyboard.GetState().IsKeyDown(Keys.Escape) && this.previousKeyBoardState.IsKeyUp(Keys.Escape)) {
					StateManager.getInstance().CurrentGameState = StateManager.getInstance().PreviousGameState;
					StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionOut;
				}
			}
			base.update(elapsed);
		}

		public override void render(SpriteBatch spriteBatch) {
			this.title.render(spriteBatch);
			foreach (MapSelection selection in this.mapSelections) {
				selection.render(spriteBatch);
			}
			this.exitToMainButton.render(spriteBatch);
		}
		#endregion Support methods

		#region Destructor
		public override void dispose() {
			if (this.exitToMainButton != null) {
				this.exitToMainButton.dispose();
			}
			if (this.title != null) {
				this.title.dispose();
			}
			foreach (MapSelection selection in this.mapSelections) {
				selection.dispose();
			}
			if (this.outroSfx != null) {
				this.outroSfx.Dispose();
			}
		}
		#endregion Destructor
	}
}
