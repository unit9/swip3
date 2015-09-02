// MonoSingleton.cs
// 
// Created by Nick McVroom-Amoakohene <nicholas@unit9.com> on 07/09/2012.
// 
// Copyright (c) 2012 unit9 ltd. www.unit9.com. All rights reserved
using System;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour {
	
    private static T _instance;
    public static T Instance {
        get {
			if(_instance != null) {
                return _instance;
            }
            else {
                string message = typeof(T).Name + " is not attached to a gameObject or the gameObject is not active.\nMake also sure to set 'Instance = this;' in your Awake() function!";
				
				Debug.LogError(message);
                return default(T);
            }
        }
       
		protected set {
            _instance = value;
        }
    }

    public static bool IsInitialized {
        get { return _instance != null; }
    }

    protected virtual void OnApplicationQuit() {
        _instance = default(T);
    }
}

