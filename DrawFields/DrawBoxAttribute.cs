using System;
using UnityEngine;
using UnityEditor;

namespace Utils.DebugFields {

public class DrawBoxAttribute : DrawFieldAttribute {
    public float thickness = 0.0f;

    public override string FieldType => "Box";
    public override string PropertyName => "Rect";

    public DrawBoxAttribute() {}
    public DrawBoxAttribute(float r, float g, float b) : base(r, g, b) {}

    private Rect GetValue(object value) => value switch {
        Rect rect => rect,
        RectInt rect => new Rect(rect.x, rect.y, rect.width, rect.height),
        _ => Rect.zero
    };
    
    public override void Draw (object value, Vector2 position) {
        if (value is not Rect or RectInt) {
            Debug.LogWarning("DrawBox attribute only considers Rect type fields. Field will be ignored");
            return;
        }
        
        var box = GetValue(value);

        box.position += position - box.size / 2f;

        var tr = new Vector3(box.xMin, box.yMax);
        var tl = new Vector3(box.xMax, box.yMax);
        var br = new Vector3(box.xMin, box.yMin);
        var bl = new Vector3(box.xMax, box.yMin);
        
        Handles.DrawLine(tr, tl, thickness);
        Handles.DrawLine(tr, br, thickness);
        Handles.DrawLine(tl, bl, thickness);
        Handles.DrawLine(br, bl, thickness);
    }
}

}
