using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof (MaterialMgr))]
public class MaterialMgrEditor:Editor
{
    public override void OnInspectorGUI()
    {
        var o = (MaterialMgr)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Match"))
        {
            o.Match();
        }
    }
}
#endif
public class MaterialMgr : MonoBehaviour
{
    [Serializable]
    public class NameMaterialPair
    {
        public string name;
        public Material[] materials;
    }
    public List<NameMaterialPair> pairs;
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
            foreach (var pair in pairs)
            {
                if (smr.name.Equals(pair.name, StringComparison.OrdinalIgnoreCase))
                {
                    smr.materials = pair.materials;
                }
            }
        }
        var mrs = t.GetComponentsInChildren<MeshRenderer>();
        foreach (var mr in mrs)
        {
            foreach (var pair in pairs)
            {
                if (mr.name.Equals(pair.name, StringComparison.OrdinalIgnoreCase))
                {
                    mr.materials = pair.materials;
                }
            }
        }
    }
}
