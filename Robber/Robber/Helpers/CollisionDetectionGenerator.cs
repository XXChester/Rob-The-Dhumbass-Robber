using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
namespace Robber {
	public static class CollisionDetectionGenerator {
		public static List<BoundingBox> generateBoundingBoxesForTexture(Texture2D texture, Placement placement) {
			List<BoundingBox> bboxes = new List<BoundingBox>();
			Vector2 topRight = new Vector2(placement.worldPosition.X + ResourceManager.TILE_SIZE, placement.worldPosition.Y);
			Vector2 bottomRight = new Vector2(placement.worldPosition.X + ResourceManager.TILE_SIZE, placement.worldPosition.Y + ResourceManager.TILE_SIZE);
			Vector2 bottomLeft = new Vector2(placement.worldPosition.X, placement.worldPosition.Y + ResourceManager.TILE_SIZE);
			float thickness = 3f;
			switch (texture.Name) {
				case Tile.TILE_NAME_TOP:
				case Tile.TILE_NAME_TOP_DOOR:
					bboxes.Add(Helper.getBBox(placement.worldPosition, new Vector2(topRight.X, topRight.Y + thickness)));
					break;
				case Tile.TILE_NAME_RIGHT:
				case Tile.TILE_NAME_RIGHT_DOOR:
					bboxes.Add(Helper.getBBox(new Vector2(topRight.X - thickness, topRight.Y), bottomRight));
					break;
				case Tile.TILE_NAME_BOTTOM:
				case Tile.TILE_NAME_BOTTOM_DOOR:
					bboxes.Add(Helper.getBBox(new Vector2(bottomLeft.X, bottomLeft.Y - thickness), bottomRight));
					break;
				case Tile.TILE_NAME_LEFT:
				case Tile.TILE_NAME_LEFT_DOOR:
					bboxes.Add(Helper.getBBox(placement.worldPosition, new Vector2(bottomLeft.X + thickness, bottomLeft.Y)));
					break;
				case Tile.TILE_NAME_TOP_LEFTT:
					bboxes.Add(Helper.getBBox(placement.worldPosition, new Vector2(topRight.X, topRight.Y + thickness)));
					bboxes.Add(Helper.getBBox(placement.worldPosition, new Vector2(bottomLeft.X + thickness, bottomLeft.Y)));
					break;
				case Tile.TILE_NAME_TOP_RIGHT:
					bboxes.Add(Helper.getBBox(placement.worldPosition, new Vector2(topRight.X, topRight.Y + thickness)));
					bboxes.Add(Helper.getBBox(new Vector2(topRight.X - thickness, topRight.Y), bottomRight));
					break;
				case Tile.TILE_NAME_BOTTOM_RIGHT:
					bboxes.Add(Helper.getBBox(new Vector2(bottomLeft.X, bottomLeft.Y - thickness), bottomRight));
					bboxes.Add(Helper.getBBox(new Vector2(topRight.X - thickness, topRight.Y), bottomRight));
					break;
				case Tile.TILE_NAME_BOTTOM_LEFT:
					bboxes.Add(Helper.getBBox(new Vector2(bottomLeft.X, bottomLeft.Y - thickness), bottomRight));
					bboxes.Add(Helper.getBBox(placement.worldPosition, new Vector2(bottomLeft.X + thickness, bottomLeft.Y)));
					break;
			}

			/*int width = texture.Width;
			int height = texture.Height;
			// get the colours from the texture
			Color[] colours = new Color[height * width];
			texture.GetData<Color>(colours);

			// transform to a 2D array so it is easier to use
			Color[,] colours2D = new Color[height, width];
			for (int i = 0; i < colours.Length; i++) {
				colours2D[i / width, i % width] = colours[i];
			}*/

			

			/*BACKUP
			for (int x = 0; x <= colours2D.GetUpperBound(1); x++) {
				pixelColour = colours2D[0, x];
				if (tileFloorColour.Equals(pixelColour) || (x + 1 == ResourceManager.TILE_SIZE)) {
					badPixelStartX = x;
					// now find how deep it goes
					for (int y = 0; y <= colours2D.GetUpperBound(0); y++) {
						pixelColour = colours2D[y, x];
						if (tileFloorColour.Equals(pixelColour)) {
							badPixelStartY = y;
							break;
						}
					}
					break;
				}
			}
			bboxes.Add(Helper.getBBox(placement.worldPosition,  new Vector2(badPixelStartX + placement.worldPosition.X, badPixelStartY + placement.worldPosition.Y)));

			// find our vertical walls
			for (int y = 0; y <= colours2D.GetUpperBound(0); y++) {
				pixelColour = colours2D[y, 0];
				if (tileFloorColour.Equals(pixelColour) || (y + 1 == ResourceManager.TILE_SIZE)) {
					badPixelStartY = y;
					// now find how deep it goes
					for (int x = 0; x <= colours2D.GetUpperBound(1); x++) {
						pixelColour = colours2D[y, x];
						if (tileFloorColour.Equals(pixelColour)) {
							badPixelStartX = x;
							break;
						}
					}
					break;
				}
			}
			bboxes.Add(Helper.getBBox(placement.worldPosition, new Vector2(badPixelStartX + placement.worldPosition.X, badPixelStartY + placement.worldPosition.Y)));*/
			
			return bboxes;
		}
	}
}
