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
		#endregion Class properties

		#region Constructor
		public Map( ContentManager content, Tile[,] tiles, int height, int width) {
			this.tiles = tiles;
			this.HEIGHT = height;
			this.WIDTH = width;
			Texture2D floorTexture = content.Load<Texture2D>("BasicTile");
			this.floor = new Floor(ref HEIGHT, ref WIDTH, floorTexture, Color.White);
			/*this.boundingBoxes.Add(Helper.getBBox(new Vector2(0f, 0f), new Vector2(671f, 0f)));
			this.boundingBoxes.Add(Helper.getBBox(new Vector2(671f, 0f), new Vector2(671f, 575f)));
			this.boundingBoxes.Add(Helper.getBBox(new Vector2(671f,575f), new Vector2(0f, 575f)));
			this.boundingBoxes.Add(Helper.getBBox(new Vector2(0f,575f), new Vector2(0f,0f)));*/
		}
		#endregion Constructor

		#region Support methods
		private MouseState previous;
		public void update(float elapsed) {
#if DEBUG
			if (Mouse.GetState().LeftButton == ButtonState.Pressed && this.previous.LeftButton == ButtonState.Released && Mouse.GetState().Y > 0 && Mouse.GetState().X > 0) {
				//Console.WriteLine(Placement.getIndex(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)));
				string message = "BBox|" + Mouse.GetState().X + "," + Mouse.GetState().Y;
				//string message = "WayPoint|" + Placement.getIndex(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)).X + ","  +
				//	Placement.getIndex(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)).Y;
				//ScriptManager.getInstance().log(message);
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
