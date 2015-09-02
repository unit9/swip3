using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShareButton : MonoBehaviour {

	public enum FBState
	{
		Login,
		Share
	}

	[SerializeField]
	U9Button FBButton = null, GoogleButton = null;

	[SerializeField]
	UILabel scoreLabel = null;

	[SerializeField]
	UILabel debugLabel = null;

	[SerializeField]
	string _FBLogin, _FBShare;

	UISprite _FBSprite;

	FBState State;

	void Awake() 
	{
		FBButton.Clicked += HandleClicked;

		if(GoogleButton != null)
			GoogleButton.Clicked += HandleGoogleButtonClicked;

		FB.Init(OnInitComplete, OnHideUnity);

		_FBSprite = GetComponent<UISprite>();
		State = FBState.Login;

		CallFBFeed();
	}

	void HandleClicked (object sender, System.EventArgs e)
	{
		if(State == FBState.Login)
		{
			FB.Login("email,publish_actions", LoginCallback);
		}
		else if(State == FBState.Share && FB.IsLoggedIn)
		{
			CallFBFeed();
		}
	}

	void HandleGoogleButtonClicked(object sender, System.EventArgs e)
	{
		Application.OpenURL("https://play.google.com/store/apps/details?id=com.unit9.swip3wear&hl=pl");
	}

	#region FB.Init() example
	
	private bool isInit = false;
	
	private void OnInitComplete()
	{
		isInit = true;

		if (FB.IsLoggedIn)
		{
			OnLogin();
		}
	}
	
	private void OnHideUnity(bool isGameShown)
	{
	}
	
	#endregion
	
	#region FB.Login() example
	
	void LoginCallback(FBResult result)
	{

		if (result.Error != null)
			Debug.LogError( "Error Response:\n" + result.Error );
		else if (!FB.IsLoggedIn)
		{
			Debug.LogWarning( "Login cancelled by Player" );
		}
		else
		{
			OnLogin();
		}
	}

	void OnLogin()
	{
		Debug.Log( "Login was successful!" );
		CallFBPublishInstall();
		State = FBState.Share;
		_FBSprite.spriteName = _FBShare;
	}

	private void CallFBLogout()
	{
		FB.Logout();
	}
	#endregion
	
	#region FB.PublishInstall() example
	
	private void CallFBPublishInstall()
	{
		FB.PublishInstall(PublishComplete);
	}
	
	private void PublishComplete(FBResult result)
	{
		Debug.Log("publish response: " + result.Text);
	}
	
	#endregion


	#region FB.Feed() example

	private void CallFBFeed()
	{
		FB.Feed (
			link: "http://swip3.unit9.com/",
			linkName: "I just scored " + scoreLabel.text + " points in SWIP3!",
			linkCaption: "SWIP3",
			linkDescription: "The addictive game even your grandma could play! Simply swipe to match 3 and rack up the points. Available on Web, Android, and Android Wear.",
			mediaSource: "http://swip3.unit9.com/images/share/facebook.png",
			actionName: "Download the app now!",
			actionLink: "http://ms.unit9.com/app/services/redirect.php?iOSLink=http://swip3.unit9.com/&AndroidLink=http://play.google.com/store/apps/details?id=com.unit9.swip3wear",
			reference: "scoreShare",
			callback: FeedCallback
		);

		debugLabel.text = "FeedSend";
	}

	void FeedCallback(FBResult result)
	{
		debugLabel.text = result.Text;
	}
	
	#endregion


}
