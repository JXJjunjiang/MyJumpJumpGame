using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour,IMgrInit
{
    private List<IMgrInit> managers;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        DontDestroyOnLoad(this.gameObject);
        managers = new List<IMgrInit>();
        managers.Add(CreateTargetManager<AudioManager>("AudioManager"));
        managers.Add(CreateTargetManager<DatabaseMgr>("DatabaseManager"));
        managers.Add(CreateTargetManager<UIManager>("UIManager"));
        managers.Add(CreateTargetManager<GameManager>("GameManager"));
        managers.Add(CreateTargetManager<DelayManager>("DelayManager"));
        foreach (var manager in managers)
        {
            manager.Init();
        }
    }

    public void UnInit()
    {
        foreach (var manager in managers)
        {
            manager.UnInit();
        }
        managers.Clear();
    }


    T CreateTargetManager<T>(string managerName) where T : MonoSingleton<T>
    {
        GameObject managerItem = new GameObject(managerName);
        managerItem.transform.SetParent(transform);
        return managerItem.RequireComponent<T>();
    }
}
