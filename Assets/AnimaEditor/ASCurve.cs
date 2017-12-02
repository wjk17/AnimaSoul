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
    public ASCurve[] eulerAngles;
    public ASCurve[] localPosition;
    public ASObjectCurve()
    {
        eulerAngles = new ASCurve[3] { new ASCurve(), new ASCurve(), new ASCurve() };
        localPosition = new ASCurve[3] { new ASCurve(), new ASCurve(), new ASCurve() };
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
    public int index;
    public float time
    {
        get { return index * UITimeLine.timePerFrame; }
    }
    public float value;
    public Vector2 inTangent;
    public Vector2 inTangentKey
    {
        get { return new Vector2(inTangent.x * UITimeLine.Fps, inTangent.y); }
    }
    public Vector2 outTangent;
    public Vector2 outTangentKey
    {
        get { return new Vector2(outTangent.x * UITimeLine.Fps, outTangent.y); }
    }
    public CurveMode inMode;
    public CurveMode outMode;
    public Vector2 ToVector2(float fps)
    {
        return new Vector2(index * (1 / fps), value);
    }
    public Vector2 ToVector2(bool convert = false)
    {
        if (convert) return new Vector2(index * (1 / UITimeLine.Fps), value);
        else return new Vector2(index, value);
    }
    public ASKey() { }
    public ASKey(int index, float value)
    {
        this.index = index;
        this.value = value;
    }
}
[Serializable]
public class ASCurve
{
    public List<ASKey> keys;
    public ASCurve() { keys = new List<ASKey>(); }
    public int IndexOf(int index)
    {
        if (!hasKey) return -1;
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i].index == index) { return i; }
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
    public bool RemoveKey(int index)
    {
        var i = IndexOf(index);
        {
            if (i > -1) { keys.RemoveAt(i); return true; }
        }
        return false;
    }
    public void InsertKey(int index, float value)
    {
        InsertKey(new ASKey(index, value));
    }
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
            if (newKey.index == keys[i].index)
            {
                keys[i].value = newKey.value;//覆盖帧（修改值）
                return;
            }
            if (newKey.index < keys[i].index)
            {
                keys.Insert(i, newKey);//插入帧到对应时间
                return;
            }
        }
        keys.Add(newKey);//插入到末尾
    }
    public float approxRange = 0.00001f;
    bool Approx(float a, float b)
    {
        var r = Mathf.Abs(a - b) < approxRange;
        return r;
    }
    public float Evaluate(float indexFloat)
    {
        if (indexFloat >= keys[keys.Count - 1].index || keys.Count == 1)
        {
            // 只有一帧，或者时间大于最后一帧时，使用最后一帧的值
            return keys[keys.Count - 1].value;
        }
        if (Approx(indexFloat, keys[0].index) || indexFloat < keys[0].index)
        {
            return keys[0].value;
        }
        for (int i = 1; i < keys.Count; i++)
        {
            if (Approx(indexFloat, keys[i].index))
            {
                return keys[i].value;
            }
            if (indexFloat < keys[i].index)
            {
                return EvaluateInner(keys[i - 1], keys[i], indexFloat);
            }
        }
        throw null;
    }
    private float EvaluateInner(ASKey k1, ASKey k2, float indexFloat)
    {
        var t = (indexFloat - k1.index) / (k2.index - k1.index);
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
            case CurveMode.Bezier: p.Add(k1.outTangent); break;
            case CurveMode.Linear: p.Add(Vector2.Lerp(k1, k2, 0.3333333f)); break;
            case CurveMode.None: break;
            default: throw null;
        }
        switch (k2.inMode)//p2
        {
            case CurveMode.Bezier: p.Add(k2.inTangent); break;
            case CurveMode.Linear: p.Add(Vector2.Lerp(k1, k2, 0.6666667f)); break;
            case CurveMode.None: break;
            default: throw null;
        }
        p.Add(k2);//p3
        var v = CalBezier(p.ToArray(), t);
        return v.y;
    }
    public float EvaluateRT(float realTime)
    {
        return EvaluateRT(realTime, UITimeLine.Fps);
    }
    // if: fps = 60;
    // so: timePerFrame = s/fps = 1/60 = 0.0166667
    public float EvaluateRT(float realTime, float fps)
    {
        var timePerFrame = 1 / fps;
        if (realTime >= keys[keys.Count - 1].index * timePerFrame || keys.Count == 1)
        {
            // 只有一帧，或者时间大于最后一帧时，使用最后一帧的值
            return keys[keys.Count - 1].value;
        }
        for (int i = 1; i < keys.Count; i++)
        {
            if (realTime < keys[i].index * timePerFrame)
            {
                var a = keys[i - 1];
                var b = keys[i];
                var t1 = a.index * timePerFrame;
                var t2 = b.index * timePerFrame;
                var t = (realTime - t1) / (t2 - t1);
                var v = EvaluateInner(a, b, t, fps);
                return Mathf.Lerp(keys[i - 1].value, keys[i].value, t);
            }
        }
        throw null;
    }

    private float EvaluateInner(ASKey k1, ASKey k2, float t, float fps)
    {
        var a = new Vector2(k1.index * fps, k1.value);
        var b = new Vector2(k2.index * fps, k2.value);
        if (k1.outMode == CurveMode.Constant)
        {
            return b.y;
        }
        else if (k2.inMode == CurveMode.Constant)
        {
            return a.y;
        }
        List<Vector2> p = new List<Vector2>();
        p.Add(a);//p0
        switch (k1.outMode)//p1
        {
            case CurveMode.Bezier: p.Add(k1.outTangent); break;
            case CurveMode.Linear: p.Add(Vector2.Lerp(a, b, 0.3333333f)); break;
            case CurveMode.None: break;
            default: throw null;
        }
        switch (k2.inMode)//p2
        {
            case CurveMode.Bezier: p.Add(k2.inTangent); break;
            case CurveMode.Linear: p.Add(Vector2.Lerp(a, b, 0.6666667f)); break;
            case CurveMode.None: break;
            default: throw null;
        }
        p.Add(a);//p3
        var v = CalBezier(p.ToArray(), t);
        return v.y;
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
            case 2:
                v = Vector2.Lerp(p[0], p[1], t);
                return v;
            default:
                throw null;
        }
    }
}
