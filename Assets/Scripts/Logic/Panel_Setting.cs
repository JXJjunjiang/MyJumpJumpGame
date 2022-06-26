using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Setting : UIBase
{
    private Button maskBtn = null;
    private Transform gameBtn = null, virbrateBtn = null;

    protected override void Awake()
    {
        base.Awake();
        maskBtn = transform.Find("Mask").GetComponent<Button>();
        maskBtn.AddListener(() =>
        {
            UIMgr.CloseUI(UIPanelType.Setting);
        });
        gameBtn = transform.Find("GameAudioBtn");
        SetGameAudioIcon();
        gameBtn.GetComponent<Button>().AddListener(() =>
        {
            DatabaseMgr.Inst.GameAudioEnable = !DatabaseMgr.Inst.GameAudioEnable;
            SetGameAudioIcon();
        });
        virbrateBtn = transform.Find("VirbrateBtn");
        SetVirbrateIcon();
        virbrateBtn.GetComponent<Button>().AddListener(() =>
        {
            DatabaseMgr.Inst.GameVirbrateEnable = !DatabaseMgr.Inst.GameVirbrateEnable;
            SetVirbrateIcon();
        });
    }

    void SetGameAudioIcon()
    {
        string iconName = DatabaseMgr.Inst.GameAudioEnable ? "SoundActive" : "SoundDeactive";
        gameBtn.GetComponent<Image>().sprite = Loader.LoadSprite(iconName);
    }

    void SetVirbrateIcon()
    {
        string iconName = DatabaseMgr.Inst.GameVirbrateEnable ? "VibActive" : "VibDeactive";
        virbrateBtn.GetComponent<Image>().sprite = Loader.LoadSprite(iconName);
    }

    public override void Close()
    {
        base.Close();
        DestroyImmediate(gameObject);
    }
}
