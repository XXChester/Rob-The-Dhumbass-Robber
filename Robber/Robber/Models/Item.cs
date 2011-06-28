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
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
namespace Robber {
	public class Item {
		#region Class variables
		protected StaticDrawable2D image;
		private Placement placement;
		#endregion Class variables

		#region Class propeties
		public BoundingBox BoundingBox { get; set; }
		#endregion Class properties

		#region Constructor
		public Item(ContentManager content, string textureName, Placement startingPlacement) {
			StaticDrawable2DParams parms = new StaticDrawable2DParams();
			parms.Texture = content.Load<Texture2D>(textureName);
			parms.Position = startingPlacement.worldPosition;
			this.image = new StaticDrawable2D(parms);
			this.placement = startingPlacement;
			this.BoundingBox = Helper.getTilePaddedBBox(this.placement.worldPosition);
		}
		#endregion Constructor

		#region Support methods
		public void updateColours(Color colour) {
			this.image.LightColour = colour;
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
