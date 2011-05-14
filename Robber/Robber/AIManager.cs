using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GWNorthEngine.AI.AStar;
namespace Robber {
	public class AIManager {
		// singleton instance
		private static AIManager instance = new AIManager();

		
		private PathFinder pathFinder;

		public PathFinder.TypeOfSpace[,] Board { get; set; }
		public List<Point> WayPoints { get; set; }

		public static AIManager getInstane() {
			return instance;
		}

		public void init(int height, int width) {
			this.pathFinder = new MazeSolver(height, width);
		}

		public Point getNextWayPoint(Point currentWayPoint, Guard.MovementDirection movementDirection) {
			int index = this.WayPoints.IndexOf(currentWayPoint);
			// Sense you can specify a starting point that is NOT a waypoint we need to account for that
			if (index == -1) {
				// send him to a random waypoint
				index = new Random().Next(this.WayPoints.Count);
				//index = 5;		//This will make the AI walk through certain walls
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

		public List<Point> findPath(Point start, Point end) {
			// backup the board first
			PathFinder.TypeOfSpace previousStartSpot = this.Board[start.Y, start.X];
			PathFinder.TypeOfSpace previousEndSpot = this.Board[end.Y, end.X];
			this.Board[start.Y, start.X] = PathFinder.TypeOfSpace.Start;
			this.Board[end.Y, end.X] = PathFinder.TypeOfSpace.End;
			List<Point> path = new List<Point>();
			this.pathFinder.findPath(this.Board);
			if (this.pathFinder.End == end) {
				path = this.pathFinder.Path;
			}
			// reset our pieces back
			this.Board[start.Y, start.X] = previousStartSpot;
			this.Board[end.Y, end.X] = previousEndSpot;
			return path;
		}
	}
}
