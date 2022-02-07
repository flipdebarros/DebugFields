using System;
using UnityEngine;
using UnityEditor;

namespace Utils.DebugFields {

public class DrawDottedBoxAttribute : DrawFieldAttribute {
    public float screenSpaceSize = 2.0f;

    public override string FieldType => "Dotted Box";
    public override string PropertyName => "Rect";

    public DrawDottedBoxAttribute() {}
    public DrawDottedBoxAttribute(float r, float g, float b, float a = 1f) : base(r, g, b, a) {}
    
    private Rect GetValue(object value) => value switch {
        Rect rect => rect,
        RectInt rect => new Rect(rect.x, rect.y, rect.width, rect.height),
        _ => Rect.zero
    };

    public override void Draw (object value, Vector2 position) {
        if (value is not Rect or RectInt) {
            Debug.LogWarning("DrawDottedBox attribute only considers Rect type fields. Field will be ignored");
            return;
        }

        var box = GetValue(value);
        
        box.position += position - box.size / 2f;

        var tr = new Vector3(box.xMin, box.yMax);
        var tl = new Vector3(box.xMax, box.yMax);
        var br = new Vector3(box.xMin, box.yMin);
        var bl = new Vector3(box.xMax, box.yMin);
        
        Handles.DrawDottedLine(tr, tl, screenSpaceSize);
        Handles.DrawDottedLine(tr, br, screenSpaceSize);
        Handles.DrawDottedLine(tl, bl, screenSpaceSize);
        Handles.DrawDottedLine(br, bl, screenSpaceSize);
    }
}

}

