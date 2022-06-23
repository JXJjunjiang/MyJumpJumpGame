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

        GameObject tempObj = null;
        if (recycleCaches.Count>0)
        {
            tempObj = Request();
        }
        else
        {
            tempObj=new GameObject();
        }
        createCaches.Enqueue(tempObj);
        tempObj.name = "platform" + index;
        ++index;
        return tempObj;

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
        createCaches = null;

        using (var e = recycleCaches.GetEnumerator())
        {
            while (e.MoveNext())
            {
                MonoBehaviour.DestroyImmediate(e.Current);
            }
        }
        recycleCaches.Clear();
        recycleCaches = null;
    }
}
