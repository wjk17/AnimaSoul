using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum RigHumanPart
{
    root,
    hips,
    body,
    head,
    leg,
    arm,
    fingers,
    foot,
    hand,
}

public static class RigHuman
{
    // 使用指定的骨骼结构图给每根骨骼添加标识，指定这根骨骼是人体哪个部位。
    public static void MatchBones(this Dictionary<Bone, Transform> dic, Transform tran, Skeleton skeleton)
    {
        foreach (Transform t in tran)
        {
            if (KeyMatch(t.name, skeleton.key))
            {
                dic.Add(skeleton.bone, t);
                foreach (var s in skeleton.sub)
                {
                    MatchBones(dic, t, s);
                }
                return;
            }
        }
    }
    private static bool KeyMatch(string name, string key)
    {
        if (key == "*") return true;
        name = name.Replace('.', '_');
        var ands = key.Split('&');
        foreach (var and in ands)
        {
            if (string.Equals(and, "_r", StringComparison.OrdinalIgnoreCase)) return name.EndsWith("_r", StringComparison.OrdinalIgnoreCase);
            if (string.Equals(and, "_l", StringComparison.OrdinalIgnoreCase)) return name.EndsWith("_l", StringComparison.OrdinalIgnoreCase);
            if (!OrResult(name, and))
            {
                return false;
            }
        }
        return true;
    }
    public static bool OrResult(string name, string key)
    {
        var s = key.Split('|');
        foreach (var n in s)
        {
            if (name.IndexOf(n, StringComparison.OrdinalIgnoreCase) > -1)
            {
                return true;
            }
        }
        return false;
    }
    // input Left HumanBone
    public static Skeleton[] MirrorBone(Bone bone, string key, params Skeleton[][] sub)
    {
        var key_l = key + "&_l";
        var key_r = key + "&_r";

        var zs = new Skeleton[2];
        zs[0] = new Skeleton(bone, key_l);
        zs[1] = new Skeleton(bone + 1, key_r);
        if (sub != null && sub.Length > 0)
        {
            var s0 = new List<Skeleton>();
            var s1 = new List<Skeleton>();

            foreach (var s in sub)
            {
                s0.Add(s[0]);
                s1.Add(s[1]);
            }
            zs[0].sub = s0.ToArray();
            zs[1].sub = s1.ToArray();
        }
        return zs;
    }
    private static Skeleton _HumanSkeletonMap;
    public static Skeleton HumanSkeletonMap
    {
        get
        {
            if (_HumanSkeletonMap == null) _HumanSkeletonMap = CreateHumanMap();
            return _HumanSkeletonMap;
        }
    }
    public static Skeleton CreateHumanMap()
    {
        // & < |
        var thumb3 = MirrorBone(Bone.thumb3_l, "thumb|pollex");
        var thumb2 = MirrorBone(Bone.thumb2_l, "thumb|pollex", thumb3);
        var thumb1 = MirrorBone(Bone.thumb1_l, "thumb|pollex", thumb2);
        var index3 = MirrorBone(Bone.index3_l, "index|first|fore");
        var index2 = MirrorBone(Bone.index2_l, "index|first|fore", index3);
        var index1 = MirrorBone(Bone.index1_l, "index|first|fore", index2);
        var middle3 = MirrorBone(Bone.middle3_l, "middle|second");
        var middle2 = MirrorBone(Bone.middle2_l, "middle|second", middle3);
        var middle1 = MirrorBone(Bone.middle1_l, "middle|second", middle2);
        var ring3 = MirrorBone(Bone.ring3_l, "ring|third");
        var ring2 = MirrorBone(Bone.ring2_l, "ring|third", ring3);
        var ring1 = MirrorBone(Bone.ring1_l, "ring|third", ring2);
        var pinky3 = MirrorBone(Bone.pinky3_l, "pinky|little");
        var pinky2 = MirrorBone(Bone.pinky2_l, "pinky|little", pinky3);
        var pinky1 = MirrorBone(Bone.pinky1_l, "pinky|little", pinky2);
        var palm1 = MirrorBone(Bone.palm1_l, "palm&1", thumb1, index1);
        var palm2 = MirrorBone(Bone.palm2_l, "palm&2", middle1);
        var palm3 = MirrorBone(Bone.palm3_l, "palm&3", ring1);
        var palm4 = MirrorBone(Bone.palm4_l, "palm&4", pinky1);
        var hand = MirrorBone(Bone.hand_l, "hand", palm1, palm2, palm3, palm4);
        var forearm = MirrorBone(Bone.forearm_l, "fore|lower&arm", hand);
        var upperarm = MirrorBone(Bone.upperarm_l, "upper&arm", forearm);
        var shoulder = MirrorBone(Bone.shoulder_l, "shoulder|elbow", upperarm);
        var head = new Skeleton(Bone.head, "head");
        var neck = new Skeleton(Bone.neck, "neck", head);
        var chest = new Skeleton(Bone.chest, "chest", neck, shoulder[0], shoulder[1]);
        var spine = new Skeleton(Bone.spine, "spine", chest);
        var heel2 = MirrorBone(Bone.heel2_l, "heel");
        var heel1 = MirrorBone(Bone.heel1_l, "heel", heel2);
        var toe = MirrorBone(Bone.toe_l, "toe");
        var foot = MirrorBone(Bone.foot_l, "foot", toe);
        var shin = MirrorBone(Bone.shin_l, "shin|calf|calves", foot, heel1);
        var thigh = MirrorBone(Bone.thigh_l, "thigh", shin);
        var hips = new Skeleton(Bone.hips, "hips", spine, thigh[0], thigh[1]);
        return hips;// new ZSkeleton(ZHumanBone.root, "*", hips);
    }
}
