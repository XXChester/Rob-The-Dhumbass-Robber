using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
namespace Robber {
	public abstract class Item {
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
			parms.Texture = LoadingUtils.loadTexture2D(content, textureName);
			parms.Origin = new Vector2(ResourceManager.TILE_SIZE / 2f);
			parms.Position = new Vector2(startingPlacement.worldPosition.X + parms.Origin.X, startingPlacement.worldPosition.Y + parms.Origin.Y);
			this.image = new StaticDrawable2D(parms);
			this.placement = startingPlacement;
		}
		#endregion Constructor

		#region Support methods
		public void updateColours(Color colour) {
			this.image.LightColour = colour;
		}

		public virtual void render(SpriteBatch spriteBatch) {
			this.image.render(spriteBatch);
		}
		#endregion Support methods

		#region Destructor
		public virtual void dispose() {
			if (this.image != null) {
				this.image.dispose();
			}
		}
		#endregion Destructor
	}
}
