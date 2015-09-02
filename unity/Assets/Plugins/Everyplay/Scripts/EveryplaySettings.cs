using UnityEngine;
using System.Collections;

public class EveryplaySettings : ScriptableObject
{
    public string clientId;
    public string clientSecret;
    public string redirectURI;

    public bool iosSupportEnabled;
    public bool androidSupportEnabled;

    public bool testButtonsEnabled;

    public bool IsEnabled {
        get {
            #if UNITY_IPHONE
            return iosSupportEnabled;
            #elif UNITY_ANDROID
            return androidSupportEnabled;
            #else
            return false;
            #endif
        }
    }

    public bool IsValid {
        get {
            if(clientId != null && clientSecret != null && redirectURI != null) {
                if(clientId.Trim().Length > 0 && clientSecret.Trim().Length > 0 && redirectURI.Trim().Length > 0) {
                    return true;
                }
            }
            return false;
        }
    }
}
