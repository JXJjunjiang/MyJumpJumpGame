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
        GameObject loadObj = Loader.LoadGame(string.Format("Player_{0}", itemInfo.id));
        GameObject spawnObj = MonoBehaviour.Instantiate(loadObj);
        SetCharacterInfo(spawnObj.transform, itemInfo as PlayerInfo);
        return spawnObj;
    }

    private void SetCharacterInfo(Transform trs,PlayerInfo info)
    {
        trs.parent = info.parent;
        trs.name = "Player_" + info.id;
        trs.localScale = info.scale;
        trs.localPosition = info.position;
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
    }
}
