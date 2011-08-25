using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GWNorthEngine.Audio;
using GWNorthEngine.Audio.Params;
namespace Robber {
	public class SoundManager {
		#region Class variables
		// singleton instance
		private static SoundManager instance = new SoundManager();
		#endregion Class variables

		#region Class properties
		public SFXEngine sfxEngine { get; set; }
		#endregion Class properties

		#region Constructor
		public SoundManager() {
			SFXEngineParams parms = new SFXEngineParams();
			parms.Muted = true;
#if !DEBUG
			parms.Muted = false;
#endif
			this.sfxEngine = new SFXEngine(parms);
		}
		#endregion Constructor

		#region Support methods
		public static SoundManager getInstance() {
			return instance;
		}

		public void update() {
			this.sfxEngine.update();
		}
		#endregion Support methods

		#region Destructor
		public void dispose() {
			this.sfxEngine.dispose();
		}
		#endregion Destructor
	}
}
