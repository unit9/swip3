using UnityEngine;
using System.Collections.Generic;

public class UIImageFont : MonoBehaviour {
	
	[SerializeField]
	UIAtlas atlas;

	public UIAtlas Atlas {
		get {
			return this.atlas;
		}
	}
	
	[SerializeField]
	float letterSpacing = 1f;

	public float LetterSpacing {
		get {
			return this.letterSpacing;
		}
	}	
	
	[System.Serializable]
	class UIImageFontCharacter {
		[SerializeField]
		string character;
		[SerializeField]
		string spriteName;

		public string Character {
			get {
				return this.character;
			}
		}

		public string SpriteName {
			get {
				return this.spriteName;
			}
		}
	}
	
	[SerializeField]
	UIImageFontCharacter[] imageFontCharacters;
	
	Dictionary<string,UISpriteData> characterMap;

	public Dictionary<string, UISpriteData> CharacterMap {
		get {
			if( characterMap == null ) {
				characterMap = new Dictionary<string, UISpriteData>();
				foreach( UIImageFontCharacter c in imageFontCharacters ) {
					characterMap.Add( c.Character, atlas.GetSprite( c.SpriteName ) );
				}
			}
			return this.characterMap;
		}
	}	
	
	void Awake() {
		
	}

	
	public UISpriteData GetCharacterSprite( char c ) {
		return CharacterMap[c.ToString()];
	}
	
}

