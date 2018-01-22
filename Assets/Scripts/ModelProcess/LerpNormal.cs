using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class LerpNormal : MonoBehaviour
{
    [Serializable]
    public class SharedNormals
    {
        public List<Vector3> normals = new List<Vector3>();
        public Vector3 lerpedNormal;
        public SharedNormals(Vector3 normal)
        {
            normals.Add(normal);
        }
        public Vector3 GetLerpNormal()
        {
            Vector3 r = Vector3.zero;
            if (normals.Count > 1)
            {
                float factor = 1f / normals.Count;
                foreach (var n in normals)
                {
                    r += n * factor;
                }
                r = r.normalized;
            }
            else { r = normals[0]; }
            lerpedNormal = r;
            return r;
        }
    }
    public List<Vector3> vertices;
    public List<Vector3> normals;
    public List<Color> colors;
    public List<SharedNormals> sharedNormals;
    public Mesh mesh;
    [ContextMenu("Colors")]
    void SetColors()
    {
        var mf = GetComponent<MeshFilter>();
        var smr = GetComponent<SkinnedMeshRenderer>();
        if (mf != null)
        {
            mesh = mf.sharedMesh;
        }
        else if (smr != null)
        {
            mesh = smr.sharedMesh;
        }
        else { Debug.Log("Non Mesh or SkinMesh"); return; }
        normals = new List<Vector3>();
        mesh.GetNormals(normals);
        vertices = new List<Vector3>();
        mesh.GetVertices(vertices);
        Vector3 v;
        sharedNormals = new List<SharedNormals>();
        for (int i = 0; i < vertices.Count; i++)
        {
            v = vertices[i];
            sharedNormals.Add(new SharedNormals(normals[i]));
            for (int j = 0; j < vertices.Count; j++)
            {
                if (i == j) continue;
                if (Approx(v, vertices[j]))
                {
                    sharedNormals[sharedNormals.Count - 1].normals.Add(normals[j]);
                }
            }
        }
        colors = new List<Color>();
        foreach (var sn in sharedNormals)
        {
            v = sn.GetLerpNormal();
            colors.Add(new Color(v.x, v.y, v.z));
        }
        mesh.colors = colors.ToArray();
    }
    bool Approx(Vector3 a, Vector3 b)
    {
        return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
    }
}
