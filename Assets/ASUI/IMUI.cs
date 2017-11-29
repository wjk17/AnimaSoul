﻿using UnityEngine;
using UnityEngine.UI;

public static class IMUI
{
    public static GUIStyle fontStyle = new GUIStyle();
    public static int fontSize = 24; // reference size设计时的字体大小
    public static float facterToRealPixel
    {
        get
        {
            return Screen.width / scaler.referenceResolution.x;
        }
    }
    public static float facterToReference
    {
        get
        {
            return scaler.referenceResolution.x / Screen.width;
        }
    }
    private static CanvasScaler _scaler;
    public static CanvasScaler scaler
    {
        get
        {
            if (_scaler == null) _scaler = Object.FindObjectOfType<CanvasScaler>();
            return _scaler;
        }
        set
        {
            _scaler = value;
        }
    }


    public static IMUICommand Cmd(IMUICmdType type, params object[] args)
    {
        var cmd = new IMUICommand();
        cmd.type = type;
        cmd.args = args;
        return cmd;
    }
    public static void DrawText(string content, Vector2 pos, Vector2 pivot)
    {
        ASUI.I.AddCommand(Cmd(IMUICmdType.DrawText, content, pos, pivot));
    }
    public static void DrawText(string content, Vector2 pos)
    {
        ASUI.I.AddCommand(Cmd(IMUICmdType.DrawText, content, pos));
    }
    public static Vector2 CalSize(string content)
    {
        var n = new GUIStyle(fontStyle);
        n.fontSize = fontSize; // 设计时的大小
        Vector2 size = n.CalcSize(new GUIContent(content));
        return size;
    }
    public static void DrawTextIM(string content, Vector2 pos)
    {
        fontStyle.fontSize = Mathf.RoundToInt(fontSize * facterToRealPixel);
        Vector2 size = fontStyle.CalcSize(new GUIContent(content)); // 计算对应样式的字符尺寸  
        GUI.Label(new Rect(pos * facterToRealPixel, size), content, fontStyle);
    }
    public static void DrawTextIM(string content, Vector2 pos, Vector2 pivot)
    {
        fontStyle.fontSize = Mathf.RoundToInt(fontSize * facterToRealPixel);
        Vector2 size = fontStyle.CalcSize(new GUIContent(content));
        GUI.Label(new Rect(pos * facterToRealPixel - Vector2.Scale(size, pivot), size), content, fontStyle);
    }
}
