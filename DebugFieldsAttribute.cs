using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utils.DebugFields {

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DebugFieldsAttribute : Attribute {

	private Color _color = Color.green;
	private Vector2 _offset;
	
	public void Draw(MonoBehaviour obj) {

		var gameObject = obj.gameObject;

		_offset = gameObject.transform.position;
		
		foreach (var attribute in obj.GetType().GetCustomAttributes(typeof(DebugModifierAttribute), false)) {
			switch (attribute) {
				case SetOffsetAttribute att:
					_offset += att.offset;
					break;
				case SetColorAttribute att:
					_color = att.color;
					break;
			}
		}

		var fields = obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(
			f => f.IsDefined(typeof(DrawFieldAttribute)));

		Handles.color = _color;
		
		foreach (var field in fields) {
			var drawer = field.GetCustomAttribute(typeof(DrawFieldAttribute)) as DrawFieldAttribute;

			var position = _offset;

			var settings = field.GetCustomAttributes(typeof(DebugModifierAttribute));
			foreach (var attribute in settings) {
				switch (attribute) {
					case SetOffsetAttribute att:
						position += att.offset;
						break;
					case SetColorAttribute att:
						Handles.color = att.color;
						break;
				}
			}
			drawer?.Draw(field.GetValue(obj), position);

			Handles.color = _color;
		}

		Handles.color = Color.white;
	}
	
}

}
