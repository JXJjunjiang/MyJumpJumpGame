using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoSingleton<UIManager>,IMgrInit
{
    private static Dictionary<UILayer, Transform> nodes;
    private static Dictionary<UILayer, Stack<UIBase>> openUIStacks;
    private Transform uiRoot;
    private static Camera uiCamera;
    private static Image blockTouchImg;
    public static bool CanTouch
    {
        get => !blockTouchImg.raycastTarget;
        set => blockTouchImg.raycastTarget = !value;
    }

    public static Camera UICamera
    {
        get => uiCamera;
        set => uiCamera = value;
    }

    public void Init()
    {
        openUIStacks = new Dictionary<UILayer, Stack<UIBase>>()
        {
            {UILayer.Bottom,new Stack<UIBase>() },
            {UILayer.Window,new Stack<UIBase>() },
            {UILayer.Pop,new Stack<UIBase>() },
            {UILayer.Top,new Stack<UIBase>() }
        };
        uiRoot = Instantiate<Transform>(Loader.LoadUI("UI").transform, transform);
        blockTouchImg = uiRoot.Find("Canvas/BlockTouchImg").GetComponent<Image>();
        blockTouchImg.raycastTarget = false;
        uiCamera = uiRoot.Find("UICamera").GetComponent<Camera>();
        nodes = new Dictionary<UILayer, Transform>()
        {
            { UILayer.Bottom,uiRoot.Find("Canvas/Bottom")},
            { UILayer.Window,uiRoot.Find("Canvas/Window")},
            { UILayer.Pop,uiRoot.Find("Canvas/Pop")},
            { UILayer.Top,uiRoot.Find("Canvas/Top")}
        };
        OpenUI<Panel_EnterGame>(UIPanel.EnterGame);
    }
    public static void OpenUI<T>(UIPanel panel) where T:UIBase
    {
        var uiInfo = DatabaseMgr.GetUIInfo(panel);
        var obj = Instantiate<GameObject>(Loader.LoadUI(uiInfo.path));
        obj.transform.SetParent(nodes[uiInfo.layer]);
        obj.GetComponent<RectTransform>().Reset();
        var uiBase = obj.RequireComponent<T>();
        uiBase.info = uiInfo;

        var openStack = openUIStacks[uiInfo.layer];
        if (openStack.Count>0&&openStack.Peek()!=null&&openStack.Peek().info.depth==uiInfo.depth)
        {
            CloseUI(uiInfo.layer);
        }

        openUIStacks[uiInfo.layer].Push(uiBase);
        uiBase.Open();
    }

    public static void CloseUI(UILayer layer)
    {
        UIBase uiBase = openUIStacks[layer].Pop();
        uiBase.Close();
    }

    public static void CloseUI(UIPanel panel)
    {
        UIInfo uiInfo = DatabaseMgr.GetUIInfo(panel);
        UIBase uiBase = openUIStacks[uiInfo.layer].Pop();
        uiBase.Close();
    }

    void OnDisable()
    {
        UnInit();
    }

    public void UnInit()
    {
        
    }
}
