using UnityEditor;
using UnityEngine;

namespace Utils.DebugFields {

public class SetOffsetAttribute : DebugModifierAttribute {
	public SetOffsetAttribute (string fieldName) : base(fieldName) { }

	public override void OnGUI(SerializedProperty property) {
		if (property.propertyType != SerializedPropertyType.Vector2) {
			Debug.LogError("SetOffset Attribute has to be on a Vector2 to modify another variable");
			return;
		}
		property.vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Offset"), property.vector2Value);
	}
}

}