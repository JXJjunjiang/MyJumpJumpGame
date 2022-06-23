using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Factory;
using System;

public class CharacterFactory : IFactory, IDisposable
{
    private Dictionary<string, GameObject> createCaches;
    public CharacterFactory()
    {
        createCaches = new Dictionary<string, GameObject>();
    }
    public GameObject Create(ItemInfoBase itemInfo)
    {
        return new GameObject();
    }

    public void Dispose()
    {
        using (var e=createCaches.GetEnumerator())
        {
            while (e.MoveNext())
            {
                MonoBehaviour.DestroyImmediate(e.Current.Value);
            }
        }
        createCaches.Clear();
        createCaches = null;
    }
}
