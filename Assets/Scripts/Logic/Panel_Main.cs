using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Main : UIBase
{
    private Button startBtn, settingBtn, moreBtn;

    protected override void Awake()
    {
        base.Awake();
        startBtn = transform.Find("StartBtn").GetComponent<Button>();
        startBtn.AddListener(()=>
        {
            //TODO 关闭主界面窗口
            UIManager.CloseUI(info.layer);
            //唤起游戏窗口
            UIManager.OpenUI<Panel_Game>(UIPanel.Game);
            //EnterGame
            EventDispatcher.Dispatch("StartGame", null);

        });
        settingBtn = transform.Find("SettingBtn").GetComponent<Button>();
        settingBtn.AddListener(() =>
        {
            //TODO 唤起设置窗口
            UIManager.OpenUI<Panel_Setting>(UIPanel.Setting);
        });
        moreBtn = transform.Find("MoreInfoBtn").GetComponent<Button>();
        moreBtn.AddListener(() =>
        {
            //TODO 唤起信息窗口
            UIManager.OpenUI<Panel_MoreInfo>(UIPanel.MoreInfo);
        });
    }
}
