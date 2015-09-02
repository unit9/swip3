#define USE_HOTWEEN

using UnityEngine;
using System.Collections;
using System.Linq;

// U9T.cs
//
// Static class of shorthand convienience methods for creating transitions.
//
// Created by Nick McVroom-Amoakohene <nicholas@unit9.com> on 06/09/2012.
// 
// Copyright (c) 2012 unit9 ltd. www.unit9.com. All rights reserved

using System.Collections.Generic;

public static class U9T {
	#region Convienience methods
	
	/// <summary>
	/// Shorthand for the <see cref="U9NullTransition"/>.
	/// </summary>
	public static U9NullTransition Null() {
		return new U9NullTransition();
	}
	
	/// <summary>
	/// Shorthand for the <see cref="U9ParallelTransition"/>.
	/// </summary>
	/// <param name='transitions'>
	/// Transitions.
	/// </param>
	public static U9ParallelTransition P(params U9Transition[] transitions) {
		ValidateTransitionArray(transitions);
		return new U9ParallelTransition(transitions);
	}
	
	/// <summary>
	/// Shorthand for the <see cref="U9SerialTransition"/>.
	/// </summary>
	/// <param name='transitions'>
	/// Transitions.
	/// </param>
	public static U9SerialTransition S(params U9Transition[] transitions) {
		ValidateTransitionArray(transitions);
		return new U9SerialTransition(transitions);
	}

	public static U9SerialTransition PrioritySequence( U9Transition[] transitions, float staggerTime = 0f ) {

		//Debug.Log ("START PRIORITY SEQUENCE -------------");
		List<U9Transition> transList = new List<U9Transition>( transitions );
		//transList.Sort( CompareTransitionPriority );
		IEnumerable enumerator = transList.OrderBy (t => t.Priority);

		int? currentPriority = null;
		U9SerialTransition serial = new U9SerialTransition ();
		List<U9Transition> parallelGroup = new List<U9Transition> ();

		foreach (U9Transition t in enumerator) {

			if (t != null) {
				if (t.Priority != currentPriority) {
					if (parallelGroup.Count > 0) {
						//Debug.Log ("Priority group: " + currentPriority + " = " + parallelGroup.Count );
						serial.AddTransition (U9T.Stagger (staggerTime, parallelGroup.ToArray ()));
						parallelGroup.Clear ();
					}
					currentPriority = t.Priority;
				}
				parallelGroup.Add (t);
			}

		}
		if (parallelGroup.Count > 0) {
			//Debug.Log ("Priority group: " + currentPriority + " = " + parallelGroup.Count );
			serial.AddTransition( U9T.Stagger( staggerTime, parallelGroup.ToArray() ) );
			parallelGroup.Clear ();
		}
		return serial;
	}

	public static int GetHighestPriority( List<U9Transition> transitions ) {
		int max = int.MinValue;
		foreach (U9Transition t in transitions) {
			if (t.Priority > max) {
				max = t.Priority;
			}
		}
		return max;
	}
	
	static int CompareTransitionPriority( U9Transition a, U9Transition b ) {
		return a.Priority.CompareTo (b.Priority);
	}

#if USE_HOTWEEN
	/// <summary>
	/// Shorthand for the <see cref="U9HOTweenTransition"/>.
	/// Uses the default constructor. For more control, use the other overloads directly.
	/// </summary>
	/// <param name='hoTweenMethod'>
	/// HOTween method.
	/// </param>
	/// <param name='target'>
	/// Target.
	/// </param>
	/// <param name='duration'>
	/// Duration.
	/// </param>
	/// <param name='parms'>
	/// Parameters.
	/// </param>
	public static U9HOTweenTransition HOT(U9HOTweenTransition.HOTweenDelegate hoTweenMethod, object target, float duration, Holoville.HOTween.TweenParms parms) {
		return new U9HOTweenTransition(hoTweenMethod, target, duration, parms);
	}

	public static U9LeanTweenTransition LT(U9LeanTweenTransition.LeanTweenDelegate hoToTween, GameObject target, Vector3 to, float duration, Hashtable args){
		return new U9LeanTweenTransition(hoToTween, target, to, duration, args);
	}

	public static U9LeanTweenTransition LT(U9LeanTweenTransition.LeanTweenDelegateFloat hoToTween, GameObject target, float to, float duration, Hashtable args){
		return new U9LeanTweenTransition(hoToTween, target, to, duration, args);
	}

	public static U9iTweenTransition T( U9iTweenTransition.iTweenDelegate hoTweenMethod, GameObject target, Hashtable args) {
		return new U9iTweenTransition( hoTweenMethod, target, args);
	}

	/// <summary>
	/// Shorthand for the <see cref="U9HOTweenTransition"/>.
	/// Uses the default constructor, with the option to control whether it should be destroyed on completion. For more control, use the other overloads directly.
	/// </summary>
	/// <param name='hoTweenMethod'>
	/// HOTween method.
	/// </param>
	/// <param name='target'>
	/// Target.
	/// </param>
	/// <param name='duration'>
	/// Duration.
	/// </param>
	/// <param name='ignoreTimescale'>
	/// Determines whether this tween should ignore timescale.
	/// </param>
	/// <param name='parms'>
	/// Parameters.
	/// </param>
	//public static U9HOTweenTransition T(U9HOTweenTransition.HOTweenDelegate hoTweenMethod, object target, float duration, Holoville.HOTween.TweenParms parms) {
//		return new U9HOTweenTransition(hoTweenMethod, target, duration, parms);
//	}
	
	/// <summary>
	/// Shorthand for the <see cref="U9HOTweenTransition"/>.
	/// Uses the default constructor, with the option to control whether it should be destroyed on completion. For more control, use the other overloads directly.
	/// </summary>
	/// <param name='hoTweenMethod'>
	/// HOTween method.
	/// </param>
	/// <param name='target'>
	/// Target.
	/// </param>
	/// <param name='duration'>
	/// Duration.
	/// </param>
	/// <param name='ignoreTimescale'>
	/// Determines whether this tween should ignore timescale.
	/// </param>
	/// <param name='destroyOnComplete'>
	/// Determines whether this tween should be destroyed when it completes.
	/// </param>
	/// <param name='parms'>
	/// Parameters.
	/// </param>
//	public static U9HOTweenTransition T(U9HOTweenTransition.U9HOTweenDelegate hoTweenMethod, object target, float duration, bool ignoreTimescale, bool destroyOnComplete, Holoville.HOTween.TweenParms parms) {
//		return new U9HOTweenTransition(hoTweenMethod, target, duration, destroyOnComplete, ignoreTimescale, parms);
//	}
#endif
	
#if !USE_HOTWEEN
	/// <summary>
	/// Shorthand for the <see cref="U9HOTweenTransition"/>.
	/// Uses the default constructor. For more control, use the other overloads directly.
	/// </summary>
	/// <param name='hoTweenMethod'>
	/// HOTween method.
	/// </param>
	/// <param name='target'>
	/// Target.
	/// </param>
	/// <param name='duration'>
	/// Duration.
	/// </param>
	/// <param name='parms'>
	/// Parameters.
	/// </param>
	public static U9iTweenTransition T( U9iTweenTransition.iTweenDelegate hoTweenMethod, GameObject target, Hashtable args) {
		return new U9iTweenTransition( hoTweenMethod, target, args);
	}

#endif
	
	/// <summary>
	/// Shorthand for the <see cref="U9WaitTransition"/>.
	/// Uses the default constructor. For more control, use the other overloads directly.
	/// </summary>
	/// <param name='waitSeconds'>
	/// Wait seconds.
	/// </param>
	public static U9WaitTransition W(float waitSeconds) {
		return new U9WaitTransition(waitSeconds);
	}
	
	/// <summary>
	/// Shorthand for the <see cref="U9WaitTransition"/>.
	/// Uses the default constructor with the option to ignore timescale. For more control, use the other overloads directly.
	/// </summary>
	/// <param name='waitSeconds'>
	/// Wait seconds.
	/// </param>
	/// <param name='ignoreTimescale'>
	/// Ignore timescale.
	/// </param>
	public static U9WaitTransition W(float waitSeconds, bool ignoreTimescale) {
		return new U9WaitTransition(waitSeconds, ignoreTimescale);
	}
	
	public static void ValidateTransitionArray( U9Transition[] transitions ) {
		for( int i = 0, ni = transitions.Length ; i < ni ; i++ ) {
			if( transitions[i] == null ) {
				transitions[i] = U9T.Null();
			}
		}
	}
	
	/// <summary>
	/// Plays transitions in parallel, with a specified interval between each one.
	/// </summary>
	/// <param name='staggerOffset'>
	/// Start time offset.
	/// </param>
	/// <param name='transitions'>
	/// Transitions to stagger.
	/// </param>
	public static U9ParallelTransition Stagger(float staggerOffset, params U9Transition[] transitions) {
		return Stagger ( staggerOffset, false, transitions );
	}
	
	/// <summary>
	/// Plays transitions in parallel, with a specified interval between each one.
	/// </summary>
	/// <param name='staggerOffset'>
	/// Start time offset.
	/// </param>
	/// <param name='ignoreTimescale'>
	/// Set to true to ignore timescale settings.
	/// </param>
	/// <param name='transitions'>
	/// Transitions to stagger.
	/// </param>
	public static U9ParallelTransition Stagger(float staggerOffset, bool ignoreTimescale, params U9Transition[] transitions) {
		U9ParallelTransition stagger = new U9ParallelTransition();
		
		float currentOffset = 0f;
		
		foreach(U9Transition t in transitions) {
			if(t && !t.IsNull ) {
				stagger.AddTransition(U9T.S(U9T.W(currentOffset, ignoreTimescale), t));
				currentOffset += staggerOffset;
			}
		}
		
		return stagger;
	}
	
	#endregion
}

