using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robber {
	public class StateManager {
		public enum GameState {
			MainMenu,
			ModeSelect,
			InGameMenu,
			InitGame,
			Reset,
			Waiting,
			Active,
			GameOver,
			MapSelection,
			Instructions,
			Exit
		}

		public enum Mode {
			Normal,
			TimeAttack
		}

		public enum TransitionState {
			None,
			TransitionIn,
			TransitionOut,
		};

		public enum GameOverType {
			None,
			Player,
			Guards
		}
		#region Class variables
		// singleton instance variable
		private static StateManager instance = new StateManager();
		private GameState gameState;
		#endregion Class variables

		#region Class properties
		public string MapInformation { get; set; }
		public GameOverType TypeOfGameOver { get; set; }
		public Mode GameMode { get; set; }
		public TransitionState CurrentTransitionState { get; set; }
		public TransitionState PreviousTransitionState { get; set; }
		public GameState PreviousGameState { get; set; }
		public GameState CurrentGameState {
			get { return this.gameState; }
			set {
				this.PreviousGameState = this.CurrentGameState;

				if (value == GameState.Active) {
					this.TypeOfGameOver = GameOverType.None;
				} else if (value == GameState.GameOver) {
					this.CurrentTransitionState = TransitionState.TransitionOut;
				}
				this.gameState = value;
			}
		}
		#endregion Class properties

		#region Constructor
		public StateManager() {
			// defaults
			this.CurrentTransitionState = TransitionState.TransitionIn;
			this.CurrentGameState = GameState.MainMenu;
			this.GameMode = Mode.Normal;

			// test settings
			//this.CurrentGameState = GameState.Instructions;
			//this.CurrentGameState = GameState.ModeSelect;
			//this.CurrentGameState = GameState.InGameMenu;
			//this.CurrentGameState = GameState.GameOver;
			//this.CurrentGameState = GameState.MapSelection;
			//this.GameMode = Mode.TimeAttack;
			//this.CurrentGameState = GameState.InitGame;
			//this.MapInformation = "Map10";
		}
		#endregion Constructor

		#region Support methods
		public static StateManager getInstance() {
			return instance;
		}
		#endregion Support methods
	}
}
