using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

using UnityEngine.SocialPlatforms;
#if UNITY_IPHONE
using UnityEngine.SocialPlatforms.GameCenter;
#endif

public class GameController : MonoBehaviour
{
	
	/***********************/
	/******** ENUMS ********/
	/***********************/
	
	public enum Edge
	{
		Top,
		Bottom,
		Left,
		Right
	}
	
	public enum View
	{
		Intro = 0,
		Game = 1,
		Info = 2,
		Quit = 3,
		Highscore = 4, 
		Newscore = 5
	}
	
	struct Blocks
	{
		public Block block;
		public GameObject plane;
	}
	
	/***********************************/
	/******** GAMEPLAY SETTINGS ********/
	/***********************************/
	
	[SerializeField]
	Block blockPrefab = null, backgroundBlockPrefab = null;
	
	[SerializeField]
	GameObject projectionPrefab = null;
	
	[SerializeField]
	int gridSize = 7, numberOfEdgeSpawns = 3, blockSize = 53, blockPaddingSize = 1, edgeGapSize = 10;
	
	[SerializeField]
	Color[] colorPalette;

	[SerializeField]
	ColorManager colorManager;
	
	/*****************************/
	/******** UI SETTINGS ********/
	/*****************************/
	
	[SerializeField]
	GameOverView normalGameOverView = null, newHighScoreGameOverView = null;
	
	[SerializeField]
	UILabel currentScoreLabel = null, bestScoreLabel = null;
	
	[SerializeField]
	U9Button infoButton = null, leaderboardsButton = null;
	
	[SerializeField]
	UISprite infoButtonSprite = null;
	
	[SerializeField]
	SwipeRecognizer swipeRecognizer = null;
	
	[SerializeField]
	GameObject _TrialRenderer;
	
	[SerializeField]
	GameObject _OnScore;
	
	[SerializeField]
	GameObject _OnMove;
	
	[SerializeField]
	GameObject _IntroEffect, _IntroLogo;
	
	/***********************************/
	/******** PRIVATE VARIABLES ********/
	/***********************************/
	
	private Block[,] blocks;
	private Vector3 gridBaseline;

	// real dangerous stuff..
	// Haxingtown
	private bool haxForSideBlocksNotSpawningInCorrectNumber; // don't mess with this 'cos if You do so Santa won't come to You this year

	/***********************************/
	/******** PUBLIC ACCESSORS *********/
	/***********************************/
	
	public WebComunication WebCom;

	public Platform Platform;
	
	public static GameController Inst { get; private set; }
	
	public U9View introView = null, infoView = null, quitView = null, gameView = null;
	public GameOverView highScoreView, newScoreView;
	
	int scoresToAdd;
	int score;
	int newScore; // needs to be here...

	public int Score 
	{
		get { return score; } 
		set 
		{ 
			scoresToAdd += value - score;
			score = value; 
		} 
	}
	
	int highScore;
	public int HighScore { get { return highScore; } set { highScore = value; PlayerPrefs.SetInt("HighScore",value); PlayerPrefs.Save(); bestScoreLabel.text = value.ToString(); } }
	
	bool gameIsOver = false;
	
	List<Blocks> BackgroundBlocks;
	
	private View _ECurrentView = View.Intro;
	public View EPrevView = View.Highscore;
	public View ECurrentView
	{
		get { return _ECurrentView; }
		set
		{
			switch(value)
			{
			case View.Game:
				EPrevView = _ECurrentView;
				_ECurrentView = View.Game;
				if(Platform == Platform.Web)
					WebCom.CurrView = (int)_ECurrentView;
				CurrentView = gameView;
				break;
			case View.Info:
				EPrevView = _ECurrentView;
				_ECurrentView = View.Info;
				if(Platform == Platform.Phone)
					CurrentView = introView;
				break;
			case View.Intro:
				EPrevView = _ECurrentView;
				_ECurrentView = View.Intro;
				if(Platform == Platform.Phone)
					CurrentView = introView;
				break;
			case View.Quit:
				EPrevView = _ECurrentView;
				_ECurrentView = View.Quit;
				if(Platform == Platform.Phone)
					CurrentView = quitView;
				break;
			case View.Highscore:
				EPrevView = _ECurrentView;
				_ECurrentView = View.Highscore;
				gameIsOver = false;
				if(Platform == Platform.Web)
					WebCom.CurrView = (int)_ECurrentView;
				CurrentView = highScoreView;
				break;
			case View.Newscore:
				EPrevView = _ECurrentView;
				_ECurrentView = View.Newscore;
				gameIsOver = false;
				if(Platform == Platform.Web)
					WebCom.CurrView = (int)_ECurrentView;
				CurrentView = newScoreView;
				break;
			}
		}
	}
	
	private U9View _CurrentView;
	public U9View CurrentView
	{
		get { return _CurrentView; }
		set 
		{ 
			if(_CurrentView != null)
			{
				if(value != quitView)
					_CurrentView.GetHideTransition().Begin();
				value.GetDisplayTransition().Begin();
			}
			_CurrentView = value;
		}
	}
	
	/********************************/
	/******** UNITY HANDLERS ********/
	/********************************/
	
	void Awake ()
	{

		if(SystemInfo.deviceModel == "Motorola Moto 360")
			RoundScreenMode();

		HOTween.Init(true, true, true);
		HOTween.EnableOverwriteManager();

		Inst = this;
		HighScore = PlayerPrefs.GetInt ("HighScore", 0);
		
		if(Platform == Platform.Web)
		{
			Application.ExternalCall("SetHighScore", HighScore);
		}
		
		ECurrentView = View.Game;
		StartCoroutine(StartGameIE());

#if UNITY_IPHONE
		GameCenterSingleton.Instance.Initialize();
#endif
		
		if(Platform == Platform.Phone)
			leaderboardsButton.Clicked += HandleLeaderboardsButtonClicked;

	}

	void RoundScreenMode()
	{
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		Transform container = transform.parent;
		container.localScale = new Vector3(0.65f,0.65f,0.65f);
		container.localPosition = new Vector3(-0.3f,-43f,0);

	}
	
	public bool GameCreated = false;
	
	public IEnumerator StartGameIE()
	{

		colorPalette = colorManager.getBlockColors ();

		foreach(Transform t in transform)
		{
			GameObject.Destroy(t.gameObject);
		}
		
		BackgroundBlocks = new List<Blocks>();
		
		// Initialise space for the blocks in the grid
		blocks = new Block[gridSize, gridSize];
		
		// Calculate the bottom left position of the grid
		gridBaseline = -0.5f*new Vector3 ( ((blockPaddingSize+blockSize)*(gridSize-1)), (blockPaddingSize+blockSize) * (gridSize-1), 0f);
		
		gridBaseline.y += 60.0f;
		
		// Initialise Game Services
		GooglePlayGames.PlayGamesPlatform.Activate ();

		// Spawn the static background blocks
		for (int j =  gridSize-2, nj = 0; j > nj; j--) {
			for (int i = 1, ni = gridSize-1; i < ni; i++) {

				Block backgroundBlockInstance = (Block) Instantiate( blockPrefab );

				backgroundBlockInstance.SquareSize = blockSize;
				backgroundBlockInstance.Color = HexColor.ColorFromHex("D2D2D2"/*"E9ECEE"*/);
				backgroundBlockInstance.transform.parent = transform;
				backgroundBlockInstance.transform.localPosition = GridPosTo3DPos(i,j,-10);
				backgroundBlockInstance.transform.localScale = Vector3.one;// * 0.001f;
				backgroundBlockInstance.IsBackground = true;
				backgroundBlockInstance.gameObject.layer =  8;

				Blocks sBlock = new Blocks();
				sBlock.block = backgroundBlockInstance;
				
				BackgroundBlocks.Add (sBlock);

				//U9Transition transitions = U9T.HOT (HOTween.To, backgroundBlockInstance.gameObject.transform, 0.3f, new TweenParms().Prop("localScale", Vector3.one,false).Ease(EaseType.EaseOutExpo));
				U9Transition transitions = U9T.LT (LeanTween.scale, backgroundBlockInstance.gameObject, Vector3.one, 0.3f, iTween.Hash("ease", LeanTweenType.easeOutExpo));
				//U9Transition transitions = U9T.T (iTween.ScaleTo, backgroundBlockInstance.gameObject, iTween.Hash ("scale", new Vector3(1,1,1), "time", 0.3f, "islocal", true, "easetype", iTween.EaseType.easeOutExpo ));

				transitions.Begin();

				yield return new WaitForSeconds(0.05f);
				
			}
		}

		
		GameObject intro = (GameObject)Instantiate(_IntroEffect);
		
		StartNewGame ();
		
		GameCreated = true;
	}
	
	public void StartGame()
	{
		colorPalette = colorManager.getBlockColors ();

		foreach(Transform t in transform)
		{
			GameObject.Destroy(t.gameObject);
		}

		BackgroundBlocks = new List<Blocks>();
		
		// Initialise space for the blocks in the grid
		blocks = new Block[gridSize, gridSize];
		
		// Calculate the bottom left position of the grid
		gridBaseline = -0.5f*new Vector3 ( ((blockPaddingSize+blockSize)*(gridSize-1)), (blockPaddingSize+blockSize) * (gridSize-1), 0f);
		
		gridBaseline.y += 60.0f;

		// Spawn the static background blocks
		for (int i = 1, ni = gridSize-1; i < ni; i++)
		{
			for (int j = 1, nj = gridSize-1; j < nj; j++) 
			{
		
				Block backgroundBlockInstance = (Block) Instantiate( blockPrefab );
		
				backgroundBlockInstance.SquareSize = blockSize;
				backgroundBlockInstance.Color = HexColor.ColorFromHex("D2D2D2"/*"E9ECEE"*/);
				backgroundBlockInstance.transform.parent = transform;
				backgroundBlockInstance.transform.localScale = Vector3.one;
				backgroundBlockInstance.transform.localPosition = GridPosTo3DPos(i,j,-10);
				backgroundBlockInstance.IsBackground = true;
				backgroundBlockInstance.gameObject.layer =  8;
		
				Blocks sBlock = new Blocks();
				sBlock.block = backgroundBlockInstance;
		
				BackgroundBlocks.Add (sBlock);
				
			}
		}

		StartNewGame ();
		
		GameCreated = true;
	}
	
	void Update() 
	{
		AddScore();
		
		if(Platform == Platform.Web)
			if(WebCom.CurrView != (int)_ECurrentView)
		{
			SwitchToView(WebCom.CurrView);
		}
	}
	
	private float _Timer;
	
	private void AddScore()
	{
		if(_Timer < Time.time)
		{
			if(scoresToAdd > 0)
			{
				_Timer = Time.time + (1.0f/scoresToAdd) * 0.2f;
				scoresToAdd--;
				currentScoreLabel.text = (score -scoresToAdd).ToString(); 
			}
		}
	}
	
	private void MoveIntro(Edge edge)
	{
		Vector3 MoveTo = new Vector3(0,0,0);
		
		switch(edge)
		{
		case Edge.Bottom:
			MoveTo = new Vector3(0, 800, 0);
			break;
		case Edge.Left:
			MoveTo = new Vector3(800, _IntroLogo.transform.position.y, 0);
			break;
		case Edge.Right:
			MoveTo = new Vector3(-800, _IntroLogo.transform.position.y, 0);
			break;
		case Edge.Top:
			MoveTo = new Vector3(0, -800, 0);
			break;
		}

		//U9Transition transitions = U9T.HOT(HOTween.To, _IntroLogo.transform, 0.2f, new TweenParms().Prop("localPosition", MoveTo, false).Ease(EaseType.EaseOutQuad));

		U9Transition transitions = U9T.LT (LeanTween.moveLocal, _IntroLogo, MoveTo, 0.2f, iTween.Hash("ease", LeanTweenType.easeOutQuad));

	
		transitions.Ended += (transition) => 
		{
			Destroy(_IntroLogo);
		};
		U9T.S (U9T.W (0.2f), transitions).Begin();
	}
	
	/****************************/
	/******** GAME LOGIC ********/
	/****************************/
	
	public void StartNewGame() 
	{
		Score = 0;
		if (Platform == Platform.Web) 
		{
			Application.ExternalCall ("StartGame");
		}
		// Spawn the initial side blocks
		SpawnBlocks (Edge.Top);
		SpawnBlocks (Edge.Bottom);
		SpawnBlocks (Edge.Left);
		SpawnBlocks (Edge.Right);

		//!!!!!!!!!!!
		// You "can't touch this!"
		haxForSideBlocksNotSpawningInCorrectNumber = false;
		//!!!!!!!!!!!
	}
	
	/// <summary>
	/// Restarts the game.
	/// </summary>
	/// <param name="displayIntro">If set to <c>true</c> display intro.</param>
	void RestartGame( bool displayIntro ) {
		
		gameIsOver = false;
		U9Transition t;
		t = U9T.S ( U9T.P( displayIntro ? introView.GetDisplayTransition() : null ));
		t.Begin ();
		
	}
	
	/// <summary>
	/// Spawns new blocks on the specified side
	/// </summary>
	/// <param name="side">Side.</param>
	void SpawnBlocks (Edge edge)
	{
		// Create a list of possible spawn points
		List<int> possibleSpawnPoints = new List<int> ();
		int numEdgeSpawnsRequired = numberOfEdgeSpawns;

		if ( haxForSideBlocksNotSpawningInCorrectNumber == false )
		{
		#region What should be done
			switch (edge) {
			case Edge.Top:
				
				for (int i = 1, ni = gridSize-1; i < ni; i++) {
					if (blocks [i, gridSize - 1]) {
						numEdgeSpawnsRequired--;
					} else {
						possibleSpawnPoints.Add (i);
					}
				}
				
				break;
			case Edge.Bottom:
				
				for (int i = 1, ni = gridSize-1; i < ni; i++) {
					if (blocks [i, 0]) {
						numEdgeSpawnsRequired--;
					} else {
						possibleSpawnPoints.Add (i);
					}
				}
				
				break;
			case Edge.Left:
				
				for (int i = 1, ni = gridSize-1; i < ni; i++) {
					if (blocks [0, i]) {
						numEdgeSpawnsRequired--;
					} else {
						possibleSpawnPoints.Add (i);
					}
				}
				
				break;
			case Edge.Right:
				
				for (int i = 1, ni = gridSize-1; i < ni; i++) {
					if (blocks [gridSize - 1, i]) {
						numEdgeSpawnsRequired--;
					} else {
						possibleSpawnPoints.Add (i);
					}
				}
				break;
			}
		#endregion
		}
		else 
		{
		#region What i am forced to do...
			for (int i = 1, ni = gridSize-1; i < ni; i++) {
				possibleSpawnPoints.Add (i);
			}
		#endregion
		}

		// Select a subset of spawn points from the list of possible spawns.
		List<int> spawnPoints = GetRandomSubset<int> (possibleSpawnPoints, numEdgeSpawnsRequired);
		
		
		// Create a list of transitions for each of the spawns
		List<U9Transition> transitions = new List<U9Transition> ();
		switch (edge) {
		case Edge.Top:
			
			for (int i = 0, ni = spawnPoints.Count; i < ni; i++) {
				transitions.Add( SpawnBlock (spawnPoints [i], gridSize - 1) );
			}
			
			break;
		case Edge.Bottom:
			
			for (int i = 0, ni = spawnPoints.Count; i < ni; i++) {
				transitions.Add( SpawnBlock (spawnPoints [i], 0) );
			}
			
			break;
		case Edge.Left:
			
			for (int i = 0, ni = spawnPoints.Count; i < ni; i++) {
				transitions.Add( SpawnBlock (0, spawnPoints [i]) );
			}
			
			break;
		case Edge.Right:
			
			for (int i = 0, ni = spawnPoints.Count; i < ni; i++) {
				transitions.Add( SpawnBlock (gridSize - 1, spawnPoints [i]) );
			}
			break;
		}

		U9T.P (transitions.ToArray ()).Begin ();
	}
	
	/// <summary>
	/// Gets the size of the colour pallette.
	/// </summary>
	/// <returns>The colour pallette size, which is based on score.</returns>
	int GetColourPalletteSize() {
		return Mathf.Min( Mathf.FloorToInt (3 + Mathf.Pow (Score, 1f / 3f) / 4f), colorPalette.Length );
	}
	
	/// <summary>
	/// Instantiates a block and returns a transition to move it into place
	/// </summary>
	/// <returns>The spawn transition.</returns>
	/// <param name="i">The index.</param>
	/// <param name="j">J.</param>
	U9Transition SpawnBlock (int i, int j)
	{
		Block b = (Block) Instantiate( blockPrefab );
		
		b.transform.parent = transform;
		b.transform.localPosition = GridPosTo3DPos (i, j, blockSize);
		b.transform.localScale = Vector3.one;
		b.Color = colorPalette [Random.Range (0, GetColourPalletteSize() )];

		Color temporaryColorHolder = b.Color;
		temporaryColorHolder.a *= 0.5f;
				
		b.Ghost = true;
		b.SquareSize = blockSize;
		b.I = i;
		b.J = j;
		blocks [i, j] = b;
		
		//return U9T.HOT(HOTween.To, b.gameObject.transform, 0.2f, new TweenParms().Prop("localPosition", GridPosTo3DPos (i, j, 0), false).Ease(EaseType.EaseOutQuad));
		//return U9T.T (iTween.MoveTo, b.gameObject, iTween.Hash ("position", GridPosTo3DPos (i, j, 0), "time", 0.2f, "islocal", true, "easetype", iTween.EaseType.easeOutQuad ));
		return U9T.LT (LeanTween.moveLocal, b.gameObject, GridPosTo3DPos (i, j, 0), 0.2f, iTween.Hash("ease", LeanTweenType.easeOutCirc));
		
	}
	
	/// <summary>
	/// Creates a transition which shifts blocks away from the given edge, checks for any matched blocks, and triggers game over if the board is full.
	/// </summary>
	/// <returns>The shift blocks transition.</returns>
	/// <param name="edge">Edge.</param>
	public U9Transition CreatePlayerMoveTransition (Edge edge)
	{
		if(_IntroLogo != null)
			MoveIntro(edge);

		for (int i = 1, ni = gridSize-1; i < ni; i++) 
		{
			for (int j = 1, nj = gridSize-1; j < nj; j++) 
			{
				Block b = blocks [i, j];
			}
		}

		newScoreView.GetHideTransition().Begin();
		highScoreView.GetHideTransition().Begin();

		// If already gameover then ignore this request and return a null transition.
		if (gameIsOver) {
			RestartGame(false);
		}
		
		// If the intro is displaying then hide it.
		if (introView != null && introView.IsDisplaying) {
			introView.GetHideTransition().Begin();
		}

		U9Transition t = U9T.P (CreateShiftBlocksTransition(edge) );

		// After the player move transition, if the game is over then start a new game, otherwise spawn blocks on the side that was just shifted
		t.Ended += (transition) => {
			
			U9Transition d = CreateMatchesTransition ();
			U9Transition gameOver = IsGameOver () ? CreateGameOverTransition() : null;
			if(gameOver != null)
			{
				gameOver.Ended += (transitionOver) => 
				{
					if( gameIsOver ) 
					{
						if(Platform == Platform.Web)
						{
							Application.ExternalCall("SetHighScore", HighScore);
							Application.ExternalCall("GameOver");
						}
						//!!!
						haxForSideBlocksNotSpawningInCorrectNumber = true;
						//!!!

						score = 0;
						scoresToAdd = 0;
						newScore = 0;

						_Timer = 0;
						
						StartGame();

						currentScoreLabel.text = "0";
						_ECurrentView = View.Game;
					}
				};
				gameOver.Begin();
			}
			else 
			{
				if(d != null) 
					d.Begin();
				
				SpawnBlocks (edge);
			}
		};
		return t;
	}
	
	/// <summary>
	/// Creates a transition that hifts all of the blocks on the board away from the given edge.
	/// Note that this must be done in a specific order - blocks that are furthest away from the
	/// given edge must be shifted first or blocks will be blocked by other blocks that have yet
	/// to move out of the way
	/// </summary>
	/// <returns>The shift blocks transition.</returns>
	/// <param name="edge">Edge.</param>
	U9Transition CreateShiftBlocksTransition( Edge edge ) {
		
		List<U9Transition> transitions = new List<U9Transition> ();
		
		switch (edge) {
		case Edge.Top:
			
			for (int i = 1, ni = gridSize-1; i < ni; i++) {
				for (int j = 1, nj = gridSize-1; j < nj; j++) {
					transitions.Add (CreateShiftBlockTransition (i, j, edge));
				}
			}
			
			for (int i = 1, ni = gridSize-1; i < ni; i++) {
				transitions.Add (CreateShiftBlockTransition (i, gridSize - 1, edge));
			}
			break;
		case Edge.Bottom:
			
			for (int i = 1, ni = gridSize-1; i < ni; i++) {
				for (int j = gridSize-2; j > 0; j--) {
					transitions.Add (CreateShiftBlockTransition (i, j, edge));
				}
			}
			
			for (int i = 1, ni = gridSize-1; i < ni; i++) {
				transitions.Add (CreateShiftBlockTransition (i, 0, edge));
			}
			break;
		case Edge.Left:
			
			for (int i = gridSize-2; i > 0; i--) {
				for (int j = 1, nj = gridSize-1; j < nj; j++) {
					transitions.Add (CreateShiftBlockTransition (i, j, edge));
				}
			}
			
			for (int i = 1, ni = gridSize-1; i < ni; i++) {
				transitions.Add (CreateShiftBlockTransition (0, i, edge));
			}
			break;
		case Edge.Right:
			
			for (int i = 1, ni = gridSize-1; i < ni; i++) {
				for (int j = 1, nj = gridSize-1; j < nj; j++) {
					transitions.Add (CreateShiftBlockTransition (i, j, edge));
				}
			}
			
			for (int i = 1, ni = gridSize-1; i < ni; i++) {
				transitions.Add (CreateShiftBlockTransition (gridSize - 1, i, edge));
			}
			break;
		}
		
		float staggerTime = 0.001f;//0.5f;
		staggerTime = staggerTime / transitions.Count;
		
		return U9T.Stagger (staggerTime, transitions.ToArray ());
		//return U9T.P (transitions.ToArray ());
	}
	
	/// <summary>
	/// Gets a random subset from the given list and of the given size.
	/// </summary>
	/// <returns>The random subset.</returns>
	/// <param name="set">Set.</param>
	/// <param name="subsetSize">Subset size.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	List<T> GetRandomSubset<T> (List<T> set, int subsetSize)
	{
		
		// Create a clone to avoid modifying the given set.
		List<T> clonedSet = new List<T> (set);
		
		List<T> subset = new List<T> ();
		for (int i = 0; i < subsetSize; i++) {
			int index = Random.Range (0, clonedSet.Count);
			subset.Add (clonedSet [index]);
			clonedSet.RemoveAt (index);
		}
		
		return subset;
	}
	
	/// <summary>
	/// Creates a transition that moves an individual as far as possible away from the given edge,
	/// until it collides with another block.
	/// </summary>
	/// <returns>The shift block transition.</returns>
	/// <param name="sourceI">Source i.</param>
	/// <param name="sourceJ">Source j.</param>
	/// <param name="edge">Edge.</param>
	U9Transition CreateShiftBlockTransition (int sourceI, int sourceJ, Edge edge)
	{
		int iDelta = 0, jDelta = 0;
		
		// Determine the direction to move in
		switch (edge) {
		case Edge.Top:
			iDelta = 0;
			jDelta = -1;
			break;
		case Edge.Bottom:
			iDelta = 0;
			jDelta = 1;
			break;
		case Edge.Left:
			iDelta = 1;
			jDelta = 0;
			break;
		case Edge.Right:
			iDelta = -1;
			jDelta = 0;
			break;
		}
		
		int destI = sourceI;
		int destJ = sourceJ;
		
		int tI = destI + iDelta;
		int tJ = destJ + jDelta;
		
		// Loop while the current position is within the main grid
		while (tI > 0 && tI < gridSize-1 && tJ < gridSize-1 && tJ > 0) {
			
			// If the current block is occupied then the current position is the final resting place. Otherwise keep moving.
			if (blocks [tI, tJ]) 
			{
				break;
			} 
			else 
			{
				destI = tI;
				destJ = tJ;
				tI += iDelta;
				tJ += jDelta;
			}
			
		}
		
		// Create the transition that actually moves the block
		return CreateMoveBlockTransition (sourceI, sourceJ, destI, destJ);
	}
	
	/// <summary>
	/// Returns a transition that moves the block in the source position to the destination position
	/// </summary>
	/// <returns>The move block transition.</returns>
	/// <param name="sourceI">Source i.</param>
	/// <param name="sourceJ">Source j.</param>
	/// <param name="destI">Destination i.</param>
	/// <param name="destJ">Destination j.</param>
	U9Transition CreateMoveBlockTransition (int sourceI, int sourceJ, int destI, int destJ)
	{
		Block b = blocks [sourceI, sourceJ];
		
		// If the source block does not exist then return a null transition.
		if (!b) {
			return U9T.Null ();
		}
	
		//!!
		// If source and dest positions are the same then return a null transition
		if (sourceI == destI && sourceJ == destJ) {
			//return U9T.T (iTween.ShakePosition, b.gameObject, iTween.Hash ("x", 0.05f, "y", 0.05f, "time", 0.3f, "islocal", false));
			return U9T.Null ();
		}
		//!!

		// If the destination position is already occupied then return a null transition
		if (blocks [destI, destJ]) {
			Debug.LogError ("Destination is already occupied by a block!");
			return U9T.Null ();
		}

		// Move the block in data
		blocks [sourceI, sourceJ] = null;
		blocks [destI, destJ] = b;
		
		// Update the blocks position
		b.I = destI;
		b.J = destJ;
		
		// Disable the fading on the block
		b.Ghost = false;

		// Return a tween to move the blocks gameObject

		//return U9T.HOT(HOTween.To, b.gameObject.transform, 0.3f, new TweenParms().Prop("localPosition", GridPosTo3DPos (destI, destJ, 0), false).Ease(EaseType.EaseOutCirc));
		//return U9T.T (iTween.MoveTo, b.gameObject, iTween.Hash ("position", GridPosTo3DPos (destI, destJ, 0), "time", 0.3f, "islocal", true, "easetype", iTween.EaseType.easeOutCirc ));
		return U9T.LT (LeanTween.moveLocal, b.gameObject, GridPosTo3DPos (destI, destJ, 0), 0.3f, iTween.Hash("ease", LeanTweenType.easeOutCirc));

	}
	
	
	/// <summary>
	/// Converts a grid-space position to a 3D-space position
	/// </summary>
	/// <returns>The 3D-space position.</returns>
	/// <param name="i">The index.</param>
	/// <param name="j">J.</param>
	/// <param name="additionalEdgeOffset">The amount of space to move outwards if the the position is on the edge.</param>
	Vector3 GridPosTo3DPos (int i, int j, int additionalEdgeOffset )
	{
		Vector3 pos = gridBaseline + new Vector3 (( blockPaddingSize + blockSize) * i, ( blockPaddingSize + blockSize ) * j, 0f);
		
		if (i == 0) {
			pos.x -= edgeGapSize + additionalEdgeOffset;
		} else if (i == gridSize - 1) {
			pos.x += edgeGapSize + additionalEdgeOffset;
		}
		
		if (j == 0) {
			pos.y -= edgeGapSize + additionalEdgeOffset;
		} else if (j == gridSize - 1) {
			pos.y += edgeGapSize + additionalEdgeOffset;
		}
		
		return pos;
	}
	
	/// <summary>
	/// Returns a transiton that animates and removes blocks, and grants a score for them
	/// </summary>
	/// <returns>The matches transition.</returns>
	U9Transition CreateMatchesTransition ()
	{
		List<U9Transition> transitions = new List<U9Transition> ();
		newScore = Score;
		//layer (14-9)
		int layer = 14; 
		// For every block in the grid that is not on an edge
		for (int i = 1, ni = gridSize-1; i < ni; i++) 
		{
			for (int j = 1, nj = gridSize-1; j < nj; j++) 
			{
				Block b = blocks [i, j];
				
				if( b ) 
				{
					List<Block> matchingBlocks = FindAdjacentMatchingBlocks (b.Color, i, j);
					
					if (matchingBlocks.Count >= 3) 
					{
						// The player receives score for each block, equal to the number of blocks matched
						int tScore = matchingBlocks.Count;		
						newScore += tScore*tScore;
						
						GameObject onScore = (GameObject)Instantiate (_OnScore, matchingBlocks[matchingBlocks.Count/2].transform.position, Quaternion.identity);
						OnScoreTransition onT = onScore.GetComponent<OnScoreTransition>();
//!!
						transitions.Add (onT.CreateOnScoreTransition(matchingBlocks, layer, tScore));
						
						// Add transitions to dissapear all matching blocks and display the score per block in their place
						foreach (Block m in matchingBlocks) 
						{
							
							blocks [m.I, m.J] = null;
							m.gameObject.layer = layer;
							GameObject BB = GetBackgroundBlock(m);
							if(BB != null)
								BB.gameObject.layer = layer;
						}
						layer--;
					}
					
					// Reset all blocks so that they can be checked again
					ResetCheckedForMatchesFlags ();
				}
			}
		}
		
		// Creates a stagger transition so that each subsequent match has a slight delay
		
		if(transitions.Count > 0)
		{
			U9Transition t = U9T.P (transitions.ToArray ());

			// Only update the score once all animations are completed
			t.Ended += (transition) => 
			{ 
				Score = newScore; 
				if(Platform == Platform.Web)
				{
					Application.ExternalCall("ScoreUpdate", Score); 
				}
				
			};
			return t;
		}
		return null;
	}
	
	/// <summary>
	/// Resets the checked for matches flags.
	/// </summary>
	void ResetCheckedForMatchesFlags ()
	{
		for (int i = 1, ni = gridSize-1; i < ni; i++) {
			for (int j = 1, nj = gridSize-1; j < nj; j++) {
				Block b = blocks [i, j];
				if( b ) {
					b.CheckedForMatches = false;
				}
			}
		}
	}
	
	GameObject GetBackgroundBlock(Block b)
	{
		float distance = 0;
		float maxDist = float.MaxValue;
		GameObject plane = null;
		
		for(int i = 0; i < BackgroundBlocks.Count; i++)
		{
			distance =  Vector3.Distance(BackgroundBlocks[i].block.transform.localPosition, b.transform.localPosition);
			if(distance < maxDist)
				//if(BackgroundBlocks[i].plane.transform.localPosition == b.transform.localPosition)
			{
				maxDist = distance;
				plane = BackgroundBlocks[i].plane;
			}
		}
		return plane;
	}
	
	/// <summary>
	/// Finds groups of blocks that match colour with the block in the given position. Also returns the block in the given position.
	/// </summary>
	/// <returns>The adjacent matching blocks.</returns>
	/// <param name="color">Color.</param>
	/// <param name="i">The index.</param>
	/// <param name="j">J.</param>
	List<Block> FindAdjacentMatchingBlocks (Color color, int i, int j)
	{
		List<Block> matchingBlocks = new List<Block> ();
		
		// If this position is on the edge then return the empty list
		if (i == 0 || i == gridSize - 1 || j == 0 || j == gridSize - 1) {
			return matchingBlocks;
		}
		
		Block b = blocks [i, j];
		
		// If this block has already been checked, or it does not match in colour then return the empty list
		if ( !b || b.CheckedForMatches || b.Color != color) {
			return matchingBlocks;
		}
		
		// Flag the block in this position as having been check already, so it's not checked again (avoids infinite loop)
		b.CheckedForMatches = true;
		
		// Add this block to the list
		matchingBlocks.Add (b);
		
		// Recurse into all adjacent positions
		matchingBlocks.AddRange (FindAdjacentMatchingBlocks (color, i + 1, j));
		matchingBlocks.AddRange (FindAdjacentMatchingBlocks (color, i - 1, j));
		matchingBlocks.AddRange (FindAdjacentMatchingBlocks (color, i, j + 1));
		matchingBlocks.AddRange (FindAdjacentMatchingBlocks (color, i, j - 1));
		
		return matchingBlocks;
	}
	
	/// <summary>
	/// Returns false if there is any space without a block in the grid, true otherwise
	/// </summary>
	/// <returns><c>true</c> if game is over; otherwise, <c>false</c>.</returns>
	bool IsGameOver() {
		
		for (int i = 1, ni = gridSize-1; i < ni; i++) {
			for (int j = 1, nj = gridSize-1; j < nj; j++) {
				if( !blocks [i, j] ) {
					return false;
				}
			}
		}
		
		return true;
	}
	
	/// <summary>
	/// Creates the game over transition
	/// </summary>
	/// <returns>The game over transition.</returns>
	U9Transition CreateGameOverTransition() 
	{

		
		int previousBestScore = HighScore;

		U9Transition gameOverViewTransition = null;

		SubmitScore ();
	
		if(Score > HighScore)
		{
			HighScore = score;
			highScoreView.Setup(HighScore);
			ECurrentView = View.Highscore;
		}
		else
		{
			newScoreView.Setup(score);
			ECurrentView = View.Newscore;
		}
		// Set the game over flag
		gameIsOver = true;
		score = 0;
		currentScoreLabel.text = "0";
		// Return a transition which clears the board and displays the appropriate game over view
		return U9T.S ( /*CreateClearBoardTransition()*/);
	}
	
	/// <summary>
	/// Causes all blocks to disappear
	/// </summary>
	/// <returns>The clear board transition.</returns>
	U9Transition CreateClearBoardTransition() {
		
		List<U9Transition> transitions = new List<U9Transition> ();
		
		for (int i = 0, ni = gridSize; i < ni; i++) {
			for (int j = 0, nj = gridSize; j < nj; j++) {
				Block b = blocks [i, j];
				if( b ) {
					transitions.Add( b.CreateDisappearTransition(0) );
				}
			}
		}
		
		// Randomises the order of the disappear transitions to make it more pretty!
		List<U9Transition> randomisedTransitions = GetRandomSubset (transitions, transitions.Count);

		float staggerTime = 0.025f; // 0.025f;
		return U9T.Stagger( staggerTime, randomisedTransitions.ToArray() );
	}
	
	/*****************************/
	/******** UI HANDLERS ********/
	/*****************************/
	
	void HandleLeaderboardsButtonClicked (object sender, System.EventArgs e)
	{      
		DisplayLeaderboard ("");
	}

	public void SwitchToView(int view)
	{
		ECurrentView = (View)view;
	}
	
	public void SwitchToView(View view)
	{
		ECurrentView = view;
	}

	/*******************************/
	/******** GAME SERVICES ********/
	/*******************************/
	
	void DisplayLeaderboard( string leaderboardID ) 
	{
		#if UNITY_ANDROID
		if (!GooglePlayGames.PlayGamesPlatform.Instance.IsAuthenticated()) 
		{
			
			System.Action<bool> authenticatedHandler = (success) => { 
				if (!success) {
					Debug.LogError ("Could not authenticate");
				} else {
					GooglePlayGames.PlayGamesPlatform.Instance.ShowLeaderboardUI (leaderboardID);
				}
			};
			
			Social.localUser.Authenticate (authenticatedHandler);
		} 
		else {
			GooglePlayGames.PlayGamesPlatform.Instance.ShowLeaderboardUI (leaderboardID);
		}
		#endif
		#if UNITY_IPHONE
		
		if(!GameCenterSingleton.Instance.IsUserAuthenticated())
		{
			GameCenterSingleton.Instance.Initialize();
		}
		else
		{
			GameCenterSingleton.Instance.ShowLeaderboardUI();
		}
		#endif
	}

	void ProcessAuthentication (bool success) 
	{
		#if UNITY_IPHONE
		if (success) 
		{
			GameCenterSingleton.Instance.ShowLeaderboardUI();
		}
		#endif
	}
	
	void SubmitScore() 
	{
		// Sends an analytics event which includes the players score along with how many attempts they have made so far
		int attempts = PlayerPrefs.GetInt("Attempts",0);
		PlayerPrefs.SetInt("Attempts",attempts+1);
		
		if(HighScore < Score)
			PlayerPrefs.SetInt ("HighScore", Score);
		
		Analytics.gua.beginHit(GoogleUniversalAnalytics.HitType.Event);
		Analytics.gua.addEventCategory("Gameplay");
		Analytics.gua.addEventAction("Score");
		Analytics.gua.addEventLabel( attempts.ToString("00000000") );
		
		Analytics.gua.addEventValue( score );
		Analytics.gua.sendHit();

		if( Social.localUser.authenticated ) 
		{
			//PostScoreAndAchivments();
		}
		
	}

	void PostScoreAndAchivments()
	{
#if Unity_ANDROID
		Social.Active.ReportScore( score, "", null );
		Social.ReportProgress("", Mathf.Clamp01((float)score/1000f), null );
		Social.ReportProgress("", Mathf.Clamp01((float)score/2000f), null );
		Social.ReportProgress("", Mathf.Clamp01((float)score/3000f), null );
		Social.ReportProgress("", Mathf.Clamp01((float)score/4000f), null );
		Social.ReportProgress("", Mathf.Clamp01((float)score/5000f), null );
#elif UNITY_IPHONE
		long scoreLong = score;
		Social.ReportProgress("",  Mathf.Clamp01((float)score/1000f) * 100.0f, null );
		Social.ReportProgress("", Mathf.Clamp01((float)score/2000f) * 100.0f, null );
		Social.ReportProgress("",  Mathf.Clamp01((float)score/3000f) * 100.0f, null );
		Social.ReportProgress("",  Mathf.Clamp01((float)score/4000f) * 100.0f, null );
		Social.ReportProgress("",  Mathf.Clamp01((float)score/5000f) * 100.0f, null );
		Social.Active.ReportScore(scoreLong,"", null);
#endif



	}
}

public enum Platform
{
	Web,
	Phone,
	Watch
}
