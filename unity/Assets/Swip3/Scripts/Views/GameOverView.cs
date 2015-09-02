using UnityEngine;
using System.Collections;

public class GameOverView : U9FadeView {

	[SerializeField]
	UILabel finalScoreLabel = null;

	//Watch only
	[SerializeField]
	UILabel bestScoreLable = null;

	/// <summary>
	/// Sets the labels for this view if they exist
	/// </summary>
	/// <param name="score">Score.</param>
	/// <param name="highScore">High score.</param>
	public void Setup(int score) 
	{
		finalScoreLabel.text = score.ToString ();

		if(GameController.Inst.Platform == Platform.Watch && bestScoreLable != null )
		{
			bestScoreLable.text = GameController.Inst.HighScore.ToString();
		}
	}
}
