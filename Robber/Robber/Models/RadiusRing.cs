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
using GWNorthEngine.Utils;
using GWNorthEngine.AI.AStar;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
namespace Robber {
	public class RadiusRing : IRenderable {
		#region Class variables
		private StaticDrawable2D ring;
		private readonly Vector2 SCALE = new Vector2(7f, 7f);
		private readonly Color COLOUR = Color.Red;
		#endregion Class variables

		#region Class propeties
		public BoundingSphere BoundingSphere { get; set; }

		#endregion Class properties

		#region Constructor
		public RadiusRing(ContentManager content, Vector2 position) {
			StaticDrawable2DParams ringParams = new StaticDrawable2DParams();
			ringParams.Position = position;
			ringParams.Texture = content.Load<Texture2D>("RadiusRing");
			ringParams.Scale = SCALE;
			ringParams.LightColour = COLOUR;
			ringParams.Origin = new Vector2(ResourceManager.TILE_SIZE / 2f, ResourceManager.TILE_SIZE / 2f);
			this.ring = new StaticDrawable2D(ringParams);
			updateBoundingSphere(position);
		}
		#endregion Constructor

		#region Support methods
		private void updateBoundingSphere(Vector2 position) {
			this.BoundingSphere = new BoundingSphere(new Vector3(position, 0f), ResourceManager.TILE_SIZE * 3.5f);
		}
		
		public void updatePosition(Vector2 newPosition) {
			this.ring.Position = newPosition;
			updateBoundingSphere(newPosition);
		}

		public void update(float elapsed) {
		}

		public void render(SpriteBatch spriteBatch) {
			if (this.ring != null && StateManager.getInstance().CurrentGameState == StateManager.GameState.Active) {
				this.ring.render(spriteBatch);
			}
		}
		#endregion Support methods

		#region Destructor
		public void dispose() {
			if (this.ring != null) {
				this.ring.dispose();
			}
		}
		#endregion Destructor
	}
}
