using UnityEngine;
using UnityEditor;

namespace Utils.DebugFields {

public class DrawDottedBoxAttribute : DrawFieldAttribute {
    public float screenSpaceSize = 2.0f;
    
    public override void Draw (object value, Vector2 position) {
        if (value is not Rect box) return;

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

