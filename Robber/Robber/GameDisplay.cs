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
namespace Robber {
	public class GameDisplay : Display{
		#region Class variables
		private Map map;
		private Player player;
		private Person[] guards;
		private Treasure[] treasures;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public GameDisplay(ContentManager content, string mapInfomarion) {
			this.map = MapLoader.load(content, mapInfomarion + ".dat", Color.Black);
			
			StreamReader reader = new StreamReader(mapInfomarion + "Locations.dat");
			Point playersLocation = new Point();
			List<Point> guardLocations = new List<Point>();
			List<Point> guardEntryPoints = new List<Point>();
			List<Point> treasureLocations = new List<Point>();
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
						} else if (components[0] == ResourceManager.GUARD_ENTRY_IDENTIFIER) {
							guardEntryPoints.Add(new Point(int.Parse(indexes[0]), int.Parse(indexes[1])));
						} else if (components[0] == ResourceManager.TREASURE_IDENTIFIER) {
							treasureLocations.Add(new Point(int.Parse(indexes[0]), int.Parse(indexes[1])));
						}
					}
				}
			} finally {
				reader.Close();
				reader.Dispose();
			}
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
			for (int i = 0; i < guardSize; i++) {
				this.guards[i] = new Guard(content, new Placement(guardLocations[i]), Guard.State.Standing);
			}
		}
		#endregion Constructor

		#region Support methods
		public override void update(float elapsed) {
			this.map.update(elapsed);
			this.player.update(elapsed);
			foreach (Guard guard in this.guards) {
				guard.update(elapsed);
			}
			foreach (Treasure treasure in this.treasures) {
				if (treasure.BoundingBox.Intersects(this.player.BoundingBox)) {
					treasure.PickedUp = true;
					this.player.CapturedTreasures++;
					break;
				}
			}
		}

		public override void render(SpriteBatch spriteBatch) {
			this.map.render(spriteBatch);
			foreach (Treasure treasure in this.treasures) {
				treasure.render(spriteBatch);
			}
			this.player.render(spriteBatch);
			foreach (Guard guard in this.guards) {
				guard.render(spriteBatch);
			}
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
				foreach (Person guard in this.guards) {
					guard.dispose();
				}
			}
			if (this.treasures != null) {
				foreach (Treasure treasure in this.treasures) {
					treasure.dispose();
				}
			}
		}
		#endregion Destructor
	}
}
