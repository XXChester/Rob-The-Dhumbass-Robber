using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace Robber {
	public class CollisionManager {
		// singleton instance
		private static CollisionManager instance = new CollisionManager();

		public List<BoundingBox> MapBoundingBoxes{ get; set; }

		public static CollisionManager getInstance() {
			return instance;
		}

		public bool collisionFound(BoundingBox bbox) {
			bool collision = false;
			foreach (BoundingBox box in this.MapBoundingBoxes) {
				if (box.Intersects(bbox)) {
					collision = true;
					break;
				}
			}
			return collision;
		}
	}
}
