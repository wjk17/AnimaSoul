using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class ReplacementShader : MonoBehaviour {
    public Shader replacementShader;
    public string replacementTag;
    void OnEnable () {
        var camera = GetComponent<Camera>();        
        if (camera != null) camera.SetReplacementShader(replacementShader, replacementTag);
    }
	void Update () {
		
	}
}
