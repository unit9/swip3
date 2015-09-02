using UnityEngine;
using System.Collections;

public class PlatformDependentToggle : MonoBehaviour
{


		[SerializeField]
		bool android, iOS, editor, everyplay, web, watch;

		// Use this for initialization
		void Awake ()
		{

				if (Screen.width <= 320 && Screen.height <= 320) {
						gameObject.SetActive (watch);
				} else {
#if UNITY_EDITOR
					gameObject.SetActive (editor);
#elif UNITY_IPHONE

					if(everyplay)
					{
						if(Everyplay.SharedInstance.IsSupported() && Everyplay.SharedInstance.IsRecordingSupported())
						{
							gameObject.SetActive(iOS);
						}
						else {
							gameObject.SetActive(false);
						}
					} else {
						gameObject.SetActive(iOS);
					}
#elif UNITY_ANDROID
					gameObject.SetActive(android);
#elif UNITY_WEBPLAYER
					gameObject.SetActive(web);
#endif
			}
		}
	

}


