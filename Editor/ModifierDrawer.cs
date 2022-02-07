using UnityEditor;
using UnityEngine;

namespace Utils.DebugFields {

[CustomPropertyDrawer(typeof(DebugModifierAttribute), true)]
public class ModifierDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) { }
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return 0f;
	}
}

}