using UnityEngine;
using Utils.DebugFields;

[DebugFields]
public class ExampleScript : MonoBehaviour {
    [SetOffset(nameof(circle))] 
    [SerializeField] private Vector2 circlePos;
    [DrawCircle]
    public float circle;

    [SetColor(nameof(box))]
    [SerializeField] private Color boxColor;
    [DrawBox(thickness = 2f)]
    public Rect box;

    [DrawDottedBox(0f, 1f, 1f, screenSpaceSize = 1.5f)]
    public Rect dottedBox;
}
