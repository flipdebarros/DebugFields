using System;

namespace Utils.DebugFields {

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | 
                AttributeTargets.Property, AllowMultiple = true)]
public abstract class DebugModifierAttribute : Attribute {
	
}

}