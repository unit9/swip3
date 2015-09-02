using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class Block : MonoBehaviour {
	
	[SerializeField]
	UISprite squareSprite = null;

	[SerializeField]
	GameObject squareGO = null;

	[SerializeField]
	PopupScoreLabel popupScoreLabelPrefab = null;

	[SerializeField]
	GameObject[] scorePrefab;

	[SerializeField]
	Material[] scoreMaterial;

	public Transform Scaling;

	Color color;

	public Color Color 
	{
		get 
		{ 
			return color; 
		} 
		set 
		{
			color = value; 
			if(squareGO)
				squareGO.renderer.material.color = value;
			else
				squareSprite.color = value; 
		} 
	}

	public int SquareSize 
	{ 
		set 
		{ 
			if(squareGO)
			{
				squareGO.transform.localScale = new Vector3(value, value, 1);
			}
			else
			{
				squareSprite.width = value; 
				squareSprite.height = value; 
			}
		} 
	}

	bool checkedForMatches;

	public bool CheckedForMatches { get { return checkedForMatches; } set { checkedForMatches = value; } }

	bool ghost;

	// Used to fade the blocks when they are on the edge
	public bool Ghost 
	{ 
		get 
		{ 
			return ghost; 
		} 
		set 
		{ 
			ghost = value; 

			if(squareGO)
				squareGO.renderer.material.color = new Color(squareGO.renderer.material.color.r,
				                                             squareGO.renderer.material.color.g,
				                                             squareGO.renderer.material.color.b,
				                                             ( value ? 0.75f : 1f ));
			else
				squareSprite.alpha = ( value ? 0.75f : 1f ); 
		} 
	}

	int i, j;

	public int I { get { return i; } set { i = value; } }

	public int J { get { return j; } set { j = value; } }

	public bool IsBackground 
	{ 
		set 
		{ 
			if(squareGO)
			{
				squareGO.transform.localPosition = new Vector3(squareGO.transform.position.x, squareGO.transform.position.y, ( value ? 1 : -1 ));
			}
			else
				squareSprite.depth = ( value ? -1 : 1 ); 
		} 
	}

	/// <summary>
	/// Creates a transition which disappears the block and spawns a score label in its place, which will itself disappear after some time.
	/// </summary>
	/// <returns>The disappear transition.</returns>
	/// <param name="score">Score.</param>
	public U9Transition CreateDisappearTransition( int score ) {

		U9Transition scorePopupTransition = null;

		//U9Transition blockDisappearTransition = U9T.HOT (iTween.ScaleTo, squareSprite.gameObject, iTween.Hash ("scale", Vector3.zero, "time", 0.25f, "easetype", iTween.EaseType.easeInOutQuad ));

		//U9Transition bDT = U9T.HOT (HOTween.To, Scaling, 0.25f, new TweenParms().Prop("localScale", Vector3.zero, false).Ease(EaseType.EaseInOutQuad));
		U9Transition bDLT = U9T.LT (LeanTween.scale, squareGO, Vector3.zero, 0.25f, iTween.Hash("ease", LeanTweenType.easeInOutQuad));

		if (score > 0) 
		{
			/*GameObject popupScoreInstance = (GameObject)Instantiate(scorePrefab[score-3], transform.position, Quaternion.identity);
			popupScoreInstance.transform.parent = transform;
			popupScoreInstance.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

			int materialIn = 5*(score-3);
			for(int i = materialIn; i < 5 + materialIn; i++)
			{
				if(scoreMaterial[i].color == color)
				{
					Color newColor = new Color(scoreMaterial[i].color.r, scoreMaterial[i].color.g, scoreMaterial[i].color.b, 0);
					popupScoreInstance.renderer.material.color = newColor;
					break;
				}
			}

			U9Transition ScaleIn = U9T.LT (LeanTween.scale, popupScoreInstance, new Vector3(53f, 53f, 53f), 1f, iTween.Hash("ease", LeanTweenType.linear));
			U9Transition FadeIn = U9T.HOT (HOTween.To, popupScoreInstance.renderer.material, 0.35f, new TweenParms().Prop("color", new Color(color.r, color.g, color.b, 1), false).Ease(EaseType.EaseInOutQuad));
			U9Transition FadeOut = U9T.HOT (HOTween.To, popupScoreInstance.renderer.material, 0.55f, new TweenParms().Prop("color", new Color(color.r, color.g, color.b, 0), false).Ease(EaseType.EaseInOutQuad).Delay(0.65f));
			
			ScaleIn.Begin();
			FadeIn.Begin();
			FadeOut.Begin();*/


			PopupScoreLabel popupScoreLabelInstance = (PopupScoreLabel)Instantiate (popupScoreLabelPrefab, transform.position, Quaternion.identity);

			popupScoreLabelInstance.transform.parent = transform;
			popupScoreLabelInstance.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			popupScoreLabelInstance.Score = score;
			Color c = new Color(color.r, color.g, color.b, 0);
			popupScoreLabelInstance.Color = c;

			U9Transition ScaleIn = U9T.LT (LeanTween.scale, popupScoreLabelInstance.gameObject, new Vector3(0.6f, 0.6f, 0.6f), 1f, iTween.Hash("ease", LeanTweenType.linear));
			U9Transition FadeIn = U9T.HOT (HOTween.To, popupScoreLabelInstance.GetComponent<UILabel>(), 0.35f, new TweenParms().Prop("color", new Color(color.r, color.g, color.b, 1), false).Ease(EaseType.EaseInOutQuad));
			U9Transition FadeOut = U9T.HOT (HOTween.To, popupScoreLabelInstance.GetComponent<UILabel>(), 0.55f, new TweenParms().Prop("color", new Color(color.r, color.g, color.b, 0), false).Ease(EaseType.EaseInOutQuad).Delay(0.65f));

			ScaleIn.Begin();
			FadeIn.Begin();
			FadeOut.Begin();
			//U9Transition FadeIn  = U9T.LT (LeanTween.alpha, popupScoreLabelInstance, 1.0f, 0.35f, iTween.Hash("ease", LeanTweenType.easeInCubic));

			Destroy(popupScoreLabelInstance, 5.0f);
		}

		bDLT.Ended += (transition) => 
		{
			Destroy (gameObject, 4.0f); 
		};
		
		return new InstantTransition( U9T.S ( bDLT) );
	}

}
