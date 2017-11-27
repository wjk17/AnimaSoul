using System;
using UnityEngine;
public class CameraEventWrapper : MonoBehaviour
{
    public Action onRenderObject;
    public Action onPostRender;
    public Action onWillRenderObject;
    public Action<RenderTexture, RenderTexture> onRenderImage;
    public Action onPreRender;
    public Action onPreCull;
    public void OnRenderObject()
    {
        if (onRenderObject != null) onRenderObject();
    }
    void OnPostRender()//所有GL的绘制只能写在这个方法里面，并将此脚本赋予给主摄像机  
    {
        if (onPostRender != null) onPostRender();
    }
    private void OnWillRenderObject()
    {
        if (onWillRenderObject != null) onWillRenderObject();
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (onRenderImage != null) onRenderImage(source, destination);
        else Graphics.Blit(source, destination);
    }
    private void OnPreRender()
    {
        if (onPreRender != null) onPreRender();
    }
    private void OnPreCull()
    {
        if (onPreCull != null) onPreCull();
    }
}