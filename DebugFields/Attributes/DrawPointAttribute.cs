using UnityEditor;
using UnityEngine;

public class DrawPointAttribute : DrawFieldAttribute {
	protected override string drawerTypeName => "Point";
	protected override string fieldName => "Position";
	protected override FieldType allowedTypes => FieldType.Vector3Float | FieldType.Vector3Int;

	protected override void OnDraw() { }
	
	protected override void OnHandles() {
		var pos = LocalToWorldPoint(GetVector3Value());
		var point = Handles.PositionHandle(pos, Quaternion.identity);
		SetValue(WorldToLocalPoint(point));
	}

	protected override void OnSettingsGUI() {
		color = Color.white;
	}
}
