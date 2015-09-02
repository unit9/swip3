// View.cs
// 
// Created by Nick McVroom-Amoakohene <nicholas@unit9.com> on 07/09/2012.
// Based on code by David Rzepa <dave@unit9.com>
// Copyright (c) 2012 unit9 ltd. www.unit9.com. All rights reserved

//#define USE_HOTWEEN

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class U9View : MonoBehaviour {
	
	[SerializeField]
	bool hideOnInit = true;
	
	[SerializeField]
	bool deactivateWhenHidden = true;

	[SerializeField]
	string soundOnDisplay = "";

	[SerializeField]
	bool fireAnalyticsEvent = false;

	public enum ViewState {
		Displayed,
		Hidden,
		Displaying,
		Hiding
	}
	
	//[SerializeField]
	ViewState state = ViewState.Hidden;
	public ViewState State {
		get {
			return this.state;
		}
		private set {
			state = value;
			if (StateChanged != null) {
				StateChanged (value);
			}
		}
	}

	public event System.Action<ViewState> StateChanged;

	public bool IsDisplaying {
		get {
			return State == ViewState.Displayed || State == ViewState.Displaying;
		}
	}
	
	public bool IsInited {
		get {
			return inited;
		}
	}
	
	U9View parentView;
	List<U9View> dependentViews;
	GameObject[] dependentGameObjects;
	
	protected List<U9View> DependentViews {
		get {
			return this.dependentViews;
		}
	}

	protected U9View ParentView {
		get {
			return this.parentView;
		}
	}	
	
	bool inited;
	
	protected virtual void Awake() {
		dependentViews = FindDependentViews(transform);
		foreach( U9View v in dependentViews ) {
			v.parentView = this;
		}
		dependentGameObjects = FindDependentGameObjects(transform).ToArray();
	}
	
	protected void AddDependentView( U9View v ) {
		dependentViews.Add(v);
		v.parentView = this;
	}
	
	List<U9View> FindDependentViews( Transform parent ) {
		List<U9View> foundViews = new List<U9View>();
		foreach( Transform t in parent ) {
			U9View view = t.GetComponent<U9View>();
			if( view ) {
				foundViews.Add( view );
			} else {
				foundViews.AddRange( FindDependentViews(t) );
			}
		}
		return foundViews;
	}
	
	List<GameObject> FindDependentGameObjects( Transform parent ) {
		List<GameObject> foundGameObjects = new List<GameObject>();
		foundGameObjects.Add (parent.gameObject);
		foreach( Transform t in parent ) {
			U9View view = t.GetComponent<U9View>();
			if( !view ) {
				foundGameObjects.Add( t.gameObject );
				foundGameObjects.AddRange( FindDependentGameObjects(t) );
			}
		}
		return foundGameObjects;
	}
	
	protected virtual void Start() {
		if( !inited && !parentView ) {
			InitView();
		}
	}

	public void AttemptInit() {
		if( !inited && !parentView ) {
			InitView();
		}
	}

	protected virtual void InitView() {

		inited = true;

		if (dependentViews == null) {
			Debug.LogError ("Being inited before Awake?");
			return;
		}

		foreach( U9View v in dependentViews ) {
			v.InitView();
		}
		
		if( hideOnInit ) {
			Hide();
		} else {
			Display();
		}
		
	}
	
	public virtual U9Transition GetDisplayTransition( bool force = false ) {
		if( ( !force && IsDisplaying ) ) {
		//	Debug.LogWarning( name + " already " + State );
			return U9T.Null();
		} else {
			state = ViewState.Displaying;
			U9Transition t = CreateDisplayTransition(force);
			AddDisplayTransitionListeners( t );
			return t;
		}
	}
	
	protected abstract U9Transition CreateDisplayTransition( bool force );
	
	protected void AddDisplayTransitionListeners( U9Transition displayTransition ) {
		displayTransition.Began += HandleDisplayTransitionBegan;
		displayTransition.Ended += HandleDisplayTransitionEnded;
	}	
	
	void HandleDisplayTransitionBegan (U9Transition transition)
	{
		BeginDisplay();
	}

	protected virtual void HandleDisplayTransitionEnded(U9Transition transition) {
		if (!transition.WasInterrupted) {
			EndDisplay ();
		}
	}
	
	public virtual U9Transition GetHideTransition( bool force = false ) {
//		if ( !force && !gameObject.activeInHierarchy) {
//			Hide ();
//			return U9T.Null ();
//		}
		if( ( !force && !IsDisplaying ) ) {
			//Debug.LogWarning( name + " already " + State );
			return U9T.Null();
		} else {
			state = ViewState.Hiding;
			U9Transition t = CreateHideTransition(force);
			AddHideTransitionListeners( t );
			return t;
		}
	}
	
	protected abstract U9Transition CreateHideTransition( bool force );
	
	protected void AddHideTransitionListeners( U9Transition hideTransition ) {
		hideTransition.Began += HandleHideTransitionBegan;
		hideTransition.Ended += HandleHideTransitionEnded;
	}
	
	void HandleHideTransitionBegan (U9Transition transition)
	{
		BeginHide();
	}

	protected virtual void HandleHideTransitionEnded(U9Transition transition) {
		if (!transition.WasInterrupted) {
			EndHide ();
		}
	}
	
	protected virtual void BeginDisplay() {


		if (!string.IsNullOrEmpty (soundOnDisplay)) {
			AudioController.Play(soundOnDisplay);
		}

		if( deactivateWhenHidden ) {
			gameObject.SetActive(true);
		}
		foreach( GameObject g in dependentGameObjects ) {
			g.SendMessage( "OnViewBeginDisplay", SendMessageOptions.DontRequireReceiver );
		}
		State = ViewState.Displaying;		
	}
	
	protected virtual void EndDisplay() {
		State = ViewState.Displayed;
		EnableInteraction ();
	}
	
	protected virtual void BeginHide() {
	
		if (!string.IsNullOrEmpty (soundOnDisplay)) {
			AudioController.Play(soundOnDisplay);
		}

		DisableInteraction ();

		State = ViewState.Hiding;
	}
	
	protected virtual void EndHide() {
		State = ViewState.Hidden;
		foreach( GameObject g in dependentGameObjects ) {
			g.SendMessage( "OnViewEndHide", SendMessageOptions.DontRequireReceiver );
		}
		if( deactivateWhenHidden ) {
			gameObject.SetActive(false);
		}
	}

	bool interactionEnabled = true;

	void OnEnable() {
		UpdateInteractionStatus ();
	}

	public void DisableInteraction() {
		if (interactionEnabled) {
			interactionEnabled = false;
			UpdateInteractionStatus ();
		}
	}



	public void EnableInteraction() {
		if (!interactionEnabled) {
			interactionEnabled = true;
			UpdateInteractionStatus ();
		}
	}

	void UpdateInteractionStatus() {
		if (gameObject.activeInHierarchy) {
			if (interactionEnabled) {
				EnableInteractionInternal ();
			} else {
				DisableInteractionInternal ();
			}
		}
	}

	protected virtual void EnableInteractionInternal() {
		foreach (GameObject g in dependentGameObjects) {
			g.SendMessage ("EnableInteraction", SendMessageOptions.DontRequireReceiver );
		}
	}

	protected virtual void DisableInteractionInternal() {
		foreach (GameObject g in dependentGameObjects) {
			g.SendMessage ("DisableInteraction", SendMessageOptions.DontRequireReceiver);
		}
	}

	public virtual void Display() {
		BeginDisplay();
		EndDisplay();
	}
	
	public virtual void Hide() { 
		BeginHide();
		EndHide();
	}
}
