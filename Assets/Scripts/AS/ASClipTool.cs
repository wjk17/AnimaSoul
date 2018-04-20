using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ASClipTool
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
    public static void GetFrameRange(ASClip clip)
    {
        int frameEnd = 0;
        foreach (var curve in clip.curves)
        {
            if (curve.timeCurve.keys.Count > 0)
            {
                frameEnd = max(frameEnd, curve.timeCurve.keys[curve.timeCurve.keys.Count - 1].frameIndex);
            }
        }
        clip.frameRange.x = 0;
        clip.frameRange.y = frameEnd;
    }
    static ASObjectCurve GetCurveByBone(List<ASObjectCurve> curves, ASBone bone)
    {
        foreach (var curve in curves)
        {
            if (curve.ast.dof.bone == bone)
                return curve;
        }
        return null;
    }
    public static bool IsRightBone(ASBone bone)
    {
        return IsLeftBone(bone - 1);
    }
    public static bool IsLeftBone(ASBone bone)
    {
        switch (bone)
        {
            case ASBone.thumb3_l:
            case ASBone.thumb2_l:
            case ASBone.thumb1_l:
            case ASBone.index3_l:
            case ASBone.index2_l:
            case ASBone.index1_l:
            case ASBone.middle3_l:
            case ASBone.middle2_l:
            case ASBone.middle1_l:
            case ASBone.ring3_l:
            case ASBone.ring2_l:
            case ASBone.ring1_l:
            case ASBone.pinky3_l:
            case ASBone.pinky2_l:
            case ASBone.pinky1_l:
            case ASBone.palm1_l:
            case ASBone.palm2_l:
            case ASBone.palm3_l:
            case ASBone.palm4_l:
            case ASBone.hand_l:
            case ASBone.forearm_l:
            case ASBone.upperarm_l:
            case ASBone.shoulder_l:
            case ASBone.heel2_l:
            case ASBone.heel1_l:
            case ASBone.toe_l:
            case ASBone.foot_l:
            case ASBone.shin_l:
            case ASBone.thigh_l:
                return true;
            default:
                return false;
        }
    }
    public static ASBone GetPairBone(ASBone bone)
    {
        switch (bone)
        {
            case ASBone.thumb3_l:
            case ASBone.thumb2_l:
            case ASBone.thumb1_l:
            case ASBone.index3_l:
            case ASBone.index2_l:
            case ASBone.index1_l:
            case ASBone.middle3_l:
            case ASBone.middle2_l:
            case ASBone.middle1_l:
            case ASBone.ring3_l:
            case ASBone.ring2_l:
            case ASBone.ring1_l:
            case ASBone.pinky3_l:
            case ASBone.pinky2_l:
            case ASBone.pinky1_l:
            case ASBone.palm1_l:
            case ASBone.palm2_l:
            case ASBone.palm3_l:
            case ASBone.palm4_l:
            case ASBone.hand_l:
            case ASBone.forearm_l:
            case ASBone.upperarm_l:
            case ASBone.shoulder_l:
            case ASBone.heel2_l:
            case ASBone.heel1_l:
            case ASBone.toe_l:
            case ASBone.foot_l:
            case ASBone.shin_l:
            case ASBone.thigh_l:
                return bone + 1;

            case ASBone.thumb3_r:
            case ASBone.thumb2_r:
            case ASBone.thumb1_r:
            case ASBone.index3_r:
            case ASBone.index2_r:
            case ASBone.index1_r:
            case ASBone.middle3_r:
            case ASBone.middle2_r:
            case ASBone.middle1_r:
            case ASBone.ring3_r:
            case ASBone.ring2_r:
            case ASBone.ring1_r:
            case ASBone.pinky3_r:
            case ASBone.pinky2_r:
            case ASBone.pinky1_r:
            case ASBone.palm1_r:
            case ASBone.palm2_r:
            case ASBone.palm3_r:
            case ASBone.palm4_r:
            case ASBone.hand_r:
            case ASBone.forearm_r:
            case ASBone.upperarm_r:
            case ASBone.shoulder_r:
            case ASBone.heel2_r:
            case ASBone.heel1_r:
            case ASBone.toe_r:
            case ASBone.foot_r:
            case ASBone.shin_r:
            case ASBone.thigh_r:
                return bone - 1;

            case ASBone.head:
            case ASBone.neck:
            case ASBone.chest:
            case ASBone.spine:
            case ASBone.hips:
            case ASBone.root:
            case ASBone.other:
            default:
                return 0;
        }
    }
    public static void GetPairs(ASClip clip)
    {
        GetPairs(clip.curves);
    }
    public static void GetPairs(List<ASObjectCurve> curves)
    {
        foreach (var curve in curves)
        {
            if (curve.ast == null) continue;
            switch (curve.ast.dof.bone)
            {
                case ASBone.thumb3_l:
                case ASBone.thumb2_l:
                case ASBone.thumb1_l:
                case ASBone.index3_l:
                case ASBone.index2_l:
                case ASBone.index1_l:
                case ASBone.middle3_l:
                case ASBone.middle2_l:
                case ASBone.middle1_l:
                case ASBone.ring3_l:
                case ASBone.ring2_l:
                case ASBone.ring1_l:
                case ASBone.pinky3_l:
                case ASBone.pinky2_l:
                case ASBone.pinky1_l:
                case ASBone.palm1_l:
                case ASBone.palm2_l:
                case ASBone.palm3_l:
                case ASBone.palm4_l:
                case ASBone.hand_l:
                case ASBone.forearm_l:
                case ASBone.upperarm_l:
                case ASBone.shoulder_l:
                case ASBone.heel2_l:
                case ASBone.heel1_l:
                case ASBone.toe_l:
                case ASBone.foot_l:
                case ASBone.shin_l:
                case ASBone.thigh_l:
                    curve.pair = GetCurveByBone(curves, curve.ast.dof.bone + 1);
                    break;

                case ASBone.thumb3_r:
                case ASBone.thumb2_r:
                case ASBone.thumb1_r:
                case ASBone.index3_r:
                case ASBone.index2_r:
                case ASBone.index1_r:
                case ASBone.middle3_r:
                case ASBone.middle2_r:
                case ASBone.middle1_r:
                case ASBone.ring3_r:
                case ASBone.ring2_r:
                case ASBone.ring1_r:
                case ASBone.pinky3_r:
                case ASBone.pinky2_r:
                case ASBone.pinky1_r:
                case ASBone.palm1_r:
                case ASBone.palm2_r:
                case ASBone.palm3_r:
                case ASBone.palm4_r:
                case ASBone.hand_r:
                case ASBone.forearm_r:
                case ASBone.upperarm_r:
                case ASBone.shoulder_r:
                case ASBone.heel2_r:
                case ASBone.heel1_r:
                case ASBone.toe_r:
                case ASBone.foot_r:
                case ASBone.shin_r:
                case ASBone.thigh_r:
                    curve.pair = GetCurveByBone(curves, curve.ast.dof.bone - 1);
                    break;

                case ASBone.head:
                case ASBone.neck:
                case ASBone.chest:
                case ASBone.spine:
                case ASBone.hips:
                case ASBone.root:
                case ASBone.other:
                default:
                    curve.pair = null;
                    break;
            }
        }
    }
}