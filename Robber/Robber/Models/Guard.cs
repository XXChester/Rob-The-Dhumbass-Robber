﻿using System;
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
		private Thread AIThread;
		private MovementDirection movementDirection;
		private const float MOVEMENT_SPEED_WALK = 60f / 1000f;
		private const float MOVEMENT_SPEED_RUN = 155f / 1000f;
		private bool foundMiddle;
		#endregion Class variables

		#region Class propeties
		public bool RunAIThread { get; set; }
		public RadiusRing Ring { get { return this.ring; } }
		#endregion Class properties

		#region Constructor
		public Guard(ContentManager content, Placement startingLocation, string state, string movementDirection)
			: base(content, "Guard", startingLocation, MOVEMENT_SPEED_WALK) {
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

			if (this.currentState == State.Patrol && !AIManager.getInstane().PlayerDetected) {
				this.destinationWayPoint = AIManager.getInstane().getNextWayPoint(base.Placement.index, this.movementDirection);
				this.path = new Stack<Point>(AIManager.getInstane().findPath(base.Placement.index, this.destinationWayPoint));
				this.closestsPoint = this.path.Pop();
			} else if (this.currentState == State.Chase || AIManager.getInstane().PlayerDetected) {
				this.path = new Stack<Point>(AIManager.getInstane().findPath(base.Placement.index));
				if (this.path.Count >= 1) {
					this.closestsPoint = path.Pop();
				}
			} else if (this.currentState == State.Standing || this.currentState == State.NotSpawned) {
				this.closestsPoint = new Point(-1, -1);
			}
			updateDirection();
			this.RunAIThread = true;
			this.AIThread= new Thread(new ThreadStart(generateMoves));
			this.AIThread.Start();

			this.ring = new RadiusRing(content, base.activeSprite.Position);
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
							if (this.path.Count >= 1 && this.foundMiddle) {
								this.closestsPoint = path.Pop();
							}
						}
					} else if (this.currentState == State.Chase) {
						// chase should regenerate the waypoint all the time
						if (this.closestsPoint == base.Placement.index) {
							this.path = new Stack<Point>(AIManager.getInstane().findPath(base.Placement.index));
							if (this.path.Count == 1) {
								Console.WriteLine("About to capture");
							}
							if (this.path.Count >= 1) {
								this.closestsPoint = path.Pop();
								this.foundMiddle = false;
							}
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

		private void updateDirection() {
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
					// need a distance check for the middle of the square
					Placement squaresPlacement = new Placement(this.closestsPoint);
					Vector2 middleOfSquare =
						new Vector2(squaresPlacement.worldPosition.X + ResourceManager.TILE_SIZE / 2f, squaresPlacement.worldPosition.Y + ResourceManager.TILE_SIZE / 2f);
					if (this.activeSprite.Position == middleOfSquare) {
						this.foundMiddle = true;
						base.direction = Direction.None;
					} else {
						Vector2 midsDirection = new Vector2(base.activeSprite.Position.X - middleOfSquare.X, base.activeSprite.Position.Y - middleOfSquare.Y);
						if (midsDirection.X < 0) {
							base.direction = Direction.Right;
						} else if (midsDirection.X > 0) {
							base.direction = Direction.Left;
						} else if (midsDirection.Y < 0) {
							base.direction = Direction.Down;
						} else if (midsDirection.Y > 0) {
							base.direction = Direction.Up;
						}
					}
				}
			}
		}

		public override void updateMove(float elapsed) {
			updateDirection();
			if (base.direction != Direction.None) {
				float moveDistance = (base.movementSpeed * elapsed);
				int decimalPlaces = 10000;
				// round our vector
				float roundedY = (long)(base.activeSprite.Position.Y * decimalPlaces);
				float roundedX = (long)(base.activeSprite.Position.X * decimalPlaces);
				base.activeSprite.Position = new Vector2(((float)roundedX / decimalPlaces), ((float)roundedY / decimalPlaces));
				
				Placement squaresPlacement = new Placement(this.closestsPoint);
				Vector2 middleOfSquare =
						new Vector2(squaresPlacement.worldPosition.X + ResourceManager.TILE_SIZE / 2f, squaresPlacement.worldPosition.Y + ResourceManager.TILE_SIZE / 2f);
				float x = base.activeSprite.Position.X - middleOfSquare.X;
				long t = ((long)(x * decimalPlaces));
				x = ((float)t / decimalPlaces);
				float y = base.activeSprite.Position.Y - middleOfSquare.Y;
				t = ((long)(y * decimalPlaces));
				y = ((float)t / decimalPlaces);
				if (x < moveDistance && x > 0) {
					moveDistance = x;
				} else if (-x < moveDistance && x < 0) {
					moveDistance = -x;
				} else if (y < moveDistance && y > 0) {
					moveDistance = y;
				} else if (-y < moveDistance && y < 0) {
					moveDistance = -y;
				}
				Vector2 newPos;
				if (base.direction == Direction.Up) {
					newPos = new Vector2(base.activeSprite.Position.X, base.activeSprite.Position.Y - moveDistance);
					//if (!CollisionManager.getInstance().wallCollisionFound(Helper.getBBox(newPos))) {
					base.activeSprite.Position = new Vector2(base.activeSprite.Position.X, base.activeSprite.Position.Y - moveDistance);
					//}
				} else if (base.direction == Direction.Right) {
					newPos = new Vector2(base.activeSprite.Position.X + moveDistance, base.activeSprite.Position.Y);
					//if (!CollisionManager.getInstance().wallCollisionFound(Helper.getBBox(newPos))) {
					base.activeSprite.Position = new Vector2(base.activeSprite.Position.X + moveDistance, base.activeSprite.Position.Y);
					//}
				} else if (base.direction == Direction.Down) {
					newPos = new Vector2(this.activeSprite.Position.X, this.activeSprite.Position.Y + moveDistance);
					//if (!CollisionManager.getInstance().wallCollisionFound(Helper.getBBox(newPos))) {
					base.activeSprite.Position = new Vector2(this.activeSprite.Position.X, this.activeSprite.Position.Y + moveDistance);
					//}
				} else if (this.direction == Direction.Left) {
					newPos = new Vector2(base.activeSprite.Position.X - moveDistance, base.activeSprite.Position.Y);
					//if (!CollisionManager.getInstance().wallCollisionFound(Helper.getBBox(newPos))) {
					base.activeSprite.Position = new Vector2(base.activeSprite.Position.X - moveDistance, base.activeSprite.Position.Y);
					//}
				}
				this.ring.updatePosition(base.activeSprite.Position);
			}
			// update our placement and bounding box
			base.Placement = new Placement(Placement.getIndex(base.activeSprite.Position));
			base.BoundingBox = Helper.getBBox(base.activeSprite.Position);
			if (base.previousPlacement.index != base.Placement.index) {
				AIManager.getInstane().Board[base.previousPlacement.index.Y, base.previousPlacement.index.X] = base.previousTypeOfSpace;
				base.previousTypeOfSpace = AIManager.getInstane().Board[base.Placement.index.Y, base.Placement.index.X];
				AIManager.getInstane().Board[base.Placement.index.Y, base.Placement.index.X] = PathFinder.TypeOfSpace.Unwalkable;
			}
			Point endNode = AIManager.getInstane().findEndNode();
			if (this.closestsPoint == endNode) {
				StateManager.getInstance().CurrentGameState = StateManager.GameState.GameOver;
				StateManager.getInstance().TypeOfGameOver = StateManager.GameOverType.Guards;
				if (ResourceManager.PLAY_SOUND) {
					ResourceManager.getInstance().PrisonCellSfx.Play();
				}
			}
			base.updateMove(elapsed);
		}

		public new void render(SpriteBatch spriteBatch) {
			this.ring.render(spriteBatch);
#if DEBUG
			spriteBatch.Draw(ResourceManager.getInstance().DebugChip, new Placement(this.destinationWayPoint).worldPosition, Color.Green);
#endif
			
			base.render(spriteBatch);
		}
		#endregion Support methods

		#region Destructor
		public new void dispose() {
			if (this.ring != null) {
				this.ring.dispose();
			}
			this.RunAIThread = false;
			this.AIThread.Abort();
			base.dispose();
		}
		#endregion Destructor
	}
}