using UnityEngine;
using System.Collections;
using System;

public class U9Button : MonoBehaviour {

	[SerializeField]
	UISprite buttonSprite = null;

	[SerializeField]
	string spritePrefix = "";

	[SerializeField]
	string hoverSound = "", pressSound = "", clickSound = "";

	[SerializeField]
	UILocalizeLabel label = null;

	public UILocalizeLabel Label { get { return label; } }
	            
	public enum Status {
		Disabled,
		Normal,
		Hover,
		Pressed
	}
	Status currentStatus = Status.Normal;

	public virtual Status CurrentStatus {
		get {
			return this.currentStatus;
		}
		private set {
			if( currentStatus != value ) {
				currentStatus = value;
				OnStatusChanged(currentStatus);
			}
		}
	}
	
	public class StatusChangeEventArgs : EventArgs {
		public Status Status { get; set; }
	}
	
	bool autoDisabled, disabled;

	bool AutoDisabled {
		get {
			return this.autoDisabled;
		}
		set {
			autoDisabled = value;
		}
	}
	
	public virtual bool Disabled {
		get {
			return this.disabled;
		}
		set {
			if (disabled != value) {
				disabled = value;
				if (value) {
					iTween.Pause(gameObject);
					CurrentStatus = Status.Disabled;
					if (label) {
						label.Label.alpha = 0.5f;
					}
				} else {
					iTween.Resume(gameObject);
					CurrentStatus = Status.Normal;
					if (label) {
						label.Label.alpha = 1f;
					}
				}
			}
		}
	}
	
	protected bool IsEnabled {
		get {
			return !( AutoDisabled || Disabled );
		}
	}
	
	public event System.EventHandler Clicked;
	public event System.EventHandler<StatusChangeEventArgs> StatusChanged;
	
	public object Data { get; set; }
	
	protected virtual void OnClicked() {
		if (Clicked != null) {
			Clicked (this, null);
		}
	}
	
	protected virtual void OnStatusChanged( Status newStatus ) {	
		if (buttonSprite) {
			string s = spritePrefix + newStatus.ToString ();
			buttonSprite.spriteName = s;
			//buttonSprite.MakePixelPerfect ();
		}

		if( StatusChanged != null ) {
			StatusChanged(this, new StatusChangeEventArgs() { Status = newStatus } );
		}
	}
	
	protected virtual void EnableInteraction() {
		AutoDisabled = false;
	}
	
	protected virtual void DisableInteraction() {
		AutoDisabled = true;
	}
	
	void OnPress (bool isPressed) {
		if( IsEnabled ) {



			if( isPressed ) {
				if( !string.IsNullOrEmpty(pressSound) ) {
					AudioController.Play(pressSound);
				}
				CurrentStatus = Status.Pressed;
			} else {
				CurrentStatus = Status.Normal;
			}
		}
	}
	
	protected virtual void OnHover (bool isOver) {
		if( IsEnabled ) {

			if( !string.IsNullOrEmpty(hoverSound) ) {
				AudioController.Play(hoverSound);
			}

			if( isOver ) {
				CurrentStatus = Status.Hover;
			} else {
				CurrentStatus = Status.Normal;
			}
		}
	}
	
	protected virtual void OnClick() {

		if( IsEnabled ) {
			if( !string.IsNullOrEmpty(clickSound) ) {
				AudioController.Play(clickSound);
			}

			OnClicked();
		}
	}	
	
}
