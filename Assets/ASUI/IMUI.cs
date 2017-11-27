using UnityEngine;
using UnityEngine.UI;

public static class IMUI
{
    public static GUIStyle fontStyle = new GUIStyle();
    public static int fontSize = 24; // reference size设计时的字体大小
    public static float screenFacter
    {
        get
        {
            return Screen.width / scaler.referenceResolution.x;
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
    public static void DrawText(string content)
    {
        DrawText(content, Vector2.zero);
    }
    public static void DrawText(string content, Vector2 pos)
    {
        fontStyle.fontSize = Mathf.RoundToInt(fontSize * screenFacter);
        Vector2 size = fontStyle.CalcSize(new GUIContent(content)); // 计算对应样式的字符尺寸  
        GUI.Label(new Rect(pos * screenFacter, size), content, fontStyle);
    }
}
