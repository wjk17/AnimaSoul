using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(PointLightPosControl))]
public class PointLightPosControlEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var o = (PointLightPosControl)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("GetPropsAndUpdate"))
        {
            o.GetProps();
            o.update = true;
        }
    }
}
#endif
[ExecuteInEditMode]
public class PointLightPosControl : MonoBehaviour
{
    public Material mat;
    public float radius;
    public bool update;
    private void OnEnable()
    {
        GetProps();
    }
    private void Start()
    {
        GetProps();
    }
    public void GetProps()
    {
        transform.position = mat.GetVector("_PointLightPos");
        radius = mat.GetFloat("_PointLightRadius");
    }
    void Update()
    {
        if (!update) return;

        mat.SetVector("_PointLightPos", transform.position);
        mat.SetFloat("_PointLightRadius", radius);
    }
}
