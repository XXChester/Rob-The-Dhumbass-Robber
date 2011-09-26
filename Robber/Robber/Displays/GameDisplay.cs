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
using GWNorthEngine.Input;
namespace Robber {
	public class GameDisplay : Display{
		#region Class variables
		private MapWalls mapWalls;
		private MapFloor mapFloor;
		private Player player;
		private Person[] guards;
		private Treasure[] treasures;
		private Dumpster[] dumpsters;
		private ColouredButton startButton;
		private Timer timer;
		private List<Point> entryExitPoints;
		private Text2D treasureText;
		private StaticDrawable2D treasure;
		private DustParticleEmitter dustEmitter;
		private SoundEffect guardDetectedSfx;
		private SoundEffect introSfx;
		private SoundEffect payDaySfx;
		private SoundEffect treasureSfx;
		private SoundEffect dumpsterCrashSfx;
		private SoundEffect dumpsterCloseSfx;
		private ContentManager content;
#if DEBUG
		private bool showAI = false;
		private bool showCD = false;
		private bool showWayPoints = false;
#endif
		private const string LEVEL_ENTRY_SFX_NAME = "LevelEntry";
		public const float BOARD_OFFSET_Y = 15f;
		#endregion Class variables

		#region Class propeties
		public float Score { 
			get {
				// scores are calculated differently dependent on the game mode
				if (StateManager.getInstance().GameMode == StateManager.Mode.TimeAttack) {
					return (this.player.CapturedTreasures * 1000 + this.timer.Time);
				} else {
					return (this.player.CapturedTreasures * 10 + (int)(this.timer.Time / 4));
				}
			} 
		}
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
			textParms.Position = new Vector2(727f, 150f);
			this.treasureText = new Text2D(textParms);

			StaticDrawable2DParams staticParms = new StaticDrawable2DParams();
			staticParms = new StaticDrawable2DParams();
			staticParms.Position = new Vector2(702f, 148f);
			staticParms.Texture = LoadingUtils.loadTexture2D(content, "Treasure1");
			this.treasure = new StaticDrawable2D(staticParms);

			//dust particle emitter
			BaseParticle2DEmitterParams particleEmitterParams = new BaseParticle2DEmitterParams();
			particleEmitterParams.ParticleTexture = LoadingUtils.loadTexture2D(content, "Dust1");
			particleEmitterParams.SpawnDelay = DustParticleEmitter.SPAWN_DELAY;
			this.dustEmitter = new DustParticleEmitter(particleEmitterParams);

			// load sound effects
			this.introSfx = LoadingUtils.loadSoundEffect(content, LEVEL_ENTRY_SFX_NAME);
			this.payDaySfx = LoadingUtils.loadSoundEffect(content, "PayDay");
			this.treasureSfx = LoadingUtils.loadSoundEffect(content, "TreasureCollect");
			this.guardDetectedSfx = LoadingUtils.loadSoundEffect(content, "GuardDetection");
			this.dumpsterCrashSfx = LoadingUtils.loadSoundEffect(content, "DumpsterCrash");
			this.dumpsterCloseSfx = LoadingUtils.loadSoundEffect(content, "DumpsterClose");
		}
		#endregion Constructor

		#region Support methods
		public void reset() {
			string mapInformation = Directory.GetCurrentDirectory() + "\\" + ResourceManager.MAP_FOLDER + StateManager.getInstance().MapInformation;

			XmlReader xmlReader = XmlReader.Create(mapInformation + "Identifiers.xml");
			this.entryExitPoints = new List<Point>();
			Point playersLocation = new Point();
			Color floorColour = Color.White;
			Color wallColour = Color.Black;
			List<Point> guardLocations = new List<Point>();
			List<string> guardDirectins = new List<string>();
			List<string> guardStates = new List<string>();
			List<Point> treasureLocations = new List<Point>();
			List<Point> dumpsterLocations = new List<Point>();
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

				// load the dumpster information
				MapLoader.loadGenericPointList(doc, MapEditor.MappingState.Dumpster, out dumpsterLocations);

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
			MapLoader.load(content, mapInformation + Constants.FILE_EXTENSION, floorColour, wallColour, out mapWalls, out mapFloor);

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

			// load dumpsters at starting points
			int dumpsterSize = dumpsterLocations.Count;
			this.dumpsters = new Dumpster[dumpsterSize];
			for (int i = 0; i < dumpsterSize; i++) {
				this.dumpsters[i] = new Dumpster(this.content, "DumpsterOpen", "DumpsterClosed", new Placement(dumpsterLocations[i]));
				// ensure these places are unwalkable by the guards
				AIManager.getInstance().Board[dumpsterLocations[i].Y, dumpsterLocations[i].X] = BasePathFinder.TypeOfSpace.Unwalkable;
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
			this.dustEmitter.PlayerPosition = Vector2.Add(this.player.Placement.worldPosition, new Vector2(ResourceManager.TILE_SIZE / 2, 0f));
			StateManager.getInstance().CurrentGameState = StateManager.GameState.Waiting;
		}

		public override void update(float elapsed) {
			if (this.player != null) {
				this.treasureText.WrittenText = " x " + this.player.CapturedTreasures;
			}
			if (this.mapWalls != null) {
				this.mapWalls.update(elapsed);
			}
			this.dustEmitter.update(elapsed);
			Vector2 mousePos = InputManager.getInstance().MousePosition;
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
				if (InputManager.getInstance().wasLeftButtonPressed()) {
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
							SoundManager.getInstance().sfxEngine.playSoundEffect(this.payDaySfx);
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
					if (guard.Ring.BoundingSphere.Intersects(this.player.BoundingBox) && !this.player.Hiding) {
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
						SoundManager.getInstance().sfxEngine.playSoundEffect(this.treasureSfx, .5f);
						break;
					}
				}
				
				// are we trying to enter a dumpster
				if (InputManager.getInstance().wasKeyPressed(Keys.Space)) {
					// check if we are close to a dumpster
					foreach (Dumpster dumpster in dumpsters) {
						if (dumpster.BoundingBox.Intersects(this.player.BoundingBox) && dumpster.AcceptingOccupants) {
							this.player.Hiding = !this.player.Hiding;// reverse the bool
							if (!this.player.Hiding) {
								// make this dumpster unusable again
								dumpster.AcceptingOccupants = false;
								SoundManager.getInstance().sfxEngine.playSoundEffect(this.dumpsterCloseSfx);
							} else {
								SoundManager.getInstance().sfxEngine.playSoundEffect(this.dumpsterCrashSfx);
							}
							break;
						}
					}
				}
			}

			// Transitions
			#region Transitions
			if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
				// start our intro sound effect if we just started to transition
				if (StateManager.getInstance().PreviousGameState == StateManager.GameState.MapSelection &&
					!SoundManager.getInstance().sfxEngine.isPlaying(LEVEL_ENTRY_SFX_NAME)) {
					StateManager.getInstance().CurrentGameState = StateManager.GameState.Waiting;
					SoundManager.getInstance().sfxEngine.playSoundEffect(this.introSfx);
					// initially create some particles
					for (int i = 0; i < 100; i++) {
						this.dustEmitter.createParticle();
					}
				}
				Color colour = base.fadeIn(Color.White);
				this.mapWalls.updateColours(base.fadeIn(this.mapWalls.Colour));
				this.mapFloor.updateColours(base.fadeIn(this.mapFloor.Colour));
				foreach (Treasure treasure in this.treasures) {
					treasure.updateColours(colour);
				}
				foreach (Dumpster dumpster in this.dumpsters) {
					dumpster.updateColours(colour);
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
				this.timer.updateColours(base.fadeIn(ResourceManager.TEXT_COLOUR), base.fadeIn(this.timer.ActiveTimeColour));
			} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
				Color colour = base.fadeOut(Color.White);
				this.mapWalls.updateColours(base.fadeOut(this.mapWalls.Colour));
				this.mapFloor.updateColours(base.fadeOut(this.mapFloor.Colour));
				foreach (Treasure treasure in this.treasures) {
					treasure.updateColours(colour);
				}
				foreach (Dumpster dumpster in this.dumpsters) {
					dumpster.updateColours(colour);
				}
				this.player.updateColours(colour);
				foreach (Guard guard in this.guards) {
					guard.updateColours(colour);
				}
				this.startButton.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR));
				if (this.startButton.isActorOver(mousePos)) {
					this.startButton.updateColours(base.fadeOut(ResourceManager.MOUSE_OVER_COLOUR));
				}

				this.dustEmitter.updateColours(base.fadeOut(DustParticleEmitter.COLOUR));

				// HUD
				this.treasure.LightColour = colour;
				this.treasureText.LightColour = base.fadeOut(ResourceManager.TEXT_COLOUR);
				this.timer.updateColours(base.fadeOut(ResourceManager.TEXT_COLOUR), base.fadeOut(this.timer.ActiveTimeColour));

				// if the alarm is blaring, turn it off
				if (SoundManager.getInstance().sfxEngine.isPlaying(Timer.DETECTED_SFX_NAME)) {
					SoundManager.getInstance().sfxEngine.stop(Timer.DETECTED_SFX_NAME);
				}
			}

			// if our transition is up
			if (base.transitionTimeElapsed()) {
				if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionIn) {
					StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.None;
					if (StateManager.getInstance().PreviousGameState == StateManager.GameState.MapSelection) {
						StateManager.getInstance().CurrentGameState = StateManager.GameState.Waiting;
					}
				} else if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.TransitionOut) {
					StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionIn;
					// clear out the particles
					this.dustEmitter.Particles.Clear();
				}
			}

			if (StateManager.getInstance().CurrentTransitionState == StateManager.TransitionState.None &&
				StateManager.getInstance().CurrentGameState == StateManager.GameState.Waiting ||
				StateManager.getInstance().CurrentGameState == StateManager.GameState.Active) {
				if (InputManager.getInstance().wasKeyPressed(Keys.Escape)) {
					StateManager.getInstance().CurrentGameState = StateManager.GameState.InGameMenu;
					StateManager.getInstance().CurrentTransitionState = StateManager.TransitionState.TransitionOut;
				}
			}
			#endregion Transitions
#if DEBUG
			if (InputManager.getInstance().wasKeyPressed(Keys.D1)) {
				this.showAI = !this.showAI;
			} else if (InputManager.getInstance().wasKeyPressed(Keys.D2)) {
				this.showCD = !this.showCD;
			} else if (InputManager.getInstance().wasKeyPressed(Keys.D3)) {
				this.showWayPoints = !this.showWayPoints;
			} else if (InputManager.getInstance().wasKeyPressed(Keys.R)) {
				reset();
			}
			// map editor
			MapEditor.getInstance().update();
#endif
			base.update(elapsed);
		}

		public override void render(SpriteBatch spriteBatch) {
			// render the floor, treasure, than walls
			this.mapFloor.render(spriteBatch);
			this.treasureText.render(spriteBatch);
			this.treasure.render(spriteBatch);
			foreach (Treasure treasure in this.treasures) {
				treasure.render(spriteBatch);
			}
			this.mapWalls.render(spriteBatch);
			foreach (Dumpster dumpster in this.dumpsters) {
				dumpster.render(spriteBatch);
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
			this.dustEmitter.render(spriteBatch);
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
				foreach (Treasure treasure in this.treasures) {
					DebugUtils.drawBoundingBox(spriteBatch, treasure.BoundingBox, debugColour, ResourceManager.getInstance().ButtonLineTexture);
				}
				foreach (Dumpster dumpster in this.dumpsters) {
					DebugUtils.drawBoundingBox(spriteBatch, dumpster.BoundingBox, debugColour, ResourceManager.getInstance().ButtonLineTexture);
				}
			}
			if (this.showWayPoints) {
				List<Point> wayPoints = AIManager.getInstance().WayPoints;
				foreach (Point wayPoint in wayPoints) {
					spriteBatch.Draw(ResourceManager.getInstance().DebugChip, new Placement(wayPoint).worldPosition, Color.Purple);
				}
			}
			if (this.showAI) {
				// draw where we cannot walk
				for (int y = 0; y < 18; y++) {
					for (int x = 0; x < 21; x++) {
						if (AIManager.getInstance().Board[y, x] == BasePathFinder.TypeOfSpace.Unwalkable) {
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
			if (this.mapWalls != null) {
				this.mapWalls.dispose();
			}
			if (this.mapFloor != null) {
				this.mapFloor.dispose();
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
			if (this.dumpsters != null) {
				foreach (Dumpster dumpster in this.dumpsters) {
					dumpster.dispose();
				}
			}
			if (this.treasure != null) {
				this.treasure.dispose();
			}
			this.timer.dispose();
			this.dustEmitter.dispose();

			// sfxs
			/*
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
			if (this.dumpsterCrashSfx != null) {
				this.dumpsterCrashSfx.Dispose();
			}
			if (this.dumpsterCloseSfx != null) {
				this.dumpsterCloseSfx.Dispose();
			}*/

			// AI
			AIManager.getInstance().dispose();
		}
		#endregion Destructor
	}
}
