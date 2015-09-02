using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

public class EveryplayLegacyCleanup : AssetPostprocessor
{
    private static string[] filesToRemove = {
            "Editor/PostprocessBuildPlayer_EveryplaySDK",
            "Editor/UpdateXcodeEveryplay.pyc",
            "Plugins/iOS/EveryplayGlesSupport.mm",
            "Plugins/iOS/EveryplayGlesSupport.h",
            "Plugins/iOS/EveryplayUnity.mm",
            "Plugins/iOS/EveryplayUnity.h"
    };
    private const string oldPrefab = "Plugins/Everyplay/Everyplay.prefab";
    private const string newTestPrefab = "Plugins/Everyplay/Helpers/EveryplayTest.prefab";

    void OnPreprocessTexture()
    {
        // Don't compress Everyplay textures, makes importing faster
        if(assetPath.Contains("Plugins/Everyplay")) {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            if(textureImporter != null) {
                textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
            }
        }

        // Legacy clean (moving asset) often fails during package import, try to do it a couple of times pre import and one time post import
        if(assetPath.Contains("Plugins/Everyplay/Images")) {
            Clean(true);
        }
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // Legacy clean (moving asset) often fails during package import, try to do it a couple of times pre import and one time post import
        foreach(string asset in importedAssets) {
            if(asset.Trim().Equals("Assets/Plugins/Everyplay/Scripts/EveryplayLegacy.cs")) {
                Clean(true);
                EveryplayWelcome.ShowWelcome();
                return;
            }
        }
    }

    public static void Clean(bool silenceErrors)
    {
        foreach(string fileName in filesToRemove) {
            if(File.Exists(System.IO.Path.Combine(Application.dataPath, fileName))) {
                AssetDatabase.DeleteAsset(System.IO.Path.Combine("Assets", fileName));
                Debug.Log("Removed legacy Everyplay file: " + fileName);
            }
        }

        if(File.Exists(System.IO.Path.Combine(Application.dataPath, oldPrefab))) {
            if(File.Exists(System.IO.Path.Combine(Application.dataPath, newTestPrefab))) {
                AssetDatabase.DeleteAsset(System.IO.Path.Combine("Assets", oldPrefab));
                Debug.Log("Removed legacy Everyplay prefab: " + oldPrefab);
            }
            else {
                string src = System.IO.Path.Combine("Assets", oldPrefab);
                string dst = System.IO.Path.Combine("Assets", newTestPrefab);
                if((AssetDatabase.ValidateMoveAsset(src, dst) == "") && (AssetDatabase.MoveAsset(src, dst) == "")) {
                    Debug.Log("Renamed and updated legacy Everyplay prefab " + oldPrefab + " to " + newTestPrefab);
                }
                else if(!silenceErrors) {
                    Debug.LogError("Updating the old Everyplay prefab failed. Please rename Plugins/Everyplay/Everyplay prefab to EveryplayTest and move it to the Plugins/Everyplay/Everyplay/Helpers folder.");
                }
            }
        }
    }
}
