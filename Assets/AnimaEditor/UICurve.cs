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
    public Vector2 rulerLength = new Vector2(200, 60);
    public Vector2 range
    {
        get { return endPos - startPos; }
    }
    public Vector2 endPos
    {
        get { return startPos + rulerLength; }
    }
    private Vector2 startPos;
    public Vector2Int startPosInt
    {
        get { return new Vector2Int(Mathf.RoundToInt(startPos.x), Mathf.RoundToInt(startPos.y)); }
    }
    public List<Vector2> points;
    public AnimationCurve acurve;
    public ASCurve curve = new ASCurve();
    public List<ASKey> keys { get { return curve.keys; } }
    public float squareSize;
    public float lineWidth;
    public int segmentCount = 10;

    bool middle;
    Vector2 oldPos;
    public float moveSensitivity = 0.3f;
    public Color keyPointColor = Color.green;
    public Color curveColor = Color.green - ASColor.V * 0.5f;
    public Color controlPointColor = Color.grey + ASColor.V * 0.3f;
    public Color controlLineColor = Color.grey;

    Vector2 ConvertV(Vector2 v)
    {
        var d = v - startPos;
        var n = MathTool.Divide(d, range);
        var x = n.x * area.sizeDelta.x + area.anchoredPosition.x;
        var y = -n.y * area.sizeDelta.y + -area.anchoredPosition.y + area.sizeDelta.y;
        return new Vector2(x, y);
    }
    void DrawBezier(ASCurve curve)
    {
        float t0 = startPos.x, t1, y;
        Vector2 v1, v2;
        for (int i = 1; i <= segmentCount; i++)
        {
            t1 = startPos.x + ((float)i / segmentCount) * range.x;
            y = curve.Evaluate(t0);
            v1 = ConvertV(new Vector2(t0, y));
            y = curve.Evaluate(t1);
            v2 = ConvertV(new Vector2(t1, y));
            t0 = t1;
            GLUI.DrawLine(v1, v2, curveColor);
        }
    }
    void DrawBezier(ASKey k1, ASKey k2)
    {
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
        points.Add(Input.mousePosition * ASUI.facterToReference);
        points[points.Count - 1] = points[points.Count - 1].SetY(ASUI.scaler.referenceResolution.y - points[points.Count - 1].y);
    }

    private void OnGUI()
    {
        var e = Event.current;
        if (e.type == EventType.mouseDown) e.Use();
    }
    void Update()
    {
        var shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        var ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (Input.GetMouseButtonDown((int)ASUI.MouseButton.Middle) && ASUI.MouseOver(area, ruler, rulerY)) { middle = true; oldPos = ASUI.mousePositionRef; }
        if (!Input.GetMouseButton((int)ASUI.MouseButton.Middle)) middle = false;
        float delta = Input.GetAxis("Mouse ScrollWheel");
        if (delta != 0)
        {
            if (ASUI.MouseOver(area, ruler))
            {
                if (shift || ctrl) rulerLength.y -= delta * rulerScalerSensitivity;
                if (!shift) rulerLength.x -= delta * rulerScalerSensitivity;
            }
            if (ASUI.MouseOver(rulerY))
            {
                if (ctrl) rulerLength.x -= delta * rulerScalerSensitivity;
                rulerLength.y -= delta * rulerScalerSensitivity;
            }
            rulerLength.x = Mathf.Clamp(rulerLength.x, 1, Mathf.Infinity);
            rulerLength.y = Mathf.Clamp(rulerLength.y, 1, Mathf.Infinity);
        }
        if (middle)
        {
            var deltaV = ASUI.mousePositionRef - oldPos;
            startPos -= deltaV * moveSensitivity;
            oldPos = ASUI.mousePositionRef;
        }
        ASUI.owner = area;
        GLUI.BeginOrtho();
        UpdateASUI();
        GLUI.BeginOrder(1);
        DrawBezier(curve);
        for (int i = 0; i < keys.Count; i++)
        {
            DrawPointOnArea(keys[i].ToVector2(), keyPointColor);
            GLUI.DrawLine(ConvertV(keys[i]), ConvertV(keys[i].inTangent), controlLineColor);
            DrawPointOnArea(keys[i].inTangent, controlPointColor);
            GLUI.DrawLine(ConvertV(keys[i]), ConvertV(keys[i].outTangent), controlLineColor);
            DrawPointOnArea(keys[i].outTangent, controlPointColor);
        }
        GLUI.EndOrder();
    }
    void DrawPointOnArea(Vector2 v, Color color)
    {
        if (MathTool.Between(v, startPos, endPos))
        {
            var d = v - startPos;
            var n = MathTool.Divide(d, range);
            var x = n.x * area.sizeDelta.x + area.anchoredPosition.x;
            var y = -n.y * area.sizeDelta.y + -area.anchoredPosition.y + area.sizeDelta.y;
            GLUI.DrawSquare(new Vector2(x, y), squareSize, color);
        }
    }
    private void MouseDown()
    {
        MouseDrag();
    }
    private void MouseDrag()
    {
        var lx = Input.mousePosition.x * ASUI.facterToReference - area.anchoredPosition.x;
        lx = lx / area.sizeDelta.x;
        lx = Mathf.Clamp01(lx);
        UITimeLine.FrameIndex = Mathf.RoundToInt(lx * rulerLength.x);
    }
    private void InitASUI()
    {
        ASUI.parent = area;
    }
    void UpdateASUI()
    {
        float x, y, deltaX, deltaY;
        Vector2 p;
        int num;
        for (int i = 0; i < rulerLength.y; i++)
        {
            deltaY = i / rulerLength.y;
            num = startPosInt.y + i;
            if ((num % spaceInRulerY) == 0)
            {
                y = deltaY * rulerY.sizeDelta.y;
                x = rulerY.anchoredPosition.x;
                y = -y - rulerY.anchoredPosition.y + rulerY.sizeDelta.y;
                p = new Vector2(x, y);
                IMUI.DrawText(num.ToString(), p, Vector2.one * 0.5f);
                x = area.anchoredPosition.x;
                p = new Vector2(x, y);
                GLUI.DrawLine(p, p + Vector2.right * area.sizeDelta.x, Color.grey - ASColor.V * 0.3f);
            }
            else if ((num % spaceLineInRulerY) == 0)
            {
                y = deltaY * area.sizeDelta.y;
                y = -y - area.anchoredPosition.y + rulerY.sizeDelta.y;
                x = area.anchoredPosition.x;
                p = new Vector2(x, y);
                GLUI.DrawLine(p, p + Vector2.right * area.sizeDelta.x, Color.grey - ASColor.V * 0.2f);
            }
        }
        for (int i = 0; i < rulerLength.x; i++)
        {
            num = startPosInt.x + i;
            deltaX = i / rulerLength.x;
            if ((num % spaceInRulerX) == 0)
            {
                x = deltaX * ruler.sizeDelta.x;
                x += ruler.anchoredPosition.x;
                y = -ruler.anchoredPosition.y;
                p = new Vector2(x, y);
                IMUI.DrawText(num.ToString(), p, Vector2.one * 0.5f);
                y = -area.anchoredPosition.y;
                p = new Vector2(x, y);
                GLUI.DrawLine(p, p + Vector2.up * area.sizeDelta.y, Color.grey - ASColor.V * 0.3f);
            }
            else if ((num % spaceLineInRulerX) == 0)
            {
                x = deltaX * area.sizeDelta.x;
                x += area.anchoredPosition.x;
                y = -area.anchoredPosition.y;
                p = new Vector2(x, y);
                GLUI.DrawLine(p, p + Vector2.up * area.sizeDelta.y, Color.grey - ASColor.V * 0.2f);
            }
            if (UITimeLine.Clip.HasKey(UITimeLine.ObjCurve, num))
            {
                x = deltaX * area.sizeDelta.x;
                x += area.anchoredPosition.x;
                y = -area.anchoredPosition.y;
                p = new Vector2(x, y);
                GLUI.commandOrder = 2;
                GLUI.DrawLine(p, p + Vector2.up * area.sizeDelta.y, lineWidth * 0.45f, Color.yellow - ASColor.V * 0.2f);
            }
        }
        deltaX = UITimeLine.FrameIndex / rulerLength.x;
        x = deltaX * area.sizeDelta.x;
        x += area.anchoredPosition.x;
        y = -area.anchoredPosition.y;
        p = new Vector2(x, y);
        GLUI.commandOrder = 1;
        GLUI.DrawLine(p, p + Vector2.up * area.sizeDelta.y, lineWidth, Color.green + ASColor.H * 0.2f);
    }
}
