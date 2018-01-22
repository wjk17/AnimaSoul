using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PEOutline : MonoBehaviour {
    public List<SubMesh> sobelInclude = new List<SubMesh>();
    public List<SubMesh> maskExclude = new List<SubMesh>();
    public bool maskExcludeAll = false;
    public int submeshIndex = -1;//all

    [Header("Sobel描边")]
    //Sobel采样距离
    public float sampleDist = 0.8f;
    public float samplerScaleInSobel = 0.3f;
    public float samplerScaleFinal = 0.3f;

    [Header("高斯模糊")]
    //采样率  
    public float samplerScale = 5;
    //迭代次数  
    public int iteration = 0;
    //描边颜色  
    public Color outLineColor = Color.black;
    //描边强度  
    [Range(0.0f, 10.0f)]
    public float outLineStrength = 3.0f;
    public float depthOffset = 0; // 深度偏移，不一定使用，不一定好用。
    public float depthExponent = 1; // 对深度值进行一次pow()
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
