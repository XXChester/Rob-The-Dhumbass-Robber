using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace Robber {
	public static class Helper {
		public static BoundingBox getBBox(Vector2 pos) {
			return new BoundingBox(new Vector3(pos, 0f), new Vector3(addTileSize(pos), 0f));
		}
		public static Vector2 addTileSize(Vector2 pos) {
			return new Vector2(pos.X + ResourceManager.TILE_SIZE, pos.Y + ResourceManager.TILE_SIZE);
		}
	}
}
