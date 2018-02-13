using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
[Serializable]
public class DOFProp // DOF控件
{
    public DOFProp(ASDOF dof)
    {
        bone = dof.bone;
        count = dof.count;
        twistMin = new FloatValueIF(dof.twistMin);
        twistMax = new FloatValueIF(dof.twistMax);
        swingXMin = new FloatValueIF(dof.swingXMin);
        swingXMax = new FloatValueIF(dof.swingXMax);
        swingZMin = new FloatValueIF(dof.swingZMin);
        swingZMax = new FloatValueIF(dof.swingZMax);
    }
    public void SetValues(ASDOF dof)
    {
        bone = dof.bone;
        count = dof.count;
        twistMin.value = dof.twistMin;
        twistMax.value = dof.twistMax;
        swingXMin.value = dof.swingXMin;
        swingXMax.value = dof.swingXMax;
        swingZMin.value = dof.swingZMin;
        swingZMax.value = dof.swingZMax;
    }
    public ASDOF ToASDOF()
    {
        var dof = new ASDOF();
        return SaveASDOF(dof);
    }
    public ASDOF SaveASDOF(ASDOF dof)
    {
        dof.bone = bone;
        dof.count = count;
        dof.twistMin = twistMin;
        dof.twistMax = twistMax;
        dof.swingXMin = swingXMin;
        dof.swingXMax = swingXMax;
        dof.swingZMin = swingZMin;
        dof.swingZMax = swingZMax;
        return dof;
    }
    public ASBone bone;
    public int count;
    public FloatValueIF twistMin;
    public FloatValueIF twistMax;
    public FloatValueIF swingXMin;
    public FloatValueIF swingXMax;
    public FloatValueIF swingZMin;
    public FloatValueIF swingZMax;
    public SliderWrapper twistSlider;
    public SliderWrapper swingXSlider;
    public SliderWrapper swingZSlider;
    public void Update(ASDOF dof)
    {
        SetValues(dof);
        twistMin.Update();
        twistMax.Update();
        swingXMin.Update();
        swingXMax.Update();
        swingZMin.Update();
        swingZMax.Update();
        twistSlider.UpdateRange();
        swingXSlider.UpdateRange();
        swingZSlider.UpdateRange();
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(UIDOFEditor))]
public class UIDOFEditorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var o = (UIDOFEditor)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Iter"))
        {
            o.Iteration();
        }
    }
}
#endif
public class UIDOFEditor : MonoBehaviour
{
    public static UIDOFEditor I
    {
        get { if (instance == null) instance = FindObjectOfType<UIDOFEditor>(); return instance; }
    }
    static UIDOFEditor instance;
    public Color labelColor = Color.black;
    public Color floatFieldColor = Color.black;
    public Color floatLabelColor = Color.black;
    public ASBone bone = ASBone.chest; // 给出个初始值
    public ASIKTarget ikTarget = ASIKTarget.LeftHand;
    public ASIKTargetSingle ikTargetSingle = ASIKTargetSingle.LeftElbow;

    public Vector3 lockPos1;
    public Vector3 lockPos2;

    public DOFProp dofP;
    public ASDOF dof;
    public ASDOFMgr dofSet;
    public ASAvatar avatar;

    public FloatValueR twist;
    public FloatValueR swingX;
    public FloatValueR swingZ;
    public BoolValueToggle IK;
    public BoolValueToggle IKSingle;
    public BoolValueToggle lockOneSide;
    public BoolValueToggle lockMirror;
    //[NonSerialized]
    public StringValueIF strif;
    [NonSerialized]
    private StringValueLabel clipNameLabel;

    public FloatValueIF twistFloatProp;
    public FloatValueIF swingXFloatProp;
    public FloatValueIF swingZFloatProp;

    public Transform target;
    public Transform end;
    public int iter;
    public ASTransDOF ast;
    public ASTransDOF astIK;
    public float alpha; // 逼近的步长
    public float theta0;
    public float theta1;
    public List<ASBone> joints;
    public List<ASBone> joints2;
    public int jointIterCount = 10;
    public int axisIterCount = 20;

    void Start()
    {
        IK = new BoolValueToggle(false);
        IKSingle = new BoolValueToggle(false);

        strif = new StringValueIF("New");
        clipNameLabel = new StringValueLabel(UIClip.clip.clipName);
        clipNameLabel.Update();
        //Debug.Log(clipNameLabel.label.text);

        ASUI.labelColor = labelColor;
        ASUI.floatFieldColor = floatFieldColor;
        ASUI.floatLabelColor = floatLabelColor;

        dofSet.Load();
        dof = dofSet[bone];
        ast = avatar[bone];
        dofP = new DOFProp(dof);

        ASUI.parent = transform.Search("Area");

        float headWidth = 40f;
        float valueWidth = 50f;
        float RWidth = 30f;

        ASUI.BeginHorizon();
        ASUI.LabelField(clipNameLabel);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.LabelField("自转");
        ASUI.FloatField("向内", dofP.twistMin, 360, OnRangeChanged);
        ASUI.FloatField("向外", dofP.twistMax, 360, OnRangeChanged);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        //ASUI.LabelField("", headWidth);
        ASUI.Button("R", RWidth, OnTwistReset);
        dofP.twistSlider = ASUI.Slider(twist, dofP.twistMin, dofP.twistMax, OnEulerValueChanged);
        ASUI.FloatFieldWithOutLabel(valueWidth, twistFloatProp, OnTwistValueChanged);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.LabelField("摆动");
        ASUI.FloatField("向前", dofP.swingXMin, 360, OnRangeChanged);
        ASUI.FloatField("向后", dofP.swingXMax, 360, OnRangeChanged);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        //ASUI.LabelField("", headWidth);
        ASUI.Button("R", RWidth, OnSwingXReset);
        dofP.swingXSlider = ASUI.Slider(swingX, dofP.swingXMin, dofP.swingXMax, OnEulerValueChanged);
        ASUI.FloatFieldWithOutLabel(valueWidth, swingXFloatProp, OnSwingXValueChanged);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.LabelField("");
        ASUI.FloatField("向内", dofP.swingZMin, 360, OnRangeChanged);
        ASUI.FloatField("向外", dofP.swingZMax, 360, OnRangeChanged);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        //ASUI.LabelField("", headWidth);
        ASUI.Button("R", RWidth, OnSwingZReset);
        dofP.swingZSlider = ASUI.Slider(swingZ, dofP.swingZMin, dofP.swingZMax, OnEulerValueChanged);
        ASUI.FloatFieldWithOutLabel(valueWidth, swingZFloatProp, OnSwingZValueChanged);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.LabelField("骨骼", headWidth);
        var nameList = new List<string>();
        nameList.AddRange(ASBoneTool.names);
        nameList.Add("手枪");
        ASUI.DropdownInt(0, avatar.setting.asts.Count, nameList.ToArray(), DropdownChange);
        //ASUI.DropdownEnum(bone, avatar.setting.asts.Count - 1, nameList.ToArray(), DropdownChange);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.Button("保存DOF", SaveAvatarSetting);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.Button("保存Clip", SaveClip);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.StringField("文件名", strif, OnFileNameChanged);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.Button("打开文件", LoadClip);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.Button("新建Clip", NewClip);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.Button("插入关键帧", InsertKeyToAllCurves);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.Button("插入丢失曲线", InsertMissCurve);
        ASUI.EndHorizon();       

        ASUI.BeginHorizon();
        ASUI.Slider(swingZ, dofP.swingZMin, dofP.swingZMax, OnIKBoneLengthChanged);
        ASUI.EndHorizon();


        ASUI.BeginHorizon();
        ASUI.LabelField("IK目标", headWidth * 4);
        ikTarget = ASIKTarget.LeftHand;
        ASUI.DropdownEnum(ikTarget, (int)ASIKTarget.Count, ikTargetNames, IKTargetChange);
        IKTargetChange((int)ikTarget);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.Toggle("使用IK", 80f * 2, IK, OnIKToggle);
        ASUI.Button("Snap", OnIKSnap); //IK目标设为当前位置
        ASUI.EndHorizon();


        ASUI.BeginHorizon();
        ASUI.LabelField("IK目标(单关节)", headWidth * 4);
        ikTargetSingle = ASIKTargetSingle.LeftHand;
        ASUI.DropdownEnum(ikTargetSingle, (int)ASIKTargetSingle.Count, ikTargetSingleNames, IKSingleTargetChange);
        IKSingleTargetChange((int)ikTargetSingle);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.Toggle("使用IK(单关节)", 80f * 3, IKSingle, OnIKSingleToggle);
        ASUI.Button("Snap", OnIKSingleSnap);
        //ASUI.Button("单关节IK目标设为当前位置", OnIKSingleSnap);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.Toggle("武器IK", 80f * 2, GunIKBoolValueToggle, OnWeaponIK);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.Toggle("锁定骨骼位置", 160, lockOneSide, LockOneSideToggle);
        ASUI.DropdownEnum(ASIKTarget.LeftLeg, (int)ASIKTarget.Count, ikTargetNames, LockTargetChange);
        LockTargetChange((int)ASIKTarget.LeftLeg);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.Toggle("锁定骨骼位置(对称)", 160, lockMirror, LockMirrorToggle);
        ASUI.DropdownEnum(ASIKTargetMirror.Leg, (int)ASIKTargetMirror.Count, ikTargetMirrorNames, LockMirrorTargetChange);
        LockMirrorTargetChange((int)ASIKTargetMirror.Leg);
        ASUI.EndHorizon();

        ASUI.I.inputCallBacks.Add(new ASGUI.InputCallBack(GetInput, 1));

        UpdateDOF();
        gun = avatar[ASBone.other].transform;
    }

    private void LockOneSideToggle(bool value)
    {
        lockOneSide.Value = value;
        if (value) { lockMirror.Value = IKSingle.Value = IK.Value = false; }
    }

    private void LockMirrorToggle(bool value)
    {
        lockMirror.Value = value;
        if (value) { lockOneSide.Value = IKSingle.Value = IK.Value = false; }
    }

    private void LockTargetChange(int index)
    {
        joints = new List<ASBone>();
        var lockTarget = (ASIKTarget)index;
        switch (lockTarget)
        {
            case ASIKTarget.RightHand:
                joints.Add(ASBone.hand_r);
                joints.Add(ASBone.forearm_r);
                joints.Add(ASBone.upperarm_r);
                break;
            case ASIKTarget.LeftHand:
                joints.Add(ASBone.hand_l);
                joints.Add(ASBone.forearm_l);
                joints.Add(ASBone.upperarm_l);
                break;
            case ASIKTarget.RightLeg:
                joints.Add(ASBone.foot_r);
                joints.Add(ASBone.shin_r);
                joints.Add(ASBone.thigh_r);
                break;
            case ASIKTarget.LeftLeg:
                joints.Add(ASBone.foot_l);
                joints.Add(ASBone.shin_l);
                joints.Add(ASBone.thigh_l);
                break;
            default: throw null;
        }
        lockPos1 = avatar[joints[0]].transform.position;
    }
    void LockMirrorTargetChange(int index)
    {
        joints = new List<ASBone>();
        joints2 = new List<ASBone>();
        var lockTargetMirror = (ASIKTargetMirror)index;
        switch (lockTargetMirror)
        {
            case ASIKTargetMirror.Hand:
                joints.Add(ASBone.hand_l);
                joints.Add(ASBone.forearm_l);
                joints.Add(ASBone.upperarm_l);
                lockPos1 = avatar[ASBone.hand_l].transform.position;
                lockPos2 = avatar[ASBone.hand_r].transform.position;
                break;
            case ASIKTargetMirror.Leg:
                joints.Add(ASBone.foot_l);
                joints.Add(ASBone.shin_l);
                joints.Add(ASBone.thigh_l);
                lockPos1 = avatar[ASBone.foot_l].transform.position;
                lockPos2 = avatar[ASBone.foot_r].transform.position;
                break;
            default: throw null;
        }
        foreach (var j in joints)
        {
            joints2.Add(j + 1); // right side
        }
    }

    public Transform gun;
    public BoolValueToggle GunIKBoolValueToggle;
    public void GunIK()
    {
        var targetPos = target.position + Gun2LeftHand;
        joints = new List<ASBone>();
        joints.Add(ASBone.hand_l);
        joints.Add(ASBone.forearm_l);
        joints.Add(ASBone.upperarm_l);
        end = avatar[ASBone.hand_l].transform;
        IKSolve(targetPos, joints.ToArray());

        targetPos = target.position + Gun2RightHand;
        joints = new List<ASBone>();
        joints.Add(ASBone.hand_r);
        joints.Add(ASBone.forearm_r);
        joints.Add(ASBone.upperarm_r);
        end = avatar[ASBone.hand_r].transform;
        IKSolve(targetPos, joints.ToArray());
    }
    public Vector3 Gun2LeftHand;
    public Vector3 Gun2RightHand;
    public void OnWeaponIK(bool value)
    {
        if (value)
        {
            Gun2LeftHand = avatar[ASBone.hand_l].transform.position - gun.position;
            Gun2RightHand = avatar[ASBone.hand_r].transform.position - gun.position;

            FindObjectOfType<GizmosAxis>().transform.position = gun.position;
            target.position = gun.position;//只有拖动gizmosaxis时才更新，所以这里手动更新

            IK.Value = false;
            IKSingle.Value = false;
        }
    }
    private void OnTwistReset()
    {
        if (ast == null) return;
        twist.value = 0;
        dofP.twistSlider.slider.value = twist.value;
        ast.euler.y = twist;
    }
    private void OnSwingXReset()
    {
        if (ast == null) return;
        swingX.value = 0;
        dofP.swingXSlider.slider.value = swingX.value;
        ast.euler.x = swingX;
    }
    private void OnSwingZReset()
    {
        if (ast == null) return;
        swingZ.value = 0;
        dofP.swingZSlider.slider.value = swingZ.value;
        ast.euler.z = swingZ;
    }
    private void OnFileNameChanged(string arg0, StringFieldWrapper arg1)
    {
        //strif.value = arg1.field.text;
    }
    private void NewClip()
    {
        UIClip.I.New(strif.field.text);
        clipNameLabel.value = UIClip.clip.clipName;
        clipNameLabel.Update();
        Debug.Log(clipNameLabel.value);
    }
    string[] ikTargetNames = new string[] { "左手", "右手", "左脚", "右脚" };
    string[] ikTargetMirrorNames = new string[] { "双手", "双脚" };
    public enum ASIKTarget
    {
        LeftHand,
        RightHand,
        LeftLeg,
        RightLeg,
        Count
    }
    public enum ASIKTargetMirror
    {
        Hand,
        Leg,
        Count
    }
    string[] ikTargetSingleNames = new string[] { "左手", "左手肘", "右手", "右手肘", "左脚", "左膝", "右脚", "右膝" };
    string[] ikTargetSingleMirrorNames = new string[] { "双手", "双肘", "双脚", "双膝" };
    public enum ASIKTargetSingle
    {
        LeftHand,
        LeftElbow,
        RightHand,
        RightElbow,
        LeftLeg,
        LeftKnee,
        RightLeg,
        RightKnee,
        Count
    }
    public enum ASIKTargetSingleMirror
    {
        Hand,
        Elbow,
        Leg,
        Knee,
        Count
    }
    void GetInput()
    {
        if (Events.Click && ASUI.MouseOver(transform.Search("Area") as RectTransform)) Events.Use();//拦截点击事件，防止穿透
    }
    void OnIKBoneLengthChanged(float v, SliderWrapper slider)
    {

    }
    void OnIKToggle(bool value)
    {
        IK.Value = value;
        if (value) { IKSingle.Value = lockOneSide.Value = lockMirror.Value = false; }
    }
    void OnIKSingleToggle(bool value)
    {
        IKSingle.Value = value;
        if (value) { IK.Value = lockOneSide.Value = lockMirror.Value = false; }
    }
    private void OnIKSnap()
    {
        var gizmos = FindObjectOfType<GizmosAxis>();
        gizmos.transform.position = end.position;
        target.position = end.position;
    }
    private void OnIKSingleSnap()
    {
        var gizmos = FindObjectOfType<GizmosAxis>();
        gizmos.transform.position = end.position;
        target.position = end.position;
    }
    void ifInsert(ASCurve asc, float v)
    {
        if (asc.keys != null && asc.keys.Count > 0)
        {
            if (asc.IndexOf(UITimeLine.FrameIndex) > -1)
            {
                asc.InsertKey(UITimeLine.FrameIndex, v);
            }
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
        foreach (var ast in avatar.setting.asts)
        {
            if (MissAst(ast)) // 插入新增的（化身ast表里有，clip里却没有的）曲线
            {
                UIClip.clip.curves.Add(new ASObjectCurve(ast));
            }
        }
    }
    public void InsertKeyToAllCurves()
    {
        foreach (var curve in UIClip.clip.curves)
        {
            //var pos = curve.ast.transform.localPosition;
            //ifInsert(curve.localPosition[0], pos.x);
            //ifInsert(curve.localPosition[1], pos.y);
            //ifInsert(curve.localPosition[2], pos.z);
            //UIClip.clip.AddEulerCurve(curve, UITimeLine.FrameIndex, curve.ast.euler);
            UIClip.clip.AddEulerPos(curve, UITimeLine.FrameIndex, curve.ast.euler, curve.ast.transform.localPosition);
        }
    }
    void LoadClip()
    {
        UIClip.I.Load(strif.field.text);
        clipNameLabel.value = UIClip.clip.clipName;
        clipNameLabel.Update();
        Debug.Log("load " + clipNameLabel.value);
    }
    void SaveClip()
    {
        UIClip.I.Save();
        Debug.Log("save " + clipNameLabel.value);
    }
    void SaveAvatarSetting()
    {
        // 先修改DOF集，再引用到当前化身（Avatar）。不同化身，如不同化身（但骨架形状类似）的人物，生物可能使用相同的DOF集。
        dofSet.Save();
        avatar.LoadFromDOFMgr();
        avatar.SaveASTs();
    }
    void OnRangeChanged(float v, FloatFieldWrapper o)
    {
        if (dof != null) dofP.SaveASDOF(dof);
        dofP.Update(dof);
        twist.value = dofP.twistSlider.slider.value;
        swingX.value = dofP.swingXSlider.slider.value;
        swingZ.value = dofP.swingZSlider.slider.value;
        OnEulerValueChanged(0, null);
    }
    private void OnTwistValueChanged(float arg0, FloatFieldWrapper arg1)
    {
        if (ast == null) return;
        float result;
        bool success = float.TryParse(twistFloatProp.field.text, out result);
        if (success)
        {
            result = Mathf.Clamp(result, dofP.twistMin, dofP.twistMax); // 只有解析成功的值会赋值
            twist.value = result;
            twistFloatProp.field.text = result.ToString();
            dofP.twistSlider.slider.value = result;
            ast.euler.y = result;
        }
    }
    private void OnSwingXValueChanged(float arg0, FloatFieldWrapper arg1)
    {
        if (ast == null) return;
        float result;
        bool success = float.TryParse(swingXFloatProp.field.text, out result);
        if (success)
        {
            result = Mathf.Clamp(result, dofP.swingXMin, dofP.swingXMax);
            swingX.value = result;
            swingXFloatProp.field.text = result.ToString();
            dofP.swingXSlider.slider.value = result;
            ast.euler.x = result;
        }
    }
    private void OnSwingZValueChanged(float arg0, FloatFieldWrapper arg1)
    {
        if (ast == null) return;
        float result;
        bool success = float.TryParse(swingZFloatProp.field.text, out result);
        if (success)
        {
            result = Mathf.Clamp(result, dofP.swingZMin, dofP.swingZMax);
            swingZ.value = result;
            swingZFloatProp.field.text = result.ToString();
            dofP.swingZSlider.slider.value = result;
            ast.euler.z = result;
        }
    }
    void OnEulerValueChanged(float v, SliderWrapper s)
    {
        if (ast == null) return;
        //if (IK) return;
        twistFloatProp.field.text = twist.ToString();
        swingXFloatProp.field.text = swingX.ToString();
        swingZFloatProp.field.text = swingZ.ToString();
        ast.euler.y = twist;
        ast.euler.x = swingX;
        ast.euler.z = swingZ;
    }
    internal void UpdateValueDisplay()
    {
        if (ast == null) return;
        twistFloatProp.field.text = twist.ToString();
        swingXFloatProp.field.text = swingX.ToString();
        swingZFloatProp.field.text = swingZ.ToString();

        twist.value = ast.euler.y;
        swingX.value = ast.euler.x;
        swingZ.value = ast.euler.z;
        dofP.twistSlider.slider.value = twist;
        dofP.swingXSlider.slider.value = swingX;
        dofP.swingZSlider.slider.value = swingZ;

        //ast.euler.y = twist; //似乎ast跟asts表里的指针指向同一实例，所以不需要更新
        //ast.euler.x = swingX;
        //ast.euler.z = swingZ;
    }
    void IKTargetChange(int index)
    {
        joints = new List<ASBone>();
        var ikTarget = (ASIKTarget)index;
        switch (ikTarget)
        {
            case ASIKTarget.RightHand:
                joints.Add(ASBone.hand_r);
                joints.Add(ASBone.forearm_r);
                joints.Add(ASBone.upperarm_r);
                break;
            case ASIKTarget.LeftHand:
                joints.Add(ASBone.hand_l);
                joints.Add(ASBone.forearm_l);
                joints.Add(ASBone.upperarm_l);
                break;
            case ASIKTarget.RightLeg:
                joints.Add(ASBone.foot_r);
                joints.Add(ASBone.shin_r);
                joints.Add(ASBone.thigh_r);
                break;
            case ASIKTarget.LeftLeg:
                joints.Add(ASBone.foot_l);
                joints.Add(ASBone.shin_l);
                joints.Add(ASBone.thigh_l);
                break;
            default: throw null;
        }
        end = avatar[joints[0]].transform;
        OnIKSnap();
    }
    void IKSingleTargetChange(int index)
    {
        joints = new List<ASBone>();
        var ikSingleTarget = (ASIKTargetSingle)index;
        switch (ikSingleTarget)
        {
            case ASIKTargetSingle.RightElbow:
                joints.Add(ASBone.forearm_r);
                joints.Add(ASBone.upperarm_r);
                break;
            case ASIKTargetSingle.RightHand:
                joints.Add(ASBone.hand_r);
                joints.Add(ASBone.forearm_r);
                break;
            case ASIKTargetSingle.LeftElbow:
                joints.Add(ASBone.forearm_l);
                joints.Add(ASBone.upperarm_l);
                break;
            case ASIKTargetSingle.LeftHand:
                joints.Add(ASBone.hand_l);
                joints.Add(ASBone.forearm_l);
                break;
            case ASIKTargetSingle.RightKnee:
                joints.Add(ASBone.shin_r);
                joints.Add(ASBone.thigh_r);
                break;
            case ASIKTargetSingle.RightLeg:
                joints.Add(ASBone.foot_r);
                joints.Add(ASBone.shin_r);
                break;
            case ASIKTargetSingle.LeftKnee:
                joints.Add(ASBone.shin_l);
                joints.Add(ASBone.thigh_l);
                break;
            case ASIKTargetSingle.LeftLeg:
                joints.Add(ASBone.foot_l);
                joints.Add(ASBone.shin_l);
                break;
            default: throw null;
        }
        end = avatar[joints[0]].transform;
        OnIKSingleSnap();
    }
    void DropdownChange(int index)
    {
        if (dof != null) dofP.SaveASDOF(dof);//先保存
        var boneInt = (ASBone)index;
        //if (boneInt < ASBone.other)
        //{
        //    dof = dofSet[boneInt];
        //    ast = avatar[boneInt];
        //}
        dof = dofSet[boneInt];
        ast = avatar[boneInt];
        UpdateDOF();
    }
    void UpdateDOF()
    {
        if (dof == null) return;
        dofP.Update(dof);
        if (ast != null)
        {
            ast.dof = dof;
            dofP.twistSlider.slider.value = ast.euler.y;
            dofP.swingXSlider.slider.value = ast.euler.x;
            dofP.swingZSlider.slider.value = ast.euler.z;
        }
    }
    void Update()
    {
        if (IK || IKSingle)
        {
            IKSolve(joints.ToArray());
            //IKSolve(forearm, uparm, hand);
            //uparm.coord.DrawRay(uparm.transform, uparm.euler, 0.5f, false);
        }
        else if (lockOneSide || lockMirror)
        {
            end = avatar[joints[0]].transform;
            IKSolve(lockPos1, joints.ToArray());
            if (lockMirror)
            {
                end = avatar[joints2[0]].transform;
                IKSolve(lockPos2, joints2.ToArray());
            }
        }
        else if (GunIKBoolValueToggle) GunIK();
    }
    float Dist(Vector3 targetPos)
    {
        var endDir = end.position - astIK.transform.position; // 当前终端方向与目标方向的距离
        var targetDir = targetPos - astIK.transform.position;
        var dist = Vector3.Distance(endDir, targetDir);
        return dist;
    }
    public bool Iteration()
    {
        return Iteration(target.position);
    }
    public bool Iteration(Vector3 targetPos)
    {
        var dict = new SortedDictionary<float, float>();
        theta0 = GetIterValue();
        dict.Add(Dist(targetPos), GetIterValue()); // 计算当前距离并存放到字典里，以距离作为Key排序

        SetIterValue(theta0 + alpha); // 计算向前步进后的距离，放进字典
        var dist = Dist(targetPos);
        if (!dict.ContainsKey(dist)) dict.Add(dist, GetIterValue()); //（字典里的值是被DOF限制后的）

        SetIterValue(theta0 - alpha); // 反向
        dist = Dist(targetPos);
        if (!dict.ContainsKey(dist)) dict.Add(dist, GetIterValue());

        foreach (var i in dict)
        {
            SetIterValue(i.Value);
            astIK.Rotate();
            break;
        }
        theta1 = GetIterValue();
        return MathTool.Approx(theta0, theta1); // 是否已经接近最佳值
    }

    float GetIterValue()
    {
        switch (iter)
        {
            case 0: return astIK.euler.z;
            case 1: return astIK.euler.x;
            case 2: return astIK.euler.y;
            default: throw null;
        }
    }
    void SetIterValue(float value)
    {
        switch (iter)
        {
            case 0: astIK.euler.z = value; break;
            case 1: astIK.euler.x = value; break;
            case 2: astIK.euler.y = value; break;
            default: throw null;
        }
        astIK.Rotate();
    }
    private void IKSolve(params ASBone[] bones)
    {
        IKSolve(target.position, bones);
    }
    private void IKSolve(Vector3 targetPos, params ASBone[] bones)
    {
        var joints = new List<ASTransDOF>();
        foreach (var bone in bones)
        {
            joints.Add(avatar[bone]);
        }
        IKSolve(targetPos, joints.ToArray());
    }
    private void IKSolve(params ASTransDOF[] joints)
    {
        IKSolve(target.position, joints);
    }
    private void IKSolve(Vector3 targetPos, params ASTransDOF[] joints)
    {
        //带DOF的IK思路：把欧拉旋转拆分为三个旋转分量，像迭代关节一样按照旋转顺序进行循环下降迭代。
        int c = jointIterCount;
        while (c > 0)
        {
            foreach (var joi in joints)
            {
                astIK = joi;
                iter = 0;
                int c2 = axisIterCount;
                while (true)
                {
                    if (Iteration(targetPos) || c2 <= 0)
                    {
                        iter++;
                        if (iter > 2) break;
                    }
                    c2--;
                }
            }
            c--;
        }
    }
}
