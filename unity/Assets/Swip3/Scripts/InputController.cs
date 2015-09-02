using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {


	[SerializeField]
	SwipeRecognizer swipeRecognizer = null;
	
	bool waitingForInput = true;
	int PrevInputCount = 0;

	void Awake() {
		swipeRecognizer.OnGesture += HandleOnGesture;
	}

	/// <summary>
	/// Listens for swipe gestures and takes the player's move
	/// </summary>
	/// <param name="gesture">Gesture.</param>
	void HandleOnGesture (SwipeGesture gesture)
	{

		if (gesture.Direction == FingerGestures.SwipeDirection.Left) {
			TakePlayerMove( GameController.Edge.Right );
		} else if (gesture.Direction == FingerGestures.SwipeDirection.Up) {
			TakePlayerMove( GameController.Edge.Bottom );
		} else if (gesture.Direction == FingerGestures.SwipeDirection.Right) {
			TakePlayerMove( GameController.Edge.Left );
		} else if (gesture.Direction == FingerGestures.SwipeDirection.Down) {
			TakePlayerMove( GameController.Edge.Top );
		}

	}

	/// <summary>
	/// Listen for arrow key input and take the player's move
	/// </summary>
	void Update() 
	{	
		if(GameController.Inst.Platform == Platform.Web)
			if(GameController.Inst.WebCom.InputUpdate > PrevInputCount)
			{
				PrevInputCount++;
				ReciveInput(GameController.Inst.WebCom.CurrInput);
			}

		if( Input.GetKeyDown(KeyCode.UpArrow) ) {
			TakePlayerMove( GameController.Edge.Bottom );
		} else if( Input.GetKeyDown(KeyCode.DownArrow) ) {
			TakePlayerMove( GameController.Edge.Top );
		} else if( Input.GetKeyDown(KeyCode.RightArrow) ) {
			TakePlayerMove( GameController.Edge.Left );
		} else if( Input.GetKeyDown(KeyCode.LeftArrow) ) {
			TakePlayerMove( GameController.Edge.Right );
		}

	}

	public void ReciveInput(int inPut)
	{

			switch(inPut)
			{
			case 0:
				TakePlayerMove( GameController.Edge.Bottom );
				break;
			case 1:
				TakePlayerMove( GameController.Edge.Top );
				break;
			case 2:
				TakePlayerMove( GameController.Edge.Left );
				break;
			case 3:
				TakePlayerMove( GameController.Edge.Right );
				break;
			}

	}

	/// <summary>
	/// Takes the player move and then waits until animations have completed before accepting any more input.
	/// </summary>
	/// <param name="edge">Edge.</param>
	void TakePlayerMove( GameController.Edge edge ) 
	{

		switch(GameController.Inst.ECurrentView)
		{
			case GameController.View.Intro:
				GameController.Inst.SwitchToView(GameController.View.Game);
				GameController.Inst.StartGame();
				break;
			case GameController.View.Info:
				GameController.Inst.SwitchToView(GameController.View.Game);
				break;
			case GameController.View.Highscore:
				GameController.Inst.SwitchToView(GameController.View.Game);
				if(GameController.Inst.Platform != Platform.Web)
					GameController.Inst.StartGame();
				break;
			case GameController.View.Newscore:
				GameController.Inst.SwitchToView(GameController.View.Game);
				if(GameController.Inst.Platform != Platform.Web)
					GameController.Inst.StartGame();
			break;
			case GameController.View.Game:
			{
				if(GameController.Inst.GameCreated)
					if (waitingForInput) 
					{
						waitingForInput = false;
						U9Transition t = GameController.Inst.CreatePlayerMoveTransition(edge);
					
						t.Ended += (transition) => {
							waitingForInput = true;
						};
						t.Begin ();
					}
				break;
			}
		}
	}
}
