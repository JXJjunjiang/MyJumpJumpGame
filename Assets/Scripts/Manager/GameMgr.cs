using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Factory;
using UnityEngine.Rendering.Universal;


public class GameMgr : MonoSingleton<GameMgr>,IMgrInit
{
    public static Vector3 PlatformPrefabSize = new Vector3(3f, 10f, 3f);// 跳跃平台预制体尺寸
    private static Vector2 ClampSpawnX = new Vector2(-10f, -5f);// 生成平台在X轴上的间隔
    private static Vector2 ClampSpawnZ = new Vector2(5f, 10f);// 生成平台在Z轴上的间隔
    private static float SpawnY = PlatformPrefabSize.y / 2f;// 平台Y轴位置

    private bool canControll = false;
    private Transform gameRoot = null;
    private Transform platformRoot = null;
    private Transform playerRoot = null;
    private Stack<Transform> spawnPlatforms = null;

    /// <summary>
    /// 是否可控制
    /// </summary>
    public bool CanControll
    {
        get => canControll;
        set => canControll = value;
    }


    public void Init()
    {
        spawnPlatforms = new Stack<Transform>();
    }

    public void GameStart()
    {
        InitData();
        InitPlayer();
    }

    public void GameEnd()
    {
        CreateFactory.Inst.Clear();
        spawnPlatforms.Clear();
    }

    private void InitData()
    {
        canControll = false;
    }

    private void InitPlayer()
    {
        if (playerRoot == null)
        {
            GameObject obj = new GameObject("PlayerRoot");
            playerRoot = obj.transform;
            playerRoot.SetParent(gameRoot);
        }
        playerRoot.Reset();
        var controller = playerRoot.RequireComponent<PlayerController>();
        controller.Uninit();
        controller.Init();
    }

    public void InitGameRoot()
    {
        if (gameRoot==null)
        {
            gameRoot = Instantiate<Transform>(Loader.LoadGame("Game").transform);
            if (platformRoot==null)
            {
                platformRoot = gameRoot.Find("PlatformRoot");
            }
        }
        gameRoot.Reset();
        Camera _mainCamera = gameRoot.Find("Main Camera").GetComponent<Camera>();
        var _cameraData = _mainCamera.GetUniversalAdditionalCameraData();
        _cameraData.cameraStack.Add(UIMgr.Inst.UICamera);
    }

    #region SpawnPlatform
    public Transform GetFirstPlatform()
    {
        return SpawnPlatform(Vector3.zero);
    }

    public Transform GetCurrentPlatform()
    {
        return SearchPool(spawnPlatforms.Peek().name);
    }

    public Transform GetNextPlatform()
    {
        Vector3 pos = GetNextPlatformPos(SearchPool(spawnPlatforms.Peek().name).localPosition);
        return SpawnPlatform(pos);
    }
    #endregion

    #region SpawnBaseMethod
    private Transform SpawnPlatform(Vector3 pos)
    {
        PlatformInfo info = new PlatformInfo(DatabaseMgr.Inst.EnvironmentID, platformRoot, PlatformPrefabSize, new Vector3(pos.x, -SpawnY, pos.z));
        Transform result = CreateFactory.Inst.Create(FactoryType.Platform, info).transform;
        spawnPlatforms.Push(result);
        canControll = false;
        result.DOLocalMoveY(SpawnY, 0.2f).onComplete = () => canControll = true;
        return result;
    }

    private Vector3 GetNextPlatformPos(Vector3 prePos)
    {
        Vector3 result = Vector3.zero;
        float x = 0;
        float z = 0;
        if (Random.Range(1,3)==1)
        {
            x = Random.Range(ClampSpawnX.x, ClampSpawnX.y);
        }
        else
        {
            z = Random.Range(ClampSpawnZ.x, ClampSpawnZ.y);
        }
        result.Set(prePos.x + x, SpawnY + DatabaseMgr.Inst.Height, prePos.z + z);
        return result;
    }

    private Transform SearchPool(string name)
    {
        using (var e=spawnPlatforms.GetEnumerator())
        {
            while (e.MoveNext())
            {
                if (e.Current.name==name)
                {
                    return e.Current;
                }
            }
        }
        return null;
    }
    #endregion

    public void UnInit()
    {
        CreateFactory.Inst.Clear();
        spawnPlatforms.Clear();
    }
}