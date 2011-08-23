using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
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
using GWNorthEngine.Tools.TilePlacer;
namespace Robber {
	public class GameDisplay : Display{
		#region Class variables
		private Map map;
		private Player player;
		private Person[] guards;
		private Treasure[] treasures;
		private ColouredButton startButton;
		private Timer timer;
		private List<Point> entryExitPoints;
		private Text2D treasureText;
		private StaticDrawable2D treasure;
		private SoundEffect cantTouchThisSfx;
		private SoundEffect guardDetectedSfx;
		private SoundEffect introSfx;
		private SoundEffect payDaySfx;
		private SoundEffect treasureSfx;
		private float payDayDelay;
		private ContentManager content;
		private const float DELAY_PAY_DAY_EMOTE = 5000f;
#if DEBUG
		private bool showAI = false;
		private bool showCD = false;
		private bool showWayPoints = false;
#endif
		public const float BOARD_OFFSET_Y = 15f;
		#endregion Class variables

		#region Class propeties
		public float Score { get { return (this.player.CapturedTreasures * 100 + this.timer.Time); } }
		#endregion Class properties

		#region Constructor
		public GameDisplay(ContentManager content) {
			this.content = content;
			this.timer = new Timer(content);

			ColouredButtonParams buttonParms = new ColouredButtonParams();
			buttonParms.Font = ResourceManager.getInstance().Font;
			buttonParms.Height = 25;
			buttonParms.LinesTexture = ResourceManager.getInstance().ButtonLineTexture;
			buttonParms.MouseOverColour = ResourceManager.MOUSE_OVER_COLOUR;
			buttonParms.RegularColour = ResourceManager.TEXT_COLOUR;
			buttonParms.StartX = 700;
			buttonParms.Width = 75;
			buttonParms.StartY = 557;

			// start button
			buttonParms.Text = "Start";
			buttonParms.TextsPosition = new Vector2(711f, buttonParms.StartY - 2f);
			this.startButton = new ColouredButton(buttonParms);

			// HUD
			Text2DParams textParms = new Text2DParams();
			textParms.Font = ResourceManager.getInstance().Font;
			textParms.LightColour = ResourceManager.TEXT_COLOUR;
			textParms.Position = new Vector2(727f, 115f);
			this.treasureText = new Text2D(textParms);

			StaticDrawable2DParams staticParms = new StaticDrawable2DParams();
			staticParms = new StaticDrawable2DParams();
			staticParms.Position = new Vector2(702f, 113f);
			staticParms.Texture = LoadingUtils.loadTexture2D(content, "Treasure1");
			this.treasure = new StaticDrawable2D(staticParms);

			// load sound effects
			this.cantTouchThisSfx = LoadingUtils.loadSoundEffect(content, "CantTouchThis");
			this.introSfx = LoadingUtils.loadSoundEffect(content, "LevelEntry");
			this.payDaySfx = LoadingUtils.loadSoundEffect(content, "PayDay");
			this.treasureSfx = LoadingUtils.loadSoundEffect(content, "TreasureCollect");
			this.guardDetectedSfx = LoadingUtils.loadSoundEffect(content, "Policia");
		}
		#endregion Constructor

		#region Support methods
		public void reset() {
			string mapInformation = Directory.GetCurrentDirectory() + "\\" + ResourceManager.MAP_FOLDER + StateManager.getInstance().MapInformation;
			CollisionManager.getInstance().MapBoundingBoxes = new List<BoundingBox>();

			XmlReader xmlReader = XmlReader.Create(mapInformation + "Identifiers.xml");
			this.entryExitPoints = new List<Point>();
			Point playersLocation = new Point();
			Color floorColour = Color.White;
			Color wallColour = Color.Black;
			List<Point> guardLocations = new List<Point>();
			List<string> guardDirectins = new List<string>();
			List<string> guardStates = new List<string>();
			List<Point> treasureLocations = new List<Point>();
			List<Point> wayPoints = new List<Point>();
			float time = 5f;//default of 5 minutes
			
			try {
				XmlDocument doc = new XmlDocument();
				doc.Load(xmlReader);
				// load the map information
				MapLoader.loadLevelInformation(doc, ref wallColour, ref floorColour, ref time);
				
				// load the player information
				MapLoader.loadPlayerInformation(doc, ref playersLocation);
				
				// load the treasure information
				MapLoader.loadGenericPointList(doc, MapEditor.MappingState.Treasure, out treasureLocations);
				
				// load the entry information
				MapLoader.loadGenericPointList(doc, MapEditor.MappingState.GuardEntry, out entryExitPoints);
				
				// load the guard information
				MapLoader.loadGuardInformation(doc, ref guardLocations, ref guardDirectins, ref guardStates);
				
				// load the waypoint information
				MapLoader.loadGenericPointList(doc, MapEditor.MappingState.WayPoint, out wayPoints);
			} finally {
				xmlReader.Close();
			}
			// load our map
			this.map = MapLoader.load(content, mapInformation + Constants.FILE_EXTENSION, floorColour, wallColour);

			// let our AI manager know about the maps way points
			AIManager.getInstance().WayPoints = wayPoints;

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
				this.guards[i] = new Guard(this.content, placement, guardStates[i], guardDirectins[i]);
			}
			this.timer.reset(time);
			this.treasureText.WrittenText = "x " + this.player.CapturedTreasures;
		}

		public override void update(float elapsed) {
			base.currentKeyBoardState = Keyboard.GetState();
			base.currentMouseState = Mouse.GetState();
			if (this.player != null) {
				this.treasureText.WrittenText = " x " + this.player.CapturedTreasures;
			}
			if (this.map != null) {
				this.map.update(elapsed);
			}
			Vector2 mousePos = new Vector2(base.currentMouseState.X, base.currentMouseState.Y);
			if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Waiting) {
				this.startButton.processActorsMovement(mousePos);
				// mouse over sfx
				if (this.startButton.isActorOver(mousePos)) {
					if (!base.previousMouseOverButton) {
						SoundManager.getInstance().sfxEngine.playSoundEffect(ResourceManager.getInstance().MouseOverSfx);
					}
					base.previousMouseOverButton = true;
				} else {
					base.previousMouseOverButton = false;
				}
				if (base.currentMouseState.LeftButton == ButtonState.Pressed && base.prevousMouseState.LeftButton == ButtonState.Released) {
					if (this.startButton.isActorOver(mousePos)) {
						StateManager.getInstance().CurrentGameState = StateManager.GameState.Active;
					}
				}
			} else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Active) {
				// check if the player won
				foreach (Point point in this.entryExitPoints) {
					if (point == this.player.Placement.index) {
						StateManager.getInstance().CurrentGameState = StateManager.GameState.GameOver;
						if (this.player.CapturedTreasures >= 1) {
							StateManager.getInstance().TypeOfGameOver = StateManager.GameOverType.Player;
							SoundManager.getInstance().sfxEngine.playSoundEffect(this.cantTouchThisSfx);
						} else {
							StateManager.getInstance().TypeOfGameOver = StateManager.GameOverType.None;
						}
						break;
					}
				}
				this.timer.update(elapsed);
				this.player.update(elapsed);
				foreach (Guard guard in this.guards) {
					guard.update(elapsed);
					if (guard.BoundingBox.Intersects(this.player.BoundingBox)) {
						StateManager.getInstance().CurrentGameState = StateManager.GameState.GameOver;
						StateManager.getInstance().TypeOfGameOver = StateManager.GameOverType.Guards;
						SoundManager.getInstance().sfxEngine.playSoundEffect(ResourceManager.getInstance().PrisonCellSfx);
						break;
					} else if (guard.Ring.BoundingSphere.Intersects(this.player.BoundingBox)) {
						// did we JUST get detected?
						if (!AIManager.getInstance().PlayerDetected) {
							SoundManager.getInstance().sfxEngine.playSoundEffect(this.guardDetectedSfx);
						}
						AIManager.getInstance().PlayerDetected = true;
					}
					
				}
				foreach (Treasure treasure in this.treasures) {
					treasure.update(elapsed);
					if (treasure.BoundingBox.Intersects(this.player.BoundingBox)) {
						treasure.PickedUp = true;
						this.player.CapturedTreasures++;
						if (this.payDayDelay >= DELAY_PAY_DAY_EMOTE) {
							SoundManager.getInstance().sfxEngine.playSoundEffect(this.payDaySfx);
							this.payDayDelay = 0f;
						}
						SoundManager.getInstance().sfxEngine.playSoundEffect(this.treasureSfx, .5f);
						break;
					}
				}
			}

			// Transitions
			#region Transitions
			if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
				Color colour = base.fadeIn(Color.White);
				this.map.updateColours(base.fadeIn(this.map.FloorColour), base.fadeIn(this.map.WallColour));
				foreach (Treasure treasure in this.treasures) {
					treasure.updateColours(colour);
				}
				this.player.updateColours(colour);
				foreach (Guard guard in this.guards) {
					guard.updateColours(colour);
				}
				this.startButton.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));
				if (this.startButton.isActorOver(mousePos)) {
					this.startButton.updateColours(base.fadeIn(ResourceManager.MOUSE_OVER_COLOUR));
				}

				// HUD
				this.treasure.LightColour = colour;
				this.treasureText.LightColour = base.fadeIn(ResourceManager.TEXT_COLOUR);
				this.timer.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR));
			} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
				Color colour = base.fadeOut(Color.White);
				this.map.updateColours(base.fadeOut(this.map.FloorColour), base.fadeOut(this.map.WallColour));
				foreach (Treasure treasure in this.treasures) {
					treasure.updateColours(colour);
				}
				this.player.updateColours(colour);
				foreach (Guard guard in this.guards) {
					guard.updateColours(colour);
				}
				this.startButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
				if (this.startButton.isActorOver(mousePos)) {
					this.startButton.updateColours(base.fadeOut(ResourceManager.MOUSE_OVER_COLOUR));
				}

				// HUD
				this.treasure.LightColour = colour;
				this.treasureText.LightColour = base.fadeOut(ResourceManager.TEXT_COLOUR);
				this.timer.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
			}

			// if our transition is up
			if (base.transitionTimeElapsed()) {
				if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
					StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.None;
					if (StateManager.getInstance().PreviousGameState == StateManager.GameState.MapSelection) {
							SoundManager.getInstance().sfxEngine.playSoundEffect(this.introSfx);
					}
					StateManager.getInstance().CurrentGameState = StateManager.GameState.Waiting;
				} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
					StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionIn;
				}
			}

			if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.None &&
				StateManager.getInstance().CurrentGameState == StateManager.GameState.Waiting ||
				StateManager.getInstance().CurrentGameState == StateManager.GameState.Active) {
				if (base.currentKeyBoardState.IsKeyDown(Keys.Escape) && base.previousKeyBoardState.IsKeyUp(Keys.Escape)) {
					StateManager.getInstance().CurrentGameState = StateManager.GameState.InGameMenu;
					StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionOut;
				}
			}
			#endregion Transitions
			this.payDayDelay += elapsed;
#if DEBUG
			if (base.currentKeyBoardState.IsKeyDown(Keys.D1) && base.previousKeyBoardState.IsKeyUp(Keys.D1)) {
				this.showAI = !this.showAI;
			} else if (base.currentKeyBoardState.IsKeyDown(Keys.D2) && base.previousKeyBoardState.IsKeyUp(Keys.D2)) {
				this.showCD = !this.showCD;
			} else if (base.currentKeyBoardState.IsKeyDown(Keys.D3) && base.previousKeyBoardState.IsKeyUp(Keys.D3)) {
				this.showWayPoints = !this.showWayPoints;
			} else if (base.currentKeyBoardState.IsKeyDown(Keys.R) && base.previousKeyBoardState.IsKeyUp(Keys.R)) {
				StateManager.getInstance().CurrentGameState = StateManager.GameState.Waiting;
				reset();
			}
			// map editor
			MapEditor.getInstance().update();
#endif
			base.update(elapsed);
		}

		public override void render(SpriteBatch spriteBatch) {
				this.map.render(spriteBatch);
				this.treasureText.render(spriteBatch);
				this.treasure.render(spriteBatch);
				foreach (Treasure treasure in this.treasures) {
					treasure.render(spriteBatch);
				}
				this.player.render(spriteBatch);
				foreach (Guard guard in this.guards) {
					guard.render(spriteBatch);
				}
				this.timer.render(spriteBatch);
				if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Reset || StateManager.getInstance().CurrentGameState == StateManager.GameState.Waiting || 
					(StateManager.getInstance().CurrentGameState == StateManager.GameState.InGameMenu && StateManager.getInstance().PreviousGameState == StateManager.GameState.Waiting)) {
					this.startButton.render(spriteBatch);
				}
#if DEBUG
			if (this.showCD) {
				Color debugColour = Color.Green;
				DebugUtils.drawBoundingBox(spriteBatch, this.player.BoundingBox, debugColour, ResourceManager.getInstance().ButtonLineTexture);
				foreach (Guard guard in this.guards) {
					DebugUtils.drawBoundingBox(spriteBatch, guard.BoundingBox, debugColour, ResourceManager.getInstance().ButtonLineTexture);
					DebugUtils.drawBoundingSphere(spriteBatch, guard.Ring.BoundingSphere, debugColour, ResourceManager.getInstance().DebugRing);
				}
				foreach (BoundingBox box in CollisionManager.getInstance().MapBoundingBoxes) {
					DebugUtils.drawBoundingBox(spriteBatch, box, debugColour, ResourceManager.getInstance().ButtonLineTexture);
				}
				foreach (BoundingBox box in CollisionManager.getInstance().FloorCenterBoxes) {
					DebugUtils.drawBoundingBox(spriteBatch, box, debugColour, ResourceManager.getInstance().ButtonLineTexture);
				}
				foreach (Treasure treasure in this.treasures) {
					DebugUtils.drawBoundingBox(spriteBatch, treasure.BoundingBox, debugColour, ResourceManager.getInstance().ButtonLineTexture);
				}
			}
			if (this.showWayPoints) {
				List<Point> wayPoints = AIManager.getInstance().WayPoints;
				foreach (Point wayPoint in wayPoints) {
					spriteBatch.Draw(ResourceManager.getInstance().DebugChip, new Placement(wayPoint).worldPosition, Color.Blue);
				}
			}
			if (this.showAI) {
				for (int y = 0; y < 18; y++) {
					for (int x = 0; x < 21; x++) {
						if (AIManager.getInstance().Board[y, x] == PathFinder.TypeOfSpace.Unwalkable) {
							spriteBatch.Draw(ResourceManager.getInstance().DebugChip, new Placement(new Point(x, y)).worldPosition, Color.Red);
						}
					}
				}
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
			if (this.treasure != null) {
				this.treasure.dispose();
			}
			this.timer.dispose();

			// sfxs
			if (this.cantTouchThisSfx != null) {
				this.cantTouchThisSfx.Dispose();
			}
			if (this.introSfx != null) {
				this.introSfx.Dispose();
			}
			if (this.payDaySfx != null) {
				this.payDaySfx.Dispose();
			}
			if (this.treasureSfx != null) {
				this.treasureSfx.Dispose();
			}
			if (this.guardDetectedSfx != null) {
				this.guardDetectedSfx.Dispose();
			}

			// AI
			AIManager.getInstance().dispose();
		}
		#endregion Destructor
	}
}
