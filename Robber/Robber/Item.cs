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
using Robber.Interfaces;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
namespace Robber {
	public class Item : IRenderable {
		#region Class variables
		private StaticDrawable2D image;
		private Placement placement;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public Item() {

		}
		#endregion Constructor

		#region Support methods
		public void update(float elapsed) {

		}

		public void render(SpriteBatch spriteBatch) {

		}
		#endregion Support methods

		#region Destructor
		public void dispose() {
			if (this.image != null) {
				this.image.dispose();
			}
		}
		#endregion Destructor
	}
}
