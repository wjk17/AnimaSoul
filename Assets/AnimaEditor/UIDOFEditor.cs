using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
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
public class UIDOFEditor : MonoBehaviourInstance<UIDOFEditor>
{
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
    public ASBone bone = ASBone.chest; // 给出个初始值

    public Vector3 lockPos1;
    public Vector3 lockPos2;

    public ASDOF dof;
    public ASDOFMgr dofSet;
    public ASAvatar avatar;

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

    public Transform gun;
    public frame frameClipBoard;

    public Vector3 Gun2LeftHand;
    public Vector3 Gun2RightHand;

    public InputField inputTwistMin;
    public InputField inputTwistMax;
    public Button buttonOnTwistReset;
    public Slider sliderTwist;
    public InputField inputTwist;

    public InputField inputSwingXMin;
    public InputField inputSwingXMax;
    public Button buttonOnSwingXReset;
    public Slider sliderSwingX;
    public InputField inputSwingX;

    public InputField inputSwingZMin;
    public InputField inputSwingZMax;
    public Button buttonSwingZReset;
    public Slider sliderSwingZ;
    public InputField inputSwingZ;

    public Toggle toggleIKSingle;
    public Button buttonIKSingleSnap;
    public Button buttonSaveAvatarSetting;
    public Button buttonSaveClip;
    public Text labelFileName;
    public InputField inputFileName;
    public Button buttonLoadClip;
    public Button buttonNewClip;
    public Button buttonInsertKeyToAllCurves;

    public Toggle toggleIK;
    public Button buttonIKSnap;
    public Toggle toggleWeaponIK;

    public Toggle toggleLockOneSide;
    public Toggle toggleLockMirror;

    public Dropdown dropBone;
    public Dropdown dropIKTarget;
    public Dropdown dropIKSingleTarget;
    public Dropdown dropLockTarget;
    public Dropdown dropLockMirrorTarget;

    public Button btnEulerReset;

    public bool ignoreChanged;
    void Start()
    {
        foreach (var curve in UIClip.clip.curves)
        {
            curve.ast.coord.originPos = curve.ast.transform.localPosition;
        }

        dofSet.Load();
        //avatar.LoadFromDOFMgr();
        dof = dofSet[bone];
        ast = avatar[bone];

        ASUI.parent = transform.Search("Area");

        // 自转  向内
        inputTwistMin.Init(360, OnInputTMinChanged);
        // 自转  向外
        inputTwistMax.Init(360, OnInputTMaxChanged);
        // R按钮  重置
        buttonOnTwistReset.Init(OnTwistReset);

        sliderTwist.Init(OnTwistSliderChanged);
        inputTwist.Init(0, OnTwistInputChanged);

        // 摆动  向前
        inputSwingXMin.Init(OnInputXMinChanged);
        // 摆动  向后
        inputSwingXMax.Init(OnInputXMaxChanged);
        // R按钮  重置
        buttonOnSwingXReset.Init(OnSwingXReset);

        sliderSwingX.Init(OnSwingXSliderChanged);
        inputSwingX.Init(OnSwingXInputChanged);

        // 摆动  向内
        inputSwingZMin.Init(OnInputZMinChanged);
        // 摆动  向外
        inputSwingZMax.Init(OnInputZMaxChanged);
        // R按钮  重置
        buttonSwingZReset.Init(OnSwingZReset);

        sliderSwingZ.Init(OnSwingZSliderChanged);
        inputSwingZ.Init(OnSwingZInputChanged);

        // 骨骼
        dropBone.Init((int)ASBone.root, ASUI.Combine(ASBoneTool.names, "手枪"), OnDropdownChanged);
        dropBone.gameObject.AddComponent<DropDownLocateSelectedItem>();
        // 保存DOF
        buttonSaveAvatarSetting.Init(SaveAvatarSetting);
        // 保存Clip
        buttonSaveClip.Init(SaveClip);
        // 文件名
        var cname = PlayerPrefs.GetString("LastOpenClipName", "New");
        inputFileName.Init(cname, OnFileNameChanged);
        // 打开文件
        buttonLoadClip.Init(LoadClip);
        // 新建Clip
        buttonNewClip.Init(NewClip);
        // 插入关键帧
        buttonInsertKeyToAllCurves.Init(InsertKeyToAllCurves);
        // IK目标
        dropIKTarget.Init(0, ikTargetNames, OnIKTargetChanged, true);
        dropIKTarget.gameObject.AddComponent<DropDownLocateSelectedItem>();
        // 使用IK
        toggleIK.Init(OnIKToggle);
        // Snap
        buttonIKSnap.Init(OnIKSnap); //IK目标设为当前位置
        // IK目标(单关节)
        dropIKSingleTarget.Init(0, ikTargetSingleNames, IKSingleTargetChange, true);
        // 使用IK(单关节)
        toggleIKSingle.Init(OnIKSingleToggle);
        // Snap
        buttonIKSingleSnap.Init(OnIKSingleSnap);
        // 武器IK
        toggleWeaponIK.Init(OnWeaponIK);
        // 锁定骨骼位置
        toggleLockOneSide.Init(OnLockOneSideToggle);
        dropLockTarget.Init(0, ikTargetNames, OnLockTargetChange, true);
        // 锁定骨骼位置(对称)
        toggleLockMirror.Init(OnLockMirrorToggle);
        dropLockMirrorTarget.Init(0, ikTargetMirrorNames, OnLockMirrorTargetChange, true);

        btnEulerReset.Init(delegate { OnSwingXReset(); OnSwingZReset(); OnTwistReset(); });

        ASUI.I.inputCallBacks.Add(new ASGUI.InputCallBack(GetInput, 1));

        UpdateDOF();
        gun = avatar[ASBone.other].transform;
    }
    private void OnLockOneSideToggle(bool value)
    {
        toggleLockOneSide.isOn = value;
        if (value) { toggleLockMirror.isOn = toggleIKSingle.isOn = toggleIK.isOn = false; }
    }
    private void OnLockMirrorToggle(bool value)
    {
        toggleLockMirror.isOn = value;
        if (value) { toggleLockOneSide.isOn = toggleIKSingle.isOn = toggleIK.isOn = false; }
    }
    private void OnLockTargetChange(int index)
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
    void OnLockMirrorTargetChange(int index)
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
    public void OnWeaponIK(bool value)
    {
        if (value)
        {
            Gun2LeftHand = avatar[ASBone.hand_l].transform.position - gun.position;
            Gun2RightHand = avatar[ASBone.hand_r].transform.position - gun.position;

            FindObjectOfType<GizmosAxis>().transform.position = gun.position;
            target.position = gun.position;//只有拖动gizmosaxis时才更新，所以这里手动更新

            toggleIK.isOn = false;
            toggleIKSingle.isOn = false;
        }
    }
    private void OnTwistReset()
    {
        sliderTwist.value = 0;
        if (ast == null) ast.euler.y = sliderTwist.value;
    }
    private void OnSwingXReset()
    {
        sliderSwingX.value = 0;
        if (ast == null) ast.euler.x = sliderSwingX.value;
    }
    private void OnSwingZReset()
    {
        sliderSwingZ.value = 0;
        if (ast == null) ast.euler.z = sliderSwingZ.value;
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
        toggleIK.isOn = value;
        if (value) { toggleIKSingle.isOn = toggleLockOneSide.isOn = toggleLockMirror.isOn = false; }
    }
    void OnIKSingleToggle(bool value)
    {
        toggleIKSingle.isOn = value;
        if (value) { toggleIK.isOn = toggleLockOneSide.isOn = toggleLockMirror.isOn = false; }
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
    public void InsertKeyToAllCurves()
    {
        UITimeLine.I.InsertKey();
    }
    private void OnFileNameChanged(string v)
    {

    }
    private void NewClip()
    {
        UIClip.I.New(inputFileName.text);
        labelFileName.text = inputFileName.text;
        Debug.Log("新建 " + labelFileName.text);
    }
    void LoadClip()
    {
        UIClip.I.Load(inputFileName.text);
        labelFileName.text = inputFileName.text;
        UIClip.I.UpdateAllCurve();
        Debug.Log("读取 " + labelFileName.text);
    }
    void SaveClip()
    {
        UIClip.I.Save(inputFileName.text);
        Debug.Log("保存 " + inputFileName.text);
    }
    void SaveAvatarSetting()
    {
        // 先修改DOF集，再引用到当前化身（Avatar）。不同化身，如不同化身（但骨架形状类似）的人物，生物可能使用相同的DOF集。
        dofSet.Save();
        avatar.LoadFromDOFMgr();
        avatar.SaveASTs();
    }
    private void OnTwistSliderChanged(float v)
    {
        if (ignoreChanged) return;
        if (ast != null) { ast.euler.y = v; UIPlayer.I.Mirror(); }
        ignoreChanged = true;
        inputTwist.text = v.ToString();
        ignoreChanged = false;
    }
    private void OnSwingXSliderChanged(float v)
    {
        if (ignoreChanged) return;
        if (ast != null) { ast.euler.x = v; UIPlayer.I.Mirror(); }
        ignoreChanged = true;
        inputSwingX.text = v.ToString();
        ignoreChanged = false;
    }
    private void OnSwingZSliderChanged(float v)
    {
        if (ignoreChanged) return;
        if (ast != null) { ast.euler.z = v; UIPlayer.I.Mirror(); }
        ignoreChanged = true;
        inputSwingZ.text = v.ToString();
        ignoreChanged = false;
    }
    private void OnTwistInputChanged(string input)
    {
        if (ignoreChanged) return;
        if (ast == null) return;
        float result;
        bool success = float.TryParse(input, out result);
        if (success)
        {
            result = Mathf.Clamp(result, ast.dof.twistMin, ast.dof.twistMax); // 只有解析成功的值会赋值                        
            ast.euler.y = result;
            ignoreChanged = true;
            sliderTwist.value = result;
            inputTwist.text = result.ToString();
            ignoreChanged = false;
        }
    }
    private void OnSwingXInputChanged(string input)
    {
        if (ignoreChanged) return;
        if (ast == null) return;
        float result;
        bool success = float.TryParse(input, out result);
        if (success)
        {
            result = Mathf.Clamp(result, ast.dof.swingXMin, ast.dof.swingXMax);
            ast.euler.x = result;
            ignoreChanged = true;
            sliderSwingX.value = result;
            inputSwingX.text = result.ToString();
            ignoreChanged = false;
        }
    }
    private void OnSwingZInputChanged(string input)
    {
        if (ignoreChanged) return;
        if (ast == null) return;
        float result;
        bool success = float.TryParse(input, out result);
        if (success)
        {
            result = Mathf.Clamp(result, ast.dof.swingZMin, ast.dof.swingZMax);
            ast.euler.z = result;
            ignoreChanged = true;
            sliderSwingZ.value = result;
            inputSwingZ.text = result.ToString();
            ignoreChanged = false;
        }
    }
    // dof范围 最小值或最大值 文本改变事件
    void OnInputXMinChanged(string s)
    {
        if (ast == null) return;
        float result;
        bool success = float.TryParse(inputSwingXMin.text, out result);
        if (success)
        {
            ast.dof.swingXMin = result;
            ignoreChanged = true;
            sliderSwingX.minValue = result;
            ignoreChanged = false;
        }
    }
    void OnInputXMaxChanged(string s)
    {
        if (ast == null) return;
        float result;
        bool success = float.TryParse(inputSwingXMax.text, out result);
        if (success)
        {
            ast.dof.swingXMax = result;
            ignoreChanged = true;
            sliderSwingX.maxValue = result;
            ignoreChanged = false;
        }
    }
    void OnInputZMinChanged(string s)
    {
        if (ast == null) return;
        float result;
        bool success = float.TryParse(inputSwingZMin.text, out result);
        if (success)
        {
            ast.dof.swingZMin = result;
            ignoreChanged = true;
            sliderSwingZ.minValue = result;
            ignoreChanged = false;
        }
    }
    void OnInputZMaxChanged(string s)
    {
        if (ast == null) return;
        float result;
        bool success = float.TryParse(inputSwingZMax.text, out result);
        if (success)
        {
            ast.dof.swingZMax = result;
            ignoreChanged = true;
            sliderSwingZ.maxValue = result;
            ignoreChanged = false;
        }
    }
    void OnInputTMinChanged(string s)
    {
        if (ast == null) return;
        float result;
        bool success = float.TryParse(inputTwistMin.text, out result);
        if (success)
        {
            ast.dof.twistMin = result;
            ignoreChanged = true;
            sliderTwist.minValue = result;
            ignoreChanged = false;
        }
    }
    void OnInputTMaxChanged(string s)
    {
        if (ast == null) return;
        float result;
        bool success = float.TryParse(inputTwistMax.text, out result);
        if (success)
        {
            ast.dof.twistMax = result;
            ignoreChanged = true;
            sliderTwist.maxValue = result;
            ignoreChanged = false;
        }
    }
    // 将最新的数值显示到面板
    internal void UpdateValueDisplay()
    {
        if (ast == null) return;
        ignoreChanged = true;
        inputTwist.text = ast.euler.y.ToString();
        inputSwingX.text = ast.euler.x.ToString();
        inputSwingZ.text = ast.euler.z.ToString();
        sliderTwist.value = ast.euler.y;
        sliderSwingX.value = ast.euler.x;
        sliderSwingZ.value = ast.euler.z;
        ignoreChanged = false;
    }
    void OnIKTargetChanged(int index)
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
    void OnDropdownChanged(int index)
    {
        var boneInt = (ASBone)index;
        dof = dofSet[boneInt];
        ast = avatar[boneInt];
        UpdateDOF();
    }
    void UpdateDOF()
    {
        if (dof == null) return;
        ignoreChanged = true;
        sliderTwist.value = ast.euler.y;
        sliderSwingX.value = ast.euler.x;
        sliderSwingZ.value = ast.euler.z;
        ignoreChanged = false;
        inputTwistMin.text = ast.dof.twistMin.ToString();
        inputTwistMax.text = ast.dof.twistMax.ToString();
        inputSwingXMin.text = ast.dof.swingXMin.ToString();
        inputSwingXMax.text = ast.dof.swingXMax.ToString();
        inputSwingZMin.text = ast.dof.swingZMin.ToString();
        inputSwingZMax.text = ast.dof.swingZMax.ToString();
    }
    private void CopyFrame()
    {
        frame n = new frame();
        n.keys = new List<key>();
        if (UIClip.clip == null || UIClip.clip.curves == null) return;
        foreach (var curve in UIClip.clip.curves)
        {
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
    ASObjectCurve GetCurve(ASBone bone)
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
    public void PasteFrame(params ASBone[] bones)
    {
        PasteFrame((ICollection<ASBone>)bones);
    }
    public void PasteFrame(ICollection<ASBone> bones)
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
    public void PasteFrameAllFrame(ICollection<ASBone> bones)
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
    public RectTransform rect;
    void Update()
    {
        var hover = ASUI.MouseOver(UICurve.I.transform as RectTransform)
            || ASUI.MouseOver(UICamera.I.rectView as RectTransform);
        if (hover && Input.GetKeyDown(KeyCode.C))
        {
            CopyFrame();
        }
        else if (hover && Input.GetKeyDown(KeyCode.V))
        {
            PasteFrame();
        }

        if (toggleIK.isOn || toggleIKSingle.isOn)
        {
            IKSolve(joints.ToArray());
            //IKSolve(forearm, uparm, hand);
            //uparm.coord.DrawRay(uparm.transform, uparm.euler, 0.5f, false);
        }
        else if (toggleLockOneSide.isOn || toggleLockMirror.isOn)
        {
            end = avatar[joints[0]].transform;
            IKSolve(lockPos1, joints.ToArray());
            if (toggleLockMirror.isOn)
            {
                end = avatar[joints2[0]].transform;
                IKSolve(lockPos2, joints2.ToArray());
            }
        }
        else if (toggleWeaponIK.isOn) GunIK();
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
