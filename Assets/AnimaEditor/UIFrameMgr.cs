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
    public Bone bone;
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
        UIDOFEditor.I.PasteFrameAllFrame(new Bone[] { Bone.shoulder_r, Bone.upperarm_r, Bone.forearm_r, Bone.hand_r });
    }

    void PasteToGun()
    {
        UIDOFEditor.I.PasteFrame(Bone.other);
    }
    void PasteToArm()
    {
        //UIDOFEditor.I.PasteFrame(ASBoneTool.arms);
        var list = new List<Bone>();
        list.Add(Bone.thumb1_l);
        list.Add(Bone.thumb2_l);
        list.Add(Bone.thumb3_l);

        list.Add(Bone.index1_l);
        list.Add(Bone.index2_l);
        list.Add(Bone.index3_l);

        list.Add(Bone.middle1_l);
        list.Add(Bone.middle2_l);
        list.Add(Bone.middle3_l);

        list.Add(Bone.ring1_l);
        list.Add(Bone.ring2_l);
        list.Add(Bone.ring3_l);

        list.Add(Bone.pinky1_l);
        list.Add(Bone.pinky2_l);
        list.Add(Bone.pinky3_l);

        //list.Add(ASBone.hand_l);

        var list2 = new List<Bone>();
        foreach (var i in list)
        {
            list2.Add(i + 1);
        }
        list.AddRange(list2);
        UIDOFEditor.I.PasteFrame(list);
    }
    void PasteAllFrame()
    {
        UIDOFEditor.I.PasteFrameAllFrame(BoneTool.arms);
    }
    void DeleteAllCurve()
    {
        foreach (var curve in UIClip.clip.curves)
        {
            curve.RemoveAtTime(UITimeLine.I.frameIndex);
        }
    }
    bool MissAst(TransDOF t) // ast是否存在于当前clip
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
                UIClip.clip.curves.Add(new CurveObj(ast));
            }
        }
    }
}
