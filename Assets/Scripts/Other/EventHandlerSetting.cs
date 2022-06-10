using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EventHandlerSetting : ScriptableObject
{
    public List<EventType> types;
    public List<EventItem> items;
}

[System.Serializable]
public class EventItem
{
    public string eventName;
    public string typeName;
}

[System.Serializable]
public class EventType
{
    public string typeName;
    public string typeDelegate;
}
