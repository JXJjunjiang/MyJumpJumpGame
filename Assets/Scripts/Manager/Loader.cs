using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    private const string PrefabPath = "Prefab/";
    private const string AudioPath = "Audio/";
    private const string AudioSettingPath = "GameAudio";

    public static GameObject LoadPrefab(string prefabName)
    {
        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError("prefab name is empty");
            return null;
        }
        GameObject targetLoad = Resources.Load<GameObject>(PrefabPath + prefabName);
        if (targetLoad==null)
        {
            Debug.LogError("prefab does not exsit in " + PrefabPath+prefabName);
            return null;
        }
        return targetLoad;
    }

    public static AudioClip LoadAudio(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
        {
            Debug.LogError("clip name is empty");
            return null;
        }
        AudioClip targetLoad = Resources.Load<AudioClip>(AudioPath + clipName);
        if (targetLoad == null)
        {
            Debug.LogError("audioclip does not exsit in " + AudioPath);
            return null;
        }
        return targetLoad;
    }

    public static AudioSetting LoadAudioSetting()
    {
        return Resources.Load<AudioSetting>(AudioSettingPath);
    }
}
