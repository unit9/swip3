using UnityEngine;
using System.Collections;

public class AsymSlideView : U9SlideView {

	protected override void BeginDisplay ()
	{
		base.BeginDisplay ();
		transform.localPosition = displayPosition - hideOffset;
	}
}
