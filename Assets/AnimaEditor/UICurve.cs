using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICurve : MonoBehaviour
{
    private RectTransform area;
    public List<Vector2> points;
    public AnimationCurve curve;
    public float squareSize;
    public float lineWidth;
    public int segmentCount = 10;
    void DrawBezier(params Vector2[] p)
    {
        float t;
        Vector2 p1, p2;
        p1 = p[0];
        for (int i = 1; i <= segmentCount; i++)
        {
            t = (float)i / segmentCount;
            p2 = CalBezier(p, t);
            GLUI.DrawLine(p1, p2);
            p1 = p2;
        }
    }//tP + t(1-t)P = 2t(1-t)P
    //2t(1-t)P + t(1-t)P = 3Pt(1-t)

    //k==2
    //i==0
    //Pi = (1-t)P0 + tP1
    //k==3
    //i==0
    //Pi = (1-t)P0 + tP1
    //i==1
    //Pi = (1-t)P0 + tP1
    Vector2 CalBezier(Vector2[] p, float t)
    {
        Vector2 v = Vector2.zero;
        v = p[0] * Mathf.Pow((1 - t), 3) +
            3 * p[1] * t * Mathf.Pow((1 - t), 2) +
            3 * p[2] * Mathf.Pow(t, 2) * (1 - t) +
            p[3] * Mathf.Pow(t, 3);
        return v;

        //int k;
        //for (int i = 0; i < p.Length; i++)
        //{
        //    k = i + 1;
        //    v += Mathf.Pow((1 - t), k - 1) * p[i] +  ;
        //}
        //return v;
    }
    void Start()
    {
        area = transform.Search("Area") as RectTransform;
        var mouse = area.gameObject.AddComponent<UIMouseEventWrapper>();
        mouse.CreateBox2D();
        mouse.onMouseDown = MouseDown;
    }
    private void MouseDown()
    {
        points.Add(Input.mousePosition * IMUI.facterToReference);
        points[points.Count - 1] = points[points.Count - 1].SetY(IMUI.scaler.referenceResolution.y - points[points.Count - 1].y);
    }
    void Update()
    {
        ASUI.owner = this;
        GLUI.BeginOrtho();
        DrawBezier(points.ToArray());
        foreach (var p in points)
        {
            GLUI.DrawSquare(p, squareSize);
            GLUI.DrawSquare(p, squareSize, lineWidth);
        }
    }
}
