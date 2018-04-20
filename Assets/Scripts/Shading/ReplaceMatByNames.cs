using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(ReplaceMatByNames))]
public class ReplaceMatByNamesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var o = (ReplaceMatByNames)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Set 1"))
        {
            o.Set(1);
        }
        if (GUILayout.Button("Set 2"))
        {
            o.Set(2);
        }
        if (GUILayout.Button("Set 3"))
        {
            o.Set(3);
        }
    }
}
#endif
[ExecuteInEditMode]
public class ReplaceMatByNames : MonoBehaviour
{
    public Material[] mats1;
    public Material[] mats2;
    public Material[] mats3;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Set(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Set(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Set(3);
    }
    public void Set(int index)
    {
        Material[] mats;
        if (index == 1) mats = mats1;
        else if (index == 2) mats = mats2;
        else if (index == 3) mats = mats3;
        else throw null;
        foreach (var render in GetComponentsInChildren<Renderer>(true))
        {
            var list = new List<Material>();
            for (int i = 0; i < render.sharedMaterials.Length; i++)
            {
                var mat = GetMatByName(mats, render.sharedMaterials[i].name);
                //Debug.Log((mat != null)? mat.name);

                if (mat != null) { list.Add(mat); }// Debug.Log(render.name); }
                else list.Add(render.sharedMaterials[i]);
            }
            render.sharedMaterials = list.ToArray();
        }
    }
    Material GetMatByName(Material[] mats, string name)
    {
        foreach (var mat in mats)
        {
            if (mat.name == name) return mat;
        }
        return null;
    }
}
