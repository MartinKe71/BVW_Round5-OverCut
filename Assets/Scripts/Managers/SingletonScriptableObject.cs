﻿using UnityEngine;

public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static T _instance = null;

    public static T Instance
    {
        get{
            if(_instance == null)
            {
                T[] results = Resources.FindObjectsOfTypeAll<T>();
                if(results.Length == 0)
                {
                    Debug.LogError("SingletonScriptableOject -> Instance -> results length is 0 for type " + typeof(T).ToString()+ ".");
                    return null;
                }
                if(results.Length > 1)
                {
                    Debug.LogError("SingletonScriptableOject -> Instance -> results length is greater that 1 for type " + typeof(T).ToString()+ ".");
                }

                _instance = results[0];
            }
            return _instance;
        }
    }
}


