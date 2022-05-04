using UnityEditor;
using UnityEngine;

public class DrawBox2DAttribute : DrawFieldAttribute {
	
	private enum Pivot {
		TopLeft, TopCenter, TopRight,
		CenterLeft, Center, CenterRight,
		BottomLeft, BottomCenter, BottomRight,
		Custom
	}
	
	protected override string drawerTypeName => "Box 2D";
	protected override string fieldName => fieldType switch {
		FieldType.Rect => "Rect",
		FieldType.Vector => "Size",
		_ => ""
	};
	protected override FieldType allowedTypes => FieldType.Rect | FieldType.Vector2Int | FieldType.Vector2Float;

	public bool dotted;

	private float _thickness = 1f;
	private float _screenSpaceSize = 2f;
	private Vector2 _offset;

	private Vector2 _customPivot;
	public Vector2 customPivot {
		get => _customPivot;
		set => _customPivot = new Vector2(
			Mathf.Clamp(value.x, -1f, 1f), 
			Mathf.Clamp(value.y, -1f, 1f));
	}
	private Vector2 _rawPivot;
	private Vector2 pivot => Vector2.Scale(_rawPivot, _box.size) * 0.5f;
	private Pivot _pivotValue = Pivot.Center;
	private Pivot PivotValue {
		get => _pivotValue;
		set{
			_rawPivot = value switch {
				Pivot.TopLeft => Vector2.up + Vector2.left,
				Pivot.TopCenter => Vector2.up,
				Pivot.TopRight => Vector2.up + Vector2.right,
				Pivot.CenterLeft => Vector2.left,
				Pivot.Center => Vector2.zero,
				Pivot.CenterRight => Vector2.right,
				Pivot.BottomLeft => Vector2.down + Vector2.left,
				Pivot.BottomCenter => Vector2.down,
				Pivot.BottomRight => Vector2.down + Vector2.right,
				Pivot.Custom => customPivot,
				_ => Vector2.zero
			};

			_pivotValue = value;
		}
	}

	private Rect _box;
	protected override void OnDraw() {
	    Vector2 offset;
	    Vector2 size;

	    switch (fieldType) {
		    case FieldType.Rect:
			    var rect = GetRectValue();
			    offset = rect.position;
			    size = rect.size;
			    break;
		    case FieldType.Vector:
			    offset = _offset;
			    size = GetVector3Value();
			    break;
		    default: return;
	    }
	    
		_box = new Rect( offset, size);
	    var pos = LocalToWorldPoint(offset - pivot);

	    var up = LocalToWorldVector(Vector2.up * size.y * 0.5f);
	    var right = LocalToWorldVector(Vector2.right * size.x * 0.5f);

	    Vector3 upperRight = pos + up + right;
	    Vector3 upperLeft = pos + up - right;
	    Vector3 lowerRight = pos - up + right;
	    Vector3 lowerLeft = pos - up - right ;

	    var upperEdge = new[] { upperLeft, upperRight };
	    var rightEdge = new[] { upperRight, lowerRight };
	    var leftEdge = new[] { upperLeft, lowerLeft };
	    var lowerEdge = new[] { lowerLeft, lowerRight };
	    

	    if (!dotted) {
		    Handles.DrawAAPolyLine(_thickness * 2f, upperEdge);
		    Handles.DrawAAPolyLine(_thickness * 2f, rightEdge);
		    Handles.DrawAAPolyLine(_thickness * 2f, leftEdge);
		    Handles.DrawAAPolyLine(_thickness * 2f, lowerEdge);
	    } else {
		    Handles.DrawDottedLines(upperEdge , _screenSpaceSize);
		    Handles.DrawDottedLines(rightEdge , _screenSpaceSize);
		    Handles.DrawDottedLines(leftEdge , _screenSpaceSize);
		    Handles.DrawDottedLines(lowerEdge , _screenSpaceSize);
	    }
    }
    
    protected override void OnHandles() {
	    var center = LocalToWorldPoint(_box.position);
	    var posHandle = (Vector2) Handles.FreeMoveHandle(center, Quaternion.identity,
		    HandleUtility.GetHandleSize(center) * 0.07f, Vector3.one, Handles.CircleHandleCap);
	    if (!posHandle.Approximately(center)) {
		    SetOffset(WorldToLocalPoint(posHandle));
		    return;
	    }
	    
	    var pos = LocalToWorldPoint(_box.position - pivot);
	    var up = LocalToWorldVector(Vector2.up * _box.size.y * 0.5f);
	    var right = LocalToWorldVector(Vector2.right * _box.size.x * 0.5f);
	    
	    if(!Mathf.Approximately(_rawPivot.y, 1f))
			DrawSizeHandle(pos + up, Vector2.up);
	    if(!Mathf.Approximately(_rawPivot.y, -1f)) 
		    DrawSizeHandle(pos - up, -Vector2.up);
	    if(!Mathf.Approximately(_rawPivot.x, 1f)) 
		    DrawSizeHandle(pos + right, Vector2.right);
	    if(!Mathf.Approximately(_rawPivot.x, -1f)) 
		    DrawSizeHandle(pos - right, -Vector2.right);
    }

    private void DrawSizeHandle(Vector2 pos, Vector2 direction) {
	    var handle = (Vector2) Handles.FreeMoveHandle(pos, Quaternion.identity, 
		    HandleUtility.GetHandleSize(pos) * 0.1f, Vector3.one, Handles.SphereHandleCap);
	    if (handle.Approximately(pos)) return;
	    
	    var change = Vector2.Scale(direction, WorldToLocalVector(handle - pos));
	    SetSize(_box.size + change);
    }

    private void SetSize(Vector2 vec) {
	    switch (fieldType) {
		    case FieldType.Vector: SetValue(vec); 
			    break;
		    case FieldType.Rect:
			    var rect = _box;
			    rect.size = vec;
			    SetValue(rect);
			    break;
	    }
    }
    
    private void SetOffset(Vector2 vec) {
	    switch (fieldType) {
		    case FieldType.Vector: _offset = vec; 
			    break;
		    case FieldType.Rect:
			    var rect = _box;
			    rect.position = vec;
			    SetValue(rect);
			    break;
	    }
    }

    protected override void OnSettingsGUI() {
	    PivotValue = (Pivot) EditorGUILayout.EnumPopup("Pivot", PivotValue);
	    customPivot = PivotValue is Pivot.Custom ? EditorGUILayout.Vector2Field("Custom Pivot", customPivot) : _rawPivot;

	    dotted = EditorGUILayout.Toggle("Dotted", dotted);
	    color = EditorGUILayout.ColorField("Color", color);

	    if (fieldType is FieldType.Vector) {
		    _offset = EditorGUILayout.Vector3Field("Offset", _offset);
	    }

	    if (!dotted) {
		    EditorGUILayout.LabelField("Thickness");
		    _thickness = EditorGUILayout.Slider(_thickness, 0.1f, 2f);
	    } else {
		    EditorGUILayout.LabelField("Size");
		    _screenSpaceSize = EditorGUILayout.Slider(_screenSpaceSize, 0.5f, 5f);
	    }
    }
}
