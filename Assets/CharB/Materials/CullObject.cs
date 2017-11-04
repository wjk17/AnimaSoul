
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullObject : MonoBehaviour
{
    public static CullObject I
    {
        get { if (_ins == null) _ins = FindObjectOfType<CullObject>(); return _ins; }
    }
    private static CullObject _ins;
    public List<GameObject> gos;
    public List<GameObject> _gos;
    public List<int> layerOrigins;
    public List<Material[]> materialsOrigins;
    public string cullingMaskName;
    public Material renderMaterial;
    private void Start()
    {
        gos = new List<GameObject>();
        _gos = new List<GameObject>();
        layerOrigins = new List<int>();
        materialsOrigins = new List<Material[]>();
    }
    private void OnPostRender()
    {
        if (skip) return;
        for (int i = 0; i < _gos.Count; i++)
        {
            if (_gos[i] == null) continue;
            _gos[i].layer = layerOrigins[i];
            _gos[i].GetComponent<Renderer>().materials = materialsOrigins[i];
        }
    }
    bool skip;
    private void OnPreCull()
    {
        _gos = new List<GameObject>(gos);//确保不会中途被修改。
        layerOrigins.Clear();
        materialsOrigins.Clear();
        skip = false;
        for (int i = 0; i < _gos.Count; i++)
        {
            if (_gos[i] == null) { skip = true; return; }//依然有可能出事，因此。
            layerOrigins.Add(gos[i].layer);
            materialsOrigins.Add(_gos[i].GetComponent<Renderer>().materials);
            _gos[i].layer = LayerMask.NameToLayer(cullingMaskName);
            var rend = _gos[i].GetComponent<Renderer>();
            var ms = new Material[rend.materials.Length];
            for (int j = 0; j < ms.Length; j++)
            {
                ms[j] = renderMaterial;
            }
            rend.materials = ms;
        }
    }
}
