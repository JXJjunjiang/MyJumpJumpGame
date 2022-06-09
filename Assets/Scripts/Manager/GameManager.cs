using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class GameManager : MonoSingleton<GameManager>,IMgrInit
{
    /// <summary>
    /// 跳跃平台预制体名称
    /// </summary>
    private const string PlatformPrefabName = "Platform";
    /// <summary>
    /// 跳跃平台预制体尺寸
    /// </summary>
    public readonly Vector3 PlatformPrefabSize = new Vector3(3f, 2f, 3f);
    /// <summary>
    /// 生成平台在X轴上的间隔
    /// </summary>
    private readonly Vector2 ClampSpawnX = new Vector2(-10f, -5f);
    /// <summary>
    /// 生成平台在Z轴上的间隔
    /// </summary>
    private readonly Vector2 ClampSpawnZ = new Vector2(5f, 10f);
    /// <summary>
    /// 平台Y轴位置
    /// </summary>
    private readonly float SpawnY = 1f;
    /// <summary>
    /// 平台对象池的最大容量
    /// </summary>
    private const int PoolMaxItemsCount = 5;

    private Transform gameRoot = null;
    private GameObject platformPrefab = null;
    private Transform platformRoot = null;
    private Transform playerRoot = null;
    private Queue<Transform> spawnPlatforms;
    private int platformIndex = 0;

    private static bool _isGameEnd = false;
    /// <summary>
    /// 游戏是否结束(主角死亡)
    /// </summary>
    public static bool IsGameEnd
    {
        get => _isGameEnd;
        set => _isGameEnd = value;
    }

    private static bool _canControll;
    /// <summary>
    /// 是否可控制
    /// </summary>
    public static bool CanControll
    {
        get => _canControll;
        set => _canControll = value;
    }


    public void Init()
    {
        gameRoot = Instantiate<Transform>(Loader.LoadGame("Game").transform);
        gameRoot.Reset();
        platformPrefab = Loader.LoadGame(PlatformPrefabName);
        platformRoot = gameRoot.Find("PlatformRoot");
        spawnPlatforms = new Queue<Transform>(PoolMaxItemsCount);
    }

    public void GameStart()
    {
        _isGameEnd = false;
        _canControll = false;
        platformIndex = 0;
        while (spawnPlatforms.Count>0)
        {
            DestroyImmediate(spawnPlatforms.Dequeue().gameObject);
        }
        spawnPlatforms.Clear();

        if (playerRoot==null)
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

    public void GameExit()
    {

    }

    public Transform SpawnPlatform(Vector3 pos)
    {
        Transform result = PoolDecollect();
        result.SetParent(platformRoot);
        result.name = string.Format("{0}[{1}]", PlatformPrefabName, platformIndex);
        ++platformIndex;
        result.Reset();
        result.localScale = PlatformPrefabSize;
        result.localPosition = new Vector3(pos.x, -SpawnY, pos.z);
        _canControll = false;
        result.DOLocalMoveY(SpawnY, 0.2f).onComplete = () => _canControll = true;//生成平台过程中，不允许玩家操作
        return result;
    }

    public void PoolCollect(Transform platform)
    {
        if (spawnPlatforms!=null)
        {
            spawnPlatforms.Enqueue(platform);
        }
    }

    public Transform PoolDecollect()
    {
        Transform result = null;
        //if (spawnPlatforms.Count>=PoolMaxItemsCount)
        //{
        //    result = spawnPlatforms.Dequeue();
        //}
        //else
        //{
            result = Instantiate<Transform>(platformPrefab.transform);
            spawnPlatforms.Enqueue(result);
        //}
        return result;
    }

    public Vector3 GetNextPlatformPos(Vector3 prePos)
    {
        Vector3 result = Vector3.zero;
        float x = 0;
        float z = 0;
        if (Random.Range(1,2)==1)
        {
            x = Random.Range(ClampSpawnX.x, ClampSpawnX.y);
        }
        else
        {
            z = Random.Range(ClampSpawnZ.x, ClampSpawnZ.y);
        }
        result.Set(prePos.x + x, SpawnY, prePos.z + z);
        return result;
    }

    public Transform SearchPool(string name)
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

    void OnDisable()
    {
        UnInit();
    }

    public void UnInit()
    {
        
    }
}