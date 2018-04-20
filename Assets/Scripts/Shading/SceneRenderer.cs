using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(SceneRenderer))]
public class SceneRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var o = (SceneRenderer)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("SetRenderList"))
        {
            o.SetRenderList();
        }
        if (GUILayout.Button("AddCommandBuffer"))
        {
            o.AddCommandBuffer();
        }
        //if (GUILayout.Button("RenderCommandBuffer"))
        //{
        //    o.RenderCommandBuffer();
        //}
    }
}
#endif

//[ExecuteInEditMode]
public class SceneRenderer : MonoBehaviourInstance<SceneRenderer>
{
    public CommandBuffer commandBuffer;
    public Renderer[] bgRenderers1;
    public Renderer[] bgRenderers;
    public Renderer[] bgRenderers3;
    public bool bMesh;
    public bool bSmr;
    [Header("清除")]
    public bool clearColor;
    public bool clearDepth;
    public Color clearColorValue;
    public float depth;

    [Header("渲染")]
    public int shaderPass = -1;
    public bool useReplaceMaterial;
    public Material replaceMaterial;
    public CameraEvent cameraEvent;
    //void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    Graphics.Blit(source, destination);
    //    RenderCommandBuffer();
    //    Graphics.ExecuteCommandBuffer(commandBuffer);
    //}
    //public bool update;
    // void Update()
    // {
    //     if(update)AddCommandBuffer();
    // }
    private void Start()
    {
        AddCommandBuffer();
    }
    public void SetRenderList()
    {
        // 获取场景中所有渲染器
        var list = new List<Renderer>();
        if (bMesh) list.AddRange(FindObjectsOfType<MeshRenderer>());
        if (bSmr) list.AddRange(FindObjectsOfType<SkinnedMeshRenderer>());
        bgRenderers = list.ToArray();
    }
    public void AddCommandBuffer()
    {
        RenderCommandBuffer();
        var camera = GetComponent<Camera>();
        camera.RemoveAllCommandBuffers();
        camera.AddCommandBuffer(cameraEvent, commandBuffer);
    }
    public void RenderCommandBuffer()
    {
        commandBuffer = new CommandBuffer();
        if (clearDepth || clearColor) commandBuffer.ClearRenderTarget(clearDepth, clearColor, clearColorValue, depth);
        foreach (var r in bgRenderers)
        {
            RenderR(r);
        }
        foreach (var r in bgRenderers3)
        {
            RenderR(r);
        }
    }
    void RenderR(Renderer r)
    {
        if (r == null) return;
        var mf = r.GetComponent<MeshFilter>();
        var smr = r.GetComponent<SkinnedMeshRenderer>();
        Mesh mesh = null;
        if (mf != null) mesh = mf.sharedMesh;
        else if (smr != null) mesh = smr.sharedMesh;

        if (mesh == null) return;
        for (int i = 0; i < mesh.subMeshCount; i++) // 绘制所有子网格。3D软件中的“顶点组”
        {
            if (useReplaceMaterial) commandBuffer.DrawRenderer(r, replaceMaterial, i, shaderPass);
            else commandBuffer.DrawRenderer(r, r.sharedMaterials[i], i, shaderPass);
        }
    }
}
