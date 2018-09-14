using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(Avator))]
public class AvatorEditor : E_ShowButtons<Avator> { }
#endif

[ExecuteInEditMode]
public class Avator : MonoXmlSL<AvatorData>
{
    public TransDOF this[Transform t]
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
    public TransDOF this[Bone bone]
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
    public TransDOF this[DOF dof]
    {
        get
        {
            return this[dof.bone];
        }
    }
    public Transform rig;
    public AvatorData setting;
    public DOFMgr dofMgr { get { return GetComponent<DOFMgr>(); } }
    public Bone selectBone;

    public float drawLineLength = 0.03f;
    public bool depthTest = false;
    public bool drawLine = true;
    public Color boneColor;

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
    public void UpdateCoordSel()
    {
        var td = GetTransDOF(selectBone);
        if (td == null || td.transform == null) return;

        td.UpdateCoord();

        Debug.Log("updateCoord " + td.transform.name);
    }
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
    public TransDOF GetTransDOF(DOF dof)
    {
        return GetTransDOF(dof.bone);
    }
    public TransDOF GetTransDOF(Bone bone)
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
    public TransDOF GetTransDOF(Transform trans)
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
        var _dic = new Dictionary<Bone, Transform>();
        _dic.Add(Bone.root, rig);
        //ZHuman.MatchBones(_dic, rig, ZHuman.HumanSkeletonMap);
        RigHuman.MatchBones(_dic, rig, RigHuman.CreateHumanMap());
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
