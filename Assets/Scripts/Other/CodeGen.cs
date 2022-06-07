using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class CodeGen
{
    private static string SettingPath = @"EventSetting";
    private static EventHandlerSetting SettingObj;
    private static string CodePath = @"Assets\Scripts\Other\";
    private static string CodeName = "EventHandler.cs";
    private static string LineFeed = "\r\n";
    private static string LineTable = "\t";


    [MenuItem("Tools/EventCodeGen")]
    public static void CreateEventCode()
    {
        if (File.Exists(CodePath + CodeName))
        {
            File.Delete(CodePath + CodeName);
        }
        File.Create(CodePath + CodeName).Dispose();
        LoadSetting();

        File.AppendAllText(CodePath + CodeName, CreateNamespace());
        File.AppendAllText(CodePath + CodeName, CreateDelegate());
        File.AppendAllText(CodePath + CodeName, CreateClass());

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 加载配置文件
    /// </summary>
    static void LoadSetting()
    {
        SettingObj = Resources.Load<EventHandlerSetting>(SettingPath);
    }

    static string CreateNamespace()
    {
        StringBuilder result = new StringBuilder();
        result.Append("using System;" + LineFeed);
        result.Append("using System.Collections;" + LineFeed);
        result.Append("using System.Collections.Generic;" + LineFeed);
        result.Append("using UnityEngine;" + LineFeed + LineFeed);
        return result.ToString();
    }

    /// <summary>
    /// 创建委托类型
    /// </summary>
    /// <returns></returns>
    static string CreateDelegate()
    {
        StringBuilder result = new StringBuilder();
        string commnPrefix = "public delegate void ";
        using (var e = SettingObj.types.GetEnumerator())
        {
            while (e.MoveNext())
            {
                result.Append(commnPrefix + e.Current.typeDelegate + ";" + LineFeed);
            }
        }
        result.Append(LineFeed);
        return result.ToString();
    }

    /// <summary>
    /// 创建EventHandler类
    /// </summary>
    /// <returns></returns>
    static string CreateClass()
    {
        StringBuilder result = new StringBuilder();
        result.Append("public class EventHandler" + LineFeed);
        result.Append("{" + LineFeed);
        List<EventItem> ls = SettingObj.items;
        for (int i = 0; i < ls.Count; i++)
        {
            string eventName = ls[i].eventName;
            string eventType = ls[i].typeName;
            result.Append(CreateEvent(eventName, eventType));
        }
        result.Append("}");
        return result.ToString();
    }

    /// <summary>
    /// 创建事件派发器
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="eventType"></param>
    /// <returns></returns>
    static string CreateEvent(string eventName, string eventType)
    {
        StringBuilder result = new StringBuilder();
        result.Append(LineFeed + "#region " + eventName + LineFeed);
        result.Append(MutiLineTable(1) + string.Format("private static List<{0}> {1}_List;", eventType + "Delegate", eventName) + LineFeed);//创建List
        result.Append(CreateListener(eventName, eventType));
        result.Append(CreateDispatch(eventName, eventType));
        result.Append("#endregion" + LineFeed);
        return result.ToString();
    }

    /// <summary>
    /// 创建Listener
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="eventType"></param>
    /// <returns></returns>
    static string CreateListener(string eventName, string eventType)
    {
        StringBuilder result = new StringBuilder();
        result.Append(MutiLineTable(1) + string.Format("public static event {0} {1}_Listener", eventType + "Delegate", eventName) + LineFeed);
        result.Append(MutiLineTable(1) + "{" + LineFeed);
        #region add
        result.Append(MutiLineTable(2) + "add" + LineFeed);
        result.Append(MutiLineTable(2) + "{" + LineFeed);
        result.Append(MutiLineTable(3) + "if(value != null)" + LineFeed);
        result.Append(MutiLineTable(3) + "{" + LineFeed);
        result.Append(MutiLineTable(4) + string.Format("if({0}_List == null)", eventName) + LineFeed);
        result.Append(MutiLineTable(4) + "{" + LineFeed);
        result.Append(MutiLineTable(5) + string.Format("{0}_List = new List<{1}>(1);", eventName, eventType + "Delegate") + LineFeed);
        result.Append(MutiLineTable(4) + "}" + LineFeed);
        result.Append(MutiLineTable(4) + string.Format("{0}_List.Add(value);", eventName) + LineFeed);
        result.Append(MutiLineTable(3) + "}" + LineFeed);
        result.Append(MutiLineTable(2) + "}" + LineFeed);
        #endregion
        #region remove
        result.Append(MutiLineTable(2) + "remove" + LineFeed);
        result.Append(MutiLineTable(2) + "{" + LineFeed);
        result.Append(MutiLineTable(3) + "if(value != null)" + LineFeed);
        result.Append(MutiLineTable(3) + "{" + LineFeed);
        result.Append(MutiLineTable(4) + string.Format("for(int i = 0;i<{0}_List.Count;i++)", eventName) + LineFeed);
        result.Append(MutiLineTable(4) + "{" + LineFeed);
        result.Append(MutiLineTable(5) + string.Format("if({0}_List[i] != null && {1}_List[i].Equals(value))", eventName, eventName) + LineFeed);
        result.Append(MutiLineTable(5) + "{" + LineFeed);
        result.Append(MutiLineTable(6) + string.Format("{0}_List.RemoveAt(i);", eventName) + LineFeed);
        result.Append(MutiLineTable(6) + "break;" + LineFeed);
        result.Append(MutiLineTable(5) + "}" + LineFeed);
        result.Append(MutiLineTable(4) + "}" + LineFeed);
        result.Append(MutiLineTable(3) + "}" + LineFeed);
        result.Append(MutiLineTable(2) + "}" + LineFeed);
        #endregion
        result.Append(MutiLineTable(1) + "}" + LineFeed);
        return result.ToString();
    }

    /// <summary>
    /// 创建Dispatch
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="eventType"></param>
    /// <returns></returns>
    static string CreateDispatch(string eventName, string eventType)
    {
        StringBuilder result = new StringBuilder();
        result.Append(MutiLineTable(1) + string.Format("public static void {0}_Dispatch(params {1})", eventName, GetTypeParameter(eventType)) + LineFeed);
        result.Append(MutiLineTable(1) + "{" + LineFeed);
        result.Append(MutiLineTable(2) + string.Format("if({0}_List == null || {1}_List.Count <= 0)", eventName, eventName) + LineFeed);
        result.Append(MutiLineTable(2) + "{" + LineFeed);
        result.Append(MutiLineTable(3) + "return;" + LineFeed);
        result.Append(MutiLineTable(2) + "}" + LineFeed);
        result.Append(MutiLineTable(2) + string.Format("for(int i = 0;i < {0}_List.Count;i++)", eventName) + LineFeed);
        result.Append(MutiLineTable(2) + "{" + LineFeed);
        if (!string.IsNullOrEmpty(GetTypeParameter(eventType)))
        {
            result.Append(MutiLineTable(3) + string.Format("{0}_List[i]?.Invoke(parameters[i]);", eventName) + LineFeed);
        }
        else
        {
            result.Append(MutiLineTable(3) + string.Format("{0}_List[i]?.Invoke();", eventName) + LineFeed);
        }
        result.Append(MutiLineTable(2) + "}" + LineFeed);
        result.Append(MutiLineTable(1) + "}" + LineFeed);
        return result.ToString();
    }

    /// <summary>
    /// 获取字符串中括号中的内容
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    static string GetTypeParameter(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return string.Empty;
        }
        using (var e = SettingObj.types.GetEnumerator())
        {
            while (e.MoveNext())
            {
                if (e.Current.typeName == typeName)
                {
                    string @delegate = e.Current.typeDelegate;
                    string result = @delegate.Substring(@delegate.IndexOf("(") + 1, @delegate.IndexOf(")") - (@delegate.IndexOf("(") + 1));
                    string[] strs = result.Split(' ');
                    strs[0] += "[]";
                    result = strs[0] + " " + strs[1];
                    return result;
                }
            }
        }
        return string.Empty;
    }

    /// <summary>
    /// 多个table
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    static string MutiLineTable(int count)
    {
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < count; i++)
        {
            result.Append(LineTable);
        }
        return result.ToString();
    }
}
