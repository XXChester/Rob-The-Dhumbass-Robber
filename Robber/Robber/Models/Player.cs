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
using GWNorthEngine.AI.AStar;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Scripting;
using GWNorthEngine.Input;
using GWNorthEngine.Utils;
namespace Robber {
	public class Player : Person{
		#region Class variables
		private SoundEffect footStepsSfx;
		private float footStepSFXDelay;
		private const float MOVEMENT_SPEED = 150f / 1000f;//. player always runs
		private const float FOOT_STEP_SFX_DELAY = 600f;
		#endregion Class variables

		#region Class propeties
		public int CapturedTreasures { get; set; }
		public bool Hiding { get; set; }
		#endregion Class properties

		#region Constructor
		public Player(ContentManager content, Placement startingLocation)
			: base(content, "Rob", startingLocation, MOVEMENT_SPEED) {

			this.footStepsSfx = LoadingUtils.loadSoundEffect(content, "FootSteps");
		}
		#endregion Constructor

		#region Support methods
		protected override void updateDirection(float elapsed) {
			if (!this.Hiding) {
				if (InputManager.getInstance().isKeyDown(Keys.Left)) {
					base.direction = Person.Direction.Left;
				} else if (InputManager.getInstance().isKeyDown(Keys.Up)) {
					base.direction = Person.Direction.Up;
				} else if (InputManager.getInstance().isKeyDown(Keys.Right)) {
					base.direction = Person.Direction.Right;
				} else if (InputManager.getInstance().isKeyDown(Keys.Down)) {
					base.direction = Person.Direction.Down;
				} else {
					// if none of our direction keys are down than we are not moving
					base.direction = Person.Direction.None;
				}

				//play movement sfx
				if (base.direction != Direction.None) {
					if (this.footStepSFXDelay >= FOOT_STEP_SFX_DELAY) {
						SoundManager.getInstance().sfxEngine.playSoundEffect(this.footStepsSfx);
						this.footStepSFXDelay = 0f;
					}
				}
				this.footStepSFXDelay += elapsed;
			}
		}

		protected override void updateLocation(float elapsed) {
			if (!this.Hiding) {
				if (this.direction != Direction.None) {
					float moveDistance = (this.movementSpeed * elapsed);
					Vector2 newPos = this.activeSprite.Position;
					if (this.direction == Direction.Up) {
						newPos = new Vector2(this.activeSprite.Position.X, this.activeSprite.Position.Y - moveDistance);
					} else if (this.direction == Direction.Right) {
						newPos = new Vector2(this.activeSprite.Position.X + moveDistance, this.activeSprite.Position.Y);
					} else if (this.direction == Direction.Down) {
						newPos = new Vector2(this.activeSprite.Position.X, this.activeSprite.Position.Y + moveDistance);
					} else if (this.direction == Direction.Left) {
						newPos = new Vector2(this.activeSprite.Position.X - moveDistance, this.activeSprite.Position.Y);
					}
					// check if the new position would result in a collision, if not, assign the sprite the position
					if (!CollisionManager.getInstance().wallCollisionFound(Helper.getPersonBBox(newPos))) {
						this.activeSprite.Position = newPos;
					}
				}
			}
			
			// call the generic code before continuing
			base.updateLocation(elapsed);

			if (AIManager.getInstance().PlayerDetected) {// if we aren't moving we still need to report where we are if we are detected
				// if we have been detected we need to tell the AI where we are
				AIManager.getInstance().Board[base.Placement.index.Y, base.Placement.index.X] = BasePathFinder.TypeOfSpace.End;
			}
		}

		public override void render(SpriteBatch spriteBatch) {
			if (!this.Hiding) {
				base.render(spriteBatch);
			}
		}
		#endregion Support methods

		#region Destructor
		public override void dispose() {
			//sfxs
			/*
			if (this.FootStepsSfx != null) {
				this.FootStepsSfx.Dispose();
			}*/
			base.dispose();
		}
		#endregion Destructor
	}
}
