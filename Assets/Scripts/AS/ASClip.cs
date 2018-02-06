using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
[Serializable]
public class ASClip
{
    [XmlIgnore]
    public string clipName;
    [XmlIgnore]
    public Vector2Int frameRange;

    public List<ASObjectCurve> curves;
    public ASClip() { curves = new List<ASObjectCurve>(); }
    public ASClip(string clipName) { this.clipName = clipName; curves = new List<ASObjectCurve>(); }
    public ASObjectCurve this[ASTransDOF t]
    {
        get
        {
            return GetCurve(t);
        }
    }
    //public ASObjectCurve this[Transform t]
    //{
    //    get
    //    {
    //        return GetCurve(t);
    //    }
    //}
    public ASObjectCurve this[string name]
    {
        get
        {
            return GetCurve(name);
        }
    }
    public ASObjectCurve GetCurve(ASTransDOF ast)
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
    //public ASObjectCurve GetCurve(Transform t)
    //{
    //    foreach (var curve in curves)
    //    {
    //        if (curve.trans == t)
    //        {
    //            return curve;
    //        }
    //    }
    //    //throw null;
    //    return null;
    //}
    public ASObjectCurve GetCurve(string name)
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
    public int IndexOf(ASTransDOF ast)
    {
        for (int i = 0; i < curves.Count; i++)
        {
            if (curves[i].ast == ast) return i;
        }
        return -1;
    }
    //public int IndexOf(Transform trans)
    //{
    //    for (int i = 0; i < curves.Count; i++)
    //    {
    //        if (curves[i].trans == trans) return i;
    //    }
    //    return -1;
    //}
    public void AddCurve(ASTransDOF ast)
    {
        if (IndexOf(ast) != -1) throw null;
        curves.Add(new ASObjectCurve(ast));
    }
    //public void AddCurve(Transform tran)
    //{
    //    if (IndexOf(tran) != -1) throw null;
    //    curves.Add(new ASObjectCurve(tran));
    //}
    public bool HasKey(ASObjectCurve curve, int frameIndex)
    {
        if (curve == null) return false;
        foreach (var c in curve.eulerAngles)
        {
            if (c.IndexOf(frameIndex) > -1) return true;
        }
        foreach (var c in curve.localPosition)
        {
            if (c.IndexOf(frameIndex) > -1) return true;
        }
        return false;
    }
    public void AddEulerPosWithLerp(ASObjectCurve curve, int frameIndex, Vector3 euler, Vector3 pos)
    {
        //ASCurve.print = true;
        if (curve.timeCurve.keys.Count < 2) curve.timeCurve.InsertKey(frameIndex, 0);// UITimeLine.FrameValue);
        else curve.timeCurve.InsertKey(frameIndex, curve.timeCurve.Evaluate(UITimeLine.FrameIndex));
        curve.eulerAngles[0].InsertKey(frameIndex, euler.x);
        curve.eulerAngles[1].InsertKey(frameIndex, euler.y);
        curve.eulerAngles[2].InsertKey(frameIndex, euler.z);
        curve.localPosition[0].InsertKey(frameIndex, pos.x);
        curve.localPosition[1].InsertKey(frameIndex, pos.y);
        curve.localPosition[2].InsertKey(frameIndex, pos.z);
    }
    public void AddEulerPos(ASObjectCurve curve, int frameIndex, Vector3 euler, Vector3 pos)
    {
        curve.timeCurve.InsertKey(frameIndex, 0);
        curve.eulerAngles[0].InsertKey(frameIndex, euler.x);
        curve.eulerAngles[1].InsertKey(frameIndex, euler.y);
        curve.eulerAngles[2].InsertKey(frameIndex, euler.z);
        curve.localPosition[0].InsertKey(frameIndex, pos.x);
        curve.localPosition[1].InsertKey(frameIndex, pos.y);
        curve.localPosition[2].InsertKey(frameIndex, pos.z);
    }
    public void AddEulerPosAllCurve(int frameIndex)
    {
        foreach (var curve in UIClip.clip.curves)
        {
            curve.timeCurve.InsertKey(frameIndex, 0);
            curve.eulerAngles[0].InsertKey(frameIndex, curve.ast.euler.x);
            curve.eulerAngles[1].InsertKey(frameIndex, curve.ast.euler.y);
            curve.eulerAngles[2].InsertKey(frameIndex, curve.ast.euler.z);
            curve.localPosition[0].InsertKey(frameIndex, curve.ast.transform.localPosition.x);
            curve.localPosition[1].InsertKey(frameIndex, curve.ast.transform.localPosition.y);
            curve.localPosition[2].InsertKey(frameIndex, curve.ast.transform.localPosition.z);
        }
    }
    public void AddEulerPosOrigin(ASObjectCurve curve, int frameIndex, Vector3 euler, Vector3 pos)
    {
        curve.timeCurve.InsertKeyOrigin(frameIndex, 0);
        //if (curve.timeCurve.keys.Count < 2) curve.timeCurve.InsertKeyOrigin(frameIndex, 0);// UITimeLine.FrameValue);
        //else curve.timeCurve.InsertKeyOrigin(frameIndex, curve.timeCurve.Evaluate(UITimeLine.FrameIndex));
        curve.eulerAngles[0].InsertKeyOrigin(frameIndex, euler.x);
        curve.eulerAngles[1].InsertKeyOrigin(frameIndex, euler.y);
        curve.eulerAngles[2].InsertKeyOrigin(frameIndex, euler.z);
        curve.localPosition[0].InsertKeyOrigin(frameIndex, pos.x);
        curve.localPosition[1].InsertKeyOrigin(frameIndex, pos.y);
        curve.localPosition[2].InsertKeyOrigin(frameIndex, pos.z);
    }
    public void RemoveKey(ASObjectCurve curve, int frameIndex)
    {
        curve.timeCurve.RemoveKey(frameIndex);
        foreach (var c in curve.eulerAngles)
        {
            c.RemoveKey(frameIndex);
        }
        foreach (var c in curve.localPosition)
        {
            c.RemoveKey(frameIndex);
        }
        curve.timeCurve.RemoveKey(frameIndex);
    }
}
