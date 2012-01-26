using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GWNorthEngine.Utils;
using GWNorthEngine.Input;
namespace Robber {
	public abstract class Display {
		#region Class variables
		protected bool previousMouseOverButton;
		private float currentTransitionTime;
		private const float TRANSITION_TIME = 600f;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public Display() {
			this.previousMouseOverButton = false;
		}
		#endregion Constructor

		#region Support methods
		protected Color fadeOut(Color colour) {
			return TransitionUtils.fadeOut(colour, Display.TRANSITION_TIME, this.currentTransitionTime);
		}

		protected Color fadeIn(Color colour) {
			return TransitionUtils.fadeIn(colour, Display.TRANSITION_TIME, this.currentTransitionTime);
		}

		public static bool resetTransitionTimes() {
			bool reset = false;
			// reset our current transition time in certain scenarios
			if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.None) {
				reset = true;
			} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn &&
				StateManager.getInstance().PreviousTransitionState == StateManager.TransitionState.TransitionOut) {
				// if we went straight from a transition out to transition in we need to reset our transition timer as well
				reset = true;
			}
			return reset;
		}

		public bool transitionTimeElapsed() {
			bool elapsed = false;
			if (this.currentTransitionTime >= Display.TRANSITION_TIME) {
				elapsed = true;
			}
			return elapsed;
		}

		public virtual void update(float elapsed) {
			if (resetTransitionTimes()) {
				this.currentTransitionTime = 0f;
			} else {
				this.currentTransitionTime += elapsed;
			}
			StateManager.getInstance().PreviousTransitionState = StateManager.getInstance().CurrentTransitionState;

			// XBox controller support
			if (StateManager.getInstance().CurrentGameState != StateManager.GameState.Active) {
				Vector2 stickValue;
				if (InputManager.getInstance().isLeftStickMoved(PlayerIndex.One, out stickValue)) {
					int multiplier = 10;
					InputManager.getInstance().updateMousePosition((int)stickValue.X * multiplier, (int)stickValue.Y * multiplier);
				}
			}
		}

		public abstract void render(SpriteBatch spriteBatch);
		#endregion Support methods

		#region Destructor
		public abstract void dispose();
		#endregion Destructor
	}
}
