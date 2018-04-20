using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
[ExecuteInEditMode]
public class AdditiveShadowCast : MonoBehaviour
{
    public Renderer[] renders;
    public Material shadowMat;
    public int pass;
    public new Light light;
    public LightEvent lightEvent;
    CommandBuffer commandbuf;
    public Vector3 offset;
    Vector3 prevOffset;
    private void Reset()
    {
        light = GetComponent<Light>();
    }
    [ContextMenu("清除无效渲染器")]
    public void ClearNullRef()
    {
        var list = new List<Renderer>();
        foreach (var render in renders)
        {
            if (render != null) list.Add(render);
        }
        renders = list.ToArray();
    }
    void OnEnable()
    {
        AddCmdBuf();
    }
    private void Update()
    {
        if(prevOffset != offset)
        {
            prevOffset = offset;
            AddCmdBuf();
        }
    }
    void AddCmdBuf()
    {
        if (light == null) light = GetComponent<Light>();
        if (light == null) return;
        if (renders == null) return;
        commandbuf = new CommandBuffer();
        commandbuf.name = "AdditiveShadowCast";
        SkinnedMeshRenderer smr;
        MeshFilter mf;
        Mesh mesh;
        var matrix = Matrix4x4.Translate(offset);
        foreach (var render in renders)
        {
            smr = render.GetComponent<SkinnedMeshRenderer>();
            mf = render.GetComponent<MeshFilter>();
            if (smr != null) mesh = smr.sharedMesh;
            else if (mf != null) mesh = mf.sharedMesh;
            else throw null;
            //commandbuf.DrawRenderer(render, shadowMat, 0, pass);
            commandbuf.DrawMesh(mesh, matrix * render.localToWorldMatrix, shadowMat, 0, pass);
        }
        light.RemoveAllCommandBuffers();
        light.AddCommandBuffer(lightEvent, commandbuf);
    }
}
