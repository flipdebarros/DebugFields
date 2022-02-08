using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utils.DebugFields {

[CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
[CanEditMultipleObjects]
public class DebugFieldsDrawer : Editor {

	private bool _enabled = true;
	private DebugFieldsAttribute _attribute;

	private void OnEnable () {
		_attribute = target.GetType().GetCustomAttribute<DebugFieldsAttribute>();
		if (_attribute == null) return;

		_attribute.target = target as MonoBehaviour;
		var fields = target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

		_attribute.fields = (from field in fields
			where field.IsDefined(typeof(DrawFieldAttribute))
			select field).ToArray();

		var modifiers = new Dictionary<string, List<FieldInfo>>();
		
		foreach (var field in fields) {
			if (!field.IsDefined(typeof(DebugModifierAttribute))) continue;
			var modifiedField = field.GetCustomAttribute<DebugModifierAttribute>().ModifyField;
			if (modifiers.ContainsKey(modifiedField))
				modifiers[modifiedField].Add(field);
			else
				modifiers.Add(modifiedField, new List<FieldInfo> {field});
		}

		_attribute.modifiers = modifiers;
	}
	
	private void OnSceneGUI () {
		if(!_enabled || _attribute == null) return;
		_attribute.Draw();
	}

	public override void OnInspectorGUI () {
		if(_attribute != null) {
			using var check = new EditorGUI.ChangeCheckScope();
			_enabled = GUILayout.Toggle(_enabled, "Debug Fields");
			if (check.changed) SceneView.RepaintAll();
		}

		base.OnInspectorGUI();
	}

}

}
