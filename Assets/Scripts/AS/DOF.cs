using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class DOF
{        
    public Bone bone;
    public int count;
    public float twistMin = 0f;
    public float twistMax = 0f;
    public float swingXMin = 0f;
    public float swingXMax = 0f;
    public float swingZMin = 0f;
    public float swingZMax = 0f;
    static DOF _fixed;
    static DOF _noLimit;
    public static DOF Fixed
    {
        get
        {
            // 这里共用一个DOF，有点危险的。因为DOF里有bone字段，指明哪一个骨骼使用。
            // 可以使用 new ASDOF();替代
            if (_fixed == null) _fixed = new DOF();
            return _fixed;
        }
    }
    /// <summary>
    /// 注意这是一个静态类成员，需要克隆后才能动态修改，TODO
    /// </summary>
    public static DOF NoLimit
    {
        get
        {
            if (_noLimit == null) _noLimit = Ball3D(-180, +180, -180, +180, -180, +180);
            return _noLimit;
        }
    }
    public static DOF Mirror(DOF origin)
    {
        var dof = new DOF();
        dof.twistMax = -origin.twistMin;
        dof.twistMin = -origin.twistMax;
        dof.swingXMax = -origin.swingXMin;
        dof.swingXMin = -origin.swingXMax;
        dof.swingZMax = origin.swingZMax;
        dof.swingZMin = origin.swingZMin;
        return dof;
    }
    public static DOF operator *(Vector3 scale, DOF dof)
    {
        dof.twistMin *= scale.y;
        dof.twistMax *= scale.y;
        dof.swingXMin *= scale.x;
        dof.swingXMax *= scale.x;
        dof.swingZMin *= scale.z;
        dof.swingZMax *= scale.z;
        return dof;
    }
    public static DOF operator *(DOF dof, Vector3 scale)
    {
        dof.twistMin *= scale.y;
        dof.twistMax *= scale.y;
        dof.swingXMin *= scale.x;
        dof.swingXMax *= scale.x;
        dof.swingZMin *= scale.z;
        dof.swingZMax *= scale.z;
        return dof;
    }
    public static DOF operator *(float scale, DOF dof)
    {
        dof.twistMin *= scale;
        dof.twistMax *= scale;
        dof.swingXMin *= scale;
        dof.swingXMax *= scale;
        dof.swingZMin *= scale;
        dof.swingZMax *= scale;
        return dof;
    }
    public static DOF operator *(DOF dof, float scale)
    {
        dof.twistMin *= scale;
        dof.twistMax *= scale;
        dof.swingXMin *= scale;
        dof.swingXMax *= scale;
        dof.swingZMin *= scale;
        dof.swingZMax *= scale;
        return dof;
    }
    //只能自转，没有这种骨骼，但可以用来作为骨骼限制。
    public DOF twist
    {
        get
        {
            return Twist(twistMin, twistMax);
        }
    }
    public static DOF Twist(float range)
    {
        var dof = new DOF();
        dof.count = 1;
        dof.twistMin = -range;
        dof.twistMax = +range;
        return dof;
    }
    public static DOF Twist(float yMin, float yMax)
    {
        var dof = new DOF();
        dof.count = 1;
        dof.twistMin = yMin;
        dof.twistMax = yMax;
        return dof;
    }
    public static DOF Hinge(float xMin, float xMax)
    {
        var dof = new DOF();
        dof.count = 1;
        dof.swingXMin = xMin;
        dof.swingXMax = xMax;
        return dof;
    }
    public static DOF Hinge2D(float xMin, float xMax, float tMin, float tMax)
    {
        var dof = new DOF();
        dof.count = 2;
        dof.twistMin = tMin;
        dof.twistMax = tMax;
        dof.swingXMin = xMin;
        dof.swingXMax = xMax;
        return dof;
    }
    public static DOF Ball(float zMin, float zMax, float xMin, float xMax)
    {
        var dof = new DOF();
        dof.count = 2;
        dof.swingXMin = xMin;
        dof.swingXMax = xMax;
        dof.swingZMin = zMin;
        dof.swingZMax = zMax;
        return dof;
    }
    public static DOF Ball3D(float zMin, float zMax, float xMin, float xMax, float tMin, float tMax)
    {
        var dof = new DOF();
        dof.count = 3;
        dof.twistMin = tMin;
        dof.twistMax = tMax;
        dof.swingXMin = xMin;
        dof.swingXMax = xMax;
        dof.swingZMin = zMin;
        dof.swingZMax = zMax;
        return dof;
    }
}

