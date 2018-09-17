using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UIDOFEditor
{
    private void OnLockTargetChange(int index)
    {
        joints = new List<Bone>();
        var lockTarget = (ASIKTarget)index;
        switch (lockTarget)
        {
            case ASIKTarget.RightHand:
                joints.Add(Bone.hand_r);
                joints.Add(Bone.forearm_r);
                joints.Add(Bone.upperarm_r);
                break;
            case ASIKTarget.LeftHand:
                joints.Add(Bone.hand_l);
                joints.Add(Bone.forearm_l);
                joints.Add(Bone.upperarm_l);
                break;
            case ASIKTarget.RightLeg:
                joints.Add(Bone.foot_r);
                joints.Add(Bone.shin_r);
                joints.Add(Bone.thigh_r);
                break;
            case ASIKTarget.LeftLeg:
                joints.Add(Bone.foot_l);
                joints.Add(Bone.shin_l);
                joints.Add(Bone.thigh_l);
                break;
            default: throw null;
        }
        lockPos1 = avatar[joints[0]].transform.position;
    }
    void OnLockMirrorTargetChange(int index)
    {
        joints = new List<Bone>();
        joints2 = new List<Bone>();
        var lockTargetMirror = (ASIKTargetMirror)index;
        switch (lockTargetMirror)
        {
            case ASIKTargetMirror.Hand:
                joints.Add(Bone.hand_l);
                joints.Add(Bone.forearm_l);
                joints.Add(Bone.upperarm_l);
                lockPos1 = avatar[Bone.hand_l].transform.position;
                lockPos2 = avatar[Bone.hand_r].transform.position;
                break;
            case ASIKTargetMirror.Leg:
                joints.Add(Bone.foot_l);
                joints.Add(Bone.shin_l);
                joints.Add(Bone.thigh_l);
                lockPos1 = avatar[Bone.foot_l].transform.position;
                lockPos2 = avatar[Bone.foot_r].transform.position;
                break;
            default: throw null;
        }
        foreach (var j in joints)
        {
            joints2.Add(j + 1); // right side
        }
    }
    public void ExBoneIK()
    {
        var targetPos = target.position + exBone2LeftHand;
        joints = new List<Bone>();
        joints.Add(Bone.hand_l);
        joints.Add(Bone.forearm_l);
        joints.Add(Bone.upperarm_l);
        end = avatar[Bone.hand_l].transform;
        IKSolve(targetPos, joints.ToArray());

        targetPos = target.position + exBone2RightHand;
        joints = new List<Bone>();
        joints.Add(Bone.hand_r);
        joints.Add(Bone.forearm_r);
        joints.Add(Bone.upperarm_r);
        end = avatar[Bone.hand_r].transform;
        IKSolve(targetPos, joints.ToArray());
    }
    void OnIKTargetChanged(int index)
    {
        joints = new List<Bone>();
        var ikTarget = (ASIKTarget)index;
        switch (ikTarget)
        {
            case ASIKTarget.RightHand:
                joints.Add(Bone.hand_r);
                joints.Add(Bone.forearm_r);
                joints.Add(Bone.upperarm_r);
                break;
            case ASIKTarget.LeftHand:
                joints.Add(Bone.hand_l);
                joints.Add(Bone.forearm_l);
                joints.Add(Bone.upperarm_l);
                break;
            case ASIKTarget.RightLeg:
                joints.Add(Bone.foot_r);
                joints.Add(Bone.shin_r);
                joints.Add(Bone.thigh_r);
                break;
            case ASIKTarget.LeftLeg:
                joints.Add(Bone.foot_l);
                joints.Add(Bone.shin_l);
                joints.Add(Bone.thigh_l);
                break;
            default: throw null;
        }
        end = avatar[joints[0]].transform;
        OnIKSnap();
    }
    void IKSingleTargetChange(int index)
    {
        joints = new List<Bone>();
        var ikSingleTarget = (ASIKTargetSingle)index;
        switch (ikSingleTarget)
        {
            case ASIKTargetSingle.RightElbow:
                joints.Add(Bone.forearm_r);
                joints.Add(Bone.upperarm_r);
                break;
            case ASIKTargetSingle.RightHand:
                joints.Add(Bone.hand_r);
                joints.Add(Bone.forearm_r);
                break;
            case ASIKTargetSingle.LeftElbow:
                joints.Add(Bone.forearm_l);
                joints.Add(Bone.upperarm_l);
                break;
            case ASIKTargetSingle.LeftHand:
                joints.Add(Bone.hand_l);
                joints.Add(Bone.forearm_l);
                break;
            case ASIKTargetSingle.RightKnee:
                joints.Add(Bone.shin_r);
                joints.Add(Bone.thigh_r);
                break;
            case ASIKTargetSingle.RightLeg:
                joints.Add(Bone.foot_r);
                joints.Add(Bone.shin_r);
                break;
            case ASIKTargetSingle.LeftKnee:
                joints.Add(Bone.shin_l);
                joints.Add(Bone.thigh_l);
                break;
            case ASIKTargetSingle.LeftLeg:
                joints.Add(Bone.foot_l);
                joints.Add(Bone.shin_l);
                break;
            default: throw null;
        }
        end = avatar[joints[0]].transform;
        OnIKSingleSnap();
    }
}
