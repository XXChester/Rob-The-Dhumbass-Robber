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
using GWNorthEngine.Tools;
using GWNorthEngine.Tools.TilePlacer;

namespace Robber {
	public class MapLoader {
		public static Map load(ContentManager content, string mapName, Color floorcolour, Color wallColour) {
			Map map = null;
			LoadResult loadResult = GWNorthEngine.Tools.TilePlacer.MapLoader.load(content, mapName);
			int height = loadResult.Height;
			int width = loadResult.Width;
			MapTile[,] mapTiles = loadResult.MapTiles;
			Tile[,] tiles = GWNorthEngine.Tools.TilePlacer.MapLoader.initTiles<Tile>(mapTiles, delegate(MapTile tile) {
				return new Tile(tile.Texture, tile.Index, wallColour);
			});
			PathFinder.TypeOfSpace[,] tileSpaces = new PathFinder.TypeOfSpace[height, width];
			TileValues tileValue;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					tileValue = loadResult.TileValues[y, x];
					// override so the AI can walk the outter wall
					if (tiles[y,x] != null && mapAsUnwalkable(tiles[y,x].Texture.Name, y, x, height, width)) {
						tileSpaces[y, x] = Translator.translateTileValueToAStarType(tileValue);
					} else {
						tileSpaces[y, x] = PathFinder.TypeOfSpace.Walkable;
					}
				}
			}

			map = new Map(content, tiles, height, width, floorcolour, wallColour);
			AIManager.getInstane().init(height, width);
			AIManager.getInstane().Board = tileSpaces;
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
