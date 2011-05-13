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
namespace Robber {
	public class Guard : Person {
		public enum State {
			Chase,
			Patrol,
			Standing,
			NotSpawned
		}
		#region Class variables
		private State currentState;
		private const float MOVEMENT_SPEED_WALK = 75f / 1000f;
		private const float MOVEMENT_SPEED_RUN = 200f / 1000f;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public Guard(ContentManager content, Placement startingLocation, State guardState)
			: base(content, "Guard", startingLocation, MOVEMENT_SPEED_WALK) {
				this.currentState = State.Patrol;
		}
		#endregion Constructor

		#region Support methods
		public override void updateMove() {
			
		}
		#endregion Support methods

		#region Destructor

		#endregion Destructor
	}
}
