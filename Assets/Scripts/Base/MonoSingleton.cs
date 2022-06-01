using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T:MonoBehaviour
{
    private static T _instance;
    public static T Inst
    {
        get => _instance;
    }

    protected virtual void Awake()
    {
        if (_instance==null)
        {
            _instance = GetComponent<T>();
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
}
