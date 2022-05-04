using UnityEngine;

[DebugFields("My Behaviour Test")]
public class MyBehaviour : MonoBehaviour {

	[DrawPoint(inheritTransform = true)]
	public Vector3 point;
	
	[DrawPoint(inheritTransform = true)]
	public Vector3Int pointInt;
	
	[DrawLine(inheritTransform = true)]
	public float myLine;
	
	[DrawLine(inheritTransform = true)]
	public int myLineInt;
	
	[DrawLine(inheritTransform = true)]
	public Vector2 myVector2;
	
	[DrawLine(inheritTransform = true)]
	public Vector3 myVector3;
	
	[DrawLine(inheritTransform = true)]
	public Vector2Int myVector2Int;
	
	[DrawLine(inheritTransform = true)]
	public Vector3Int myVector3Int;

	[DrawCircle2D(inheritTransform = true)]
	public float myCircle;
	
	[DrawCircle2D(inheritTransform = true)]
	public int myCircleInt;
	
	[DrawBox2D(inheritTransform = true)]
	public Rect myBox;

	[DrawBox2D(inheritTransform = true)]
	public RectInt myBoxInt;
	
	[DrawBox2D(inheritTransform = true)]
	public Vector2 myBoxVector2;

	[DrawBox2D(inheritTransform = true)]
	public Vector2Int myBoxVector2Int;
}


