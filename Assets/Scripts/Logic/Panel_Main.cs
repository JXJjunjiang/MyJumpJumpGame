using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Main : UIBase
{
    private Button startBtn, settingBtn,topicBtn, moreBtn;

    protected override void Awake()
    {
        base.Awake();
        startBtn = transform.Find("StartBtn").GetComponent<Button>();
        startBtn.AddListener(()=>
        {
            UIManager.OpenUI<Panel_Game>(UIPanel.Game);
            EventHandler.GameStart_Dispatch();
        });
        settingBtn = transform.Find("SettingBtn").GetComponent<Button>();
        settingBtn.AddListener(() =>
        {
            UIManager.OpenUI<Panel_Setting>(UIPanel.Setting);
        });
        topicBtn = transform.Find("TopicBtn").GetComponent<Button>();
        topicBtn.AddListener(() =>
        {
            UIManager.OpenUI<Panel_Topic>(UIPanel.Topic);
        });
        moreBtn = transform.Find("MoreInfoBtn").GetComponent<Button>();
        moreBtn.AddListener(() =>
        {
            UIManager.OpenUI<Panel_MoreInfo>(UIPanel.MoreInfo);
        });
    }

    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();
        DestroyImmediate(gameObject);
    }
}
