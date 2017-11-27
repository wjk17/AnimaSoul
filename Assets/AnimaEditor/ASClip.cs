using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class ASClip
{
    public List<ASObjectCurve> curves;
    public void AddEulerPos(int frameIndex, Vector3 euler, Vector3 pos)
    {
        var time = frameIndex * 999;//??
        var bone = 999;
        curves[bone].eulerAngles[0].InsertKey(time, euler.z);
        curves[bone].eulerAngles[1].InsertKey(time, euler.x);
        curves[bone].eulerAngles[2].InsertKey(time, euler.y);
        curves[bone].localPosition[0].InsertKey(time, pos.z);
        curves[bone].localPosition[1].InsertKey(time, pos.x);
        curves[bone].localPosition[2].InsertKey(time, pos.y);
    }
    public void RemoveKey(int frameIndex)
    {
        throw new NotImplementedException();
    }
}
