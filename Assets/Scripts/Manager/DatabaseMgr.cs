using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseMgr : MonoSingleton<DatabaseMgr>,IMgrInit
{
    private const string Key_CharacterID = "CharacterID";
    private const string Key_Height = "Height";
    private const string Key_EnvironmentID = "EnvironmentID";
    private const string Key_GameAudioEnable = "GameAudioEnable";
    private const string Key_GameVirbrateEnable = "GameVirbrateEnable";

    private int height;
    private int characterId;
    private int environmentId;
    private int gameAudioEnable;
    private int gameVirbrateEnable;

    private Dictionary<UIPanelType, UIInfo> uiDatas;
    private Dictionary<int, string> heightInfo;
    private List<int> characterIds;
    private List<int> enviromentIds;

    public int Height
    {
        get => height;
        set => height = value;
    }

    public int CharacterID
    {
        get => characterId;
        set => characterId = value;
    }

    public int EnvironmentID
    {
        get => environmentId;
        set => environmentId = value;
    }

    public bool GameAudioEnable
    {
        get => gameAudioEnable == 1;
        set => gameAudioEnable = value ? 1 : 0;
    }

    public bool GameVirbrateEnable
    {
        get => gameVirbrateEnable == 1;
        set => gameVirbrateEnable = value ? 1 : 0;
    }

    public void Init()
    {
        uiDatas = new Dictionary<UIPanelType, UIInfo>()
        {
            { UIPanelType.EnterGame,new UIInfo(UIPanelType.EnterGame,UILayer.Top,1,"EnterGame")},
            { UIPanelType.Main,new UIInfo(UIPanelType.Main,UILayer.Bottom,1,"Main")},
            { UIPanelType.MoreInfo,new UIInfo(UIPanelType.MoreInfo,UILayer.Bottom,2,"MoreInfo") },
            { UIPanelType.Setting,new UIInfo(UIPanelType.Setting,UILayer.Bottom,2,"Setting") },
            { UIPanelType.Topic,new UIInfo(UIPanelType.Topic,UILayer.Bottom,2,"Topic") },
            { UIPanelType.Game,new UIInfo(UIPanelType.Game,UILayer.Bottom,1,"GameUI")},
            { UIPanelType.Fail,new UIInfo(UIPanelType.Fail,UILayer.Pop,1,"GameFail") },
            { UIPanelType.HeightTips,new UIInfo(UIPanelType.HeightTips,UILayer.Window,1,"HeightTips") }
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

    public string GetHeightLabel()
    {
        return heightInfo[height];
    }

    public bool IsMatchAnyHeight()
    {
        return heightInfo.ContainsKey(height);
    }

    public UIInfo GetUIInfo(UIPanelType panel)
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

    public List<int>GetEnvironments()
    {
        return enviromentIds;
    }

    private void ReadData()
    {
        height = PlayerPrefs.HasKey(Key_Height) ? PlayerPrefs.GetInt(Key_Height) : 0;
        characterId = PlayerPrefs.HasKey(Key_CharacterID) ? PlayerPrefs.GetInt(Key_CharacterID) : 0;
        environmentId = PlayerPrefs.HasKey(Key_EnvironmentID) ? PlayerPrefs.GetInt(Key_EnvironmentID) : 0;
        gameAudioEnable = PlayerPrefs.HasKey(Key_GameAudioEnable) ? PlayerPrefs.GetInt(Key_GameAudioEnable) : 1;
        gameVirbrateEnable = PlayerPrefs.HasKey(Key_GameVirbrateEnable) ? PlayerPrefs.GetInt(Key_GameVirbrateEnable) : 1;
    }
    private void WriteData()
    {
        PlayerPrefs.SetInt(Key_Height, height);
        PlayerPrefs.SetInt(Key_CharacterID, characterId);
        PlayerPrefs.SetInt(Key_EnvironmentID, environmentId);
        PlayerPrefs.SetInt(Key_GameAudioEnable, gameAudioEnable);
        PlayerPrefs.SetInt(Key_GameVirbrateEnable, gameVirbrateEnable);
    }

    public void UnInit()
    {
        WriteData();
    }
}
