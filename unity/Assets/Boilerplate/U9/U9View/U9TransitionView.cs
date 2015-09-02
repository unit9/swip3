using UnityEngine;
using System.Collections;

public abstract class U9TransitionView : U9View
{

	[SerializeField]
#if USE_HOTWEEN
	protected EaseType easeType = EaseType.EaseInOutQuad;
#else
	protected iTween.EaseType transitionEaseType = iTween.EaseType.easeInOutQuad;
#endif
	
	[SerializeField]
	protected float transitionDuration = 0.35f;
	
	[SerializeField]
	protected bool ignoreTimeScale = true;
	
}

