using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(NormalsController))]
public class NormalsControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var o = (NormalsController)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("ResetNormal"))
        {
            o.ResetNormal();
        }
        if (GUILayout.Button("GetNormals"))
        {
            o.GetNormals();
        }
    }
}
#endif
[ExecuteInEditMode]
public class NormalsController : MonoBehaviour
{
    public Mesh origin;
    //public List<Vector3> normals;
    public Vector3 add;
    Vector3 dir;
    //public float weight;

    public Color lineColor = Color.red;
    public float lineLength = 0.1f;
    public bool lineDepthTest = false;
    public bool showNormals;

    public Color wireframeColor = Color.yellow;
    public bool showWireframe;

    public Color selectColor = Color.blue;
    public int selectIndex = 0;

    Vector3[] verts;
    Vector3[] normals;
    Vector3[] normalsOrigin;
    int[] tris;

    Mesh mesh;
    public void GetNormals()
    {
        var smr = GetComponent<SkinnedMeshRenderer>();
        var mf = GetComponent<MeshFilter>();
        mesh = Instantiate(origin);
        mesh.name = "Temp Mesh";
        if (smr != null) smr.sharedMesh = mesh;
        else if (mf != null) mf.sharedMesh = mesh;

        verts = mesh.vertices;
        normals = mesh.normals;
        normalsOrigin = mesh.normals;
        tris = mesh.triangles;
    }
    void Update()
    {
        if (showWireframe)
        {
            for (int i = 0; i < tris.Length; i += 3)
            {
                var p1 = transform.TransformPoint(verts[tris[i]]);
                var p2 = transform.TransformPoint(verts[tris[i + 1]]);
                var p3 = transform.TransformPoint(verts[tris[i + 2]]);
                Debug.DrawLine(p1, p2, wireframeColor);
                Debug.DrawLine(p2, p3, wireframeColor);
            }
        }
        if (showNormals)
        {
            for (int i = 0; i < verts.Length; i++)
            {
                DrawNormal(i, lineColor);
            }
        }
        {
            var i = selectIndex;
            DrawNormal(i, selectColor);

            if (mesh != null)
            {
                normals[i] = (normalsOrigin[i] + add).normalized;
                mesh.normals = normals;
            }
        }
    }
    void DrawSelect()
    {
        selectIndex = (int)Mathf.Repeat(selectIndex, verts.Length);
        var pos = transform.TransformPoint(verts[selectIndex]);
        var nor = transform.TransformDirection(normals[selectIndex]);
        Debug.DrawRay(pos, (nor + add) * lineLength, selectColor, 0, lineDepthTest);
    }
    void DrawNormal(int i, Color color)
    {
        var pos = transform.TransformPoint(verts[i]);
        var nor = transform.TransformDirection(normals[i]);
        Debug.DrawRay(pos, nor * lineLength, color, 0, lineDepthTest);
    }

    internal void ResetNormal()
    {
        var i = selectIndex;
        if (mesh != null)
        {
            normals[i] = normalsOrigin[i];
            mesh.normals = normals;
        }
    }
}
