using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof (MaterialMgr2))]
public class MaterialMgr2Editor:Editor
{
    public override void OnInspectorGUI()
    {
        var o = (MaterialMgr2)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Match"))
        {
            o.Match();
        }
    }
}
#endif
public class MaterialMgr2 : MonoBehaviour
{
    public List<Material> mats;
    [ContextMenu("Match")]
    public void Match()
    {
        Match(transform);
    }
    public void Match(Transform t)
    {
        var smrs = t.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var smr in smrs)
        {
            foreach (var mat in mats)
            {
                if (smr.name.Equals(mat.name, StringComparison.InvariantCultureIgnoreCase))
                {
                    smr.material = mat;
                }
            }
        }
        var mrs = t.GetComponentsInChildren<MeshRenderer>();
        foreach (var mr in mrs)
        {
            foreach (var mat in mats)
            {
                if (mr.name.Equals(mat.name, StringComparison.OrdinalIgnoreCase))
                {
                    mr.material = mat;
                }
            }
        }
    }
}
