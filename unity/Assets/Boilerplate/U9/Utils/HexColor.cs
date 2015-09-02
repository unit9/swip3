using UnityEngine;

public class HexColor : MonoBehaviour
{

	public static Color ColorFromHex( string hex ) {
		int red = int.Parse( hex.Substring(0,2) , System.Globalization.NumberStyles.AllowHexSpecifier);
		int green = int.Parse( hex.Substring(2,2) , System.Globalization.NumberStyles.AllowHexSpecifier);
		int blue = int.Parse(hex.Substring(4,2) , System.Globalization.NumberStyles.AllowHexSpecifier);
		
		return new Color( (float)red/255, (float)green/255, (float)blue/255 );
	}
	
}

