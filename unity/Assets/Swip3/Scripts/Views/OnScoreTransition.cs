using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class OnScoreTransition : MonoBehaviour
{
	public Projector Proj;

	public U9Transition CreateOnScoreTransition(List<Block> blocks, int layer, int tScore) 
	{
		int layerMask = 1 << layer;
		layerMask = ~layerMask;
		Proj.ignoreLayers = layerMask;

		U9Transition onScoreTransition = null;

		transform.parent = blocks[blocks.Count/2].transform;
		transform.localScale = Vector3.one;

		//onScoreTransition = U9T.T (iTween.MoveTo, this.gameObject, iTween.Hash("position", new Vector3(0, 0, -1000), "time", 0.4f, "islocal", true, "easetype", iTween.EaseType.easeInOutQuad));

		//onScoreTransition.Ended += (transition) =>
		//{
			float staggerTime = 0f;//0.45f;
			List<U9Transition> transitions = new List<U9Transition> ();
			foreach (Block m in blocks) 
			{
				transitions.Add (m.CreateDisappearTransition (tScore));
			}

			staggerTime = staggerTime/transitions.Count;

			U9Transition t = U9T.Stagger ( staggerTime, transitions.ToArray ());
			t.Begin();

			Destroy (this.gameObject); 
		//};
		
		return new InstantTransition( onScoreTransition );
	}

}


