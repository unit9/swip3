using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;

[CustomEditor(typeof(EveryplaySettings))]
public class EveryplaySettingsEditor : Editor
{
    public const string settingsFile = "EveryplaySettings";
    public const string settingsFileExtension = ".asset";
    public const string testButtonsResourceFile = "everyplay-test-buttons.png";
    private static GUIContent labelClientId = new GUIContent("Client id");
    private static GUIContent labelClientSecret = new GUIContent("Client secret");
    private static GUIContent labelRedirectURI = new GUIContent("Redirect URI");
    private static GUIContent labelIOsSupport = new GUIContent("iOS enabled [?]", "Check to enable Everyplay replay recording on iOS devices");
    private static GUIContent labelAndroidSupport = new GUIContent("Android enabled [?]", "Check to enable Everyplay replay recording on Android devices");
    private static GUIContent labelTestButtons = new GUIContent("Enable test buttons [?]", "Check to overlay easy-to-use buttons for testing Everyplay in your game");
    private EveryplaySettings currentSettings = null;
    private bool iosSupportEnabled;
    private bool androidSupportEnabled;
    private bool testButtonsEnabled;

    [MenuItem("Edit/Everyplay Settings")]
    public static void ShowSettings()
    {
        EveryplaySettings settingsInstance = (EveryplaySettings)Resources.Load(settingsFile);

        if(settingsInstance == null) {
            settingsInstance = CreateEveryplaySettings();
        }

        if(settingsInstance != null) {
            EveryplayPostprocessor.ValidateAndUpdateFacebook();
            EveryplayLegacyCleanup.Clean(false);
            Selection.activeObject = settingsInstance;
        }
    }

    public override void OnInspectorGUI()
    {
        try {
            // Might be null when this gui is open and this file is being reimported
            if(target == null) {
                Selection.activeObject = null;
                return;
            }

            currentSettings = (EveryplaySettings)target;
            bool showAndroidSettings = CheckForAndroidSDK();

            if(currentSettings != null) {
                EditorGUILayout.HelpBox("1) Enter your game credentials", MessageType.None);

                if(!currentSettings.IsValid) {
                    EditorGUILayout.HelpBox("Invalid or missing game credentials, Everyplay disabled. Check your game credentials at https://developers.everyplay.com/", MessageType.Error);
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(labelClientId, GUILayout.Width(108), GUILayout.Height(18));
                currentSettings.clientId = TrimmedText(EditorGUILayout.TextField(currentSettings.clientId));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(labelClientSecret, GUILayout.Width(108), GUILayout.Height(18));
                currentSettings.clientSecret = TrimmedText(EditorGUILayout.TextField(currentSettings.clientSecret));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(labelRedirectURI, GUILayout.Width(108), GUILayout.Height(18));
                currentSettings.redirectURI = TrimmedText(EditorGUILayout.TextField(currentSettings.redirectURI));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox("2) Enable recording on these platforms", MessageType.None);

                EditorGUILayout.BeginVertical();

                iosSupportEnabled = EditorGUILayout.Toggle(labelIOsSupport, currentSettings.iosSupportEnabled);

                if(iosSupportEnabled != currentSettings.iosSupportEnabled) {
                    currentSettings.iosSupportEnabled = iosSupportEnabled;
                    EveryplayPostprocessor.SetEveryplayEnabledForTarget(BuildTargetGroup.iPhone, currentSettings.iosSupportEnabled);
                    EditorUtility.SetDirty(currentSettings);
                }

                if(showAndroidSettings) {
                    androidSupportEnabled = EditorGUILayout.Toggle(labelAndroidSupport, currentSettings.androidSupportEnabled);

                    if(androidSupportEnabled != currentSettings.androidSupportEnabled) {
                        currentSettings.androidSupportEnabled = androidSupportEnabled;
                        EveryplayPostprocessor.SetEveryplayEnabledForTarget(BuildTargetGroup.Android, currentSettings.androidSupportEnabled);
                        EditorUtility.SetDirty(currentSettings);
                    }
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.HelpBox("3) Try out Everyplay", MessageType.None);

                EditorGUILayout.BeginVertical();
                testButtonsEnabled = EditorGUILayout.Toggle(labelTestButtons, currentSettings.testButtonsEnabled);
                if(testButtonsEnabled != currentSettings.testButtonsEnabled) {
                    currentSettings.testButtonsEnabled = testButtonsEnabled;
                    EditorUtility.SetDirty(currentSettings);
                    EnableTestButtons(testButtonsEnabled);
                }
                EditorGUILayout.EndVertical();
            }
        }
        catch(Exception e) {
            if(e != null) {
            }
        }
    }

    private static string TrimmedText(string txt)
    {
        if(txt != null) {
            return txt.Trim();
        }
        return "";
    }

    private static EveryplaySettings CreateEveryplaySettings()
    {
        EveryplaySettings everyplaySettings = (EveryplaySettings)ScriptableObject.CreateInstance(typeof(EveryplaySettings));

        if(everyplaySettings != null) {
            if(!Directory.Exists(System.IO.Path.Combine(Application.dataPath, "Plugins/Everyplay/Resources"))) {
                AssetDatabase.CreateFolder("Assets/Plugins/Everyplay", "Resources");
            }

            AssetDatabase.CreateAsset(everyplaySettings, "Assets/Plugins/Everyplay/Resources/" + settingsFile + settingsFileExtension);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return everyplaySettings;
        }

        return null;
    }

    public void EnableTestButtons(bool enable) {
        string dstFile = "Plugins/Everyplay/Resources/" + testButtonsResourceFile;
        if(enable) {
            string sourceFile = "Plugins/Everyplay/Images/" + testButtonsResourceFile;
            if(!File.Exists(System.IO.Path.Combine(Application.dataPath, dstFile)) && File.Exists(System.IO.Path.Combine(Application.dataPath, sourceFile))) {
                AssetDatabase.CopyAsset("Assets/" + sourceFile, "Assets/" + dstFile);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        else {
            if(File.Exists(System.IO.Path.Combine(Application.dataPath, dstFile))) {
                AssetDatabase.DeleteAsset("Assets/" + dstFile);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }

    public static bool CheckForAndroidSDK()
    {
        if(System.IO.File.Exists(System.IO.Path.Combine(Application.dataPath, "Plugins/Android/everyplay/AndroidManifest.xml")) || System.IO.File.Exists(System.IO.Path.Combine(Application.dataPath, "Plugins/Android/Everyplay/AndroidManifest.xml"))) {
            return true;
        }
        return false;
    }

    void OnDisable()
    {
        if(currentSettings != null) {
            EditorUtility.SetDirty(currentSettings);
            currentSettings = null;
        }
    }
}
