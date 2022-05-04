using UnityEditor;
using UnityEngine;

public class DrawLineAttribute : DrawFieldAttribute {

	public enum Direction {
		Right, Left,
		Up, Down,
		Forward, Backward,
		Custom
	}

	protected override string drawerTypeName => "Line";
	protected override string fieldName => fieldType switch {
		FieldType.Numerical => "Lenght",
		FieldType.Vector => "Size",
		_ => ""
	};
	protected override FieldType allowedTypes => FieldType.Numerical | FieldType.Vector;
	
	public bool dotted;

	private Vector3 _direction = Vector3.right;

	private Direction _directionValue = Direction.Right;
	private Direction DirectionValue {
		get => _directionValue;
		set{
			_direction = value switch {
				Direction.Right => Vector3.right,
				Direction.Left => Vector3.left,
				Direction.Up => Vector3.up,
				Direction.Down => Vector3.down,
				Direction.Forward => Vector3.forward,
				Direction.Backward => Vector3.back,
				_ => _direction
			};
			_directionValue = value;
		}
	}
	
	private float _thickness = 1f;
	private float _screenSpaceSize = 2f;
	private Vector3 _offset;

	private Vector3 _lineStart, _lineEnd;

	protected override void OnDraw() {
		
		_lineStart = LocalToWorldPoint(_offset);
		
		Vector3 lenght;
		switch (fieldType) {
			case FieldType.Numerical: lenght = GetFloatValue() * _direction.normalized; break;
			case FieldType.Vector: lenght = GetVector3Value(); break;
			default: return;
		}
		_lineEnd = _lineStart + LocalToWorldVector(lenght);

		if (!dotted) Handles.DrawAAPolyLine(_thickness * 2f, _lineStart, _lineEnd);
		else Handles.DrawDottedLine(_lineStart, _lineEnd, _screenSpaceSize);
	}

	protected override void OnHandles() {
		var startHandle = Handles.FreeMoveHandle(_lineStart, rotation, HandleUtility.GetHandleSize(_lineStart) * 0.05f, 
			Vector3.one, Handles.DotHandleCap);

		var endHandle = Handles.FreeMoveHandle(_lineEnd, Quaternion.identity, HandleUtility.GetHandleSize(_lineEnd) * 0.1f,
			Vector3.one, Handles.SphereHandleCap);
		
		var startPoint = WorldToLocalPoint(startHandle);
		var endPoint = WorldToLocalPoint(endHandle) - startPoint;

		if (!startHandle.Approximately(_lineStart)) _offset = startPoint;
		if (endHandle.Approximately(_lineEnd)) return;
		switch (fieldType) {
			case FieldType.Numerical:
				DirectionValue = Direction.Custom;
				_direction = endPoint.normalized;
				if(!Mathf.Approximately(endPoint.magnitude, GetFloatValue()))
					SetValue(endPoint.magnitude);
				break;
			case FieldType.Vector: SetValue(endPoint); break;
		}
	}


	protected override void OnSettingsGUI() {
		if (fieldType is FieldType.Numerical) {
			DirectionValue = (Direction) EditorGUILayout.EnumPopup("Direction", DirectionValue);
			_direction = DirectionValue is Direction.Custom
				? EditorGUILayout.Vector3Field("Custom Direction", _direction)
				: _direction;
		}
		dotted = EditorGUILayout.Toggle("Dotted", dotted);
		color = EditorGUILayout.ColorField("Color", color);
		
		_offset = EditorGUILayout.Vector3Field("Offset", _offset);

		if (!dotted) {
			EditorGUILayout.LabelField("Thickness");
			_thickness = EditorGUILayout.Slider(_thickness, 0.1f, 2f);
		} else {
			EditorGUILayout.LabelField("Size");
			_screenSpaceSize = EditorGUILayout.Slider(_screenSpaceSize, 0.5f, 5f);
		}
	}
}
