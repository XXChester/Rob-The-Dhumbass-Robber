using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
using GWNorthEngine.AI.AStar;
using GWNorthEngine.Scripting;
namespace Robber {
	public class MapSeletMenu : Display {
	#region Class variables
		private List<MapSelection> mapSelections;
		private ColouredButton returnToModeSelectButton;
		private StaticDrawable2D title;
		private StaticDrawable2D noPreviewImage;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public MapSeletMenu(GraphicsDevice device, ContentManager content) {
			ColouredButtonParams buttonParms = new ColouredButtonParams();
			buttonParms.Font = ResourceManager.getInstance().Font;
			buttonParms.Height = 25;
			buttonParms.LinesTexture = ResourceManager.getInstance().ButtonLineTexture;
			buttonParms.MouseOverColour = ResourceManager.MOUSE_OVER_COLOUR;
			buttonParms.RegularColour = ResourceManager.TEXT_COLOUR;
			buttonParms.StartX = 580;
			buttonParms.Width = 205;
			buttonParms.StartY = 557;
			buttonParms.Text = "Mode Selection";
			buttonParms.TextsPosition = new Vector2(605f, buttonParms.StartY - 2);
			this.returnToModeSelectButton = new ColouredButton(buttonParms);

			// read in our map names from the Maps directory
			string[] maps = Directory.GetFiles(ResourceManager.MAP_FOLDER, "*.png");
			for (int i = 0; i < maps.Length; i++) {
				maps[i] = StringUtils.scrubPathAndExtFromFileName(maps[i]);
			}
			Array.Reverse(maps);

			// load up our map selections via the names from the dierctory
			this.mapSelections = new List<MapSelection>(maps.Length);
			for (int i = 0; i < maps.Length; i++) {
				this.mapSelections.Add(new MapSelection(device, maps[i], i, buttonParms.Width, buttonParms.Height, buttonParms.StartX, buttonParms.StartY));
			}

			// title
			StaticDrawable2DParams staticParms = new StaticDrawable2DParams();
			staticParms.Position = new Vector2(0f, -20f);
			staticParms.Texture = ResourceManager.getInstance().TitleTexture;
			this.title = new StaticDrawable2D(staticParms);

			// no preview image
			staticParms.Position = new Vector2(20f, 100f);
			staticParms.Texture = LoadingUtils.loadTexture2D(content, "NoPreview");
			staticParms.Scale = new Vector2(.8f, .8f);
			staticParms.LightColour = ResourceManager.TEXT_COLOUR;
			this.noPreviewImage = new StaticDrawable2D(staticParms);
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
			this.returnToModeSelectButton.processActorsMovement(mousePos);
			// mouse over sfx
			if (this.returnToModeSelectButton.isActorOver(mousePos)) {
				if (!base.previousMouseOverButton) {
					SoundManager.getInstance().sfxEngine.playSoundEffect(ResourceManager.getInstance().MouseOverSfx);
				}
				base.previousMouseOverButton = true;
			} else {
				bool foundInLoop = false;
				foreach (MapSelection selection in this.mapSelections) {
					if (selection.PreviewButton.isActorOver(mousePos)) {
						if (!base.previousMouseOverButton) {
							SoundManager.getInstance().sfxEngine.playSoundEffect(ResourceManager.getInstance().MouseOverSfx);
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
					if (this.returnToModeSelectButton.isActorOver(mousePos)) {
						StateManager.getInstance().CurrentGameState = StateManager.GameState.ModeSelect;
						StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionOut;
					} else {
						foreach (MapSelection selection in this.mapSelections) {
							if (selection.PreviewButton.isActorOver(mousePos)) {
								// Need a call back to load the correct map here
								StateManager.getInstance().CurrentGameState = StateManager.GameState.InitGame;
								StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionOut;
								StateManager.getInstance().MapInformation = selection.MapName;
								break;
							}
						}
					}
				}
			} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
				this.title.LightColour = base.fadeIn(Color.White);
				this.returnToModeSelectButton.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));
				this.noPreviewImage.LightColour = base.fadeIn(ResourceManager.TEXT_COLOUR);
				if (this.returnToModeSelectButton.isActorOver(mousePos)) {
					this.returnToModeSelectButton.updateColours(base.fadeIn(ResourceManager.MOUSE_OVER_COLOUR));
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
				this.returnToModeSelectButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
				this.noPreviewImage.LightColour = base.fadeOut(ResourceManager.TEXT_COLOUR);
				if (this.returnToModeSelectButton.isActorOver(mousePos)) {
					this.returnToModeSelectButton.updateColours(base.fadeOut(ResourceManager.MOUSE_OVER_COLOUR));
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
			bool foundMouseOver = false;
			foreach (MapSelection selection in this.mapSelections) {
				selection.render(spriteBatch);
				if (selection.PreviewButton.isActorOver(new Vector2(base.currentMouseState.X, base.currentMouseState.Y))) {
					foundMouseOver = true;
				}
			}
			if (!foundMouseOver) {
				this.noPreviewImage.render(spriteBatch);
			}
			this.returnToModeSelectButton.render(spriteBatch);
		}
		#endregion Support methods

		#region Destructor
		public override void dispose() {
			if (this.returnToModeSelectButton != null) {
				this.returnToModeSelectButton.dispose();
			}
			if (this.title != null) {
				this.title.dispose();
			}
			foreach (MapSelection selection in this.mapSelections) {
				selection.dispose();
			}
			if (this.noPreviewImage != null) {
				this.noPreviewImage.dispose();
			}
		}
		#endregion Destructor
	}
}
