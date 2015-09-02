using UnityEngine;
using System.Collections;

public class UIImageFontLabel : MonoBehaviour {
	
	[SerializeField]
	UIImageFont imageFont = null;
	
	[SerializeField]
	string text;
	public string Text {
		get {
			return this.text;
		}
		set {
			if( this.text != value) {
				this.text = value;
				UpdateSprites();
			}
		}
	}
	
	[SerializeField]
	int depth = 0;
	
	UISprite[] sprites;
	
	void Start() {
		UpdateSprites();
	}
	
	void UpdateSprites() {
		
		// Clean up the old sprites
		if( sprites != null ) {
			foreach( UISprite s in sprites ) {
				Destroy(s.gameObject);
			}
		}		
		
		sprites = new UISprite[text.Length];
		
		float currentX = 0f;
		
		// Create the new ones
		for( int i = 0, ni = this.text.Length ; i < ni ; i++ ) {
			GameObject go = new GameObject(text[i].ToString());	
			
			go.layer = gameObject.layer;
			go.transform.parent = transform;
			go.transform.localPosition = new Vector3( currentX, 0f, 0f );
			
			sprites[i] = go.AddComponent<UISprite>();
			sprites[i].atlas = imageFont.Atlas;
			sprites[i].spriteName = imageFont.GetCharacterSprite( text[i] ).name;
			sprites[i].depth = depth;
			sprites[i].MakePixelPerfect();

			currentX += sprites[i].transform.localScale.x + imageFont.LetterSpacing;
		}
	}
	
}
