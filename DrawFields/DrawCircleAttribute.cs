using UnityEngine;
using UnityEditor;

namespace Utils.DebugFields {

public class DrawCircleAttribute : DrawFieldAttribute {
	public float thickness = 0.0f;
	
	public override void Draw (object value, Vector2 position) {

		var radius = value switch {
			float f => f,
			int i => i,
			_ => 0
		};
		
		Handles.DrawWireDisc(position, Vector3.forward, radius, thickness);
	}

}

}