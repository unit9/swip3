// Created by David Rzepa
// Copyright (c) unit9 ltd. 2012


using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class ChangeAudioImportSettings : ScriptableObject {
	
	[MenuItem ("U9/Audio/Change Audio Settings/SetStreamFromDisc")]
    static void SetStreamFromDisc() {
        SelectedSetStreamFromDisc();
    }
	
	[MenuItem ("U9/Audio/Change Audio Settings/SetNot3D")]
    static void SetNot3D() {
        SelectedSetNot3D();
    }
	
	[MenuItem ("U9/Audio/Change Audio Settings/SetMono")]
    static void SetMono() {
        SelectedSetMono();
    }
	
	static void SelectedSetStreamFromDisc() {
   		
        Object[] audios = GetSelectedAudio();
        //Selection.objects = new Object[0];
        foreach (Object audio in audios)  {		
            string path = AssetDatabase.GetAssetPath(audio);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
			if( audioImporter ) {
				Debug.Log("SetStreamFromDisc on: " + path);
				audioImporter.loadType = AudioImporterLoadType.StreamFromDisc;
				AssetDatabase.ImportAsset(path);
				AssetDatabase.Refresh();
			}
        }
    }
	
	static void SelectedSetNot3D() {
   		
        Object[] audios = GetSelectedAudio();
        //Selection.objects = new Object[0];
        foreach (Object audio in audios)  {		
            string path = AssetDatabase.GetAssetPath(audio);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
			if( audioImporter ) {
				Debug.Log("SelectedSetNot3D on: " + path);
				audioImporter.threeD = false;
				AssetDatabase.ImportAsset(path);
				AssetDatabase.Refresh();
			}
        }
    }
	
	static void SelectedSetMono() {
   		
        Object[] audios = GetSelectedAudio();
        //Selection.objects = new Object[0];
        foreach (Object audio in audios)  {		
            string path = AssetDatabase.GetAssetPath(audio);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
			if( audioImporter ) {
				Debug.Log("SelectedSetNot3D on: " + path);
				audioImporter.forceToMono = true;
				AssetDatabase.ImportAsset(path);
				AssetDatabase.Refresh();
			}
        }
    }
	
	[MenuItem ("U9/Audio/Change Audio Settings/SetCompressed")]
	static void SelectedSetCompressed() {
   		
        Object[] audios = GetSelectedAudio();
        //Selection.objects = new Object[0];
        foreach (Object audio in audios)  {		
            string path = AssetDatabase.GetAssetPath(audio);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
			if( audioImporter ) {
				Debug.Log("SetCompressed on: " + path);
				audioImporter.format = AudioImporterFormat.Compressed;
				AssetDatabase.ImportAsset(path);
				AssetDatabase.Refresh();
			}
        }
    }
	
	[MenuItem ("U9/Audio/Change Audio Settings/SetUncompressed")]
	static void SelectedSetUncompressed() {
   		
        Object[] audios = GetSelectedAudio();
        //Selection.objects = new Object[0];
        foreach (Object audio in audios)  {		
            string path = AssetDatabase.GetAssetPath(audio);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
			if( audioImporter ) {
				audioImporter.format = AudioImporterFormat.Native;
				AssetDatabase.ImportAsset(path);
				AssetDatabase.Refresh();
			}
        }
    }

    static Object[] GetSelectedAudio()
    {
        return Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets );
    }
	
}
