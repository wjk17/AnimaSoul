using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class DOFLiminator
{
    private static ASDOF finger(int index)
    {
        ASDOF dof;
        switch (index)
        {
            case 0: dof = ASDOF.Ball(-10, +10, -80, 30); break;//第一截手指
            case 1: dof = ASDOF.Hinge(-100, 30); break;//第二截手指
            case 2: dof = ASDOF.Hinge(-80, 30); break;//第三届（末尾）手指
            default: throw new Exception();
        }
        return dof;
    }
    public static ASDOF HumanDOF(ASBone key)
    {
        return HumanDOF(key, false);
    }
    static void AddDOF(ASBone zb, bool mirror = false)
    {
        var dof = HumanDOF(zb);
        dof.bone = zb;
        dofs.Add(dof);
        if (mirror)
        {
            AddDOF(zb + 1);
        }
    }
    static List<ASDOF> dofs;
    public static List<ASDOF> DefaultHumanDOF()
    {
        dofs = new List<ASDOF>();
        //第三截手指
        AddDOF(ASBone.thumb3_l,true);
        AddDOF(ASBone.index3_l, true);
        AddDOF(ASBone.middle3_l, true);
        AddDOF(ASBone.ring3_l, true);
        AddDOF(ASBone.pinky3_l, true);
        //第二截手指
        AddDOF(ASBone.thumb2_l, true);
        AddDOF(ASBone.index2_l, true);
        AddDOF(ASBone.middle2_l, true);
        AddDOF(ASBone.ring2_l, true);
        AddDOF(ASBone.pinky2_l, true);
        //第一截手指
        AddDOF(ASBone.thumb1_l, true);
        AddDOF(ASBone.index1_l, true);
        AddDOF(ASBone.middle1_l, true);
        AddDOF(ASBone.ring1_l, true);
        AddDOF(ASBone.pinky1_l, true);
        //掌骨（固定）
        AddDOF(ASBone.palm1_l, true);
        AddDOF(ASBone.palm2_l, true);
        AddDOF(ASBone.palm3_l, true);
        AddDOF(ASBone.palm4_l, true);
        //四肢
        AddDOF(ASBone.hand_l, true);
        AddDOF(ASBone.forearm_l, true);
        AddDOF(ASBone.upperarm_l, true);
        AddDOF(ASBone.shoulder_l, true);
        AddDOF(ASBone.head);
        AddDOF(ASBone.neck);
        AddDOF(ASBone.chest);
        AddDOF(ASBone.spine);
        AddDOF(ASBone.heel2_l, true);
        AddDOF(ASBone.heel1_l, true);
        AddDOF(ASBone.toe_l, true);
        AddDOF(ASBone.foot_l, true);
        AddDOF(ASBone.shin_l, true);
        AddDOF(ASBone.thigh_l, true);
        AddDOF(ASBone.hips);
        AddDOF(ASBone.root);
        AddDOF(ASBone.other);
        return dofs;
    }
    private static ASDOF HumanDOF(ASBone key, bool mirror)
    {
        ASDOF dof;
        switch (key)
        {
            //第三截手指
            case ASBone.thumb3_l:
            case ASBone.index3_l:
            case ASBone.middle3_l:
            case ASBone.ring3_l:
            case ASBone.pinky3_l: dof = finger(2); break;
            //第二截手指
            case ASBone.thumb2_l: dof = ASDOF.Ball(-20, +20, -80, +40); break;
            case ASBone.index2_l:
            case ASBone.middle2_l:
            case ASBone.ring2_l:
            case ASBone.pinky2_l: dof = finger(1); break;
            //第一截手指
            case ASBone.thumb1_l: dof = ASDOF.Ball(-15, +15, -30, +15); break;
            case ASBone.index1_l: dof = ASDOF.Hinge(-100, 0); break;
            case ASBone.middle1_l:
            case ASBone.ring1_l:
            case ASBone.pinky1_l: dof = finger(0); break;
            //掌骨（固定）
            case ASBone.palm1_l:
            case ASBone.palm2_l:
            case ASBone.palm3_l:
            case ASBone.palm4_l: dof = new ASDOF(); break;
            //四肢
            case ASBone.hand_l: dof = ASDOF.Ball(-25, +55, -90, +90); break;
            case ASBone.forearm_l: dof = ASDOF.Hinge2D(-150, +0, -145, +10); break;
            case ASBone.upperarm_l: dof = ASDOF.Ball3D(-140, +40, -135, +90, -90, +90); break;
            case ASBone.shoulder_l: dof = ASDOF.Ball(-20, +20, -20, +20); break;
            case ASBone.head: dof = ASDOF.Hinge(-35, +40); break;
            case ASBone.neck: dof = ASDOF.Ball3D(-55, +55, -50, +60, -70, +70); break;
            case ASBone.chest: dof = ASDOF.Ball3D(-25, +25, -15, +40, -40, +40); break;
            case ASBone.spine: dof = ASDOF.Ball3D(-25, +25, -15, +40, -40, +40); break;
            case ASBone.heel2_l: dof = new ASDOF(); break;
            case ASBone.heel1_l: dof = new ASDOF(); break;
            case ASBone.toe_l: dof = ASDOF.Hinge(-40, +50); break;
            case ASBone.foot_l: dof = ASDOF.Ball(-35, +20, -45, +20); break;
            case ASBone.shin_l: dof = ASDOF.Hinge(0, 150); break;
            //case HumanSkeleton.thigh: dof = DOF.Ball3D( -25, +125, -25, +45, -45, +45); dof.Offset(15, 180, 180); break;
            case ASBone.thigh_l: dof = ASDOF.Hinge(-125, +25); break;
            case ASBone.hips: dof = new ASDOF(); break;
            case ASBone.root: dof = new ASDOF(); break;
            case ASBone.other: dof = ASDOF.NoLimit; break;
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
    public static void LimitDOF(ASTransDOF ast, ASDOF dof)
    {
        ast.euler = LimitDOF(ast.euler, dof);
    }
    public static Vector3 LimitDOF(Vector3 V, ASDOF dof)
    {
        var x = Mathf.Clamp(V.x, dof.swingXMin, dof.swingXMax);
        var y = Mathf.Clamp(V.y, dof.twistMin, dof.twistMax);
        var z = Mathf.Clamp(V.z, dof.swingZMin, dof.swingZMax);
        var result = new Vector3(x, y, z);
        return result;
    }
}
