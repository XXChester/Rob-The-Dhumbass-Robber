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
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
using GWNorthEngine.AI.AStar;
using GWNorthEngine.Scripting;
using GWNorthEngine.Input;
namespace Robber {
	public class InstructionsMenu : Display {
		#region Class variables
		private ColouredButton returnToMainButton;
		private StaticDrawable2D backGround;
		private StaticDrawable2D title;
		private SoundEffect outroSfx;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public InstructionsMenu(ContentManager content) {
			ColouredButtonParams buttonParms = new ColouredButtonParams();
			buttonParms.Font = ResourceManager.getInstance().Font;
			buttonParms.Height = 25;
			buttonParms.LinesTexture = ResourceManager.getInstance().ButtonLineTexture;
			buttonParms.MouseOverColour = ResourceManager.MOUSE_OVER_COLOUR;
			buttonParms.RegularColour = ResourceManager.TEXT_COLOUR;
			buttonParms.StartX = 560;
			buttonParms.Width = 225;

			// return button
			buttonParms.StartY = 557;
			buttonParms.Text = "Return To Main Menu";
			buttonParms.TextsPosition = new Vector2(570f, buttonParms.StartY - 2);
			this.returnToMainButton = new ColouredButton(buttonParms);

			// title
			StaticDrawable2DParams staticParms = new StaticDrawable2DParams();
			staticParms.Position = new Vector2(0f, 0f);
			staticParms.Texture = LoadingUtils.loadTexture2D(content, "Instructions");
			this.title = new StaticDrawable2D(staticParms);

			// background
			staticParms.Position = new Vector2(80f, 25f);
			staticParms.Texture = LoadingUtils.loadTexture2D(content, "HowToPlay");
			this.backGround = new StaticDrawable2D(staticParms);

			// sound effects
			this.outroSfx = LoadingUtils.loadSoundEffect(content, "WhereWeGonnaRob");
		}
		#endregion Constructor

		#region Support methods
		public override void update(float elapsed) {
			Vector2 mousePos = InputManager.getInstance().MousePosition;

			this.returnToMainButton.processActorsMovement(mousePos);
			// mouse over sfx
			if (this.returnToMainButton.isActorOver(mousePos)) {
				if (!base.previousMouseOverButton) {
						SoundManager.getInstance().sfxEngine.playSoundEffect(ResourceManager.getInstance().MouseOverSfx);
				}
				base.previousMouseOverButton = true;
			} else {
				base.previousMouseOverButton = false;
			}
			if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.None) {
				if (InputManager.getInstance().wasLeftButtonPressed()) {
					if (this.returnToMainButton.isActorOver(mousePos)) {
						StateManager.getInstance().CurrentGameState = StateManager.GameState.MainMenu;
						StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionOut;
					}
				}
			} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
				this.backGround.LightColour = base.fadeIn(Color.White);
				this.title.LightColour = base.fadeIn(Color.White);
				this.returnToMainButton.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));
				if (this.returnToMainButton.isActorOver(mousePos)) {
					this.returnToMainButton.updateColours(base.fadeIn(ResourceManager.MOUSE_OVER_COLOUR));
				}
			} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
				this.backGround.LightColour = base.fadeOut(Color.White);
				this.title.LightColour = base.fadeOut(Color.White);
				this.returnToMainButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
				if (this.returnToMainButton.isActorOver(mousePos)) {
					this.returnToMainButton.updateColours(base.fadeOut(ResourceManager.MOUSE_OVER_COLOUR));
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
				if (InputManager.getInstance().wasKeyPressed(Keys.Escape)) {
					StateManager.getInstance().CurrentGameState = StateManager.getInstance().PreviousGameState;
					StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionOut;
				}
			}
			base.update(elapsed);
		}

		public override void render(SpriteBatch spriteBatch) {
			this.backGround.render(spriteBatch);
			this.title.render(spriteBatch);
			this.returnToMainButton.render(spriteBatch);
		}
		#endregion Support methods

		#region Destructor
		public override void dispose() {
			if (this.returnToMainButton != null) {
				this.returnToMainButton.dispose();
			}
			if (this.backGround != null) {
				this.backGround.dispose();
			}
			if (this.title != null) {
				this.title.dispose();
			}
			if (this.outroSfx != null) {
				this.outroSfx.Dispose();
			}
		}
		#endregion Destructor
	}
}
