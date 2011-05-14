using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Robber.Interfaces;
namespace Robber {
	public abstract class Display {
		#region Class variables
		protected KeyboardState currentKeyBoardState;
		protected KeyboardState previousKeyBoardState;
		protected MouseState currentMouseState;
		protected MouseState prevousMouseState;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor

		#endregion Constructor

		#region Support methods
		public virtual void update(float elapsed) {
			this.prevousMouseState = Mouse.GetState();
			this.previousKeyBoardState = this.currentKeyBoardState;
		}

		public abstract void render(SpriteBatch spriteBatch);
		#endregion Support methods

		#region Destructor
		public abstract void dispose();
		#endregion Destructor
	}
}
