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
namespace Robber {
	public class MapFloor : IRenderable {
		#region Class variables
		private Tile[,] tiles;
		#endregion Class variables

		#region Class propeties
		public Color Colour { get; set; }
		#endregion Class properties

		#region Constructor
		public MapFloor(Tile[,] tiles, Color renderColour) {
			this.tiles = tiles;
			this.Colour = renderColour;
		}
		#endregion Constructor

		#region Support methods
		public void updateColours(Color colour) {
			foreach (Tile tile in this.tiles) {
				if (tile != null) {
					tile.updateColours(colour);
				}
			}
		}

		public void update(float elapsed) {

		}

		public void render(SpriteBatch spriteBatch) {
			if (this.tiles != null) {
				foreach (Tile tile in this.tiles) {
					if (tile != null) {
						tile.render(spriteBatch);
					}
				}
			}
		}
		#endregion Support methods

		#region Destructor
		public void dispose() {
			if (this.tiles != null) {
				foreach (Tile tile in this.tiles) {
					if (tile != null) {
						tile.dispose();
					}
				}
			}
		}
		#endregion Destructor
	}
}
