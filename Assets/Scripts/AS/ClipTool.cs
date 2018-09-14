using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClipTool
{
    //public static ASClip PingPong(ASClip clip)
    //{
    //    var c = new ASClip();
    //    c.frameRange.y = clip.frameRange.y * 2;
    //    c
    //}
    //public static 
    public static int max(int a, int b)
    {
        return (a > b) ? a : b;
    }
    public static void GetFrameRange(Clip clip)
    {
        int frameEnd = 0;
        foreach (var curveObj in clip.curves)
        {
            foreach (var curve in curveObj.curves)
            {
                if (curve.Count > 0)
                {
                    frameEnd = max(frameEnd, Mathf.RoundToInt(curve.Last().time));
                }
            }
        }
        clip.frameRange.x = 0;
        clip.frameRange.y = frameEnd;
    }
    static CurveObj GetCurveByBone(List<CurveObj> curves, Bone bone)
    {
        foreach (var curve in curves)
        {
            if (curve.ast.dof.bone == bone)
                return curve;
        }
        return null;
    }
    public static bool IsRightBone(Bone bone)
    {
        return IsLeftBone(bone - 1);
    }
    public static bool IsLeftBone(Bone bone)
    {
        switch (bone)
        {
            case Bone.thumb3_l:
            case Bone.thumb2_l:
            case Bone.thumb1_l:
            case Bone.index3_l:
            case Bone.index2_l:
            case Bone.index1_l:
            case Bone.middle3_l:
            case Bone.middle2_l:
            case Bone.middle1_l:
            case Bone.ring3_l:
            case Bone.ring2_l:
            case Bone.ring1_l:
            case Bone.pinky3_l:
            case Bone.pinky2_l:
            case Bone.pinky1_l:
            case Bone.palm1_l:
            case Bone.palm2_l:
            case Bone.palm3_l:
            case Bone.palm4_l:
            case Bone.hand_l:
            case Bone.forearm_l:
            case Bone.upperarm_l:
            case Bone.shoulder_l:
            case Bone.heel2_l:
            case Bone.heel1_l:
            case Bone.toe_l:
            case Bone.foot_l:
            case Bone.shin_l:
            case Bone.thigh_l:
                return true;
            default:
                return false;
        }
    }
    public static Bone GetPairBone(Bone bone)
    {
        switch (bone)
        {
            case Bone.thumb3_l:
            case Bone.thumb2_l:
            case Bone.thumb1_l:
            case Bone.index3_l:
            case Bone.index2_l:
            case Bone.index1_l:
            case Bone.middle3_l:
            case Bone.middle2_l:
            case Bone.middle1_l:
            case Bone.ring3_l:
            case Bone.ring2_l:
            case Bone.ring1_l:
            case Bone.pinky3_l:
            case Bone.pinky2_l:
            case Bone.pinky1_l:
            case Bone.palm1_l:
            case Bone.palm2_l:
            case Bone.palm3_l:
            case Bone.palm4_l:
            case Bone.hand_l:
            case Bone.forearm_l:
            case Bone.upperarm_l:
            case Bone.shoulder_l:
            case Bone.heel2_l:
            case Bone.heel1_l:
            case Bone.toe_l:
            case Bone.foot_l:
            case Bone.shin_l:
            case Bone.thigh_l:
                return bone + 1;

            case Bone.thumb3_r:
            case Bone.thumb2_r:
            case Bone.thumb1_r:
            case Bone.index3_r:
            case Bone.index2_r:
            case Bone.index1_r:
            case Bone.middle3_r:
            case Bone.middle2_r:
            case Bone.middle1_r:
            case Bone.ring3_r:
            case Bone.ring2_r:
            case Bone.ring1_r:
            case Bone.pinky3_r:
            case Bone.pinky2_r:
            case Bone.pinky1_r:
            case Bone.palm1_r:
            case Bone.palm2_r:
            case Bone.palm3_r:
            case Bone.palm4_r:
            case Bone.hand_r:
            case Bone.forearm_r:
            case Bone.upperarm_r:
            case Bone.shoulder_r:
            case Bone.heel2_r:
            case Bone.heel1_r:
            case Bone.toe_r:
            case Bone.foot_r:
            case Bone.shin_r:
            case Bone.thigh_r:
                return bone - 1;

            case Bone.head:
            case Bone.neck:
            case Bone.chest:
            case Bone.spine:
            case Bone.hips:
            case Bone.root:
            case Bone.other:
            default:
                return 0;
        }
    }
    public static void GetPairs(Clip clip)
    {
        GetPairs(clip.curves);
    }
    public static void GetPairs(List<CurveObj> curves)
    {
        foreach (var curve in curves)
        {
            if (curve.ast == null) continue;
            switch (curve.ast.dof.bone)
            {
                case Bone.thumb3_l:
                case Bone.thumb2_l:
                case Bone.thumb1_l:
                case Bone.index3_l:
                case Bone.index2_l:
                case Bone.index1_l:
                case Bone.middle3_l:
                case Bone.middle2_l:
                case Bone.middle1_l:
                case Bone.ring3_l:
                case Bone.ring2_l:
                case Bone.ring1_l:
                case Bone.pinky3_l:
                case Bone.pinky2_l:
                case Bone.pinky1_l:
                case Bone.palm1_l:
                case Bone.palm2_l:
                case Bone.palm3_l:
                case Bone.palm4_l:
                case Bone.hand_l:
                case Bone.forearm_l:
                case Bone.upperarm_l:
                case Bone.shoulder_l:
                case Bone.heel2_l:
                case Bone.heel1_l:
                case Bone.toe_l:
                case Bone.foot_l:
                case Bone.shin_l:
                case Bone.thigh_l:
                    curve.pair = GetCurveByBone(curves, curve.ast.dof.bone + 1);
                    break;

                case Bone.thumb3_r:
                case Bone.thumb2_r:
                case Bone.thumb1_r:
                case Bone.index3_r:
                case Bone.index2_r:
                case Bone.index1_r:
                case Bone.middle3_r:
                case Bone.middle2_r:
                case Bone.middle1_r:
                case Bone.ring3_r:
                case Bone.ring2_r:
                case Bone.ring1_r:
                case Bone.pinky3_r:
                case Bone.pinky2_r:
                case Bone.pinky1_r:
                case Bone.palm1_r:
                case Bone.palm2_r:
                case Bone.palm3_r:
                case Bone.palm4_r:
                case Bone.hand_r:
                case Bone.forearm_r:
                case Bone.upperarm_r:
                case Bone.shoulder_r:
                case Bone.heel2_r:
                case Bone.heel1_r:
                case Bone.toe_r:
                case Bone.foot_r:
                case Bone.shin_r:
                case Bone.thigh_r:
                    curve.pair = GetCurveByBone(curves, curve.ast.dof.bone - 1);
                    break;

                case Bone.head:
                case Bone.neck:
                case Bone.chest:
                case Bone.spine:
                case Bone.hips:
                case Bone.root:
                case Bone.other:
                default:
                    curve.pair = null;
                    break;
            }
        }
    }
}