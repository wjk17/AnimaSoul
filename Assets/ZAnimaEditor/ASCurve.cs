using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ASObjectCurves
{
    public string name;
    public Transform trans;
    public ASCurve[] eulerAngles;
    public ASCurve[] localPosition;
    public ASObjectCurves()
    {
        eulerAngles = new ASCurve[3];
        localPosition = new ASCurve[3];
    }
    public ASObjectCurves Clone()
    {
        var n = new ASObjectCurves();
        n.name = name;
        n.eulerAngles = eulerAngles.Clone() as ASCurve[];
        n.localPosition = localPosition.Clone() as ASCurve[];
        return n;
    }
}
[Serializable]
public class ASKey
{
    public float time;
    public Vector3 value;
}
[Serializable]
public class ASCurve
{
    public bool hasKey
    {
        get { return keys != null && keys.Count > 0; }
    }
    public List<ASKey> keys;
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
    public void InsertKey(ASKey newKey) // 根据时间顺序插入。已有此帧则修改值
    {
        if (!hasKey)
        {
            keys = new List<ASKey>();
            keys.Add(newKey);
            return;
        }
        if (newKey.time == keys[0].time)
        {
            keys[0].value = newKey.value;//第一帧
            return;
        }
        for (int i = 1; i < keys.Count; i++)
        {
            if (newKey.time == keys[i].time)
            {
                keys[i].value = newKey.value;//覆盖帧（修改值）
                return;
            }
            if (newKey.time < keys[i].time)
            {
                keys.Insert(i, newKey);//插入帧到对应时间
                return;
            }
        }
        keys.Add(newKey);//插入到末尾
    }
    public Vector3 Evaluate(float realTime)
    {
        if (realTime >= keys[keys.Count - 1].time || keys.Count == 1)
        {
            return keys[keys.Count - 1].value;
        }
        for (int i = 1; i < keys.Count; i++)
        {
            if (realTime < keys[i].time)
            {
                var a = keys[i - 1].time;
                var b = keys[i].time;
                var t = (realTime - a) / (b - a);
                return Vector3.Lerp(keys[i - 1].value, keys[i].value, t);
            }
        }
        throw new Exception("意外");
    }
}
