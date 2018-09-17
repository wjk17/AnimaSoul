using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFrameMgr2 : MonoSingleton<UIFrameMgr2>
{
    public Button btnConvertToRelativePos;
    public Button btnGetTPose;
    public List<Vector3> tPoseList;

    void Start()
    {
        btnConvertToRelativePos.Init(SetAllCurveToLinear);
        btnGetTPose.Init(GetTPose);
    }
    void GetTPose()
    {
        tPoseList = new List<Vector3>();
        for (int i = 0; i < UIClip.clip.curves.Count; i++)
        {
            var pos = UIClip.clip.curves[i].Pos(0);
            tPoseList.Add(pos);
        }
    }
    void SetAllCurveToLinear()
    {
        for (int i = 0; i < UIClip.clip.curves.Count; i++)
        {
            int c = 0;
            foreach (var curve in UIClip.clip.curves[i].poss)
            {
                foreach (var key in curve.keys)
                {
                    var tpos = 0f;
                    switch (c)
                    {
                        case 0: tpos = UIClip.clip.curves[i].ast.coord.originPos.x; break;
                        case 1: tpos = UIClip.clip.curves[i].ast.coord.originPos.y; break;
                        case 2: tpos = UIClip.clip.curves[i].ast.coord.originPos.z; break;
                        default: break;
                    }
                    key.value -= tpos;
                }
                c++;
            }
        }
    }
}
