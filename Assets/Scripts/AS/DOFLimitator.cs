using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class DOFLiminator
{
    private static DOF finger(int index)
    {
        DOF dof;
        switch (index)
        {
            case 0: dof = DOF.Ball(-10, +10, -80, 30); break;//第一截手指
            case 1: dof = DOF.Hinge(-100, 30); break;//第二截手指
            case 2: dof = DOF.Hinge(-80, 30); break;//第三届（末尾）手指
            default: throw new Exception();
        }
        return dof;
    }
    public static DOF HumanDOF(Bone key)
    {
        return HumanDOF(key, false);
    }
    static void AddDOF(Bone zb, bool mirror = false)
    {
        var dof = HumanDOF(zb);
        dof.bone = zb;
        dofs.Add(dof);
        if (mirror)
        {
            AddDOF(zb + 1);
        }
    }
    static List<DOF> dofs;
    public static List<DOF> DefaultHumanDOF()
    {
        dofs = new List<DOF>();
        //第三截手指
        AddDOF(Bone.thumb3_l,true);
        AddDOF(Bone.index3_l, true);
        AddDOF(Bone.middle3_l, true);
        AddDOF(Bone.ring3_l, true);
        AddDOF(Bone.pinky3_l, true);
        //第二截手指
        AddDOF(Bone.thumb2_l, true);
        AddDOF(Bone.index2_l, true);
        AddDOF(Bone.middle2_l, true);
        AddDOF(Bone.ring2_l, true);
        AddDOF(Bone.pinky2_l, true);
        //第一截手指
        AddDOF(Bone.thumb1_l, true);
        AddDOF(Bone.index1_l, true);
        AddDOF(Bone.middle1_l, true);
        AddDOF(Bone.ring1_l, true);
        AddDOF(Bone.pinky1_l, true);
        //掌骨（固定）
        AddDOF(Bone.palm1_l, true);
        AddDOF(Bone.palm2_l, true);
        AddDOF(Bone.palm3_l, true);
        AddDOF(Bone.palm4_l, true);
        //四肢
        AddDOF(Bone.hand_l, true);
        AddDOF(Bone.forearm_l, true);
        AddDOF(Bone.upperarm_l, true);
        AddDOF(Bone.shoulder_l, true);
        AddDOF(Bone.head);
        AddDOF(Bone.neck);
        AddDOF(Bone.chest);
        AddDOF(Bone.spine);
        AddDOF(Bone.heel2_l, true);
        AddDOF(Bone.heel1_l, true);
        AddDOF(Bone.toe_l, true);
        AddDOF(Bone.foot_l, true);
        AddDOF(Bone.shin_l, true);
        AddDOF(Bone.thigh_l, true);
        AddDOF(Bone.hips);
        AddDOF(Bone.root);
        AddDOF(Bone.other);
        return dofs;
    }
    private static DOF HumanDOF(Bone key, bool mirror)
    {
        DOF dof;
        switch (key)
        {
            //第三截手指
            case Bone.thumb3_l:
            case Bone.index3_l:
            case Bone.middle3_l:
            case Bone.ring3_l:
            case Bone.pinky3_l: dof = finger(2); break;
            //第二截手指
            case Bone.thumb2_l: dof = DOF.Ball(-20, +20, -80, +40); break;
            case Bone.index2_l:
            case Bone.middle2_l:
            case Bone.ring2_l:
            case Bone.pinky2_l: dof = finger(1); break;
            //第一截手指
            case Bone.thumb1_l: dof = DOF.Ball(-15, +15, -30, +15); break;
            case Bone.index1_l: dof = DOF.Hinge(-100, 0); break;
            case Bone.middle1_l:
            case Bone.ring1_l:
            case Bone.pinky1_l: dof = finger(0); break;
            //掌骨（固定）
            case Bone.palm1_l:
            case Bone.palm2_l:
            case Bone.palm3_l:
            case Bone.palm4_l: dof = new DOF(); break;
            //四肢
            case Bone.hand_l: dof = DOF.Ball(-25, +55, -90, +90); break;
            case Bone.forearm_l: dof = DOF.Hinge2D(-150, +0, -145, +10); break;
            case Bone.upperarm_l: dof = DOF.Ball3D(-140, +40, -135, +90, -90, +90); break;
            case Bone.shoulder_l: dof = DOF.Ball(-20, +20, -20, +20); break;
            case Bone.head: dof = DOF.Hinge(-35, +40); break;
            case Bone.neck: dof = DOF.Ball3D(-55, +55, -50, +60, -70, +70); break;
            case Bone.chest: dof = DOF.Ball3D(-25, +25, -15, +40, -40, +40); break;
            case Bone.spine: dof = DOF.Ball3D(-25, +25, -15, +40, -40, +40); break;
            case Bone.heel2_l: dof = new DOF(); break;
            case Bone.heel1_l: dof = new DOF(); break;
            case Bone.toe_l: dof = DOF.Hinge(-40, +50); break;
            case Bone.foot_l: dof = DOF.Ball(-35, +20, -45, +20); break;
            case Bone.shin_l: dof = DOF.Hinge(0, 150); break;
            //case HumanSkeleton.thigh: dof = DOF.Ball3D( -25, +125, -25, +45, -45, +45); dof.Offset(15, 180, 180); break;
            case Bone.thigh_l: dof = DOF.Hinge(-125, +25); break;
            case Bone.hips: dof = new DOF(); break;
            case Bone.root: dof = new DOF(); break;
            case Bone.other: dof = DOF.NoLimit; break;
            default:
                if (!mirror)//first time goto left bones(guess is a right bone)
                {
                    return (HumanDOF(key - 1, true));//second time come to default will fail to else
                }
                else
                {
                    throw new Exception("unknown HumanSkeleton");
                }
        }
        return dof;
    }
    public static void LimitDOF(TransDOF ast, DOF dof)
    {
        ast.euler = LimitDOF(ast.euler, dof);
    }
    public static Vector3 LimitDOF(Vector3 V, DOF dof)
    {
        var x = Mathf.Clamp(V.x, dof.swingXMin, dof.swingXMax);
        var y = Mathf.Clamp(V.y, dof.twistMin, dof.twistMax);
        var z = Mathf.Clamp(V.z, dof.swingZMin, dof.swingZMax);
        var result = new Vector3(x, y, z);
        return result;
    }
}
