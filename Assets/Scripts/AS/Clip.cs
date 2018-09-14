using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
[Serializable]
public class Clip
{
    [XmlIgnore] public string clipName;
    [XmlIgnore] public Vector2Int frameRange;

    public List<CurveObj> curves;
    public Clip() { curves = new List<CurveObj>(); }
    public Clip(string clipName) { this.clipName = clipName; curves = new List<CurveObj>(); }
    public CurveObj this[TransDOF t]
    {
        get
        {
            return GetCurve(t);
        }
    }
    public CurveObj this[string name]
    {
        get
        {
            return GetCurve(name);
        }
    }
    public CurveObj GetCurve(TransDOF ast)
    {
        foreach (var curve in curves)
        {
            if (curve.ast == ast)
            {
                return curve;
            }
        }
        //throw null;
        return null;
    }
    public CurveObj GetCurve(string name)
    {
        foreach (var curve in curves)
        {
            if (curve.name == name)
            {
                return curve;
            }
        }
        throw null;
    }
    public int IndexOf(TransDOF ast)
    {
        for (int i = 0; i < curves.Count; i++)
        {
            if (curves[i].ast == ast) return i;
        }
        return -1;
    }
    public void AddCurve(TransDOF ast)
    {
        if (IndexOf(ast) != -1) throw null;
        curves.Add(new CurveObj(ast));
    }

    //public bool HasKey(CurveObj curve, int frameIndex)
    //{
    //    if (curve == null) return false;
    //    foreach (var c in curve.rots)
    //    {
    //        if (c.IndexOf(frameIndex) > -1) return true;
    //    }
    //    foreach (var c in curve.poss)
    //    {
    //        if (c.IndexOf(frameIndex) > -1) return true;
    //    }
    //    return false;
    //}
    //public void AddEulerPosWithLerp(CurveObj curve, int frameIndex, Vector3 euler, Vector3 pos)
    //{
    //    if (curve.timeCurve.keys.Count < 2) curve.timeCurve.InsertKey(frameIndex, 0);// UITimeLine.FrameValue);
    //    else curve.timeCurve.InsertKey(frameIndex, curve.timeCurve.Evaluate(UITimeLine.FrameIndex));
    //    curve.rot.x.Add(new Key2(frameIndex, euler.x));
    //    curve.rot.y.Add(new Key2(frameIndex, euler.y));
    //    curve.rot.z.Add(new Key2(frameIndex, euler.z));
    //    curve.poss[0].InsertKey(frameIndex, pos.x);
    //    curve.poss[1].InsertKey(frameIndex, pos.y);
    //    curve.poss[2].InsertKey(frameIndex, pos.z);
    //}
    public void AddEulerPosAllCurve(int frameIndex)
    {
        var c = 0;
        foreach (var curve in UIClip.clip.curves)
        {
            if (curve.ast == null) continue;
            var pos = curve.ast.transform.localPosition;
            var os = curve.ast.coord.originPos;
            curve.AddEulerPos(UITimeLine.I.frameIndex, curve.ast.euler, pos - os);
            c++;
        }
        Debug.Log("插入到 " + c.ToString() + " 条曲线");
    }
    //public void AddEulerPosOrigin(CurveObj curve, int frameIndex, Vector3 euler, Vector3 pos)
    //{
    //    curve.timeCurve.InsertKeyOrigin(frameIndex, 0);
    //    //if (curve.timeCurve.keys.Count < 2) curve.timeCurve.InsertKeyOrigin(frameIndex, 0);// UITimeLine.FrameValue);
    //    //else curve.timeCurve.InsertKeyOrigin(frameIndex, curve.timeCurve.Evaluate(UITimeLine.FrameIndex));
    //    curve.rot[0].InsertKeyOrigin(frameIndex, euler.x);
    //    curve.rot[1].InsertKeyOrigin(frameIndex, euler.y);
    //    curve.rot[2].InsertKeyOrigin(frameIndex, euler.z);
    //    curve.poss[0].InsertKeyOrigin(frameIndex, pos.x);
    //    curve.poss[1].InsertKeyOrigin(frameIndex, pos.y);
    //    curve.poss[2].InsertKeyOrigin(frameIndex, pos.z);
    //}
    //public void RemoveKey(CurveObj curve, int frameIndex)
    //{
    //    curve.timeCurve.RemoveKey(frameIndex);
    //    foreach (var c in curve.rot)
    //    {
    //        c.RemoveKey(frameIndex);
    //    }
    //    foreach (var c in curve.poss)
    //    {
    //        c.RemoveKey(frameIndex);
    //    }
    //    curve.timeCurve.RemoveKey(frameIndex);
    //}
}
