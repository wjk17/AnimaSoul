using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOCList : MonoBehaviour
{
    RectTransform area;
    public RectTransform a;
    public RectTransform b;
    void Start()
    {
        area = transform.Search("Area") as RectTransform;
    }
    public Vector2 p, p2;
    public float width = 20f;
    void Update()
    {
        ASUI.owner = area;
        GLUI.BeginOrtho();
        p = MathTool.ReverseY(area.anchoredPosition);
        int i = 0;
        for (; i < UITimeLine.Clip.curves.Count; i++)
        {
            var c = UITimeLine.Clip.curves[i];
            var n = c.trans.name.ToString();
            IMUI.DrawText(n, p);
            p += Vector2.up * IMUI.CalSize(n).y;
            if (p.y > -area.anchoredPosition.y + area.rect.height) break;
        }
        p = MathTool.ReverseY(area.anchoredPosition);
        p.x += area.rect.width * 0.5f;
        var showN = (float)i / UITimeLine.Clip.curves.Count;
        p2 = p + Vector2.up * showN * area.rect.height;
        GLUI.DrawLine(p, p2, width, Color.black);
        GLUI.DrawLine(new Vector2(), new Vector2(1600, 900), width, Color.black);

    }
}
