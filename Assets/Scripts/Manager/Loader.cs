using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    private const string GamePath = "Prefab/Game/";
    private const string UIPath = "Prefab/UI/";
    private const string SpritePath = "Sprite/";
    private const string AudioPath = "Audio/";
    private const string AudioSettingPath = "GameAudio";

    public static GameObject LoadUI(string uiName)
    {
        if (string.IsNullOrEmpty(uiName))
        {
            Debug.LogError("ui name is empty");
            return null;
        }
        GameObject targetLoad = Resources.Load<GameObject>(UIPath + uiName);
        if (targetLoad==null)
        {
            Debug.LogError("prefab does not exsit in " + UIPath+uiName);
            return null;
        }
        return targetLoad;
    }

    public static Sprite LoadSprite(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName))
        {
            Debug.LogError("spriteName name is empty");
            return null;
        }
        Sprite targetLoad = Resources.Load<Sprite>(SpritePath + spriteName);
        if (targetLoad == null)
        {
            Debug.LogError("sprite does not exsit in " + SpritePath + spriteName);
            return null;
        }
        return targetLoad;
    }

    public static GameObject LoadGame(string gameName)
    {
        if (string.IsNullOrEmpty(gameName))
        {
            Debug.LogError("game name is empty");
            return null;
        }
        GameObject targetLoad = Resources.Load<GameObject>(GamePath + gameName);
        if (targetLoad == null)
        {
            Debug.LogError("prefab does not exsit in " + GamePath + gameName);
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
