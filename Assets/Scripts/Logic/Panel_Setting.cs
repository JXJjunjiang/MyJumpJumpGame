using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Setting : UIBase
{
    private Button maskBtn, bgmBtn, gameBtn, virbrateBtn;

    protected override void Awake()
    {
        base.Awake();
        maskBtn = transform.Find("Mask").GetComponent<Button>();
        maskBtn.AddListener(() =>
        {
            UIManager.CloseUI(UIPanel.Setting);
        });
        bgmBtn = transform.Find("BGMBtn").GetComponent<Button>();
        bgmBtn.AddListener(() =>
        {
            //TODO 设置setting
        });
        gameBtn = transform.Find("GameAudioBtn").GetComponent<Button>();
        gameBtn.AddListener(() =>
        {
            //TODO 设置setting
        });
        virbrateBtn = transform.Find("VirbrateBtn").GetComponent<Button>();
        virbrateBtn.AddListener(() =>
        {
            //TODO 设置Setting
        });
    }

    public override void Close()
    {
        base.Close();
        DestroyImmediate(gameObject);
    }
}
