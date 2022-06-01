using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyExtension
{
    public static T RequireComponent<T>(this GameObject obj) where T:Behaviour
    {
        T component = obj.GetComponent<T>();
        if (component==null)
        {
            component = obj.AddComponent<T>();
        }
        return component;
    }
}
