using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class ASDOF
{    
    public ASBone bone;
    public int count;
    public float twistMin = 0f;
    public float twistMax = 0f;
    public float swingXMin = 0f;
    public float swingXMax = 0f;
    public float swingZMin = 0f;
    public float swingZMax = 0f;
    static ASDOF _fixed;
    static ASDOF _noLimit;
    public static ASDOF Fixed
    {
        get
        {
            if (_fixed == null) _fixed = new ASDOF();
            return _fixed;
        }
    }
    public static ASDOF NoLimit
    {
        get
        {
            if (_noLimit == null) _noLimit = Ball3D(-180, +180, -180, +180, -180, +180);
            return _noLimit;
        }
    }
    public static ASDOF Mirror(ASDOF origin)
    {
        var dof = new ASDOF();
        dof.twistMax = -origin.twistMin;
        dof.twistMin = -origin.twistMax;
        dof.swingXMax = -origin.swingXMin;
        dof.swingXMin = -origin.swingXMax;
        dof.swingZMax = origin.swingZMax;
        dof.swingZMin = origin.swingZMin;
        return dof;
    }
    public static ASDOF operator *(Vector3 scale, ASDOF dof)
    {
        dof.twistMin *= scale.y;
        dof.twistMax *= scale.y;
        dof.swingXMin *= scale.x;
        dof.swingXMax *= scale.x;
        dof.swingZMin *= scale.z;
        dof.swingZMax *= scale.z;
        return dof;
    }
    public static ASDOF operator *(ASDOF dof, Vector3 scale)
    {
        dof.twistMin *= scale.y;
        dof.twistMax *= scale.y;
        dof.swingXMin *= scale.x;
        dof.swingXMax *= scale.x;
        dof.swingZMin *= scale.z;
        dof.swingZMax *= scale.z;
        return dof;
    }
    public static ASDOF operator *(float scale, ASDOF dof)
    {
        dof.twistMin *= scale;
        dof.twistMax *= scale;
        dof.swingXMin *= scale;
        dof.swingXMax *= scale;
        dof.swingZMin *= scale;
        dof.swingZMax *= scale;
        return dof;
    }
    public static ASDOF operator *(ASDOF dof, float scale)
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
    public ASDOF twist
    {
        get
        {
            return Twist(twistMin, twistMax);
        }
    }
    public static ASDOF Twist(float range)
    {
        var dof = new ASDOF();
        dof.count = 1;
        dof.twistMin = -range;
        dof.twistMax = +range;
        return dof;
    }
    public static ASDOF Twist(float yMin, float yMax)
    {
        var dof = new ASDOF();
        dof.count = 1;
        dof.twistMin = yMin;
        dof.twistMax = yMax;
        return dof;
    }
    public static ASDOF Hinge(float xMin, float xMax)
    {
        var dof = new ASDOF();
        dof.count = 1;
        dof.swingXMin = -xMax; // 按照常识习惯反转方向，向前方的旋转（逆时针）为正。
        dof.swingXMax = -xMin;
        return dof;
    }
    public static ASDOF Hinge2D(float xMin, float xMax, float tMin, float tMax)
    {
        var dof = new ASDOF();
        dof.count = 2;
        dof.twistMin = tMin;
        dof.twistMax = tMax;
        dof.swingXMin = xMin;
        dof.swingXMax = xMax;
        return dof;
    }
    public static ASDOF Ball(float zMin, float zMax, float xMin, float xMax)
    {
        var dof = new ASDOF();
        dof.count = 2;
        dof.swingXMin = xMin;
        dof.swingXMax = xMax;
        dof.swingZMin = zMin;
        dof.swingZMax = zMax;
        return dof;
    }
    public static ASDOF Ball3D(float zMin, float zMax, float xMin, float xMax, float tMin, float tMax)
    {
        var dof = new ASDOF();
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

