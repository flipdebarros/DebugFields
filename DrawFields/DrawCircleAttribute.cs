using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Utils.DebugFields {

public class DrawCircleAttribute : DrawFieldAttribute {
	public float thickness = 0.5f;

	private float GetValue(object value) => value switch {
		float f => f,
		int f => f,
		uint f => f,
		_ => 0f
	};
	
	public override string FieldType => "Circle";
	public override string PropertyName => "Radius";
	
	public DrawCircleAttribute() {}
	public DrawCircleAttribute(float r, float g, float b, float a = 1f) : base(r, g, b, a) {}
	
	public override void Draw (object value, Vector2 position) {
		if (value is not (float or int or uint)) {
			Debug.LogWarning("DrawCircle attribute only considers some numeric type fields. Field will be ignored");
			return;
		}
		
		Handles.DrawWireDisc(position, Vector3.forward, GetValue(value), thickness);
	}

}

}