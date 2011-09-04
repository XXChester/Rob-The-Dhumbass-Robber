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
using GWNorthEngine.Utils;
using GWNorthEngine.Scripting;
using GWNorthEngine.AI.AStar;
namespace Robber {
	public class MapWalls : IRenderable {
		#region Class variables
		private Tile[,] tiles;
		#endregion Class variables

		#region Class propeties
		public Tile[,] Tiles { get { return this.tiles; } }
		public Color Colour { get; set; }
		#endregion Class properties

		#region Constructor
		public MapWalls(Tile[,] tiles, Color wallColour) {
			this.Colour = wallColour;
			this.tiles = tiles;
		}
		#endregion Constructor

		#region Support methods
		public void updateColours(Color wallColour) {
			foreach (Tile tile in this.tiles) {
				if (tile != null) {
					if (Tile.COLOUR_OVERRIDE_TILES.Contains<string>(tile.Texture.Name)) {
						tile.updateColours(Color.White);
					} else {
						tile.updateColours(wallColour);
					}
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
