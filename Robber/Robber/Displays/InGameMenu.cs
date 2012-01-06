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
	public class InGameMenu : Display {
		#region Class variables
		private ColouredButton returnToGameButton;
		private ColouredButton exitToMainButton;
		private StaticDrawable2D backGround;
		private StaticDrawable2D title;
		private QuestionMarkEmitter particleEmitter;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public InGameMenu(ContentManager content) {
			ColouredButtonParams buttonParms = new ColouredButtonParams();
			buttonParms.Font = ResourceManager.getInstance().Font;
			buttonParms.Height = 25;
			buttonParms.LinesTexture = ResourceManager.getInstance().ButtonLineTexture;
			buttonParms.MouseOverColour = ResourceManager.MOUSE_OVER_COLOUR;
			buttonParms.RegularColour = ResourceManager.TEXT_COLOUR;
			buttonParms.StartX = 580;
			buttonParms.Width = 205;

			// play button
			buttonParms.StartY = 515;
			buttonParms.Text = "Return to Game";
			buttonParms.TextsPosition = new Vector2(610f, buttonParms.StartY - 2f);
			this.returnToGameButton = new ColouredButton(buttonParms);

			// exit button
			buttonParms.StartY = 557;
			buttonParms.Text = "Exit To Main Menu";
			buttonParms.TextsPosition = new Vector2(590f, buttonParms.StartY - 2);
			this.exitToMainButton = new ColouredButton(buttonParms);

			// title
			StaticDrawable2DParams staticParms = new StaticDrawable2DParams();
			staticParms.Position = new Vector2(0f, 0f);
			staticParms.Texture = LoadingUtils.load<Texture2D>(content, "Paused");
			this.title = new StaticDrawable2D(staticParms);

			// background
			staticParms.Position = new Vector2(-10f, 0f);
			staticParms.Texture = LoadingUtils.load<Texture2D>(content, "InGameMenu");
			this.backGround = new StaticDrawable2D(staticParms);

			// setup the particle emitter
			BaseParticle2DEmitterParams particleEmitterParams = new BaseParticle2DEmitterParams();
			particleEmitterParams.ParticleTexture = LoadingUtils.load<Texture2D>(content, "QuestionParticle");
			particleEmitterParams.SpawnDelay = QuestionMarkEmitter.SPAWN_DELAY;
			this.particleEmitter = new QuestionMarkEmitter(particleEmitterParams);
			
		}
		#endregion Constructor

		#region Support methods
		public override void update(float elapsed) {
			Vector2 mousePos = InputManager.getInstance().MousePosition;

			this.returnToGameButton.processActorsMovement(mousePos);
			this.exitToMainButton.processActorsMovement(mousePos);
			// mouse over sfx
			if (this.returnToGameButton.isActorOver(mousePos) || this.exitToMainButton.isActorOver(mousePos)) {
				if (!base.previousMouseOverButton) {
						SoundManager.getInstance().sfxEngine.playSoundEffect(ResourceManager.getInstance().MouseOverSfx);
				}
				base.previousMouseOverButton = true;
			} else {
				base.previousMouseOverButton = false;
			}
			if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.None) {
				this.particleEmitter.Emitt = true;
				if (InputManager.getInstance().wasLeftButtonPressed()) {
					if (this.returnToGameButton.isActorOver(mousePos)) {
						StateManager.getInstance().CurrentGameState = StateManager.getInstance().PreviousGameState;
						StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionOut;
					} else if (this.exitToMainButton.isActorOver(mousePos)) {
						StateManager.getInstance().CurrentGameState = StateManager.GameState.MainMenu;
						StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionOut;
					}
				}
			} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
				this.particleEmitter.Emitt = false;
				this.backGround.LightColour = base.fadeIn(Color.White);
				this.title.LightColour = base.fadeIn(Color.White);
				this.returnToGameButton.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));
				this.exitToMainButton.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));
				if (this.returnToGameButton.isActorOver(mousePos)) {
						this.returnToGameButton.updateColours(base.fadeIn(ResourceManager.MOUSE_OVER_COLOUR));
				} else if (this.exitToMainButton.isActorOver(mousePos)) {
					this.exitToMainButton.updateColours(base.fadeIn(ResourceManager.MOUSE_OVER_COLOUR));
				}
			} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
				this.particleEmitter.Emitt = false;
				this.backGround.LightColour = base.fadeOut(Color.White);
				this.title.LightColour = base.fadeOut(Color.White);
				this.returnToGameButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
				this.exitToMainButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
				if (this.returnToGameButton.isActorOver(mousePos)) {
					this.returnToGameButton.updateColours(base.fadeOut(ResourceManager.MOUSE_OVER_COLOUR));
				} else if (this.exitToMainButton.isActorOver(mousePos)) {
					this.exitToMainButton.updateColours(base.fadeOut(ResourceManager.MOUSE_OVER_COLOUR));
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
			this.particleEmitter.update(elapsed);
			base.update(elapsed);
		}

		public override void render(SpriteBatch spriteBatch) {
			this.backGround.render(spriteBatch);
			this.title.render(spriteBatch);
			this.particleEmitter.render(spriteBatch);
			this.returnToGameButton.render(spriteBatch);
			this.exitToMainButton.render(spriteBatch);
		}
		#endregion Support methods

		#region Destructor
		public override void dispose() {
			if (this.returnToGameButton != null) {
				this.returnToGameButton.dispose();
			}
			if (this.exitToMainButton != null) {
				this.exitToMainButton.dispose();
			}
			if (this.backGround != null) {
				this.backGround.dispose();
			}
			if (this.title != null) {
				this.title.dispose();
			}
			if (this.particleEmitter != null) {
				this.particleEmitter.dispose();
			}
		}
		#endregion Destructor
	}
}
