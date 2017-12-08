using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class ASClip
{
    public List<ASObjectCurve> curves;
    public ASClip() { curves = new List<ASObjectCurve>(); }
    public ASObjectCurve this[Transform t]
    {
        get
        {
            return GetCurve(t);
        }
    }
    public ASObjectCurve this[string name]
    {
        get
        {
            return GetCurve(name);
        }
    }
    public ASObjectCurve GetCurve(Transform t)
    {
        foreach (var curve in curves)
        {
            if (curve.trans == t)
            {
                return curve;
            }
        }
        throw null;
    }
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
    public int IndexOf(Transform trans)
    {
        for (int i = 0; i < curves.Count; i++)
        {
            if (curves[i].trans == trans) return i;
        }
        return -1;
    }
    public void AddCurve(Transform tran)
    {
        if (IndexOf(tran) != -1) throw null;
        curves.Add(new ASObjectCurve(tran));
    }
    public bool HasKey(ASObjectCurve curve, int frameIndex)
    {
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
    public void AddEulerPos(ASObjectCurve curve, int frameIndex, Vector3 euler, Vector3 pos)
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
    public void RemoveKey(ASObjectCurve curve, int frameIndex)
    {
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
