using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class ASObjectCurve
{
    public string name;
    public Transform trans;
    public ASCurve[] eulerAngles;
    public ASCurve[] localPosition;
    public ASObjectCurve()
    {
        eulerAngles = new ASCurve[3];
        localPosition = new ASCurve[3];
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
[Serializable]
public class ASKey
{
    public float time;
    public float value;
    public ASKey() { }
    public ASKey(float time, float value)
    {
        this.time = time;
        this.value = value;
    }
}
[Serializable]
public class ASCurve
{
    public List<ASKey> keys;
    public ASCurve() { keys = new List<ASKey>(); }
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
    public void InsertKey(float time, float value)
    {
        InsertKey(new ASKey(time, value));
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
    public float Evaluate(float realTime)
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
                return Mathf.Lerp(keys[i - 1].value, keys[i].value, t);
            }
        }
        throw new Exception("意外");
    }
}
