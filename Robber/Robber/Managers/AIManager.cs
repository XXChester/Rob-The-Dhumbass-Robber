using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using GWNorthEngine.AI.AStar;
namespace Robber {
	public class AIManager {
		#region Class variables
		// singleton instance
		private static AIManager instance = new AIManager();
		private PathFinder pathFinder;
		private Thread thread;
		private bool running;
		private List<PathRequest> pathRequests;
		#endregion Class variables

		#region Class properties
		public PathFinder.TypeOfSpace[,] Board { get; set; }
		public List<Point> WayPoints { get; set; }
		public bool PlayerDetected { get; set; }
		#endregion Class properties

		#region Constructor
		public AIManager() {
			this.running = true;
			this.pathRequests = new List<PathRequest>();
			this.thread = new Thread(new ThreadStart(requestProcessor));
			this.thread.Start();
		}
		#endregion Constructor

		#region Support methods
		public static AIManager getInstance() {
			return instance;
		}

		private void requestProcessor() {
			do {
				Thread.Sleep(10);

				if (this.pathRequests.Count >= 1) {
					// clone the current requests
					Queue<PathRequest> clonedRequests = null;
					lock (this.pathRequests) {
						clonedRequests = new Queue<PathRequest>(this.pathRequests);
					}

					// process the requests
					PathRequest request = null;
					Stack<Point> path = null;
					List<int> processedRequests = new List<int>();
					for (int i = 0; i < clonedRequests.Count; i++) {
						request = clonedRequests.Dequeue();
						if (request != null) {
							path = new Stack<Point>(findPath(request.Start, request.End));
							request.CallBack.Invoke(path);
						}
						processedRequests.Add(i);
					}

					// remove the requests from the master list so we do not run them again
					for (int i = processedRequests.Count - 1; i >= 0; i--) {
						this.pathRequests.RemoveAt(processedRequests[i]);
					}
				}
			} while (this.running);
		}

		private List<Point> findPath(Point start) {
			Point end = findEndNode();
			return findPath(start, end);
		}

		private List<Point> findPath(Point start, Point end) {
			// backup the board first
			PathFinder.TypeOfSpace previousStartSpot = this.Board[start.Y, start.X];
			PathFinder.TypeOfSpace previousEndSpot = this.Board[end.Y, end.X];
			this.Board[start.Y, start.X] = PathFinder.TypeOfSpace.Start;
			this.Board[end.Y, end.X] = PathFinder.TypeOfSpace.End;
			List<Point> path = new List<Point>();
			this.pathFinder.findPath(this.Board);
			path = this.pathFinder.Path;
			// reset our pieces back
			this.Board[start.Y, start.X] = previousStartSpot;
			this.Board[end.Y, end.X] = previousEndSpot;
			return path;
		}

		public void init(int height, int width) {
			this.pathFinder = new MazeSolver(height, width);
			this.PlayerDetected = false;
		}

		public void requestPath(Point start, PathRequest.FoundPathCallBack callBack) {
			requestPath(start, findEndNode(), callBack);
		}

		public void requestPath(Point start, Point end,  PathRequest.FoundPathCallBack callBack) {
			lock (this.pathRequests) {
				this.pathRequests.Add(new PathRequest(start, end, callBack));
			}
		}

		public Point getNextWayPoint(Point currentWayPoint, Guard.MovementDirection movementDirection) {
			int index = this.WayPoints.IndexOf(currentWayPoint);
			// Sense you can specify a starting point that is NOT a waypoint we need to account for that
			if (index == -1) {
				if (movementDirection == Guard.MovementDirection.Clockwise) {
					index = 0;
				} else {
					index = this.WayPoints.Count;
				}
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

		public Point findEndNode() {
			Point end = new Point();
			for (int y = 0; y <= this.Board.GetUpperBound(0); y++) {
				for (int x = 0; x <= this.Board.GetUpperBound(1); x++) {
					if (this.Board[y, x] == PathFinder.TypeOfSpace.End) {
						end = new Point(x, y);
						break;
					}
				}
			}
			return end;
		}
		#endregion Support methods

		#region Destructor
		~AIManager() {
			dispose();
		}

		public void dispose() {
			this.running = false;
			this.thread.Abort();
		}
		#endregion Destructor
	}
}
