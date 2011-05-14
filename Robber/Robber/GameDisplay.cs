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
using Robber.Interfaces;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
using GWNorthEngine.AI.AStar;
using GWNorthEngine.Scripting;
namespace Robber {
	public class GameDisplay : Display{
		#region Class variables
		private Map map;
		private Player player;
		private Person[] guards;
		private Treasure[] treasures;
		private Color wallColour;
		private ColouredButton replayButton;
		private GraphicsDevice device;
		private ContentManager content;
		private string mapInformation;
		private DebugUtils debugUtils;
		private Texture2D debugChip;
		private Texture2D debugRing;
		#endregion Class variables

		#region Class propeties
		public bool ShowAI { get; set; }
		#endregion Class properties

		#region Constructor
		public GameDisplay(GraphicsDevice device, ContentManager content, string mapInformation) {
			this.device = device;
			this.content = content;
			this.mapInformation = mapInformation;
			CollisionManager.getInstance().MapBoundingBoxes = new List<BoundingBox>();

			// load our map
			this.wallColour = Color.Black;
			this.map = MapLoader.load(content, mapInformation + ".dat", this.wallColour);
			reset(device, content, mapInformation, false);

			// replay button
			ColouredButtonParams buttonParms = new ColouredButtonParams();
			buttonParms.Font = ResourceManager.getInstance().Font;
			buttonParms.Height = 50;
			buttonParms.LinesTexture = ResourceManager.getInstance().ButtonLineTexture;
			buttonParms.MouseOverColour = ResourceManager.MOUSE_OVER_COLOUR;
			buttonParms.RegularColour = ResourceManager.TEXT_COLOUR;
			buttonParms.StartX = 275;
			buttonParms.StartY = 267;
			buttonParms.Text = "Replay";
			buttonParms.TextsPosition = new Vector2(293f, 278f);
			buttonParms.Width = 100;
			this.replayButton = new ColouredButton(buttonParms);
#if WINDOWS
#if DEBUG
			this.debugUtils = new DebugUtils(TextureUtils.create2DColouredTexture(device, 2, 2, Color.White));
			this.debugChip = TextureUtils.create2DColouredTexture(device, 32, 32, Color.White);
			this.debugRing = TextureUtils.create2DRingTexture(device, 16, Color.White);
#endif
#endif
		}
		#endregion Constructor

		#region Support methods
		private void reset(GraphicsDevice device, ContentManager content, string mapInformation, bool cleanUp) {
			if (cleanUp) {
				foreach (Guard guard in this.guards) {
					guard.RunAIThread = false;
				}
			}
			StreamReader reader = new StreamReader(mapInformation + "Locations.dat");
			Point playersLocation = new Point();
			List<Point> guardLocations = new List<Point>();
			List<string> guardDirectins = new List<string>();
			List<string> guardStates = new List<string>();
			List<Point> guardEntryPoints = new List<Point>();
			List<Point> treasureLocations = new List<Point>();
			List<Point> boundingBoxPoints = new List<Point>();
			List<Point> wayPoints = new List<Point>();
			try {
				string[] components = null;
				string[] indexes = null;
				string line = null;
				while (!reader.EndOfStream) {
					line = reader.ReadLine();
					if (!line.StartsWith(@"//")) {
						components = line.Split(ResourceManager.SEPARATOR);
						indexes = components[1].Split(',');
						if (components[0] == ResourceManager.PLAYER_IDENTIFIER) {
							playersLocation = new Point(int.Parse(indexes[0]), int.Parse(indexes[1]));
						} else if (components[0] == ResourceManager.GUARD_IDENTIFIER) {
							guardLocations.Add(new Point(int.Parse(indexes[0]), int.Parse(indexes[1])));
							guardStates.Add(components[2]);
							guardDirectins.Add(components[3]);
						} else if (components[0] == ResourceManager.GUARD_ENTRY_IDENTIFIER) {
							guardEntryPoints.Add(new Point(int.Parse(indexes[0]), int.Parse(indexes[1])));
						} else if (components[0] == ResourceManager.TREASURE_IDENTIFIER) {
							treasureLocations.Add(new Point(int.Parse(indexes[0]), int.Parse(indexes[1])));
						} else if (components[0] == ResourceManager.BOUNDING_BOX_IDENTIFIER) {
							boundingBoxPoints.Add(new Point(int.Parse(indexes[0]), int.Parse(indexes[1])));
						} else if (components[0] == ResourceManager.WAYPOINT_IDENTIFIER) {
							wayPoints.Add(new Point(int.Parse(indexes[0]), int.Parse(indexes[1])));
						}
					}
				}
			} finally {
				reader.Close();
				reader.Dispose();
			}
			// let our AI manager know about the maps way points
			AIManager.getInstane().WayPoints = wayPoints;

			List<BoundingBox> mapBoundingBoxes = new List<BoundingBox>();
			for (int i = 0; i < boundingBoxPoints.Count; i += 2) {
				//mapBoundingBoxes.Add(Helper.getBBox(boundingBoxPoints[i], boundingBoxPoints[i + 1]));

			}
			CollisionManager.getInstance().MapBoundingBoxes.AddRange(mapBoundingBoxes);

			// load player at starting point
			this.player = new Player(content, new Placement(playersLocation));

			// load treasure at starting points
			int treasureSize = treasureLocations.Count;
			this.treasures = new Treasure[treasureSize];
			string treasureFileName;
			Random random = new Random();
			for (int i = 0; i < treasureSize; i++) {
				treasureFileName = "Treasure" + random.Next(1, 3);
				this.treasures[i] = new Treasure(content, treasureFileName, new Placement(treasureLocations[i]));
			}

			// load guard(s) at starting points
			int guardSize = guardLocations.Count;
			this.guards = new Person[guardSize];
			Placement placement;
			for (int i = 0; i < guardSize; i++) {
				placement = new Placement(guardLocations[i]);
				this.guards[i] = new Guard(device, content, placement, guardStates[i], guardDirectins[i]);
			}
		}

		public override void update(float elapsed) {
			base.currentKeyBoardState = Keyboard.GetState();
			base.currentMouseState = Mouse.GetState();
			this.map.update(elapsed);
			if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Active) {
				this.player.update(elapsed);
				foreach (Guard guard in this.guards) {
					guard.update(elapsed);
					if (guard.BoundingSphere.Intersects(this.player.BoundingSphere)) {
						StateManager.getInstance().CurrentGameState = StateManager.GameState.GameOver;
						break;
					}
				}
			} else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.GameOver) {
				Vector2 mousePos = new Vector2(base.currentMouseState.X, base.currentMouseState.Y);
				this.replayButton.processActorsMovement(mousePos);
				if (base.currentMouseState.LeftButton == ButtonState.Pressed && base.prevousMouseState.LeftButton == ButtonState.Released) {
					if (this.replayButton.isActorOver(mousePos)) {
						StateManager.getInstance().CurrentGameState = StateManager.GameState.Active;
						reset(this.device, this.content, this.mapInformation, true);
						Console.WriteLine("Reset");
					}
				}
			}
			foreach (Treasure treasure in this.treasures) {
				if (treasure.BoundingBox.Intersects(this.player.BoundingSphere)) {
					treasure.PickedUp = true;
					this.player.CapturedTreasures++;
					break;
				}
			}
			if (base.currentKeyBoardState.IsKeyDown(Keys.D1) && base.previousKeyBoardState.IsKeyUp(Keys.D1)) {
				if (ShowAI) {
					ShowAI = false;
				} else {
					ShowAI = true;
				}
			}
			base.update(elapsed);
		}

		public override void render(SpriteBatch spriteBatch) {
			this.map.render(spriteBatch);
			foreach (Treasure treasure in this.treasures) {
				treasure.render(spriteBatch);
			}
			if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Active) {
				this.player.render(spriteBatch);
				foreach (Guard guard in this.guards) {
					guard.render(spriteBatch);
				}
			} else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.GameOver) {
				this.replayButton.render(spriteBatch);
			}
#if DEBUG
			if (this.ShowAI) {
				DebugUtils.drawBoundingSphere(spriteBatch, this.player.BoundingSphere, Color.Green, this.debugRing);
				//this.debugUtils.drawBoundingBox(spriteBatch, this.player.BoundingBox, Color.Green);
				foreach (Guard guard in this.guards) {
					//this.debugUtils.drawBoundingBox(spriteBatch, guard.BoundingBox, Color.Green);
					DebugUtils.drawBoundingSphere(spriteBatch, guard.BoundingSphere, Color.Green, this.debugRing);
				}
				/*for (int y = 0; y < 18; y++) {
					for (int x = 0; x < 22; x++) {
						if (AIManager.getInstane().Board[y, x] == PathFinder.TypeOfSpace.Unwalkable) {
							spriteBatch.Draw(this.debugChip, new Placement(new Point(x, y)).worldPosition, Color.Red);
						}
					}
				}*/
			}
#endif
		}
		#endregion Support methods

		#region Destructor
		public override void dispose() {
			if (this.map != null) {
				this.map.dispose();
			}
			if (this.player != null) {
				this.player.dispose();
			}
			if (this.guards != null) {
				foreach (Guard guard in this.guards) {
					guard.dispose();
				}
			}
			if (this.treasures != null) {
				foreach (Treasure treasure in this.treasures) {
					treasure.dispose();
				}
			}
			if (this.replayButton != null) {
				this.replayButton.dispose();
			}
#if DEBUG
			this.debugChip.Dispose();
			this.debugRing.Dispose();
#endif
		}
		#endregion Destructor
	}
}
