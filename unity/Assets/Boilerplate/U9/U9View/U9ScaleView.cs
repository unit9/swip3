// U9ScaleView.cs
// 
// Created by David Rzepa on 23/11/2012.
// 
// Copyright (c) 2012 unit9 ltd. www.unit9.com. All rights reserved
using UnityEngine;
using System.Collections;

public class U9ScaleView : U9TransitionView {
	
	[SerializeField]
	protected Vector3 displayScale = Vector3.one;
	
	[SerializeField]
	protected Vector3 hideScale = Vector3.zero;

	protected override void InitView ()
	{
		if( hideScale == Vector3.zero ) {
			hideScale = 0.01f*Vector3.one;
		}
		base.InitView ();
	}

	#region implemented abstract members of U9View
	protected override U9Transition CreateDisplayTransition ( bool force )
	{
		return U9T.T( iTween.ScaleTo, gameObject, iTween.Hash( "scale", displayScale, "time", transitionDuration, "easetype", transitionEaseType, "ignoretimescale", ignoreTimeScale ) );
	}

	protected override U9Transition CreateHideTransition ( bool force )
	{
		return U9T.T( iTween.ScaleTo, gameObject, iTween.Hash( "scale", hideScale, "time", transitionDuration, "easetype", transitionEaseType, "ignoretimescale", ignoreTimeScale ) );
	}
	#endregion
	
	protected override void EndHide ()
	{
		base.EndHide ();
		transform.localScale = hideScale;
	}

}

