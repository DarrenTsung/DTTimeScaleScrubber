using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTTimeScaleScrubber.Internal {
	public static class ScreenUtil {
		public static Vector2 ScreenToViewportPoint(Vector2 screenPoint) {
			return new Vector2(screenPoint.x / (float)Screen.width, screenPoint.y / (float)Screen.height);
		}
	}
}