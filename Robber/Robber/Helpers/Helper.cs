using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace Robber {
	public static class Helper {
		public static BoundingBox getSmallerBBox(Vector2 pos, float shrinkBy) {
			return getSmallerBBox(pos, shrinkBy, shrinkBy);
		}

		public static BoundingBox getSmallerBBox(Vector2 pos, float shrinkBy, float extraPush) {
			Vector2 min = new Vector2(pos.X - ResourceManager.TILE_SIZE / shrinkBy, pos.Y - ResourceManager.TILE_SIZE / shrinkBy + ResourceManager.TILE_SIZE / extraPush);
			Vector2 max = new Vector2(pos.X + ResourceManager.TILE_SIZE / shrinkBy, pos.Y + ResourceManager.TILE_SIZE / shrinkBy + ResourceManager.TILE_SIZE / extraPush);
			return new BoundingBox(new Vector3(min, 0f), new Vector3(max, 0f));
		}

		public static BoundingBox getPersonBBox(Vector2 pos) {
			return getSmallerBBox(pos, 4.5f, 3.75f);
		}

		public static BoundingBox getTilePaddedBBox(Vector2 pos) {
			return new BoundingBox(new Vector3(pos, 0f), new Vector3(addTileSize(pos), 0f));
		}
		
		public static BoundingSphere getBSphere(Vector2 pos) {
			return new BoundingSphere(new Vector3(pos, 0f), (float)(ResourceManager.TILE_SIZE / 2));
		}

		public static BoundingBox getBBox(Point pos1, Point pos2) {
			Vector2 min = new Vector2(Math.Min(pos1.X, pos2.X), Math.Min(pos1.Y, pos2.Y));
			Vector2 max = new Vector2(Math.Max(pos1.X, pos2.X), Math.Max(pos1.Y, pos2.Y));
			return getBBox(min, max);
		}

		public static BoundingBox getBBox(Vector2 pos1, Vector2 pos2) {
			Vector3 min = new Vector3(Math.Min(pos1.X, pos2.X), Math.Min(pos1.Y, pos2.Y), 0f);
			Vector3 max = new Vector3(Math.Max(pos1.X, pos2.X), Math.Max(pos1.Y, pos2.Y), 0f);
			return new BoundingBox(min, max);
		}

		public static Vector2 addTileSize(Vector2 pos) {
			return new Vector2(pos.X + ResourceManager.TILE_SIZE, pos.Y + ResourceManager.TILE_SIZE);
		}

		public static Point subtractPoint(Point p1, Point p2) {
			return new Point(p1.X - p2.X, p1.Y - p2.Y);
		}

		public static BoundingBox destroyBB() {
			return new BoundingBox(new Vector3(-1), new Vector3(-1));
		}
	}
}
