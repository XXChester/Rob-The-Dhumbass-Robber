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
using GWNorthEngine.AI.AStar;
using GWNorthEngine.Tools.TilePlacer;
namespace Robber {
	public class Tile : MapTile, IRenderable {
		#region Class variables
		private StaticDrawable2D image;
		public const string TILE_NAME_TOP = "TopWall";
		public const string TILE_NAME_TOP_DOOR = "TopDoor";
		public const string TILE_NAME_RIGHT = "RightWall";
		public const string TILE_NAME_RIGHT_DOOR = "RightDoor";
		public const string TILE_NAME_BOTTOM = "BottomWall";
		public const string TILE_NAME_BOTTOM_DOOR = "BottomDoor";
		public const string TILE_NAME_LEFT = "LeftWall";
		public const string TILE_NAME_LEFT_DOOR = "LeftDoor";
		public const string TILE_NAME_TOP_LEFTT = "TopLeft";
		public const string TILE_NAME_TOP_RIGHT = "TopRight";
		public const string TILE_NAME_BOTTOM_RIGHT = "BottomRight";
		public const string TILE_NAME_BOTTOM_LEFT = "BottomLeft";
		public const string TILE_NAME_EXIT = "ExitTile";

		public static string[] COLOUR_OVERRIDE_TILES = new string[] {
			TILE_NAME_LEFT_DOOR, TILE_NAME_RIGHT_DOOR, TILE_NAME_BOTTOM_DOOR, TILE_NAME_TOP_DOOR, TILE_NAME_EXIT
		};
		#endregion Class variables

		#region Class propeties
		#endregion Class properties

		#region Constructor
		public Tile(Texture2D texture, Point index, Color renderColour)
		:base(index, texture) {
			StaticDrawable2DParams parms = new StaticDrawable2DParams();
			parms.Position = new Vector2(base.WorldPosition.X, base.WorldPosition.Y  + GameDisplay.BOARD_OFFSET_Y);
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
		public override void dispose() {
			if (this.image != null) {
				this.image.dispose();
			}
			base.dispose();
		}
		#endregion Destructor
	}
}
