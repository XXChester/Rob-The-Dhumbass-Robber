using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
namespace Robber {
	public class Dumpster : Item, IRenderable {
		#region Class variables
		private StaticDrawable2D closedDumpster;
		private bool acceptingOccupants;
		#endregion Class variables

		#region Class propeties
		public bool AcceptingOccupants {
			get { return this.acceptingOccupants; }
			set {
				this.acceptingOccupants = value;
				if (!value) {
					base.BoundingBox = Helper.destroyBB();
				}
			}
		}
		#endregion Class properties

		#region Constructor
		public Dumpster(ContentManager content, string openDumpsterTextureName, string closedDumpsterTextureName, Placement startingPlacement)
			: base(content, openDumpsterTextureName, startingPlacement) {
			this.acceptingOccupants = true;
			StaticDrawable2DParams parms = new StaticDrawable2DParams();
			parms.Texture = LoadingUtils.loadTexture2D(content, closedDumpsterTextureName);
			parms.Origin = new Vector2(ResourceManager.TILE_SIZE / 2f);
			parms.Position = new Vector2(startingPlacement.worldPosition.X + parms.Origin.X, startingPlacement.worldPosition.Y + parms.Origin.Y);
			this.closedDumpster = new StaticDrawable2D(parms);

			Vector2 bboxPositionMin = startingPlacement.worldPosition;
			Vector2 bboxPositionMax = Vector2.Add(startingPlacement.worldPosition, new Vector2(ResourceManager.TILE_SIZE));
			base.BoundingBox = Helper.getBBox(bboxPositionMin, bboxPositionMax);
		}
		#endregion Constructor

		#region Support methods
		public void update(float elapsed) {
			// do nothing
		}

		public override void render(SpriteBatch spriteBatch) {
			if (this.acceptingOccupants) {
				base.render(spriteBatch);
			} else {
				this.closedDumpster.render(spriteBatch);
			}
		}
		#endregion Support methods

		#region Destructor
		public override void dispose() {
			if (this.closedDumpster != null) {
				this.closedDumpster.dispose();
			}
			base.dispose();
		}
		#endregion Destructor
	}
}
