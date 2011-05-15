using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	public class Timer : IRenderable{
		#region Class variables
		private Text2D firstPart;
		private Text2D secondPart;
		private Text2D timeText;
		private float time;
		private const string FIRST_PART = "Time Until";
		private const string SECOND_PART = "Detection;";
		#endregion Class variables

		#region Class propeties
		public float Time { get { return this.time; } }
		#endregion Class properties

		#region Constructor
		public Timer() {
			Text2DParams parms = new Text2DParams();
			parms.Font = ResourceManager.getInstance().Font;
			parms.LightColour = ResourceManager.TEXT_COLOUR;
			parms.Position = new Vector2(682f, 14f);
			parms.WrittenText = FIRST_PART;
			this.firstPart = new Text2D(parms);

			parms.Position = new Vector2(684f,39f);
			parms.WrittenText = SECOND_PART;
			this.secondPart = new Text2D(parms);

			parms.Position = new Vector2(702f, 63f);
			parms.WrittenText = "0";
			this.timeText = new Text2D(parms);
#if WINDOWS
#if DEBUG
			ScriptManager.getInstance().registerObject(this.firstPart, "first");
			ScriptManager.getInstance().registerObject(this.secondPart, "second");
			ScriptManager.getInstance().registerObject(this.timeText, "third");
#endif
#endif
		}
		#endregion Constructor

		#region Support methods
		public void updateColours(Color colour) {
			this.firstPart.LightColour = colour;
			this.secondPart.LightColour = colour;
			this.timeText.LightColour = colour;
		}

		public void reset(float timeUntilDetection) {
			this.time = timeUntilDetection * 60f * 1000f;// passed in as minutes, convert to mas minutes, convert to miliseconds
		}

		public void update(float elapsed) {
			this.time -= elapsed;
			if (this.time <= 0f) {
				//Alert the authorities
				AIManager.getInstane().PlayerDetected = true;
				this.timeText.WrittenText = "0";
			} else {
				this.timeText.WrittenText = this.time.ToString();
			}
		}

		public void render(SpriteBatch spriteBatch) {
			this.firstPart.render(spriteBatch);
			this.secondPart.render(spriteBatch);
			this.timeText.render(spriteBatch);
		}
		#endregion Support methods

		#region Destructor
		public void dispose() {
			
		}
		#endregion Destructor
	}
}
