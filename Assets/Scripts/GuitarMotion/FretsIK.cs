using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Esa;
public enum Finger
{
    Thumb, Index, Middle, Ring, Pinky
}
[ExecuteInEditMode]
public class FretsIK : MonoBehaviour
{
    public FretsPos fretsPos;
    public int fretsCount = 22;
    public float gizmosRadius = 0.05f;
    public Color gizmosColor = Color.red;
    //Vector3 targetPos;
    [Range(0, 21)] public int fret;
    [Range(0, 5)] public int chord;
    [Range(0, 4)] public int finger;

    public Transform target;
    public Transform end;

    public int handFret;

    public Transform target2;
    public Transform end2;

    public Transform target3;
    public Transform end3;

    public int iter;
    public TransDOF astIK;

    public float alpha = 0.1f; // 逼近的步长
    public float theta0;
    public float theta1;

    public int jointIterCount = 10;
    public int axisIterCount = 20;

    public float minValue = -45;

    [HideInInspector] [ToggleAttribute] public bool solveOn = false;

    Avator avatar { get { if (_avatar == null) _avatar = UIDOFEditor.I.avatar; return _avatar; } }
    Avator _avatar;
    List<Bone> joints;

    int distIdx = 0;

    void Solve(int chord, int fret, int finger, int section)
    {
        //targetPos = fretsPos.frets[chord * fretsCount + fret].position;
        target = fretsPos.frets[chord * fretsCount + fret];
    }
    private void Update()
    {
        Solve(chord, fret, 0, 0);
        if (solveOn) Solve();
    }
    Bone[] ArmBones()
    {
        var bones = new List<Bone>();
        bones.Add(Bone.upperarm_l);
        bones.Add(Bone.forearm_l);
        bones.Add(Bone.hand_l);
        return bones.ToArray();
    }
    [Button]
    void Solve()
    {
        SetHand();
        SetBones();
        distIdx = 1; IKSolve(ArmBones());
        distIdx = 0; IKSolve(joints.ToArray());
    }
    public int hand = 2;
    private void SetHand()
    {
        while (Mathf.Abs(hand - fret) > 2)
        {
            if (hand > fret) hand--;
            else hand++;
        }
        target2 = fretsPos.hands1[hand];
        target3 = fretsPos.hands2[hand];
    }
    void SetBones()
    {
        joints = new List<Bone>();
        switch ((Finger)finger)
        {
            case Finger.Thumb:
                joints.Add(Bone.thumb3_l);
                joints.Add(Bone.thumb2_l);
                joints.Add(Bone.thumb1_l);
                break;
            case Finger.Index:
                joints.Add(Bone.index1_l);
                joints.Add(Bone.index2_l);
                joints.Add(Bone.index3_l);
                break;
            case Finger.Middle:
                joints.Add(Bone.middle1_l);
                joints.Add(Bone.middle2_l);
                joints.Add(Bone.middle3_l);
                break;
            case Finger.Ring:
                joints.Add(Bone.ring1_l);
                joints.Add(Bone.ring2_l);
                joints.Add(Bone.ring3_l);
                break;
            case Finger.Pinky:
                joints.Add(Bone.pinky1_l);
                joints.Add(Bone.pinky2_l);
                joints.Add(Bone.pinky3_l);
                break;
            default:
                break;
        }
        joints.Reverse();
        end = avatar[joints[0]].transform.GetChild(0);
    }
    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = gizmosColor;
            Gizmos.DrawWireSphere(target.position, gizmosRadius);
        }
    }

    public bool Iteration()
    {
        var dict = new SortedDictionary<float, float>();
        theta0 = GetIterValue();
        dict.Add(Dist(), GetIterValue()); // 计算当前距离并存放到字典里，以距离作为Key排序

        SetIterValue(theta0 + alpha); // 计算向前步进后的距离，放进字典
        var dist = Dist();
        if (!dict.ContainsKey(dist)) dict.Add(dist, GetIterValue()); //（字典里的值是被DOF限制后的）

        SetIterValue(theta0 - alpha); // 反向
        dist = Dist();
        if (!dict.ContainsKey(dist)) dict.Add(dist, GetIterValue());

        foreach (var i in dict)
        {
            SetIterValue(i.Value);
            astIK.Rotate();
            break;
        }
        theta1 = GetIterValue();
        return theta0.Approx(theta1); // 是否已经接近最佳值
    }
    float Dist()
    {
        return distIdx == 0 ? Dist1() : (Dist2() + Dist3());
    }
    float Dist1()
    {
        var endDir = end.position - astIK.transform.position; // 当前终端方向与目标方向的距离
        var targetDir = target.position - astIK.transform.position;
        var dist = Vector3.Distance(endDir, targetDir);
        return dist;
    }
    float Dist2()
    {
        var endDir = end2.position - astIK.transform.position;
        var targetDir = target2.position - astIK.transform.position;
        var dist = Vector3.Distance(endDir, targetDir);
        return dist;
    }
    float Dist3()
    {
        var endDir = end3.position - astIK.transform.position;
        var targetDir = target3.position - astIK.transform.position;
        var dist = Vector3.Distance(endDir, targetDir);
        return dist;
    }
    float GetIterValue()
    {
        switch (iter)
        {
            case 0: return astIK.euler.z;
            case 1: return astIK.euler.x;
            case 2: return astIK.euler.y;
            default: throw null;
        }
    }
    float LimitX(float value)
    {
        switch (astIK.dof.bone)
        {
            case Bone.thumb3_l:
            case Bone.index3_l:
            case Bone.middle3_l:
            case Bone.ring3_l:
            case Bone.pinky3_l:
                value = Mathf.Min(minValue, value); break;
        }
        return value;
    }
    void SetIterValue(float value)
    {
        switch (iter)
        {
            case 0: astIK.euler.z = value; break;
            case 1: astIK.euler.x = LimitX(value); break;
            case 2: astIK.euler.y = value; break;
            default: throw null;
        }
        astIK.Rotate();
    }
    private void IKSolve(params Bone[] bones)
    {
        var joints = new List<TransDOF>();
        foreach (var bone in bones)
        {
            joints.Add(avatar[bone]);
        }
        IKSolve(joints.ToArray());
    }
    private void IKSolve(params TransDOF[] joints)
    {
        //带DOF的IK思路：把欧拉旋转拆分为三个旋转分量，像迭代关节一样按照旋转顺序进行循环下降迭代。
        int c = jointIterCount;
        while (c > 0)
        {
            foreach (var joi in joints)
            {
                astIK = joi;
                iter = 0;
                int c2 = axisIterCount;
                while (true)
                {
                    if (Iteration() || c2 <= 0)
                    {
                        iter++;
                        if (iter > 2) break;
                    }
                    c2--;
                }
            }
            c--;
        }
    }
}
