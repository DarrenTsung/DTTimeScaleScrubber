#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DTTimeScaleScrubber.Internal {
	public static class AssetDatabaseUtil {
		public static T LoadSpecificAssetNamed<T>(string assetName) where T : UnityEngine.Object {
			string searchString = string.Format("{0} t:{1}", assetName, typeof(T).Name);
			string[] guids = AssetDatabase.FindAssets(searchString);

			if (guids.Length <= 0) {
				Debug.LogError(string.Format("LoadSpecificAssetNamed: Can't find anything matching '{0}' anywhere in the project", searchString));
				return default(T);
			}

			if (guids.Length > 2) {
				Debug.LogWarning(string.Format("LoadSpecificAssetNamed: More than one asset found for '{0}' in the project!", searchString));
			}
			string guid = guids[0];
			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			return AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;
		}
	}
}
#endif