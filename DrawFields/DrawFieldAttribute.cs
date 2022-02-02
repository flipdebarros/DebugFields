using System;
using UnityEngine;

namespace Utils.DebugFields {

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, 
	AllowMultiple = true)]
public abstract class DrawFieldAttribute : Attribute {
	public abstract void Draw(object value, Vector2 position);
}

}