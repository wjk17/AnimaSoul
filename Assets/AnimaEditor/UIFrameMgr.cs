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
    private void Start()
    {
        insertMissButton.onClick.AddListener(InsertMissCurve);
        deleteAllCurveButton.onClick.AddListener(DeleteAllCurve);
        pasteAllFrameButton.onClick.AddListener(PasteAllFrame);
        pasteToArmButton.onClick.AddListener(PasteToArm);
    }
    void PasteToArm()
    {
        UIDOFEditor.I.PasteFrame(ASBoneTool.arms);
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
