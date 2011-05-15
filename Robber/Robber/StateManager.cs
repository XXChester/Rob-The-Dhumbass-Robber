using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robber {
	public class StateManager {
		public enum GameState {
			MainMenu,
			InGameMenu,
			InitGame,
			InitReturnToMain,
			Active,
			GameOver,
			Exit
		}
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
		public GameOverType TypeOfGameOver { get; set; }
		public GameState CurrentGameState {
			get { return this.gameState; }
			set {
				if (value == GameState.Active) {
					this.TypeOfGameOver = GameOverType.None;
				}
				this.gameState = value;
			}
		}
		#endregion Class properties

		#region Constructor
		public StateManager() {
			this.CurrentGameState = GameState.InGameMenu;

		}
		#endregion Constructor

		#region Support methods
		public static StateManager getInstance() {
			return instance;
		}
		#endregion Support methods
	}
}
