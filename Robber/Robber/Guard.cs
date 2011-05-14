using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
		private Stack<Point> path;
		private Thread AIThread;
		private MovementDirection movementDirection;
		private const float MOVEMENT_SPEED_WALK = 60f / 1000f;
		private const float MOVEMENT_SPEED_RUN = 155f / 1000f;
		private Texture2D chipTexture;
		#endregion Class variables

		#region Class propeties
		public bool RunAIThread { get; set; }
		#endregion Class properties

		#region Constructor
		public Guard(GraphicsDevice device, ContentManager content, Placement startingLocation, string state, string movementDirection)
			: base(content, "Guard", startingLocation, MOVEMENT_SPEED_WALK, false) {
			// figure out our direction
			if (movementDirection == MovementDirection.Clockwise.ToString()) {
				this.movementDirection = MovementDirection.Clockwise;
			} else {
				this.movementDirection = MovementDirection.CounterClockwise;
			}
			// figure out our starting state
			State initalState;
			if (state == State.Chase.ToString()) {
				initalState = State.Chase;
			} else if (state == State.Patrol.ToString()) {
				initalState = State.Patrol;
			} else if (state == State.Standing.ToString()) {
				initalState = State.Standing;
			} else {
				initalState = State.NotSpawned;
			}
			updateState(initalState);

			if (this.currentState == State.Patrol || AIManager.getInstane().PlayerDetected) {
				this.destinationWayPoint = AIManager.getInstane().getNextWayPoint(base.Placement.index, this.movementDirection);
				this.path = new Stack<Point>(AIManager.getInstane().findPath(base.Placement.index, this.destinationWayPoint));
				this.closestsPoint = this.path.Pop();
			}
			base.previousDirection = Direction.None;
			updateDirection();
			this.chipTexture = TextureUtils.create2DColouredTexture(device, 32, 32, Color.White);
			this.RunAIThread = true;
			this.AIThread= new Thread(new ThreadStart(generateMoves));
			this.AIThread.Start();
		}
		#endregion Constructor

		#region Support methods
		private void generateMoves() {
			do {
				Thread.Sleep(10);
				if (AIManager.getInstane().PlayerDetected) {
					updateState(State.Chase);
				}
				lock (AIManager.getInstane().Board) {
					// patrol can just generate the waypoint once
					if (this.currentState == State.Patrol) {
						if (base.Placement.index == this.destinationWayPoint) {
							this.destinationWayPoint = AIManager.getInstane().getNextWayPoint(base.Placement.index, this.movementDirection);
							this.path = new Stack<Point>(AIManager.getInstane().findPath(base.Placement.index, this.destinationWayPoint));
						} else if (base.Placement.index == this.closestsPoint) {
							if (this.path.Count >= 1) {
								this.closestsPoint = path.Pop();
							}
						}
					} else if (this.currentState == State.Chase) {
						// chase should regenerate the waypoint all the time
						this.path = new Stack<Point>(AIManager.getInstane().findPath(base.Placement.index));
						if (this.path.Count >= 1) {
							this.closestsPoint = path.Pop();
						}
					}
				}
				if (!this.RunAIThread) {
					break;
				}
			} while (this.RunAIThread);
		}

		private void updateState(State newState) {
			if (newState == State.Chase) {
				base.movementSpeed = MOVEMENT_SPEED_RUN;
			} else if (newState == State.Patrol) {
				base.movementSpeed = MOVEMENT_SPEED_WALK;
			} else if (newState == State.Standing) {
				base.direction = Direction.None;
			}
			this.currentState = newState;
		}

		private void updateDirection() {
			// figure out what direction our closests point is
			Point temp = new Point(base.Placement.index.X - this.closestsPoint.X, base.Placement.index.Y - this.closestsPoint.Y);
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
		}

		public override void updateMove() {
			updateDirection();
		}

		public new void render(SpriteBatch spriteBatch) {
#if DEBUG
			spriteBatch.Draw(this.chipTexture, new Placement(this.destinationWayPoint).worldPosition, Color.Green);
#endif
			
			base.render(spriteBatch);
		}
		#endregion Support methods

		#region Destructor
		public new void dispose() {
			this.chipTexture.Dispose();
			this.RunAIThread = false;
			this.AIThread.Abort();
			base.dispose();
		}
		#endregion Destructor
	}
}
