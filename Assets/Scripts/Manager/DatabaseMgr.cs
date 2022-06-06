using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIPanel
{
    Main=1,
    Setting,
    MoreInfo,
    EnterGame,
    ExitGame,
    Game,
    Account
}
public enum UILayer
{
    Bottom=1,
    Window=2,
    Pop=3,
    Top=4
}
public enum UIAnim
{
    None=1,
    Fade=2,
    Scale=3,
    Pos=4
}
public class DatabaseMgr : MonoSingleton<DatabaseMgr>,IMgrInit
{
    private static Dictionary<UIPanel, UIInfo> uiDatas;
    protected override void Awake()
    {
        base.Awake();
        Init();
    }


    public void Init()
    {
        uiDatas = new Dictionary<UIPanel, UIInfo>()
        {
            {UIPanel.EnterGame,new UIInfo(UIPanel.EnterGame,UILayer.Top,1,"EnterGame")},
            {UIPanel.Main,new UIInfo(UIPanel.Main,UILayer.Bottom,1,"Main")},
            {UIPanel.MoreInfo,new UIInfo(UIPanel.MoreInfo,UILayer.Bottom,2,"MoreInfo") },
            {UIPanel.Setting,new UIInfo(UIPanel.Setting,UILayer.Bottom,2,"Setting") }
        };
    }

    public static UIInfo GetUIInfo(UIPanel panel)
    {
        if (!uiDatas.ContainsKey(panel))
        {
            Debug.LogError("ui data does not contains panel:" + panel);
            return null;
        }
        return uiDatas[panel];
    }
    void OnDisable()
    {
        UnInit();
    }
    public void UnInit()
    {
    }
}
