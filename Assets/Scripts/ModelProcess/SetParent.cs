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
    [ContextMenu("Clear")]
    void Clear()
    {
        var list = new List<Transform>();
        foreach (var child in childs)
        {
            if (child != null) list.Add(child);
        }
        childs = list.ToArray();
    }
    void Awake()
    {
        foreach (var child in childs)
        {
            child.SetParent(parent, true);
        }
    }
}
