using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(FretsPos))]
public class FretsPosEditor : E_ShowButtons<FretsPos> { }
#endif
[ExecuteInEditMode]
public class FretsPos : MonoBehaviour, IFind
{
    public PosDataLerp posDatas;
    public MeshArray meshArray;
    [Range(0, 21)]
    public int fret;
    [Range(0, 5)]
    public int chord;
    public float gizmosRadius = 0.05f;
    public Color gizmosColor = Color.red;
    public float angle;
    public float cos;
    public List<Transform> frets;
    public List<Transform> hands1;
    public List<Transform> hands2;
    public Vector3 handOffset1;
    Vector3 _handOffset1;
    public Vector3 handOffset2;
    Vector3 _handOffset2;
    public string findName { get { return "frets"; } }
    [Range(0, 1)]
    public float nearFret = 0.8f; // 有多靠近品柱

    [ShowButton]
    void ClearGOs()
    {
        this.Clear();
        frets.Clear();
        hands1.Clear();
        hands2.Clear();
    }
    private void Update()
    {
        if(_handOffset1!= handOffset1 ||
            _handOffset2 != handOffset2)
        {
            _handOffset1 = handOffset1;
            _handOffset2 = handOffset2;
            GetFrets();
        }
    }
    [ShowButton]
    void GetFrets()
    {
        ClearGOs();
        for (int chord = 0; chord < 6; chord++) // chords
        {
            var fretPrevX = 0f;
            for (int fret = 0; fret < 22; fret++)
            {
                var fretCurrX = Mathf.Abs(meshArray.spacings[fret].x);
                var bl = Mathf.Lerp(fretPrevX, fretCurrX, nearFret);
                fretPrevX = fretCurrX;

                var t = new GameObject().transform;
                t.SetParent(this.Find());
                t.name = "chord " + chord.ToString() + " fret " + fret.ToString();

                var v = posDatas.vectors[chord];
                var cn = v.normalized;
                var b = v.toX0Z();
                var bn = b.normalized;

                angle = Vector3.Angle(b, v) * Mathf.Deg2Rad;
                cos = Mathf.Cos(angle);

                var cl = bl / cos;
                var c = cn * cl;

                var a = posDatas.pairs[chord].a.position;

                t.position = a + c;
                frets.Add(t);
            }
        }
        GetHands();
    }
    private void GetHands()
    {
        for (int fret = 0; fret < 22; fret++)
        {
            var t1 = new GameObject().transform;
            t1.SetParent(this.Find());
            t1.name = "hand1 " + fret.ToString();

            var t2 = new GameObject().transform;
            t2.SetParent(this.Find());
            t2.name = "hand2 " + fret.ToString();

            t1.position = frets[fret].position + handOffset1;
            hands1.Add(t1);
            t2.position = frets[fret].position + handOffset2;
            hands2.Add(t2);
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var fret in frets)
        {
            //Debug.DrawRay(a, b, gizmosColor);
            //Debug.DrawRay(a, c, gizmosColor);

            Gizmos.color = gizmosColor;
            //Gizmos.DrawSphere(fret.position, gizmosRadius);
            Gizmos.DrawWireSphere(fret.position, gizmosRadius);
        }
        foreach (var hand in hands1)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(hand.position, gizmosRadius);
        }
        foreach (var hand in hands2)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(hand.position, gizmosRadius);
        }
    }
}
