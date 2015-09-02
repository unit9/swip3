// MonoMultiton.cs
// 
// Created by Nick McVroom-Amoakohene <nicholas@unit9.com> on 07/09/2012.
// 
// Copyright (c) 2012 unit9 ltd. www.unit9.com. All rights reserved
using System;
using System.Collections.Generic;
using UnityEngine;

public class MonoMultiton<T> : MonoBehaviour
{
    private static readonly Dictionary<object, T> _instances = new Dictionary<object, T>();

    public static T GetInstance(object key) {
        lock(_instances) {   
            T _instance;
			
            if(_instances.TryGetValue(key, out _instance)) {
				return _instance;
            }
			else {
                string message = typeof(T).Name + " is not attached to a gameObject or the gameObject is not active.\nMake also sure to set 'SetInstance(key, this)' in your Awake() function!";
				
				Debug.LogError(message);
               	return default(T);
			}
        }
    }
	
	protected static void SetInstance(object key, T instance) {
		_instances.Add(key, instance);
	}

//    public static bool IsInitialized {
//        get { return _instances.Count > 0; }
//    }

    protected virtual void OnApplicationQuit() {
        _instances.Clear();
    }
}