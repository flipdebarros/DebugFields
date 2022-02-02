using UnityEngine;

namespace Utils.DebugFields {
public class SetColorAttribute : DebugModifierAttribute {

	public Color color;

	public SetColorAttribute(float r, float g, float b, float a = 1f) {
		color = new Color(r, g, b, a);
	}

}

}
