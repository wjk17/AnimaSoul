using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ASBoneTool
{
    public static ASBone[] arms = new ASBone[]
    {
        ASBone.forearm_l,
        ASBone.forearm_r,
        ASBone.hand_l,
        ASBone.hand_r,
        ASBone.upperarm_l,
        ASBone.upperarm_r,
        ASBone.shoulder_l,
        ASBone.shoulder_r
    };
    public static string[] names = new string[]
    {
    "拇指3(左)", "拇指3(右)",
    "拇指2(左)", "拇指2(右)",
    "拇指1(左)", "拇指1(右)",
    "食指3(左)", "食指3(右)",
    "食指2(左)", "食指2(右)",
    "食指1(左)", "食指1(右)",
    "中指3(左)", "中指3(右)",
    "中指2(左)", "中指2(右)",
    "中指1(左)", "中指1(右)",
    "无名指3(左)", "无名指3(右)",
    "无名指2(左)", "无名指2(右)",
    "无名指1(左)", "无名指1(右)",
    "尾指3(左)", "尾指3(右)",
    "尾指2(左)", "尾指2(右)",
    "尾指1(左)", "尾指1(右)",
    "掌骨1(左)", "掌骨1(右)",
    "掌骨2(左)", "掌骨2(右)",
    "掌骨3(左)", "掌骨3(右)",
    "掌骨4(左)", "掌骨4(右)",
    "手(左)", "手(右)",
    "前臂(左)", "前臂(右)",
    "上臂(左)", "上臂(右)",
    "肩膀(左)", "肩膀(右)",
    "头",
    "颈部",
    "胸部",
    "脊椎",
    "脚跟2(左)", "脚跟2(右)",
    "脚跟1(左)", "脚跟1(右)",
    "脚趾(左)", "脚趾(右)",
    "脚(左)", "脚(右)",
    "小腿(左)", "小腿(右)",
    "大腿(左)", "大腿(右)",
    "屁股",
    "根",
    //"其他",
    };
}
public enum ASBone
{
    thumb3_l, thumb3_r,
    thumb2_l, thumb2_r,
    thumb1_l, thumb1_r,
    index3_l, index3_r,
    index2_l, index2_r,
    index1_l, index1_r,
    middle3_l, middle3_r,
    middle2_l, middle2_r,
    middle1_l, middle1_r,
    ring3_l, ring3_r,
    ring2_l, ring2_r,
    ring1_l, ring1_r,
    pinky3_l, pinky3_r,
    pinky2_l, pinky2_r,
    pinky1_l, pinky1_r,
    palm1_l, palm1_r,
    palm2_l, palm2_r,
    palm3_l, palm3_r,
    palm4_l, palm4_r,

    hand_l, hand_r,
    forearm_l, forearm_r,
    upperarm_l, upperarm_r,
    shoulder_l, shoulder_r,
    head,
    neck,
    chest,
    spine,
    heel2_l, heel2_r,
    heel1_l, heel1_r,
    toe_l, toe_r,
    foot_l, foot_r,
    shin_l, shin_r,
    thigh_l, thigh_r,
    hips,
    root,
    other,
}

public class ZSkeleton
{
    public ASBone bone;
    public string key;
    public ZSkeleton[] sub;
    public ZSkeleton(ASBone bone, string key, params ZSkeleton[] sub)
    {
        this.bone = bone;
        this.key = key;
        this.sub = sub;
    }
}
