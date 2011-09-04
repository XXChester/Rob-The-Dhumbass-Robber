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
namespace Robber {
	public class Treasure : Item, IRenderable {
		#region Class variables
		private bool pickedUp;
		#endregion Class variables

		#region Class propeties
		public bool PickedUp {
			get { return this.pickedUp; }
			set {
				this.pickedUp = value;
				if (value) {
					base.BoundingBox = Helper.destroyBB();
				}
			}
		}
		#endregion Class properties

		#region Constructor
		public Treasure(ContentManager content, string textureName, Placement startingPlacement)
			: base(content, textureName, startingPlacement, true) {
				this.pickedUp = false;
		}
		#endregion Constructor

		#region Support methods
		public void update(float elapsed) {
			// do nothing
		}

		public override void render(SpriteBatch spriteBatch) {
			if (!this.pickedUp) {
				base.render(spriteBatch);
			}
		}
		#endregion Support methods

		#region Destructor

		#endregion Destructor
	}
}
