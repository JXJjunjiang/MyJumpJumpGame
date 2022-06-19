using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseMgr : MonoSingleton<DatabaseMgr>,IMgrInit
{
    private static Dictionary<UIPanel, UIInfo> uiDatas;
    private static Dictionary<int, string> heightInfo;
    private static List<int> characterIds;
    private static List<int> enviromentIds;
    private const string Key_PlatformPos = "PlatformPos";
    private const string Key_Score = "Score";
    private const string Key_PlayerPos = "PlayerPos";

    private static int _score;
    public static int Score
    {
        get => _score;
        set => _score = value;
    }

    private static int characterId;
    public static int CharacterID
    {
        get => characterId;
        set => characterId = value;
    }

    private static int enviromentId;
    public static int EnviromentID
    {
        get => enviromentId;
        set => enviromentId = value;
    }

    public void Init()
    {
        uiDatas = new Dictionary<UIPanel, UIInfo>()
        {
            { UIPanel.EnterGame,new UIInfo(UIPanel.EnterGame,UILayer.Top,1,"EnterGame")},
            { UIPanel.Main,new UIInfo(UIPanel.Main,UILayer.Bottom,1,"Main")},
            { UIPanel.MoreInfo,new UIInfo(UIPanel.MoreInfo,UILayer.Bottom,2,"MoreInfo") },
            { UIPanel.Setting,new UIInfo(UIPanel.Setting,UILayer.Bottom,2,"Setting") },
            { UIPanel.Topic,new UIInfo(UIPanel.Topic,UILayer.Bottom,2,"Topic") },
            { UIPanel.Game,new UIInfo(UIPanel.Game,UILayer.Bottom,1,"GameUI")},
            { UIPanel.Fail,new UIInfo(UIPanel.Fail,UILayer.Pop,1,"GameFail") },
            { UIPanel.HeightTips,new UIInfo(UIPanel.HeightTips,UILayer.Window,1,"HeightTips") }
        };
        heightInfo = new Dictionary<int, string>()
        {
            {5,"咕咕咕咕咕咕过ID黑丝哦积分new我军费能狂怒哇hi偶尔" },
            {10,"大大1玩法的时触发舒服 彩色电视 2我第三册ad啊" },
            {20,"冯绍峰顺丰到付身上的防守打法是法师法师法师法师发全是爱舒服舒服" },
            {25,"大大的43额的房产税打确定第三方方式发舒服 氛围房地产撒大声地" },
            {30,"大事反身代词小反身代词吸附石的操作想问问发从上到下这是大V从行为发生的初学者" },
            {40,"UI好久看不那么UI好几百年没回家不看美女会加快不那么，火箭难看吗，窘困，吗" },
            {50,"0破解卡利玛你， 优惠近半年没会加快不那么回家不那么8亿会尽可能买黄金看你们我， " },
            {70," 如果他不是份小吃金科没， 过滤法从自己快乐柠檬，地方vcoklm,.方便查viojklm,。托付给不草佩可莉姆，。 " },
            {80,"对焊看美味的，wesdcx wefsdcx c发Greg v地方小吃而奋斗v支持杏仁粉v第三次写入放大v从写入放大v从地方v现场" }
        };
        characterIds = new List<int>() { 0, 1, 2, 3, 4 };
        enviromentIds = new List<int>() { 0, 1, 2, 3, 4 };
        ReadData();
    }

    public static string GetHeightLabel()
    {
        return heightInfo[_score];
    }

    public static bool IsMatchAnyHeight()
    {
        return heightInfo.ContainsKey(_score);
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

    public List<int> GetCharacters()
    {
        return characterIds;
    }

    public List<int>GetEnviroments()
    {
        return enviromentIds;
    }

    private void ReadData()
    {
        _score = PlayerPrefs.GetInt(Key_Score);
    }
    private void WriteData()
    {
        PlayerPrefs.SetInt(Key_Score, _score);
    }

    Vector3 String2Vector(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return Vector3.zero;
        }
        Vector3 result = Vector3.zero;
        try{
            string[] strs = str.Split(',');
            result.Set(float.Parse(strs[0]), float.Parse(strs[1]), float.Parse(strs[1]));
        }
        catch{
            Debug.LogErrorFormat("[{0}] can not convert to vector3", str);
        }
        return result;
    }

    string Vector2String(Vector3 vector)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        result.Append(vector.x+",");
        result.Append(vector.y + ",");
        result.Append(vector.z);
        return result.ToString();
    }

    public void UnInit()
    {
        WriteData();
    }
}
