using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParent : MonoBehaviour
{
    public Transform[] childs;
    public Transform parent;
    //private void Reset()
    //{
    //    parent = transform;
    //}
    void Awake()
    {
        foreach (var child in childs)
        {
            child.SetParent(parent, true);
        }
    }
}
