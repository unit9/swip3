using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(UISmartTiledSprite))]
public class UISmartTiledSpriteInspector : UISpriteInspector {
	
	protected override void DrawCustomProperties ()
	{
		UISmartTiledSprite smartTiledSprite = target as UISmartTiledSprite;
		float tileOffsetX = EditorGUILayout.FloatField ( "Tile Offset X: " , smartTiledSprite.TileOffset.x);
		float tileOffsetY = EditorGUILayout.FloatField ( "Tile Offset Y: " , smartTiledSprite.TileOffset.y);
		smartTiledSprite.TileOffset = new Vector2 (tileOffsetX, tileOffsetY);
	}
	
}
