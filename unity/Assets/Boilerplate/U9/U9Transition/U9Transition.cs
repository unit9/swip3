// U9Transition.cs
// 
// Created by Nick McVroom-Amoakohene <nicholas@unit9.com> on 03/09/2012.
// Based on code by David Rzepa <dave@unit9.com>
// Copyright (c) 2012 unit9 ltd. www.unit9.com. All rights reserved
using UnityEngine;

public abstract class U9Transition {
	
	public delegate void TransitionEventHandler(U9Transition transition);
	
	/// <summary>
	/// Occurs when transition started.
	/// </summary>
	public event TransitionEventHandler Began;
	
	/// <summary>
	/// Occurs when transition completed.
	/// </summary>
	public event TransitionEventHandler Ended;
	
	/// <summary>
	/// Occurs when transition interrupted.
	/// </summary>
	public event TransitionEventHandler Interrupted;
	
	/// <summary>
	/// Occurs when transition stopped.
	/// </summary>
	public event TransitionEventHandler Stopped;
	
	/// <summary>
	/// Occurs when transition paused.
	/// </summary>
	public event TransitionEventHandler Paused;
	
	/// <summary>
	/// Occurs when transition resumed.
	/// </summary>
	public event TransitionEventHandler Resumed;
	
	/// <summary>
	/// Gets or sets the data associated with this transition.
	/// </summary>
	/// <value>
	/// The data.
	/// </value>
	public object Data { get; set; }

	public int Priority { get; set; }

	protected bool completeOnInterrupt;
	bool wasInterrupted;
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="U9Transition"/> was interrupted.
	/// </summary>
	/// <value>
	/// <c>true</c> if it was interrupted; otherwise, <c>false</c>.
	/// </value>
	public bool WasInterrupted {
		get { return this.wasInterrupted; }
		protected set { this.wasInterrupted = value; }
	}
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="U9Transition"/> should complete on interrupt.
	/// </summary>
	/// <value>
	/// <c>true</c> if it should complete on interrupt; otherwise, <c>false</c>.
	/// </value>
	public bool CompleteOnInterrupt {
		get { return this.completeOnInterrupt; }
	}	
	
	protected bool hasEnded;

	public bool IsCompleted {
		get {
			return this.hasEnded;
		}
		set {
			hasEnded = value;
		}
	}	

	public virtual bool IsNull {
		get {
			return false;
		}
	}

	protected U9Transition() { }
	
	/// <summary>
	/// Play this transition.
	/// </summary>
	public virtual void Begin() { 
		OnBegan(this);
	}
	
	/// <summary>
	/// Raises the transition started event.
	/// </summary>
	protected void OnBegan(U9Transition t) {
		if(Began != null)
			Began(t);
	}
	
	/// <summary>
	/// Pause this transition.
	/// </summary>
	public virtual void Pause() { }
	
	/// <summary>
	/// Raises the transition paused event.
	/// </summary>
	protected void OnPaused(U9Transition t) {
		if(Paused != null)
			Paused(t);
	}
	
	/// <summary>
	/// Resume this transition.
	/// </summary>
	public virtual void Resume() { }
	
	/// <summary>
	/// Raises the transition resumed event.
	/// </summary>
	protected void OnResumed(U9Transition t) {
		if(Resumed != null)
			Resumed(t);
	}
	
	/// <summary>
	/// Stop this transition.
	/// </summary>
	public virtual void Stop() { OnStopped(this); }
	
	/// <summary>
	/// Raises the transition stopped event.
	/// </summary>
	protected void OnStopped(U9Transition t) {
		if(Stopped != null)
			Stopped(t);
	}
	
	/// <summary>
	/// Interrupt this transition.
	/// </summary>
	protected virtual void Interrupt() { OnInterrupted(this); }
	
	/// <summary>
	/// Raises the transition interrupted event.
	/// </summary>
	protected void OnInterrupted(U9Transition t) {
		hasEnded = true;
		wasInterrupted = true;
		
		if(Interrupted != null)
			Interrupted(t);
		
		//End();
	}
	
	/// <summary>
	/// Complete this transition.
	/// </summary>
	protected virtual void End() {
		OnEnded(this);
	}
	
	/// <summary>
	/// Raises the transition completed event.
	/// </summary>
	protected void OnEnded(U9Transition t) {
		hasEnded = true;
		
		if(Ended != null)
			Ended(t);
	}
	
	#region Operator overloads
	
	public static implicit operator bool(U9Transition t) {
		if(t == null)
			return false;
		else
			return true;
	}
	
	#endregion
}
