using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UICurve : MonoBehaviour
{
    public static UICurve I
    { get { if (instance == null) instance = FindObjectOfType<UICurve>(); return instance; } }
    private static UICurve instance;
    private RectTransform area;
    private RectTransform rulerX;
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
    public Vector2 startPos;
    public Vector2Int startPosInt
    {
        get { return new Vector2Int(Mathf.RoundToInt(startPos.x), Mathf.RoundToInt(startPos.y)); }
    }
    public List<Vector2> points;
    public AnimationCurve acurve;
    public ASObjectCurve curve
    {
        get { return UITimeLine.ObjCurve; }
    }
    public List<ASKey> keys { get { return curve.timeCurve.keys; } }
    public float squareSize;
    public float lineWidth;
    public int segmentCount = 10;

    public Color keyPointColor = Color.green;
    public Color curveColor = Color.green - ASColor.V * 0.5f;
    public Color controlPointColor = Color.grey + ASColor.V * 0.3f;
    public Color controlLineColor = Color.grey;
    public Color trueTimeRhombusEdgeColor = Color.red + Color.green * 0.5f;
    public Color trueTimeCurveColor = Color.red;
    public Color rulerValueLineColor;
    public Color rulerIndexLineColor = Color.green + ASColor.H * 0.2f;
    Vector2 oldPos;
    bool use, left, right, middle, shift, ctrl, alt;
    public int ind;
    public int tt; // -1 intan 0 point 1 outtan
    public float clickDist = 5;
    public Vector2 p;
    public bool showTrueTimeArea;
    public bool showTrueTimeCurve;
    public bool showControlPoints;
    public bool syncTwoSideTangentDir;
    public bool flipTwoSideTangent;
    public bool controlY;
    public float tangentSlopeCalDeltaX;
    public Vector2 inT;
    public Vector2 outT;
    public float timeCurveN;
    public float GenerateRealTime(float t)
    {
        Vector2 a = Vector2.zero, b = Vector2.zero;
        int rInd = -1;
        for (int i = 1; i < keys.Count; i++)
        {
            a = keys[i - 1];
            b = keys[i];
            if (MathTool.Between(t, a.x, b.x))
            {
                rInd = i;
                break;
            }
        }
        if (rInd > -1)
        {
            var m = Vector2.Lerp(a, b, 0.5f);
            var len = m.x - a.x;
            var top = m + new Vector2(0, len);
            var btm = m - new Vector2(0, len);
            var r = Vector2.Distance(top, a);

            var curveV = new Vector2(t, curve.timeCurve.Evaluate(t));

            var axis = top - a;
            var vLocal = curveV - a;
            var ln = Vector2.Dot(vLocal, axis) / r; // ab两帧的值一定要相等，否则投影出的长度不正确，因为坐标轴不是正交的。
            var tn = Mathf.Clamp01(ln / r);
            var rt = Mathf.Lerp(a.x, b.x, tn);
            return rt;
        }
        else return t;
    }
    void DrawBezier()
    {
        if (curve == null || keys.Count == 0) return;
        // 绘制插值曲线        
        ASCurve.tangentSlopeCalDeltaX = tangentSlopeCalDeltaX;
        float t0 = startPos.x, t1, y;
        Vector2 v1, v2;
        for (int i = 1; i <= segmentCount; i++)
        {
            t1 = startPos.x + ((float)i / segmentCount) * range.x;
            y = curve.timeCurve.Evaluate(t0);
            v1 = ConvertV(new Vector2(t0, y));
            y = curve.timeCurve.Evaluate(t1);
            v2 = ConvertV(new Vector2(t1, y));
            t0 = t1;
            GLUI.DrawLine(v1, v2, lineWidth, curveColor);
        }
        GLUI.BeginOrder(2);
        // 绘制控制点和线
        for (int i = 0; i < keys.Count; i++)
        {
            DrawPointOnArea(keys[i].ToVector2(), keyPointColor);
            if (showControlPoints && keys[i].inMode == CurveMode.Bezier)//插值模式不是贝塞尔就不画
            {
                GLUI.DrawLine(ConvertV(keys[i]), ConvertV(keys[i].inTangentAbs), lineWidth, controlLineColor);
                DrawPointOnArea(keys[i].inTangentAbs, controlPointColor); //画点
            }
            if (showControlPoints && keys[i].outMode == CurveMode.Bezier)
            {
                GLUI.DrawLine(ConvertV(keys[i]), ConvertV(keys[i].outTangentAbs), lineWidth, controlLineColor);
                DrawPointOnArea(keys[i].outTangentAbs, controlPointColor);
            }
        }
        Vector2 a = Vector2.zero, b = Vector2.zero, m, top, btm;
        Vector2 aV, bV, topV, btmV;
        float len;
        GLUI.BeginOrder(1);
        if (!showTrueTimeArea) return;
        //int rInd = -1;
        //for (int i = 1; i < keys.Count; i++)
        //{
        //    a = keys[i - 1];
        //    b = keys[i];
        //    if (MathTool.Between(UITimeLine.FrameIndex, a.x, b.x))
        //    {
        //        rInd = i;
        //    }
        //}
        //if (rInd > -1)
        //{
        //    m = Vector2.Lerp(a, b, 0.5f);
        //    len = m.x - a.x;
        //    top = m + new Vector2(0, len);
        //    btm = m - new Vector2(0, len);
        //    var r = Vector2.Distance(top, a);

        //    var t = UITimeLine.FrameIndex;
        //    var curveV = new Vector2(t, curve.timeCurve.Evaluate(t));

        //    var axis = top - a;
        //    var vLocal = curveV - a;
        //    var ln = Vector2.Dot(vLocal, axis) / r; // ab两帧的值一定要相等，否则投影出的长度不正确，因为坐标轴不是正交的。

        //    DrawPointOnArea(a + new Vector2(0, ln), Color.red);
        //    DrawPointOnArea(a + new Vector2(0, r), Color.red);

        //    timeCurveN = Mathf.Clamp01(ln / r);
        //}
        // 画菱形，用于时间曲线
        for (int i = 1; i < keys.Count; i++)
        {
            a = keys[i - 1];
            b = keys[i];
            m = Vector2.Lerp(a, b, 0.5f);
            len = m.x - a.x;
            top = m + new Vector2(0, len);
            btm = m - new Vector2(0, len);
            aV = ConvertV(a);
            bV = ConvertV(b);
            topV = ConvertV(top);
            btmV = ConvertV(btm);
            if (showTrueTimeArea)
            {
                GLUI.DrawLine(aV, topV, trueTimeRhombusEdgeColor); // 菱形的真实时间区域
                GLUI.DrawLine(topV, bV, trueTimeRhombusEdgeColor);
                GLUI.DrawLine(bV, btmV, trueTimeRhombusEdgeColor);
                GLUI.DrawLine(btmV, aV, trueTimeRhombusEdgeColor);
            }
            if (!showTrueTimeCurve) continue;
            var r = Vector2.Distance(top, a);
            t0 = 0;
            for (int s = 1; s <= segmentCount; s++) // 画真实时间曲线（错误，暂没修，正确方法见前面timeCurveN，因为曲线和top-a的插值是不一样的）
            {
                t1 = ((float)s / segmentCount);

                v1 = Vector2.Lerp(top, b, t0);
                v2 = Vector2.Lerp(top, b, t1);
                var tA = Mathf.Lerp(a.x, b.x, t0);
                var tB = Mathf.Lerp(a.x, b.x, t1);
                var flat1 = new Vector2(tA, curve.timeCurve.Evaluate(tA));
                var flat2 = new Vector2(tA, curve.timeCurve.Evaluate(tB));
                var d1 = Vector2.Distance(flat1, v1);
                var d2 = Vector2.Distance(flat2, v2);

                v1 = Vector2.Lerp(a, b, t0);
                v1.y += (r - d1);
                v2 = Vector2.Lerp(a, b, t1);
                v2.y += (r - d2);

                v1 = ConvertV(v1);
                v2 = ConvertV(v2);

                GLUI.DrawLine(v1, v2, lineWidth, trueTimeCurveColor);
                t0 = t1;
            }
        }
    }
    void DrawPointOnArea(Vector2 v, Color color)
    {
        if (MathTool.Between(v, startPos, endPos))
        {
            var d = v - startPos;
            var n = MathTool.Divide(d, range);
            var x = n.x * area.rect.width;// + area.anchoredPosition.x;
            var y = -n.y * area.rect.height + area.rect.height; //-area.anchoredPosition.y;

            GLUI.DrawSquare(ASUI.AbsRefPos(area) + new Vector2(x, y), squareSize, lineWidth, color);
        }
    }
    void Start()
    {
        area = transform.Search("Area") as RectTransform;
        rulerX = transform.Search("Ruler X") as RectTransform;
        rulerY = transform.Search("Ruler Y") as RectTransform;
        InitASUI();

        ASUI.I.inputCallBacks.Add(new ASGUI.InputCallBack(GetInput, 2));
    }
    public bool over;
    public void GetInput()
    {
        shift = Events.shift;
        ctrl = Events.ctrl;
        alt = Events.Alt;
        use = false;
        over = ASUI.MouseOver(area, rulerX, rulerY);
        var simMidDown = Events.MouseDown(MouseButton.Left) && alt;
        if ((Events.MouseDown(MouseButton.Middle) || simMidDown) && over) { oldPos = ASUI.mousePositionRef; MouseDown(MouseButton.Middle); middle = true; }
        if (Events.MouseDown(MouseButton.Left) && over && !simMidDown) { oldPos = ASUI.mousePositionRef; MouseDown(MouseButton.Left); left = true; }
        if (Events.MouseDown(MouseButton.Right) && over) { oldPos = ASUI.mousePositionRef; MouseDown(MouseButton.Right); right = true; }
        var simMid = Events.Mouse(MouseButton.Left) && alt;
        if (!Events.Mouse(MouseButton.Middle) && !simMid) middle = false;
        if (!Events.Mouse(MouseButton.Left) || simMid) left = false;
        if (!Events.Mouse(MouseButton.Right)) right = false;
        if (middle) MouseDrag(MouseButton.Middle);
        if (left) MouseDrag(MouseButton.Left);
        if (right) MouseDrag(MouseButton.Right);
        var codes = Enum.GetValues(typeof(KeyCode));
        foreach (KeyCode c in codes)
        {
            if (Events.KeyDown(c)) KeyDown(c);
        }
        foreach (KeyCode c in codes)
        {
            if (Events.KeyUp(c)) KeyUp(c);
        }
        foreach (KeyCode c in codes)
        {
            if (Events.Key(c)) Key(c);
        }
        float delta = Events.Axis("Mouse ScrollWheel");
        if (delta != 0)
        {
            if (ASUI.MouseOver(area, rulerX))
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
    }
    private void Key(KeyCode c)
    {
    }
    private void KeyUp(KeyCode c)
    {
    }
    void KeyDown(KeyCode code)
    {
        if (code == KeyCode.P)
        {
            var type = 0;
            var index = -1;
            for (int i = 0; i < keys.Count; i++)
            {
                if (ASUI.mouseDistLT(ConvertV(keys[i])) < clickDist) { index = i; type = 0; }
                if (showControlPoints && keys[i].inMode == CurveMode.Bezier)//不是贝塞尔就不控制切点
                {
                    if (ASUI.mouseDistLT(ConvertV(keys[i].inTangentAbs)) < clickDist) { index = i; type = -1; }
                }
                if (showControlPoints && keys[i].outMode == CurveMode.Bezier)
                {
                    if (ASUI.mouseDistLT(ConvertV(keys[i].outTangentAbs)) < clickDist) { index = i; type = 1; }
                }
            }
            if (index > -1)
            {
                if (type == -1)
                {
                    keys[index].inMode = CurveMode.None;
                }
                else if (type == 1)
                {
                    keys[index].outMode = CurveMode.None;
                }
            }
        }
        if (code == KeyCode.X)
        {
            var type = 0;
            var index = -1;
            for (int i = 0; i < keys.Count; i++)
            {
                if (ASUI.mouseDistLT(ConvertV(keys[i])) < clickDist) { index = i; type = 0; }
                if (showControlPoints && keys[i].inMode == CurveMode.Bezier)//不是贝塞尔就不控制切点
                {
                    if (ASUI.mouseDistLT(ConvertV(keys[i].inTangentAbs)) < clickDist) { index = i; type = -1; }
                }
                if (showControlPoints && keys[i].outMode == CurveMode.Bezier)
                {
                    if (ASUI.mouseDistLT(ConvertV(keys[i].outTangentAbs)) < clickDist) { index = i; type = 1; }
                }
            }
            if (index > -1)
            {
                var frameIndex = keys[index].frameIndex;
                if (type == 0)
                {
                    foreach (var curve in UIClip.clip.curves)
                    {
                        UIClip.clip.RemoveKey(curve, frameIndex);
                    }
                }
            }
        }
    }
    private void MouseDown(MouseButton button)
    {
        if (curve == null || keys.Count == 0) return;
        use = true;
        if (button == MouseButton.Left)
        {
            ind = -1;
            for (int i = 0; i < keys.Count; i++)
            {
                if (ASUI.mouseDistLT(ConvertV(keys[i])) < clickDist) { ind = i; tt = 0; }
                if (showControlPoints && keys[i].inMode == CurveMode.Bezier)//不是贝塞尔就不控制切点
                {
                    if (ASUI.mouseDistLT(ConvertV(keys[i].inTangentAbs)) < clickDist) { ind = i; tt = -1; }
                }
                if (showControlPoints && keys[i].outMode == CurveMode.Bezier)
                {
                    if (ASUI.mouseDistLT(ConvertV(keys[i].outTangentAbs)) < clickDist) { ind = i; tt = 1; }
                }
            }
            MouseDrag(button);
        }
    }
    int SetAllKeysPointFrameIndex(int frameIndexOrigin, int frameIndexNew)
    {
        int r = -1;
        int i;
        foreach (var curve in UIClip.clip.curves)
        {
            i = -1;
            i = curve.timeCurve.MoveKey(frameIndexOrigin, frameIndexNew);
            if (i > -1) r = i;
            i = -1;
            i = curve.eulerAngles[0].MoveKey(frameIndexOrigin, frameIndexNew);
            if (i > -1) r = i;
            i = -1;
            i = curve.eulerAngles[1].MoveKey(frameIndexOrigin, frameIndexNew);
            if (i > -1) r = i;
            i = -1;
            i = curve.eulerAngles[2].MoveKey(frameIndexOrigin, frameIndexNew);
            if (i > -1) r = i;
            i = -1;
            i = curve.localPosition[0].MoveKey(frameIndexOrigin, frameIndexNew);
            if (i > -1) r = i;
            i = -1;
            i = curve.localPosition[1].MoveKey(frameIndexOrigin, frameIndexNew);
            if (i > -1) r = i;
            i = -1;
            i = curve.localPosition[2].MoveKey(frameIndexOrigin, frameIndexNew);
            if (i > -1) r = i;
        }
        return r;
    }
    int SetAllKeysPoint(int i, int frameIndex, float v)
    {
        int frameIndexAfter = -1;
        foreach (var curve in UIClip.clip.curves)
        {
            frameIndexAfter = curve.SetKeysPoint(i, frameIndex, v);
        }
        if (frameIndexAfter == -1)
        {
            frameIndexAfter = frameIndex;
        }
        return frameIndexAfter;
    }
    void SetAllKeysInTangent(int i, Vector2 vector2)
    {
        foreach (var curve in UIClip.clip.curves)
        {
            curve.SetKeysInTangent(i, vector2);
        }
    }
    void SetAllKeysOutTangent(int i, Vector2 vector2)
    {
        foreach (var curve in UIClip.clip.curves)
        {
            curve.SetKeysOutTangent(i, vector2);
        }
    }
    private void MouseDrag(MouseButton button)
    {
        use = true;
        var deltaV = ASUI.mousePositionRef - oldPos;
        deltaV = MathTool.Divide(deltaV, area.rect.size);
        deltaV = Vector2.Scale(deltaV, rulerLength);

        switch (button)
        {
            case MouseButton.Left:
                var l = ASUI.mousePositionRefLT - ASUI.AbsRefPos(area);
                l.y = area.rect.height - l.y;
                l = MathTool.Divide(l, area.rect.size);
                p = (startPos + Vector2.Scale(l, rulerLength));
                if (ind > -1)
                {
                    if (tt == 0)
                    {
                        //控制位置改变后的索引，比如第1帧移动到了第10帧，此时返回索引是10
                        //ind = curve.SetKeysPoint(ind, keys[ind].frameIndex + Mathf.RoundToInt(deltaV.x), keys[ind].value + (controlY ? deltaV.y : 0f));
                        SetAllKeysPointFrameIndex(keys[ind].frameIndex, keys[ind].frameIndex + Mathf.RoundToInt(deltaV.x));
                        ind = curve.SetKeysPoint(ind, keys[ind].frameIndex + Mathf.RoundToInt(deltaV.x), keys[ind].value + (controlY ? deltaV.y : 0f));
                    }
                    else if (tt == -1)
                    {
                        inT = keys[ind].inTangent + deltaV;
                        inT.x = Mathf.Clamp(inT.x, float.NegativeInfinity, 0);
                        outT = keys[ind].outTangent;
                        //curve.SetKeysInTangent(ind, inT);
                        //if (ctrl != flipTwoSideTangent) curve.SetKeysOutTangent(ind, -inT);
                        //else if (shift != (syncTwoSideTangentDir && inT.normalized.magnitude > 0.001f)) curve.SetKeysOutTangent(ind, -inT.normalized * outT.magnitude);//相反的方向移动，保持原来的长度
                        SetAllKeysInTangent(ind, inT);
                        if (ctrl != flipTwoSideTangent) SetAllKeysOutTangent(ind, -inT);
                        else if (shift != (syncTwoSideTangentDir && inT.normalized.magnitude > 0.001f)) SetAllKeysOutTangent(ind, -inT.normalized * outT.magnitude);//相反的方向移动，保持原来的长度
                    }
                    // 0.001f防止在当前切点位置为0时，normalized也是0，因此使反方向切点位置归0的问题。
                    else if (tt == 1)
                    {
                        inT = keys[ind].inTangent;
                        outT = keys[ind].outTangent + deltaV;
                        outT.x = Mathf.Clamp(outT.x, 0, float.PositiveInfinity);
                        //curve.SetKeysOutTangent(ind, outT);
                        //if (ctrl != flipTwoSideTangent) curve.SetKeysInTangent(ind, -outT);
                        //else if (shift != (syncTwoSideTangentDir && outT.normalized.magnitude > 0.001f)) curve.SetKeysInTangent(ind, -outT.normalized * inT.magnitude);
                        SetAllKeysOutTangent(ind, outT);
                        if (ctrl != flipTwoSideTangent) SetAllKeysInTangent(ind, -outT);
                        else if (shift != (syncTwoSideTangentDir && outT.normalized.magnitude > 0.001f)) SetAllKeysInTangent(ind, -outT.normalized * inT.magnitude);
                    }
                }
                else // timeline
                {
                    UITimeLine.FrameIndex = Mathf.RoundToInt(p.x);
                    UITimeLine.FrameValue = p.y;
                }
                break;
            case MouseButton.Right:
                break;

            case MouseButton.Middle:

                startPos -= deltaV;
                break;
            default: throw null;
        }
        oldPos = ASUI.mousePositionRef;
    }
    int lastFrameIndex;
    public Vector2 min;
    public Vector2 max;
    void Update()
    {
        ASUI.owner = area;
        min = ASUI.Rect(ASUI.owner)[0];
        max = ASUI.Rect(ASUI.owner)[1];
        GLUI.BeginOrtho();
        UpdateASUI();
        GLUI.BeginOrder(1);
        DrawBezier();
        if (UITimeLine.FrameIndex != lastFrameIndex) OnFrameIndexChanged();
        lastFrameIndex = UITimeLine.FrameIndex;
        GLUI.EndOrder();
    }
    private void OnFrameIndexChanged()
    {
        //if (curve == null || keys.Count == 0) return;
        UIClip.I.UpdateAllCurve();
        //var trueTime = GenerateRealTime(UITimeLine.FrameIndex);
        //UIDOFEditor.I.ast.euler = UITimeLine.ObjCurve.EulerAngles(trueTime);
    }
    Vector2 ConvertV(Vector2 v)
    {
        var d = v - startPos;
        var n = MathTool.Divide(d, range);
        var x = n.x * area.rect.width;// + area.anchoredPosition.x;
        var y = area.rect.height - n.y * area.rect.height;// -area.anchoredPosition.y;

        return ASUI.AbsRefPos(area) + new Vector2(x, y);
    }
    Vector2 ConvertV2(Vector2 v)
    {
        var d = v - startPos;
        var n = MathTool.Divide(d, range);
        var x = n.x * area.rect.width + area.anchoredPosition.x;
        var y = area.rect.height - n.y * area.rect.height - area.anchoredPosition.y;

        return ASUI.AbsRefPos(area) + new Vector2(x, y);
    }
    private void InitASUI()
    {
        ASUI.parent = area;
    }
    void UpdateASUI()
    {
        float nX, nY;
        Vector2 p;
        int num;
        var areaP = ASUI.AbsRefPos(area);
        var rulerYP = ASUI.AbsRefPos(rulerY);
        var rulerXP = ASUI.AbsRefPos(rulerX);
        for (int i = 0; i < rulerLength.y; i++)
        {
            nY = i / rulerLength.y;
            num = startPosInt.y + i;
            if ((num % spaceInRulerY) == 0)
            {
                p = rulerYP;
                p.x += rulerY.rect.width * 0.5f;
                p.y += rulerY.rect.height - nY * rulerY.rect.height;
                IMUI.DrawText(num.ToString(), p, Vector2.one * 0.5f); // 画字

                p = areaP;
                p.y += area.rect.height - nY * area.rect.height;
                GLUI.DrawLine(p, p + Vector2.right * area.rect.width, Color.grey - ASColor.V * 0.3f); // 画横线
            }
            else if ((num % spaceLineInRulerY) == 0)
            {
                p = areaP;
                p.y += area.rect.height - nY * area.rect.height;
                GLUI.DrawLine(p, p + Vector2.right * area.rect.width, Color.grey - ASColor.V * 0.2f); // 画横线
            }
        }
        for (int i = 0; i < rulerLength.x; i++)
        {
            num = startPosInt.x + i;
            nX = i / rulerLength.x;
            var dX = nX * area.rect.width;
            if ((num % spaceInRulerX) == 0)
            {
                p = rulerXP;
                p.x += dX;
                p.y += rulerX.rect.height * 0.5f;//居中
                IMUI.DrawText(num.ToString(), p, Vector2.one * 0.5f);//画字

                p = areaP;
                p.x += dX;
                GLUI.DrawLine(p, p + Vector2.up * area.rect.height, Color.grey - ASColor.V * 0.3f);
            }
            else if ((num % spaceLineInRulerX) == 0)
            {
                p = areaP;
                p.x += nX * area.rect.width;
                GLUI.DrawLine(p, p + Vector2.up * area.rect.height, Color.grey - ASColor.V * 0.2f);
            }
        }
        if (MathTool.Between(UITimeLine.FrameIndex, startPos.x, endPos.x))
        {
            nX = (UITimeLine.FrameIndex - startPos.x) / rulerLength.x;
            p = areaP;
            p.x += nX * area.rect.width;
            GLUI.commandOrder = 2;
            GLUI.DrawLine(p, p + Vector2.up * area.rect.height, lineWidth, rulerIndexLineColor);
        }
        if (MathTool.Between(UITimeLine.FrameValue, startPos.y, endPos.y))
        {
            nY = (UITimeLine.FrameValue - startPos.y) / rulerLength.y;
            p = areaP;
            p.y += area.rect.height - nY * area.rect.height;
            GLUI.commandOrder = 1;
            GLUI.DrawLine(p, p + Vector2.right * area.rect.width, lineWidth, rulerValueLineColor);
        }
    }
}
