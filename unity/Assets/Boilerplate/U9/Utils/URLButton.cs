using UnityEngine;
using System.Collections;

public class URLButton : MonoBehaviour {

	[SerializeField]
	U9Button button = null;

	[SerializeField]
	string urlToOpen = "http://";

	void Reset() {
		button = GetComponent<U9Button> ();
	}

	void Awake() {
		button.Clicked += HandleClicked;
	}

	void HandleClicked (object sender, System.EventArgs e)
	{
		Application.OpenURL (urlToOpen);
	}


}
