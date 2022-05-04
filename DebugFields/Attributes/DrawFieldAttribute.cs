using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public abstract class DrawFieldAttribute : PropertyAttribute {

	[Flags]
	protected enum FieldType {
		Int = 1,
		Float = (1 << 1),
		RectInt = (1 << 2),
		RectFloat = (1 << 3),
		Vector2Int = (1 << 4),
		Vector3Int = (1 << 5),
		Vector2Float = (1 << 6),
		Vector3Float = (1 << 7),

		Numerical = Int | Float,
		Rect = RectInt | RectFloat,
		Vector = Vector2Int | Vector2Float | Vector3Int | Vector3Float
	}

	private MonoBehaviour _behaviour;
	private Transform _transform;
	private FieldInfo _info;
	private SerializedProperty _property;

	protected Vector3 position => _transform.position;
	protected Quaternion rotation => inheritTransform ? _transform.rotation : Quaternion.identity;
	protected Vector3 localScale => inheritTransform ? _transform.localScale : Vector3.one;

	public string fieldDisplayName => _property.displayName + " (" + drawerTypeName + ")";
	protected abstract string drawerTypeName { get; }
	protected abstract string fieldName { get; }
	protected abstract FieldType allowedTypes { get; }
	
	private FieldType _fieldType = 0;
	protected FieldType fieldType {
		get{
			if ((_fieldType & FieldType.Numerical) != 0) return FieldType.Numerical;
			if ((_fieldType & FieldType.Rect) != 0) return FieldType.Rect;
			if ((_fieldType & FieldType.Vector) != 0) return FieldType.Vector;
			return 0;
		}
	}
	
	public bool inheritTransform;
	protected Color color = Color.green;

	public void Init(MonoBehaviour behaviour, FieldInfo info, SerializedProperty property) {
		_behaviour = behaviour;
		_transform = behaviour.transform;
		_property = property;
		_info = info;
		
		if(!ValidType(_property.propertyType))
			Debug.LogError("Drawer in field " + _property.name + " is not allowed with this type.");

		color = Random.ColorHSV(0f, 1f, 0.9f, 1f, 0.9f, 1f);
	}
	public bool ValidType(SerializedPropertyType type) {
		_fieldType = type switch {
			SerializedPropertyType.Integer => FieldType.Int,
			SerializedPropertyType.Float => FieldType.Float,
			SerializedPropertyType.Vector2 => FieldType.Vector2Float,
			SerializedPropertyType.Vector3 => FieldType.Vector3Float,
			SerializedPropertyType.Rect => FieldType.RectFloat,
			SerializedPropertyType.Vector2Int => FieldType.Vector2Int,
			SerializedPropertyType.Vector3Int => FieldType.Vector3Int,
			SerializedPropertyType.RectInt => FieldType.RectInt,
			_ => 0
		};

		return (_fieldType & allowedTypes) != 0;
	}


	public void Draw() {
		Handles.color = color;
		OnDraw();
		OnHandles();
		Handles.color = Color.white;
	}
	protected abstract void OnDraw();
	protected abstract void OnHandles();
	public void OnGUI() {
		EditorGUILayout.PropertyField(_property, new GUIContent(fieldName), true);
		OnSettingsGUI();
	}
	protected abstract void OnSettingsGUI();

	protected void SetValue(float value) {
		switch (_fieldType) {
			case FieldType.Int:
				_property.intValue = Mathf.RoundToInt(value);
				break;
			case FieldType.Float:
				_property.floatValue = value;
				break;
		}
	}
	protected void SetValue(Vector3 value) {
		switch (_fieldType) {
			case FieldType.Vector2Float:
				_property.vector2Value = value;
				break;
			case FieldType.Vector3Float:
				_property.vector3Value = value;
				break;
			case FieldType.Vector2Int:
				_property.vector2IntValue = Vector2Int.RoundToInt(value);
				break;
			case FieldType.Vector3Int:
				_property.vector3IntValue = Vector3Int.RoundToInt(value);
				break;
		}
	}
	protected void SetValue(Rect value) {
		switch (_fieldType) {
			case FieldType.RectInt:
				_property.rectIntValue = value.RoundToInt();
				break;
			case FieldType.RectFloat:
				_property.rectValue = value;
				break;
		}
	}

	private object Value => _info.GetValue(_behaviour);
	protected float GetFloatValue() {
		return Value switch {
			float f => f,
			int f => f,
			uint f => f,
			_ => 0
		};
	}
	protected Vector3 GetVector3Value() {
		return Value switch {
			Vector2 v => v,
			Vector3 v => v,
			Vector2Int v => (Vector2) v,
			Vector3Int v => v,
			_ => Vector3.zero
		};
	}
	protected Rect GetRectValue() {
		return Value switch {
			Rect rect => rect,
			RectInt rect => rect.ToRect(),
			_ => Rect.zero
		};
	}

	//Position, scale and rotation
	protected Vector3 WorldToLocalPoint(Vector3 v) =>
		!inheritTransform ? v + position : _transform.worldToLocalMatrix * ((Vector4) v + new Vector4(0,0,0,1));
	protected Vector2 WorldToLocalPoint(Vector2 v) => WorldToLocalPoint((Vector3) v);
	//Position, scale and rotation
	protected Vector3 LocalToWorldPoint(Vector3 v) =>
		!inheritTransform ? v + position: _transform.localToWorldMatrix * ((Vector4) v + new Vector4(0,0,0,1));
	protected Vector2 LocalToWorldPoint(Vector2 v) => LocalToWorldPoint((Vector3) v);
	
	//Scale and rotation
	protected Vector3 WorldToLocalVector(Vector3 v) =>
		!inheritTransform ? v :  _transform.worldToLocalMatrix * v;
	protected Vector2 WorldToLocalVector(Vector2 v) => WorldToLocalVector((Vector3) v);
	//Scale and rotation
	protected Vector3 LocalToWorldVector(Vector3 v) =>
		!inheritTransform ? v : _transform.localToWorldMatrix * v;
	protected Vector2 LocalToWorldVector(Vector2 v) => LocalToWorldVector((Vector3) v);

	
	protected Vector3 WorldToLocalScale(Vector3 v) {
		var matrix = Matrix4x4.Scale(localScale).inverse;
		return !inheritTransform ? v : matrix * v;
	}
	protected Vector2 WorldToLocalScale(Vector2 v) => WorldToLocalScale((Vector3) v);
	
	protected Vector3 LocalToWorldScale(Vector3 v) => 
		!inheritTransform ? v : Vector3.Scale(v, _transform.localScale);
	protected Vector2 LocalToWorldScale(Vector2 v) => LocalToWorldScale((Vector3) v);
}

public static class RectExtensions {
	public static RectInt RoundToInt(this Rect rect) =>
		new(Vector2Int.RoundToInt(rect.position), Vector2Int.RoundToInt(rect.size));
	public static Rect ToRect(this RectInt rect) => new (rect.position, rect.size);
}

public static class VectorExtensions {
	public static bool Approximately(this Vector3 vec, Vector3 other) => Mathf.Approximately((vec - other).magnitude, 0f);
	public static bool Approximately(this Vector2 vec, Vector2 other) => Mathf.Approximately((vec - other).magnitude, 0f);
}