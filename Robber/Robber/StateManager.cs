using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robber {
	public class StateManager {
		public enum GameState {
			Active,
			GameOver
		}
		#region Class variables
		// singleton instance variable
		private static StateManager instance = new StateManager();
		private GameState gameState;
		#endregion Class variables

		#region Class properties
		public GameState CurrentGameState {
			get { return this.gameState; }
			set {
				this.gameState = value;
			}
		}
		#endregion Class properties

		#region Constructor
		public StateManager() {
			
		}
		#endregion Constructor

		#region Support methods
		public static StateManager getInstance() {
			return instance;
		}
		#endregion Support methods
	}
}
