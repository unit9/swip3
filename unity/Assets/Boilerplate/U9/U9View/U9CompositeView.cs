using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class U9CompositeView : U9View {
	
	[SerializeField]
	U9View[] additionalViews;
	
	public enum CompositionType {
		Parallel,
		Serial,
		Stagger
	}
	[SerializeField]
	CompositionType compositionType;
	
	protected override void Awake ()
	{
		base.Awake ();
//		foreach( U9View v in additionalViews ) {
//			AddDependentView(v);
//		}
	}
	
	#region implemented abstract members of U9View
	protected override U9Transition CreateDisplayTransition ( bool force )
	{
		List<U9Transition> ts = new List<U9Transition>();
//		foreach( U9View v in DependentViews ) {
//			ts.Add( v.GetDisplayTransition(force) );
//		}
		foreach( U9View v in additionalViews ) {
			ts.Add( v.GetDisplayTransition(force) );
		}	
		return CreateCompositeTransition( ts.ToArray() );
	}

	protected override U9Transition CreateHideTransition ( bool force )
	{
		List<U9Transition> ts = new List<U9Transition>();
//		foreach( U9View v in DependentViews ) {
//			ts.Add( v.GetHideTransition(force) );
//		}
		foreach( U9View v in additionalViews ) {
			ts.Add( v.GetHideTransition(force) );
		}		
		ts.Reverse();
		return CreateCompositeTransition( ts.ToArray() );
	}
	
	#endregion
	
	public override void Hide ()
	{
		base.Hide ();
//		foreach( U9View v in DependentViews ) {
//			v.Hide();
//		}
		foreach( U9View v in additionalViews ) {
			v.Hide();
		}
	}
	
	public override void Display ()
	{
		base.Display ();
//		foreach( U9View v in DependentViews ) {
//			v.Display();
//		}
		foreach( U9View v in additionalViews ) {
			v.Display();
		}
	}
	
	U9Transition CreateCompositeTransition( U9Transition[] ts ) {
		switch( compositionType ) {
			case CompositionType.Parallel:
				return U9T.P ( ts );
			case CompositionType.Serial:
				return U9T.S ( ts );
			case CompositionType.Stagger:
				return U9T.Stagger ( 0.1f, ts );
			default:
				return null;
		}
	}

	protected override void EnableInteractionInternal ()
	{
		base.EnableInteractionInternal ();
		foreach( U9View v in additionalViews ) {
			v.EnableInteraction ();
		}
	}

	protected override void DisableInteractionInternal ()
	{
		base.DisableInteractionInternal ();
		foreach( U9View v in additionalViews ) {
			v.DisableInteraction ();
		}
	}
}
