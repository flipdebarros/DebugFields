using UnityEngine;

namespace Utils.DebugFields {

public abstract class DrawFieldAttribute : PropertyAttribute {
	public Color color;
	public bool IsColorDefined { get; private set; }
	public abstract string FieldType { get; }
	public abstract string PropertyName { get; }

	protected DrawFieldAttribute() { }

	protected DrawFieldAttribute(float r, float g, float b, float a = 1f) {
		color = new Color(r, g, b, a);
		IsColorDefined = true;
	}
	
	public abstract void Draw(object value, Vector2 position);
}

}