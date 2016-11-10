using UnityEngine;
using System.Collections;

public class BubbleData{

	public static readonly Color BLACK = new Color(0,0,0,1);

	private string line = "";
	public string Line{
		get { return line; }
		set { this.line = value; }
	}

	private Color baseColor = Color.white;
	public Color BaseColor{
		get { return baseColor; }
		set { this.baseColor = value; }
	}

	private Color outlineColor = Color.black;
	public Color OutlineColor{
		get { return outlineColor; }
		set { this.outlineColor = value; }
	}

	private Color textColor = Color.white;
	public Color TextColor{
		get { return textColor; }
		set { this.textColor = value; }
	}
	private Color textOutlineColor = Color.black;
	public Color TextOutlineColor{
		get { return textOutlineColor; }
		set { this.textOutlineColor = value; }
	}

	public Vector2 origin;
	public Vector2 Origin{
		get { return origin; }
		set { this.origin = value; }
	}

	public Vector2 destiny;
	public Vector2 Destiny{
		get { return destiny; }
		set { this.destiny = value; }
	}

	public BubbleData(string line, Vector2 origin, Vector2 destiny){
		initialize (line, origin, destiny, Color.white, Color.black, Color.white, Color.black);
	}

	public BubbleData (string line, Vector2 origin, Vector2 destiny, Color baseColor, Color outlineColor, Color textColor, Color textOutlineColor){
		initialize (line, origin, destiny, baseColor, outlineColor, textColor, textOutlineColor);
	}


	private void initialize (string line, Vector2 origin, Vector2 destiny, Color baseColor, Color outlineColor, Color textColor, Color textOutlineColor){
		this.line = line;

		this.origin = origin;
		this.destiny = destiny;

		this.baseColor = baseColor;
		this.outlineColor = outlineColor;
		this.textColor = textColor;
		this.textOutlineColor = textOutlineColor;
	}
}
