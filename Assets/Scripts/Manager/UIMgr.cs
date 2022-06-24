using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIMgr : MonoSingleton<UIMgr>,IMgrInit
{
    private static Dictionary<UILayer, Transform> nodes;
    private static Dictionary<UILayer, Stack<UIBase>> openUIStacks;
    private static Dictionary<UIPanelType, UIBase> hideUIDic;

    private Transform uiRoot;
    private Camera uiCamera;
    private Image blockTouchImg;

    public bool CanTouch
    {
        get => !blockTouchImg.raycastTarget;
        set => blockTouchImg.raycastTarget = !value;
    }

    public Camera UICamera
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
        hideUIDic = new Dictionary<UIPanelType, UIBase>();
        OpenUI<Panel_EnterGame>(UIPanelType.EnterGame);
    }
    public static void OpenUI<T>(UIPanelType panel) where T:UIBase
    {
        UIBase uiBase = null;
        UIInfo uiInfo = DatabaseMgr.Inst.GetUIInfo(panel);
        if (!hideUIDic.ContainsKey(panel))
        {
            var obj = Instantiate<GameObject>(Loader.LoadUI(uiInfo.path));
            obj.transform.SetParent(nodes[uiInfo.layer]);
            obj.GetComponent<RectTransform>().Reset();
            uiBase = obj.RequireComponent<T>();
            uiBase.info = uiInfo;

            var openStack = openUIStacks[uiInfo.layer];
            if (openStack.Count > 0 && openStack.Peek() != null && openStack.Peek().info.depth == uiInfo.depth)
            {
                CloseUI(uiInfo.layer);
            }
        }
        else
        {
            uiBase = hideUIDic[panel];
            hideUIDic.Remove(panel);
            uiBase.gameObject.SetActive(true);
        }
        openUIStacks[uiInfo.layer].Push(uiBase);
        uiBase.Open();
    }

    public static void CloseUI(UILayer layer)
    {
        UIBase uiBase = openUIStacks[layer].Pop();
        uiBase.Close();
    }

    public static void CloseUI(UIPanelType panel)
    {
        UIInfo uiInfo = DatabaseMgr.Inst.GetUIInfo(panel);
        UIBase uiBase = openUIStacks[uiInfo.layer].Pop();
        uiBase.Close();
    }

    public static void HideUI(UIPanelType panel)
    {
        UIInfo uiInfo = DatabaseMgr.Inst.GetUIInfo(panel);
        UIBase uiBase = openUIStacks[uiInfo.layer].Pop();
        uiBase.gameObject.SetActive(false);
        hideUIDic.Add(panel, uiBase);
    }


    void OnDisable()
    {
        UnInit();
    }

    public void UnInit()
    {
        
    }
}
