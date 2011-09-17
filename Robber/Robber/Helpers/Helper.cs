using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace Robber {
	public static class Helper {
		public static BoundingBox getSmallerBBox(Vector2 pos, float shrinkBy, float extraPush) {
			Vector2 min = new Vector2(pos.X - ResourceManager.TILE_SIZE / shrinkBy, pos.Y - ResourceManager.TILE_SIZE / shrinkBy + ResourceManager.TILE_SIZE / extraPush);
			Vector2 max = new Vector2(pos.X + ResourceManager.TILE_SIZE / shrinkBy, pos.Y + ResourceManager.TILE_SIZE / shrinkBy + ResourceManager.TILE_SIZE / extraPush);
			return new BoundingBox(new Vector3(min, 0f), new Vector3(max, 0f));
		}

		public static BoundingBox getPersonBBox(Vector2 pos) {
			return getSmallerBBox(pos, 4.5f, 3.75f);
		}

		public static BoundingBox getBBox(Vector2 pos1, Vector2 pos2) {
			Vector3 min = new Vector3(Math.Min(pos1.X, pos2.X), Math.Min(pos1.Y, pos2.Y), 0f);
			Vector3 max = new Vector3(Math.Max(pos1.X, pos2.X), Math.Max(pos1.Y, pos2.Y), 0f);
			return new BoundingBox(min, max);
		}

		public static BoundingBox destroyBB() {
			return new BoundingBox(new Vector3(-100), new Vector3(-100));
		}
	}
}
