using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMgr : MonoSingleton<AudioMgr>,IMgrInit
{
    private const string buttonMusic = "Button";
    
    private Dictionary<string, GameObject> audioItems;
    private AudioSetting gameAudio;

    public void Init()
    {
        audioItems = new Dictionary<string, GameObject>();
        gameAudio = Loader.LoadAudioSetting();
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
        StopAudio(buttonMusic);
    }

    public void PlayAudio(string audioName,bool isAwake,bool isLoop,string tag="")
    {
        AudioSource audioSource = null;
        if (audioItems.ContainsKey(audioName+tag))
        {
            audioItems[audioName + tag].SetActive(true);
            audioSource = audioItems[audioName + tag].RequireComponent<AudioSource>();
        }
        else
        {
            audioSource = CreateAudioItem(audioName + tag);
        }
        audioSource.playOnAwake = isAwake;
        audioSource.loop = isLoop;
        audioSource.clip = Loader.LoadAudio(audioName);
        audioSource.Play();
    }

    public void StopAudio(string audioName,string tag="")
    {
        if (!audioItems.ContainsKey(audioName + tag))
        {
            Debug.LogWarning("The target Audio does not exsit in cache");
            return;
        }
        audioItems[audioName + tag].SetActive(false);
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

    public void UnInit()
    {
    }
}
