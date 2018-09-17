using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UIDOFEditor {

    private void CopyFrame()
    {
        frame n = new frame();
        n.keys = new List<key>();
        if (UIClip.clip == null || UIClip.clip.curves == null) return;
        foreach (var curve in UIClip.clip.curves)
        {
            if (curve.ast == null) continue;
            var rot = curve.ast.euler;
            var pos = curve.ast.transform.localPosition;
            var t = new Tran2E(pos, rot);
            key k = new key();
            k.bone = curve.ast.dof.bone;
            k.t2 = t;
            n.keys.Add(k);
        }
        Debug.Log("copy " + n.keys.Count.ToString() + " keys");
        frameClipBoard = n;
    }
    CurveObj GetCurve(Bone bone)
    {
        foreach (var curve in UIClip.clip.curves)
        {
            if (curve.ast.dof.bone == bone)
            {
                return curve;
            }
        }
        return null;
    }
    public void PasteFrame()
    {
        PasteFrame(null);
    }
    public void PasteFrame(params Bone[] bones)
    {
        PasteFrame((ICollection<Bone>)bones);
    }
    public void PasteFrame(ICollection<Bone> bones)
    {
        if (frameClipBoard == null) return;
        var c = 0;
        foreach (var key in frameClipBoard.keys)
        {
            var curve = GetCurve(key.bone);
            if (curve != null && (bones == null || bones.Count == 0 || bones.Contains(curve.ast.dof.bone)))
            {
                c++;
                curve.ast.transform.localPosition = key.t2.pos;
                curve.ast.euler = key.t2.rot;
            }
        }
        Debug.Log("paste " + c + " keys");
    }
    public void PasteFrameAllFrame(ICollection<Bone> bones)
    {
        if (frameClipBoard == null) return;
        var c = 0;
        foreach (var key in frameClipBoard.keys)
        {
            var curve = GetCurve(key.bone);
            if (curve != null && (bones == null || bones.Count == 0 || bones.Contains(curve.ast.dof.bone)))
            {
                c++;
                curve.ast.transform.localPosition = key.t2.pos;
                curve.ast.euler = key.t2.rot;
            }
        }
        Debug.Log("paste " + c + " keys");
    }
}
