using UnityEngine;
using System.Collections;

public class U9LeanTweenTransition : U9Transition
{
	
	public delegate int LeanTweenDelegate( GameObject go, Vector3 to, float time, Hashtable args, out LTDescr ltd );
	public delegate int LeanTweenDelegateFloat( GameObject go, float to, float time, Hashtable args, out LTDescr ltd );

	LeanTweenDelegate tweenDelegate;
	LeanTweenDelegateFloat tweenDelegateFloat;
	GameObject go;
	Vector3 to;
	float toFloat;
	float time;
	Hashtable args;
	LTDescr ltd;
	
	string name;
	static long uid = 0;
	
	public U9LeanTweenTransition( LeanTweenDelegate tweenDelegate, GameObject go,  Vector3 to, float time, Hashtable args) 
	{
		this.tweenDelegate = tweenDelegate;
		this.go = go;
		this.to = to;
		this.time = time;
		this.args = args;
	}

	public U9LeanTweenTransition( LeanTweenDelegateFloat tweenDelegateFloat, GameObject go,  float to, float time, Hashtable args) 
	{
		this.tweenDelegateFloat = tweenDelegateFloat;
		this.go = go;
		this.toFloat = to;
		this.time = time;
		this.args = args;
	}
	
	public override void Begin ()
	{
		base.Begin ();

		//args.Add( "ignoretimescale", true );
		if (tweenDelegate != null) 
		{
			name = tweenDelegate (go, to, time, args, out ltd).ToString ();
		} 
		else 
		{
			name = tweenDelegateFloat (go, toFloat, time, args, out ltd).ToString ();
		}
		if(ltd != null)
			ltd.setOnComplete(TweenComplete);
	}
	
	void TweenComplete() {
		if (!IsCompleted) {
			OnEnded (this);
		}
	}
	
	void TweenInterrupted() {
		#if UNITY_EDITOR
		Debug.Log ("TweenInterrupted - " + name + " : " + go + ", type: " + tweenDelegate.Method.Name, go );
		#endif
		
		if (!IsCompleted) {
			OnInterrupted (this);
		}
	}
	
	public override void Stop ()
	{
		base.Stop ();
		iTween.StopByName (go, name);
	}
	
	public override string ToString ()
	{
		return string.Format ("[U9iTweenTransition={0}, Hash={1}]", tweenDelegate.Method.Name, SaveController.FormatHashtable(args,"") );
	}
}

