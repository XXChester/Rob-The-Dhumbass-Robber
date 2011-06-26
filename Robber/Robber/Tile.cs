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
using GWNorthEngine.AI.AStar;
using GWNorthEngine.Tools.TilePlacer;
namespace Robber {
	public class Tile : MapTile, IRenderable {
		#region Class variables
		private StaticDrawable2D image;
		#endregion Class variables

		#region Class propeties
		/*public BoundingBox[] BoundingBoxes { get; set; }
		public Vector2 TopLimitation { get; set; }
		public Vector2 RightLimitation { get; set; }
		public Vector2 BottomLimitation { get; set; }
		public Vector2 LeftLimitation { get; set; }*/
		#endregion Class properties

		#region Constructor
		public Tile(Texture2D texture, Point index, Color renderColour)
		:base(index, texture) {
			StaticDrawable2DParams parms = new StaticDrawable2DParams();
			parms.Position = base.WorldPosition;
			parms.Texture = texture;
			parms.LightColour = renderColour;
			this.image = new StaticDrawable2D(parms);
		}
		#endregion Constructor

		#region Support methods
		public void updateColours(Color colour) {
			this.image.LightColour = colour;
		}

		public void update(float elapsed) {

		}

		public void render(SpriteBatch spriteBatch) {
			if (this.image != null) {
				this.image.render(spriteBatch);
			}
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
