using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClip : MonoBehaviourInstance<UIClip>
{
    public static ASClip clip
    {
        get
        {
            if (_clip == null) I.Load();
            return _clip;
        }
        set { _clip = value; }
    }
    private static ASClip _clip;
    public string path;
    public string folder = "Clips/";
    public string clipName = "Default";
    // 拖动时间轴绿色线（当前帧）时会更新所有曲线
    public void UpdateAllCurve()
    {
        var index = UITimeLine.FrameIndex;
        float trueTime = 0, trueTime2 = 0, trueTime3 = 0;
        float maxIndex = 0;
        if (UICurve.I.keys.Count >= 1)
        {
            maxIndex = UICurve.I.keys[UICurve.I.keys.Count - 1].frameIndex;
        }
        float n = maxIndex == 0 ? 0 : index / maxIndex;
        float t = 0;
        if (!UIPlayer.I.toggleFlip.isOn)
        {
            trueTime3 = UICurve.I.GenerateRealTime(index);
        }
        //n = Mathf.Repeat(0, 1.25f);
        //n = Mathf.Clamp(n, 0, 1.25f);
        //n = Mathf.Repeat(n, 1.25f);
        index = Mathf.RoundToInt(n * maxIndex);
        if (MathTool.Between(n, 0, 1))
        {
            trueTime = UICurve.I.GenerateRealTime(index);
        }
        else if (MathTool.Between(n, 1, 1.25f))
        {
            trueTime = UICurve.I.GenerateRealTime(maxIndex);
            trueTime2 = UICurve.I.GenerateRealTime(0);
            t = (n - 1) / 0.25f;
        }
        else if (MathTool.Between(n, 1.25f, 2.25f))
        {
            n -= 1.25f;
            trueTime2 = n * maxIndex;
            t = 1;
        }
        else if (MathTool.Between(n, 2.25f, 2.5f))
        {
            trueTime = maxIndex;
            trueTime2 = 0;
            t = (n - 2.25f) / 0.25f;
        }
        else
        {
            trueTime = UICurve.I.GenerateRealTime(index);
        }

        Vector3 v1, v2;
        int c = 0;
        foreach (var curve in clip.curves)
        {
            if (curve.ast != null)
            {
                if (!UIPlayer.I.toggleFlip.isOn) // 如果打开了翻转动画
                {
                    v1 = curve.EulerAngles(trueTime3);
                    v2 = Vector3.zero;
                    t = 0;
                }
                else if (n > 2.25f)
                {
                    v2 = curve.EulerAngles(trueTime2);
                    if (curve.pair != null)
                    {
                        v1 = curve.pair.EulerAngles(trueTime);
                    }
                    else
                    {
                        v1 = curve.EulerAngles(trueTime);
                        if (curve.ast.dof.bone != ASBone.root)
                        {
                            v1.y = -v1.y;
                            v1.z = -v1.z;
                        }
                    }
                }
                else
                {
                    v1 = curve.EulerAngles(trueTime);
                    if (curve.pair != null)
                    {
                        v2 = curve.pair.EulerAngles(trueTime2);
                    }
                    else
                    {
                        v2 = curve.EulerAngles(trueTime2);
                        if (curve.ast.dof.bone != ASBone.root)
                        {
                            v2.y = -v2.y;
                            v2.z = -v2.z;
                        }
                    }
                }
                curve.ast.euler = Vector3.Lerp(v1, v2, t);
                if (UITranslator.I.update.isOn)
                {
                    if (curve.localPosition[0].keys != null && curve.localPosition[0].keys.Count > 0)//有位置曲线才更新位置
                    {
                        v1 = curve.LocalPosition(trueTime);
                        v2 = curve.LocalPosition(trueTime2);

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

    public bool Load(string clipName)//不存在文件则返回false
    {
        var dataPath = Application.dataPath;
        var rootPath = dataPath + "/../";
        path = rootPath + folder + clipName + ".clip";
        if (System.IO.File.Exists(path))
        {
            _clip = Serializer.XMLDeSerialize<ASClip>(path);
            _clip.clipName = clipName;
            foreach (var curve in _clip.curves)
            {
                var trans = UIDOFEditor.I.avatar.transform.Search(curve.name);
                curve.ast = UIDOFEditor.I.avatar.GetTransDOF(trans);
            }
            ASClipTool.GetPairs(_clip.curves);
            ASClipTool.GetFrameRange(_clip);

            PlayerPrefs.SetString("LastOpenClipName", clipName);
            PlayerPrefs.Save();

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Load()
    {
        var dataPath = Application.dataPath;
        var rootPath = dataPath + "/../";
        path = rootPath + folder + clipName + ".clip";
        if (System.IO.File.Exists(path))
        {
            _clip = Serializer.XMLDeSerialize<ASClip>(path);
            _clip.clipName = clipName;
            foreach (var curve in _clip.curves)
            {
                //curve.trans = UIDOFEditor.I.avv atar.transform.Search(curve.name);
                var trans = UIDOFEditor.I.avatar.transform.Search(curve.name);
                curve.ast = UIDOFEditor.I.avatar.GetTransDOF(trans);
            }
            ASClipTool.GetPairs(_clip.curves);
            ASClipTool.GetFrameRange(_clip);
            //PlayerPrefs.SetString("LastOpenClipName", clipName);
            //PlayerPrefs.Save();
            return true;
        }
        else  // 不存在clip文件则新建一个
        {
            _clip = new ASClip(clipName);
            foreach (var ast in UIDOFEditor.I.avatar.setting.asts)
            {
                //_clip.AddCurve(ast.transform);
                _clip.AddCurve(ast);
            }
            ASClipTool.GetPairs(_clip.curves);
            ASClipTool.GetFrameRange(_clip);
            //PlayerPrefs.SetString("LastOpenClipName", clipName);
            //PlayerPrefs.Save();
            return false;
        }
    }
    public void New(string clipName)
    {
        var c = new ASClip(clipName);
        foreach (var ast in UIDOFEditor.I.avatar.setting.asts)
        {
            c.AddCurve(ast);
        }
        ASClipTool.GetPairs(c.curves);
        ASClipTool.GetFrameRange(c);
        PlayerPrefs.SetString("LastOpenClipName", clipName);
        PlayerPrefs.Save();

        var dataPath = Application.dataPath;
        var rootPath = dataPath + "/../";
        path = rootPath + folder + clipName + ".clip";
        Serializer.XMLSerialize(c, path);
        clip = c;
    }
    public void Save(string clipName)
    {
        var dataPath = Application.dataPath;
        var rootPath = dataPath + "/../";
        path = rootPath + folder + clipName + ".clip";
        Serializer.XMLSerialize(clip, path);
    }
    public void Save()
    {
        var dataPath = Application.dataPath;
        var rootPath = dataPath + "/../";
        path = rootPath + folder + clip.clipName + ".clip";
        Serializer.XMLSerialize(clip, path);
    }
}
