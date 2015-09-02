// U9ParallelTransition.cs
// 
// Created by Nick McVroom-Amoakohene <nicholas@unit9.com> on 04/09/2012.
// Based on code by David Rzepa <dave@unit9.com>
// Copyright (c) 2012 unit9 ltd. www.unit9.com. All rights reserved
using System.Collections.Generic;
using UnityEngine;

public class U9ParallelTransition : U9Transition {
	
	List<U9Transition> transitions;
	int transitionsFinished = 0;

	public override bool IsNull {
		get {
			for (int i = 0; i < transitions.Count; i++) {
				U9Transition t = transitions [i];
				if (!t.IsNull) {
					return false;
				}
			}
			return true;
		}
	}

	public U9ParallelTransition() : base() {
		transitions = new List<U9Transition>();
	}
	
	public U9ParallelTransition(params U9Transition[] transitions) : this() {
		this.transitions.AddRange(transitions);
	}
	
	public void AddTransition(U9Transition transition) {
		this.transitions.Add(transition);
	}
	
	public void AddTransition(params U9Transition[] transitions) {
		this.transitions.AddRange(transitions);
	}
	
	public void Clear() {
		this.transitions.Clear();
	}
	
	void TransitionCompletedReceiver() {
		transitionsFinished++;
		
		if(transitionsFinished == transitions.Count) {
			End();
		}
	}
	
	void OnTransitionEnded(U9Transition t) {

		WasInterrupted = WasInterrupted | t.WasInterrupted;

		t.Interrupted -= HandleTransitionInterrupted;
		t.Ended -= OnTransitionEnded;		
		TransitionCompletedReceiver();
	}
	
	#region implemented abstract members of U9Transition
	
	public override void Begin() {
		base.Begin();
		
		transitionsFinished = 0;
		
		if(transitions.Count > 0) {
			foreach(U9Transition t in transitions) {
				t.Interrupted += HandleTransitionInterrupted;
				t.Ended += OnTransitionEnded;
				t.Begin();
			}
		}
		else {
			End();
		}
	}

	void HandleTransitionInterrupted (U9Transition transition)
	{
		WasInterrupted = true;
		transition.Interrupted -= HandleTransitionInterrupted;
		transition.Ended -= OnTransitionEnded;		
		TransitionCompletedReceiver();
	}
	
	public override void Stop() {
		base.Stop();
		
		if(transitions.Count > 0) {
			foreach(U9Transition t in transitions) {
				t.Stop();
			}
		}
	}
	#endregion
	
	public override string ToString ()
	{
		string s = "P[";
		foreach(U9Transition t in transitions) {
			s += t.ToString() + ",";
		}
		s.Remove(s.Length-2);
		s += "]";
		return s;
	}
}

