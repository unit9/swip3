using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Text;

public class SaveController : MonoSingleton<SaveController> {
	
	#region Constant Fields
	#endregion

	#region Serialized Fields
	#endregion
	
	#region Fields
	bool disableAutoSaving = false;
	List<ISerializable> serializables;
	List<ISerializable> globalSerializables;

	bool loaded = false;
	#endregion
	
	#region Constructors
	#endregion
	
	#region Finalizers (Destructors)
	#endregion
	
	#region Delegates
	#endregion
	
	#region Events
	#endregion
	
	#region Enums
	#endregion
	
	#region Interfaces
	#endregion
	
	#region Properties
	public string CurrentSaveSlot { get; set; } 
	#endregion
	
	#region Indexers
	#endregion
	
	#region Methods
	void Awake() {
		Instance = this;
		serializables = new List<ISerializable>();
		globalSerializables = new List<ISerializable>();
#if UNITY_EDITOR
		disableAutoSaving = PlayerPrefs.GetString("DisableAutoSave") == "True";

//		StringBuilder sb = new StringBuilder();
//		for( int i = 0 ; i < 50000 ; i++ ) {
//			sb.Append( Encoding.Unicode.GetString( new byte[] { (byte)UnityEngine.Random.Range(0,255) } ) );
//		}
//
//		string s = "Hello my name is Dave and I'm scrambling your brainz!";
//		string scrambled = CEncryption.Scramble(s);
//		string descrambled = CEncryption.Unscramble(scrambled);
//		Debug.Log("[SCRAMBLE TEST]" + ( scrambled.CompareTo(descrambled) == 0 ? "PASSED" : "FAILED: " + descrambled ) );
#endif
	}
	
	public void RegisterSerializable( ISerializable serializable ) {
		serializables.Add( serializable );
	}

	public void RegisterGlobalSerializable( ISerializable serializable ) {
		globalSerializables.Add( serializable );
	}
	
	public void SaveToCurrentSlot( bool force = false ) {
#if UNITY_EDITOR
		Debug.Log ("[ATTEMPTING SAVE]: " + CurrentSaveSlot);
		#endif

		if(!loaded)
			return;

		if (string.IsNullOrEmpty (CurrentSaveSlot)) {
			return;
		}
		if (disableAutoSaving && !force) {
			return;
		}

		PlayerPrefs.SetInt( CurrentSaveSlot + "Version", 1 );
		PlayerPrefs.SetString( CurrentSaveSlot, CEncryption.Scramble(SaveData()) );
		PlayerPrefs.SetString( "GlobalSave", CEncryption.Scramble(SaveGlobalData()) );
		PlayerPrefs.Save();


	}

	public void LoadFromCurrentSlot() {

		try {
			LoadData (LoadSaveStringFromSlot (CurrentSaveSlot));
			LoadGlobalData(LoadSaveStringFromGlobal());
		} catch( Exception e ) {
						DisableSavingToPreventCorruption (e);
				}

	}

	public string LoadSaveStringFromSlot( string slot ) {
		int saveFileVersion = PlayerPrefs.GetInt (slot + "Version", 0);
		if (saveFileVersion == 0) {
			return PlayerPrefs.GetString( slot );
		} else {
			return CEncryption.Unscramble (PlayerPrefs.GetString( slot ));
		}
	}

	public string LoadSaveStringFromGlobal() {
		string slot ="GlobalSave";
		return CEncryption.Unscramble (PlayerPrefs.GetString( slot ));

	}

	const string scrambler = "PTfrVYVoKwOwlHQmy1rkSWhJzNheAYA3I2IUD1Kz5CLO528AesNMRk9peuyOS9elHmmjKadDked6Ddx9WtB1T6i3BcxXo5VlnBiY";

	/*string Scramble( string s ) {
		StringBuilder sb = new StringBuilder ();
		for (int i = 0; i < s.Length ; i++) {
			sb.Append ( (char)( (char)s [i] ^ (char)scrambler [i % scrambler.Length]) );
		}
		Debug.Log ("SCRAMBLE: " + sb.ToString () + "(" + sb.Length + ")" );
	}*/

	public bool SlotIsEmpty(string slot) {
		return !PlayerPrefs.HasKey (slot);
	}

	public void Unload() {
		foreach( ISerializable serializable in serializables ) {
			serializable.Unload ();
		}
		loaded = false;
		CurrentSaveSlot = "";
	}

	string SaveData() {
		Hashtable data = new Hashtable();
		foreach( ISerializable serializable in serializables ) {
			data.Add( serializable.ID, serializable.Serialize() );
		}
		string s = MiniJSON.jsonEncode(data);
		//Debug.Log ("SAVE: " + s);
		return s;
	}

	string SaveGlobalData() {
		Hashtable data = new Hashtable();

		foreach( ISerializable serializable in globalSerializables ) {
		
			data.Add( serializable.ID, serializable.Serialize() );

		}
		string s = MiniJSON.jsonEncode(data);
		//Debug.Log ("SAVE: " + s);
		return s;
	}
	
	void LoadData( string dataString ) {
		//Debug.Log ("Load: " + dataString );

		//try {

			Hashtable data = (Hashtable)MiniJSON.jsonDecode( dataString );
			foreach( ISerializable serializable in serializables ) {
				if( data != null && data.ContainsKey( serializable.ID ) ) {
					serializable.Deserialize( (Hashtable)data[serializable.ID] );
				} else {
					serializable.Deserialize( new Hashtable() );
				}
			}

		loaded = true;

//		} catch( Exception e ) {
//			Debug.LogError( new Exception("Error loading from save data: " + e.ToString(), e ) );
//			Application.LoadLevel (0);
//		}

	}

	void LoadGlobalData(string dataString  ) {
		//Debug.Log ("Load: " + dataString );
		
		try {



			Hashtable data = (Hashtable)MiniJSON.jsonDecode( dataString );
			foreach( ISerializable serializable in globalSerializables ) {
				if( data != null && data.ContainsKey( serializable.ID ) ) {
					serializable.Deserialize( (Hashtable)data[serializable.ID] );
				} else {
					serializable.Deserialize( new Hashtable() );
				}
			}
			
		} catch( Exception e ) {
			Debug.LogError( new Exception("Error loading from global save data: " + e.ToString(), e ) );
			//Application.LoadLevel (0);
		}
		
	}
	
	public static void LogHashtable( Hashtable data ) {
		Debug.Log ( FormatHashtable(data,"") );
	}
	
	public static string FormatHashtable( Hashtable data, string tab ) {
		string s = "";

		if (data == null) {
			Debug.LogError("null Hashtable");
			return "null";
		}

		foreach( DictionaryEntry e in data ) {
			Hashtable t = e.Value as Hashtable;
			if( t != null ) {
				s += tab+e.Key.ToString() + " {\n";
				s += tab+FormatHashtable( t, tab+"   " );
				s += tab+"}\n";
			} else {
				s += tab+e.Key + " : " + e.Value.ToString() + "\n";
			}
		}
		return s;
	}

	public void DisableSavingToPreventCorruption( Exception e ) {
		disableAutoSaving = true;
		Debug.LogException (e);

		string[] stackTraceSplit = e.StackTrace.Split ('\n')[0].Split('\\');
		stackTraceSplit = stackTraceSplit [stackTraceSplit.Length - 1].Split ('/');	
	}

#if UNITY_EDITOR
	bool hidden = false;
	void OnGUI() {

		if (!hidden) {
						if (GUILayout.Button ("SAVE")) {
								SaveToCurrentSlot (true);
						}
						bool t = GUILayout.Toggle (disableAutoSaving, "Disable auto save");
						if (t != disableAutoSaving) {
								disableAutoSaving = t;
								PlayerPrefs.SetString ("DisableAutoSave", disableAutoSaving.ToString ());
								PlayerPrefs.Save ();
						}
						hidden = GUILayout.Toggle (hidden, "Hide");
				}
	}
#endif

	#endregion
	
	#region Structs
	#endregion
	
	#region Classes
	#endregion
	
	
	
	
}
