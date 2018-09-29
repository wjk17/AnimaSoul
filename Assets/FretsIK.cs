using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(FretsIK))]
public class FretsIKEditor : E_ShowButtons<FretsIK> { }
#endif
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
    public int iter;
    public TransDOF astIK;

    public float alpha = 0.1f; // 逼近的步长
    public float theta0;
    public float theta1;

    public int jointIterCount = 10;
    public int axisIterCount = 20;

    public float minValue = -45;

    [HideInInspector] [ShowToggle] public bool solveOn = false;

    Avator avatar { get { if (_avatar == null) _avatar = UIDOFEditor.I.avatar; return _avatar; } }
    Avator _avatar;
    List<Bone> joints;

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
    [ShowButton]
    void Solve()
    {
        SetBones();
        IKSolve(target.position, joints.ToArray());
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
        Gizmos.color = gizmosColor;
        Gizmos.DrawWireSphere(target.position, gizmosRadius);
    }

    public bool Iteration(Vector3 targetPos)
    {
        var dict = new SortedDictionary<float, float>();
        theta0 = GetIterValue();
        dict.Add(Dist(targetPos), GetIterValue()); // 计算当前距离并存放到字典里，以距离作为Key排序

        SetIterValue(theta0 + alpha); // 计算向前步进后的距离，放进字典
        var dist = Dist(targetPos);
        if (!dict.ContainsKey(dist)) dict.Add(dist, GetIterValue()); //（字典里的值是被DOF限制后的）

        SetIterValue(theta0 - alpha); // 反向
        dist = Dist(targetPos);
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
    float Dist(Vector3 targetPos)
    {
        var endDir = end.position - astIK.transform.position; // 当前终端方向与目标方向的距离
        var targetDir = targetPos - astIK.transform.position;
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
    private void IKSolve(Vector3 targetPos, params Bone[] bones)
    {
        var joints = new List<TransDOF>();
        foreach (var bone in bones)
        {
            joints.Add(avatar[bone]);
        }
        IKSolve(targetPos, joints.ToArray());
    }
    private void IKSolve(params TransDOF[] joints)
    {
        IKSolve(target.position, joints);
    }
    private void IKSolve(Vector3 targetPos, params TransDOF[] joints)
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
                    if (Iteration(targetPos) || c2 <= 0)
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
