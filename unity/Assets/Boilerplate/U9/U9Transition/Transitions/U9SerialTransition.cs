// U9SerialTransition.cs
// 
// Created by Nick McVroom-Amoakohene <nicholas@unit9.com> on 04/09/2012.
// Based on code by David Rzepa <dave@unit9.com>
// Copyright (c) 2012 unit9 ltd. www.unit9.com. All rights reserved
using System.Collections.Generic;
using UnityEngine;

public class U9SerialTransition : U9Transition {

	List<U9Transition> transitions;
	U9Transition current;
	
	int completedTransitions = 0;

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

	public enum InterruptedBehaviourType {
		InterruptOnInterrupt,
		CompleteOnInterrupt,
		ContinueOnInterrupt
	}
	
	InterruptedBehaviourType interruptBehaviour = InterruptedBehaviourType.ContinueOnInterrupt;
	
	public InterruptedBehaviourType InterruptBehaviour {
		get {
			return this.interruptBehaviour;
		}
		set {
			interruptBehaviour = value;
		}
	}

	public U9SerialTransition() : base() {
		transitions = new List<U9Transition>();
	}
	
	public U9SerialTransition(params U9Transition[] transitions) : this() {
		
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
	
	void StartNextTransition() {
		
		if(completedTransitions < transitions.Count) {
			if(!current || current.IsCompleted) {
				
				current = transitions[completedTransitions];
				
				current.Interrupted += OnTransitionInterrupted;
				current.Ended += OnTransitionEnded;
				
				current.IsCompleted = false;
				current.Begin();

			}
		} 
		else {
			End();
		}
	}
	
	void OnTransitionInterrupted(U9Transition t) {

		WasInterrupted = true;

		t.Interrupted -= OnTransitionInterrupted;
		t.Ended -= OnTransitionEnded;
		
		TransitionInterruptedReceiver();
	}
	
	void OnTransitionEnded(U9Transition t) {

		WasInterrupted = WasInterrupted | t.WasInterrupted;

		t.Interrupted -= OnTransitionInterrupted;
		t.Ended -= OnTransitionEnded;
		
		TransitionCompletedReceiver();
	}
	
	void TransitionCompletedReceiver() {
		if(completedTransitions < transitions.Count) {
			completedTransitions++;
			StartNextTransition();
		} 
		else {
			End();
		}
	}
	
	void TransitionInterruptedReceiver() {
		switch(interruptBehaviour) {
			case InterruptedBehaviourType.CompleteOnInterrupt:
				End();
				break;
			case InterruptedBehaviourType.ContinueOnInterrupt:
				TransitionCompletedReceiver();
				break;
			case InterruptedBehaviourType.InterruptOnInterrupt:
				OnInterrupted(this);
				break;
		}
	}
	
	#region implemented abstract members of U9Transition
	public override void Begin() {
		base.Begin();
		
		completedTransitions = 0;
		
		StartNextTransition();
	}

	public override void Stop() {
		base.Stop();
		
		for(int i = 0, ni = transitions.Count; i < ni; i++) {
			transitions[i].Stop();
		}
	}
	
	#endregion	
	
	public override string ToString ()
	{
		string s = "S[";
		foreach(U9Transition t in transitions) {
			s += t.ToString() + ",";
		}
		s.Remove(s.Length-2);
		s += "]";
		return s;
	}
}

