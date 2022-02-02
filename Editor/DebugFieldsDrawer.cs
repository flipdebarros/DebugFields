using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utils.DebugFields {
public class DebugFieldsDrawer : Editor {

	private void OnSceneGUI () {
		
		if(target.GetType().GetCustomAttribute(typeof(DebugFieldsAttribute))is DebugFieldsAttribute debug) 
			debug.Draw(target as MonoBehaviour);

	}

}

}
