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
    }
}
#endif
public class CopyTransformMatchName : MonoBehaviour
{
    public Transform from;
    public Transform to;
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
