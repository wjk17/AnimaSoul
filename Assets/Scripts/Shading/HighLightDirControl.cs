using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class HighLightDirControl : MonoBehaviour
{
    public Material[] mats;
    public Transform target;
    Transform prevTarget;
    public Vector3 weight;
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
        Vector3 dir = mats[0].GetVector("_HighClrDir");
        dir *= (transform.position - target.position).magnitude;
        transform.position = target.position + dir;
        weight = mats[0].GetVector("_ViewDirWeight");
    }
    void Update()
    {
        if (!update || target == null) return;
        if (prevTarget != target)
        {
            GetProps();
            prevTarget = target;
        }
        var dir = transform.position - target.position;
        dir = transform.InverseTransformDirection(dir);
        transform.GetChild(0).forward = -dir;
        //mats[0].SetVector("_HighClrDir", dir);
        //mats[0].SetVector("_ViewDirWeight", weight);
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].SetVector("_HighClrDir", dir);
            mats[i].SetVector("_ViewDirWeight", weight);
        }
    }
}
