using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class GameManager : MonoSingleton<GameManager>,IMgrInit
{
    private const string PlatformPrefabName = "Platform";
    public readonly Vector3 PlatformPrefabSize = new Vector3(3f, 2f, 3f);
    /// <summary>
    /// 生成平台在X轴上的间隔
    /// </summary>
    private readonly Vector2 ClampSpawnX = new Vector2(-10f, -5f);
    /// <summary>
    /// 生成平台在Z轴上的间隔
    /// </summary>
    private readonly Vector2 ClampSpawnZ = new Vector2(5f, 10f);
    private readonly float SpawnY = 1f;

    private Transform gameRoot = null;
    private GameObject platformPrefab = null;
    private Transform platformRoot = null;
    private List<Transform> spawnPlatform;

    protected override void Awake ()
    {
        base.Awake();
        Init();
        EventDispatcher.Attach("StartGame", StartGame);
    }

    public void Init()
    {
        gameRoot = Instantiate<Transform>(Loader.LoadPrefab("Game").transform);
        platformPrefab = Loader.LoadPrefab(PlatformPrefabName);
        platformRoot = gameRoot.Find("PlatformRoot");
        spawnPlatform = new List<Transform>();
    }

    void StartGame(EventArgs args)
    {
        /*生成一个平台，主角从天而降，落稳后再次生成另一个新平台*/
        SpawnPlatform(Vector3.zero);
    }

    /// <summary>
    /// 生成跳跃平台
    /// </summary>
    /// <param name="spawnPos"></param>
    /// <returns></returns>
    public JumpTrs GeneratePlatform(Transform prePlatform)
    {
        Vector3 spawnPos = prePlatform == null ? Vector3.zero : GetNextPlatformPos(prePlatform.localPosition);
        Transform result = SpawnPlatform(spawnPos);
        return new JumpTrs(SearchSpawnList(prePlatform.name), result);
    }

    public Transform SpawnPlatform(Vector3 pos)
    {
        Transform result = Instantiate<Transform>(platformPrefab.transform, platformRoot);
        result.name = PlatformPrefabName + spawnPlatform.Count.ToString();
        spawnPlatform.Add(result);
        result.Reset();
        result.localScale = PlatformPrefabSize;
        result.localPosition = new Vector3(pos.x, -SpawnY, pos.z);
        result.DOLocalMoveY(SpawnY, 0.2f);
        return result;
    }

    Vector3 GetNextPlatformPos(Vector3 prePos)
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

    Transform SearchSpawnList(string name)
    {
        using (var e=spawnPlatform.GetEnumerator())
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
public class JumpTrs
{
    public Transform currentTrs;
    public Transform nextTrs;
    public JumpTrs(Transform cur,Transform next)
    {
        this.currentTrs = cur;
        this.nextTrs = next;
    }
    private JumpTrs()
    {

    }
}
public interface IMgrInit
{
    void Init();

    void UnInit();
}
