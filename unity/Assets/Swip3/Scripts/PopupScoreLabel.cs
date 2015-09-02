using UnityEngine;
using System.Collections;

public class PopupScoreLabel : MonoBehaviour {

	[SerializeField]
	U9View view = null;

	[SerializeField]
	UILabel scoreLabel = null;
	
	public Color Color {
		set {
			scoreLabel.color = value;
		}
	}

	int score;

	public int Score {
		get {
			return score;
		}
		set {
			score = value;
			scoreLabel.text = "+" + value.ToString();
		}
	}

	public U9Transition CreatePopupTransition() 
	{
		return view.GetHideTransition (true);
	}

}
