using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Esa;
public partial class UIDOFEditor
{
    float Dist(Vector3 targetPos)
    {
        var endDir = end.position - astIK.transform.position; // 当前终端方向与目标方向的距离
        var targetDir = targetPos - astIK.transform.position;
        var dist = Vector3.Distance(endDir, targetDir);
        return dist;
    }
    [Button]
    public bool Iteration()
    {
        return Iteration(target.position);
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
    void SetIterValue(float value)
    {
        switch (iter)
        {
            case 0: astIK.euler.z = value; break;
            case 1: astIK.euler.x = value; break;
            case 2: astIK.euler.y = value; break;
            default: throw null;
        }
        astIK.Rotate();
    }
    private void IKSolve(params Bone[] bones)
    {
        IKSolve(target.position, bones);
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
