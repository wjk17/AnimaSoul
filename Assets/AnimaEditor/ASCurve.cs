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
        eulerAngles = new ASCurve[3] { new ASCurve(), new ASCurve() , new ASCurve() };
        localPosition = new ASCurve[3] { new ASCurve(), new ASCurve(), new ASCurve() };
    }
    public ASObjectCurve(Transform trans):this()
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
[Serializable]
public class ASKey
{
    public int index;
    public float value;
    public float InTangent;
    public float OutTangent;
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
    // if: fps = 60;
    // so: timePerFrame = s/fps = 1/60 = 0.0166667
    public float Evaluate(float realTime, float fps)
    {
        float timePerFrame = 1 / fps;
        if (realTime >= keys[keys.Count - 1].index * timePerFrame || keys.Count == 1)
        {
            return keys[keys.Count - 1].value;
        }
        for (int i = 1; i < keys.Count; i++)
        {
            if (realTime < keys[i].index * timePerFrame)
            {
                var a = keys[i - 1].index * timePerFrame;
                var b = keys[i].index * timePerFrame;
                var t = (realTime - a) / (b - a);
                return Mathf.Lerp(keys[i - 1].value, keys[i].value, t);
            }
        }
        throw new Exception("意外");
    }
}
