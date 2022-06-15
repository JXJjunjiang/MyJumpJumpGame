using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayManager : MonoSingleton<DelayManager>, IMgrInit
{
    private Dictionary<string, Coroutine> coroutineCache;
    public void Init()
    {
        coroutineCache = new Dictionary<string, Coroutine>();
    }

    public void DelayDo(string tag,float delayTime,System.Action callback)
    {
        if (string.IsNullOrEmpty(tag))
        {
            Debug.LogError("Tag can not be null or empty");
            return;
        }
        if (coroutineCache.ContainsKey(tag))
        {
            Debug.LogError("can not exsit same tag");
            return;
        }
        if (callback==null)
        {
            Debug.LogError("callback is null");
            return;
        }
        coroutineCache.Add(tag, StartCoroutine(Delay(tag, delayTime, callback)));
    }

    private IEnumerator Delay(string tag,float delayTime,System.Action callback)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        callback?.Invoke();
        coroutineCache.Remove(tag);
    }

    public void UnInit()
    {
        using (var e=coroutineCache.GetEnumerator())
        {
            while (e.MoveNext())
            {
                StopCoroutine(e.Current.Value);
            }
        }
        coroutineCache.Clear();
    }
}
