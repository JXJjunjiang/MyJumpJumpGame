using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Setting : UIBase
{
    private Button maskBtn = null;
    private Transform gameBtn = null, virbrateBtn = null;
    private AudioSetting audioSet = null;

    protected override void Awake()
    {
        base.Awake();
        audioSet = Loader.LoadAudioSetting();
        maskBtn = transform.Find("Mask").GetComponent<Button>();
        maskBtn.AddListener(() =>
        {
            UIMgr.CloseUI(UIPanelType.Setting);
        });
        gameBtn = transform.Find("GameAudioBtn");
        SetGameAudioIcon();
        gameBtn.GetComponent<Button>().AddListener(() =>
        {
            audioSet.EnableGameMusic = !audioSet.EnableGameMusic;
            SetGameAudioIcon();
        });
        virbrateBtn = transform.Find("VirbrateBtn");
        SetVirbrateIcon();
        virbrateBtn.GetComponent<Button>().AddListener(() =>
        {
            audioSet.EnableVibrate = !audioSet.EnableVibrate;
            SetVirbrateIcon();
        });
    }

    void SetGameAudioIcon()
    {
        string iconName = audioSet.EnableGameMusic ? "SoundActive" : "SoundDeactive";
        gameBtn.GetComponent<Image>().sprite = Loader.LoadSprite(iconName);
    }

    void SetVirbrateIcon()
    {
        string iconName = audioSet.EnableVibrate ? "VibActive" : "VibDeactive";
        virbrateBtn.GetComponent<Image>().sprite = Loader.LoadSprite(iconName);
    }

    public override void Close()
    {
        base.Close();
        DestroyImmediate(gameObject);
    }
}
