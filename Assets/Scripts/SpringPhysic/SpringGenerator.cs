using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChan;
using System;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(SpringGenerator))]
public class SpringGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var o = (SpringGenerator)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate"))
        {
            o.Generate();
        }
        if (GUILayout.Button("Apply"))
        {
            o.Apply(o.rig);
        }
        if (GUILayout.Button("ClearBones"))
        {
            o.ClearBones();
        }
    }
}
#endif

public class SpringGenerator : MonoBehaviour
{
    public Transform rig;
    public Transform[] rigs;
    public Vector3 boneAxis = Vector3.up;
    public SpringCollider[] coliders;
    public SpringCollider[] coliders2;
    public float radius;
    public void Generate(params Transform[] rigs)
    {
        foreach (var rig in rigs)
        {
            var mgr = rig.GetComponent<SpringManager>();
            var springBones = new List<SpringBone>();
            foreach (var t in rig.GetComponentsInChildren<Transform>())
            {
                if (t.name.IndexOf("bone", StringComparison.OrdinalIgnoreCase) < 0) continue;
                if (t != rig && t.childCount > 0)
                {
                    var bone = t.GetComponent<SpringBone>();
                    if (bone == null) bone = t.gameObject.AddComponent<SpringBone>();
                    springBones.Add(bone);
                }
            }
            mgr.springBones = springBones.ToArray();
            Apply(rig);
        }
    }
    public void Generate()
    {
        Generate(rig);
    }
    public void Apply(Transform rig)
    {
        foreach (var bone in rig.GetComponentsInChildren<SpringBone>())
        {
            bone.child = bone.transform.GetChild(0);
            bone.boneAxis = bone.child.localPosition.normalized;
            if ((bone.name.IndexOf(".r") != -1 || bone.name.IndexOf(".l") != -1) == false)
            {
                bone.colliders = coliders2;
            }
            else
            {
                bone.colliders = coliders;
            }
            bone.radius = radius;
        }
    }

    internal void ClearBones()
    {
        var bones = rig.GetComponentsInChildren<SpringBone>();
        foreach (var bone in bones)
        {
            DestroyImmediate(bone);
        }
    }
}

