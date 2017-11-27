using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Distort : MonoBehaviour
{    
    //public override bool CheckResources()
    //{
    //    CheckSupport(true);

    //    mat = CheckShaderAndCreateMaterial(shader, mat);

    //    if (!isSupported)
    //        ReportAutoDisable();
    //    return isSupported;
    //}
    //public Shader shader;
    public Material mat = null;
    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //if (CheckResources() == false)
        //{
        //    Graphics.Blit(source, destination);
        //    return;
        //}
        Graphics.Blit(source, destination, mat);
        //Graphics.Blit(source, destination);
    }
}
