using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDispatcher
{
    public delegate void Listener(EventArgs args);
    private static Dictionary<string, Listener> cacheEvents = new Dictionary<string, Listener>();

    public static void Attach(string tag,Listener listen)
    {
        if (cacheEvents==null)
        {
            cacheEvents = new Dictionary<string, Listener>();
        }
        if (cacheEvents.ContainsKey(tag))
        {
            Debug.LogWarning("this tag already exsit in cache,please check agin,tag name:" + tag);
            return;
        }
        if (listen==null)
        {
            Debug.LogWarning("listen is null,cache failed");
            return;
        }
        cacheEvents.Add(tag, listen);
    }

    public static void Detach(string tag)
    {
        if (!cacheEvents.ContainsKey(tag))
        {
            Debug.LogWarning("tag is not exsit in cache,tag name:"+tag);
            return;
        }
        cacheEvents.Remove(tag);
    }

    public static void Dispatch(string tag,EventArgs args)
    {
        if (!cacheEvents.ContainsKey(tag))
        {
            Debug.LogWarning("this tag does not exsit in cache,please check agin,tag name:" + tag);
            return;
        }
        if (cacheEvents[tag]==null)
        {
            Debug.LogWarning("this listen is null,invoke failed");
            return;
        }
        cacheEvents[tag].Invoke(args);
    }
}
public class EventArgs
{
    private List<System.Object> parameters;
    public int Count
    {
        get
        {
            if (parameters!=null)
            {
                return parameters.Count;
            }
            else
            {
                Debug.Log("parameters is not init");
                return 0;
            }
        }
    }
    public EventArgs(params System.Object[] parameters)
    {
        if (this.parameters==null)
        {
            this.parameters = new List<object>();
        }
        for (int i = 0; i < parameters.Length; i++)
        {
            this.parameters.Add(parameters[i]);
        }
    }
    public System.Object this[int index]
    {
        get
        {
            if (index>=0||index<parameters.Count)
            {
                return parameters[index];
            }
            else
            {
                Debug.LogError("index must be in range of parameters");
                return null;
            }
        }
    }
}
