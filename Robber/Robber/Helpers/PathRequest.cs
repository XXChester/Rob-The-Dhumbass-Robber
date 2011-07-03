using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace Robber {
	public class PathRequest {
		#region Class variables
		public delegate void FoundPathCallBack(Stack<Point> path);
		#endregion Class variables

		#region Class properties
		public FoundPathCallBack CallBack { get; set; }
		public Point Start { get; set; }
		public Point End { get; set; }
		#endregion Class properties

		#region Constructor
		public PathRequest(Point start, Point end, FoundPathCallBack callBack) {
			this.Start = start;
			this.End = end;
			this.CallBack = callBack;
		}
		#endregion Constructor
	}
}
