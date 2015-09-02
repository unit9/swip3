using System;
using UnityEngine;

[Serializable]
public class SerializableTimeSpan
{
	
	[SerializeField]
	int days = 0, hours = 0, minutes = 0, seconds = 0;

	public int Days {
		get {
			return this.days;
		}
	}

	public int Hours {
		get {
			return this.hours;
		}
	}

	public int Minutes {
		get {
			return this.minutes;
		}
	}

	public int Seconds {
		get {
			return this.seconds;
		}
	}	
	
	public SerializableTimeSpan (int days, int hours, int minutes, int seconds)
	{
		this.days = days;
		this.hours = hours;
		this.minutes = minutes;
		this.seconds = seconds;
	}

	public bool NonZero() {
		return ( days > 0 || hours > 0 || minutes > 0 || seconds > 0 );
	}
	
	public TimeSpan ToTimeSpan() {
		return new TimeSpan( days, hours, minutes, seconds );
	}
	
}
