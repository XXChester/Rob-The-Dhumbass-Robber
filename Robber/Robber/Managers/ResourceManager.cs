using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using GWNorthEngine.Utils;
namespace Robber {
	public class ResourceManager {
		#region Class variables
		// singleton instance variable
		private static ResourceManager instance = new ResourceManager();
		public const int TILE_SIZE = 32;
		public const char SEPARATOR = '|';
		public const string PLAYER_IDENTIFIER = "Player";
		public const string GUARD_IDENTIFIER = "Guard";
		public const string GUARD_ENTRY_IDENTIFIER = "GuardEntry";
		public const string TREASURE_IDENTIFIER = "Treasure";
		public const string BOUNDING_BOX_IDENTIFIER = "BBox";
		public const string WAYPOINT_IDENTIFIER = "WayPoint";
		public const string TIME_INDENTIFIER = "Time";
		public const string COLOUR_IDENTIFIER = "Colours";
		public static readonly Color TEXT_COLOUR = Color.Gray;
		public static readonly Color MOUSE_OVER_COLOUR = Color.White;
		public const bool PLAY_SOUND = false;
		#endregion Class variables

		#region Class properties
		public SpriteFont Font { get; set; }
		public Texture2D ButtonLineTexture { get; set; }
		public Texture2D TitleTexture { get; set; }
		public SoundEffect MouseOverSfx { get; set; }
		public SoundEffect FootStepsSfx { get; set; }
		public SoundEffect PrisonCellSfx { get; set; }
#if DEBUG
		public Texture2D DebugChip { get; set; }
		public Texture2D DebugRing { get; set; }
#endif
		#endregion Class properties

		#region Support methods
		public static ResourceManager getInstance() {
			return instance;
		}

		public void init(GraphicsDevice device, ContentManager content) {
			this.Font = content.Load<SpriteFont>("Font");
			this.ButtonLineTexture = TextureUtils.create2DColouredTexture(device, 2, 2, Color.White);
			this.TitleTexture = content.Load<Texture2D>("Title");
			this.MouseOverSfx = content.Load<SoundEffect>("MouseOverButton");
			this.FootStepsSfx = content.Load<SoundEffect>("FootSteps");
			this.PrisonCellSfx = content.Load<SoundEffect>("CellDoor");
#if DEBUG
			this.DebugChip = TextureUtils.create2DColouredTexture(device, 32, 32, Color.White);
			this.DebugRing = TextureUtils.create2DRingTexture(device, 112, Color.White);
#endif
		}
		#endregion Support methods

		#region Destructor
		public void dispose() {
			if (this.ButtonLineTexture != null) {
				this.ButtonLineTexture.Dispose();
			}
			if (this.TitleTexture != null) {
				this.TitleTexture.Dispose();
			}
			if (this.MouseOverSfx != null) {
				this.MouseOverSfx.Dispose();
			}
			if (this.FootStepsSfx != null) {
				this.FootStepsSfx.Dispose();
			}
			if (this.PrisonCellSfx != null) {
				this.PrisonCellSfx.Dispose();
			}
#if DEBUG
			if (this.DebugChip != null) {
				this.DebugChip.Dispose();
			}
			if (this.DebugRing != null) {
				this.DebugRing.Dispose();
			}
#endif
		}
		#endregion Destructor
	}
}
