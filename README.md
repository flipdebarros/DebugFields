# DebugFields
This repo contains some unity attributes and property drawers to help visualize some fields in a MonoBehaviour.
While working on my 2D game projects in unity, I often found myself having to find a way to visualize some parameters for some script on the actual scene. And even after learning about the OnDrawGizmos, it still felt repetitive doing that for every field I want to debug. I created this utility that, just by adding an attribute to it, draws the variables in the scene. I started with functions to draw circles and boxes, but I intend to expand this tool as it becomes necessary.

<img src="https://i.imgur.com/XAFgqUb.png" alt="example image" width="585"/>

## How to use it

Before anything can be drawn to the scene, your MonoBehaviour class needs the DebugFields attribute. For example:
	
````c#
using Utils.DebugFields;

[DebugFields]
public class ExampleScript : MonoBehaviour { } 
`````
	
This will add a toggle on the inspector that can turn on and off the visualization in the scene.
Only this will not be enough to visualize your variables, you'll need now to mark which fields* you want to have drawn.

Passing in 3 float values in the attribute will change the default color your fields will be drawn.

````c#
[DebugFields(1f, 0f, 0f)] //Default color set to red
public class ExampleScript : MonoBehaviour { }
`````

*all draw and modifier fields have to be serialized (be public or private with the SerializeFieldAttribute)
	
## Draw Field Attribute

When applied to serialized fields, draw something on the scene using its value.
For any of them, you can pass RGB values as parameters to override its color.

````c#
[DrawCircle(1f, 1f, 0f)] // override color to yellow
public float circle;
`````

### DrawCircle

For a <i>float</i> or <i>int</i> field you can draw a circle with that value as a radius.

Example:

````c#
[DrawCircle]
public float circle;
	
[DrawCircle(thickness = 1f)] // changes thickness of circle edge 
public float circle;
`````

### DrawBox or DrawDottedBox

For a <i>Rect</i> field you can draw a box with that Rect as its position and size.

Example:

````c#
[DrawBox()]
public Rect box;
	
[DrawBox(thickness = 2f)] // changes thickness of box edge 
public Rect box;
	
[DrawDottedBox()]
public Rect dottedBox;
	
[DrawDottedBox(0f, 1f, 1f, screenSpaceSize = 1.5f)] // overrides color to cian and sets screenspace size of segments to 1.5f
public Rect dottedBox;
`````

## Debug Modify Attribute

When applied to serialized fields, creates a property linked to another field that changes how it is drawn.

### SetOffset

Used in a <i>Vector2</i> field, changes the position the referenced field is drawn on the scene (relative to the parent object position).

````c#
[SetOffset(nameof(circle))] // set the offset of circle
public Vector2 circlePos;
[DrawCircle]
public float circle;
`````

### SetColor

Used in a <i>Color</i> field, changes the color the referenced field is drawn on the scene.

````c#
[SetColor(nameof(box))] // overrides the color of the box
public Color boxColor;
[DrawBox()]
public Rect box;
`````
