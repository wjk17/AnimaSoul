using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Esa.UI
{
    public enum IKTarget
    {
        LeftHand,
        RightHand,
        LeftLeg,
        RightLeg,
        Count
    }
    public enum IKTargetMirror
    {
        Hand,
        Leg,
        Count
    }
    public enum IKTargetSingle
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
    public enum IKTargetSingleMirror
    {
        Hand,
        Elbow,
        Leg,
        Knee,
        Count
    }
    public partial class UIDOFEditor
    {
        public static string[] ikTargetNames = new string[] { "左手", "右手", "左脚", "右脚" };
        public static string[] ikTargetMirrorNames = new string[] { "双手", "双脚" };

        public static string[] ikTargetSingleNames = new string[] { "左手", "左手肘", "右手", "右手肘", "左脚", "左膝", "右脚", "右膝" };
        public static string[] ikTargetSingleMirrorNames = new string[] { "双手", "双肘", "双脚", "双膝" };
    }
}