using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public ASDOFMgr dofMgr { get { return GetComponent<ASDOFMgr>(); } }
    public ASBone selectBone;

    public float drawLineLength = 0.5f;
    public bool depthTest = false;
    public bool drawLine = false;
    public Color boneColor;

    [ShowButton]
    public void SaveASTs()
    {
        var dataPath = Application.dataPath;
        var rootPath = dataPath + "/../";
        path = rootPath + folder + fileName;
        Serializer.XMLSerialize(setting, path);
    }
    [ShowButton]
    public void LoadASTs()
    {
        var dataPath = Application.dataPath;
        var rootPath = dataPath + "/../";
        path = rootPath + folder + fileName;
        setting = Serializer.XMLDeSerialize<AvatarSetting>(path);
    }
    [ShowButton]
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
    [ShowButton("ChildrenSwap Y & Z (exclude self)")]
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
    [ShowButton]
    public void ClearTrans()
    {
        foreach (var t in setting.asts)
        {
            t.transform = null;
        }
    }
    [ShowButton]
    void Match()
    {
        Match(true);
    }
    [ShowButton("Match(not Init)")]
    void MatchNotInit()
    {
        Match(false);
    }
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
    public void Update()
    {
#if UNITY_EDITOR
        if (drawLine)
            setting.DrawLines(boneColor, drawLineLength, depthTest);
        if (Application.isPlaying)
#endif
            setting.UpdateTrans();
    }
    [ShowButton]
    public void SetOrigin()
    {
        foreach (var t in setting.asts)
        {
            if (t != null && t.transform != null)
                t.coord.origin = t.transform.localRotation;
        }
    }
}
