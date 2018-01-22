/******************************************************************** 
 FileName: OutlinePostEffectCmdBuffer.cs 
 Description: 后处理描边效果CommandBuffer版本 
 Created: 2017/06/07 
 by puppet_master 
*********************************************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;

[Serializable]
public struct SubMesh
{
    public Renderer render;
    public int index;
    public SubMesh(Renderer render, int index)
    {
        this.render = render;
        this.index = index;
    }
}
public class OutlinePostEffectCmdBuffer : PostEffectBase
{
    private CommandBuffer commandBuffer = null;
    private Material outlineMaterial = null;
    //描边prepass shader（渲染纯色贴图的shader） 以及渲染深度 
    public Shader outlineShader = null;

    public RenderTexture sceneDepth;
    //目标对象  
    public PEOutline[] targetPEO;
    public DepthTextureMode mode;

    public RenderTexture targetColor;
    public RenderTexture targetDepth;
    public RenderTexture targetFinal;
    public Camera cam;
    public RenderTexture temp1;
    public RenderTexture temp2;
    public RenderTexture tempFinal;
    public RenderTexture tempFinal2;
    public bool inited = true;
    public bool sobel = true;
    private void OnEnable()
    {
        targetPEO = FindObjectsOfType<PEOutline>();

        inited = false;
        cam = GetComponent<Camera>();
        cam.depthTextureMode = mode;
    }
    void InitRT(int width, int height)
    {
        sceneDepth = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        targetColor = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default);
        targetDepth = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.Depth, RenderTextureReadWrite.Linear);
        targetFinal = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default);
        temp1 = RenderTexture.GetTemporary(width, height, 0);
        temp2 = RenderTexture.GetTemporary(width, height, 0);
        tempFinal = RenderTexture.GetTemporary(width, height, 0);
        tempFinal2 = RenderTexture.GetTemporary(width, height, 0);

        _Material.SetTexture("_SceneDepthTex", sceneDepth);
        outlineMaterial.SetTexture("_DepthTex", targetDepth);
    }
    [ContextMenu("Init")]
    void Init(int width, int height)
    {
        if (outlineShader == null)
            return;
        if (outlineMaterial == null)
            outlineMaterial = new Material(outlineShader);

        inited = true;
        InitRT(width, height);

    }
    public Color ClearRenderTargetColor;
    public GameObject targetView;
    public List<Renderer> listView;
    bool SubMeshContains(List<SubMesh> list, SubMesh submesh)
    {
        foreach (var item in list)
        {
            if (item.render == submesh.render && item.index == submesh.index) return true;
        }
        return false;
    }
    void RTCommandBuffer(GameObject target, PEOutline peo)
    {
        if (commandBuffer != null) commandBuffer.Clear();
        commandBuffer = new CommandBuffer();
        if (cam.actualRenderingPath == RenderingPath.Forward)
            commandBuffer.Blit(BuiltinRenderTextureType.Depth, sceneDepth);
        else commandBuffer.Blit(BuiltinRenderTextureType.ResolvedDepth, sceneDepth); //渲染整个场景深度        

        commandBuffer.SetRenderTarget(targetColor, targetDepth);
        commandBuffer.ClearRenderTarget(true, true, ClearRenderTargetColor);

        var renderers = target.GetComponentsInChildren<Renderer>();
        int c = 0;
        var subMeshUsed = new List<SubMesh>();
        foreach (Renderer r in renderers)
        {
            c++;
            if (!r.enabled) continue;

            if (c == 1 && peo != null && peo.submeshIndex > -1)
            {
                commandBuffer.DrawRenderer(r, outlineMaterial, peo.submeshIndex, 0); // 要描边的纯色轮廓快
                subMeshUsed.Add(new SubMesh(r, peo.submeshIndex));
            }
            else
            {
                var mf = r.GetComponent<MeshFilter>();
                var smr = r.GetComponent<SkinnedMeshRenderer>();
                Mesh mesh = null;
                if (mf != null) mesh = mf.sharedMesh;
                else if (smr != null) mesh = smr.sharedMesh;
                for (int i = 0; i < mesh.subMeshCount; i++)
                {
                    commandBuffer.DrawRenderer(r, outlineMaterial, i, 0); // ，要描边的纯色轮廓快
                    subMeshUsed.Add(new SubMesh(r, i));
                }
            }
        }
        foreach (var inc in peo.sobelInclude)
        {
            commandBuffer.DrawRenderer(inc.render, outlineMaterial, inc.index, 0); // 额外的，描边要包括的纯色轮廓快
            subMeshUsed.Add(inc);

        }
        var bgRenderers = FindObjectsOfType<Renderer>();
        foreach (var r in bgRenderers)
        {
            if (!r.enabled) continue;

            var mf = r.GetComponent<MeshFilter>();
            var smr = r.GetComponent<SkinnedMeshRenderer>();
            Mesh mesh = null;
            if (mf != null) mesh = mf.sharedMesh;
            else if (smr != null) mesh = smr.sharedMesh;

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                if (SubMeshContains(subMeshUsed, new SubMesh(r, i))) continue;
                if (SubMeshContains(peo.maskExclude, new SubMesh(r, i))) continue;
                if (peo.maskExcludeAll) continue;//不被任何物体遮挡
                commandBuffer.DrawRenderer(r, outlineMaterial, i, 2); // 用来绘制遮挡关系的 其他物体
            }
        }
        Color clr;
        ColorUtility.TryParseHtmlString("0A0A0A00", out clr);
        commandBuffer.Blit(targetColor, targetFinal, outlineMaterial, 1);//合并颜色和深度，深度存放到g通道，颜色b通道
    }
    void OnDisable()
    {
        if (temp1)
        {
            RenderTexture.ReleaseTemporary(temp1);
            RenderTexture.ReleaseTemporary(temp2);
            temp1 = null;
            temp2 = null;
        }
        if (sceneDepth)
        {
            RenderTexture.ReleaseTemporary(sceneDepth);
            sceneDepth = null;
        }
        if (targetColor)
        {
            RenderTexture.ReleaseTemporary(targetColor);
            targetColor = null;
        }
        if (targetDepth)
        {
            RenderTexture.ReleaseTemporary(targetDepth);
            targetDepth = null;
        }
        if (targetFinal)
        {
            RenderTexture.ReleaseTemporary(targetFinal);
            targetFinal = null;
        }
        if (outlineMaterial)
        {
            DestroyImmediate(outlineMaterial);
            outlineMaterial = null;
        }
        if (commandBuffer != null)
        {
            commandBuffer.Release();
            commandBuffer = null;
        }
    }
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!inited) Init(source.width, source.height);
        if (_Material && outlineMaterial && targetPEO != null && targetPEO.Length > 0)
        {
            Graphics.Blit(source, tempFinal);
            foreach (var peo in targetPEO)
            {
                if (peo == null || !peo.enabled) continue;
                //创建描边prepass的command buffer,直接通过Graphic执行
                RTCommandBuffer(peo.gameObject, peo);
                Graphics.ExecuteCommandBuffer(commandBuffer);
                if (sobel) Sobel(peo, tempFinal, tempFinal2);
                else BlurAndAdd(peo, tempFinal, tempFinal2);

                Graphics.Blit(tempFinal2, tempFinal);
            }
            Graphics.Blit(tempFinal2, destination);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
    public bool blur;
    public bool blurB;
    void Sobel(PEOutline peo, RenderTexture source, RenderTexture destination)
    {
        _Material.SetFloat("_SampleDistance", peo.sampleDist);
        //Sobel
        Graphics.Blit(targetFinal, temp1, _Material, 3);

        if (blur) Blur(temp1, temp2, peo);//模糊
        if (blurB) BlurB(temp1, temp2, peo);//模糊

        //轮廓图和场景图叠加  
        _Material.SetTexture("_BlurTex", temp1);
        //通过Command Buffer可以设置自定义材质的颜色              
        _Material.SetFloat("_DepthOffset", peo.depthOffset);
        _Material.SetFloat("_DepthExponent", peo.depthExponent);
        _Material.SetColor("_OutlineCol", peo.outLineColor);
        _Material.SetFloat("_OutlineStrength", peo.outLineStrength);
        Graphics.Blit(source, destination, _Material, 2);
    }
    void BlurB(RenderTexture src, RenderTexture temp, PEOutline peo)
    {
        //高斯模糊，两次模糊，横向纵向，使用pass0进行高斯模糊
        _Material.SetVector("_offsets", new Vector4(0, peo.samplerScaleInSobel, 0, 0));
        Graphics.Blit(src, temp, _Material, 4);
        _Material.SetVector("_offsets", new Vector4(peo.samplerScaleInSobel, 0, 0, 0));
        Graphics.Blit(temp, src, _Material, 4);

        //如果有叠加再进行迭代模糊处理  
        for (int i = 0; i < peo.iteration; i++)
        {
            _Material.SetVector("_offsets", new Vector4(0, peo.samplerScaleInSobel, 0, 0));
            Graphics.Blit(src, temp, _Material, 4);
            _Material.SetVector("_offsets", new Vector4(peo.samplerScaleInSobel, 0, 0, 0));
            Graphics.Blit(temp, src, _Material, 4);
        }
    }
    void Blur(RenderTexture src, RenderTexture temp, PEOutline peo)
    {
        //高斯模糊，两次模糊，横向纵向，使用pass0进行高斯模糊
        _Material.SetVector("_offsets", new Vector4(0, peo.samplerScaleFinal, 0, 0));
        Graphics.Blit(src, temp, _Material, 0);
        _Material.SetVector("_offsets", new Vector4(peo.samplerScaleFinal, 0, 0, 0));
        Graphics.Blit(temp, src, _Material, 0);

        //如果有叠加再进行迭代模糊处理  
        for (int i = 0; i < peo.iteration; i++)
        {
            _Material.SetVector("_offsets", new Vector4(0, peo.samplerScaleFinal, 0, 0));
            Graphics.Blit(src, temp, _Material, 0);
            _Material.SetVector("_offsets", new Vector4(peo.samplerScaleFinal, 0, 0, 0));
            Graphics.Blit(temp, src, _Material, 0);
        }
    }
    private void BlurAndAdd(PEOutline peo, RenderTexture source, RenderTexture destination)
    {

        //高斯模糊，两次模糊，横向纵向，使用pass0进行高斯模糊
        _Material.SetVector("_offsets", new Vector4(0, peo.samplerScale, 0, 0));
        Graphics.Blit(targetFinal, temp1, _Material, 0);
        _Material.SetVector("_offsets", new Vector4(peo.samplerScale, 0, 0, 0));
        Graphics.Blit(temp1, temp2, _Material, 0);

        //如果有叠加再进行迭代模糊处理  
        for (int i = 0; i < peo.iteration; i++)
        {
            _Material.SetVector("_offsets", new Vector4(0, peo.samplerScale, 0, 0));
            Graphics.Blit(temp2, temp1, _Material, 0);
            _Material.SetVector("_offsets", new Vector4(peo.samplerScale, 0, 0, 0));
            Graphics.Blit(temp1, temp2, _Material, 0);
        }

        ////用模糊图和原始图计算出轮廓图  
        //_Material.SetTexture("_BlurTex", temp2);
        //Graphics.Blit(targetFinal, temp1, _Material, 1);


        //轮廓图和场景图叠加  
        _Material.SetTexture("_BlurTex", temp2);
        //通过Command Buffer可以设置自定义材质的颜色              
        _Material.SetFloat("_DepthOffset", peo.depthOffset);
        _Material.SetFloat("_DepthExponent", peo.depthExponent);
        _Material.SetColor("_OutlineCol", peo.outLineColor);
        _Material.SetFloat("_OutlineStrength", peo.outLineStrength);
        Graphics.Blit(source, destination, _Material, 2);
    }
}