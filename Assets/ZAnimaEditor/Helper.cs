using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public static class ListTool
{
    public static void RemoveDuplicate<T>(params List<T>[] source)
    {
        foreach (var s in source)
        {
            RemoveDuplicate(s);
        }
    }
    public static void RemoveDuplicate<T>(List<T> source)
    {
        var unique = new List<T>();
        var origin = new List<T>(source);
        source.Clear();
        foreach (var i in origin)
        {
            if (unique.Contains(i) == false)
            {
                source.Add(i);
                unique.Add(i);
            }
        }
    }
    public static void BooleanIntersec<T>(out List<T> c, List<T> a, List<T> b)
    {
        c = new List<T>();
        foreach (var i in a)
        {
            if (b.Contains(i)) c.Add(i);
        }
    }
    public static List<T> BooleanIntersec<T>(List<T> a, List<T> b)
    {
        var c = new List<T>();
        foreach (var i in a)
        {
            if (b.Contains(i)) c.Add(i);
        }
        return c;
    }
    public static void BooleanSubject<T>(List<T> main, List<T> b)
    {
        var n = main.ToArray();
        main.Clear();
        foreach (var i in n)
        {
            if (!b.Contains(i)) main.Add(i);
        }
    }
}
public enum MeshType
{
    Skinned,
    Normal,
}
public class MeshWrapper
{
    public MeshType meshType;
    public Mesh mesh;
    public MeshFilter mf;
    public SkinnedMeshRenderer smr;
    public static implicit operator MeshWrapper(Transform t)
    {
        return new MeshWrapper(t);
    }
    public MeshWrapper(Transform t)
    {
        smr = t.GetComponent<SkinnedMeshRenderer>();
        if (smr != null)
        {
            smr.sharedMesh = Mesh.Instantiate(smr.sharedMesh);
            meshType = MeshType.Skinned;
            mesh = smr.sharedMesh;
        }
        else
        {
            mf = t.GetComponent<MeshFilter>();
            if (mf != null)
            {
                meshType = MeshType.Normal;
                mesh = mf.mesh;
            }
            else
            {
                throw new Exception("no smr or mf");
            }
        }
    }
    public void BakeSkinMesh()
    {
        smr.BakeMesh(mesh);
        var go = smr.gameObject;
        var mats = smr.sharedMaterials;
        UnityEngine.Object.Destroy(smr);
        mf = go.AddComponent<MeshFilter>();
        mf.mesh = mesh;
        var mr = go.AddComponent<MeshRenderer>();
        mr.sharedMaterials = mats;
    }
}
public static class MeshTool
{
    public static void RemoveSkin(this SkinnedMeshRenderer smr)
    {
        smr.rootBone = null;
        smr.bones = null;
        smr.sharedMesh.bindposes = null;
        smr.sharedMesh.boneWeights = null;
    }
}
public static class ComTool
{
    public static void Switch<T>(ref T a, ref T b)
    {
        T c = a;
        a = b;
        b = c;
    }
    public static void DestroyAuto(this UnityEngine.Object target)
    {
        if (Application.isPlaying == false)
        {
            UnityEngine.Object.DestroyImmediate(target);
        }
        else
        {
            UnityEngine.Object.Destroy(target);
        }
    }
    public static T GetComOrAdd<T>(this Component c) where T : Component
    {
        var com = c.GetComponent<T>();
        if (com == null) com = c.gameObject.AddComponent<T>();
        return com;
    }
    public static T GetCom<T>(this GameObject go) where T : Component
    {
        var com = go.GetComponent<T>();
        if (com == null) com = go.AddComponent<T>();
        return com;
    }
    public static T GetCom<T>(this Transform t) where T : Component
    {
        var com = t.GetComponent<T>();
        if (com == null) com = t.gameObject.AddComponent<T>();
        return com;
    }
}

[Serializable]
public struct Tran3
{
    public static Tran3 _Origin;
    public static Tran3 Origin
    {
        get
        {
            return new Tran3(Vector3.zero, Quaternion.identity, Vector3.one);
        }
    }
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scl;
    //public Tran3()
    //{
    //    pos = Vector3.zero;
    //    rot = Quaternion.identity;
    //    scl = Vector3.one;
    //}
    public Tran3(Transform transform)
    {
        pos = transform.position;
        rot = transform.rotation;
        scl = transform.lossyScale;
    }
    public Tran3(Transform transform, bool local)
    {
        pos = local ? transform.localPosition : transform.position;
        rot = local ? transform.localRotation : transform.rotation;
        scl = local ? transform.localScale : transform.lossyScale;
    }
    public Tran3(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        pos = position;
        rot = rotation;
        scl = scale;
    }
    public Tran3 Mirror()
    {
        pos = pos.Mirror();
        rot = rot.Mirror();
        return this;
    }
    public static implicit operator Tran3(Transform t)
    {
        return new Tran3(t);
    }
    public static implicit operator Matrix4x4(Tran3 t)
    {
        return Matrix4x4.TRS(t.pos, t.rot, t.scl);
    }
}

#region Trans类
[Serializable]
public class Tran2E
{
    public Vector3 pos;
    public Vector3 rot;
    public Tran2E(Vector3 pos, Vector3 rot)
    {
        this.pos = pos;
        this.rot = rot;
    }
}
/// <summary>
/// 包含Position和Rotation的类型
/// 用于快捷操作Transform，缩短成员名
/// </summary>
[Serializable]
public class Tran2 : ICloneable
{
    public Tran2 copy
    {
        get { return (Tran2)Clone(); }
    }

    public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
    {
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
    }
    public static implicit operator Tran2(Transform t)
    {
        return new Tran2(t);
    }
    public static implicit operator Matrix4x4(Tran2 t)
    {
        return Matrix4x4.TRS(t.pos, t.rot, Vector3.one);
    }
    public static Quaternion QuaternionFromMatrix2(Matrix4x4 m)
    {
        // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
        Quaternion q = new Quaternion();
        q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
        q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
        q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
        q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
        q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
        q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
        q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
        return q;
    }
    public static Tran2 operator *(Tran2 t, float f)
    {
        var pos = t.pos * f;
        var rot = Quaternion.Lerp(Quaternion.identity, t.rot, f);
        return new Tran2(pos, rot);
    }
    public static Tran2 operator *(Matrix4x4 matrix, Tran2 t)
    {
        var pos = matrix.MultiplyPoint3x4(t.pos);
        var rot = QuaternionFromMatrix2(matrix);
        return new Tran2(pos, rot * t.rot);
    }
    public static Tran2 operator /(Matrix4x4 matrix, Tran2 t)
    {
        var m = matrix.inverse;
        var pos = m.MultiplyPoint3x4(t.pos);
        var rot = QuaternionFromMatrix2(m);
        return new Tran2(pos, rot * t.rot);
    }
    public object Clone()
    {
        return new Tran2(pos, rot);
    }
    public const bool Local = true;
    public const bool World = false;
    public Vector3 pos;
    public Quaternion rot;
    public static Tran2 Absolute(Transform parent, Tran2 local)
    {
        Tran2 t2 = new Tran2();
        t2.pos = parent.TransformPoint(local.pos);
        t2.rot = parent.rotation * local.rot;
        return t2;
    }
    // 无视原本父子关系，计算指定的“父子”对象之间的相对坐标和旋转值。
    public static Tran2 Relative(Transform parent, Transform child)
    {
        var localPos = parent.InverseTransformPoint(child.position);
        var localRot = Quaternion.Inverse(parent.rotation) * child.rotation;
        return new Tran2(localPos, localRot);
    }
    public static Tran2 Lerp(Tran2 t2, Transform t, float time)
    {
        var nt = new Tran2();
        nt.pos = Vector3.Lerp(t2.pos, t.position, time);
        nt.rot = Quaternion.Lerp(t2.rot, t.rotation, time);
        return nt;
    }
    public static Tran2 Lerp(Transform t, Tran2 t2, float time)
    {
        var nt = new Tran2();
        nt.pos = Vector3.Lerp(t.position, t2.pos, time);
        nt.rot = Quaternion.Lerp(t.rotation, t2.rot, time);
        return nt;
    }
    public static Tran2 Lerp(Tran2 t1, Tran2 t2, float time)
    {
        var nt = new Tran2();
        nt.pos = Vector3.Lerp(t1.pos, t2.pos, time);
        nt.rot = Quaternion.Lerp(t1.rot, t2.rot, time);
        return nt;
    }
    public static Tran2 Lerp(Transform t1, Transform t2, float time)
    {
        var nt = new Tran2();
        nt.pos = Vector3.Lerp(t1.position, t2.position, time);
        nt.rot = Quaternion.Lerp(t1.rotation, t2.rotation, time);
        return nt;
    }
    public static Tran2 Lerp(Component c2, Component c, float time)
    {
        var nt = new Tran2();
        nt.pos = Vector3.Lerp(c2.transform.position, c.transform.position, time);
        nt.rot = Quaternion.Lerp(c2.transform.rotation, c.transform.rotation, time);
        return nt;
    }
    public Tran2()
    {
        pos = Vector3.zero;
        rot = Quaternion.identity;
    }
    public Tran2(Transform transform)
    {
        pos = transform.position;
        rot = transform.rotation;
    }
    public Tran2(Transform transform, bool local)
    {
        pos = local ? transform.localPosition : transform.position;
        rot = local ? transform.localRotation : transform.rotation;
    }
    public Tran2(Vector3 position, Quaternion rotation)
    {
        pos = position;
        rot = rotation;
    }
    public Tran2 Mirror()
    {
        pos = pos.Mirror();
        rot = rot.Mirror();
        return this;
    }
}
#endregion

public class Log
{
    public string log = "";
    public void Tab(string[] str)
    {
        foreach (var s in str)
        {
            log = log.Length == 0 ? s : log + "\t" + s;
        }
    }
    public void Tab(string str)
    {
        log = log.Length == 0 ? str : log + "\t" + str;
    }
    public void Add(string[] str)
    {
        foreach (var s in str)
        {
            log = log.Length == 0 ? s : log + "\r\n" + s;
        }
    }
    public void Add(string str)
    {
        log = log.Length == 0 ? str : log + "\r\n" + str;
    }
    public void Print()
    {
        Debug.Log(log.Length != 0 ? log : "没有内容");
    }
    public void Print(string str)
    {
        log = str + log;
        Print();
    }
    public void PrintRN(string str)
    {
        log = str + "\r\n" + log;
        Print();
        log = "";
    }
    public bool Dirty()
    {
        return log.Length > 0;
    }
    public void Clear()
    {
        log = "";
    }
}

public static class Tool
{
    public static float Repeat(float t, float length)
    {
        if (length == 0) return 0f;
        if (t / length == 1f) return length;
        return (t - (Mathf.Floor(t / length) * length));
    }
    public static float MapRange(this float value, float min1, float max1, float min2, float max2)
    {
        return (value - min1) / (max1 - min1) * (max2 - min2) + min2;
    }
    public static bool IsOdd(this int n)
    {
        return Convert.ToBoolean(n & 1);
    }
    public static Vector3 ColumnsV3(int left, int top, int columnWeight, int bottom, int value)
    {
        return new Vector3(left + value / bottom * columnWeight, top + (value % bottom), 0f);
    }
    public static Vector3 ColumnsV3(float left, float top, float columnWeight, float bottom, float value)
    {
        return new Vector3(left + (value / bottom) * columnWeight, top + (value % bottom), 0f);
    }
    public static Vector2 Columns(float left, float top, float columnWeight, float bottom, float value)
    {
        return new Vector2(left + (value / bottom) * columnWeight, top + (value % bottom));
    }

    #region Mirror
    // Mirror X
    public static void FlipLocalRotation(this Transform tran)
    {
        tran.localRotation = new Quaternion(tran.localRotation.x, -tran.localRotation.y, -tran.localRotation.z, tran.localRotation.w);
    }
    public static void FlipRotation(this Transform tran)
    {
        tran.rotation = new Quaternion(tran.rotation.x, -tran.rotation.y, -tran.rotation.z, tran.rotation.w);
    }
    public static void FlipLocalPosition(this Transform trans)
    {
        trans.localPosition = new Vector3(-trans.localPosition.x, trans.localPosition.y, trans.localPosition.z);
    }
    public static void FlipPosition(this Transform tran)
    {
        tran.position = new Vector3(-tran.position.x, tran.position.y, tran.position.z);
    }
    public static Vector3 Mirror(this Vector3 vect)
    {
        return new Vector3(-vect.x, vect.y, vect.z);
    }
    public static Vector3 MirrorEuler(this Vector3 vect)
    {
        return new Vector3(vect.x, -vect.y, -vect.z);
    }
    public static Quaternion Mirror(this Quaternion quat)
    {
        return new Quaternion(quat.x, -quat.y, -quat.z, quat.w);
    }
    public static void ScaleAbs(this GameObject target)
    {
        target.transform.localScale = new Vector3(target.transform.localScale.x, target.transform.localScale.y, target.transform.localScale.z);
    }
    public static void ScaleAbs(this Transform target)
    {
        target.localScale = new Vector3(target.localScale.x, target.localScale.y, target.localScale.z);
    }
    #endregion
    public static void Print(this GUIStyle style)
    {
        Log log = new Log();
        log.Add("name: " + style.name);
        log.Add("alignment: " + style.alignment);
        log.Print();
    }
    public static string emptyString = string.Empty;
    public static string NotNull(this string str)
    {
        return str != null ? str : "";// emptyString;
    }
    public static void Print(this Type t)
    {
        PrintInstanceInfor(t);
    }
    public static void PrintInstanceInfor(Type t)
    {
        foreach (MemberInfo member in t.GetMembers())
        {
            member.Name.Add();
        }
        Print();
        foreach (MethodInfo method in t.GetMethods())
        {
            method.Name.Add();
        }
        Print();
        foreach (PropertyInfo property in t.GetProperties())
        {
            property.Name.Add();
        }
        Print();
    }
    static string log;
    public static void Add(this string str)
    {
        log += "\r\n" + str;
    }
    public static void Print()
    {
        Debug.Log(log);
    }
    public static float Print(this float f)
    {
        Debug.Log(f.ToString() + "\r\n");
        return f;
    }
    public static int Print(this int i)
    {
        Debug.Log(i.ToString() + "\r\n");
        return i;
    }
    public static string Print(this string str)
    {
        if (str == null)
            Debug.Log("null String");
        else if (str.Length == 0)
            Debug.Log("empty String");
        else
            Debug.Log(str + "\r\n");
        return str;
    }
    public static string Print(this string str, string start)
    {
        Debug.Log(start + "\r\n" + str);
        return str;
    }
    public static bool EndsWithArray(this string str, params string[] strs)
    {
        foreach (string s in strs)
        {
            if (str.EndsWith(s))
                return true;
        }
        return false;
    }
    public static bool StartsWithArray(this string str, params string[] strs)
    {
        foreach (string s in strs)
        {
            if (str.StartsWith(s))
                return true;
        }
        return false;
    }
    /// <summary>
    /// 大驼峰（首字母大写）
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string UpCamel(this string str)
    {
        if (str.Length == 1)
        {
            return str.ToUpper();
        }
        else if (str.Length > 1)
        {
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }
        else
        {
            return str;
        }
    }
    // 获取选中对象Pivot
    public static Vector3 GetPivot(Transform[] trans)
    {
        if (trans == null || trans.Length == 0)
            return Vector3.zero;
        if (trans.Length == 1)
            return trans[0].position;

        float minX = Mathf.Infinity;//无限大
        float minY = Mathf.Infinity;
        float minZ = Mathf.Infinity;

        float maxX = -Mathf.Infinity;//无限小
        float maxY = -Mathf.Infinity;
        float maxZ = -Mathf.Infinity;

        foreach (Transform tr in trans)//AABB
        {
            if (tr.position.x < minX)
                minX = tr.position.x;
            if (tr.position.y < minY)
                minY = tr.position.y;
            if (tr.position.z < minZ)
                minZ = tr.position.z;

            if (tr.position.x > maxX)
                maxX = tr.position.x;
            if (tr.position.y > maxY)
                maxY = tr.position.y;
            if (tr.position.z > maxZ)
                maxZ = tr.position.z;
        }

        return new Vector3((minX + maxX) / 2.0f, (minY + maxY) / 2.0f, (minZ + maxZ) / 2.0f);
    }
    // 计算中心点
    public static Vector3 GetCenter(GameObject[] g)
    {
        Vector3 center = Vector3.zero;
        for (int i = 0; i < g.Length; i++)
        {
            center += g[i].transform.position;
        }
        var selectedCount = g.Length;
        center = new Vector3(center.x / selectedCount, center.y / selectedCount, center.z / selectedCount);
        return center;
    }
    public static Vector3 toX00(this Vector3 v3)
    {
        return new Vector3(v3.x, 0f);
    }
    public static Vector3 to1Y1(this Vector3 v3)
    {
        return new Vector3(1f, v3.y, 1f);
    }
    public static Vector3 toX1Z(this Vector3 v3)
    {
        return new Vector3(v3.x, 1f, v3.z);
    }

    public static Vector3 to0Y0(this Vector3 v3)
    {
        return new Vector3(0f, v3.y);
    }
    public static Vector3 to00Z(this Vector3 v3)
    {
        return new Vector3(0f, 0f, v3.z);
    }
    public static Vector3 toXY0(this Vector3 v3)
    {
        return new Vector3(v3.x, v3.y);
    }
    public static Vector3 toXY1(this Vector3 v3)
    {
        return new Vector3(v3.x, v3.y, 1f);
    }
    public static Vector3 toX0Z(this Vector3 v3)
    {
        return new Vector3(v3.x, 0f, v3.z);
    }
    public static Vector3 to0YZ(this Vector3 v3)
    {
        return new Vector3(0, v3.y, v3.z);
    }
    public static Vector2 yzV2(this Vector3 v3)
    {
        return new Vector2(v3.y, v3.z);
    }
    public static Vector3 toYZ(this Vector2 v2)
    {
        return new Vector3(0, v2.x, v2.y);
    }
    public static Vector2 Divide(Vector2 a, Vector2 b)
    {
        Vector2 n;
        n.x = a.x / b.x;
        n.y = a.y / b.y;
        return n;
    }
    public static void Divide(ref Vector2 a, Vector2 b)
    {
        a.x = a.x / b.x;
        a.y = a.y / b.y;
    }
    public static Vector3 Abs(Vector3 a)
    {
        return new Vector3(Math.Abs(a.x), Math.Abs(a.y), Math.Abs(a.z));
    }
    //public static float ApproxCeil(this float f)
    //{
    //    string dotLeft;
    //    string s =f.ToString();
    //    int os  = s.IndexOf('.');
    //    if (os > 0)
    //    {
    //        s = s.Substring(os + 1);
    //        os = s.IndexOf('9');
    //        s.Substring(os, 1);
    //        os
    //    }
    //}
    //public static Vector3 ApproxCeil(this Vector3 vector)
    //{
    //    vector.SetX(vector.x)
    //}
    public static Vector3 Center(this Transform transform)
    {
        Vector3 center = Vector3.zero;
        bool first = true;
        foreach (Transform t in transform)
        {
            if (first)
            {
                first = false;
                center = t.position;
            }
            else
            {
                Vector3 v3 = t.position - center;
                center += v3.normalized * v3.magnitude * 0.5f;
            }
        }
        return center;
    }
    public static int RepeatAbs(this int t, int length)
    {
        var mod = t % length;
        return mod == 0 ? 0 : (t < 0 ? length + mod : mod);
    }
    #region Vector3
    public static float PosX(this string target)
    {
        return target.transform().position.x;
    }
    public static float PosY(this string target)
    {
        return target.transform().position.y;
    }
    public static float PosZ(this string target)
    {
        return target.transform().position.z;
    }
    public static void SetPosX(this string target, float f)
    {
        Transform trans = target.transform();
        trans.position = new Vector3(f, trans.position.y, trans.position.z);
    }
    public static void SetPosY(this string target, float f)
    {
        Transform trans = target.transform();
        trans.position = new Vector3(trans.position.x, f, trans.position.z);
    }
    public static void SetPosZ(this string target, float f)
    {
        Transform trans = target.transform();
        trans.position = new Vector3(trans.position.x, trans.position.y, f);
    }
    public static void PlusPosX(this Transform trans, float f)
    {
        trans.position = new Vector3(trans.position.x + f, trans.position.y, trans.position.z);
    }
    public static void PlusPosY(this Transform trans, float f)
    {
        trans.position = new Vector3(trans.position.x, trans.position.y + f, trans.position.z);
    }
    public static void PlusPosZ(this Transform trans, float f)
    {
        trans.position = new Vector3(trans.position.x, trans.position.y, trans.position.z + f);
    }
    public static void SetPosX(this Transform trans, float f)
    {
        trans.position = new Vector3(f, trans.position.y, trans.position.z);
    }
    public static void SetPosY(this Transform trans, float f)
    {
        trans.position = new Vector3(trans.position.x, f, trans.position.z);
    }
    public static void SetPosZ(this Transform trans, float f)
    {
        trans.position = new Vector3(trans.position.x, trans.position.y, f);
    }
    public static Vector3 SetX(this Vector3 vec, float f)
    {
        return new Vector3(f, vec.y, vec.z);
    }
    public static Vector3 SetY(this Vector3 vec, float f)
    {
        return new Vector3(vec.x, f, vec.z);
    }
    public static Vector3 SetZ(this Vector3 vec, float f)
    {
        return new Vector3(vec.x, vec.y, f);
    }
    public static Vector3 signY(this Vector3 V, float sign)
    {
        Vector3 result;
        if (Mathf.Sign(V.y) != sign) result = new Vector3(V.x, V.y * -1, V.z);
        else result = V;
        return result;
    }
    public static void SetLocalScaleX(this Component com, float f)
    {
        com.transform.localScale = new Vector3(f, com.transform.localScale.y, com.transform.localScale.z);
    }
    public static void SetLocalScaleY(this Component com, float f)
    {
        com.transform.localScale = new Vector3(com.transform.localScale.x, f, com.transform.localScale.z);
    }
    public static void SetLocalScaleZ(this Component com, float f)
    {
        com.transform.localScale = new Vector3(com.transform.localScale.x, com.transform.localScale.y, f);
    }
    public static void SetLocalPosX(this Component com, float f)
    {
        com.transform.localPosition = new Vector3(f, com.transform.localPosition.y, com.transform.localPosition.z);
    }
    public static void SetLocalPosY(this Component com, float f)
    {
        com.transform.localPosition = new Vector3(com.transform.localPosition.x, f, com.transform.localPosition.z);
    }
    public static void SetLocalPosZ(this Component com, float f)
    {
        com.transform.localPosition = new Vector3(com.transform.localPosition.x, com.transform.localPosition.y, f);
    }
    public static void SetLocalPosX(this Transform trans, float f)
    {
        trans.localPosition = new Vector3(f, trans.localPosition.y, trans.localPosition.z);
    }
    public static void SetLocalPosY(this Transform trans, float f)
    {
        trans.localPosition = new Vector3(trans.localPosition.x, f, trans.localPosition.z);
    }
    public static void SetLocalPosZ(this Transform trans, float f)
    {
        trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, f);
    }
    //component
    public static void SetLocalEulerX(this Component com, float f)
    {
        com.transform.localEulerAngles = new Vector3(f, com.transform.localEulerAngles.y, com.transform.localEulerAngles.z);
    }
    public static void SetLocalEulerY(this Component com, float f)
    {
        com.transform.localEulerAngles = new Vector3(com.transform.localEulerAngles.x, f, com.transform.localEulerAngles.z);
    }
    public static void SetLocalEulerZ(this Component com, float f)
    {
        com.transform.localEulerAngles = new Vector3(com.transform.localEulerAngles.x, com.transform.localEulerAngles.y, f);
    }
    //transform
    public static void SetLocalEulerX(this Transform trans, float f)
    {
        trans.localEulerAngles = new Vector3(f, trans.localEulerAngles.y, trans.localEulerAngles.z);
    }
    public static void SetLocalEulerY(this Transform trans, float f)
    {
        trans.localEulerAngles = new Vector3(trans.localEulerAngles.x, f, trans.localEulerAngles.z);
    }
    public static void SetLocalEulerZ(this Transform trans, float f)
    {
        trans.localEulerAngles = new Vector3(trans.localEulerAngles.x, trans.localEulerAngles.y, f);
    }
    public static void SetLocalRotX(this Transform trans, float f)
    {
        trans.localRotation = new Quaternion(f, trans.localRotation.y, trans.localRotation.z, trans.localRotation.w);
    }
    public static void SetLocalRotY(this Transform trans, float f)
    {
        trans.localRotation = new Quaternion(trans.localRotation.x, f, trans.localRotation.z, trans.localRotation.w);
    }
    public static void SetLocalRotZ(this Transform trans, float f)
    {
        trans.localRotation = new Quaternion(trans.localRotation.x, trans.localRotation.y, f, trans.localRotation.w);
    }
    public static void SetLocalRotW(this Transform trans, float f)
    {
        trans.localRotation = new Quaternion(trans.localRotation.x, trans.localRotation.y, trans.localRotation.z, f);
    }
    public static void SetScaleX(this Transform trans, float f)
    {
        trans.localScale = new Vector3(f, trans.localScale.y, trans.localScale.z);
    }
    public static void SetScaleY(this Transform trans, float f)
    {
        trans.localScale = new Vector3(trans.localScale.x, f, trans.localScale.z);
    }
    public static void SetScaleZ(this Transform trans, float f)
    {
        trans.localScale = new Vector3(trans.localScale.x, trans.localScale.y, f);
    }
    #endregion
    #region Trans快捷操作
    public static bool m_searchIgnoreCase = true;
    public static bool SearchIgnoreCase
    {
        get { return m_searchIgnoreCase; }
        set { m_searchIgnoreCase = value; }
    }
    public static void SetParent(this GameObject target, GameObject parent, bool worldStays)
    {
        target.transform.SetParent(parent.transform, worldStays);
    }
    public static void SetParent(this GameObject target, GameObject parent)
    {
        target.transform.SetParent(parent.transform, true);
    }
    public static void SetParent(this GameObject target, Transform parent, bool worldStays)
    {
        target.transform.SetParent(parent, worldStays);
    }
    public static void SetParent(this GameObject target, Transform parent)
    {
        target.transform.SetParent(parent, true);
    }
    public static void SetParent(this Transform target, GameObject parent, bool worldStays)
    {
        target.SetParent(parent.transform, worldStays);
    }
    public static void SetParent(this Transform target, GameObject parent)
    {
        target.SetParent(parent.transform, true);
    }
    public static void Reset(this GameObject target)
    {
        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;
    }
    public static void Reset(this Transform target)
    {
        target.localPosition = Vector3.zero;
        target.localRotation = Quaternion.identity;
    }
    public static void SetLocalTrans(this GameObject target, Transform trans)
    {
        target.transform.localPosition = trans.localPosition;
        target.transform.localRotation = trans.localRotation;
    }
    public static void SetTran2(this GameObject target, GameObject source, bool local)
    {
        if (local) SetLocalTrans(target, source.transform);
        else SetTran2(target, source.transform);
    }
    public static void SetTran2(this GameObject target, Transform trans, bool local)
    {
        if (local) SetLocalTrans(target, trans);
        else SetTran2(target, trans);
    }
    public static void SetTran2(this GameObject target, GameObject source)
    {
        target.transform.position = source.transform.position;
        target.transform.rotation = source.transform.rotation;
    }
    public static void SetTran2(this GameObject target, Transform trans)
    {
        target.transform.position = trans.position;
        target.transform.rotation = trans.rotation;
    }


    // 默认世界坐标
    public static void SetTran3(this Transform target, Transform transform)
    {
        target.position = transform.position;
        target.rotation = transform.rotation;
        target.localScale = transform.localScale;
    }
    public static void SetTran3Local(this Transform to, Transform from)
    {
        to.localPosition = from.localPosition;
        to.localRotation = from.localRotation;
        to.localScale = from.localScale;
    }
    public static void SetTran2(this Transform target, Transform transform)
    {
        target.position = transform.position;
        target.rotation = transform.rotation;
    }
    public static void SetTran2(this Transform target, Tran2 trans, bool local)
    {
        if (local) SetTran2Local(target, trans);
        else SetTran2(target, trans);
    }
    public static void SetTran2(this Transform target, Tran2 trans)
    {
        target.position = trans.pos;
        target.rotation = trans.rot;
    }
    public static void SetTran2(this Component target, Tran2 trans)
    {
        target.transform.position = trans.pos;
        target.transform.rotation = trans.rot;
    }
    public static void SetTran2Local(this Component target, Tran2 trans)
    {
        target.transform.localPosition = trans.pos;
        target.transform.localRotation = trans.rot;
    }
    public static void SetTran2Local(this Transform target, Tran2 trans)
    {
        target.localPosition = trans.pos;
        target.localRotation = trans.rot;
    }
    public static Tran2 LocalTran2(this Component target)
    {
        return new Tran2(target.transform.localPosition, target.transform.localRotation);
    }
    public static Tran2 Tran2(this Component target)
    {
        return new Tran2(target.transform.position, target.transform.rotation);
    }
    public static Tran2 LocalTran2(this Transform target)
    {
        return new Tran2(target.localPosition, target.localRotation);
    }
    public static Tran2 Tran2(this Transform target)
    {
        return new Tran2(target.position, target.rotation);
    }
    public static Tran3 LocalTran3(this Component target)
    {
        return new Tran3(target.transform.localPosition, target.transform.localRotation, target.transform.localScale);
    }
    public static Tran3 Tran3(this Component target)
    {
        return new Tran3(target.transform.position, target.transform.rotation, target.transform.lossyScale);
    }
    public static Tran3 LocalTran3(this Transform target)
    {
        return new Tran3(target.localPosition, target.localRotation, target.transform.localScale);
    }
    public static Tran3 Tran3(this Transform target)
    {
        return new Tran3(target.position, target.rotation, target.lossyScale);
    }
    public static Tran2 LocMirrorX(this Transform target)
    {
        Vector3 pos = new Vector3(-target.localPosition.x, target.localPosition.y, target.localPosition.z);
        Quaternion quat = new Quaternion(target.localRotation.x, -target.localRotation.y, -target.localRotation.z, target.localRotation.w);
        return new Tran2(pos, quat);
    }
    //private static Tran LocMirrorX( this Transform target )
    //{
    //    Vector3 pos = new Vector3( -target.localPosition.x, target.localPosition.y, target.localPosition.z );
    //    Vector3 euler = new Vector3( target.localEulerAngles.x, -target.localEulerAngles.y, -target.localEulerAngles.z );
    //    return new Tran( pos, Quaternion.Euler( euler ) );
    //}
    public static Tran2 MirrorX(this GameObject target, GameObject pivot)
    {
        var P = new GameObject("pivot");
        P.SetTran2(pivot);
        var origin = target.transform.parent;
        target.SetParent(P, true);
        target.ScaleAbs();
        P.transform.localScale = P.transform.localScale.Mirror();
        target.SetParent(origin, true);
        UnityEngine.Object.DestroyImmediate(P);
        return new Tran2(target.transform.position, target.transform.rotation);
    }
    public static Tran2 MirrorX(this GameObject target, Transform pivot)
    {
        target.SetParent(pivot, true);
        pivot.localScale = pivot.localScale.Mirror();
        return new Tran2(target.transform.position, target.transform.rotation);
    }
    public static Tran2 MirrorX(this Transform target, Transform pivot)
    {
        target.SetParent(pivot, true);
        pivot.localScale = pivot.localScale.Mirror();
        return new Tran2(target.position, target.rotation);
    }
    public static Tran2 MirrorX(this Transform target, bool local)
    {
        if (local) return LocMirrorX(target);
        else return MirrorX(target);
    }
    public static Tran2 MirrorX(this Transform target)
    {
        Vector3 pos = new Vector3(-target.position.x, target.position.y, target.position.z);
        Quaternion quat = new Quaternion(target.rotation.x, -target.rotation.y, -target.rotation.z, target.rotation.w);
        return new Tran2(pos, quat);
    }
    //public static Tran MirrorX( this Transform target )
    //{
    //    Vector3 pos = new Vector3( -target.position.x, target.position.y, target.position.z );
    //    Vector3 euler = new Vector3( target.eulerAngles.x, -target.eulerAngles.y, -target.eulerAngles.z );
    //    return new Tran( pos, Quaternion.Euler( euler ) );
    //}
    public static float fix(float f)
    {
        if (f > 1)
        {
            return fix(f - 1);
        }
        else
        {
            return f;
        }
    }
    #endregion

    #region Transform组件查找
    public static Transform[] GetComponentsRecursively(this Transform target)
    {
        var list = new List<Transform>();
        foreach (Transform child in target)
        {
            list.Add(child);
            list.AddRange(GetComponentsRecursively(child));
        }
        return list.ToArray();
    }

    public static GameObject Root(string rootname)
    {
        foreach (GameObject go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            if (rootname.Equals(go.name, StringComparison.OrdinalIgnoreCase))
            {
                return go;
            }
        return null;
    }

    // 查找包含禁用状态的的对象
    public static GameObject Search(string rootname, string name)
    {
        var result = SearchInternal(rootname, name, m_searchIgnoreCase);
        return (result == null) ? null : result;
    }
    public static GameObject Search(string rootname, string name, bool ignoreCase)
    {
        var result = SearchInternal(rootname, name, ignoreCase);
        return (result == null) ? null : result;
    }
    public static GameObject SearchInternal(string rootname, string name, bool ignoreCase)
    {
        if (ignoreCase)
        {
            foreach (GameObject go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                if (rootname.Equals(go.name, StringComparison.OrdinalIgnoreCase))
                {
                    GameObject g = go.Search(name, ignoreCase);
                    if (g != null) return g.gameObject;
                }
        }
        else
        {
            foreach (GameObject go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                if (rootname == go.name)
                {
                    GameObject g = go.Search(name);
                    if (g != null) return g.gameObject;
                }
        }
        return null;
    }

    // 查找所有对象
    public static Transform Search(string name, bool ignoreCase)
    {
        GameObject[] GO = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject go in GO)
        {
            GameObject g = go.Search(name, ignoreCase);
            if (g != null) return g.transform;
        }
        return null;
    }
    public static Transform Search(string name)
    {
        return Search(name, m_searchIgnoreCase);
    }
    // 返回所有匹配名字的子对象
    public static List<Transform> SearchAll(this Transform target, string name)
    {
        List<Transform> match = new List<Transform>();
        foreach (Transform child in target)
        {
            if (child.name == name) match.Add(child);
            match.AddRange(SearchAll(child, name));
        }
        return match;
    }
    #region Property go\t\pos\rot\euler
    public static GameObject gameObject(this string target)
    {
        var result = Search(target);
        return result == null ? null : result.gameObject;
    }
    public static Transform transform(this string target)
    {
        return Search(target);
    }
    // these property don't check null,let compiler throw them.
    public static Vector3 position(this string target)
    {
        return Search(target).position;
    }
    public static Quaternion rotation(this string target)
    {
        return Search(target).rotation;
    }
    public static Vector3 eulerAngles(this string target)
    {
        return Search(target).eulerAngles;
    }
    #endregion
    #region Search
    // 查找所有层级子对象（包括自己），返回第一个匹配
    public static GameObject Search(this GameObject target, string name, bool ignoreCase)
    {
        Transform result = SearchInternal(target.transform, name, ignoreCase);
        return (result != null) ? result.gameObject : null;
    }
    public static GameObject Search(this GameObject target, string name)
    {
        Transform result = SearchInternal(target.transform, name, m_searchIgnoreCase);
        return (result != null) ? result.gameObject : null;
    }
    public static Transform Search(this Transform target, string name, bool ignoreCase)
    {
        return SearchInternal(target, name, ignoreCase);
    }
    public static Transform Search(this Transform target, string name)
    {
        return SearchInternal(target, name, m_searchIgnoreCase);
    }
    public static Transform SearchInternal(this Transform target, string name, bool ignoreCase)
    {
        if (ignoreCase)
        {
            foreach (var c in target.GetComponentsInChildren<Transform>(true))
                if (c.name.Equals(name, StringComparison.OrdinalIgnoreCase)) return c;
        }
        else
        {
            foreach (var c in target.GetComponentsInChildren<Transform>(true))
                if (c.name == name) return c;
        }
        return null;
    }
    public static Transform SearchRecursive(this Transform target, string name)
    {
        foreach (Transform child in target)
        {
            if (child.name == name)
                return child;

            Transform result = Search(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
    #endregion
    public static List<Transform> SearchEndWith(this Transform target, string postfix, bool includeInactive = true)
    {
        List<Transform> match = new List<Transform>();
        foreach (Transform c in target.GetComponentsInChildren<Transform>(includeInactive))
            if (c.name.EndsWith(postfix)) match.Add(c);
        return match;
    }
    #endregion

    #region 浮点数近似比较
    /// <summary>
    /// 浮点数近似比较（用处近似于Mathf.Approximately，可调整精度）
    /// 保留dec位小数的近似比较（默认2位）
    /// </summary>
    /// <param name="f1">浮点数1</param>
    /// <param name="f2">浮点数2</param>
    /// <returns>返回是否近似</returns>
    public static bool Approx(this float f1, float f2)
    {
        return Approx(f1, f2, 2);
    }
    public static bool Approx(float f1, float f2, int dec)
    {
        float factor = Mathf.Pow(10, dec);
        return Approx(f1, f2, factor);
    }
    public static bool Approx(float f1, float f2, float factor)
    {
        return
        (Ceil(f1, factor) == Ceil(f2, factor)) ||
        (Floor(f1, factor) == Ceil(f2, factor)) ||
        (Floor(f1, factor) == Floor(f2, factor)) ||
        (Ceil(f1, factor) == Floor(f2, factor));
    }
    public static float Ceil(float f, float factor)//进一
    {
        f *= factor;
        f = Mathf.Ceil(f);
        f /= factor;
        return f;
    }
    public static float Floor(float f, float factor)//退一
    {
        f *= factor;
        f = Mathf.Floor(f);
        f /= factor;
        return f;
    }
    #endregion
}
