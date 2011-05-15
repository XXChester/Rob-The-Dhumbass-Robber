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
	public class Floor : IRenderable {
		#region Class variables
		private Tile[,] tiles;
		public static Color floorColour = Color.White;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public Floor(ref int HEIGHT, ref int WIDTH, Texture2D floorTexture, Color renderColour) {
			int width = WIDTH - 1;//TODO: problem with my map loader, the width is actually shorter by 1 but no time to fix this now
			this.tiles = new Tile[HEIGHT, width];
			CollisionManager.getInstance().FloorCenterBoxes = new List<BoundingBox>();
			floorColour = renderColour;
			//Placement collisionPlacement;
			for (int y = 0; y < HEIGHT; y++) {
				for (int x = 0; x < width; x++) {
					this.tiles[y, x] = new Tile(floorTexture, new Point(x, y), renderColour);
					/*collisionPlacement = new Placement(new Point(x, y));
					collisionPlacement.worldPosition =
						new Vector2(collisionPlacement.worldPosition.X + ResourceManager.TILE_SIZE / 2, collisionPlacement.worldPosition.Y + ResourceManager.TILE_SIZE / 2);
					CollisionManager.getInstance().FloorCenterBoxes.Add(Helper.getFloorBBox(collisionPlacement.worldPosition));*/
				}
			}
		}
		#endregion Constructor

		#region Support methods
		public void updateColours(Color colour) {
			foreach (Tile tile in this.tiles) {
				tile.updateColours(colour);
			}
		}

		public void update(float elapsed) {

		}

		public void render(SpriteBatch spriteBatch) {
			if (this.tiles != null) {
				foreach (Tile tile in this.tiles) {
					tile.render(spriteBatch);
				}
			}
		}
		#endregion Support methods

		#region Destructor
		public void dispose() {
			if (this.tiles != null) {
				foreach (Tile tile in this.tiles) {
					tile.dispose();
				}
			}
		}
		#endregion Destructor
	}
}
