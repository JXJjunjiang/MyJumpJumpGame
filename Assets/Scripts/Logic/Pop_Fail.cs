﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pop_Fail : UIBase
{
    private Text showText;
    private Button admitBtn;
    private const string label = "最高分:{0}";
    protected override void Awake()
    {
        base.Awake();
        showText = transform.Find("Label").RequireComponent<Text>();
        admitBtn = transform.Find("AdmitBtn").RequireComponent<Button>();
        admitBtn.AddListener(() =>
        {
            UIManager.CloseUI(UIPanel.Fail);
            UIManager.CloseUI(UIPanel.Game);
            UIManager.OpenUI<Panel_Main>(UIPanel.Main);
            GameManager.Inst.GameExit();
        });
    }

    public override void Open()
    {
        base.Open();
        showText.text = string.Format(label, DatabaseMgr.Score);
        DatabaseMgr.Score = 0;
    }

    public override void Close()
    {
        base.Close();
        DestroyImmediate(gameObject);
    }
}
