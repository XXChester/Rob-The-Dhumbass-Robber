using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
using GWNorthEngine.AI.AStar;
using GWNorthEngine.Scripting;
using GWNorthEngine.Input;
namespace Robber {
	public class MainMenu : Display {
		#region Class variables
		private ColouredButton playButton;
		private ColouredButton instructionsButton;
		private ColouredButton exitButton;
		private StaticDrawable2D backGround;
		private StaticDrawable2D title;
		private SoundEffect introSfx;
		private SoundEffect[] idleSfxs;
		private SoundEffect outroSfx;
		private float timeIdle;
		private int idleLastPlayed;
		private const float PLAY_IDLE_AT = 20000f;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public MainMenu(ContentManager content) {
			ColouredButtonParams buttonParms = new ColouredButtonParams();
			buttonParms.Font = ResourceManager.getInstance().Font;
			buttonParms.Height = 25;
			buttonParms.LinesTexture = ResourceManager.getInstance().ButtonLineTexture;
			buttonParms.MouseOverColour = ResourceManager.MOUSE_OVER_COLOUR;
			buttonParms.RegularColour = ResourceManager.TEXT_COLOUR;
			buttonParms.StartX = 640;
			buttonParms.Width = 150;

			// play button
			buttonParms.StartY = 473;
			buttonParms.Text = "Play";
			buttonParms.TextsPosition = new Vector2(690f, buttonParms.StartY - 2f);
			this.playButton = new ColouredButton(buttonParms);

			// instructions button
			buttonParms.StartY = 515;
			buttonParms.Text = "Instructions";
			buttonParms.TextsPosition = new Vector2(650f, buttonParms.StartY - 2f);
			this.instructionsButton = new ColouredButton(buttonParms);

			// exit button
			buttonParms.StartY = 557;
			buttonParms.Text = "Exit";
			buttonParms.TextsPosition = new Vector2(690f, buttonParms.StartY - 2);
			this.exitButton = new ColouredButton(buttonParms);
			
			// title
			StaticDrawable2DParams staticParms = new StaticDrawable2DParams();
			staticParms.Position = new Vector2(0f, -20f);
			staticParms.Texture = LoadingUtils.loadTexture2D(content, "Title");
			this.title = new StaticDrawable2D(staticParms);

			// background
			staticParms.Position = new Vector2(0f, 0f);
			staticParms.Texture = LoadingUtils.loadTexture2D(content, "MainMenu");
			this.backGround = new StaticDrawable2D(staticParms);

			// load sound effects
			this.introSfx = LoadingUtils.loadSoundEffect(content, "Introduction");
			this.outroSfx = LoadingUtils.loadSoundEffect(content, "LetsGo");
			this.idleSfxs = new SoundEffect[3];
			this.idleSfxs[0] = LoadingUtils.loadSoundEffect(content, "Rules");
			this.idleSfxs[1] = LoadingUtils.loadSoundEffect(content, "HaventGotAllDay");
			this.idleSfxs[2] = LoadingUtils.loadSoundEffect(content, "LetsRobSomething");
			// tired of hearing this when debugging and not starting in this state
			if (StateManager.getInstance().CurrentGameState == StateManager.GameState.MainMenu) {
				SoundManager.getInstance().sfxEngine.playSoundEffect(this.introSfx);
			}
#if WINDOWS
#if DEBUG
			ScriptManager.getInstance().registerObject(((ColouredButton)this.playButton).Text, "playText");
#endif
#endif
		}
		#endregion Constructor

		#region Support methods
		public override void update(float elapsed) {
			Vector2 mousePos = InputManager.getInstance().MousePosition;

			this.playButton.processActorsMovement(mousePos);
			this.instructionsButton.processActorsMovement(mousePos);
			this.exitButton.processActorsMovement(mousePos);
			// mouse over sfx
			if (this.playButton.isActorOver(mousePos) || this.exitButton.isActorOver(mousePos)) {
				if (!base.previousMouseOverButton) {
					SoundManager.getInstance().sfxEngine.playSoundEffect(ResourceManager.getInstance().MouseOverSfx);
				}
				base.previousMouseOverButton = true;
			} else {
				base.previousMouseOverButton = false;
			}
			if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.None) {
				if (InputManager.getInstance().wasLeftButtonPressed()) {
					if (this.playButton.isActorOver(mousePos)) {
						StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionOut;
						StateManager.getInstance().CurrentGameState = StateManager.GameState.ModeSelect;
						SoundManager.getInstance().sfxEngine.playSoundEffect(this.outroSfx);
					} else if (this.instructionsButton.isActorOver(mousePos)) {
						StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionOut;
						StateManager.getInstance().CurrentGameState = StateManager.GameState.Instructions;
					} else if (this.exitButton.isActorOver(mousePos)) {
						StateManager.getInstance().CurrentGameState = StateManager.GameState.Exit;
					}
				}
			} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
				if (this.playButton.isActorOver(mousePos)) {
					this.playButton.updateColours(base.fadeIn(ResourceManager.MOUSE_OVER_COLOUR));
					this.instructionsButton.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));
					this.exitButton.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));
				} else if (this.exitButton.isActorOver(mousePos)) {
					this.exitButton.updateColours(base.fadeIn(ResourceManager.MOUSE_OVER_COLOUR));
					this.instructionsButton.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));
					this.playButton.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));
				} else if (this.instructionsButton.isActorOver(mousePos)) {
					this.instructionsButton.updateColours(base.fadeIn(ResourceManager.MOUSE_OVER_COLOUR));
					this.playButton.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));
					this.exitButton.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));
				} else {
					this.playButton.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));
					this.exitButton.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));
					this.instructionsButton.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));
				}
				this.backGround.LightColour = base.fadeIn(Color.White);
				this.title.LightColour = base.fadeIn(Color.White);
			} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
				if (this.playButton.isActorOver(mousePos)) {
					this.playButton.updateColours(base.fadeOut(ResourceManager.MOUSE_OVER_COLOUR));
					this.exitButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
					this.instructionsButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
				} else if (this.exitButton.isActorOver(mousePos)) {
					this.exitButton.updateColours(base.fadeOut(ResourceManager.MOUSE_OVER_COLOUR));
					this.playButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
					this.instructionsButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
				} else if (this.instructionsButton.isActorOver(mousePos)) {
					this.playButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
					this.exitButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
					this.instructionsButton.updateColours(base.fadeOut(ResourceManager.MOUSE_OVER_COLOUR));
				} else {
					this.playButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
					this.exitButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
					this.instructionsButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
				}
				this.backGround.LightColour = base.fadeOut(Color.White);
				this.title.LightColour = base.fadeOut(Color.White);
				//reset the idle timer
				this.timeIdle = 0f;
			}
			// if our transition time is up change our state
			if (base.transitionTimeElapsed()) {
				if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
					StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.None;
				} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
					StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionIn;
				}
			}

			this.timeIdle += elapsed;
			if (this.timeIdle >= PLAY_IDLE_AT) {
				Random rand = new Random();
				int idleToPlay = this.idleLastPlayed;
				do {
					idleToPlay = rand.Next(this.idleSfxs.Length);
				} while (idleToPlay == this.idleLastPlayed);
				SoundManager.getInstance().sfxEngine.playSoundEffect(this.idleSfxs[idleToPlay]);
				this.idleLastPlayed = idleToPlay;
				this.timeIdle = 0f;
			}
			base.update(elapsed);
		}

		public override void render(SpriteBatch spriteBatch) {
			this.backGround.render(spriteBatch);
			this.title.render(spriteBatch);
			this.playButton.render(spriteBatch);
			this.instructionsButton.render(spriteBatch);
			this.exitButton.render(spriteBatch);
		}
		#endregion Support methods

		#region Destructor
		public override void dispose() {
			if (this.playButton != null) {
				this.playButton.dispose();
			}
			if (this.instructionsButton != null) {
				this.instructionsButton.dispose();
			}
			if (this.exitButton != null) {
				this.exitButton.dispose();
			}
			if (this.backGround != null) {
				this.backGround.dispose();
			}
			if (this.title != null) {
				this.title.dispose();
			}
			
			//sfxs
			/*if (this.introSfx != null) {
				this.introSfx.Dispose();
			}
			if (this.outroSfx != null) {
				this.outroSfx.Dispose();
			}
			if (this.idleSfxs != null) {
				foreach (SoundEffect sfx in this.idleSfxs) {
					sfx.Dispose();
				}
			}*/
		}
		#endregion Destructor
	}
}
