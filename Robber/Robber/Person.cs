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
using GWNorthEngine.Utils;
using GWNorthEngine.AI.AStar;
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
		private bool updateAI;
		private PathFinder.TypeOfSpace previousTypeOfSpace;
		protected Animated2DSprite activeSprite;
		protected float movementSpeed;
		protected Direction direction;
		protected Direction previousDirection;
		protected KeyboardState currentKeyBoardState;
		protected KeyboardState previousKeyBoardState;
		protected Placement previousPlacement;
		#endregion Class variables

		#region Class propeties
		public Texture2D ActiveTexture { get { return this.activeSprite.Texture; } }
		public Color LightColour { get { return this.activeSprite.LightColour; } }
		public Placement Placement { get; set; }
		public BoundingBox BoundingBox { get; set; }
		#endregion Class properties

		#region Constructor
		public Person(ContentManager content, string fileStartsWith, Placement startingLocation, float movementSpeed, bool updateAI) {
			string fileName = fileStartsWith + "Right";
			Animated2DSpriteParams parms = new Animated2DSpriteParams();
			parms.AnimationState = AnimationManager.AnimationState.PlayForward;
			parms.Content = content;
			parms.Origin = new Vector2(16f, 16f);
			parms.FrameRate = 200f;
			parms.LoadingType = Animated2DSprite.LoadingType.WholeSheetReadFramesFromFile;
			parms.TexturesName = fileName;
			parms.TotalFrameCount = 1;
			parms.Position = startingLocation.worldPosition;
			this.rightSprite = new Animated2DSprite(parms);
			fileName = fileStartsWith + "Left";
			parms.TexturesName = fileName;
			parms.Texture2D = null;
			this.leftSprite = new Animated2DSprite(parms);
			fileName = fileStartsWith + "Down";
			parms.TexturesName = fileName;
			parms.Texture2D = null;
			this.downSprite = new Animated2DSprite(parms);
			fileName = fileStartsWith + "Up";
			parms.TexturesName = fileName;
			parms.Texture2D = null;
			this.upSprite = new Animated2DSprite(parms);
			this.Placement = startingLocation;
			this.direction = Direction.None;
			this.movementSpeed = movementSpeed;
			this.BoundingBox = Helper.getBBox(this.Placement.worldPosition);
			this.activeSprite = this.rightSprite;
			this.updateAI = updateAI;
			this.previousTypeOfSpace = AIManager.getInstane().Board[this.Placement.index.Y, this.Placement.index.X];
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
				} else if (this.direction == Direction.Down) {
					if (updateSprite) {
						this.downSprite.Position = this.activeSprite.Position;
						this.activeSprite = this.downSprite;
					}
					this.activeSprite.Position = new Vector2(this.activeSprite.Position.X, this.activeSprite.Position.Y + moveDistance);
				} else if (this.direction == Direction.Left) {
					if (updateSprite) {
						this.leftSprite.Position = this.activeSprite.Position;
						this.activeSprite = this.leftSprite;
					}
					this.activeSprite.Position = new Vector2(this.activeSprite.Position.X - moveDistance, this.activeSprite.Position.Y);
				}
				this.activeSprite.Position = new Vector2(MathHelper.Clamp(this.activeSprite.Position.X, 0f, 671), MathHelper.Clamp(this.activeSprite.Position.Y, 0, 575));
			}
			// update our placement and bounding box
			this.Placement = new Placement(Placement.getIndex(this.activeSprite.Position));
			this.BoundingBox = Helper.getBBox(this.Placement.worldPosition);
			// if there was a collision we cannot move there
			if (CollisionManager.getInstance().collisionFound(this.BoundingBox)) {
				this.activeSprite.Position = this.previousPlacement.worldPosition;
				this.Placement = new Placement(Placement.getIndex(this.activeSprite.Position));
				this.BoundingBox = Helper.getBBox(this.Placement.worldPosition);
			}
			if (this.updateAI) {
				if (this.previousPlacement.index != this.Placement.index) {
					AIManager.getInstane().Board[this.previousPlacement.index.Y, this.previousPlacement.index.X] = this.previousTypeOfSpace;
					this.previousTypeOfSpace = AIManager.getInstane().Board[this.Placement.index.Y, this.Placement.index.X];
				}
				// if we have been detected we need to tell the AI where we are
				AIManager.getInstane().updatePlayerPosition(this.Placement.index);
			}
			this.previousDirection = this.direction;
			this.previousKeyBoardState = this.currentKeyBoardState;
			this.previousPlacement = this.Placement;
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
