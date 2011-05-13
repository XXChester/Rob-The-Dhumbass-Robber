using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Robber.Interfaces;
namespace Robber {
	public abstract class Display {
		#region Class variables

		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor

		#endregion Constructor

		#region Support methods
		public abstract void update(float elapsed);
		public abstract void render(SpriteBatch spriteBatch);
		#endregion Support methods

		#region Destructor
		public abstract void dispose();
		#endregion Destructor
	}
}
