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
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
using GWNorthEngine.AI.AStar;
using GWNorthEngine.Scripting;
namespace Robber {
	public class MapSelection : IRenderable {
		#region Class variables
		private StaticDrawable2D previewImage;
		#endregion Class variables

		#region Class propeties
		public ColouredButton PreviewButton { get; set; }
		public string MapName { get; set; }
		#endregion Class properties

		#region Constructor
		public MapSelection(ContentManager content, string mapName, int index, int width, int height, int startX, int startY) {
			this.MapName = mapName;
			ColouredButtonParams buttonParms = new ColouredButtonParams();
			buttonParms.Font = ResourceManager.getInstance().Font;
			buttonParms.Height = height;
			buttonParms.LinesTexture = ResourceManager.getInstance().ButtonLineTexture;
			buttonParms.MouseOverColour = ResourceManager.MOUSE_OVER_COLOUR;
			buttonParms.RegularColour = ResourceManager.TEXT_COLOUR;
			buttonParms.StartX = startX;
			buttonParms.Width = width;
			buttonParms.StartY = startY - ((index + 1) * height + (17 * (index + 1)));
			buttonParms.Text = mapName;
			buttonParms.TextsPosition = new Vector2(startX + (width / 2 - 30), buttonParms.StartY - 2f);
			this.PreviewButton = new ColouredButton(buttonParms);

			// title
			StaticDrawable2DParams staticParms = new StaticDrawable2DParams();
			staticParms.Position = new Vector2(20f, 100f);
			staticParms.Texture = LoadingUtils.loadTexture2D(content,  mapName + "preview");
			staticParms.Scale = new Vector2(.8f, .8f);
			this.previewImage = new StaticDrawable2D(staticParms);
		}
		#endregion Constructor

		#region Support methods
		public void updateColours(Color buttonColour, Color previewColour) {
			this.PreviewButton.updateColours(buttonColour);
			this.previewImage.LightColour = previewColour;
		}

		public void update(float elapsed) {
			this.PreviewButton.processActorsMovement(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
		}

		public void render(SpriteBatch spriteBatch) {
			if (this.PreviewButton.isActorOver(new Vector2(Mouse.GetState().X, Mouse.GetState().Y))) {
				this.previewImage.render(spriteBatch);
			}
			this.PreviewButton.render(spriteBatch);
		}
		#endregion Support methods

		#region Destructor
		public void dispose() {
			if (this.PreviewButton != null) {
				this.PreviewButton.dispose();
			}
			if (this.previewImage != null) {
				this.previewImage.dispose();
			}
		}
		#endregion Destructor
	}
}
