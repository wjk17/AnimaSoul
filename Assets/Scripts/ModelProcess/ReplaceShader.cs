using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceShader : MonoBehaviour
{
    public Shader shaderOrigin;
    public Shader shaderReplacement;
    public GameObject[] gos;
    [ContextMenu("Replace")]
    void Start()
    {
        foreach (var go in gos)
        {
            var render = go.GetComponent<Renderer>();
            if (render == null) continue;
            foreach (var mat in render.sharedMaterials)
            {
                if (mat != null && mat.shader == shaderOrigin)
                {
                    mat.shader = shaderReplacement;
                    Debug.Log(mat.name);
                }
            }
        }

        //foreach (var go in gos)
        //{
        //    var mat = go.GetComponent<Material>();

        //    if (mat != null && mat.shader == shaderOrigin)
        //    {
        //        mat.shader = shaderReplacement;
        //        Debug.Log(mat.name);
        //    }

        //}

        //var renders = FindObjectsOfType<Renderer>();
        //foreach (var item in renders)
        //{
        //    foreach (var mat in item.sharedMaterials)
        //    {
        //        if (mat != null && mat.shader == shaderOrigin)
        //        {
        //            mat.shader = shaderReplacement;
        //            Debug.Log(mat.name);
        //        }
        //    }
        //}
        //var mats = FindObjectsOfType<Material>();
        //foreach (var mat in mats)
        //{
        //    if (mat != null && mat.shader == shaderOrigin)
        //    {
        //        mat.shader = shaderReplacement;
        //        Debug.Log(mat.name);
        //    }
        //}
    }
}
