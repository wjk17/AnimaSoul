using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(CopyTransformMatchName))]
public class CopyTransformMatchNameEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var o = (CopyTransformMatchName)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("SetLocalTrans3"))
        {
            o.SetLocalTrans3();
        }
        if (GUILayout.Button("SetMesh"))
        {
            o.SetMesh();
        }
        if (GUILayout.Button("GetPairs"))
        {
            o.GetPairs();
        }
    }
}
#endif
public class CopyTransformMatchName : MonoBehaviour
{
    public Transform from;
    public Transform to;
    public Dictionary<Transform, Transform> pairs;
    public bool Synchro = false;
    private void Update()
    {
        if (Synchro)
        {
            foreach (var p in pairs)
            {
                p.Value.SetTran3(p.Key);
            }
        }
    }
    private void Start()
    {
        if (from == null || to == null) return;
        GetPairs();
    }
    public void GetPairs()
    {
        pairs = new Dictionary<Transform, Transform>();
        List<string> fromNames = new List<string>();
        List<string> toName = new List<string>();
        var froms = from.GetComponentsInChildren<Transform>();
        for (int i = 0; i < froms.Length; i++)
        {
            fromNames.Add(froms[i].name);
        }
        var tos = to.GetComponentsInChildren<Transform>();
        foreach (var t in tos)
        {
            if (t == to) continue;
            var i = fromNames.IndexOf(t.name);
            if (i > 0 && !pairs.ContainsKey(froms[i]))
            {
                pairs.Add(froms[i], t);
            }
        }
    }
    public void SetMesh()
    {
        List<string> fromNames = new List<string>();
        List<string> toName = new List<string>();
        var froms = from.GetComponentsInChildren<Transform>();
        for (int i = 0; i < froms.Length; i++)
        {
            fromNames.Add(froms[i].name);
        }
        var tos = to.GetComponentsInChildren<Transform>();
        foreach (var t in tos)
        {
            if (t == to) continue;
            var i = fromNames.IndexOf(t.name);
            if (i > 0)
            {
                var mf = t.GetComponent<MeshFilter>();
                var mf2 = froms[i].GetComponent<MeshFilter>();
                var smr = t.GetComponent<SkinnedMeshRenderer>();
                var smr2 = froms[i].GetComponent<SkinnedMeshRenderer>();
                if (mf != null && mf2 != null) mf.sharedMesh = mf2.sharedMesh;
                if (smr != null && smr2 != null) smr.sharedMesh = smr2.sharedMesh;
            }
        }
    }
    public void SetLocalTrans3()
    {
        List<string> fromNames = new List<string>();
        List<string> toName = new List<string>();
        var froms = from.GetComponentsInChildren<Transform>();
        for (int i = 0; i < froms.Length; i++)
        {
            fromNames.Add(froms[i].name);
        }
        var tos = to.GetComponentsInChildren<Transform>();
        foreach (var t in tos)
        {
            if (t == to) continue;
            var i = fromNames.IndexOf(t.name);
            if (i > 0)
            {
                t.SetTran3Local(froms[i]);
            }
        }
    }
}
