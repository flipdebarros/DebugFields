using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool("Debug Fields")]
public class DebugFieldsTool  : EditorTool {

	private const string displayName = "Debug Fields";

	private List<GameObject> _gameObjects;
	private List<DebugObject> _objects;

	private class DebugObject {
		public Transform transform;
		public MonoBehaviour behaviour;
		public SerializedObject serializedObject;
		public bool active;
		public string name;
		public List<Field> fields;

		public DebugObject(MonoBehaviour monoBehaviour) {
			var gameObjectName = monoBehaviour.gameObject.name; 
			transform = monoBehaviour.transform;
			behaviour = monoBehaviour;
			serializedObject = new SerializedObject(monoBehaviour);
			active = true;

			var type = monoBehaviour.GetType();
			var attr = type.GetCustomAttribute<DebugFieldsAttribute>();
			var attrName = attr.GetName();
			name = gameObjectName + " ( " + attrName + " )";
			
			
			fields = (from fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				where fieldInfo.IsDefined(typeof(DrawFieldAttribute), true)
				select new Field(fieldInfo, behaviour, serializedObject)).ToList();

		}
		
		public void DrawObject() {
			if(!active) return;
			serializedObject.Update();
			fields.ForEach(f => f.DrawField());
			serializedObject.ApplyModifiedProperties();
		}

	}
	private class Field {
		public bool active;
		public FieldInfo info;
		public DrawFieldAttribute attribute;
		
		public bool settingsFoldout;

		public Field(FieldInfo fieldInfo, MonoBehaviour behaviour, SerializedObject serializedObject) {
			info = fieldInfo;
			active = true;

			var property = serializedObject.FindProperty(info.Name);
			attribute = info.GetCustomAttribute<DrawFieldAttribute>();
			attribute.Init(behaviour, info, property);
		}

		public void DrawField() {
			if (!active) return; 
			attribute.Draw();
		}
	}

	private void OnEnable() {
		_objects = GetObjects() ?? new List<DebugObject>();
		Selection.selectionChanged += OnSelectionChanged;
	}
	private void OnDisable() {
		Selection.selectionChanged -= OnSelectionChanged;
	}
	private void OnSelectionChanged() {
		_objects = GetObjects() ?? _objects;
		SceneView.RepaintAll();
	}
	
	private static List<DebugObject> GetObjects() {
		if (Selection.gameObjects.Length == 0) return null;

		return (from gameObject in Selection.gameObjects
			from monoBehaviour in gameObject.GetComponents<MonoBehaviour>()
			where monoBehaviour.GetType().IsDefined(typeof(DebugFieldsAttribute))
			select new DebugObject(monoBehaviour)).ToList();

	}

	private bool _foldTool;
	private Vector2 _scrollPos;
	public override void OnToolGUI(EditorWindow window) {
		if(_objects.Count <= 0) return;

		_objects.ForEach(obj => obj.DrawObject());
		
		Handles.BeginGUI();

		if (_foldTool) {
			using var scope = new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.MinWidth(250));
			_foldTool = EditorGUILayout.Foldout(_foldTool, displayName, true);
		}
		else {
			using var scrollView =
				new EditorGUILayout.ScrollViewScope(_scrollPos, EditorStyles.helpBox, GUILayout.MinWidth(250));
			_scrollPos = scrollView.scrollPosition;

			_foldTool = EditorGUILayout.Foldout(_foldTool, displayName, true);

			using var check = new EditorGUI.ChangeCheckScope();
			_objects.ForEach(DrawObjectInspection);
			if (check.changed) SceneView.RepaintAll();
		}

		Handles.EndGUI();
		
	}

	private static void DrawObjectInspection(DebugObject debugObject) {
		debugObject.serializedObject.Update();
		
		debugObject.active = EditorGUILayout.ToggleLeft(debugObject.name, debugObject.active);
		
		using (new GUILayout.VerticalScope(EditorStyles.helpBox)) {
			using var disable = new EditorGUI.DisabledScope(!debugObject.active);
			debugObject.fields.ForEach(DrawPropertyInspection);
		}
			
		EditorGUILayout.Space(10, true);
		debugObject.serializedObject.ApplyModifiedProperties();
	}
	private static void DrawPropertyInspection(Field field) {
		using (new GUILayout.HorizontalScope()) {
			field.settingsFoldout = EditorGUILayout.Foldout(field.settingsFoldout, field.attribute.fieldDisplayName, true);
			field.active = EditorGUILayout.Toggle(field.active, GUILayout.Width(20), GUILayout.ExpandWidth(false));
		}

		if (!field.settingsFoldout) return;
		
		using (new EditorGUI.DisabledScope(!field.active))
			field.attribute.OnGUI();
	}

}
