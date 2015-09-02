using UnityEngine;
using System.Collections;

public class QuitView : U9CompositeView {

	[SerializeField]
	U9Button yesButton = null, noButton = null;

	[SerializeField]
	protected GameController.View nextView;

	protected override void Awake ()
	{
		base.Awake ();
		yesButton.Clicked += HandleYesButtonClicked;
		noButton.Clicked += HandleNoButtonClicked;
	}

	void HandleNoButtonClicked (object sender, System.EventArgs e)
	{
		GameController.Inst.SwitchToView(GameController.Inst.EPrevView);
		//GetHideTransition().Begin();
	}

	void HandleYesButtonClicked (object sender, System.EventArgs e)
	{
		Application.Quit ();
	}


}
