using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AvatorData
{
    public List<TransDOF> asts;
    public string name = "Default Avatar Data";
    public AvatorData() { }
    public TransDOF GetTransDOF(Bone bone)
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
        DrawBones(boneColor, depthTest);
        DrawCoords(drawLineLength, depthTest);
    }
    void DrawBones(Color boneColor, bool depthTest)
    {
        // exclude root(0) and hips(1)
        for (int i = 2; i < asts.Count; i++)
        {
            var t = asts[i].transform;
            if (t != null) Debug.DrawLine(t.position, t.parent.position, boneColor, 0, depthTest);
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
            if (t.transform != null)
                t.Update();
        }
    }
}
