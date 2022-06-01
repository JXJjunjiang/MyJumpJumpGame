using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePort : MonoBehaviour
{
    private Transform managerParentTrs;

    private void Awake()
    {
        CreateManagers();
    }

    void CreateManagers()
    {
        if (managerParentTrs==null)
        {
            managerParentTrs = new GameObject("Managers").transform;
            DontDestroyOnLoad(managerParentTrs.gameObject);
        }
        CreateTargetManager<AudioManager>("AudioManager");
        CreateTargetManager<CoroutineManager>("CoroutineManager");
        CreateTargetManager<GameManager>("GameManager");
    }

    void CreateTargetManager<T>(string managerName) where T : MonoSingleton<T>
    {
        GameObject managerItem = new GameObject(managerName);
        managerItem.transform.SetParent(managerParentTrs);
        managerItem.RequireComponent<T>();
    }
}
