using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Factory;
using System;

public class PlatformFactory : IFactory,IPool, IDisposable
{
    private Queue<GameObject> createCaches;
    private Stack<GameObject> recycleCaches;
    private const int MaxCreateCount = 5;
    private int index = 0;
    public PlatformFactory()
    {
        createCaches = new Queue<GameObject>();
        recycleCaches = new Stack<GameObject>();
    }

    public GameObject Create(ItemInfoBase itemInfo)
    {
        if (createCaches.Count>=MaxCreateCount)
        {
            Recycle(createCaches.Dequeue());
        }

        GameObject tempObj = recycleCaches.Count > 0 ? Request() : MonoBehaviour.Instantiate(Loader.LoadGame(string.Format("Platform_{0}", itemInfo.id)));
        createCaches.Enqueue(tempObj);
        ++index;
        SetPlatformInfo(tempObj.transform, itemInfo as PlatformInfo);
        return tempObj;
    }

    void SetPlatformInfo(Transform trs,PlatformInfo info)
    {
        trs.parent = info.parent;
        trs.name = "Platform_" + index;
        trs.localScale = info.scale;
        trs.localPosition = info.position;
    }

    public GameObject Request()
    {
        return recycleCaches.Pop();
    }

    public void Recycle(GameObject obj)
    {
        recycleCaches.Push(obj);
    }

    public void Dispose()
    {
        using (var e=createCaches.GetEnumerator())
        {
            while (e.MoveNext())
            {
                MonoBehaviour.DestroyImmediate(e.Current);
            }
        }
        createCaches.Clear();

        using (var e = recycleCaches.GetEnumerator())
        {
            while (e.MoveNext())
            {
                MonoBehaviour.DestroyImmediate(e.Current);
            }
        }
        recycleCaches.Clear();
    }
}
