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
namespace Robber {
	public class Player : Person{
		#region Class variables
		private const float MOVEMENT_SPEED = 150f / 1000f;//. player always runs
		#endregion Class variables

		#region Class propeties
		public int CapturedTreasures { get; set; }
		#endregion Class properties

		#region Constructor
		public Player(ContentManager content, Placement startingLocation)
			: base(content, "Player", startingLocation, MOVEMENT_SPEED, true) {

		}
		#endregion Constructor

		#region Support methods
		public override void updateMove() {
			if (this.currentKeyBoardState.IsKeyDown(Keys.Left)) {
				this.direction = Person.Direction.Left;
			} else if (this.currentKeyBoardState.IsKeyDown(Keys.Up)) {
				this.direction = Person.Direction.Up;
			} else if (this.currentKeyBoardState.IsKeyDown(Keys.Right)) {
				this.direction = Person.Direction.Right;
			} else if (this.currentKeyBoardState.IsKeyDown(Keys.Down)) {
				this.direction = Person.Direction.Down;
			} else {
				// if none of our direction keys are down than we are not moving
				if (this.previousKeyBoardState.IsKeyDown(Keys.Left) || this.previousKeyBoardState.IsKeyDown(Keys.Up) || this.previousKeyBoardState.IsKeyDown(Keys.Right) || 
					this.previousKeyBoardState.IsKeyDown(Keys.Down)) {
					this.direction = Person.Direction.None;
				}
			}
		}
		#endregion Support methods

		#region Destructor

		#endregion Destructor
	}
}
