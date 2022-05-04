using UnityEditor;
using UnityEngine;

public class DrawCircle2DAttribute : DrawFieldAttribute {
	protected override string drawerTypeName => "Circle 2D";
	protected override string fieldName => "Radius";
	protected override FieldType allowedTypes => FieldType.Numerical;
	
	private float _thickness = 1f;
	private Vector3 _offset;

	protected override void OnDraw() {
	    var pos = LocalToWorldPoint(_offset);
	    var radius = GetFloatValue();

	    Handles.DrawWireDisc(pos, Vector3.forward, radius, _thickness);
	}
    
    protected override void OnHandles() {
	    var pos = LocalToWorldPoint(_offset);
	    var radius = GetFloatValue();
	    
	    SetValue(Handles.RadiusHandle(rotation, pos, radius, true));
    }
    protected override void OnSettingsGUI() {
	    color = EditorGUILayout.ColorField("Color", color);
	    _offset = EditorGUILayout.Vector3Field("Offset", _offset);
	    EditorGUILayout.LabelField("Thickness");
	    _thickness = EditorGUILayout.Slider(_thickness, 0.1f, 2f);
    }
}
