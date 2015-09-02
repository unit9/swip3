using UnityEngine;
using System.Text;
using System.Collections;

public class UILocalizeLabel : MonoBehaviour
{
	
	[HideInInspector]
	[SerializeField]
	UILabel uiLabel;
	
	[SerializeField]
	string stringKey;
	
	object[] stringParams;

	public UILabel Label {
		get {
			return this.uiLabel;
		}
	}
	
	void Reset() {
		uiLabel = GetComponent<UILabel>();
	}
	
	void Awake() {
		Localization.instance.LocalizationChanged += HandleLocalizationChanged;
	}
	
	public void UpdateKeyData( string stringKey, params object[] stringParams ) {
		this.stringKey = stringKey;
		this.stringParams = stringParams;

				string text; 
		if( stringParams != null ) {
			text = string.Format( Localization.instance.Get (stringKey), stringParams );
		} else {
			text = Localization.instance.Get (stringKey);
		}

				int currentIndex = 0;
				bool start = true;
				while (currentIndex < text.Length) {
					currentIndex = text.IndexOf ('*',currentIndex);
					if (currentIndex >= 0) {

								if (start) {
									text = text.Insert (currentIndex, "[666666]");
								} else {
					text = text.Insert (currentIndex+1, "[3f1e00]");
								}
							
				// + 1 for the * and + 8 for the [??????]
							currentIndex += 9;
							start = !start;
					} else {
							break;
					}
				}
				
				uiLabel.text = text;

		//GetTypingAnimationTransition ().Begin ();
	}
	
	void HandleLocalizationChanged()
	{
		UpdateKeyData( stringKey, stringParams );	
	}

}

