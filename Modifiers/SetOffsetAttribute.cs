using UnityEngine;

namespace Utils.DebugFields {

public class SetOffsetAttribute : DebugModifierAttribute {

	public Vector2 offset;
	
	public SetOffsetAttribute (float x, float y) {
		offset = new Vector2(x, y);
	}
	
	
}

}