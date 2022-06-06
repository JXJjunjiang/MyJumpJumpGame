using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>,IMgrInit
{
    private AudioSetting gameAudio;
    private const string backgroundMusic= "BGM";
    private const string buttonMusic = "Button";
    private Dictionary<string, GameObject> audioItems;


    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    public void Init()
    {
        audioItems = new Dictionary<string, GameObject>();
        gameAudio = Loader.LoadAudioSetting();
    }

    public void PlayBGM()
    {
        if (gameAudio.EnableBackgroundMusic)
        {
            PlayAudio(backgroundMusic, true, true);
        }
    }

    public void StopBGM()
    {
        StopAudio(backgroundMusic, true);
    }

    public void PlayButtonAudio()
    {
        if (gameAudio.EnableGameMusic)
        {
            PlayAudio(buttonMusic, false, false);
        }
    }

    public void StopButton()
    {
        StopAudio(buttonMusic, false);
    }

    public void PlayAudio(string audioName,bool isAwake,bool isLoop,string tag="")
    {
        AudioSource audioSource = null;
        if (audioItems.ContainsKey(audioName+tag))
        {
#if UNITY_EDITOR
            audioItems[audioName + tag].SetActive(true);
#endif
            audioSource = audioItems[audioName + tag].RequireComponent<AudioSource>();
        }
        else
        {
            CreateAudioItem(audioName + tag);
        }
        audioSource.playOnAwake = isAwake;
        audioSource.loop = isLoop;
        audioSource.clip = Loader.LoadAudio(audioName);
        audioSource.Play();
#if UNITY_EDITOR
        if (!isLoop)//如果在编辑器模式下，非循环播放，则播放完直接销毁
        {
            CoroutineManager.Inst.Run(CheckAudioFinish(audioSource, audioName + tag), audioName + tag);
        }
#endif
    }

    public void StopAudio(string audioName,bool isLoop=false,string tag="")
    {
        if (!audioItems.ContainsKey(audioName + tag))
        {
            Debug.LogError("The target Audio does not exsit in cache");
            return;
        }
#if UNITY_EDITOR
        DestroyImmediate(audioItems[audioName + tag]);
        audioItems.Remove(audioName + tag);
        if (!isLoop)
        {
            CoroutineManager.Inst.DestroyCoroutine(audioName + tag);
        }
#elif ANDROID
        audioItems[audioName + tag].SetActive(false);
#endif
    }

    public bool IsPlaying(string audioName,string tag="")
    {
        if (!audioItems.ContainsKey(audioName+tag))
        {
            Debug.LogError("The target Audio does not exsit in cache");
            return false;
        }
        return audioItems[audioName + tag].RequireComponent<AudioSource>().isPlaying;
    }


    AudioSource CreateAudioItem(string itemName)
    {
        GameObject obj = new GameObject(itemName);
        obj.transform.SetParent(transform);
        audioItems.Add(itemName, obj);
        return obj.RequireComponent<AudioSource>();
    }

    IEnumerator CheckAudioFinish(AudioSource audioSource,string audioItemName)
    {
        while (audioSource.isPlaying)
        {
            yield return new WaitForSecondsRealtime(1f);
        }
        if (audioItems.ContainsKey(audioItemName))
        {
            DestroyImmediate(audioItems[audioItemName]);
            audioItems.Remove(audioItemName);
        }
    }

    void OnDisable()
    {
        UnInit();
    }

    public void UnInit()
    {
    }
}
