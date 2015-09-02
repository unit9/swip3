using UnityEngine;
using System.Collections;

public class U9FadeView : U9TransitionView {
	
	[SerializeField]
	MonoFader[] fadables;
	
	public abstract class MonoFader : MonoBehaviour {
		public abstract float Alpha { get; set; }
		public abstract bool Fading { set; }
	}

	U9Transition lastTransition;

	#region implemented abstract members of U9View
	protected override U9Transition CreateDisplayTransition ( bool force )
	{
		U9Transition t = U9T.T ( iTween.ValueTo, gameObject, iTween.Hash( "time", transitionDuration, "from", 0f, "to", 1f, "easetype", transitionEaseType, "onupdate", "SetAlphas", "ignoretimescale", ignoreTimeScale ) );
		t.Began += HandleTransitionBegan;
		return t;
	}

	void HandleTransitionBegan (U9Transition transition)
	{
		if (lastTransition != null) {
			lastTransition.Stop();
		}
		lastTransition = transition;
	}

	protected override U9Transition CreateHideTransition ( bool force )
	{
		U9Transition t =  U9T.T ( iTween.ValueTo, gameObject, iTween.Hash( "time", transitionDuration, "from", 1f, "to", 0f, "easetype", transitionEaseType, "onupdate", "SetAlphas", "ignoretimescale", ignoreTimeScale ) );
		t.Began += HandleTransitionBegan;
		return t;
	}
	#endregion
	
	protected override void BeginDisplay ()
	{
		base.BeginDisplay ();
		for( int i = 0, ni = fadables.Length ; i < ni ; i++ ) {
			fadables[i].Fading = true;
		}
	}
	
	protected override void EndDisplay ()
	{
		base.EndDisplay ();
		for( int i = 0, ni = fadables.Length ; i < ni ; i++ ) {
			fadables[i].Fading = false;
		}
		SetAlphas( 1f );
	}
	
	protected override void BeginHide ()
	{
		base.BeginHide ();
		for( int i = 0, ni = fadables.Length ; i < ni ; i++ ) {
			fadables[i].Fading = true;
		}
	}
	
	protected override void EndHide ()
	{
		base.EndHide ();	
		for( int i = 0, ni = fadables.Length ; i < ni ; i++ ) {
			fadables[i].Fading = true;
		}
		SetAlphas( 0f );
	}
	
	void SetAlphas( float a ) {
		for( int i = 0, ni = fadables.Length ; i < ni ; i++ ) {
			fadables[i].Alpha = a;
		}
	}

}

