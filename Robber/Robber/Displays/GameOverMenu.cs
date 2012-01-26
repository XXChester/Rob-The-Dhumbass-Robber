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
using GWNorthEngine.Input;
namespace Robber {
	public class GameOverMenu : Display {
		#region Class variables
		private Text2D gameOverText;
		private StaticDrawable2D winBackGround;
		private StaticDrawable2D looseBackGround;
		private ColouredButton replayButton;
		private ColouredButton mapSelectionButton;
		private ScoreCalculationDelegate scoreCalculatorCallBack;
		private ResetBoardDelegate resetCallBack;
		public delegate float ScoreCalculationDelegate();
		public delegate void ResetBoardDelegate();
		#endregion Class variables

		#region Constructor
		public GameOverMenu(ContentManager content, ScoreCalculationDelegate scoreCalculator, ResetBoardDelegate resetCallBack) {
			this.scoreCalculatorCallBack = (ScoreCalculationDelegate) scoreCalculator;
			this.resetCallBack = resetCallBack;
			ColouredButtonParams buttonParms = new ColouredButtonParams();
			buttonParms.Font = ResourceManager.getInstance().Font;
			buttonParms.Height = 25;
			buttonParms.LinesTexture = ResourceManager.getInstance().ButtonLineTexture;
			buttonParms.MouseOverColour = ResourceManager.MOUSE_OVER_COLOUR;
			buttonParms.RegularColour = ResourceManager.TEXT_COLOUR;
			buttonParms.StartX = 580;
			buttonParms.Width = 205;
			buttonParms.StartY = 515;

			// replay button
			buttonParms.Text = "Replay";
			buttonParms.TextsPosition = new Vector2(645f, buttonParms.StartY - 2f);
			this.replayButton = new ColouredButton(buttonParms);

			// Map selection button
			buttonParms.StartY = 557;
			buttonParms.Text = "Map Selection";
			buttonParms.TextsPosition = new Vector2(610f, buttonParms.StartY - 2f);
			this.mapSelectionButton = new ColouredButton(buttonParms);

			// game over text
			Text2DParams textParams = new Text2DParams();
			textParams.Font = ResourceManager.getInstance().Font;
			textParams.Position = new Vector2(200f, 50f);
			this.gameOverText = new Text2D(textParams);

			// loosing background
			StaticDrawable2DParams staticParms = new StaticDrawable2DParams();
			staticParms.Position = new Vector2(-10f, 0f);
			staticParms.Texture = LoadingUtils.load<Texture2D>(content, "BackGround1");
			this.looseBackGround = new StaticDrawable2D(staticParms);

			// winning background
			staticParms.Texture = LoadingUtils.load<Texture2D>(content, "BackGround2");
			this.winBackGround = new StaticDrawable2D(staticParms);
		}
		#endregion Constructor

		#region Support methods
		public override void update(float elapsed) {
			Vector2 mousePos = InputManager.getInstance().MousePosition;
			if (StateManager.getInstance().CurrentGameState == StateManager.GameState.GameOver) {
				this.replayButton.processActorsMovement(mousePos);
				this.mapSelectionButton.processActorsMovement(mousePos);
				// mouse over sfx
				if (this.replayButton.isActorOver(mousePos) || this.mapSelectionButton.isActorOver(mousePos)) {
					if (!base.previousMouseOverButton) {
						SoundManager.getInstance().sfxEngine.playSoundEffect(ResourceManager.getInstance().MouseOverSfx);
					}
					base.previousMouseOverButton = true;
				} else {
					base.previousMouseOverButton = false;
				}
				if (InputManager.getInstance().wasLeftButtonPressed() ||
					InputManager.getInstance().wasButtonPressed(PlayerIndex.One, Buttons.A)) {
					if (this.replayButton.isActorOver(mousePos)) {
						StateManager.getInstance().CurrentGameState = StateManager.GameState.Reset;
						StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionOut;
					} else if (this.mapSelectionButton.isActorOver(mousePos)) {
						StateManager.getInstance().CurrentGameState = StateManager.GameState.MapSelection;
						StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionOut;
					}
				}
				if (StateManager.getInstance().TypeOfGameOver == StateManager.GameOverType.Guards) {
					this.gameOverText.Position = new Vector2(200f, this.gameOverText.Position.Y);
					this.gameOverText.WrittenText = "You loose; you have to be quick";
				} else if (StateManager.getInstance().TypeOfGameOver == StateManager.GameOverType.Player) {
					this.gameOverText.Position = new Vector2(125f, this.gameOverText.Position.Y);
					this.gameOverText.WrittenText = "Congratulations you won, you score is " + this.scoreCalculatorCallBack.Invoke();
				} else if (StateManager.getInstance().TypeOfGameOver == StateManager.GameOverType.None) {
					this.gameOverText.Position = new Vector2(75f, this.gameOverText.Position.Y);
					this.gameOverText.WrittenText = "You loose, you need to capture at least 1 piece of treasure";
				}
			}

			// Transitions
			#region Transitions
			if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
				Color colour = base.fadeIn(Color.White);
				this.looseBackGround.LightColour = colour;
				this.winBackGround.LightColour = colour;
				this.gameOverText.LightColour = colour;
				this.replayButton.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));
				if (this.replayButton.isActorOver(mousePos)) {
					this.replayButton.updateColours(base.fadeIn(ResourceManager.MOUSE_OVER_COLOUR));
				}
				this.mapSelectionButton.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));
				if (this.mapSelectionButton.isActorOver(mousePos)) {
					this.mapSelectionButton.updateColours(base.fadeIn(ResourceManager.MOUSE_OVER_COLOUR));
				}
			} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
				Color colour = base.fadeOut(Color.White);
				this.looseBackGround.LightColour = colour;
				this.winBackGround.LightColour = colour;
				this.gameOverText.LightColour = colour;
				this.replayButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
				if (this.replayButton.isActorOver(mousePos)) {
					this.replayButton.updateColours(base.fadeOut(ResourceManager.MOUSE_OVER_COLOUR));
				}
				this.mapSelectionButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
				if (this.mapSelectionButton.isActorOver(mousePos)) {
					this.mapSelectionButton.updateColours(base.fadeOut(ResourceManager.MOUSE_OVER_COLOUR));
				}
			}

			// if our transition is up
			if (base.transitionTimeElapsed()) {
				if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
					StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.None;
				} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
					StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionIn;
					if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Reset) {
						this.resetCallBack.Invoke();
						Console.WriteLine("Reset");
					}
				}
			}
			#endregion Transitions
			base.update(elapsed);
		}

		public override void render(SpriteBatch spriteBatch) {
			if (StateManager.getInstance().TypeOfGameOver == StateManager.GameOverType.Guards ||
				StateManager.getInstance().TypeOfGameOver == StateManager.GameOverType.None) {
				this.looseBackGround.render(spriteBatch);
			} else {
				if (this.winBackGround != null) {
					this.winBackGround.render(spriteBatch);
				}
			}
			this.replayButton.render(spriteBatch);
			this.mapSelectionButton.render(spriteBatch);
			this.gameOverText.render(spriteBatch);
		}
		#endregion Support methods

		#region Destructor
		public override void dispose() {
			if (this.replayButton != null) {
				this.replayButton.dispose();
			}
			if (this.mapSelectionButton != null) {
				this.mapSelectionButton.dispose();
			}
			if (this.looseBackGround != null) {
				this.looseBackGround.dispose();
			}
			if (this.winBackGround != null) {
				this.winBackGround.dispose();
			}
		}
		#endregion Destructor
	}
}
