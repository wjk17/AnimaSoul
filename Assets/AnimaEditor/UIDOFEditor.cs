using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
[Serializable]
public class DOFProp
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
    public DOFProp dofP;
    public ASDOF dof;
    public ASDOFMgr dofSet;
    public ASAvatar avatar;

    public FloatValueR twist;
    public FloatValueR swingX;
    public FloatValueR swingZ;
    public bool IK = true;
    void Start()
    {
        ASUI.labelColor = labelColor;
        ASUI.floatFieldColor = floatFieldColor;
        ASUI.floatLabelColor = floatLabelColor;
        dof = dofSet[bone];
        ast = avatar[bone];
        dofP = new DOFProp(dof);

        ASUI.parent = transform.Search("Area");

        float headWidth = 40f;

        ASUI.BeginHorizon();
        ASUI.LabelField("自转");
        ASUI.FloatField("向内", dofP.twistMin, OnRangeChanged);
        ASUI.FloatField("向外", dofP.twistMax, OnRangeChanged);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.LabelField("", headWidth);
        dofP.twistSlider = ASUI.Slider(twist, dofP.twistMin, dofP.twistMax, OnDofValueChanged);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.LabelField("摆动");
        ASUI.FloatField("向前", dofP.swingXMin, OnRangeChanged);
        ASUI.FloatField("向后", dofP.swingXMax, OnRangeChanged);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.LabelField("", headWidth);
        dofP.swingXSlider = ASUI.Slider(swingX, dofP.swingXMin, dofP.swingXMax, OnDofValueChanged);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.LabelField("");
        ASUI.FloatField("向内", dofP.swingZMin, OnRangeChanged);
        ASUI.FloatField("向外", dofP.swingZMax, OnRangeChanged);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.LabelField("", headWidth);
        dofP.swingZSlider = ASUI.Slider(swingZ, dofP.swingZMin, dofP.swingZMax, OnDofValueChanged);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.LabelField("骨骼", headWidth);
        ASUI.DropdownEnum(bone, (int)ASBone.root, ASBoneTool.names, DropdownChange);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.Button("保存", SaveAvatarSetting);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.Slider(swingZ, dofP.swingZMin, dofP.swingZMax, OnIKBoneLengthChanged);
        ASUI.EndHorizon();

        ASUI.BeginHorizon();
        ASUI.Toggle("使用IK", 80f, OnToggle);
        ASUI.Button("IK目标设为当前位置", OnIKSnap);
        ASUI.EndHorizon();

        UpdateDOF();
    }
    void OnIKBoneLengthChanged(float v, SliderWrapper slider)
    {

    }
    private void OnIKSnap()
    {
    }
    void SaveAvatarSetting()
    {
        // 先修改DOF集，再引用到当前化身。不同化身，如不同化身（但骨架形状类似）的人物，生物可能使用相同的DOF集。
        dofSet.Save();
        avatar.LoadFromDOFMgr();
        avatar.SaveASTs();
    }
    void OnToggle(bool value)
    {

    }
    void OnRangeChanged(float v, FloatFieldWrapper o)
    {
        if (dof != null) dofP.SaveASDOF(dof);
        dofP.Update(dof);
        twist.value = dofP.twistSlider.slider.value;
        swingX.value = dofP.swingXSlider.slider.value;
        swingZ.value = dofP.swingZSlider.slider.value;
        OnDofValueChanged(0, null);
    }
    void OnDofValueChanged(float v, SliderWrapper s)
    {
        if (ast == null) return;
        if (IK) return;
        ast.euler.y = twist;
        ast.euler.x = swingX;
        ast.euler.z = swingZ;
    }
    void DropdownChange(int index)
    {
        if (dof != null) dofP.SaveASDOF(dof);//先保存
        var bone = (ASBone)index;
        dof = dofSet[bone];
        ast = avatar[bone];
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
    public Transform target;
    public Transform end;
    public int iter;
    public ASTransDOF ast;
    public float alpha; // 逼近的步长
    public float theta0;
    public float theta1;
    void Update()
    {
        if (IK)
        {
            var hand = avatar[ASBone.hand_l];
            var forearm = avatar[ASBone.forearm_l];
            var uparm = avatar[ASBone.upperarm_l];
            var joints = new List<ASBone>();
            joints.Add(ASBone.foot_l);
            joints.Add(ASBone.shin_l);
            joints.Add(ASBone.thigh_l);
            end = avatar[ASBone.foot_l].transform;
            IKSolve(joints.ToArray());
            //IKSolve(forearm, uparm, hand);
            uparm.coord.DrawRay(uparm.transform, uparm.euler, 0.5f, false);
        }
    }
    float Dist()
    {
        var endDir = end.position - ast.transform.position; // 当前终端方向与目标方向的距离
        var targetDir = target.position - ast.transform.position;
        var dist = Vector3.Distance(endDir, targetDir);
        return dist;
    }
    public bool Iteration()
    {
        var dict = new SortedDictionary<float, float>();
        theta0 = GetIterValue();
        dict.Add(Dist(), GetIterValue()); // 计算当前距离并存放到字典里，以距离作为Key排序

        SetIterValue(theta0 + alpha); // 计算向前步进后的距离，放进字典
        var dist = Dist();
        if (!dict.ContainsKey(dist)) dict.Add(dist, GetIterValue()); //（字典里的值是被DOF限制后的）

        SetIterValue(theta0 - alpha); // 反向
        dist = Dist();
        if (!dict.ContainsKey(dist)) dict.Add(dist, GetIterValue());

        foreach (var i in dict)
        {
            SetIterValue(i.Value);
            ast.Rotate();
            break;
        }
        theta1 = GetIterValue();
        return Approx(theta0, theta1); // 是否已经接近最佳值
    }
    public float approxRange = 0.00001f;
    bool Approx(float a, float b)
    {
        var r = Mathf.Abs(a - b) < approxRange;
        return r;
    }
    float GetIterValue()
    {
        switch (iter)
        {
            case 0: return ast.euler.z;
            case 1: return ast.euler.x;
            case 2: return ast.euler.y;
            default: throw null;
        }
    }
    void SetIterValue(float value)
    {
        switch (iter)
        {
            case 0: ast.euler.z = value; break;
            case 1: ast.euler.x = value; break;
            case 2: ast.euler.y = value; break;
            default: throw null;
        }
        ast.Rotate();
    }
    private void IKSolve(params ASBone[] bones)
    {
        var joints = new List<ASTransDOF>();
        foreach (var bone in bones)
        {
            joints.Add(avatar[bone]);
        }
        IKSolve(joints.ToArray());
    }
    public int jointIterCount = 10;
    public int axisIterCount = 20;
    private void IKSolve(params ASTransDOF[] joints)
    {
        //带DOF的IK思路：把欧拉旋转拆分为三个旋转分量，像迭代关节一样按照旋转顺序进行循环下降迭代。
        int c = jointIterCount;
        while (c > 0)
        {
            foreach (var joi in joints)
            {
                ast = joi;
                iter = 0;
                int c2 = axisIterCount;
                while (true)
                {
                    if (Iteration() || c2 <= 0)
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
