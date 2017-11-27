using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GLUI
{
    static Material lineMaterial;
    public static void SetLineMaterial()
    {
        if (!lineMaterial)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
        // Apply the line material
        lineMaterial.SetPass(0);
    }
    public static void ClearCmd()
    {
        ASUI.I.glCommands.Clear();
    }
    public static GLUICommand Cmd(GLUICmdType type, params object[] args )
    {
        var cmd = new GLUICommand();
        cmd.type = type;
        cmd.args = args;
        return cmd;
    }
    public static void BeginOrtho() // 正交变换
    {
        ASUI.I.glCommands.Add(Cmd(GLUICmdType.LoadOrtho));
    }
    public static void DrawLine(Vector2 pos1, Vector2 pos2)
    {
        ASUI.I.glCommands.Add(Cmd(GLUICmdType.DrawLineOrtho, pos1, pos2));
    }
    public static void DrawLine(Vector2 pos1, Vector2 pos2,Color color)
    {
        ASUI.I.glCommands.Add(Cmd(GLUICmdType.DrawLineOrtho, pos1, pos2, color));
    }
    // 左下角原点（0,0），右上角（1,1）
    public static void DrawLineOrtho(Vector2 pos1, Vector2 pos2)
    {
        DrawLineOrtho(pos1, pos2, Color.black);
    }
    public static void DrawLineOrtho(Vector2 pos1, Vector2 pos2, Color color)
    {
        pos1.x /= IMUI.scaler.referenceResolution.x;
        pos1.y = IMUI.scaler.referenceResolution.y - pos1.y;
        pos1.y /= IMUI.scaler.referenceResolution.y;
        pos2.x /= IMUI.scaler.referenceResolution.x;
        pos2.y = IMUI.scaler.referenceResolution.y - pos2.y;
        pos2.y /= IMUI.scaler.referenceResolution.y;
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(pos1);
        GL.Vertex(pos2);
        GL.End();
    }
    public static void DrawPoint()
    {

    }
    public static void DrawCircle(Vector3 pos, float radius, Color color, float accurracy = 0.01f)
    {
        DrawCircle(pos.x, pos.y, pos.z, radius, color, accurracy);
    }
    static void DrawCircle(float x, float y, float z, float r, Color color, float accuracy)
    {
        SetLineMaterial();
        GL.PushMatrix();
        //绘制2D图像    
        GL.LoadOrtho();

        float stride = r * accuracy;
        float size = 1 / accuracy;
        float x1 = x, x2 = x, y1 = 0, y2 = 0;
        float x3 = x, x4 = x, y3 = 0, y4 = 0;

        double squareDe;
        squareDe = r * r - Math.Pow(x - x1, 2);
        squareDe = squareDe > 0 ? squareDe : 0;
        y1 = (float)(y + Math.Sqrt(squareDe));
        squareDe = r * r - Math.Pow(x - x1, 2);
        squareDe = squareDe > 0 ? squareDe : 0;
        y2 = (float)(y - Math.Sqrt(squareDe));
        for (int i = 0; i < size; i++)
        {
            x3 = x1 + stride;
            x4 = x2 - stride;
            squareDe = r * r - Math.Pow(x - x3, 2);
            squareDe = squareDe > 0 ? squareDe : 0;
            y3 = (float)(y + Math.Sqrt(squareDe));
            squareDe = r * r - Math.Pow(x - x4, 2);
            squareDe = squareDe > 0 ? squareDe : 0;
            y4 = (float)(y - Math.Sqrt(squareDe));

            //绘制线段
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex(new Vector3(x1 / Screen.width, y1 / Screen.height, z));
            GL.Vertex(new Vector3(x3 / Screen.width, y3 / Screen.height, z));
            GL.End();
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex(new Vector3(x2 / Screen.width, y1 / Screen.height, z));
            GL.Vertex(new Vector3(x4 / Screen.width, y3 / Screen.height, z));
            GL.End();
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex(new Vector3(x1 / Screen.width, y2 / Screen.height, z));
            GL.Vertex(new Vector3(x3 / Screen.width, y4 / Screen.height, z));
            GL.End();
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex(new Vector3(x2 / Screen.width, y2 / Screen.height, z));
            GL.Vertex(new Vector3(x4 / Screen.width, y4 / Screen.height, z));
            GL.End();

            x1 = x3;
            x2 = x4;
            y1 = y3;
            y2 = y4;
        }
        GL.PopMatrix();
    }
}
