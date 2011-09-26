using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
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
		protected Animated2DSprite rightSprite;
		protected Animated2DSprite leftSprite;
		protected BasePathFinder.TypeOfSpace previousTypeOfSpace;
		protected Animated2DSprite activeSprite;
		protected float movementSpeed;
		protected Direction direction;
		protected Direction previousDirection;
		protected Placement previousPlacement;
		private const float FRAME_RATE = 150f;
		#endregion Class variables

		#region Class propeties
		public Texture2D ActiveTexture { get { return this.activeSprite.Texture; } }
		public Animated2DSprite Sprite { get { return this.activeSprite; } }
		public Placement Placement { get; set; }
		public BoundingBox BoundingBox { get; set; }
		#endregion Class properties

		#region Constructor
		public Person(ContentManager content, string fileStartsWith, Placement startingLocation, float movementSpeed) {
			BaseAnimationManagerParams animationParams = new BaseAnimationManagerParams();
			animationParams.AnimationState = AnimationManager.AnimationState.PlayForward;
			animationParams.FrameRate = FRAME_RATE;
			animationParams.TotalFrameCount = 4;
			Animated2DSpriteParams spriteParams = new Animated2DSpriteParams();
			spriteParams.Content = content;
			spriteParams.Origin = new Vector2(ResourceManager.TILE_SIZE / 2, ResourceManager.TILE_SIZE / 2);
			spriteParams.LoadingType = Animated2DSprite.LoadingType.WholeSheetReadFramesFromFile;
			spriteParams.TexturesName = fileStartsWith + "Right";
			spriteParams.AnimationParams = animationParams; ;
			// we actually want his feet in the middle, not his chest which is where the origin is
			spriteParams.Position = new Vector2(startingLocation.worldPosition.X + spriteParams.Origin.X, startingLocation.worldPosition.Y);
			this.rightSprite = new Animated2DSprite(spriteParams);
			spriteParams.SpriteEffect = SpriteEffects.FlipHorizontally;
			this.leftSprite = new Animated2DSprite(spriteParams);
			this.Placement = startingLocation;
			this.direction = Direction.None;
			this.movementSpeed = movementSpeed;
			this.activeSprite = this.rightSprite;
			this.BoundingBox = Helper.getPersonBBox(this.activeSprite.Position);
			this.previousTypeOfSpace = AIManager.getInstance().Board[this.Placement.index.Y, this.Placement.index.X];
		}
		#endregion Constructor

		#region Support methods
		protected abstract void updateDirection(float elapsed);

		protected virtual void updateSpritesVisuals(float elapsed) {
			updateDirection(elapsed);
			if (this.previousDirection != this.direction) {
				if (this.direction == Direction.None) {
					// we are no longer moving so stop our sprite
					this.activeSprite.reset(true);
				} else {
					this.activeSprite.AnimationManager.State = AnimationManager.AnimationState.PlayForward;
				}
			}
			if (this.direction != Direction.None) {
				// we are moving
				// if we are not moving in the same direction we need to change our sprite
				bool updateSprite = false;
				if (this.direction != this.previousDirection) {
					updateSprite = true;
				}
				if (this.direction == Direction.Up) {
					if (updateSprite) {
						this.leftSprite.Position = this.activeSprite.Position;
						if (this.previousDirection == Direction.Left) {
							this.activeSprite = this.leftSprite;
						} else if (this.previousDirection == Direction.Right) {
							this.activeSprite = this.rightSprite;
						} else {
							this.activeSprite = this.leftSprite;
						}
					}
				} else if (this.direction == Direction.Right) {
					if (updateSprite) {
						this.rightSprite.Position = this.activeSprite.Position;
						this.activeSprite = this.rightSprite;
					}
				} else if (this.direction == Direction.Down) {
					if (updateSprite) {
						this.rightSprite.Position = this.activeSprite.Position;
						if (this.previousDirection == Direction.Left) {
							this.activeSprite = this.leftSprite;
						} else if (this.previousDirection == Direction.Right) {
							this.activeSprite = this.rightSprite;
						} else {
							this.activeSprite = this.rightSprite;
						}
					}
				} else if (this.direction == Direction.Left) {
					if (updateSprite) {
						this.leftSprite.Position = this.activeSprite.Position;
						this.activeSprite = this.leftSprite;
					}
				}
			}
		}

		protected virtual void updateLocation(float elapsed) {
			// update our placement and bounding box
			this.Placement = new Placement(Placement.getIndex(this.activeSprite.Position));
			this.BoundingBox = Helper.getPersonBBox(this.activeSprite.Position);

			if (this.previousPlacement.index != this.Placement.index) {
				AIManager.getInstance().Board[this.previousPlacement.index.Y, this.previousPlacement.index.X] = this.previousTypeOfSpace;
				this.previousTypeOfSpace = AIManager.getInstance().Board[this.Placement.index.Y, this.Placement.index.X];
			}
		}

		public void updateColours(Color colour) {
			this.activeSprite.LightColour = colour;
		}

		public void update(float elapsed) {
			if (this.activeSprite != null) {
				this.activeSprite.update(elapsed);
			}
			updateSpritesVisuals(elapsed);
			updateLocation(elapsed);
			this.previousDirection = this.direction;
			this.previousPlacement = this.Placement;
		}

		public virtual void render(SpriteBatch spriteBatch) {
			if (this.activeSprite != null) {
				this.activeSprite.render(spriteBatch);
			}
		}
		#endregion Support methods

		#region Destructor
		public virtual void dispose() {
			if (this.leftSprite!= null) {
				this.leftSprite.dispose();
			}
			if (this.rightSprite != null) {
				this.rightSprite.dispose();
			}
			if (this.activeSprite != null) {
				this.activeSprite.dispose();
			}
		}
		#endregion Destructor
	}
}
