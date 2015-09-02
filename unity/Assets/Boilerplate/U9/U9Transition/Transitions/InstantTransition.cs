using UnityEngine;
using System.Collections;

public class InstantTransition : U9Transition {
	
	U9Transition childTransition;
	
	public InstantTransition( U9Transition childTransition ) {
		this.childTransition = childTransition;
	}
	
	public override void Begin ()
	{
		base.Begin ();
		if( childTransition != null ) {
			childTransition.Begin();
		}
		End();
	}
	
}
