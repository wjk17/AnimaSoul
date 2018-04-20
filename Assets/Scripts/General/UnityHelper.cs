using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourInstance<T> : MonoBehaviour where T : Object
{
    public static T I
    {
        get { if (_i == null) _i = FindObjectOfType<T>(); return _i; }
    }
    private static T _i;
}