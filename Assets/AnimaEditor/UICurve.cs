using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICurve : MonoBehaviour
{
    private RectTransform area;
    private RectTransform ruler;
    private RectTransform rulerY;
    public int spaceInRulerX = 10; // 标尺每隔多少帧有一个帧数数字
    public int spaceInRulerY = 10; // 标尺每隔多少帧有一个帧数数字（Y轴）
    public int spaceLineInRulerX = 5; // 多少帧画一条线
    public int spaceLineInRulerY = 5;
    public float rulerScalerSensitivity = 20;
    public float rulerLength = 200;
    public float rulerYLength = 60;
    public List<Vector2> points;
    public AnimationCurve acurve;
    public ASCurve curve = new ASCurve();
    public float squareSize;
    public float lineWidth;
    public int segmentCount = 10;
    void DrawBezier(params Vector2[] p)
    {
        float t;
        Vector2 p1, p2;
        p1 = p[0];
        for (int i = 1; i <= segmentCount; i++)
        {
            t = (float)i / segmentCount;
            p2 = CalBezier(p, t);
            GLUI.DrawLine(p1, p2);
            p1 = p2;
        }
    }
    Vector2 CalBezier(Vector2[] p, float t)
    {
        Vector2 v = Vector2.zero;
        switch (p.Length)
        {
            case 4:
                v = p[0] * Mathf.Pow((1 - t), 3) +
                    3 * p[1] * t * Mathf.Pow((1 - t), 2) +
                    3 * p[2] * Mathf.Pow(t, 2) * (1 - t) +
                    p[3] * Mathf.Pow(t, 3);
                return v;
            case 3:
                v = p[0] * Mathf.Pow((1 - t), 2) +
                    2 * p[1] * t * (1 - t) +
                    p[2] * Mathf.Pow(t, 2);
                return v;
            default:
                throw null;
        }
    }
    void Start()
    {
        area = transform.Search("Area") as RectTransform;
        ruler = transform.Search("Ruler") as RectTransform;
        rulerY = transform.Search("Ruler Y") as RectTransform;
        var mouse = area.gameObject.AddComponent<UIMouseEventWrapper>();
        mouse.CreateBox2D();
        mouse.onMouseDown = MouseDown;
        mouse.onMouseDrag = MouseDrag;
        InitASUI();
    }
    private void MouseDown2()
    {
        points.Add(Input.mousePosition * IMUI.facterToReference);
        points[points.Count - 1] = points[points.Count - 1].SetY(IMUI.scaler.referenceResolution.y - points[points.Count - 1].y);
    }
    [ContextMenu("EE")]
    void EE()
    {
        var a = new ASKey(0, points[0].y);
        a.outTangent = points[1];
        var b = new ASKey(0, points[3].y);
        b.inTangent = points[2];
        curve.keys.Add(a);
        curve.keys.Add(b);
    }
    void Update()
    {
        var shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        var ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        float delta = Input.GetAxis("Mouse ScrollWheel");
        if (delta != 0)
        {
            if (ASUI.MouseOver(area, ruler))
            {
                if (shift || ctrl) rulerYLength -= delta * rulerScalerSensitivity;
                if (!shift) rulerLength -= delta * rulerScalerSensitivity;
            }
            if (ASUI.MouseOver(rulerY))
            {
                if (ctrl) rulerLength -= delta * rulerScalerSensitivity;
                rulerYLength -= delta * rulerScalerSensitivity;
            }
        }
        ASUI.owner = this;
        GLUI.BeginOrtho();
        UpdateASUI();
        GLUI.BeginOrder(1);
        DrawBezier(points.ToArray());
        for (int i = 0; i < curve.keys.Count; i++)
        {

        }
        foreach (var p in points)
        {
            GLUI.DrawSquare(p, squareSize);
            GLUI.DrawSquare(p, squareSize, lineWidth);
        }
        GLUI.EndOrder();
    }
    private void MouseDown()
    {
        MouseDrag();
    }
    private void MouseDrag()
    {
        var lx = Input.mousePosition.x * IMUI.facterToReference - area.anchoredPosition.x;
        lx = lx / area.sizeDelta.x;
        lx = Mathf.Clamp01(lx);
        UITimeLine.FrameIndex = Mathf.RoundToInt(lx * rulerLength);
    }
    private void InitASUI()
    {
        ASUI.parent = area;
    }
    void UpdateASUI()
    {
        float x, y, deltaX, deltaY;
        Vector2 p;
        for (int i = 0; i < 1000; i++)
        {
            deltaY = i / rulerYLength;
            if ((i % spaceInRulerY) == 0)
            {
                y = deltaY * rulerY.sizeDelta.y;
                if (y + IMUI.CalSize(i.ToString()).y > rulerY.sizeDelta.y) break;
                x = rulerY.anchoredPosition.x;
                y = -y - rulerY.anchoredPosition.y + rulerY.sizeDelta.y;
                p = new Vector2(x, y);
                IMUI.DrawText(i.ToString(), p, Vector2.one * 0.5f);
                x = area.anchoredPosition.x;
                p = new Vector2(x, y);
                GLUI.DrawLine(p, p + Vector2.right * area.sizeDelta.x, Color.grey - ASColor.V * 0.3f);
            }
            else if ((i % spaceLineInRulerY) == 0)
            {
                y = deltaY * area.sizeDelta.y;
                if (y > area.sizeDelta.y) break;
                y = -y - area.anchoredPosition.y + rulerY.sizeDelta.y;
                x = area.anchoredPosition.x;
                p = new Vector2(x, y);
                GLUI.DrawLine(p, p + Vector2.right * area.sizeDelta.x, Color.grey - ASColor.V * 0.2f);
            }
        }
        for (int i = 0; i < 1000; i++)
        {
            deltaX = i / rulerLength;
            if ((i % spaceInRulerX) == 0)
            {
                x = deltaX * ruler.sizeDelta.x;
                if (x + IMUI.CalSize(i.ToString()).x > ruler.sizeDelta.x) break;
                x += ruler.anchoredPosition.x;
                y = -ruler.anchoredPosition.y;
                p = new Vector2(x, y);
                IMUI.DrawText(i.ToString(), p, Vector2.one * 0.5f);
                y = -area.anchoredPosition.y;
                p = new Vector2(x, y);
                GLUI.DrawLine(p, p + Vector2.up * area.sizeDelta.y, Color.grey - ASColor.V * 0.3f);
            }
            else if ((i % spaceLineInRulerX) == 0)
            {
                x = deltaX * area.sizeDelta.x;
                if (x > area.sizeDelta.x) break;
                x += area.anchoredPosition.x;
                y = -area.anchoredPosition.y;
                p = new Vector2(x, y);
                GLUI.DrawLine(p, p + Vector2.up * area.sizeDelta.y, Color.grey - ASColor.V * 0.2f);
            }
            if (UITimeLine.Clip.HasKey(UITimeLine.ObjCurve, i))
            {
                x = deltaX * area.sizeDelta.x;
                if (x > area.sizeDelta.x) break;
                x += area.anchoredPosition.x;
                y = -area.anchoredPosition.y;
                p = new Vector2(x, y);
                GLUI.commandOrder = 2;
                GLUI.DrawLine(p, p + Vector2.up * area.sizeDelta.y, lineWidth * 0.45f, Color.yellow - ASColor.V * 0.2f);
            }
        }
        deltaX = UITimeLine.FrameIndex / rulerLength;
        x = deltaX * area.sizeDelta.x;
        if (x > area.sizeDelta.x) return;
        x += area.anchoredPosition.x;
        y = -area.anchoredPosition.y;
        p = new Vector2(x, y);
        GLUI.commandOrder = 1;
        GLUI.DrawLine(p, p + Vector2.up * area.sizeDelta.y, lineWidth, Color.green + ASColor.H * 0.2f);
    }
}
