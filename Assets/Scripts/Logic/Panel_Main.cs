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
            UIMgr.OpenUI<Panel_Game>(UIPanelType.Game);
            GameMgr.Inst.GameStart();
        });
        settingBtn = transform.Find("SettingBtn").GetComponent<Button>();
        settingBtn.AddListener(() =>
        {
            UIMgr.OpenUI<Panel_Setting>(UIPanelType.Setting);
        });
        topicBtn = transform.Find("TopicBtn").GetComponent<Button>();
        topicBtn.AddListener(() =>
        {
            UIMgr.OpenUI<Panel_Topic>(UIPanelType.Topic);
        });
        moreBtn = transform.Find("MoreInfoBtn").GetComponent<Button>();
        moreBtn.AddListener(() =>
        {
            UIMgr.OpenUI<Panel_MoreInfo>(UIPanelType.MoreInfo);
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
