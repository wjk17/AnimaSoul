using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum RotSeq
{
    zyx, zyz, zxy, zxz, yxz, yxy,
    yzx, yzy, xyz, xyx, xzy, xzx
}
// 出处：
// http://bediyap.com/programming/convert-quaternion-to-euler-rotations/
public static class MathTool
{
    public static Vector3 TwoAxisRot(float r11, float r12, float r21, float r31, float r32)
    {
        Vector3 ret = new Vector3();
        ret.x = Mathf.Atan2(r11, r12);
        ret.y = Mathf.Acos(r21);
        ret.z = Mathf.Atan2(r31, r32);
        return ret;
    }
    public static Vector3 ThreeAxisRot(float r11, float r12, float r21, float r31, float r32)
    {
        Vector3 ret = new Vector3();
        ret.x = Mathf.Atan2(r31, r32);
        ret.y = Mathf.Asin(r21);
        ret.z = Mathf.Atan2(r11, r12);
        return ret;
    }
    public static Vector3 Quaternion2Euler(Quaternion q, RotSeq rotSeq)
    {
        switch (rotSeq)
        {
            case RotSeq.zyx:
                return ThreeAxisRot(2 * (q.x * q.y + q.w * q.z),
                    q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                    -2 * (q.x * q.z - q.w * q.y),
                    2 * (q.y * q.z + q.w * q.x),
                    q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z);

            case RotSeq.zyz:
                return TwoAxisRot(2 * (q.y * q.z - q.w * q.x),
                    2 * (q.x * q.z + q.w * q.y),
                    q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                    2 * (q.y * q.z + q.w * q.x),
                    -2 * (q.x * q.z - q.w * q.y));

            case RotSeq.zxy:
                return ThreeAxisRot(-2 * (q.x * q.y - q.w * q.z),
                    q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                    2 * (q.y * q.z + q.w * q.x),
                    -2 * (q.x * q.z - q.w * q.y),
                    q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z);

            case RotSeq.zxz:
                return TwoAxisRot(2 * (q.x * q.z + q.w * q.y),
                    -2 * (q.y * q.z - q.w * q.x),
                    q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                    2 * (q.x * q.z - q.w * q.y),
                    2 * (q.y * q.z + q.w * q.x));

            case RotSeq.yxz:
                return ThreeAxisRot(2 * (q.x * q.z + q.w * q.y),
                    q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                    -2 * (q.y * q.z - q.w * q.x),
                    2 * (q.x * q.y + q.w * q.z),
                    q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z);

            case RotSeq.yxy:
                return TwoAxisRot(2 * (q.x * q.y - q.w * q.z),
                    2 * (q.y * q.z + q.w * q.x),
                    q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                    2 * (q.x * q.y + q.w * q.z),
                    -2 * (q.y * q.z - q.w * q.x));

            case RotSeq.yzx:
                return ThreeAxisRot(-2 * (q.x * q.z - q.w * q.y),
                    q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                    2 * (q.x * q.y + q.w * q.z),
                    -2 * (q.y * q.z - q.w * q.x),
                    q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z);

            case RotSeq.yzy:
                return TwoAxisRot(2 * (q.y * q.z + q.w * q.x),
                    -2 * (q.x * q.y - q.w * q.z),
                    q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                    2 * (q.y * q.z - q.w * q.x),
                    2 * (q.x * q.y + q.w * q.z));

            case RotSeq.xyz:
                return ThreeAxisRot(-2 * (q.y * q.z - q.w * q.x),
                    q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                    2 * (q.x * q.z + q.w * q.y),
                    -2 * (q.x * q.y - q.w * q.z),
                    q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z);

            case RotSeq.xyx:
                return TwoAxisRot(2 * (q.x * q.y + q.w * q.z),
                    -2 * (q.x * q.z - q.w * q.y),
                    q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                    2 * (q.x * q.y - q.w * q.z),
                    2 * (q.x * q.z + q.w * q.y));

            case RotSeq.xzy:
                return ThreeAxisRot(2 * (q.y * q.z + q.w * q.x),
                    q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                    -2 * (q.x * q.y - q.w * q.z),
                    2 * (q.x * q.z + q.w * q.y),
                    q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z);

            case RotSeq.xzx:
                return TwoAxisRot(2 * (q.x * q.z - q.w * q.y),
                    2 * (q.x * q.y + q.w * q.z),
                    q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                    2 * (q.x * q.z + q.w * q.y),
                    -2 * (q.x * q.y - q.w * q.z));

            default:
                Debug.LogError("No good sequence");
                return Vector3.zero;
        }
    }

}
public static class QuaternionTool
{
    public static Quaternion FromEuler(Vector3 euler, RotSeq seq)
    {
        Quaternion result;
        var x = Quaternion.Euler(euler.x, 0, 0);
        var y = Quaternion.Euler(0, euler.y, 0);
        var z = Quaternion.Euler(0, 0, euler.z);
        switch (seq)
        {
            case RotSeq.zyx:
                result = z * y * x;
                break;
            case RotSeq.yzx:
                result = y * z * x;
                break;
            case RotSeq.xzy:
                result = x * z * y;
                break;
            case RotSeq.zxy:
                result = z * x * y;
                break;
            case RotSeq.yxz:
                result = y * x * z;
                break;
            case RotSeq.xyz:
                result = x * y * z;
                break;
            default:
                throw new Exception();
        }
        return result;
    }
    public static Quaternion normalize(Quaternion q)
    {
        float norm = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
        q.x /= norm;
        q.y /= norm;
        q.z /= norm;
        q.w /= norm;
        return q;
    }

    public static float norm(Quaternion q)
    {
        return Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
    }


    ///////////////////////////////
    // Quaternion to Euler
    ///////////////////////////////

    public static void twoaxisrot(float r11, float r12, float r21, float r31, float r32, float[] res)
    {
        res[0] = Mathf.Atan2(r11, r12);
        res[1] = Mathf.Acos(r21);
        res[2] = Mathf.Atan2(r31, r32);
    }

    public static void threeaxisrot(float r11, float r12, float r21, float r31, float r32, float[] res)
    {
        res[0] = Mathf.Atan2(r31, r32);
        res[1] = Mathf.Asin(r21);
        res[2] = Mathf.Atan2(r11, r12);
    }
    // note: 
    // return values of res[] depends on rotSeq.
    // i.e.
    // for rotSeq zyx, 
    // x = res[0], y = res[1], z = res[2]
    // for rotSeq xyz
    // z = res[0], y = res[1], x = res[2]
    // ...
    public static Vector3 ToEuler(Quaternion q, RotSeq rotSeq)
    {
        q = normalize(q);
        var v = new float[3];
        switch (rotSeq)
        {
            case RotSeq.zyx:
                threeaxisrot(2 * (q.x * q.y + q.w * q.z),
                               q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                              -2 * (q.x * q.z - q.w * q.y),
                               2 * (q.y * q.z + q.w * q.x),
                               q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                               v);
                break;

            case RotSeq.zyz:
                twoaxisrot(2 * (q.y * q.z - q.w * q.x),
                             2 * (q.x * q.z + q.w * q.y),
                             q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                             2 * (q.y * q.z + q.w * q.x),
                            -2 * (q.x * q.z - q.w * q.y),
                            v);
                break;

            case RotSeq.zxy:
                threeaxisrot(-2 * (q.x * q.y - q.w * q.z),
                                q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                                2 * (q.y * q.z + q.w * q.x),
                               -2 * (q.x * q.z - q.w * q.y),
                                q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                                v);
                break;

            case RotSeq.zxz:
                twoaxisrot(2 * (q.x * q.z + q.w * q.y),
                            -2 * (q.y * q.z - q.w * q.x),
                             q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                             2 * (q.x * q.z - q.w * q.y),
                             2 * (q.y * q.z + q.w * q.x),
                             v);
                break;

            case RotSeq.yxz:
                threeaxisrot(2 * (q.x * q.z + q.w * q.y),
                               q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                              -2 * (q.y * q.z - q.w * q.x),
                               2 * (q.x * q.y + q.w * q.z),
                               q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                               v);
                break;

            case RotSeq.yxy:
                twoaxisrot(2 * (q.x * q.y - q.w * q.z),
                             2 * (q.y * q.z + q.w * q.x),
                             q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                             2 * (q.x * q.y + q.w * q.z),
                            -2 * (q.y * q.z - q.w * q.x),
                            v);
                break;

            case RotSeq.yzx:
                threeaxisrot(-2 * (q.x * q.z - q.w * q.y),
                                q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                                2 * (q.x * q.y + q.w * q.z),
                               -2 * (q.y * q.z - q.w * q.x),
                                q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                                v);
                break;

            case RotSeq.yzy:
                twoaxisrot(2 * (q.y * q.z + q.w * q.x),
                            -2 * (q.x * q.y - q.w * q.z),
                             q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                             2 * (q.y * q.z - q.w * q.x),
                             2 * (q.x * q.y + q.w * q.z),
                             v);
                break;

            case RotSeq.xyz:
                threeaxisrot(-2 * (q.y * q.z - q.w * q.x),
                              q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                              2 * (q.x * q.z + q.w * q.y),
                             -2 * (q.x * q.y - q.w * q.z),
                              q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                              v);
                break;

            case RotSeq.xyx:
                twoaxisrot(2 * (q.x * q.y + q.w * q.z),
                            -2 * (q.x * q.z - q.w * q.y),
                             q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                             2 * (q.x * q.y - q.w * q.z),
                             2 * (q.x * q.z + q.w * q.y),
                             v);
                break;

            case RotSeq.xzy:
                threeaxisrot(2 * (q.y * q.z + q.w * q.x),
                               q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                              -2 * (q.x * q.y - q.w * q.z),
                               2 * (q.x * q.z + q.w * q.y),
                               q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                               v);
                break;

            case RotSeq.xzx:
                twoaxisrot(2 * (q.x * q.z - q.w * q.y),
                             2 * (q.x * q.y + q.w * q.z),
                             q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                             2 * (q.x * q.z + q.w * q.y),
                            -2 * (q.x * q.y - q.w * q.z),
                            v);
                break;
            default:
                throw new Exception();
        }
        return new Vector3(v[0], v[1], v[2]) * Mathf.Rad2Deg;
    }
}