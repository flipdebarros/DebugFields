using System;
using JetBrains.Annotations;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class)]
[BaseTypeRequired(typeof(MonoBehaviour))]
public class DebugFieldsAttribute : Attribute {

	private string _name;

	public DebugFieldsAttribute(string name) {
		_name = name;
	}

	public string GetName() => _name;

}
