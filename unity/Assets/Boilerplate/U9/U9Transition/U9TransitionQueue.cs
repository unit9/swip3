using UnityEngine;
using System.Collections.Generic;
using System;

public class U9TransitionQueue {

	Queue<U9Transition> queue;

	public Action QueueEmptied;

	public U9TransitionQueue() {
		queue = new Queue<U9Transition>();
	}

	public void AddTransition( U9Transition t ) {
		queue.Enqueue( t );
		if( queue.Count == 1 ) {
			BeginNextTransition();
		}
	}

	void BeginNextTransition() {
		if( queue.Count > 0 ) {
			U9Transition t = queue.Peek();
			t.Ended += HandleCurrentTransitionEnded;
			//Debug.Log ("BEGINNING QUEUED TRANS!");
			t.Begin();
		} else {
			if( QueueEmptied != null ) {
				QueueEmptied();
			}
		}
	}

	void HandleCurrentTransitionEnded (U9Transition transition)
	{
		queue.Dequeue ();
		BeginNextTransition();
	}

}
