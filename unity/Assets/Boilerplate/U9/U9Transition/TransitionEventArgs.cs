using UnityEngine;
using System.Collections.Generic;

public class TransitionEventArgs : System.EventArgs {

	List<U9Transition> transitions;

	public List<U9Transition> Transitions {
		get {
			return transitions;
		}
	}

	public TransitionEventArgs() {
		transitions = new List<U9Transition>();
	}

}
