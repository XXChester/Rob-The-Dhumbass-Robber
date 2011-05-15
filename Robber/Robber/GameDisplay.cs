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
		private Timer timer;
		private List<Point> entryExitPoints;
		private Text2D gameOverText;
		private StaticDrawable2D winBackGround;
		private StaticDrawable2D looseBackGround;


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
			this.timer = new Timer();

			// load our map
			CollisionManager.getInstance().MapBoundingBoxes = new List<BoundingBox>();
			this.wallColour = Color.Black;
			this.map = MapLoader.load(content, mapInformation + ".dat", this.wallColour);
			reset(false);

			// replay button
			ColouredButtonParams buttonParms = new ColouredButtonParams();
			buttonParms.Font = ResourceManager.getInstance().Font;
			buttonParms.Height = 25;
			buttonParms.LinesTexture = ResourceManager.getInstance().ButtonLineTexture;
			buttonParms.MouseOverColour = ResourceManager.MOUSE_OVER_COLOUR;
			buttonParms.RegularColour = ResourceManager.TEXT_COLOUR;
			buttonParms.StartX = 693;
			buttonParms.Width = 75;

			// play button
			buttonParms.StartY = 557;
			buttonParms.Text = "Play";
			buttonParms.TextsPosition = new Vector2(710f, buttonParms.StartY - 2f);
			this.replayButton = new ColouredButton(buttonParms);

			// game over text
			Text2DParams textParams = new Text2DParams();
			textParams.Font = ResourceManager.getInstance().Font;
			textParams.Position = new Vector2(200f, 50f);
			this.gameOverText = new Text2D(textParams);

			// background
			StaticDrawable2DParams staticParms = new StaticDrawable2DParams();
			staticParms.Position = new Vector2(-10f, 0f);
			staticParms.Texture = content.Load<Texture2D>("BackGround1");
			this.looseBackGround = new StaticDrawable2D(staticParms);
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
		public void reset(bool cleanUp) {
			if (cleanUp) {
				foreach (Guard guard in this.guards) {
					guard.RunAIThread = false;
				}
			}
			StreamReader reader = new StreamReader(this.mapInformation + "Indentifiers.dat");
			this.entryExitPoints = new List<Point>();
			Point playersLocation = new Point();
			List<Point> guardLocations = new List<Point>();
			List<string> guardDirectins = new List<string>();
			List<string> guardStates = new List<string>();
			List<Point> treasureLocations = new List<Point>();
			List<Point> boundingBoxPoints = new List<Point>();
			List<Point> wayPoints = new List<Point>();
			float time = 0f;
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
							this.entryExitPoints.Add(new Point(int.Parse(indexes[0]), int.Parse(indexes[1])));
						} else if (components[0] == ResourceManager.TREASURE_IDENTIFIER) {
							treasureLocations.Add(new Point(int.Parse(indexes[0]), int.Parse(indexes[1])));
						} else if (components[0] == ResourceManager.BOUNDING_BOX_IDENTIFIER) {
							boundingBoxPoints.Add(new Point(int.Parse(indexes[0]), int.Parse(indexes[1])));
						} else if (components[0] == ResourceManager.WAYPOINT_IDENTIFIER) {
							wayPoints.Add(new Point(int.Parse(indexes[0]), int.Parse(indexes[1])));
						} else if (components[0] == ResourceManager.TIME_INDENTIFIER) {
							time = float.Parse(indexes[0]);
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
				mapBoundingBoxes.Add(Helper.getBBox(boundingBoxPoints[i], boundingBoxPoints[i + 1]));

			}
			CollisionManager.getInstance().MapBoundingBoxes.AddRange(mapBoundingBoxes);

			// load player at starting point
			this.player = new Player(this.content, new Placement(playersLocation));

			// load treasure at starting points
			int treasureSize = treasureLocations.Count;
			this.treasures = new Treasure[treasureSize];
			string treasureFileName;
			Random random = new Random();
			for (int i = 0; i < treasureSize; i++) {
				treasureFileName = "Treasure" + random.Next(1, 3);
				this.treasures[i] = new Treasure(this.content, treasureFileName, new Placement(treasureLocations[i]));
			}

			// load guard(s) at starting points
			int guardSize = guardLocations.Count;
			this.guards = new Person[guardSize];
			Placement placement;
			for (int i = 0; i < guardSize; i++) {
				placement = new Placement(guardLocations[i]);
				this.guards[i] = new Guard(this.device, this.content, placement, guardStates[i], guardDirectins[i]);
			}
			this.timer.reset(time);
		}

		public override void update(float elapsed) {
			base.currentKeyBoardState = Keyboard.GetState();
			base.currentMouseState = Mouse.GetState();
			this.map.update(elapsed);
			// check if the player won
			foreach (Point point in this.entryExitPoints) {
				if (point == this.player.Placement.index) {
					StateManager.getInstance().CurrentGameState = StateManager.GameState.GameOver;
					StateManager.getInstance().TypeOfGameOver = StateManager.GameOverType.Player;
					break;
				}
			}
			if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Active) {
				this.timer.update(elapsed);
				this.player.update(elapsed);
				foreach (Guard guard in this.guards) {
					guard.update(elapsed);
					if (guard.BoundingBox.Intersects(this.player.BoundingBox)) {
						StateManager.getInstance().CurrentGameState = StateManager.GameState.GameOver;
						StateManager.getInstance().TypeOfGameOver = StateManager.GameOverType.Guards;
						break;
					} else if (guard.Ring.BoundingSphere.Intersects(this.player.BoundingBox)) {
						AIManager.getInstane().PlayerDetected = true;
					}
				}
				foreach (Treasure treasure in this.treasures) {
					treasure.update(elapsed);
					if (treasure.BoundingBox.Intersects(this.player.BoundingBox)) {
						treasure.PickedUp = true;
						this.player.CapturedTreasures++;
						break;
					}
				}
			} else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.GameOver) {
				Vector2 mousePos = new Vector2(base.currentMouseState.X, base.currentMouseState.Y);
				this.replayButton.processActorsMovement(mousePos);
				if (base.currentMouseState.LeftButton == ButtonState.Pressed && base.prevousMouseState.LeftButton == ButtonState.Released) {
					if (this.replayButton.isActorOver(mousePos)) {
						StateManager.getInstance().CurrentGameState = StateManager.GameState.Active;
						reset(true);
						Console.WriteLine("Reset");
					}
				}
				if (StateManager.getInstance().TypeOfGameOver == StateManager.GameOverType.Guards) {
					this.gameOverText.WrittenText = "You loose; you have to be quick";
				} else if (StateManager.getInstance().TypeOfGameOver == StateManager.GameOverType.Player) {
					this.gameOverText.WrittenText = "Congratulations you won, you score is " + this.player.CapturedTreasures * 100 + this.timer.Time;
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
			if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Active) {
				this.map.render(spriteBatch);
				foreach (Treasure treasure in this.treasures) {
					treasure.render(spriteBatch);
				}
				this.player.render(spriteBatch);
				foreach (Guard guard in this.guards) {
					guard.render(spriteBatch);
				}
				this.timer.render(spriteBatch);
			} else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.GameOver) {
				if (StateManager.getInstance().TypeOfGameOver == StateManager.GameOverType.Guards) {
					this.looseBackGround.render(spriteBatch);
				} else {
					if (this.winBackGround != null) {
						this.winBackGround.render(spriteBatch);
					}
				}
				this.replayButton.render(spriteBatch);
				this.gameOverText.render(spriteBatch);
			}
#if DEBUG
			if (this.ShowAI) {
				Color debugColour = Color.Green;
				this.debugUtils.drawBoundingBox(spriteBatch, this.player.BoundingBox, debugColour);
				foreach (Guard guard in this.guards) {
					this.debugUtils.drawBoundingBox(spriteBatch, guard.BoundingBox,debugColour);
				}
				foreach (BoundingBox box in CollisionManager.getInstance().MapBoundingBoxes) {
					this.debugUtils.drawBoundingBox(spriteBatch, box, debugColour);
				}
				foreach (BoundingBox box in CollisionManager.getInstance().FloorCenterBoxes) {
					this.debugUtils.drawBoundingBox(spriteBatch, box, debugColour);
				}
				foreach (Treasure treasure in this.treasures) {
					this.debugUtils.drawBoundingBox(spriteBatch, treasure.BoundingBox, debugColour);
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
			if (this.looseBackGround !=null) {
				this.looseBackGround.dispose();
			}
			if (this.winBackGround != null) {
				this.winBackGround.dispose();
			}
#if DEBUG
			this.debugChip.Dispose();
			this.debugRing.Dispose();
#endif
		}
		#endregion Destructor
	}
}
