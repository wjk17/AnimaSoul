using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
[Serializable]
public class Coordinate
{
    /// 本地坐标轴
    public Vector3 up;
    public Vector3 forward;/// 本地坐标轴
    public Vector3 right; /// 本地坐标轴

    //初始姿势euler（0,0,0），不保存，程序开始时获取
    [XmlIgnore] public Quaternion origin;
    [XmlIgnore] public Vector3 originPos;
    public Coordinate() { }
    public Coordinate World(Transform t, Vector3 euler)
    {
        var n = new Coordinate(this);
        n *= Quaternion.AngleAxis(euler.y, n.up);
        n *= Quaternion.AngleAxis(euler.x, n.right);
        n *= Quaternion.AngleAxis(euler.z, n.forward);
        var p = t.parent;
        if (p != null)
        {
            n.up = p.TransformDirection(n.up);
            n.right = p.TransformDirection(n.right);
            n.forward = p.TransformDirection(n.forward);
        }
        return n;
    }
    public void DrawRay(Transform t, Vector3 euler, float length, bool depthTest = false)
    {
        var n = World(t, euler);
        var origin = Gizmos.color;
        Debug.DrawRay(t.position, n.forward * length, Color.blue, 0, depthTest);
        Debug.DrawRay(t.position, n.right * length, Color.red, 0, depthTest);
        Debug.DrawRay(t.position, n.up * length, Color.green, 0, depthTest);
    }
    public Coordinate(Coordinate c)
    {
        up = c.up;
        forward = c.forward;
        right = c.right;
        origin = c.origin;
        originPos = c.originPos;
    }
    public Coordinate(Transform t)
    {
        var p = t.parent;
        // 将3条坐标轴转为本地
        up = p != null ? p.InverseTransformDirection(t.up) : t.up;
        forward = p != null ? p.InverseTransformDirection(t.forward) : t.forward;
        right = p != null ? p.InverseTransformDirection(t.right) : t.right;
        origin = t.localRotation;
        originPos = t.localPosition;
    }
    public Quaternion Rotate(Vector3 euler)
    {
        var n = new Coordinate(this);
        Quaternion result = origin;
        Quaternion rot;

        rot = Quaternion.AngleAxis(euler.z, n.forward);
        result = rot * result;
        n *= rot;

        rot = Quaternion.AngleAxis(euler.x, n.right);
        result = rot * result;
        n *= rot;

        rot = Quaternion.AngleAxis(euler.y, n.up);
        result = rot * result;

        if (MathTool.IsNaN(result))
        {
            Debug.LogError("Nan");
        }

        return result;
    }
    public static Coordinate operator *(Coordinate coord, Quaternion rot)
    {
        coord.up = rot * coord.up;
        coord.forward = rot * coord.forward;
        coord.right = rot * coord.right;
        return coord;
    }
}
