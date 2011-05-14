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
