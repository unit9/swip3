using UnityEngine;
// U9NullTransition.cs
// 
// Created by Nick McVroom-Amoakohene <nicholas@unit9.com> on 06/09/2012.
// Based on code by David Rzepa <dave@unit9.com>
// Copyright (c) 2012 unit9 ltd. www.unit9.com. All rights reserved

public class U9NullTransition : U9Transition {
	
	public U9NullTransition() : base() { }

	public override bool IsNull {
		get {
			return true;
		}
	}

	#region implemented abstract members of U9Transition
	public override void Begin() {
		base.Begin();
		End();
	}

	public override void Stop() {
		base.Stop();
	}
	#endregion
	
	protected override void End ()
	{
		base.End ();
	}
}

