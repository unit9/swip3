using UnityEngine;
using System.Collections;
using System;

public class U9DragArea : MonoBehaviour {

	public class DragEventArgs : EventArgs {
		public bool Pressed { get; set; }
		public Vector2 Delta { get; set; }
	}
	
	public event System.EventHandler<DragEventArgs> Dragged;
	public event System.Action<Vector3> Clicked;

	void OnPress( bool pressed ) {
		OnDragged( new DragEventArgs() { Pressed = pressed, Delta = Vector2.zero } );
	}
	
	void OnDrag( Vector2 delta ) {
		OnDragged( new DragEventArgs() { Pressed = true, Delta = delta } );
	}

	void OnClick() {
		if (UICamera.currentTouch.totalDelta.magnitude < 10f) {
			if (Clicked != null) {
				Clicked ( UICamera.lastHit.point );
			}
		}
	}

	void OnDragged( DragEventArgs args ) {
		if( Dragged != null ) {
			Dragged(this,args);
		}
	}
	
	void OnDrop() {
		OnDragged( new DragEventArgs() { Pressed = false, Delta = Vector2.zero } );
	}
//	
//	void OnScroll( float delta ) {
//		OnDragged( new DragEventArgs() { Pressed = true, Delta = new Vector2(0f,-10f*delta) } );
//		OnDragged( new DragEventArgs() { Pressed = false, Delta = new Vector2(0f,-10f*delta) } );
//	}
	
}
