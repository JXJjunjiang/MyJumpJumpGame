using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class GameManager : Singalton<GameManager>
{
    private const string PlatformPrefabName = "Platform";
    public readonly Vector3 PlatformPrefabSize = new Vector3(3f, 2f, 3f);
    private readonly Vector2 ClampSpawnX = new Vector2(-10f, -5f);
    private readonly Vector2 ClampSpawnZ = new Vector2(5f, 10f);
    private readonly float SpawnY = 1f;

    private GameObject platformPrefab = null;
    private Transform platformRoot = null;
    private List<Transform> spawnPlatform;

    protected override void Awake ()
    {
        base.Awake();
        platformPrefab = Resources.Load(PlatformPrefabName) as GameObject;
        platformRoot = GameObject.Find("PlatformRoot").transform;
        spawnPlatform = new List<Transform>();
        spawnPlatform.Add(GameObject.Find("Platform").transform);
    }

    /// <summary>
    /// 生成跳跃平台
    /// </summary>
    /// <param name="spawnPos"></param>
    /// <returns></returns>
    public JumpTrs GeneratePlatform(Transform prePlatform)
    {
        Vector3 spawnPos = GetNextPlatformPos(prePlatform.localPosition);
        Transform result = Instantiate<GameObject>(platformPrefab, platformRoot).transform;
        result.name = PlatformPrefabName + spawnPlatform.Count.ToString();
        spawnPlatform.Add(result);
        result.Reset();
        result.localScale = PlatformPrefabSize;
        result.localPosition = new Vector3(spawnPos.x, -SpawnY, spawnPos.z);
        result.DOLocalMoveY(SpawnY, 0.2f);

        return new JumpTrs(SearchSpawnList(prePlatform.name), result);
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
}

public class Singalton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get => _instance;
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else
        {
            _instance = null;
            DestroyImmediate(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
public static class UnityExtension
{
    public static void Reset(this Transform trs)
    {
        trs.localScale = Vector3.one;
        trs.localPosition = Vector3.zero;
        trs.localRotation = Quaternion.identity;
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
