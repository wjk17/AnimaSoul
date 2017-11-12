using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeltaRotationMgr : MonoBehaviour
{
    public DeltaRotation[] coms;
    public float length = 1;
    public bool depthTest = false;
    void Start()
    {
        coms = GetComponentsInChildren<DeltaRotation>();
    }
    void Update()
    {
        foreach (var com in coms)
        {
            com.coord.DrawRay(com.transform, com.euler, length, depthTest);
        }
    }
}
