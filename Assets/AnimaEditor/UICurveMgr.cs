using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICurveMgr : MonoBehaviour
{
    public Button setAllCurveToLinear;
    void Start()
    {
        setAllCurveToLinear.onClick.AddListener(SetAllCurveToLinear);
    }
    void SetAllCurveToLinear()
    {
        foreach (var oc in UIClip.clip.curves)
        {
            foreach (var curve in oc.curves)
            {
                foreach (var key in curve.keys)
                {
                    key.inMode = CurveMode.Linear;
                    key.outMode = CurveMode.Linear;
                }
            }
        }
    }
}
