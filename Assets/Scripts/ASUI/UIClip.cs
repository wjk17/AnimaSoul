using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClip : MonoBehaviour
{
    public static UIClip I
    {
        get { if (instance == null) instance = FindObjectOfType<UIClip>(); return instance; }
    }
    private static UIClip instance;
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
        var trueTime = UICurve.I.GenerateRealTime(UITimeLine.FrameIndex);
        //foreach (var ast in UIDOFEditor.I.avatar.setting.asts)
        //{
        //    ast.euler = 
        //}
        //foreach (var curve in clip.curves)
        //{
        //    if (curve.trans != null)
        //    {
        //        UIDOFEditor.I.avatar.GetTransDOF(curve.trans).euler = curve.EulerAngles(trueTime);
        //    }
        //}
        foreach (var curve in clip.curves)
        {
            if (curve.ast != null)
            {
                curve.ast.euler = curve.EulerAngles(trueTime);
                if (UITranslator.I.update.isOn)
                {
                    if (curve.localPosition[0].keys != null && curve.localPosition[0].keys.Count > 0)//有位置曲线才更新位置
                    {
                        curve.ast.transform.localPosition = curve.LocalPosition(trueTime);
                    }
                }
                if (curve.ast.dof.bone == UIDOFEditor.I.ast.dof.bone)//把值实时显示到两个编辑器
                {
                    UIDOFEditor.I.UpdateValueDisplay();
                    UITranslator.I.UpdateValueDisplay();
                }
            }
        }
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
                //curve.trans = UIDOFEditor.I.avv atar.transform.Search(curve.name);
                var trans = UIDOFEditor.I.avatar.transform.Search(curve.name);
                curve.ast = UIDOFEditor.I.avatar.GetTransDOF(trans);
            }
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

        var dataPath = Application.dataPath;
        var rootPath = dataPath + "/../";
        path = rootPath + folder + clipName + ".clip";
        Serializer.XMLSerialize(c, path);
        clip = c;
    }
    public void Save()
    {
        var dataPath = Application.dataPath;
        var rootPath = dataPath + "/../";
        path = rootPath + folder + clip.clipName + ".clip";
        Serializer.XMLSerialize(clip, path);
    }
}
