using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(ASAvatar))]
public class ASAvatarEditor : E_ShowButtons<ASAvatar> { }
#endif

[Serializable]
public class AvatarSetting
{
    public List<ASTransDOF> asts;
    public string name = "Default Avatar Setting";
    public AvatarSetting() { }
    public ASTransDOF GetTransDOF(ASBone bone)
    {
        foreach (var ast in asts)
        {
            if (bone == ast.dof.bone)
                return ast;
        }
        return null;
    }

    public void DrawLines(Color boneColor, float drawLineLength, bool depthTest)
    {
        DrawBones(boneColor,depthTest);
        DrawCoords(drawLineLength, depthTest);
    }
    void DrawBones(Color boneColor, bool depthTest)
    {
        // exclude root(0) and hips(1)
        for (int i = 2; i < asts.Count; i++)
        {
            var ast = asts[i];
            var t = ast.transform;
            Debug.DrawLine(t.position, t.parent.position, boneColor, 0, depthTest);
        }
    }
    void DrawCoords(float drawLineLength, bool depthTest)
    {
        foreach (var t in asts)
        {
            if (t.transform != null)
                t.coord.DrawRay(t.transform, t.euler, drawLineLength, depthTest);
        }
    }
    internal void UpdateTrans()
    {
        foreach (var t in asts)
        {
            t.Update();
        }
    }
}
