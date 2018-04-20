using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class DirectionalLightPosControl : MonoBehaviour
{
    public Material mat;
    public Transform target;
    Transform prevTarget;
    public Vector3 weight;
    public Transform child;
    public bool update;
    private void OnEnable()
    {
        GetProps();
    }
    private void Start()
    {
        GetProps();
    }
    void GetProps()
    {
        Vector3 dir = mat.GetVector("_BaseCustomDir");
        dir *= (transform.position - target.position).magnitude;
        transform.position = target.position + dir;
        weight = mat.GetVector("_CustomDirWeight");
    }
    void SetLightDir(Vector3 dir)
    {
        dir = dir.normalized;
        mat.SetVector("_BaseCustomDir", dir);
        transform.GetChild(0).forward = -dir;
    }
    void Update()
    {
        if (!update || target == null) return;

        if(prevTarget != target)
        {
            GetProps();
            prevTarget = target;
        }

        SetLightDir(transform.position - target.position);
        mat.SetVector("_CustomDirWeight", weight);
    }
}
