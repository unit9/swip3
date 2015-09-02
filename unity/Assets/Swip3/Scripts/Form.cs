using UnityEngine;
using System.Collections;
/***
 *Changing X value of selected Game objects to adjust view to screen widith; 
 ***/
public class Form : MonoBehaviour 
{
	[Header("Header")]
	public UISprite HBackground;
	public GameObject HLogo;
	public GameObject HLeaderboards;
	public GameObject HAbaou;
	public GameObject HExit;

	[Header("Footer")]
	public UISprite FBackground;
	public GameObject FU9Logo;
	public GameObject FScores;

	private int Widith;
	private int Height;
	private float Ratio;

	public float Margin;
	 
	// Use this for initialization
	void Start () 
	{
		Widith = Screen.width;
		Height = Screen.height;
		Ratio = Widith/Height;

		HBackground.width = Widith *2;
		FBackground.width = Widith *2;

		Debug.Log(Widith);

		HLogo.transform.position = new Vector3((20.0f-Ratio)*Margin, HLogo.transform.position.y, HLogo.transform.position.z);
	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}
