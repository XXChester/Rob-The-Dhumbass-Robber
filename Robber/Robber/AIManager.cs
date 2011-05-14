using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GWNorthEngine.AI.AStar;
namespace Robber {
	public class AIManager {
		#region Class variables
		// singleton instance
		private static AIManager instance = new AIManager();
		private PathFinder pathFinder;
		#endregion Class variables

		#region Class properties
		public PathFinder.TypeOfSpace[,] Board { get; set; }
		public List<Point> WayPoints { get; set; }
		public bool PlayerDetected { get; set; }
		#endregion Class properties

		#region Support methods
		public static AIManager getInstane() {
			return instance;
		}

		public void init(int height, int width) {
			this.pathFinder = new MazeSolver(height, width);
			this.PlayerDetected = false;
		}

		public void updatePlayerPosition(Point oldPosition, Point newPosition) {
			// If I was detected I need to update where I am
			if (this.PlayerDetected == true && oldPosition != newPosition) {
				this.Board[oldPosition.Y, oldPosition.X] = PathFinder.TypeOfSpace.Walkable;
				this.Board[newPosition.Y, newPosition.X] = PathFinder.TypeOfSpace.End;
			}
		}

		public Point getNextWayPoint(Point currentWayPoint, Guard.MovementDirection movementDirection) {
			int index = this.WayPoints.IndexOf(currentWayPoint);
			// Sense you can specify a starting point that is NOT a waypoint we need to account for that
			if (index == -1) {
				// send him to a random waypoint
				index = new Random().Next(this.WayPoints.Count);
			}
			if (movementDirection == Guard.MovementDirection.Clockwise) {
				if (index == this.WayPoints.Count - 1) {
					index = 0;
				} else {
					index++;
				}
			} else {
				if (index == 0) {
					index = this.WayPoints.Count - 1;
				} else {
					index--;
				}
			}
			return this.WayPoints[index];
		}

		public List<Point> findPath(Point start) {
			// find our end point
			Point end = new Point();
			for (int y = 0; y < this.Board.GetUpperBound(0); y++) {
				for (int x = 0; x < this.Board.GetUpperBound(1); x++) {
					if (this.Board[y, x] == PathFinder.TypeOfSpace.End) {
						end = new Point(x, y);
						break;
					}
				}
			}
			return findPath(start, end);
		}

		public List<Point> findPath(Point start, Point end) {
			// backup the board first
			PathFinder.TypeOfSpace previousStartSpot = this.Board[start.Y, start.X];
			PathFinder.TypeOfSpace previousEndSpot = this.Board[end.Y, end.X];
			this.Board[start.Y, start.X] = PathFinder.TypeOfSpace.Start;
			this.Board[end.Y, end.X] = PathFinder.TypeOfSpace.End;
			List<Point> path = new List<Point>();
			this.pathFinder.findPath(this.Board);
			//if (this.pathFinder.End == end) {
				path = this.pathFinder.Path;
			//}
			// reset our pieces back
			this.Board[start.Y, start.X] = previousStartSpot;
			this.Board[end.Y, end.X] = previousEndSpot;
			return path;
		}
		#endregion Support methods
	}
}
