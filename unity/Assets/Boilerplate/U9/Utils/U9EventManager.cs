using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class U9EventManager : MonoSingleton<U9EventManager>
{

	public class U9EventArgs : EventArgs
	{

		public U9EventArgs (string eventID, object[] args)
		{
			this.EventID = eventID;
			Data = new Hashtable ();
			for (int i = 0, ni = args.Length; i < ni; i += 2) {
				Data.Add (args [i], args [i + 1]);
			}
			Transitions = new List<U9Transition> ();
		}

		public string EventID { get; private set; }
		public Hashtable Data { get; private set; }
		public List<U9Transition> Transitions { get; private set; }
	}

	class U9Event
	{

		public U9Event (string eventID)
		{
			this.EventID = eventID;
		}

		public string EventID { get; private set; }

		public event EventHandler<U9EventArgs> Fired;

		public U9EventArgs OnFired (object source, object[] args)
		{
			if (Fired != null) {
				U9EventArgs e = new U9EventArgs (EventID, args);
				Fired (source, e);
				return e;
			} else {
				return null;
			}

		}

		public bool HasListeners ()
		{
			return Fired != null && Fired.GetInvocationList ().Length > 0;
		}

	}

	Dictionary<string,U9Event> events;

	void Awake ()
	{
		Instance = this;
		events = new Dictionary<string, U9Event> ();
	}

	public void AddEventHandler (string eventID, EventHandler<U9EventArgs> handler)
	{
		U9Event e;
		if (!events.TryGetValue (eventID, out e)) {
			e = new U9Event (eventID);
			events.Add (eventID, e);
		}
		e.Fired += handler;
	}

	public void RemoveEventHandler (string eventID, EventHandler<U9EventArgs> handler)
	{
		U9Event e;
		if (events.TryGetValue (eventID, out e)) {
			e.Fired -= handler;
			if (!e.HasListeners ()) {
				events.Remove (eventID);
			}
		}  
	}

	public U9Transition FireEvent (string eventID, object source, params object[] args)
	{
		//Debug.Log ("FIRE EVENT: " + eventID);
		U9Event e;
		U9Transition transition = U9T.Null (); 
		if (events.TryGetValue (eventID, out e)) {
			U9EventArgs eventArgs = e.OnFired (source, args);
			if (eventArgs != null && eventArgs.Transitions.Count > 0) {
				transition = U9T.P (eventArgs.Transitions.ToArray ());
			}
		}
		return transition;
	}

}
