using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class U9HOTweenTransition : U9Transition
{

	public delegate Tweener HOTweenDelegate( object go, float duration , TweenParms args );
	
	HOTweenDelegate tweenDelegate;
	object go;
	TweenParms args;
	float duration;
	
	string name;
	static long uid = 0;
	
	public U9HOTweenTransition( HOTweenDelegate tweenDelegate, object go, float duration, TweenParms args ) 
	{
		this.tweenDelegate = tweenDelegate;
		this.go = go;
		this.args = args;
		this.duration = duration;
	}
	
	public override void Begin ()
	{
		base.Begin ();

		args.OnComplete(TweenComplete);
		args.OnPause(TweenInterrupted);
		args.Id(tweenDelegate.ToString() + uid++);

		tweenDelegate( go, duration, args );
	}
	
	public void TweenComplete() {

		if (IsCompleted) {
			OnEnded (this);
		}
	}
	
	void TweenInterrupted() {

		if (!IsCompleted) {
			OnInterrupted (this);
		}
	}
	
	public override void Stop ()
	{
		base.Stop ();
		HOTween.Kill(go);
	}
}

