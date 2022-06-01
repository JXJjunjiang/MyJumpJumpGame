using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoSingleton<CoroutineManager>
{
    private Dictionary<string, GameObject> coroutineCache;

    protected override void Awake()
    {
        base.Awake();
        coroutineCache = new Dictionary<string, GameObject>();
    }

    public void Run(IEnumerator coroutine,string tag)
    {
        if (coroutineCache.ContainsKey(tag))
        {
            Debug.LogError("this coroutine has run,tag:" + tag);
            return;
        }
        CoroutineComponent component = CreateCoroutineItem(tag);
        component.CreateCoroutine(coroutine,tag);
    }

    CoroutineComponent CreateCoroutineItem(string tag)
    {
        GameObject obj = new GameObject(tag);
        obj.transform.SetParent(transform);
        return obj.RequireComponent<CoroutineComponent>();
    }

    public void DestroyCoroutine(string tag)
    {
        if (!coroutineCache.ContainsKey(tag))
        {
            Debug.LogWarning("this coroutine does not exsit in cache,tag:" + tag);
            return;
        }
        DestroyImmediate(coroutineCache[tag]);
        coroutineCache.Remove(tag);
    }
}
