// Analytics integration example for GoogleUniversalAnalytics helper class.
//
// Copyright 2013 Jetro Lauha (Strobotnik Ltd)
// http://strobotnik.com
// http://jet.ro
//
// $Revision: 533 $
//
// File version history:
// 2013-04-26, 1.0.0 - Initial version
// 2013-05-03, 1.0.1 - Automatic events with OnLevelWasLoaded.
// 2013-09-01, 1.1.1 - Granularized some of the system info statistics.
//                     Different way to generate client ID for Android.
//                     Send 1st launch data only when network is reachable.
// 2013-09-25, 1.1.3 - Unity 3.5 support.
// 2013-12-12, 1.1.4 - Fix for trying to use Handheld class on Windows 8.
// 2013-12-17, 1.2.0 - Added disableAnalyticsByUserOptOut in PlayerPrefs
//                     and use of gua.analyticsDisabled.
// 2014-02-11, 1.3.0 - Changed trackingID to be invalid dummy by default.

using UnityEngine;
using System.Collections;

public class Analytics : MonoBehaviour
{
    public string trackingID = "UA-XXXXXXX-Y"; // dummy id - use your actual id!
    public string appName = "GoogleUniversalAnalyticsForUnityExample";
    public string appVersion = "0.0.1";
    public string newLevelAnalyticsEventPrefix = "level-";
    public bool useHTTPS = false;

    public static GoogleUniversalAnalytics gua = null;

    // Private default instance.
    private static Analytics instance = null;
    // The default instance as a property.
    public static Analytics Instance { get { return instance; } }

    private string sceneName = "";

    private const string disableAnalyticsByUserOptOutPrefKey = "GoogleUniversalAnalytics_optOut";


    int getPOSIXTime()
    {
        return (int)(System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds;
    }

    // Example of a helper method. See commented-out
    // exampleAnalyticsTestHits() at end of this file for
    // more ideas of what to track, and what you could
    // make helper methods for.
    public static void changeScreen(string newScreenName)
    {
        gua.sendAppScreenHit(newScreenName);
    }


    // If analyticsDisabled is true, all analytics is disabled = no hits are sent.
    // If analyticsDisabled is false, analytics are enabled and hits will be sent
    // unless there is no internet reachability.
    // This setting is persistent (saved to PlayerPrefs).
    public static void setPlayerPref_disableAnalyticsByUserOptOut(bool analyticsDisabled)
    {
        if (gua != null)
            gua.analyticsDisabled = analyticsDisabled;
        PlayerPrefs.SetInt(disableAnalyticsByUserOptOutPrefKey, analyticsDisabled ? 1 : 0);
        PlayerPrefs.Save();
    }


    void Awake()
    {
        //Debug.Log("AnalyticsTesting Awake()");

        // prevent additional Analytics objects being created on level reloads
        if (instance)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        string clientID = "";
        const string clientIdPrefKey = "GoogleUniversalAnalytics_clientID";
        // If you want to force a new client ID for temporary testing,
        // uncomment following line for a single run:
        //////// PlayerPrefs.DeleteKey(clientIdPrefKey);
        // (remember to disable it again)

        if (PlayerPrefs.HasKey(clientIdPrefKey))
            clientID = PlayerPrefs.GetString(clientIdPrefKey);
        if (clientID.Length < 8 || !PlayerPrefs.HasKey(clientIdPrefKey))
        {
            // Need to generate unique & anonymous client ID for analytics.
            #if UNITY_ANDROID
            // Don't use SystemInfo.deviceUniqueIdentifier on Android,
            // as it would add requirement of READ_PHONE_STATE permission.
            // So we only use self-generated timestamp + random value.
            //Debug.Log("Creating new Android clientID for analytics...");
            clientID = getPOSIXTime().ToString("X8") + Random.Range(0, 0x7fffffff).ToString("x8");
#else
            // On most platforms we use device unique identifier but
            // additionally shuffle a random value inside it to make
            // sure it's anonymous.
            char[] id = (Random.Range(0, 0x7fffffff).ToString("x8") + SystemInfo.deviceUniqueIdentifier).ToCharArray();
            for (int a = 0; a < 8; ++a)
            {
                char c = id[a];
                int idx = Random.Range(0, id.Length);
                id[a] = id[idx];
                id[idx] = c;
            }
            clientID = new string(id);
            #endif

            //Debug.Log("Created client id for analytics: " + clientID);
            PlayerPrefs.SetString(clientIdPrefKey, clientID);
            PlayerPrefs.Save();
        }

        //bool useStringEscaping = true; // see the docs about this
        if (gua == null)
            gua = GoogleUniversalAnalytics.Instance;
        gua.initialize(trackingID, clientID, appName, appVersion, useHTTPS);
        //gua.setStringEscaping(useStringEscaping); // see the docs about this

        if (PlayerPrefs.HasKey(disableAnalyticsByUserOptOutPrefKey))
        {
            gua.analyticsDisabled = (PlayerPrefs.GetInt(disableAnalyticsByUserOptOutPrefKey, 0) != 0);
        }

        if (!gua.analyticsDisabled)
        {
            // Start by sending a hit with some generic info, including an app
            // screen hit with the first level name, since first scene doesn't get
            // automatically call to OnLevelWasLoaded.
            gua.beginHit(GoogleUniversalAnalytics.HitType.Appview);
            gua.addApplicationVersion();
            gua.addScreenResolution(Screen.currentResolution.width, Screen.currentResolution.height);
            gua.addViewportSize(Screen.width, Screen.height);
#if !UNITY_METRO && !UNITY_WEBPLAYER && !UNITY_STANDALONE && !UNITY_3_5 && !UNITY_FLASH
            gua.addScreenColors(Handheld.use32BitDisplayBuffer ? 32 : 16);
#endif
            // Note: this adds language e.g. as "English", although Google API example has "en-us".
            gua.addUserLanguage(Application.systemLanguage.ToString());
            //gua.addCustomDimension(1, "ScreenDPI");
            //gua.addCustomMetric(1, (int)Screen.dpi);
            gua.addContentDescription(newLevelAnalyticsEventPrefix + Application.loadedLevelName);
            gua.sendHit();


            // Next, client SystemInfo statistics are submitted ONCE on the first
            // launch when internet is reachable.

            // If you make a few version upgrades and at some point want to get
            // fresh statistics of your active users, update the category string
            // below and after next update users will re-submit SystemInfo once.
            const string category = "SystemInfo_since_v001";
            const string prefKey = "GoogleUniversalAnalytics_" + category;

            // Existing pref key could be deleted with following command:
            //// PlayerPrefs.DeleteKey(prefKey);
            // Warning: Do not enable that code row here (except for single time
            //          testing). Otherwise all following single time statistics
            //          hits would be sent on each launch.

            if (Application.internetReachability != NetworkReachability.NotReachable &&
                !PlayerPrefs.HasKey(prefKey))
            {
                gua.sendEventHit(category, "ScreenDPI", ((int)Screen.dpi).ToString(), (int)Screen.dpi);

                gua.sendEventHit(category, "operatingSystem", SystemInfo.operatingSystem);
                gua.sendEventHit(category, "processorType", SystemInfo.processorType);
                gua.sendEventHit(category, "processorCount", SystemInfo.processorCount.ToString(), SystemInfo.processorCount);
                // round down to 128MB chunks for label
                gua.sendEventHit(category, "systemMemorySize", (128 * (SystemInfo.systemMemorySize / 128)).ToString(), SystemInfo.systemMemorySize);
                // round down to 16MB chunks for label
                gua.sendEventHit(category, "graphicsMemorySize", (16 * (SystemInfo.graphicsMemorySize / 16)).ToString(), SystemInfo.graphicsMemorySize);
                gua.sendEventHit(category, "graphicsDeviceName", SystemInfo.graphicsDeviceName);
                gua.sendEventHit(category, "graphicsDeviceVendor", SystemInfo.graphicsDeviceVendor);
                gua.sendEventHit(category, "graphicsDeviceID", SystemInfo.graphicsDeviceID.ToString(), SystemInfo.graphicsDeviceID);
                gua.sendEventHit(category, "graphicsDeviceVendorID", SystemInfo.graphicsDeviceVendorID.ToString(), SystemInfo.graphicsDeviceVendorID);
                gua.sendEventHit(category, "graphicsDeviceVersion", SystemInfo.graphicsDeviceVersion);
                gua.sendEventHit(category, "graphicsShaderLevel", SystemInfo.graphicsShaderLevel.ToString(), SystemInfo.graphicsShaderLevel);
                // round down to chunks for label (10-5000 MPix depending on value range)
                int pixelFillrateChunkSize = 5000;
                if (SystemInfo.graphicsPixelFillrate < 40000)
                    pixelFillrateChunkSize = 2000;
                if (SystemInfo.graphicsPixelFillrate < 10000)
                    pixelFillrateChunkSize = 1000;
                else if (SystemInfo.graphicsPixelFillrate < 4000)
                    pixelFillrateChunkSize = 500;
                else if (SystemInfo.graphicsPixelFillrate < 1300)
                    pixelFillrateChunkSize = 100;
                else if (SystemInfo.graphicsPixelFillrate < 200)
                    pixelFillrateChunkSize = 20;
                else if (SystemInfo.graphicsPixelFillrate < 100)
                    pixelFillrateChunkSize = 10;
                gua.sendEventHit(category, "graphicsPixelFillrate", (pixelFillrateChunkSize * (SystemInfo.graphicsPixelFillrate / pixelFillrateChunkSize)).ToString(), SystemInfo.graphicsPixelFillrate);
                gua.sendEventHit(category, "deviceType", SystemInfo.deviceType.ToString());
                // round down to 512 chunks for label
#if !UNITY_3_5
                gua.sendEventHit(category, "maxTextureSize", (512 * (SystemInfo.maxTextureSize / 512)).ToString(), SystemInfo.maxTextureSize);
                gua.sendEventHit(category, "supports3DTextures", SystemInfo.supports3DTextures ? "yes" : "no", SystemInfo.supports3DTextures ? 1 : 0);
                gua.sendEventHit(category, "supportsComputeShaders", SystemInfo.supportsComputeShaders ? "yes" : "no", SystemInfo.supportsComputeShaders ? 1 : 0);
                gua.sendEventHit(category, "supportsInstancing", SystemInfo.supportsInstancing ? "yes" : "no", SystemInfo.supportsInstancing ? 1 : 0);
                gua.sendEventHit(category, "npotSupport", SystemInfo.npotSupport.ToString());
#endif
                gua.sendEventHit(category, "supportsShadows", SystemInfo.supportsShadows ? "yes" : "no", SystemInfo.supportsShadows ? 1 : 0);
                gua.sendEventHit(category, "supportsRenderTextures", SystemInfo.supportsRenderTextures ? "yes" : "no", SystemInfo.supportsRenderTextures ? 1 : 0);
                gua.sendEventHit(category, "supportedRenderTargetCount", SystemInfo.supportedRenderTargetCount.ToString(), SystemInfo.supportedRenderTargetCount);

                gua.sendEventHit(category, "deviceModel", SystemInfo.deviceModel);

                gua.sendEventHit(category, "supportsAccelerometer", SystemInfo.supportsAccelerometer ? "yes" : "no", SystemInfo.supportsAccelerometer ? 1 : 0);
                gua.sendEventHit(category, "supportsGyroscope", SystemInfo.supportsGyroscope ? "yes" : "no", SystemInfo.supportsGyroscope ? 1 : 0);
                gua.sendEventHit(category, "supportsLocationService", SystemInfo.supportsLocationService ? "yes" : "no", SystemInfo.supportsLocationService ? 1 : 0);
                gua.sendEventHit(category, "supportsVibration", SystemInfo.supportsVibration ? "yes" : "no", SystemInfo.supportsVibration ? 1 : 0);
                gua.sendEventHit(category, "supportsImageEffects", SystemInfo.supportsImageEffects ? "yes" : "no", SystemInfo.supportsImageEffects ? 1 : 0);
                PlayerPrefs.SetInt(prefKey, getPOSIXTime());
                PlayerPrefs.Save();
            }

        } // !gua.analyticsDisabled
	} // Awake

    void OnLevelWasLoaded(int level)
    {
        if (!sceneName.Equals(Application.loadedLevelName))
        {
            //Debug.Log("AnalyticsTesting: app screen event - switch to scene " + Application.loadedLevelName);
            sceneName = Application.loadedLevelName;
            GoogleUniversalAnalytics gua = GoogleUniversalAnalytics.Instance;
            gua.sendAppScreenHit(newLevelAnalyticsEventPrefix + sceneName);
        }
    }

    void Start()
    {
        //exampleAnalyticsTestHits();
    }

    /*
    void exampleAnalyticsTestHits()
    {
        GoogleUniversalAnalytics gua = GoogleUniversalAnalytics.Instance;


        // event for entering "test-start" screen
        gua.sendAppScreenHit("test-start");


        // event in "test-main" category with "test-enter-shop" action,
        // rest of the parameters are optional label and value.
        gua.sendEventHit("test-main", "test-enter-shop", "TestEnterShop", 123);


        // ad hoc example of a purchase transaction containing bunch of items
        // Note: Not sure if there is recommendation if item prices should
        //       include or exclude shipping & taxes. Probably best to compare
        //       the two options, pick one and stick to it. Here we use item
        //       prices which include all the extra costs.
        string transactionID = gua.clientID + UnityEngine.Random.Range(0, 1000000000);
        // here are some test values which are hopefully reasonable enough
        string currencyCode = "EUR"; // http://en.wikipedia.org/wiki/ISO_4217#Active_codes
        int itemQuantity = 10;
        double totalPrice = 9.99;
        double itemPrice = totalPrice / itemQuantity;
        double vatPc = 0.24; // for example - Finland has 24% VAT as of 2013
        double shippingPc = 0.30; // let's use sales channel cost as "shipping", and assume it equals 30% after taxes
        double tax = totalPrice - totalPrice / (1 + vatPc);
        double shipping = (totalPrice - tax) * shippingPc;
        gua.sendTransactionHit(transactionID, "test-affiliation", totalPrice, shipping, tax, "EUR");
        string itemName = "test-item3";
        string itemCode = "TESTSKU003";
        string itemCategory = "test-items";
        gua.sendItemHit(transactionID, itemName, itemPrice, itemQuantity, itemCode, itemCategory, currencyCode);


        // some examples of social media events: (+1 and Like)
        gua.sendSocialHit("GooglePlus", "plus", "test-social-target-gplus");
        gua.sendSocialHit("Facebook", "like", "test-social-target-fb");


        // could log exceptions like this (this one is non-fatal)
        // Note: If a truly fatal exception occurs, handling/sending those to
        //       GUA might not be possible with the Unity's WWW class, so for
        //       reliable hard exception tracking you should look at using some
        //       dedicated packages with OS-native implementation.
        gua.sendExceptionHit("test-exception", false);


        // example of timing event, e.g. for measuring average loading times...
        gua.sendUserTimingHit("loadtimes", "init", 100, "test-loadtimes-init");


        // test custom appview event of going to screen named "test-end",
        // with the event containing forced end of current session
        //
        // (The default gua.sendSomething()-style helpers check for the
        //  analyticsDisabled as the first thing, but for custom hits it's
        //  good to check that first like we do here. This way we won't
        //  build a hit for nothing to be discarded by gua.sendHit() if
        //  analytics is disabled due to user opt-out, for example.)
        //
        if (!gua.analyticsDisabled)
        {
            gua.beginHit(GoogleUniversalAnalytics.HitType.Appview);
            gua.addApplicationVersion();
            gua.addContentDescription("test-end");
            gua.addSessionControl(false);
            gua.sendHit();
        }
    }
    */
}
