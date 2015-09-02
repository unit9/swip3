using UnityEngine;
using System.Collections;

public class HeaderView : U9View 
{
	[SerializeField]
	U9Button _ExitButton, _InformationButton, _LeaderboardsButton;

	[SerializeField]
	U9View _InfoView;

	[SerializeField]
	GameObject _Logo;

	[SerializeField]
	U9View _QuitView;

	protected override void Awake ()
	{
		base.Awake ();
		_ExitButton.Clicked += HandleExitClicked;
		_InformationButton.Clicked += HandleInfoClicked;

		if(GameController.Inst.Platform == Platform.Phone)
		{
			_LeaderboardsButton.Clicked += HandleLeaderboardClicked;
		}
	}

	void HandleLeaderboardClicked (object sender, System.EventArgs e)
	{

	}

	void HandleInfoClicked (object sender, System.EventArgs e)
	{
		GameController.Inst.SwitchToView(GameController.View.Info);
	}
	
	void HandleExitClicked (object sender, System.EventArgs e)
	{
		if(GameController.Inst.Platform == Platform.Web)
		{
			Application.Quit ();
		}
		else
		{
			if(!GameController.Inst.GameCreated)
			{
				GameController.Inst.StartGame();
				GameController.Inst.SwitchToView(GameController.View.Game);
			}
			else
				GameController.Inst.SwitchToView(GameController.View.Game);
		}
	}

	void Update()
	{


		if(GameController.Inst.Platform == Platform.Web)
		{
			if(GameController.View.Game == GameController.Inst.ECurrentView)
			{
				_ExitButton.gameObject.SetActive(false);
				_InformationButton.gameObject.SetActive(true);

			}
			else if(_InformationButton.gameObject.activeSelf)
			{
				_ExitButton.gameObject.SetActive(true);
				_InformationButton.gameObject.SetActive(false);
			}
		}
		else
		{		
			if (Input.GetKeyDown(KeyCode.Escape) && GameController.View.Quit != GameController.Inst.ECurrentView) 
			{ 
				GameController.Inst.SwitchToView(GameController.View.Quit);
				_QuitView.GetDisplayTransition().Begin();
			}

			if(GameController.View.Info == GameController.Inst.ECurrentView
			   ||GameController.View.Intro == GameController.Inst.ECurrentView)
			{
				_Logo.SetActive(false);
				_ExitButton.gameObject.SetActive(true);
				_InformationButton.gameObject.SetActive(false);
				_LeaderboardsButton.gameObject.SetActive(false);
				
			}
			else if(!(GameController.View.Quit == GameController.Inst.ECurrentView && GameController.View.Info == GameController.Inst.EPrevView))
			{
				_Logo.SetActive(true);
				_ExitButton.gameObject.SetActive(false);
				_InformationButton.gameObject.SetActive(true);
				_LeaderboardsButton.gameObject.SetActive(true);
			}
		}
	}
	
	#region implemented abstract members of U9View
	protected override U9Transition CreateDisplayTransition (bool force)
	{
		throw new System.NotImplementedException ();
	}
	protected override U9Transition CreateHideTransition (bool force)
	{
		throw new System.NotImplementedException ();
	}
	#endregion
}
