using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utils.DebugFields {

[CustomPropertyDrawer(typeof(DrawFieldAttribute), true)]
public class FieldDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		if (attribute is not DrawFieldAttribute field) return;

		label.text += " (" + field.FieldType + ")";

		using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
			EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(property, new GUIContent(field.PropertyName)); 
			foreach (var modifier in DrawModifiers(property)) {
				var (attr, prop) = modifier;
				attr.OnGUI(prop);
			}
		}
	}

	private List<(DebugModifierAttribute, SerializedProperty)> DrawModifiers (SerializedProperty property) {
		var targetType = property.serializedObject.targetObject.GetType();
		var fields = targetType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

		return ( 
			from field in fields 
			let att = field.GetCustomAttribute<DebugModifierAttribute>() 
			where att != null && att.ModifyField.Equals(property.name) 
			select (att, property.serializedObject.FindProperty(field.Name))).ToList();
	}
}

}