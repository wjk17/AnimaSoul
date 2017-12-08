using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;

[Serializable]
public class ASObjectCurve
{
    public string name;
    [XmlIgnore]
    public Transform trans;
    public ASCurve[] eulerAngles;//x,y,z
    public ASCurve[] localPosition;//x,y,z
    public ASCurve timeCurve;
    public Vector3 EulerAngles(float frameIndex)
    {
        return new Vector3(eulerAngles[0].Evaluate(frameIndex), eulerAngles[1].Evaluate(frameIndex), eulerAngles[2].Evaluate(frameIndex));
    }
    public Vector3 LocalPosition(float frameIndex)
    {
        return new Vector3(localPosition[0].Evaluate(frameIndex), localPosition[1].Evaluate(frameIndex), localPosition[2].Evaluate(frameIndex));
    }
    public ASObjectCurve()
    {
        eulerAngles = new ASCurve[3] { new ASCurve(), new ASCurve(), new ASCurve() };
        localPosition = new ASCurve[3] { new ASCurve(), new ASCurve(), new ASCurve() };
        timeCurve = new ASCurve();
    }
    public ASObjectCurve(Transform trans) : this()
    {
        this.trans = trans;
        this.name = trans.name;
    }
    public ASObjectCurve Clone()
    {
        var n = new ASObjectCurve();
        n.name = name;
        n.eulerAngles = eulerAngles.Clone() as ASCurve[];
        n.localPosition = localPosition.Clone() as ASCurve[];
        return n;
    }
    internal void SetKeysOutTangent(int i, Vector2 vector2)
    {
        if (i < 0) throw null;
        //foreach (var c in eulerAngles)
        //{
        //    if (i < c.keys.Count) c.keys[i].outTangent = vector2;
        //}
        //foreach (var c in localPosition)
        //{
        //    if (i < c.keys.Count) c.keys[i].outTangent = vector2;
        //}
        if (i < timeCurve.keys.Count) timeCurve.keys[i].outTangent = vector2;
    }
    internal void SetKeysInTangent(int i, Vector2 vector2)
    {
        if (i < 0) throw null;
        //foreach (var c in eulerAngles)
        //{
        //    if (i < c.keys.Count) c.keys[i].inTangent = vector2;
        //}
        //foreach (var c in localPosition)
        //{
        //    if (i < c.keys.Count) c.keys[i].inTangent = vector2;
        //}
        if (i < timeCurve.keys.Count) timeCurve.keys[i].inTangent = vector2;
    }
    internal int SetKeysPoint(int i, int frameIndex, float v)
    {
        if (i < 0) throw null;
        //foreach (var c in eulerAngles)
        //{
        //    if (i < c.keys.Count) { c.keys[i].value = v; c.SetIndex(i, frameIndex); }
        //}
        //foreach (var c in localPosition)
        //{
        //    if (i < c.keys.Count) { c.keys[i].value = v; c.SetIndex(i, frameIndex); }
        //}
        var tc = timeCurve;
        if (i < tc.keys.Count)
        {
            tc.keys[i].value = v; tc.SetIndex(i, frameIndex);
            return tc.IndexOf(frameIndex);
        }
        return i;
    }
}
public enum CurveMode
{
    Bezier,
    Constant, // 常数，硬转折，左端值优先，比如左右两点都是常数模式，使用左边点的值。
    Linear, // 一端线性一端贝塞尔时，线性端的控制点为线性方向t=0.3333333f
    None, // 一端none一端Bezier，就会使用二阶贝塞尔；两端none即是一阶贝塞尔，等同线性插值。
}
[Serializable]
public class ASKey
{
    public static implicit operator Vector2(ASKey a)
    {
        return a.ToVector2();
    }
    public int frameIndex;
    public float time
    {
        get { return frameIndex * UITimeLine.timePerFrame; }
    }
    public float value;
    public Vector2 inTangent;
    public Vector2 inTangentAbs { set { inTangent = value - this; } get { return this + inTangent; } }
    public Vector2 inTangentKey
    {
        get { return new Vector2(inTangent.x * UITimeLine.Fps, inTangent.y); }
    }
    public Vector2 outTangentAbs { set { outTangent = value - this; } get { return this + outTangent; } }
    public Vector2 outTangent;
    public Vector2 outTangentKey
    {
        get { return new Vector2(outTangent.x * UITimeLine.Fps, outTangent.y); }
    }
    public CurveMode inMode = CurveMode.None;
    public CurveMode outMode = CurveMode.None;
    public Vector2 ToVector2(float fps)
    {
        return new Vector2(frameIndex * (1 / fps), value);
    }
    public Vector2 ToVector2(bool convert = false)
    {
        if (convert) return new Vector2(frameIndex * (1 / UITimeLine.Fps), value);
        else return new Vector2(frameIndex, value);
    }
    public ASKey() { }
    public ASKey(int index, float value)
    {
        this.frameIndex = index;
        this.value = value;
    }
}
[Serializable]
public class ASCurve
{
    public List<ASKey> keys;
    public ASCurve() { keys = new List<ASKey>(); }
    public int IndexOf(int frameIndex)
    {
        if (!hasKey) return -1;
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i].frameIndex == frameIndex) { return i; }
        }
        return -1;
    }
    public bool hasKey
    {
        get { return keys != null && keys.Count > 0; }
    }
    public ASCurve Clone()
    {
        var n = new ASCurve();
        var newKeys = new List<ASKey>();
        foreach (var k in keys)
        {
            newKeys.Add(k);
        }
        n.keys = newKeys;
        return n;
    }
    public bool RemoveKey(int frameIndex)
    {
        var i = IndexOf(frameIndex);
        {
            if (i > -1) { keys.RemoveAt(i); return true; }
        }
        return false;
    }
    public void InsertKey(int index, float value)
    {
        InsertKey(new ASKey(index, value));
    }
    public float approxRange = 0.00001f;
    bool Approx(float a, float b)
    {
        var r = Mathf.Abs(a - b) < approxRange;
        return r;
    }
    public static float tangentSlopeCalDeltaX = 0.0000001f;
    //public static bool print = true;
    public void InsertKey(ASKey newKey) // 根据帧序号插入。已有此帧则修改值
    {
        if (!hasKey)//初始化
        {
            keys = new List<ASKey>();
            keys.Add(newKey);
            return;
        }
        for (int i = 0; i < keys.Count; i++)
        {
            if (newKey.frameIndex == keys[i].frameIndex)
            {
                keys[i].value = newKey.value;//覆盖帧（修改值）                
                return;
            }
            if (newKey.frameIndex < keys[i].frameIndex)
            {
                if (i == 0)//插入到最左边
                {
                    var b = keys[i];
                    if (b.inMode == CurveMode.None)
                    {
                        b.inMode = CurveMode.Bezier;
                        b.inTangentAbs = Vector2.Lerp(b, newKey, 0.3333333f);
                    }
                    newKey.outMode = CurveMode.Bezier;
                    newKey.outTangentAbs = Vector2.Lerp(newKey, b, 0.3333333f);
                }
                else if (i > 0 && i < keys.Count)//插在两个帧之间
                {
                    var t = newKey.frameIndex;
                    var a = keys[i - 1];
                    var b = keys[i];
                    var dxI = -tangentSlopeCalDeltaX;//delta X 带方向
                    var dxO = +tangentSlopeCalDeltaX;
                    var v = EvaluateInner(a, b, t);
                    var vI = EvaluateInner(a, b, t + dxI); // 函数 Y
                    var vO = EvaluateInner(a, b, t + dxO);
                    //var vIE = Evaluate(newKey.frameIndex + dxI);
                    //var vOE = Evaluate(newKey.frameIndex + dxO);
                    //if (print)
                    //{
                    //    Debug.Log("vIE:" + vIE);
                    //    Debug.Log("vIO:" + vOE);
                    //    Debug.Log("vI:" + vI);
                    //    Debug.Log("vO:" + vO);
                    //    print = false;
                    //}
                    var mI = (vI - v) / dxI; // 斜率
                    var mO = (vO - v) / dxO;
                    var rangeAN = newKey.frameIndex - a.frameIndex; // ab帧中间插入新帧n后，ab各自与新帧的距离（范围）n = new，r =range
                    var rangeBN = b.frameIndex - newKey.frameIndex;
                    var r = b.frameIndex - a.frameIndex;
                    var oldLenA = a.outTangent.magnitude;
                    var norLenA = oldLenA / r; // 旧长度与旧范围ab的比例
                    var newLenA = norLenA * rangeAN;
                    var oldLenB = b.inTangent.magnitude;
                    var norLenB = oldLenB / r;
                    var newLenB = norLenB * rangeBN;
                    var x = r * 0.25f; // 以中点作为临时切点，然后再根据旧的比例调整长度
                    var xI = -x;
                    var xO = +x;
                    newKey.inTangent = new Vector2(xI, xI * mI);
                    newKey.outTangent = new Vector2(xO, xO * mO);
                    if (a.outMode == CurveMode.None)
                    {
                        a.outMode = CurveMode.Bezier;
                        a.outTangent = Vector2.Lerp(a, newKey, 0.3333333f);
                    }
                    else if (a.outMode == CurveMode.Bezier)//更改左边帧的OutTan
                    {
                        newKey.inTangent = newKey.inTangent.normalized * norLenB * rangeAN;
                        newKey.inMode = CurveMode.Bezier;
                        a.outTangent = a.outTangent.normalized * newLenA;
                    }
                    if (b.inMode == CurveMode.None)
                    {
                        b.inMode = CurveMode.Bezier;
                        b.inTangent = Vector2.Lerp(b, newKey, 0.3333333f);
                    }
                    else if (b.inMode == CurveMode.Bezier) //更改右边帧的InTan
                    {
                        newKey.outTangent = newKey.outTangent.normalized * norLenA * rangeBN;
                        newKey.outMode = CurveMode.Bezier;
                        b.inTangent = b.inTangent.normalized * newLenB;
                    }
                }
                keys.Insert(i, newKey);//插入帧到对应时间
                return;
            }
        }
        { // 插入到末尾
            var a = keys[keys.Count - 1];
            if (a.outMode == CurveMode.None)
            {
                a.outMode = CurveMode.Bezier;
                a.outTangentAbs = Vector2.Lerp(a, newKey, 0.3333333f);
            }
            newKey.inMode = CurveMode.Bezier;
            newKey.inTangentAbs = Vector2.Lerp(newKey, a, 0.3333333f);
            keys.Add(newKey);
        }
    }
    public float Evaluate(float indexFloat)
    {
        if (!hasKey) return 0;
        if (indexFloat >= keys[keys.Count - 1].frameIndex || keys.Count == 1)
        {
            // 只有一帧，或者时间大于最后一帧时，使用最后一帧的值
            return keys[keys.Count - 1].value;
        }
        if (Approx(indexFloat, keys[0].frameIndex) || indexFloat < keys[0].frameIndex)
        {
            return keys[0].value;
        }
        for (int i = 1; i < keys.Count; i++)
        {
            if (Approx(indexFloat, keys[i].frameIndex))
            {
                return keys[i].value;
            }
            if (indexFloat < keys[i].frameIndex)
            {
                return EvaluateInner(keys[i - 1], keys[i], indexFloat);
            }
        }
        throw null;
    }
    private float EvaluateInner(ASKey k1, ASKey k2, float indexFloat)
    {
        var t = (indexFloat - k1.frameIndex) / (k2.frameIndex - k1.frameIndex);
        if (k1.outMode == CurveMode.Constant)
        {
            return k1.value;
        }
        else if (k2.inMode == CurveMode.Constant)
        {
            return k2.value;
        }
        List<Vector2> p = new List<Vector2>();
        p.Add(k1);//p0
        switch (k1.outMode)//p1
        {
            case CurveMode.Bezier: p.Add(k1.outTangentAbs); break;
            case CurveMode.Linear: p.Add(Vector2.Lerp(k1, k2, 0.3333333f)); break;
            case CurveMode.None: break;
            default: throw null;
        }
        switch (k2.inMode)//p2
        {
            case CurveMode.Bezier: p.Add(k2.inTangentAbs); break;
            case CurveMode.Linear: p.Add(Vector2.Lerp(k1, k2, 0.6666667f)); break;
            case CurveMode.None: break;
            default: throw null;
        }
        p.Add(k2);//p3
        var v = CalBezier(p.ToArray(), t);
        return v.y;
    }
    #region MyRegion
    //public float EvaluateRT(float realTime)
    //{
    //    return EvaluateRT(realTime, UITimeLine.Fps);
    //}
    //// if: fps = 60;
    //// so: timePerFrame = s/fps = 1/60 = 0.0166667
    //public float EvaluateRT(float realTime, float fps)
    //{
    //    var timePerFrame = 1 / fps;
    //    if (realTime >= keys[keys.Count - 1].frameIndex * timePerFrame || keys.Count == 1)
    //    {
    //        // 只有一帧，或者时间大于最后一帧时，使用最后一帧的值
    //        return keys[keys.Count - 1].value;
    //    }
    //    for (int i = 1; i < keys.Count; i++)
    //    {
    //        if (realTime < keys[i].frameIndex * timePerFrame)
    //        {
    //            var a = keys[i - 1];
    //            var b = keys[i];
    //            var t1 = a.frameIndex * timePerFrame;
    //            var t2 = b.frameIndex * timePerFrame;
    //            var t = (realTime - t1) / (t2 - t1);
    //            var v = EvaluateInner(a, b, t, fps);
    //            return Mathf.Lerp(keys[i - 1].value, keys[i].value, t);
    //        }
    //    }
    //    throw null;
    //}

    //private float EvaluateInner(ASKey k1, ASKey k2, float t, float fps)
    //{
    //    var a = new Vector2(k1.frameIndex * fps, k1.value);
    //    var b = new Vector2(k2.frameIndex * fps, k2.value);
    //    if (k1.outMode == CurveMode.Constant)
    //    {
    //        return b.y;
    //    }
    //    else if (k2.inMode == CurveMode.Constant)
    //    {
    //        return a.y;
    //    }
    //    List<Vector2> p = new List<Vector2>();
    //    p.Add(a);//p0
    //    switch (k1.outMode)//p1
    //    {
    //        case CurveMode.Bezier: p.Add(k1.outTangent); break;
    //        case CurveMode.Linear: p.Add(Vector2.Lerp(a, b, 0.3333333f)); break;
    //        case CurveMode.None: break;
    //        default: throw null;
    //    }
    //    switch (k2.inMode)//p2
    //    {
    //        case CurveMode.Bezier: p.Add(k2.inTangent); break;
    //        case CurveMode.Linear: p.Add(Vector2.Lerp(a, b, 0.6666667f)); break;
    //        case CurveMode.None: break;
    //        default: throw null;
    //    }
    //    p.Add(a);//p3
    //    var v = CalBezier(p.ToArray(), t);
    //    return v.y;
    //} 
    #endregion
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
            case 2:
                v = Vector2.Lerp(p[0], p[1], t);
                return v;
            default:
                throw null;
        }
    }
    internal void MoveKey(int indO, int ind, float v)
    {
        var i = IndexOf(indO);
        if (i > -1)
        {
            var k = keys[IndexOf(indO)];
            var inT = k.inTangent - k;
            var outT = k.outTangent - k;
            RemoveKey(indO);
            var n = new ASKey(ind, v);
            n.inTangent = n + inT;
            n.outTangent = n + outT;
            InsertKey(n);
        }
        else InsertKey(ind, v);
    }
    public void SetIndex(int i, int frameIndex)
    {
        keys[i].frameIndex = frameIndex;
        keys.Sort(SortList);
    }
    private int SortList(ASKey a, ASKey b)
    {
        if (a.frameIndex > b.frameIndex) { return 1; }
        else if (a.frameIndex < b.frameIndex) { return -1; }
        return 0;
    }
}
