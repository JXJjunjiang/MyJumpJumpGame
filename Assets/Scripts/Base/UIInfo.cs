using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInfo
{
    public UIPanelType panel;
    public UILayer layer;
    public int depth;
    public string path;

    public UIInfo(UIPanelType panel, UILayer layer, int depth, string path)
    {
        this.panel = panel;
        this.layer = layer;
        this.depth = depth;
        this.path = path;
    }
}
