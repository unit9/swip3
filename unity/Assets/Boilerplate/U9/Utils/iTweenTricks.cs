using UnityEngine;
using System.Collections;

public class iTweenTricks : MonoBehaviour {

	[SerializeField]
	AnimationCurve xAnimation, yAnimation, rotAnimation, scaleAnimation, alphaAnimation;

	[SerializeField]
	float time = 0.3f, timeJitter = 0f;

	[SerializeField]
	iTween.LoopType loopType = iTween.LoopType.pingPong;

	Vector3 startPos;
	float startRot;
	Vector3 startScale;
	
	Transform cachedTransform;

	[SerializeField]
	UISprite[] sprites;

	[SerializeField]
	bool ignoreTimescale = false;

	void Start() {
		cachedTransform = transform;
		startPos = cachedTransform.localPosition;
		startRot = cachedTransform.localEulerAngles.z;
		startScale = cachedTransform.localScale;
		iTween.ValueTo ( gameObject, iTween.Hash ( "from", 0f, "to", 1f, "onupdate", "UpdateTricks", "time", time + Random.Range(-timeJitter,timeJitter), "easetype", iTween.EaseType.linear, "looptype", loopType, "ignoretimescale", ignoreTimescale ));
	}

	void UpdateTricks( float param ) {
		cachedTransform.localPosition = startPos + new Vector3 (xAnimation.Evaluate (param), yAnimation.Evaluate (param), 0f);
		cachedTransform.localRotation = Quaternion.Euler( 0f, 0f, startRot + rotAnimation.Evaluate(param) );
		cachedTransform.localScale = startScale + Vector3.one*scaleAnimation.Evaluate (param);

		float alpha = alphaAnimation.Evaluate (param);
		for (int i = 0; i < sprites.Length; i++) {
			sprites [i].alpha = alpha;
		}
	}

}
