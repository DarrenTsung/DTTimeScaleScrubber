using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTTimeScaleScrubber.Internal {
	public static class IEnumerableGenericExtensions {
		public static Vector2 Average(this IEnumerable<Vector2> enumerable) {
			int count = 0;
			Vector2 sum = Vector2.zero;

			foreach (Vector2 element in enumerable) {
				sum += element;
				count++;
			}

			return sum / count;
		}
	}
}