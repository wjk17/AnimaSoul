using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GLUI
{
    static Material lineMaterial;
    internal static int commandOrder
    {
        set { _commandOrder = value; }
        get { var order = _commandOrder; if (!keepOrder) _commandOrder = 0; return order; }//每次使用后归0
    }
    static int _commandOrder;
    static bool keepOrder = false;
    public static void BeginOrder(int order)
    {
        _commandOrder = order;
        keepOrder = true;
    }
    public static void EndOrder()
    {
        keepOrder = false;
    }
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
    public static GLUICommand Cmd(int order, GLUICmdType type, params object[] args)
    {
        var cmd = new GLUICommand();
        cmd.order = order;
        cmd.type = type;
        cmd.args = args;
        return cmd;
    }
    public static void BeginOrtho() // 正交变换
    {
        ASUI.I.AddCommand(Cmd(-1, GLUICmdType.LoadOrtho));
    }
    //左上角坐标
    public static void DrawLine(Vector2 p1, Vector2 p2)
    {
        ASUI.I.AddCommand(Cmd(commandOrder, GLUICmdType.DrawLineOrtho, p1, p2));
    }
    public static void DrawLine(Vector2 p1, Vector2 p2, Color color)
    {
        ASUI.I.AddCommand(Cmd(commandOrder, GLUICmdType.DrawLineOrtho, p1, p2, color));
    }
    public static void DrawLine(Vector2 p1, Vector2 p2, float width)
    {
        ASUI.I.AddCommand(Cmd(commandOrder, GLUICmdType.DrawLineOrtho, p1, p2, width));
    }
    public static void DrawLine(Vector2 p1, Vector2 p2, float width, Color color)
    {
        ASUI.I.AddCommand(Cmd(commandOrder, GLUICmdType.DrawLineOrtho, p1, p2, width, color));
    }
    public static void DrawLineWidth(Vector2 p1, Vector2 p2, float width)
    {
        DrawLineWidth(p1, p2, width, Color.black);
    }
    public static void DrawLineWidth(Vector2 p1, Vector2 p2, float width, Color color)
    {
        var v = p2 - p1;
        var v2 = p1 - p2;
        width *= 0.5f;
        var p1a = p1 + new Vector2(-v.y, v.x).normalized * width;
        var p1b = p1 + new Vector2(v.y, -v.x).normalized * width;
        var p2a = p2 + new Vector2(-v2.y, v2.x).normalized * width;
        var p2b = p2 + new Vector2(v2.y, -v2.x).normalized * width;
        DrawQuads(p1a, p1b, p2a, p2b, color);
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
    public static void DrawSquareSoild()
    {

    }
    internal static void DrawSquare(Vector2 p, float wh)
    {
        DrawSquare(p, wh, Color.black);
    }
    internal static void DrawSquare(Vector2 p, float wh, Color color)
    {
        wh *= 0.5f;
        var p1 = p - Vector2.one * wh; // lt
        var p2 = new Vector2(p.x + wh, p.y - wh); // rt
        var p3 = p + Vector2.one * wh; // rb
        var p4 = new Vector2(p.x - wh, p.y + wh); // lb
        DrawLine(p1, p2, color);
        DrawLine(p2, p3, color);
        DrawLine(p3, p4, color);
        DrawLine(p4, p1, color);
    }
    internal static void DrawSquare(Vector2 p, float wh, float lineWidth)
    {
        DrawSquare(p, wh, lineWidth, Color.black);
    }
    internal static void DrawSquare(Vector2 p, float wh, float lineWidth, Color color)
    {
        wh *= 0.5f;
        var p12a = p - Vector2.one * wh;
        var p12b = new Vector2(p.x + wh, p.y - wh);
        var p23a = new Vector2(p.x + wh, p.y - wh);
        var p23b = p + Vector2.one * wh;
        var p34a = p + Vector2.one * wh;
        var p34b = new Vector2(p.x - wh, p.y + wh);
        var p41a = new Vector2(p.x - wh, p.y + wh);
        var p41b = p - Vector2.one * wh;
        lineWidth *= 0.5f;
        var width = lineWidth * 0.5f;
        p12a.x -= width;
        p12b.x += width;
        p34a.x += width;
        p34b.x -= width;
        DrawLine(p12a, p12b, lineWidth, color);
        DrawLine(p23a, p23b, lineWidth, color);
        DrawLine(p34a, p34b, lineWidth, color);
        DrawLine(p41a, p41b, lineWidth, color);
    }
    public static void DrawQuads(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Color color)
    {
        DrawQuads(p1.x, p1.y, p2.x, p2.y, p3.x, p3.y, p4.x, p4.y, color);
    }
    public static void DrawQuads(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, Color color)
    {
        GL.Begin(GL.QUADS);
        GL.Color(color);
        y1 = IMUI.scaler.referenceResolution.y - y1;
        y2 = IMUI.scaler.referenceResolution.y - y2;
        y3 = IMUI.scaler.referenceResolution.y - y3;
        y4 = IMUI.scaler.referenceResolution.y - y4;
        GL.Vertex3(x1 / IMUI.scaler.referenceResolution.x, y1 / IMUI.scaler.referenceResolution.y, 0);
        GL.Vertex3(x2 / IMUI.scaler.referenceResolution.x, y2 / IMUI.scaler.referenceResolution.y, 0);
        GL.Vertex3(x3 / IMUI.scaler.referenceResolution.x, y3 / IMUI.scaler.referenceResolution.y, 0);
        GL.Vertex3(x4 / IMUI.scaler.referenceResolution.x, y4 / IMUI.scaler.referenceResolution.y, 0);
        GL.End();
    }
    public static void DrawCircle(Vector3 pos, float radius, Color color, float accurracy = 0.01f)
    {
        DrawCircle(pos.x, pos.y, pos.z, radius, color, accurracy);
    }
    static void DrawCircle(float x, float y, float z, float r, Color color, float accuracy)
    {
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
    }
}
