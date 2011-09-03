using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace Robber {
	public class CollisionManager {
		#region Class variables
		// singleton instance
		private static CollisionManager instance = new CollisionManager();
		#endregion Class variables

		#region Class properties
		public List<BoundingBox> MapBoundingBoxes{ get; set; }
		#endregion Class properties

		#region Support methods
		public static CollisionManager getInstance() {
			return instance;
		}

		public bool wasCollision(BoundingBox bbox, List<BoundingBox> boxes) {
			bool collision = false;
			foreach (BoundingBox box in boxes) {
				if (box.Intersects(bbox)) {
					collision = true;
					break;
				}
			}
			return collision;
		}

		public bool wallCollisionFound(BoundingBox bbox) {
			return wasCollision(bbox, this.MapBoundingBoxes);
		}
		
		public bool collisionFound(BoundingSphere bphere) {
			bool collision = false;
			foreach (BoundingBox box in this.MapBoundingBoxes) {
				if (box.Intersects(bphere)) {
					collision = true;
					break;
				}
			}
			return collision;
		}
		#endregion Support methods
	}
}
