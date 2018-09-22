using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(UIClip))]
public class UIClipEditor : E_ShowButtons<UIClip> { }
#endif
public class UIClip : MonoSingleton<UIClip>
{
    public Clip clip
    {
        get
        {
            if (_clip == null || _clip.curves.Count == 0) I.Load();
            return _clip;
        }
        set { _clip = value; }
    }
    [SerializeField]
    private Clip _clip;
    [Header("ReadOnly")]
    public string path;
    public string folder = "Clips/";
    public string clipName = "Default";
    // 拖动时间轴绿色线（当前帧）时会更新所有曲线
    public void UpdateAllCurve()
    {
        var index = UITimeLine.I.frameIdx;
        float trueTime = 0, trueTime2 = 0, trueTime3 = 0;
        float endTime = 0;
        if (UICurve.I.curve.Count >= 1)
        {
            endTime = UICurve.I.curve.Last().time;
        }
        float n = endTime == 0 ? 0 : index / endTime;
        float t = 0;
        if (!UIPlayer.I.toggleFlip.isOn)
        {
            //trueTime3 = UICurve.I.GenerateRealTime(index);
            trueTime3 = index;
        }
        //n = Mathf.Repeat(0, 1.25f);
        //n = Mathf.Clamp(n, 0, 1.25f);
        //n = Mathf.Repeat(n, 1.25f);
        index = Mathf.RoundToInt(n * endTime);
        if (MathTool.Between(n, 0, 1))
        {
            //trueTime = UICurve.I.GenerateRealTime(index);
            trueTime3 = index;
        }
        else if (MathTool.Between(n, 1, 1.25f))
        {
            trueTime = endTime;
            trueTime2 = 0;
            t = (n - 1) / 0.25f;
        }
        else if (MathTool.Between(n, 1.25f, 2.25f))
        {
            n -= 1.25f;
            trueTime2 = n * endTime;
            t = 1;
        }
        else if (MathTool.Between(n, 2.25f, 2.5f))
        {
            trueTime = endTime;
            trueTime2 = 0;
            t = (n - 2.25f) / 0.25f;
        }
        else
        {
            trueTime = index;
        }

        Vector3 v1, v2;
        int c = 0;
        foreach (var curve in clip.curves)
        {
            if (curve.ast != null)
            {
                if (!UIPlayer.I.toggleFlip.isOn) // 如果打开了翻转动画
                {
                    v1 = curve.Rot(trueTime3);
                    v2 = Vector3.zero;
                    t = 0;
                }
                else if (n > 2.25f)
                {
                    v2 = curve.Rot(trueTime2);
                    if (curve.pair != null)
                    {
                        v1 = curve.pair.Rot(trueTime);
                    }
                    else
                    {
                        v1 = curve.Rot(trueTime);
                        if (curve.ast.dof.bone != Bone.root)
                        {
                            v1.y = -v1.y;
                            v1.z = -v1.z;
                        }
                    }
                }
                else
                {
                    v1 = curve.Rot(trueTime);
                    if (curve.pair != null)
                    {
                        v2 = curve.pair.Rot(trueTime2);
                    }
                    else
                    {
                        v2 = curve.Rot(trueTime2);
                        if (curve.ast.dof.bone != Bone.root)
                        {
                            v2.y = -v2.y;
                            v2.z = -v2.z;
                        }
                    }
                }
                curve.ast.euler = Vector3.Lerp(v1, v2, t);
                if (UITranslator.I.update.isOn)
                {
                    if (curve.poss[0].keys != null && curve.poss[0].keys.Count > 0)//有位置曲线才更新位置
                    {
                        v1 = curve.Pos(trueTime);
                        v2 = curve.Pos(trueTime2);

                        //var os = new Vector3();
                        //if (UIFrameMgr2.I.tPoseList != null && c < UIFrameMgr2.I.tPoseList.Count)
                        //{
                        //    os = UIFrameMgr2.I.tPoseList[c];
                        //}
                        var os = curve.ast.coord.originPos;
                        curve.ast.transform.localPosition = os + Vector3.Lerp(v1, v2, t);
                    }
                }
                if (UIDOFEditor.I.ast == null || UIDOFEditor.I.ast.dof == null)
                {
                    int a = 0;
                }
                if (curve.ast.dof.bone == UIDOFEditor.I.ast.dof.bone)//把值实时显示到两个编辑器
                {
                    UIDOFEditor.I.UpdateValueDisplay();
                    UITranslator.I.UpdateValueDisplay();
                }
            }
            c++;
        }
        UIPlayer.I.Mirror();
    }
    [ShowButton]
    void Load_()
    {
        Load(name);
    }
    public System.Action onLoadClip;
    public bool Load(string clipName)//不存在文件则返回false
    {
        path = UIClipList.I.clipPath + clipName + ".clip";
        if (System.IO.File.Exists(path))
        {
            _clip = Serializer.XMLDeSerialize<Clip>(path);
            _clip.clipName = clipName;
            foreach (var curve in _clip.curves)
            {
                var trans = UIDOFEditor.I.avatar.transform.Search(curve.name);
                curve.ast = UIDOFEditor.I.avatar.GetTransDOF(trans);
            }
            ClipTool.GetPairs(_clip.curves);
            ClipTool.GetFrameRange(_clip);

            PlayerPrefs.SetString("LastOpenClipName", clipName);
            PlayerPrefs.Save();
            if (onLoadClip != null) onLoadClip();
            return true;
        }
        else
        {
            Debug.Log("路径: " + path + " 不存在。");
            return false;
        }
    }

    public bool Load()
    {
        path = UIClipList.I.clipPath + clipName + ".clip";
        if (System.IO.File.Exists(path))
        {
            _clip = Serializer.XMLDeSerialize<Clip>(path);
            _clip.clipName = clipName;
            foreach (var curve in _clip.curves)
            {
                //curve.trans = UIDOFEditor.I.avv atar.transform.Search(curve.name);
                var trans = UIDOFEditor.I.avatar.transform.Search(curve.name);
                curve.ast = UIDOFEditor.I.avatar.GetTransDOF(trans);
            }
            ClipTool.GetPairs(_clip.curves);
            ClipTool.GetFrameRange(_clip);
            //PlayerPrefs.SetString("LastOpenClipName", clipName);
            //PlayerPrefs.Save();
            return true;
        }
        else  // 不存在clip文件则新建一个
        {
            _clip = new Clip(clipName);
            foreach (var ast in UIDOFEditor.I.avatar.data.asts)
            {
                _clip.AddCurve(ast);
            }
            ClipTool.GetPairs(_clip.curves);
            ClipTool.GetFrameRange(_clip);
            //PlayerPrefs.SetString("LastOpenClipName", clipName);
            //PlayerPrefs.Save();
            return false;
        }
    }
    [ShowButton]
    void NewClip()
    {
        New(name);
    }
    public void New(string clipName)
    {
        var c = new Clip(clipName);
        foreach (var ast in UIDOFEditor.I.avatar.data.asts)
        {
            c.AddCurve(ast);
        }
        ClipTool.GetPairs(c.curves);
        ClipTool.GetFrameRange(c);
        PlayerPrefs.SetString("LastOpenClipName", clipName);
        PlayerPrefs.Save();

        path = UIClipList.I.clipPath + clipName + ".clip";
        clip = Serializer.XMLSerialize(c, path);
    }
    public void Save(string clipName)
    {
        path = UIClipList.I.clipPath + clipName + ".clip";
        Serializer.XMLSerialize(clip, path);
    }
    public void Save()
    {
        path = UIClipList.I.clipPath + clip.clipName + ".clip";
        Serializer.XMLSerialize(clip, path);
    }
}
