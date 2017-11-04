using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(ZSkinnedMeshRenderer))]
public class ZSkinnedMeshRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var o = (ZSkinnedMeshRenderer)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("ReadBoneWeightFile"))
        {
            o.ReadBoneWeightFile();
        }
    }
}
#endif
class ZBoneWeight
{
    public float[] weights;
    public int[] boneIndices;
}
public class ZSkinnedMeshRenderer : MonoBehaviour
{
    SkinnedMeshRenderer smr;
    Transform[] bones;
    Transform rootBone;
    Matrix4x4[] bindPoses;
    Matrix4x4[] boneMatrices;
    Vector3[] vertices;
    ZBoneWeight[] boneWeights;
    Mesh mesh;
    MeshFilter mf;
    MeshRenderer mr;
    public TextAsset text;
    public string objectHead = ":{";
    public string objectEnd = "}";
    public string boneWeightHead = "[";
    public string boneWeightEnd = "]";
    string ExtractPath(ref string str, string head, string end)
    {
        string m = "";
        var index = str.IndexOf(head);
        if (index > -1)
        {
            var r = str.Substring(index + head.Length);
            str = str.Substring(0, index);
            var ind = r.IndexOf(end);
            if (ind >= 0)
            {
                m = r.Substring(0, ind);
                if (ind + 1 < r.Length)
                {
                    var r2 = r.Substring(ind + 1);
                    str += r2;
                    return m;
                }
            }
            else str += r;
        }
        return m;
    }
    public void ReadBoneWeightFile()
    {
        boneWeights = new ZBoneWeight[vertices.Length];
        string str = text.text;
        var obj = ExtractPath(ref str, transform.name + objectHead, objectEnd);
        if (obj.Length > 0)
        {
            int i = 0;
            while (true)
            {
                var vertex = ExtractPath(ref obj, boneWeightHead, boneWeightEnd);
                if (vertex.Length == 0) break;
                var boneWeightStrs = vertex.Split('_');//权重导出还缺少骨骼索引，
                var bws = new List<float>();
                foreach (var boneWeightStr in boneWeightStrs)
                {
                    bws.Add(int.Parse(boneWeightStr) * 0.0001f);
                }
                boneWeights[i].weights =  bws.ToArray();
                //boneWeights[i].boneIndices[]
            }
        }
    }
    private void Start()
    {
        ReadBoneWeightFile();
        //GetFromSMR();
    }
    public void GetFromSMR()
    {
        smr = GetComponent<SkinnedMeshRenderer>();
        mf = GetComponent<MeshFilter>();
        bones = smr.bones;
        rootBone = smr.rootBone;
        mesh = smr.sharedMesh;
        bindPoses = mesh.bindposes;
        vertices = mesh.vertices;

        Destroy(smr);
        mr = gameObject.AddComponent<MeshRenderer>();
        var nl = new List<Vector3>();
        for (int i = 0; i < vertices.Length; i++)
        {
            nl.Add(GetPos(i));
        }
        mf.mesh.vertices = nl.ToArray();
    }
    public Vector3 GetPos(int b)
    {
        var weight = boneWeights[b];
        Matrix4x4 vertexMatrix = new Matrix4x4();
        for (int n = 0; n < 16; n++)
        {
            for (int i = 0; i < weight.boneIndices.Length; i++)
            {
                vertexMatrix[n] += boneMatrices[weight.boneIndices[i]][n] * weight.weights[i];
            }
        }
        var vertex = vertexMatrix.MultiplyPoint3x4(vertices[b]);
        return vertex;
    }
}
