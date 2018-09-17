using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UIDOFEditor {
    string[] ikTargetNames = new string[] { "左手", "右手", "左脚", "右脚" };
    string[] ikTargetMirrorNames = new string[] { "双手", "双脚" };
    public enum ASIKTarget
    {
        LeftHand,
        RightHand,
        LeftLeg,
        RightLeg,
        Count
    }
    public enum ASIKTargetMirror
    {
        Hand,
        Leg,
        Count
    }
    string[] ikTargetSingleNames = new string[] { "左手", "左手肘", "右手", "右手肘", "左脚", "左膝", "右脚", "右膝" };
    string[] ikTargetSingleMirrorNames = new string[] { "双手", "双肘", "双脚", "双膝" };
    public enum ASIKTargetSingle
    {
        LeftHand,
        LeftElbow,
        RightHand,
        RightElbow,
        LeftLeg,
        LeftKnee,
        RightLeg,
        RightKnee,
        Count
    }
    public enum ASIKTargetSingleMirror
    {
        Hand,
        Elbow,
        Leg,
        Knee,
        Count
    }
}
