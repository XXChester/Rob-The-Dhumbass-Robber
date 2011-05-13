using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robber {
	public class ResourceManager {
		#region Class variables
		// singleton instance variable
		private static ResourceManager instance = new ResourceManager();
		public const int TILE_SIZE = 32;
		public const char SEPARATOR = '|';
		public const string PLAYER_IDENTIFIER = "Player";
		public const string GUARD_IDENTIFIER = "Guard";
		public const string GUARD_ENTRY_IDENTIFIER = "GuardEntry";
		#endregion Class variables

		#region Class properties

		#endregion Class properties

		#region Support methods
		public static ResourceManager getInstance() {
			return instance;
		}
		#endregion Support methods
	}
}
