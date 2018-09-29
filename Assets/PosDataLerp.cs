using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct TransPair
{
    public Transform a;
    public Transform b;
}
[ExecuteInEditMode]
public class PosDataLerp : MonoBehaviour
{
    public List<TransPair> pairs;
    public List<Vector3> vectors;
    //[Range(0, 1)]
    //public float t;
    public float gizmosRadius = 0.5f;
    public Color gizmosColor = Color.red;
    void Start()
    {

    }
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) UpdatePos();
        foreach (var v in vectors)
        {
            Gizmos.color = gizmosColor;
            //Gizmos.DrawWireSphere(v, gizmosRadius);
        }
    }
    void Update()
    {
        UpdatePos();
    }
    void UpdatePos()
    {
        vectors = new List<Vector3>();
        foreach (var pair in pairs)
        {
            //poss.Add(Vector3.Lerp(pair.a.position, pair.b.position, t));
            vectors.Add(pair.b.position - pair.a.position);
        }
    }
    
}
