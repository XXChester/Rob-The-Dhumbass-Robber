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
using GWNorthEngine.AI.AStar;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
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
		private RadiusRing ring;
		private State currentState;
		private Point destinationWayPoint;
		private Point closestsPoint;
		private Stack<Point> path;
		private MovementDirection movementDirection;
		private PathRequest.FoundPathCallBack callBackDelegate;
		private const float MOVEMENT_SPEED_WALK = 60f / 1000f;
		private const float MOVEMENT_SPEED_RUN = 155f / 1000f;
		#endregion Class variables

		#region Class propeties
		public RadiusRing Ring { get { return this.ring; } }
		#endregion Class properties

		#region Constructor
		public Guard(ContentManager content, Placement startingLocation, string state, string movementDirection)
			: base(content, "Guard", startingLocation, MOVEMENT_SPEED_WALK) {
			this.callBackDelegate = delegate(Stack<Point> path) {
				this.path = path;
			};
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

			if (this.currentState == State.Patrol && !AIManager.getInstance().PlayerDetected) {
				this.destinationWayPoint = AIManager.getInstance().getNextWayPoint(base.Placement.index, this.movementDirection);
				AIManager.getInstance().requestPath(base.Placement.index, this.destinationWayPoint, delegate(Stack<Point> path) {
					this.callBackDelegate.Invoke(path);
					if (this.path != null && this.path.Count >= 1) {
						this.closestsPoint = this.path.Pop();
					}
				});
			} else if (this.currentState == State.Chase || AIManager.getInstance().PlayerDetected) {
				AIManager.getInstance().requestPath(base.Placement.index, delegate(Stack<Point> path) {
					this.callBackDelegate.Invoke(path);
					if (this.path != null && this.path.Count >= 1) {
						this.closestsPoint = this.path.Pop();
					}
				});
			} else if (this.currentState == State.Standing || this.currentState == State.NotSpawned) {
				this.closestsPoint = new Point(-1, -1);
			}

			this.ring = new RadiusRing(content, base.activeSprite.Position);
		}
		#endregion Constructor

		#region Support methods
		private void generateMove() {
			if (AIManager.getInstance().PlayerDetected) {
				updateState(State.Chase);
			}

			// patrol can just generate the waypoint once
			if (this.currentState == State.Patrol) {
				if (base.Placement.index == this.destinationWayPoint) {
					this.destinationWayPoint = AIManager.getInstance().getNextWayPoint(base.Placement.index, this.movementDirection);
					AIManager.getInstance().requestPath(base.Placement.index, this.destinationWayPoint, this.callBackDelegate);
				} else if (base.Placement.index == this.closestsPoint) {
					if (this.path.Count >= 1) {
						this.closestsPoint = path.Pop();
					}
				}
			} else if (this.currentState == State.Chase) {
				// chase should regenerate the waypoint all the time
				if (this.closestsPoint == base.Placement.index) {
					AIManager.getInstance().requestPath(base.Placement.index, delegate(Stack<Point> path) {
						this.callBackDelegate.Invoke(path);
						if (this.path.Count >= 1) {
							this.closestsPoint = path.Pop();
						}
					});
				}
			}
		}

		private void updateState(State newState) {
			if (newState == State.Chase) {
				base.movementSpeed = MOVEMENT_SPEED_RUN;
				base.leftSprite.AnimationManager.FrameRate = 150f;
			} else if (newState == State.Patrol) {
				base.movementSpeed = MOVEMENT_SPEED_WALK;
				base.leftSprite.AnimationManager.FrameRate = 200f;
			} else if (newState == State.Standing) {
				base.direction = Direction.None;
				base.leftSprite.AnimationManager.FrameRate = 0f;
			}
			this.currentState = newState;
		}

		private bool withinDestinationSquare() {
			bool withinDestination = false;
			Point temp = new Point(base.Placement.index.X - this.closestsPoint.X, base.Placement.index.Y - this.closestsPoint.Y);
			if (temp.X == 0 && temp.Y == 0) {
				withinDestination = true;
			}
			return withinDestination;
		}

		private Vector2 getMiddleOfCurrentSquare() {
			Placement squaresPlacement = new Placement(this.closestsPoint);
			return Vector2.Add(squaresPlacement.worldPosition, new Vector2(ResourceManager.TILE_SIZE / 2f));
		}

		private Vector2 getMiddleOfSprite() {
			return new Vector2(this.activeSprite.Position.X, this.activeSprite.Position.Y + this.activeSprite.Origin.Y);
		}

		protected override void updateDirection(float elapsed) {
			// figure out what direction our closests point is
			if (this.closestsPoint.X == -1 && this.closestsPoint.Y == -1) {
				base.direction = Direction.None;
			} else {
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
					Vector2 middleOfSquare = getMiddleOfCurrentSquare();
					Vector2 middleOfSprite = getMiddleOfSprite();
					Vector2 difference = Vector2.Subtract(middleOfSquare, middleOfSprite);
					if (Vector2.Zero == difference) {
						base.direction = Direction.None;
						generateMove();
					} else {
						// find the direction of the middle
						if (difference.X < 0) {
							base.direction = Direction.Left;
						} else if (difference.X > 0) {
							base.direction = Direction.Right;
						} else if (difference.Y > 0) {
							base.direction = Direction.Down;
						} else if (difference.Y < 0) {
							base.direction = Direction.Up;
						}
					}
				}
			}
		}

		protected override void updateLocation(float elapsed) {
			this.ring.updatePosition(base.activeSprite.Position);
			if (base.previousPlacement.index != base.Placement.index) {
				AIManager.getInstance().Board[base.Placement.index.Y, base.Placement.index.X] = PathFinder.TypeOfSpace.Unwalkable;
			}
			Point endNode = AIManager.getInstance().findEndNode();
			if (this.closestsPoint == endNode) {
				StateManager.getInstance().CurrentGameState = StateManager.GameState.GameOver;
				StateManager.getInstance().TypeOfGameOver = StateManager.GameOverType.Guards;
				SoundManager.getInstance().sfxEngine.playSoundEffect(ResourceManager.getInstance().PrisonCellSfx);
			}

			if (this.direction != Direction.None) {
				float moveDistanceX = (this.movementSpeed * elapsed);
				float moveDistanceY = (this.movementSpeed * elapsed);
				if (withinDestinationSquare()) {
					// check if the center is less of a distance than the default via the elapsed * moevment speed
					Vector2 currentSquaresMid = getMiddleOfCurrentSquare();
					Vector2 middleOfSprite = getMiddleOfSprite();
					Vector2 max = Vector2.Max(currentSquaresMid, middleOfSprite);
					Vector2 min = Vector2.Min(currentSquaresMid, middleOfSprite);
					Vector2 difference = Vector2.Subtract(max, min);

					if (base.direction == Direction.Up) {
						if (difference.Y < moveDistanceY) {
							moveDistanceY = difference.Y;
						}
					} else if (base.direction == Direction.Down) {
						if (difference.Y < moveDistanceY) {
							moveDistanceY = difference.Y;
						}
					} else if (base.direction == Direction.Left) {
						if (difference.X < moveDistanceX) {
							moveDistanceX = difference.X;
						}
					} else if (base.direction == Direction.Right) {
						if (difference.X < moveDistanceX) {
							moveDistanceX = difference.X;
						}
					}
				}
				Vector2 newPos = base.activeSprite.Position;
				if (this.direction == Direction.Up) {
					newPos = new Vector2(this.activeSprite.Position.X, this.activeSprite.Position.Y - moveDistanceY);
				} else if (this.direction == Direction.Right) {
					newPos = new Vector2(this.activeSprite.Position.X + moveDistanceX, this.activeSprite.Position.Y);
				} else if (this.direction == Direction.Down) {
					newPos = new Vector2(this.activeSprite.Position.X, this.activeSprite.Position.Y + moveDistanceY);
				} else if (this.direction == Direction.Left) {
					newPos = new Vector2(this.activeSprite.Position.X - moveDistanceX, this.activeSprite.Position.Y);
				}
				base.activeSprite.Position = newPos;
			}

			// call the generic code before continuing
			base.updateLocation(elapsed);

			if (this.previousPlacement.index != this.Placement.index) {
				AIManager.getInstance().Board[this.Placement.index.Y, this.Placement.index.X] = PathFinder.TypeOfSpace.Unwalkable;
			}
		}

		public new void render(SpriteBatch spriteBatch) {
			this.ring.render(spriteBatch);
#if DEBUG
			spriteBatch.Draw(ResourceManager.getInstance().DebugChip, new Placement(this.destinationWayPoint).worldPosition, Color.Green);
#endif
			
			base.render(spriteBatch);
#if DEBUG
			Vector2 sp = getMiddleOfSprite();
			Vector2 start = Vector2.Subtract(sp, new Vector2(2f));
			Vector2 end = Vector2.Add(sp, new Vector2(2f));
			BoundingBox bbox = Helper.getBBox(start, end);
			DebugUtils.drawBoundingBox(spriteBatch, bbox, Color.Yellow, ResourceManager.getInstance().ButtonLineTexture);
#endif
		}
		#endregion Support methods

		#region Destructor
		public new void dispose() {
			if (this.ring != null) {
				this.ring.dispose();
			}
			base.dispose();
		}
		#endregion Destructor
	}
}
