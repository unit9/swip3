// SLideView.cs
// 
// Created by Nick McVroom-Amoakohene <nicholas@unit9.com> on 08/10/2012.
// 
// Copyright (c) 2012 unit9 ltd. www.unit9.com. All rights reserved
using UnityEngine;
using System.Collections;
//using Holoville.HOTween;

public class U9SlideView : U9TransitionView {
	
	[SerializeField]
	protected Vector3 hideOffset;
	
	protected Vector3 displayPosition;
	
	protected override void InitView ()
	{
		TarePosition ();
		base.InitView ();
	}

	public void TarePosition() {
		displayPosition = transform.localPosition;
		}

	protected override U9Transition CreateDisplayTransition ( bool force )
	{
		return U9T.T ( iTween.MoveTo, gameObject, iTween.Hash("position", displayPosition, "isLocal", true, "easetype", transitionEaseType, "time", transitionDuration, "ignoretimescale", ignoreTimeScale ) );	
	}
	
	protected override U9Transition CreateHideTransition ( bool force )
	{
		return U9T.T ( iTween.MoveTo, gameObject, iTween.Hash("position", displayPosition+hideOffset, "isLocal", true, "easetype", transitionEaseType, "time", transitionDuration, "ignoretimescale", ignoreTimeScale ) );
	}
	
	public override void Display ()
	{
		base.Display ();
		transform.localPosition = displayPosition;
	}
	
	public override void Hide ()
	{
		transform.localPosition = displayPosition+hideOffset;
		base.Hide ();
	}
	
}

