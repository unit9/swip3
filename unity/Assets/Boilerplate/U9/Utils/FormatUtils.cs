using UnityEngine;
using System.Collections;

public class FormatUtils : MonoBehaviour
{

	public static string FormatTime( float seconds ) {
       int currentMinutes = Mathf.FloorToInt( seconds / 60f );
       int currentSeconds = Mathf.FloorToInt(seconds - 60f*currentMinutes);
           
       string min = "", sec = "";
       
      	min = "" + currentMinutes;
       
       if (currentSeconds < 10)
           sec = "0" + currentSeconds;
       else
           sec = "" + currentSeconds;
       
       return min  + ":" + sec;
   }
	
}

