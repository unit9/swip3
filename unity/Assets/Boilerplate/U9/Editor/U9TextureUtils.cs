using UnityEngine;
using System.Collections;
using UnityEditor;

public class U9TextureUtils : MonoBehaviour {

	[MenuItem ("U9/Utils/Generate Bleed Textures")]
	public static void GenerateBleedTexture() {
		Object[] selectedTextures = Selection.GetFiltered( typeof(Texture2D), SelectionMode.DeepAssets );

		foreach( Object o in selectedTextures ) {
			Texture2D t = (Texture2D)o;
			Texture2D bleedTexture = new Texture2D( t.width+8, t.height+8 );

			for( int i = 0, ni = bleedTexture.width ; i < ni ; i++ ) {
				for( int j = 0, nj = bleedTexture.height ; j < nj ; j++ ) {
					bleedTexture.SetPixel( i, j, t.GetPixelBilinear( (float)(i-4)/t.width, (float)(j-4)/t.height ) );
				}
			}
			bleedTexture.Apply();

			string originalPath = AssetDatabase.GetAssetPath(t);
			string withoutExtension = originalPath.Split('.')[0];

			byte[] bytes = bleedTexture.EncodeToPNG();
			System.IO.File.WriteAllBytes(withoutExtension+"Bleed.png", bytes);
			bytes = null;
			
			// Re-import the newly created texture, turning off the 'readable' flag
			AssetDatabase.Refresh();

		}

	}

}
