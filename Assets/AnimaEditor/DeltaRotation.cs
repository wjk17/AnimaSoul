using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(DeltaRotation))]
public class DeltaRotationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var o = (DeltaRotation)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("UpdateCoord"))
        {
            o.UpdateCoord();
        }
        if (GUILayout.Button("CopyToChildren"))
        {
            o.CopyToChildren();
        }
    }
}
#endif
[Serializable]
public class ASTransform
{
    public ASTransform(ASDOF dof)
    {
        this.dof = dof;
    }
    public ASDOF dof;
    public Coordinate coord;
    public Vector3 euler;
    public Vector3 right = new Vector3(1, 0, 0); // 用来转换坐标轴
    public Vector3 up = new Vector3(0, 1, 0);
    public Vector3 forward = new Vector3(0, 0, 1);
}
[Serializable]
public class Coordinate
{
    public Vector3 up;
    public Vector3 forward;
    public Vector3 right;
    public Quaternion origin;
    public void DrawRay(Transform t, Vector3 euler, float length, bool depthTest)
    {
        var n = new Coordinate(this);
        n *= Quaternion.AngleAxis(euler.y, n.up);
        n *= Quaternion.AngleAxis(euler.x, n.right);
        n *= Quaternion.AngleAxis(euler.z, n.forward);
        var p = t.parent;
        if (p != null)
        {
            n.up = p.TransformDirection(n.up);
            n.right = p.TransformDirection(n.right);
            n.forward = p.TransformDirection(n.forward);
        }
        Debug.DrawRay(t.position, n.forward * length, Color.blue, 0, depthTest);
        Debug.DrawRay(t.position, n.right * length, Color.red, 0, depthTest);
        Debug.DrawRay(t.position, n.up * length, Color.green, 0, depthTest);
    }
    public Coordinate(Coordinate c)
    {
        up = c.up;
        forward = c.forward;
        right = c.right;
        origin = c.origin;
    }
    public Coordinate(Transform t)
    {
        var p = t.parent;
        up = p != null ? p.InverseTransformDirection(t.up) : t.up;
        forward = p != null ? p.InverseTransformDirection(t.forward) : t.forward;
        right = p != null ? p.InverseTransformDirection(t.right) : t.right;
        origin = t.localRotation;
    }
    public Quaternion Rotate(Vector3 euler)
    {
        var n = new Coordinate(this);
        Quaternion result = origin;
        Quaternion rot;

        rot = Quaternion.AngleAxis(euler.y, n.up);
        n *= rot;
        result = rot * origin;

        rot = Quaternion.AngleAxis(euler.x, n.right);
        n *= rot;
        result = rot * result;

        result = Quaternion.AngleAxis(euler.z, n.forward) * result;

        return result;
    }
    public static Coordinate operator *(Coordinate coord, Quaternion rot)
    {
        coord.up = rot * coord.up;
        coord.forward = rot * coord.forward;
        coord.right = rot * coord.right;
        return coord;
    }
}
public class DeltaRotation : MonoBehaviour
{
    public Coordinate coord;
    public Vector3 euler;
    public Vector3 right = new Vector3(1, 0, 0); // 用来转换坐标轴
    public Vector3 up = new Vector3(0, 1, 0);
    public Vector3 forward = new Vector3(0, 0, 1);
    Vector3 ToCoord(Coordinate c, Vector3 v)
    {
        if (v.x != 0)
        {
            return v.x * c.right;
        }
        else if (v.y != 0)
        {
            return v.y * c.up;
        }
        else// if (v.z != 0)
        {
            return v.z * c.forward;
        }
    }
    public void UpdateCoord()
    {
        transform.localRotation = coord.origin;
        Start();
    }
    private void Start()
    {
        var n = new Coordinate(transform);
        coord = new Coordinate(n);
        coord.right = ToCoord(n, right);
        coord.up = ToCoord(n, up);
        coord.forward = ToCoord(n, forward);
    }
    void Update()
    {
        transform.localRotation = coord.Rotate(euler);
    }
    internal void CopyToChildren()
    {
        var ts = GetComponentsInChildren<Transform>(true);
        foreach (var t in ts)
        {
            if (t.name.EndsWith("_end", StringComparison.InvariantCultureIgnoreCase) == false)
            {
                var dr = t.GetComOrAdd<DeltaRotation>();
                dr.coord = coord;
                dr.right = right;
                dr.up = up;
                dr.forward = forward;
            }
        }
    }
}
