using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
public enum ZHumanPart
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
public static class ZHuman
{
    // 使用指定的骨骼结构图给每根骨骼添加标识，指定这根骨骼是人体哪个部位。
    public static void MatchBones(this Dictionary<ASBone, Transform> dic, Transform tran, ZSkeleton skeleton)
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
    // input Left ZHumanBone
    public static ZSkeleton[] MirrorBone(ASBone bone, string key, params ZSkeleton[][] sub)
    {
        var key_l = key + "&_l";
        var key_r = key + "&_r";

        var zs = new ZSkeleton[2];
        zs[0] = new ZSkeleton(bone, key_l);
        zs[1] = new ZSkeleton(bone + 1, key_r);
        if (sub != null && sub.Length > 0)
        {
            var s0 = new List<ZSkeleton>();
            var s1 = new List<ZSkeleton>();

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
    private static ZSkeleton _HumanSkeletonMap;
    public static ZSkeleton HumanSkeletonMap
    {
        get
        {
            if (_HumanSkeletonMap == null) _HumanSkeletonMap = CreateHumanMap();
            return _HumanSkeletonMap;
        }
    }
    public static ZSkeleton CreateHumanMap()
    {
        // & < |
        var thumb3 = MirrorBone(ASBone.thumb3_l, "thumb|pollex");
        var thumb2 = MirrorBone(ASBone.thumb2_l, "thumb|pollex", thumb3);
        var thumb1 = MirrorBone(ASBone.thumb1_l, "thumb|pollex", thumb2);
        var index3 = MirrorBone(ASBone.index3_l, "index|first|fore");
        var index2 = MirrorBone(ASBone.index2_l, "index|first|fore", index3);
        var index1 = MirrorBone(ASBone.index1_l, "index|first|fore", index2);
        var middle3 = MirrorBone(ASBone.middle3_l, "middle|second");
        var middle2 = MirrorBone(ASBone.middle2_l, "middle|second", middle3);
        var middle1 = MirrorBone(ASBone.middle1_l, "middle|second", middle2);
        var ring3 = MirrorBone(ASBone.ring3_l, "ring|third");
        var ring2 = MirrorBone(ASBone.ring2_l, "ring|third", ring3);
        var ring1 = MirrorBone(ASBone.ring1_l, "ring|third", ring2);
        var pinky3 = MirrorBone(ASBone.pinky3_l, "pinky|little");
        var pinky2 = MirrorBone(ASBone.pinky2_l, "pinky|little", pinky3);
        var pinky1 = MirrorBone(ASBone.pinky1_l, "pinky|little", pinky2);
        var palm1 = MirrorBone(ASBone.palm1_l, "palm&1", thumb1, index1);
        var palm2 = MirrorBone(ASBone.palm2_l, "palm&2", middle1);
        var palm3 = MirrorBone(ASBone.palm3_l, "palm&3", ring1);
        var palm4 = MirrorBone(ASBone.palm4_l, "palm&4", pinky1);
        var hand = MirrorBone(ASBone.hand_l, "hand", palm1, palm2, palm3, palm4);
        var forearm = MirrorBone(ASBone.forearm_l, "fore|lower&arm", hand);
        var upperarm = MirrorBone(ASBone.upperarm_l, "upper&arm", forearm);
        var shoulder = MirrorBone(ASBone.shoulder_l, "shoulder|elbow", upperarm);
        var head = new ZSkeleton(ASBone.head, "head");
        var neck = new ZSkeleton(ASBone.neck, "neck", head);
        var chest = new ZSkeleton(ASBone.chest, "chest", neck, shoulder[0], shoulder[1]);
        var spine = new ZSkeleton(ASBone.spine, "spine", chest);
        var heel2 = MirrorBone(ASBone.heel2_l, "heel");
        var heel1 = MirrorBone(ASBone.heel1_l, "heel", heel2);
        var toe = MirrorBone(ASBone.toe_l, "toe");
        var foot = MirrorBone(ASBone.foot_l, "foot", toe);
        var shin = MirrorBone(ASBone.shin_l, "shin|calf|calves", foot, heel1);
        var thigh = MirrorBone(ASBone.thigh_l, "thigh", shin);
        var hips = new ZSkeleton(ASBone.hips, "hips", spine, thigh[0], thigh[1]);
        return hips;// new ZSkeleton(ZHumanBone.root, "*", hips);
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(ASAvatar))]
public class ASAvatarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var o = (ASAvatar)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Load"))
        {
            o.LoadASTs();
        }
        if (GUILayout.Button("Save"))
        {
            o.SaveASTs();
        }
        if (GUILayout.Button("Match"))
        {
            o.Match(true);
        }
        if (GUILayout.Button("Match(not Init)"))
        {
            o.Match(false);
        }
        if (GUILayout.Button("ClearTrans"))
        {
            o.ClearTrans();
        }
        if (GUILayout.Button("UpdateCoord"))
        {
            o.UpdateCoord();
        }
        if (GUILayout.Button("SetOrigin"))
        {
            o.SetOrigin();
        }
        if (GUILayout.Button("ChildrenSwap Y & Z (exclude self)"))
        {
            o.ChildrenSwapYAndZ();
        }
    }
}
#endif
[Serializable]
public class AvatarSetting
{
    public List<ASTransDOF> asts;
    public string name = "Default Avatar Setting";
    public AvatarSetting() { }
    public ASTransDOF GetTransDOF(ASBone bone)
    {
        //foreach (var ast in asts)
        //{
        //    if (bone == ast.dof.bone)
        //        return ast;
        //}
        return null;
    }
}
[ExecuteInEditMode]
public class ASAvatar : MonoBehaviour
{
    public ASTransDOF this[Transform t]
    {
        get
        {
            try
            {
                return GetTransDOF(t);
            }
            catch
            {
#if UNITY_EDITOR
                throw;
#else
                return null;
#endif
            }
        }
    }
    public ASTransDOF this[ASBone bone]
    {
        get
        {
            try
            {
                return GetTransDOF(bone);
            }
            catch
            {
#if UNITY_EDITOR
                throw;
#else
                return null;
#endif
            }
        }
    }
    public ASTransDOF this[ASDOF dof]
    {
        get
        {
            return this[dof.bone];
        }
    }
    public Transform rig;
    public AvatarSetting setting;
    public string path;
    public string folder = "Settings/";
    public string fileName = "AvatarSetting.xml";
    public void SaveASTs()
    {
        var dataPath = Application.dataPath;
        var rootPath = dataPath + "/../";
        path = rootPath + folder + fileName;
        Serializer.XMLSerialize(setting, path);
    }
    public void LoadASTs()
    {
        var dataPath = Application.dataPath;
        var rootPath = dataPath + "/../";
        path = rootPath + folder + fileName;
        setting = Serializer.XMLDeSerialize<AvatarSetting>(path);
    }
    public ASDOFMgr dofMgr { get { return GetComponent<ASDOFMgr>(); } }
    public ASBone selectBone;
    public void UpdateCoord()
    {
        int count = 0;
        //Start();
        foreach (var t in setting.asts)
        {
            //t.Init();
            if (t != null || t.transform != null)
            {
                t.UpdateCoord();
                count++;
            }
        }
        Debug.Log("UpdateCoords " + count.ToString());
    }
    //public void UpdateCoord()
    //{
    //    var td = GetTransDOF(selectBone);
    //    if (td == null || td.transform == null) return;

    //    td.UpdateCoord();

    //    Debug.Log("updateCoord " + td.transform.name);
    //}
    public void ChildrenSwapYAndZ()
    {
        var td = GetTransDOF(selectBone);
        if (td == null || td.transform == null) return;
        foreach (var t in td.transform.GetComponentsInChildren<Transform>(true))
        {
            if (t == td.transform) continue;
            var transD = GetTransDOF(t);
            if (transD == null) continue;
            var up = transD.up;
            transD.up = transD.forward;
            transD.forward = up;
            transD.UpdateCoord();
            Debug.Log("swap " + transD.transform.name);
        }
    }
    public void Reset()
    {
        rig = transform;
    }
    public ASTransDOF GetTransDOF(ASDOF dof)
    {
        return GetTransDOF(dof.bone);
    }
    public ASTransDOF GetTransDOF(ASBone bone)
    {
        foreach (var t in setting.asts)
        {
            if (t.dof.bone == bone)
            {
                return t;
            }
        }
        return null;
    }
    public ASTransDOF GetTransDOF(Transform trans)
    {
        foreach (var t in setting.asts)
        {
            if (t.transform == trans)
            {
                return t;
            }
        }
        return null;
    }
    //public ASBone GetBone(Transform t)
    //{
    //    foreach (var d in dic)
    //    {
    //        if (t == d.Value) return d.Key;
    //    }
    //    return ASBone.other;
    //}
    public void ClearTrans()
    {
        foreach (var t in setting.asts)
        {
            t.transform = null;
        }
    }
    [ContextMenu("Match")]
    public void Match(bool init)
    {
        var _dic = new Dictionary<ASBone, Transform>();
        _dic.Add(ASBone.root, rig);
        //ZHuman.MatchBones(_dic, rig, ZHuman.HumanSkeletonMap);
        ZHuman.MatchBones(_dic, rig, ZHuman.CreateHumanMap());
        foreach (var item in _dic)
        {
            var ast = GetTransDOF(item.Key);
            if (ast != null)
            {
                ast.transform = item.Value;
                Debug.Log("match: " + ast.transform.name);
            }
        }
        if (init) Start();
    }
    public void LoadFromDOFMgr()
    {
        foreach (var ast in setting.asts)
        {
            var dof = dofMgr.GetDOF(ast.dof.bone);
            ast.dof = dof;
        }
    }
    private void Start()
    {
        foreach (var t in setting.asts)
        {
            t.Init(); // 获取坐标轴的初始值
        }
    }
    public float drawLineLength = 0.5f;
    public bool depthTest = false;
    public bool drawLine = false;
    public void Update()
    {
        foreach (var t in setting.asts)
        {
#if UNITY_EDITOR
            if (drawLine && t.transform != null)
                t.coord.DrawRay(t.transform, t.euler, drawLineLength, depthTest);
            if (Application.isPlaying)
#endif
                t.Update();
        }
    }
    public void SetOrigin()
    {
        foreach (var t in setting.asts)
        {
            if (t != null && t.transform != null)
                t.coord.origin = t.transform.localRotation;
        }
    }
}
