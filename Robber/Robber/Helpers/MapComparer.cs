using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robber {
	public class MapComparer : Comparer<string> {

		public override int Compare(string x, string y) {
			if (x == null || y == null) {
				throw new ArgumentException("Arguments cannot be null");
			}
			// first strip the "Map" text from the string than convert the to int
			int tempX = int.Parse(x.Replace("Map", ""));
			int tempY = int.Parse(y.Replace("Map", ""));

			// reverse sort
			int result = 1;
			if (tempX == tempY) {
				result = 0;
			} else if (tempX > tempY) {
				result = -1;
			}

			return result;
		}
	}
}
