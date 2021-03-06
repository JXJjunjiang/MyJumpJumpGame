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
        managers.Add(CreateTargetManager<AudioMgr>("AudioManager"));
        managers.Add(CreateTargetManager<DatabaseMgr>("DatabaseManager"));
        managers.Add(CreateTargetManager<UIMgr>("UIManager"));
        managers.Add(CreateTargetManager<GameMgr>("GameManager"));
        managers.Add(CreateTargetManager<Factory.CreateFactory>("Factory"));
        foreach (var manager in managers)
        {
            manager.Init();
        }
        SetOrientation();
    }

    public void UnInit()
    {
        foreach (var manager in managers)
        {
            manager.UnInit();
        }
        managers.Clear();
    }

    private void OnDestroy()
    {
        UnInit();
    }

    private void SetOrientation()
    {
#if UNITY_ANDROID
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToPortrait = true;
#endif
    }

    T CreateTargetManager<T>(string managerName) where T : MonoSingleton<T>
    {
        GameObject managerItem = new GameObject(managerName);
        managerItem.transform.SetParent(transform);
        return managerItem.RequireComponent<T>();
    }
}
