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
using GWNorthEngine.Scripting;
namespace Robber {
	public abstract class Person : IRenderable {
		protected enum Direction {
			Up,
			Right,
			Down,
			Left,
			None
		}
		#region Class variables
		private Animated2DSprite upSprite;
		private Animated2DSprite rightSprite;
		private Animated2DSprite downSprite;
		private Animated2DSprite leftSprite;
		protected Animated2DSprite activeSprite;
		protected float movementSpeed;
		protected Direction direction;
		protected Direction previousDirection;
		protected KeyboardState currentKeyBoardState;
		protected KeyboardState previousKeyBoardState;
		#endregion Class variables

		#region Class propeties
		public Placement Placement { get; set; }
		#endregion Class properties

		#region Constructor
		public Person(ContentManager content, string fileStartsWith, Placement startingLocation, float movementSpeed) {
			string fileName = fileStartsWith + "Right";
			Animated2DSpriteParams parms = new Animated2DSpriteParams();
			parms.AnimationState = AnimationManager.AnimationState.PlayForward;
			parms.Content = content;
			parms.FrameRate = 200f;
			parms.LoadingType = Animated2DSprite.LoadingType.WholeSheetReadFramesFromFile;
			parms.TexturesName = fileName;
			parms.TotalFrameCount = 1;
			parms.Position = startingLocation.worldPosition;
			this.rightSprite = new Animated2DSprite(parms);
			fileName = fileStartsWith + "Left";
			parms.TexturesName = fileName;
			this.leftSprite = new Animated2DSprite(parms);
			fileName = fileStartsWith + "Down";
			parms.TexturesName = fileName;
			this.downSprite = new Animated2DSprite(parms);
			fileName = fileStartsWith + "Up";
			parms.TexturesName = fileName;
			this.upSprite = new Animated2DSprite(parms);
			this.activeSprite = rightSprite;
			this.Placement = startingLocation;
			this.direction = Direction.None;
			this.movementSpeed = movementSpeed;
		}
		#endregion Constructor

		#region Support methods
		public abstract void updateMove();

		public void update(float elapsed) {
			this.currentKeyBoardState = Keyboard.GetState();
			if (this.activeSprite != null) {
				this.activeSprite.update(elapsed);
			}
			updateMove();
			if (this.direction != Direction.None) {
				float moveDistance = (movementSpeed * elapsed);
				// we are moving
				// if we are not moving in the same direction we need to change our sprite
				bool updateSprite = false;
				if (this.direction != this.previousDirection) {
					updateSprite = true;
				}
				if (this.direction == Direction.Up) {
					if (updateSprite) {
						this.upSprite.Position = this.activeSprite.Position;
						this.activeSprite = this.upSprite;
					}
					this.activeSprite.Position = new Vector2(this.activeSprite.Position.X, this.activeSprite.Position.Y - moveDistance);
				} else if (this.direction == Direction.Right) {
					if (updateSprite) {
						this.rightSprite.Position = this.activeSprite.Position;
						this.activeSprite = this.rightSprite;
					}
					this.activeSprite.Position = new Vector2(this.activeSprite.Position.X + moveDistance, this.activeSprite.Position.Y);
					if (updateSprite) {
						this.downSprite.Position = this.activeSprite.Position;
						this.activeSprite = this.downSprite;
					}
				} else if (this.direction == Direction.Down) {
					this.activeSprite.Position = new Vector2(this.activeSprite.Position.X, this.activeSprite.Position.Y + moveDistance);
				} else if (this.direction == Direction.Left) {
					if (updateSprite) {
						this.leftSprite.Position = this.activeSprite.Position;
						this.activeSprite = this.leftSprite;
					}
					this.activeSprite.Position = new Vector2(this.activeSprite.Position.X - moveDistance, this.activeSprite.Position.Y);
				}
			}
			this.previousDirection = this.direction;
			this.previousKeyBoardState = this.currentKeyBoardState;
		}

		public void render(SpriteBatch spriteBatch) {
			if (this.activeSprite != null) {
				this.activeSprite.render(spriteBatch);
			}
		}
		#endregion Support methods

		#region Destructor
		public void dispose() {
			if (this.leftSprite!= null) {
				this.leftSprite.dispose();
			}
			if (this.rightSprite != null) {
				this.rightSprite.dispose();
			}
			if (this.upSprite != null) {
				this.upSprite.dispose();
			}
			if (this.downSprite != null) {
				this.downSprite.dispose();
			}
			if (this.activeSprite != null) {
				this.activeSprite.dispose();
			}
		}
		#endregion Destructor
	}
}
