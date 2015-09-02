using UnityEngine;
using System.Collections;
using System;

public class GenericAnalytics : MonoBehaviour {

	DateTime sessionStartTime;

	bool firstSession;

	// Use this for initialization
	void Start () {

		sessionStartTime = DateTime.Now;

		firstSession = PlayerPrefs.GetInt ("FirstSession", 1) == 1;

		float hours = (float)DateTime.Now.TimeOfDay.TotalHours;

		Analytics.gua.beginHit (GoogleUniversalAnalytics.HitType.Event);
		Analytics.gua.addEventCategory ("Usage");
		Analytics.gua.addEventAction ("Time Of Day");
		Analytics.gua.addEventLabel (Mathf.RoundToInt ((float)hours).ToString ());
		Analytics.gua.addEventValue ( Mathf.RoundToInt ( (float)hours ) );
		Analytics.gua.sendHit ();


	}
	
	void OnApplicationPause( bool paused ) {
		TrackSession (paused);
	}

	void OnApplicationQuit() {
		TrackSession (true);
	}

	void TrackSession( bool ending ) {
		Analytics.gua.beginHit (GoogleUniversalAnalytics.HitType.Appview);
		Analytics.gua.addSessionControl (!ending);
		Analytics.gua.sendHit ();
		
		if (firstSession) {
			firstSession = false;
			PlayerPrefs.SetInt("FirstSession",0);
			
			Analytics.gua.beginHit (GoogleUniversalAnalytics.HitType.Event);
			Analytics.gua.addEventCategory ("Usage");
			Analytics.gua.addEventAction ("First Session Length");
			
			Analytics.gua.addEventValue ( Mathf.RoundToInt( (float)(DateTime.Now-sessionStartTime).TotalSeconds ) );
			Analytics.gua.sendHit ();
		}
	}
}
