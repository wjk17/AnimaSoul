using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
[Serializable]
public class ASTransDOF // 带关节限制（DOF）的变换。其实是一个可序列化的DeltaRotation Wrapper
{
    public ASTransDOF() { }
    public ASTransDOF(ASDOF dof)
    {
        this.dof = dof;
    }
    [XmlIgnore]
    public Transform transform;
    public ASDOF dof;
    public Coordinate coord;
    public Vector3 euler;
    public Vector3 right = new Vector3(1, 0, 0); // 用来转换坐标轴
    public Vector3 up = new Vector3(0, 1, 0);
    public Vector3 forward = new Vector3(0, 0, 1);
    Vector3 ToCoord(Coordinate c, Vector3 v)
    {
        if (v.x != 0)
        {
            return v.x * c.right;
        }
        else if (v.y != 0)
        {
            return v.y * c.up;
        }
        else// if (v.z != 0)
        {
            return v.z * c.forward;
        }
    }
    public void UpdateCoord()
    {
        transform.localRotation = coord.origin;
        Init();
    }
    public void Init()
    {
        var n = new Coordinate(transform);
        coord = new Coordinate(n);
        coord.right = ToCoord(n, right);
        coord.up = ToCoord(n, up);
        coord.forward = ToCoord(n, forward);
    }
    public void Rotate()
    {
        euler = DOFLiminator.LimitDOF(euler, dof);
        transform.localRotation = coord.Rotate(euler);
    }
    public void Update()
    {
        Rotate();
    }
}
[Serializable]
public class Coordinate
{
    public Vector3 up;
    public Vector3 forward;
    public Vector3 right;
    [XmlIgnore]//初始姿势euler（0,0,0），不保存，程序开始时获取
    public Quaternion origin;
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
    }
    public Coordinate(Transform t)
    {
        var p = t.parent;
        // 将3条坐标轴转为本地
        up = p != null ? p.InverseTransformDirection(t.up) : t.up;
        forward = p != null ? p.InverseTransformDirection(t.forward) : t.forward;
        right = p != null ? p.InverseTransformDirection(t.right) : t.right;
        origin = t.localRotation;
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
