using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FretsMesh : MonoBehaviour
{
    [Range(0, 3)]
    public int index;
    public float radius = 0.1f;
    public Color color1 = Color.red;
    public Color color2 = Color.blue;
    public Color colorChord = Color.green;
    public bool solid = true;
    public int[] upper;
    public int[] lower;
    public List<float> t_chords;
    public List<float> t_chordsM;
    public bool drawDebug = true;

    private void OnDrawGizmos()
    {
        var mesh = GetComponent<MeshFilter>().sharedMesh;
        var m = transform.localToWorldMatrix;

        var posU = mesh.GetVerts(upper).Mul3x4(m);
        var posL = mesh.GetVerts(lower).Mul3x4(m);
        if (drawDebug)
        {
            GizmosTool.DrawSpheres(radius, solid, color1, posU);
            GizmosTool.DrawSpheres(radius, solid, color2, posL);
        }

        var left = new Vector3[] { posU[0], posL[0] };
        var right = new Vector3[] { posU[1], posL[1] };
        t_chordsM = t_chords.Combine(t_chords.Rever()._sub(1));
        foreach (var t in t_chordsM)
        {
            Debug.DrawLine(left.Lerp(t), right.Lerp(t), colorChord);
        }
    }
}
