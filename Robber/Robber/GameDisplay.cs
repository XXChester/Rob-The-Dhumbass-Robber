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
		private Person player;
		private Person[] guards;
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
							playersLocation = new Point(int.Parse(indexes[1]), int.Parse(indexes[0]));
						} else if (components[0] == ResourceManager.GUARD_IDENTIFIER) {
							guardLocations.Add(new Point(int.Parse(indexes[1]), int.Parse(indexes[0])));
						} else if (components[1] == ResourceManager.GUARD_ENTRY_IDENTIFIER) {
							guardEntryPoints.Add(new Point(int.Parse(indexes[1]), int.Parse(indexes[0])));
						}
					}
				}
			} finally {
				reader.Close();
				reader.Dispose();
			}
			this.player = new Player(content, new Placement(playersLocation));
		}
		#endregion Constructor

		#region Support methods
		public override void update(float elapsed) {
			this.map.update(elapsed);
			this.player.update(elapsed);
		}

		public override void render(SpriteBatch spriteBatch) {
			this.map.render(spriteBatch);
			this.player.render(spriteBatch);
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
		}
		#endregion Destructor
	}
}
