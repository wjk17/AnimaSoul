using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UIDOFEditor
{
    private void OnLockTargetChange(int index)
    {
        joints = new List<ASBone>();
        var lockTarget = (ASIKTarget)index;
        switch (lockTarget)
        {
            case ASIKTarget.RightHand:
                joints.Add(ASBone.hand_r);
                joints.Add(ASBone.forearm_r);
                joints.Add(ASBone.upperarm_r);
                break;
            case ASIKTarget.LeftHand:
                joints.Add(ASBone.hand_l);
                joints.Add(ASBone.forearm_l);
                joints.Add(ASBone.upperarm_l);
                break;
            case ASIKTarget.RightLeg:
                joints.Add(ASBone.foot_r);
                joints.Add(ASBone.shin_r);
                joints.Add(ASBone.thigh_r);
                break;
            case ASIKTarget.LeftLeg:
                joints.Add(ASBone.foot_l);
                joints.Add(ASBone.shin_l);
                joints.Add(ASBone.thigh_l);
                break;
            default: throw null;
        }
        lockPos1 = avatar[joints[0]].transform.position;
    }
    void OnLockMirrorTargetChange(int index)
    {
        joints = new List<ASBone>();
        joints2 = new List<ASBone>();
        var lockTargetMirror = (ASIKTargetMirror)index;
        switch (lockTargetMirror)
        {
            case ASIKTargetMirror.Hand:
                joints.Add(ASBone.hand_l);
                joints.Add(ASBone.forearm_l);
                joints.Add(ASBone.upperarm_l);
                lockPos1 = avatar[ASBone.hand_l].transform.position;
                lockPos2 = avatar[ASBone.hand_r].transform.position;
                break;
            case ASIKTargetMirror.Leg:
                joints.Add(ASBone.foot_l);
                joints.Add(ASBone.shin_l);
                joints.Add(ASBone.thigh_l);
                lockPos1 = avatar[ASBone.foot_l].transform.position;
                lockPos2 = avatar[ASBone.foot_r].transform.position;
                break;
            default: throw null;
        }
        foreach (var j in joints)
        {
            joints2.Add(j + 1); // right side
        }
    }
    public void GunIK()
    {
        var targetPos = target.position + exBone2LeftHand;
        joints = new List<ASBone>();
        joints.Add(ASBone.hand_l);
        joints.Add(ASBone.forearm_l);
        joints.Add(ASBone.upperarm_l);
        end = avatar[ASBone.hand_l].transform;
        IKSolve(targetPos, joints.ToArray());

        targetPos = target.position + exBone2RightHand;
        joints = new List<ASBone>();
        joints.Add(ASBone.hand_r);
        joints.Add(ASBone.forearm_r);
        joints.Add(ASBone.upperarm_r);
        end = avatar[ASBone.hand_r].transform;
        IKSolve(targetPos, joints.ToArray());
    }
    void OnIKTargetChanged(int index)
    {
        joints = new List<ASBone>();
        var ikTarget = (ASIKTarget)index;
        switch (ikTarget)
        {
            case ASIKTarget.RightHand:
                joints.Add(ASBone.hand_r);
                joints.Add(ASBone.forearm_r);
                joints.Add(ASBone.upperarm_r);
                break;
            case ASIKTarget.LeftHand:
                joints.Add(ASBone.hand_l);
                joints.Add(ASBone.forearm_l);
                joints.Add(ASBone.upperarm_l);
                break;
            case ASIKTarget.RightLeg:
                joints.Add(ASBone.foot_r);
                joints.Add(ASBone.shin_r);
                joints.Add(ASBone.thigh_r);
                break;
            case ASIKTarget.LeftLeg:
                joints.Add(ASBone.foot_l);
                joints.Add(ASBone.shin_l);
                joints.Add(ASBone.thigh_l);
                break;
            default: throw null;
        }
        end = avatar[joints[0]].transform;
        OnIKSnap();
    }
    void IKSingleTargetChange(int index)
    {
        joints = new List<ASBone>();
        var ikSingleTarget = (ASIKTargetSingle)index;
        switch (ikSingleTarget)
        {
            case ASIKTargetSingle.RightElbow:
                joints.Add(ASBone.forearm_r);
                joints.Add(ASBone.upperarm_r);
                break;
            case ASIKTargetSingle.RightHand:
                joints.Add(ASBone.hand_r);
                joints.Add(ASBone.forearm_r);
                break;
            case ASIKTargetSingle.LeftElbow:
                joints.Add(ASBone.forearm_l);
                joints.Add(ASBone.upperarm_l);
                break;
            case ASIKTargetSingle.LeftHand:
                joints.Add(ASBone.hand_l);
                joints.Add(ASBone.forearm_l);
                break;
            case ASIKTargetSingle.RightKnee:
                joints.Add(ASBone.shin_r);
                joints.Add(ASBone.thigh_r);
                break;
            case ASIKTargetSingle.RightLeg:
                joints.Add(ASBone.foot_r);
                joints.Add(ASBone.shin_r);
                break;
            case ASIKTargetSingle.LeftKnee:
                joints.Add(ASBone.shin_l);
                joints.Add(ASBone.thigh_l);
                break;
            case ASIKTargetSingle.LeftLeg:
                joints.Add(ASBone.foot_l);
                joints.Add(ASBone.shin_l);
                break;
            default: throw null;
        }
        end = avatar[joints[0]].transform;
        OnIKSingleSnap();
    }
}
