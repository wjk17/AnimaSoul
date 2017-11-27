using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(ZIKSolver), true)]
public class ZIKSolverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var ik = (ZIKSolver)target;
        if (GUILayout.Button("计算关节链（x迭代次数）"))
        {
            ik.Solve();
        }
        if (GUILayout.Button("计算下一个关节"))
        {
            ik.SolveStep();
        }
    }
}
#endif

[ExecuteInEditMode]
public class ZIKSolver : MonoBehaviour
{
    public List<Joint> origin;
    public Transform target;

    public int iteration; // 迭代次数
    public int i;

    private List<WRay> rays = new List<WRay>();
    public Transform[] cachedTrans;
    private Transform _cachePool;
    public bool solveOn;
    public bool skipClip;

    private Transform cachePool
    {
        get
        {
            if (_cachePool == null)
            {
                _cachePool = (new GameObject("_ik_cachePool")).transform;
            }
            return _cachePool;
        }
    }
    public IKBoneChain chain = new IKBoneChain();

    public void SetChainSkip(bool on)
    {
        foreach (var o in origin)
        {
            o.skipClip = on;
        }
    }

    internal void SetChain(Transform end, int length)
    {        
        int i = 0;
        Transform t;
        t = end;
        origin = new List<Joint>();
        while (true)
        {
            var j = new Joint();
            j.skipClip = true;
            j.maxDeltaAngle = 360;
            j.maxAngle = 360;
            j.minAngle = -360;            
            j.transform = t;
            j.name = j.transform.name;
            j.dof = ASDOF.NoLimit;
            origin.Add(j);
            i++;
            if (i == length) break;
            t = t.parent;
        }
        origin.Reverse();
    }
    //// 终端不一定是骨骼链末端
    //public void SetChain(Transform end, int length, Transform target)
    //{
    //    this.target = target;
    //    SetChain(end, length);
    //}
    //public void Update()
    //{
    //    if (solveOn) Solve();
    //    foreach (var ray in rays)
    //    {
    //        //ray.Draw();
    //    }
    //}
    public void OnEnable()
    {
        i = 0;
    }
    public void SolveStep()
    {

        var ts = cachePool.GetComponentsInChildren<Transform>();
        var less = origin.Count - ts.Length;
        if (less > 0)
        {
            var t = ts[ts.Length - 1];
            while (less > 0)
            {
                less--;
                var go = new GameObject("_cachedTrans");
                go.transform.SetParent(t);
                t = go.transform;
            }
        }
        cachedTrans = cachePool.GetComponentsInChildren<Transform>();
        chain.joints = new List<Joint>();
        for (int i = 0; i < origin.Count; i++)
        {
            var j = origin[i].Clone();
            j.transform = cachedTrans[i];
            j.transform.localPosition = origin[i].transform.localPosition;
            j.transform.localRotation = origin[i].transform.localRotation;
            j.transform.localScale = origin[i].transform.localScale;
            chain.joints.Add(j);
        }
        chain.joints[0].transform.SetParent(origin[0].transform.parent, false);
        chain.joints.Reverse();
        chain.end = chain.joints[0].transform;
        chain.joints.RemoveAt(0);
        chain.iteration = iteration;
        chain.target = target;


        //rays = chain.Solve();

        var ray = chain.Solve(i % chain.joints.Count);
        if (ray != null) rays = new List<WRay>(new WRay[] { ray });

        chain.joints.Reverse();
        for (int i = 0; i < chain.joints.Count; i++)
        {
            var j = chain.joints[i];
            origin[i].transform.localRotation = j.transform.localRotation;
        }
        origin[origin.Count - 1].transform.localRotation = chain.end.transform.localRotation;

        i++;
    }
    public void Solve()
    {
        var ts = cachePool.GetComponentsInChildren<Transform>();
        var less = origin.Count - ts.Length;
        if (less > 0)
        {
            var t = ts[ts.Length - 1];
            while (less > 0)
            {
                less--;
                var go = new GameObject("_cachedTrans");
                go.transform.SetParent(t);
                t = go.transform;
            }
        }
        cachedTrans = cachePool.GetComponentsInChildren<Transform>();
        chain.joints = new List<Joint>();
        for (int i = 0; i < origin.Count; i++)
        {
            var j = origin[i].Clone();
            j.transform = cachedTrans[i];
            j.transform.localPosition = origin[i].transform.localPosition;
            j.transform.localRotation = origin[i].transform.localRotation;
            j.transform.localScale = origin[i].transform.localScale;
            chain.joints.Add(j);
        }
        chain.joints[0].transform.SetParent(origin[0].transform.parent, false);
        chain.joints.Reverse();
        chain.end = chain.joints[0].transform;
        chain.joints.RemoveAt(0);
        chain.iteration = iteration;
        chain.target = target;
        rays = chain.Solve();

        chain.joints.Reverse();
        for (int i = 0; i < chain.joints.Count; i++)
        {
            var j = chain.joints[i];
            origin[i].transform.localRotation = j.transform.localRotation;
        }
        origin[origin.Count - 1].transform.localRotation = chain.end.transform.localRotation;
    }
}

