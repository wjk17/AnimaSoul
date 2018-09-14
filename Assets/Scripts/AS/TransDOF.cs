using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
[Serializable]
public class TransDOF // 带关节限制（DOF）的变换。其实是一个可序列化的DeltaRotation Wrapper
{    
    public TransDOF() { }
    public TransDOF(DOF dof)
    {
        this.dof = dof;
    }
    [XmlIgnore] public Transform transform;
    [XmlIgnore] public Vector3 euler;

    public DOF dof;
    public Coordinate coord;
    public Vector3 right = new Vector3(1, 0, 0); // 用来转换坐标轴
    public Vector3 up = new Vector3(0, 1, 0);
    public Vector3 forward = new Vector3(0, 0, 1);
    public Vector3 upWorld
    {
        get
        {
            var v = up;
            if (v.x != 0)
            {
                v = v.x * transform.right;
            }
            else if (v.y != 0)
            {
                v = v.y * transform.up;
            }
            else // (v.z != 0)
            {
                v = v.z * transform.forward;
            }
            //Debug.Log(transform.up.ToString() + " vs " + v.ToString());
            //直接计算失败
            //var upW = transform.localRotation * transform.parent.TransformDirection(coord.up);
            return transform.up;
            //右边身体的up会翻转，所以失败
            //return v;
        }
    }
    public float swingX
    {
        set { euler.x = dof.swingXMin + value * rangeX; }
    }
    public float rangeX { get { return dof.swingXMax - dof.swingXMin; } }
    public float swingZ
    {
        set { euler.z = dof.swingZMin + value * rangeZ; }
    }
    public float rangeZ { get { return dof.swingZMax - dof.swingZMin; } }
    public float twistT
    {
        set { euler.x = dof.twistMin + value * rangeY; }
    }
    public float rangeY { get { return dof.twistMax - dof.twistMin; } }
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
        if (MathTool.IsNaN(euler))
        {
            Debug.LogError("Nan");
        }

        euler = DOFLiminator.LimitDOF(euler, dof);

        if (MathTool.IsNaN(euler))
        {
            Debug.LogError("Nan"); // 用于断点
        }

        var rot = coord.Rotate(euler);

        if (MathTool.IsNaN(rot))
        {
            Debug.LogError("Nan");
        }
        transform.localRotation = rot;
    }
    public void Update()
    {
        euler = MathTool.NaNTo0(euler);
        Rotate();
    }
}
