using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GWNorthEngine.Utils;
using GWNorthEngine.AI.AStar;
namespace Robber {
	public class MapLoader {
		public static Map load(ContentManager content, string mapName, Color floorcolour, Color wallColour) {
			Map map = null;
			StreamReader reader = new StreamReader(mapName);
			try {
				Tile[,] mapPieces = new Tile[0, 0];// have to assign it here to avoid a compilation error below even though it would be assigned
				Tile mapPiece;
				Texture2D texture = null;
				Point pieceIndex;

				PathFinder.TypeOfSpace[,] tileSpaces = null;
				List<Texture2D> textureList = new List<Texture2D>();
				const string TILE_HEADER = "MapTile";
				int height = -1;
				int width = -1;
				int tileNumber;
				int texturesIndex;
				int x, y;
				string textureName;
				string[] components;
				while (!reader.EndOfStream) {
					components = reader.ReadLine().Split(ResourceManager.SEPARATOR);
					if (components.Length == 2) {// first line is the size of the map
						height = (Int32.Parse(components[0]));
						width = (Int32.Parse(components[1]));
						mapPieces = new Tile[height, width];
						// init our AI board
						tileSpaces = new PathFinder.TypeOfSpace[height, width - 1];
						for (int j = 0; j < height; j++) {
							for (int k = 0; k < width - 1; k++) {
								tileSpaces[j, k] = PathFinder.TypeOfSpace.Walkable;
							}
						}
					} else if (components.Length == 3) {// rest of the file are tiles
						texturesIndex = -1;
						tileNumber = -1;
						tileNumber = Int32.Parse((components[0].Replace(TILE_HEADER, "")));
						textureName = components[1];
						// have to remove the file extension
						textureName = textureName.Remove(textureName.IndexOf('.'));
						texture = content.Load<Texture2D>(textureName);
						texturesIndex = textureList.IndexOf(texture);
						if (texturesIndex == -1) {
							textureList.Add(texture);
						} else {
							texture = textureList[texturesIndex];
						}
						if (tileNumber > (width - 1)) {
							x = (tileNumber % width);
							y = (tileNumber / width);
						} else {
							x = tileNumber - 1;
							y = 0;
						}
						pieceIndex = new Point(x, y);
						// override so the AI can walk the outter wall
						if (mapAsUnwalkable(textureName, y,x, height, width - 1)) {
							tileSpaces[y, x] = EnumUtils.numberToEnum<PathFinder.TypeOfSpace>(int.Parse(components[2]));
						}
						mapPiece = new Tile(texture, pieceIndex, wallColour);
						mapPieces[pieceIndex.Y, pieceIndex.X] = mapPiece;
					}
				}
				map = new Map(content, mapPieces, height, width, floorcolour, wallColour);
				AIManager.getInstane().init(height, width - 1);
				AIManager.getInstane().Board = tileSpaces;
			} finally {
				reader.Close();
				reader.Dispose();
			}
			return map;
		}

		private static bool mapAsUnwalkable(string textureName, int y, int x, int height, int width) {
			width -= 1;
			height -= 1;
			bool map = true;
			if (y == 0 || y == height || x == 0 || x == width) {
				if (textureName != "BottomLeft" && textureName != "BottomRight" && textureName != "TopLeft" && textureName != "TopRight") {
					map = false;
				} else {
					// make sure we are not a corner of the board
					if ((y == 0 && x == 0) || (y == height && x == width) || (y == 0 && x == width) || (y == height && x == 0)) {
						map = false;
					}
				}
			}
			return map;
		}
	}
}
