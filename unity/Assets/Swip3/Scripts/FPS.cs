using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour {

	public UILabel label;

	private float lastTime = 0;

	public  float updateInterval = 0.5F;
	
	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;
		
		// Interval ended - update GUI text and start new interval
		if( timeleft <= 0.0 )
		{
			// display two fractional digits (f2 format)
			float fps = accum/frames;
			string format = System.String.Format("{0:F2} FPS",fps);
			label.text = format;
			
			if(fps < 10)
				label.color = Color.red;
			else if(fps < 30)
				label.color = Color.yellow;
			else
				label.color = Color.green;

			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;
		}
	}
}
