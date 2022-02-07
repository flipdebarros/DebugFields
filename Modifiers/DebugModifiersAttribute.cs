using UnityEditor;
using UnityEngine;

namespace Utils.DebugFields {
public abstract class DebugModifierAttribute : PropertyAttribute {
	public string ModifyField { get; }

	protected DebugModifierAttribute(string fieldName) {
		ModifyField = fieldName;
	}

	public abstract void OnGUI(SerializedProperty property);
}

}