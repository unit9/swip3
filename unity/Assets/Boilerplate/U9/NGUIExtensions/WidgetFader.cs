using UnityEngine;
using System.Collections;

public class WidgetFader : U9FadeView.MonoFader
{

	[SerializeField]
	UIWidget[] widgets;
	
	[SerializeField]
	UIButtonColor[] buttonColors;
	
	float[] widgetAlphas, buttonColorHoverAlphas, buttonColorPressedAlphas;
	
	public void Reset() {
		widgets = (UIWidget[])GetComponentsInChildren<UIWidget>(true);
		buttonColors = (UIButtonColor[])GetComponentsInChildren<UIButtonColor>(true);
		UpdateAlphas();
	}
	
	void Awake() {
		UpdateAlphas();
	}
	
	void UpdateAlphas() {
		widgetAlphas = new float[widgets.Length];
		for( int i = 0, ni = widgets.Length ; i < ni ; i++ ) {
			widgetAlphas[i] = widgets[i].color.a;
		}
		
		buttonColorHoverAlphas = new float[buttonColors.Length];
		buttonColorPressedAlphas = new float[buttonColors.Length];
		for( int i = 0, ni = buttonColors.Length ; i < ni ; i++ ) {
			buttonColorHoverAlphas[i] = buttonColors[i].hover.a;
			buttonColorPressedAlphas[i] = buttonColors[i].pressed.a;
		}
	}
	
	float alpha;	
	#region implemented abstract members of U9FadeView.MonoFader
	public override float Alpha {
		get {
			return this.alpha;
		}
		set {
			this.alpha = value;
			for( int i = 0, ni = widgets.Length ; i < ni ; i++ ) {
				Color tColor = widgets[i].color;
				tColor.a = widgetAlphas[i]*value;
				widgets[i].color = tColor;
			}
			for( int i = 0, ni = buttonColors.Length ; i < ni ; i++ ) {
				Color tColor = buttonColors[i].hover;
				tColor.a = buttonColorHoverAlphas[i]*value;
				buttonColors[i].hover = tColor;
				
				tColor = buttonColors[i].pressed;
				tColor.a = buttonColorPressedAlphas[i]*value;
				buttonColors[i].pressed = tColor;
			}
		}
	}
	#endregion
	
	#region implemented abstract members of U9FadeView.MonoFader
	public override bool Fading {
		set {
			for( int i = 0, ni = buttonColors.Length ; i < ni ; i++ ) {
				buttonColors[i].enabled = !value;
			}
		}
	}
	#endregion
}

