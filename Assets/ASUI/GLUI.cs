using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GLUI
{
    public static int accurate = 100;
    public static float radius = 3.0f;

    static Material lineMaterial;
    static CameraEventWrapper cameraEvent;
    public static void Init()
    {
        if (cameraEvent != null) return;
        cameraEvent = ASUI.I.camera.GetComOrAdd<CameraEventWrapper>();
        cameraEvent.onRenderObject = RenderObject;
        cameraEvent.onPostRender = PostRender;
    }
    static void SetLineMaterial()
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
    static void RenderObject() // Test
    {
        SetLineMaterial();
        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(ASUI.I.camera.transform.localToWorldMatrix);
        // Draw lines
        GL.Begin(GL.LINES);
        DrawLineFan(accurate, radius);
        GL.End();
        GL.PopMatrix();
    }
    static void PostRender() // Test
    {
        SetLineMaterial();
        GL.LoadOrtho();//设置绘制2D图像          

        //画从左下到右上的线  
        GL.Begin(GL.LINES);
        GL.Color(Color.white);
        GL.Vertex(new Vector2(0, 0));
        GL.Vertex(new Vector2(1, 1));
        GL.End();

        //画从左上到右下的线  
        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        GL.Vertex(new Vector2(0, 1));
        GL.Vertex(new Vector2(1, 0));
        GL.End();
    }
    public static void BeginOrtho()
    {
        SetLineMaterial();
        GL.LoadOrtho();//设置绘制2D图像       
    }
    public static void DrawLineOrtho(Vector2 pos1, Vector2 pos2)
    {
        DrawLineOrtho(pos1, pos2, Color.black);
    }
    // 左上角原点（0,0），右下（1,1）
    public static void DrawLineOrtho(Vector2 pos1, Vector2 pos2, Color color)
    {
        pos1.x /= IMUI.scaler.referenceResolution.x;
        pos1.y /= IMUI.scaler.referenceResolution.y;
        pos2.x /= IMUI.scaler.referenceResolution.x;
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
    private static void DrawLineFan(int count, float radius)
    {
        for (int i = 0; i < count; ++i)
        {
            float a = i / (float)count;
            float angle = a * Mathf.PI * 2;
            // Vertex colors change from red to green
            GL.Color(new Color(a, 1 - a, 0, 0.8F));
            // One vertex at transform position
            GL.Vertex3(0, 0, 0);
            // Another vertex at edge of circle
            GL.Vertex3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        }
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
