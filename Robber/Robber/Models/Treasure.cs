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
namespace Robber {
	public class Treasure : Item, IRenderable {
		#region Class variables

		#endregion Class variables
		public bool PickedUp { get; set; }
		#region Class propeties

		#endregion Class properties

		#region Constructor
		public Treasure(ContentManager content, string textureName, Placement startingPlacement)
			: base(content, textureName, startingPlacement) {
				this.PickedUp = false;
		}
		#endregion Constructor

		#region Support methods
		public void update(float elapsed) {
			if (this.PickedUp) {
				base.BoundingBox = Helper.destroyBB();
			}
		}

		public void render(SpriteBatch spriteBatch) {
			if (!this.PickedUp) {
				base.image.render(spriteBatch);
			}
		}
		#endregion Support methods

		#region Destructor

		#endregion Destructor
	}
}
