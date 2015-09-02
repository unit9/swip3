// U9WaitTransition.cs
// 
// Created by Nick McVroom-Amoakohene <nicholas@unit9.com> on 03/09/2012.
// Based on code by David Rzepa <dave@unit9.com>
// Copyright (c) 2012 unit9 ltd. www.unit9.com. All rights reserved
using UnityEngine;
using System.Collections;

public class U9WaitTransitionHelper : MonoBehaviour {
	
}

public class U9WaitTransition : U9Transition {
	
	static U9WaitTransitionHelper helper;
	
	/// <summary>
	/// Gets a helper GameObject for transitions, to enable them to run coroutines and invokes.
	/// See <see cref="U9WaitTransition"/> for example usage.
	/// </summary>
	public U9WaitTransitionHelper Helper {
		private set {
			helper = value;
		}
		
		get { 
			if(!helper) { 
				helper = new GameObject("Helper").AddComponent<U9WaitTransitionHelper>(); 
			} 
			return helper; 
		}
	}
	
	float waitSeconds; 
	bool ignoreTimeScale;
	
	float endTime;
	
	public U9WaitTransition(float waitSeconds) : this(waitSeconds,false) {

	}
	
	public U9WaitTransition(float waitSeconds, bool ignoreTimescale) : base() {
		//Debug.Log ("WAIT, ignoreTimescale: " + ignoreTimescale );
		this.waitSeconds = waitSeconds;
		this.ignoreTimeScale = ignoreTimescale;
	}

	#region implemented abstract members of U9Transition
	public override void Begin() {
		base.Begin();
		if( ignoreTimeScale ) {
			endTime = Time.realtimeSinceStartup + waitSeconds;
			Helper.StartCoroutine( WaitIgnoringTimeScaleCoroutine() );
		} else {
			endTime = Time.time + waitSeconds;
			Helper.StartCoroutine( WaitCoroutine() );
		}
	}

	public override void Stop() {
	}
	#endregion
	
	IEnumerator WaitCoroutine() {
		//while( Time.time < endTime ) {
			yield return new WaitForSeconds(waitSeconds);
		//}
		OnEnded(this);
	}
	
	IEnumerator WaitIgnoringTimeScaleCoroutine() {
		while( Time.realtimeSinceStartup < endTime ) {
			//Debug.Log ("WaitIgnoringTimeScaleCoroutine");
			yield return new WaitForEndOfFrame();
		}
		OnEnded(this);
	}
	
}