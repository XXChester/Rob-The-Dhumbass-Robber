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
using GWNorthEngine.Utils;
using GWNorthEngine.Scripting;
using GWNorthEngine.AI.AStar;
namespace Robber {
	public class Map : IRenderable {
		#region Class variables
		private Tile[,] tiles;
		private Floor floor;
		private readonly int HEIGHT;
		private readonly int WIDTH;
		#endregion Class variables

		#region Class propeties
		public Tile[,] Tiles { get { return this.tiles; } }
		public Color WallColour { get; set; }
		public Color FloorColour { get; set; }
		#endregion Class properties

		#region Constructor
		public Map( ContentManager content, Tile[,] tiles, int height, int width, Color floorColour, Color wallColour) {
			this.FloorColour = floorColour;
			this.WallColour = wallColour;
			this.tiles = tiles;
			this.HEIGHT = height;
			this.WIDTH = width;
			Texture2D floorTexture = content.Load<Texture2D>("BasicTile");
			this.floor = new Floor(ref HEIGHT, ref WIDTH, floorTexture, floorColour);
		}
		#endregion Constructor

		#region Support methods
		public void updateColours(Color floorColour, Color wallColour) {
			foreach (Tile tile in this.tiles) {
				if (tile != null) {
					tile.updateColours(wallColour);
				}
			}
			this.floor.updateColours(floorColour);
		}

		private MouseState previous;
		public void update(float elapsed) {
#if DEBUG
			if (Mouse.GetState().LeftButton == ButtonState.Pressed && this.previous.LeftButton == ButtonState.Released && Mouse.GetState().Y >= 0 && Mouse.GetState().X >= 0) {
				//Console.WriteLine(Placement.getIndex(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)));
				//string message = "BBox|";
				//string suffix = Mouse.GetState().X + "," + Mouse.GetState().Y;
				//string message = "Treasure|";
				//string message = "Guard|";
				//string message = "WayPoint|";
				string message = "GuardEntry|";
				//string message = "Player|";
				string suffix = Placement.getIndex(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)).X + "," + Placement.getIndex(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)).Y;
				Console.WriteLine(suffix);
				//ScriptManager.getInstance().log(message + suffix);
			}
			this.previous = Mouse.GetState();
#endif
		}

		public void render(SpriteBatch spriteBatch) {
			if (this.floor != null) {
				this.floor.render(spriteBatch);
			}
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
			if (this.floor != null) {
				this.floor.dispose();
			}
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
