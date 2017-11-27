using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalEulerAngles : MonoBehaviour
{
    public Vector3 localEulerAngles;
    public bool set;
    void Update()
    {
        if (set) transform.localEulerAngles = localEulerAngles;
        else localEulerAngles = transform.localEulerAngles;
    }
}
