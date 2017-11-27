using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class IKBoneChain // IK骨骼链
{
    public List<Joint> joints;
    public Transform end;
    public Transform target;
    public int iteration; // 计算的迭代次数
    public List<WRay> Solve()
    {
        List<WRay> result = new List<WRay>();
        int i;
        for (i = 0; i < iteration; i++)
        {
            foreach (var joint in joints)
            {
                var ray = IKTool.IKSolve(joint, target, end);
                if (ray != null) result.Add(ray);
            }
            //if (IKTool.Close(target.position, end.position)) { Debug.Log(i.ToString() + " 次迭代   完成"); return; }
            if (IKTool.Close(target.position, end.position)) return result;
        }
        return result;
        //Debug.Log(i.ToString() + " 次迭代  未完成");
    }
    public WRay Solve(int i)
    {
        var result = IKTool.IKSolve(joints[i], target, end);
        if (IKTool.Close(target.position, end.position)) Debug.Log("完成");
        return result;
    }
}
public class WRay
{
    public Vector3 pos;
    public Vector3 dir;
    public Color color;
    public void Draw()
    {
        Debug.DrawRay(pos, dir, color);
    }
    public WRay(Vector3 pos, Vector3 dir, Color color)
    {
        this.pos = pos;
        this.dir = dir * 3f;
        this.color = color;
    }
}
public static class IKTool
{
    public static float closest = 0.0001f;
    public static RotSeq seq = RotSeq.zxy;
    public static bool Close(Vector3 a, Vector3 b)
    {
        return CloseZero((a - b).sqrMagnitude);
    }
    public static bool Close(float a, float b)
    {
        return Mathf.Approximately(a, b) || Mathf.Abs(a - b) <= closest;
    }
    public static bool CloseZero(float f)
    {
        return Mathf.Approximately(f, 0f) || float.IsNaN(f) || Mathf.Abs(f) <= closest;
    }
    /// <summary>
    /// 计算骨骼链
    /// </summary>
    /// <param name="关节"></param>
    /// <param name="目标位置"></param>
    /// <param name="末端效应器"></param>
    /// <param name="每次迭代最大旋转角度"></param>
    /// <param name="迭代次数"></param>
    public static WRay IKSolve(Joint joint, Transform target, Transform end)
    {
        var joi = joint.Clone();
        // 关节相对于目标位置和末端效应器的位置（世界坐标）
        Vector3 absJoint2End = end.position - joi.transform.position;
        Vector3 absJoint2Target = target.position - joi.transform.position;

        // 转为本地坐标
        Quaternion invRotation = Quaternion.Inverse(joi.transform.rotation);
        Vector3 localJoint2End = invRotation * absJoint2End;
        Vector3 localJoint2Target = invRotation * absJoint2Target;


        Vector3 rotateAxis = Vector3.Cross(localJoint2Target, localJoint2End).normalized;
        float deltaAngle = Vector3.Angle(localJoint2End.normalized, localJoint2Target.normalized);
        //float deltaAngle2 = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(localJoint2End.normalized, localJoint2Target.normalized));
        //float deltaAngle3 = Vector3.SignedAngle(localJoint2End.normalized, localJoint2Target.normalized, rotateAxis);

        if (CloseZero(deltaAngle)) return null;

        Quaternion deltaRotation = Quaternion.FromToRotation(localJoint2End.normalized, localJoint2Target.normalized);
        //Quaternion deltaRotation2 = Quaternion.Inverse(Quaternion.AngleAxis(deltaAngle2, rotateAxis));
        //Quaternion deltaRotation3 = Quaternion.AngleAxis(deltaAngle3, rotateAxis);

        //if (joint.name == "spine")
        //{
        //    var local = QuaternionTool.ToEuler(joi.transform.localRotation, seq);
        //    var xz = Quaternion.FromToRotation(localJoint2End.xz().normalized, localJoint2Target.xz().normalized);
        //    var y = QuaternionTool.ToEuler(xz, seq).y;
        //    local.y = Mathf.Clamp(local.y + y, joi.dof.twistMin, joi.dof.twistMax);

        //    var yz = Quaternion.FromToRotation(localJoint2End.yz().normalized, localJoint2Target.yz().normalized);
        //    var x = QuaternionTool.ToEuler(yz, seq).x;
        //    local.x = Mathf.Clamp(local.x + x, joi.dof.swingXMin, joi.dof.swingXMax);

        //    var xy = Quaternion.FromToRotation(localJoint2End.xy().normalized, localJoint2Target.xy().normalized);
        //    var z = QuaternionTool.ToEuler(xy, seq).z;
        //    local.z = Mathf.Clamp(local.z + z, joi.dof.swingZMin, joi.dof.swingZMax);
        //}

        joi.transform.localRotation *= deltaRotation;
        //var e = QuaternionTool.ToEuler(joi.transform.localRotation, seq);
        //e = DOFLiminator.LimitDOF(e, joint.dof);
        //joi.transform.localRotation = QuaternionTool.FromEuler(e, seq);

        //joi.transform.localRotation = QuaternionTool.FromEuler(local,seq);
        //var e = QuaternionTool.ToEuler(deltaRotation, seq);


        //e = DOFLiminator.LimitDOF(e, joint.dof);


        //var q = QuaternionTool.FromEuler(e, seq);
        //var q = QuaternionTool.FromEuler(e, RotSeq.zyx);
        //joi.transform.localRotation *= q;
        //joi.transform.localRotation *= deltaRotation;

        var ray = new WRay(joi.transform.position, rotateAxis, Color.green);
        return ray;
    }
    public static float limit(float angle, float max)
    {
        bool inverse = false;
        if (angle > 180)
        {
            angle = 360 - angle;
            inverse = true;
        }
        angle = Mathf.Clamp(angle, -max, max);
        return inverse ? -angle : angle;
    }
}

[Serializable]
public class Joint
{
    public RigidbodyConstraints constraints;
    public float maxDeltaAngle;
    public float maxAngle;
    public float minAngle;
    public Transform transform;
    public bool skipClip;
    public ASDOF dof;
    public string name;
    public Joint Clone()
    {
        return (Joint)MemberwiseClone();
    }
}
