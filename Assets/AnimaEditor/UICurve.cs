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

    public float moveSensitivity = 0.3f;
    public Color keyPointColor = Color.green;
    public Color curveColor = Color.green - ASColor.V * 0.5f;
    public Color controlPointColor = Color.grey + ASColor.V * 0.3f;
    public Color controlLineColor = Color.grey;
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
            GLUI.DrawLine(v1, v2, lineWidth, curveColor);
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
        InitASUI();

        ASUI.I.inputCallBacks.Add(new ASGUI.InputCallBack(GetInput, 2));
    }
    Vector2 oldPos;
    bool use, left, right, middle, shift, ctrl;
    public void GetInput()
    {
        shift = Events.shift;
        ctrl = Events.ctrl;
        use = false;
        var over = ASUI.MouseOver(area, ruler, rulerY);
        if (Events.MouseDown(MouseButton.Middle) && over) { oldPos = ASUI.mousePositionRef; MouseDown(MouseButton.Middle); middle = true; }
        if (Events.MouseDown(MouseButton.Left) && over) { oldPos = ASUI.mousePositionRef; MouseDown(MouseButton.Left); left = true; }
        if (Events.MouseDown(MouseButton.Right) && over) { oldPos = ASUI.mousePositionRef; MouseDown(MouseButton.Right); right = true; }
        if (!Events.Mouse(MouseButton.Middle)) middle = false;
        if (!Events.Mouse(MouseButton.Left)) left = false;
        if (!Events.Mouse(MouseButton.Right)) right = false;
        if (middle) MouseDrag(MouseButton.Middle);
        if (left) MouseDrag(MouseButton.Left);
        if (right) MouseDrag(MouseButton.Right);

        float delta = Events.Axis("Mouse ScrollWheel");
        if (delta != 0)
        {
            if (ASUI.MouseOver(area, ruler))
            {
                use = true;
                if (shift || ctrl) rulerLength.y -= delta * rulerScalerSensitivity;
                if (!shift) rulerLength.x -= delta * rulerScalerSensitivity;
            }
            if (ASUI.MouseOver(rulerY))
            {
                use = true;
                if (ctrl) rulerLength.x -= delta * rulerScalerSensitivity;
                rulerLength.y -= delta * rulerScalerSensitivity;
            }
            rulerLength.x = Mathf.Clamp(rulerLength.x, 1, Mathf.Infinity);
            rulerLength.y = Mathf.Clamp(rulerLength.y, 1, Mathf.Infinity);
        }
        if (use) Events.Use();
        ipRef = ASUI.mousePositionRef;

    }
    public int ind;
    public int t; // -1 intan 0 point 1 outtan
    public float clickDist = 5;
    public float dist1;
    public float dist1in;
    public float dist1out;
    public Vector2 V1;
    public Vector2 V1in;
    public Vector2 V1out;
    public Vector2 ipRef;
    private void MouseDown(MouseButton button)
    {
        use = true;
        if (button == MouseButton.Left)
        {
            ind = -1;
            V1 = ConvertV(keys[0]);
            V1in = ConvertV(keys[0].inTangent);
            V1out = ConvertV(keys[0].outTangent);
            dist1 = ASUI.mouseDistLT(V1);
            dist1in = ASUI.mouseDistLT(V1in);
            dist1out = ASUI.mouseDistLT(V1out);

            for (int i = 0; i < keys.Count; i++)
            {
                if (ASUI.mouseDistLT(ConvertV(keys[i])) < clickDist) { ind = i; t = 0; }
                if (ASUI.mouseDistLT(ConvertV(keys[i].inTangent)) < clickDist) { ind = i; t = -1; }
                if (ASUI.mouseDistLT(ConvertV(keys[i].outTangent)) < clickDist) { ind = i; t = 1; }
            }
            MouseDrag(button);
        }
    }
    public Vector2 p;
    private void MouseDrag(MouseButton button)
    {
        use = true;
        var deltaV = ASUI.mousePositionRef - oldPos;
        switch (button)
        {
            case MouseButton.Left:
                var l = ASUI.mousePositionRefLT - new Vector2(area.anchoredPosition.x, -area.anchoredPosition.y);
                l.y = area.sizeDelta.y - l.y;
                l = MathTool.Divide(l, area.sizeDelta);
                p = (startPos + Vector2.Scale(l, rulerLength));
                if (ind > -1)
                {
                    deltaV = MathTool.Divide(deltaV, area.sizeDelta);
                    deltaV = Vector2.Scale(deltaV, rulerLength);
                    if (t == 0)
                    {
                        keys[ind].index += Mathf.RoundToInt(deltaV.x); keys[ind].value += deltaV.y;
                        keys[ind].inTangent += deltaV;
                        keys[ind].outTangent += deltaV;
                    }
                    if (t == -1) keys[ind].inTangent += deltaV;
                    if (t == 1) keys[ind].outTangent += deltaV;
                }
                else // timeline
                {
                    UITimeLine.FrameIndex = Mathf.RoundToInt(p.x);
                }
                break;
            case MouseButton.Right:

                break;

            case MouseButton.Middle:

                startPos -= deltaV * moveSensitivity;
                break;
            default: throw null;
        }
        oldPos = ASUI.mousePositionRef;
    }
    ASKey ConvertK(Vector2 v)
    {
        var d = v - startPos;
        var n = MathTool.Divide(d, range);
        var x = n.x * area.sizeDelta.x + area.anchoredPosition.x;
        var y = -n.y * area.sizeDelta.y + -area.anchoredPosition.y + area.sizeDelta.y;
        return new ASKey(Mathf.RoundToInt(x), y);
    }
    Vector2 ConvertR(Vector2 v)
    {
        var d = v - startPos;
        var n = MathTool.Divide(d, range);
        var x = n.x * area.sizeDelta.x + area.anchoredPosition.x;
        var y = -n.y * area.sizeDelta.y + -area.anchoredPosition.y + area.sizeDelta.y;
        return new Vector2(x, y);
    }
    void Update()
    {
        ASUI.owner = area;
        GLUI.BeginOrtho();
        UpdateASUI();
        GLUI.BeginOrder(1);
        DrawBezier(curve);
        for (int i = 0; i < keys.Count; i++)
        {
            DrawPointOnArea(keys[i].ToVector2(), keyPointColor);
            GLUI.DrawLine(ConvertV(keys[i]), ConvertV(keys[i].inTangent), lineWidth, controlLineColor);
            DrawPointOnArea(keys[i].inTangent, controlPointColor);
            GLUI.DrawLine(ConvertV(keys[i]), ConvertV(keys[i].outTangent), lineWidth, controlLineColor);
            DrawPointOnArea(keys[i].outTangent, controlPointColor);
        }
        GLUI.EndOrder();
    }
    Vector2 ConvertV2(Vector2 v)
    {
        var d = v - startPos;
        var n = MathTool.Divide(d, range);
        var x = n.x * area.sizeDelta.x + area.anchoredPosition.x;
        var y = -n.y * area.sizeDelta.y + -area.anchoredPosition.y + area.sizeDelta.y;
        return new Vector2(x, y);
    }
    Vector2 ConvertV(Vector2 v)
    {
        var d = v - startPos;
        var n = MathTool.Divide(d, range);
        var x = n.x * area.sizeDelta.x + area.anchoredPosition.x;
        var y = -n.y * area.sizeDelta.y + -area.anchoredPosition.y + area.sizeDelta.y;
        return new Vector2(x, y);
    }
    void DrawPointOnArea(Vector2 v, Color color)
    {
        if (MathTool.Between(v, startPos, endPos))
        {
            var d = v - startPos;
            var n = MathTool.Divide(d, range);
            var x = n.x * area.sizeDelta.x + area.anchoredPosition.x;
            var y = -n.y * area.sizeDelta.y + -area.anchoredPosition.y + area.sizeDelta.y;
            GLUI.DrawSquare(new Vector2(x, y), squareSize, lineWidth, color);
        }
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
        if (MathTool.Between(UITimeLine.FrameIndex, startPos.x, endPos.x))
        {
            deltaX = (UITimeLine.FrameIndex - startPos.x) / rulerLength.x;
            x = deltaX * area.sizeDelta.x;
            x += area.anchoredPosition.x;
            y = -area.anchoredPosition.y;
            p = new Vector2(x, y);
            GLUI.commandOrder = 1;
            GLUI.DrawLine(p, p + Vector2.up * area.sizeDelta.y, lineWidth, Color.green + ASColor.H * 0.2f);
        }
    }
}
