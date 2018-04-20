using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class frame
{
    public List<key> keys;
}
public class key
{
    public Tran2E t2;
    public ASBone bone;
}
public class UIFrameMgr : MonoBehaviour
{
    public Button insertMissButton;
    public Button deleteAllCurveButton;
    public Button pasteAllFrameButton;
    public Button pasteToArmButton;
    public Button pasteToGunButton;
    public Button pasteLeftHandToAllFrameBtn;
    private void Start()
    {
        insertMissButton.onClick.AddListener(InsertMissCurve);
        deleteAllCurveButton.onClick.AddListener(DeleteAllCurve);
        pasteAllFrameButton.onClick.AddListener(PasteAllFrame);
        pasteToArmButton.onClick.AddListener(PasteToArm);
        pasteToGunButton.onClick.AddListener(PasteToGun);
        pasteLeftHandToAllFrameBtn.onClick.AddListener(pasteLeftHandToAllFrame);
    }
    private void pasteLeftHandToAllFrame()
    {
        UIDOFEditor.I.PasteFrameAllFrame(new ASBone[] { ASBone.shoulder_r, ASBone.upperarm_r, ASBone.forearm_r, ASBone.hand_r });
    }

    void PasteToGun()
    {
        UIDOFEditor.I.PasteFrame(ASBone.other);
    }
    void PasteToArm()
    {
        //UIDOFEditor.I.PasteFrame(ASBoneTool.arms);
        var list = new List<ASBone>();
        list.Add(ASBone.thumb1_l);
        list.Add(ASBone.thumb2_l);
        list.Add(ASBone.thumb3_l);

        list.Add(ASBone.index1_l);
        list.Add(ASBone.index2_l);
        list.Add(ASBone.index3_l);

        list.Add(ASBone.middle1_l);
        list.Add(ASBone.middle2_l);
        list.Add(ASBone.middle3_l);

        list.Add(ASBone.ring1_l);
        list.Add(ASBone.ring2_l);
        list.Add(ASBone.ring3_l);

        list.Add(ASBone.pinky1_l);
        list.Add(ASBone.pinky2_l);
        list.Add(ASBone.pinky3_l);

        //list.Add(ASBone.hand_l);

        var list2 = new List<ASBone>();
        foreach (var i in list)
        {
            list2.Add(i + 1);
        }
        list.AddRange(list2);
        UIDOFEditor.I.PasteFrame(list);
    }
    void PasteAllFrame()
    {
        UIDOFEditor.I.PasteFrameAllFrame(ASBoneTool.arms);
    }
    void DeleteASC(ASCurve asc)
    {
        if (asc.keys != null && asc.keys.Count > 0)
        {
            if (asc.IndexOf(UITimeLine.FrameIndex) > -1)
            {
                asc.RemoveKey(UITimeLine.FrameIndex);
            }
        }
    }
    void DeleteAllCurve()
    {
        foreach (var curve in UIClip.clip.curves)
        {
            DeleteASC(curve.timeCurve);
            DeleteASC(curve.eulerAngles[0]);
            DeleteASC(curve.eulerAngles[1]);
            DeleteASC(curve.eulerAngles[2]);
            DeleteASC(curve.localPosition[0]);
            DeleteASC(curve.localPosition[1]);
            DeleteASC(curve.localPosition[2]);
        }
    }
    bool MissAst(ASTransDOF t) // ast是否存在于当前clip
    {
        foreach (var curve in UIClip.clip.curves)
        {
            if (curve.ast == t) return false;
        }
        return true;
    }
    void InsertMissCurve()
    {
        foreach (var ast in UIDOFEditor.I.avatar.setting.asts)
        {
            if (MissAst(ast)) // 插入新增的（化身ast表里有，clip里却没有的）曲线
            {
                UIClip.clip.curves.Add(new ASObjectCurve(ast));
            }
        }
    }
}
