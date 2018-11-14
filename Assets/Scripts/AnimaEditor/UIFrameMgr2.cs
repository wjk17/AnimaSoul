using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFrameMgr2 : MonoSingleton<UIFrameMgr2>
{
    public UnityEngine.UI.Button btnConvertToRelativePos;
    public UnityEngine.UI.Button btnGetTPose;
    public List<Vector3> tPoseList;

    void Start()
    {
        this.AddInputCB();
        btnConvertToRelativePos.Init(SetAllCurveToLinear);
        btnGetTPose.Init(GetTPose);
    }
    void GetTPose()
    {
        tPoseList = new List<Vector3>();
        for (int i = 0; i < UIClip.I.clip.curves.Count; i++)
        {
            var pos = UIClip.I.clip.curves[i].Pos(0);
            tPoseList.Add(pos);
        }
    }
    void SetAllCurveToLinear()
    {
        for (int i = 0; i < UIClip.I.clip.curves.Count; i++)
        {
            int c = 0;
            foreach (var curve in UIClip.I.clip.curves[i].poss)
            {
                foreach (var key in curve.keys)
                {
                    var tpos = 0f;
                    switch (c)
                    {
                        case 0: tpos = UIClip.I.clip.curves[i].ast.coord.originPos.x; break;
                        case 1: tpos = UIClip.I.clip.curves[i].ast.coord.originPos.y; break;
                        case 2: tpos = UIClip.I.clip.curves[i].ast.coord.originPos.z; break;
                        default: break;
                    }
                    key.value -= tpos;
                }
                c++;
            }
        }
    }
}
