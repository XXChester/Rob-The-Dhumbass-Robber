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
using GWNorthEngine.Utils;
namespace Robber {
	public class Guard : Person {
		public enum State {
			Chase,
			Patrol,
			Standing,
			NotSpawned
		}

		public enum MovementDirection {
			Clockwise,
			CounterClockwise
		}
		#region Class variables
		private State currentState;
		private Point destinationWayPoint;
		private Point closestsPoint;
		private Queue<Point> path;
		private MovementDirection movementDirection;
		private const float MOVEMENT_SPEED_WALK = 60f / 1000f;
		private const float MOVEMENT_SPEED_RUN = 155f / 1000f;
		private Texture2D chipTexture;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public Guard(GraphicsDevice device, ContentManager content, Placement startingLocation, string state, string movementDirection)
			: base(content, "Guard", startingLocation, MOVEMENT_SPEED_WALK) {
			// figure out our direction
			if (movementDirection == MovementDirection.Clockwise.ToString()) {
				this.movementDirection = MovementDirection.Clockwise;
			} else {
				this.movementDirection = MovementDirection.CounterClockwise;
			}
			// figure out our starting state
			if (state == State.Chase.ToString()) {
				this.currentState = State.Chase;
				base.movementSpeed = MOVEMENT_SPEED_RUN;
			} else if (state == State.Patrol.ToString()) {
				this.currentState = State.Patrol;
			} else if (state == State.Standing.ToString()) {
				this.currentState = State.Standing;
			} else {
				this.currentState = State.NotSpawned;
			}

			if (this.currentState == State.Patrol) {
				this.destinationWayPoint = AIManager.getInstane().getNextWayPoint(base.Placement.index, this.movementDirection);
				this.path = new Queue<Point>(AIManager.getInstane().findPath(base.Placement.index, this.destinationWayPoint));
				this.closestsPoint = this.path.Dequeue();
			}
			this.chipTexture = TextureUtils.create2DColouredTexture(device, 32, 32, Color.White);
		}
		#endregion Constructor

		#region Support methods
		public override void updateMove() {
			if (this.currentState == State.Chase || this.currentState == State.Patrol) {
				if (base.Placement.index == this.destinationWayPoint) {
					this.destinationWayPoint = AIManager.getInstane().getNextWayPoint(base.Placement.index, this.movementDirection);
					this.path = new Queue<Point>(AIManager.getInstane().findPath(base.Placement.index, this.destinationWayPoint));
				} else if (base.Placement.index != this.closestsPoint) {
					// figure out what direction our next waypoint is
					Point temp = new Point(base.Placement.index.X - closestsPoint.X, base.Placement.index.Y - closestsPoint.Y);
					if (temp.X <= -1) {
						base.direction = Direction.Right;
					} else if (temp.X >= 1) {
						base.direction = Direction.Left;
					} else if (temp.Y <= -1) {
						base.direction = Direction.Down;
					} else if (temp.Y >= 1) {
						base.direction = Direction.Up;
					} else {
						base.direction = Direction.None;
					}
				} else {
					this.closestsPoint = path.Dequeue();
				}
			}
		}

		public void render(SpriteBatch spriteBatch) {
#if DEBUG
			spriteBatch.Draw(this.chipTexture, new Placement(this.closestsPoint).worldPosition, Color.Green);
#endif
			
			base.render(spriteBatch);
		}
		#endregion Support methods

		#region Destructor
		~Guard() {
			this.chipTexture.Dispose();
		}
		#endregion Destructor
	}
}
