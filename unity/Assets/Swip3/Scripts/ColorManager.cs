using UnityEngine;
using System.Collections;

public class ColorManager : MonoBehaviour {

	[SerializeField]
	Color[] blocksPalette;

	[SerializeField]
	Color backgroundBlockColor;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Color[] getBlockColors()
	{
		return blocksPalette;
	}

	public Color getBackgroundBlockColor()
	{
		return backgroundBlockColor;
	}
}
