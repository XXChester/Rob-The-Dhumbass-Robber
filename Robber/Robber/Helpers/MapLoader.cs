using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;
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

		private static object loadObject<T>(XmlDocument doc, string header, string[] searchStrings) {
			return loadObject<T>(doc, header, searchStrings, 0);
		}

		private static object loadObject<T>(XmlDocument doc, string header, string[] searchStrings, int nodeDepth) {
			T[] result = new T[searchStrings.Length];
			XmlNodeList nodes = doc.GetElementsByTagName(header)[nodeDepth].ChildNodes;
			XmlNode node;
			Type type = typeof(T);
			MethodInfo parseMethod = type.GetMethod("Parse", new Type[] { typeof(string)});
			for (int i = 0; i < nodes.Count; i++) {
				node = nodes[i];
				for (int j = 0; j < searchStrings.Length; j++) {
					if (node.Name == searchStrings[j]) {
						if (type == typeof(string)) {
							result[j] = (T)((object)node.FirstChild.Value);
						} else {
							result[j] = (T)parseMethod.Invoke(type, new object[] { ((string)node.FirstChild.Value) });
						}
					}
				}
			}
			return result;
		}

		private static object loadObject<T>(XmlDocument doc, MapEditor.MappingState header) {
			return loadObject<T>(doc, header.ToString(), new string[] { MapEditor.XML_X, MapEditor.XML_Y });
		}

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
					if (tiles[y, x] != null && mapAsUnwalkable(tiles[y, x].Texture.Name, y, x, height, width)) {
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

		public static void loadGenericPointList(XmlDocument doc, MapEditor.MappingState header, out List<Point> positions) {
			loadGenericPointList(doc, header.ToString(), out positions);
		}

		public static void loadGenericPointList(XmlDocument doc, string header, out List<Point> positions) {
			XmlNodeList nodes = doc.GetElementsByTagName(header);
			positions = new List<Point>();
			string[] xySearch = new string[] { MapEditor.XML_X, MapEditor.XML_Y };
			int[] intResults;
			for (int i = 0; i < nodes.Count; i++) {
				intResults = (int[])loadObject<int>(doc, header, xySearch, i);
				positions.Add(new Point(intResults[0], intResults[1]));
			}

		}
		public static void loadLevelInformation(XmlDocument doc, out Color wallColour, out Color floorColour, out float time) {
			string[] rgb = new string[] { MapEditor.XML_R, MapEditor.XML_G, MapEditor.XML_B };
			int[] result = (int[])loadObject<Int32>(doc, MapEditor.XML_WALL_COLOUR, rgb);
			wallColour = new Color(result[0], result[1], result[2]);

			result = (int[])loadObject<Int32>(doc, MapEditor.XML_FLOOR_COLOUR, rgb);
			floorColour = new Color(result[0], result[1], result[2]);

			time = (float)(((float[])loadObject<float>(doc, MapEditor.XML_TIME, new string[] { MapEditor.XML_MINUTES }))[0]);
		}

		public static void loadPlayerInformation(XmlDocument doc, out Point playerStart) {
			int[] result = (int[])loadObject<Int32>(doc, MapEditor.MappingState.PlayerStart);
			playerStart = new Point(result[0], result[1]);
		}

		public static void loadGuardInformation(XmlDocument doc, out List<Point> guardPositions, out List<string> directions, out List<string> states) {
			XmlNodeList guardNodes = doc.GetElementsByTagName(MapEditor.XML_HEADER_GUARD);
			directions = new List<string>();
			states = new List<string>();
			string[] stateSearch = new string[] { MapEditor.XML_GUARD_STATE };
			string[] directionSearch = new string[] { MapEditor.XML_GUARD_DIRECTION };
			for (int i = 0; i < guardNodes.Count; i++) {
				states.Add(((string[])loadObject<string>(doc, MapEditor.XML_HEADER_GUARD, stateSearch, i))[0]);
				directions.Add(((string[])loadObject<string>(doc, MapEditor.XML_HEADER_GUARD, directionSearch, i))[0]);
			}
			loadGenericPointList(doc, MapEditor.XML_GUARD_POSITION, out guardPositions);

		}
	}
}
