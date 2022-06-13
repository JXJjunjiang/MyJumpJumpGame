using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseMgr : MonoSingleton<DatabaseMgr>,IMgrInit
{
    private static Dictionary<UIPanel, UIInfo> uiDatas;
    private const string Key_PlatformPos = "PlatformPos";
    private const string Key_Score = "Score";
    private const string Key_PlayerPos = "PlayerPos";

    private static int _score;
    public static int Score
    {
        get => _score;
        set => _score = value;
    }

    private static Vector3 _platformPos;
    public static Vector3 curPlatformPos
    {
        get => _platformPos;
        set => _platformPos = value;
    }

    private static Vector3 _playerPos;
    public static Vector3 PlayerPos
    {
        get => _playerPos;
        set => _playerPos = value;
    }

    public void Init()
    {
        uiDatas = new Dictionary<UIPanel, UIInfo>()
        {
            { UIPanel.EnterGame,new UIInfo(UIPanel.EnterGame,UILayer.Top,1,"EnterGame")},
            { UIPanel.Main,new UIInfo(UIPanel.Main,UILayer.Bottom,1,"Main")},
            { UIPanel.MoreInfo,new UIInfo(UIPanel.MoreInfo,UILayer.Bottom,2,"MoreInfo") },
            { UIPanel.Setting,new UIInfo(UIPanel.Setting,UILayer.Bottom,2,"Setting") },
            { UIPanel.Game,new UIInfo(UIPanel.Game,UILayer.Bottom,1,"GameUI")},
            { UIPanel.Fail,new UIInfo(UIPanel.Fail,UILayer.Pop,1,"GameFail") },
            { UIPanel.HeightTips,new UIInfo(UIPanel.HeightTips,UILayer.Window,1,"HeightTips") }
        };
        ReadData();
    }

    public static string GetHeightLabel()
    {
        return string.Empty;
    }

    public static bool IsMatchAnyHeight()
    {
        return false;
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

    private void ReadData()
    {
        _score = PlayerPrefs.GetInt(Key_Score);
        #region platform
        string readPlatform = string.Empty;
        string[] platStrs = new string[3];
        Vector3 platPos = Vector3.zero;
        try{
            readPlatform = PlayerPrefs.GetString(Key_PlatformPos);
            platStrs = readPlatform.Split(',');
            platPos = new Vector3(float.Parse(platStrs[0]), float.Parse(platStrs[1]), float.Parse(platStrs[1]));
        }
        catch{
            Debug.LogError("init fail,because read data can not convert to float");
            return;
        }
        _platformPos = platPos;
        #endregion
        #region player
        string readPlayer = string.Empty;
        string[] playerStrs = new string[3];
        Vector3 playerPos = Vector3.zero;
        try{
            readPlayer = PlayerPrefs.GetString(Key_PlayerPos);
            playerStrs = readPlatform.Split(',');
            playerPos = new Vector3(float.Parse(platStrs[0]), float.Parse(platStrs[1]), float.Parse(platStrs[1]));
        }
        catch{
            Debug.LogError("init fail,because read data can not convert to float");
            return;
        }
        _playerPos = platPos;
        #endregion
    }
    private void WriteData()
    {

    }
    public void UnInit()
    {
        WriteData();
    }
}
