using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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
    //[NonSerialized]
    public SliderWrapper twistSlider;
    //[NonSerialized]
    public SliderWrapper swingXSlider;
    //[NonSerialized]
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
public class UIDOFEditor : MonoBehaviour
{
    public InputField inputFieldPrefab;
    public Text labelPrefab;
    public Dropdown dropdownPrefab;
    public Slider sliderPrefab;
    private void InitPrefabs()
    {
        ASGUI.inputFieldPrefab = inputFieldPrefab;
        inputFieldPrefab.gameObject.SetActive(false);
        ASGUI.labelPrefab = labelPrefab;
        labelPrefab.gameObject.SetActive(false);
        ASGUI.dropdownPrefab = dropdownPrefab;
        dropdownPrefab.gameObject.SetActive(false);
        ASGUI.sliderPrefab = sliderPrefab;
        sliderPrefab.gameObject.SetActive(false);
    }
    public Color labelColor = Color.black;
    public Color floatFieldColor = Color.black;
    public Color floatLabelColor = Color.black;
    public ASBone bone;
    public DOFProp dofP;
    public ASDOF dof;
    public ASDOFMgr mgr;
    public ASAvatar avatar;
    public Transform trans;

    public FloatValueR twist;
    public FloatValueR swingX;
    public FloatValueR swingZ;
    void Start()
    {
        InitPrefabs();
        ASGUI.labelColor = labelColor;
        ASGUI.floatFieldColor = floatFieldColor;
        ASGUI.floatLabelColor = floatLabelColor;
        dof = mgr.GetDOF(bone);
        dofP = new DOFProp(dof);

        ASGUI.parent = transform.Search("Area");

        float headWidth = 40f;

        ASGUI.BeginHorizon();
        ASGUI.LabelField("自转");
        ASGUI.FloatField("向内", dofP.twistMin);
        ASGUI.FloatField("向外", dofP.twistMax);
        ASGUI.EndHorizon();

        ASGUI.BeginHorizon();
        ASGUI.LabelField("", headWidth);
        dofP.twistSlider = ASGUI.Slider(twist, dofP.twistMin, dofP.twistMax, OnTwistChanged);
        ASGUI.EndHorizon();

        ASGUI.BeginHorizon();
        ASGUI.LabelField("摆动");
        ASGUI.FloatField("向前", dofP.swingXMin);
        ASGUI.FloatField("向后", dofP.swingXMax);
        ASGUI.EndHorizon();

        ASGUI.BeginHorizon();
        ASGUI.LabelField("", headWidth);
        dofP.swingXSlider = ASGUI.Slider(swingX, dofP.swingXMin, dofP.swingXMax, OnSwingXChanged);
        ASGUI.EndHorizon();

        ASGUI.BeginHorizon();
        ASGUI.LabelField("");
        ASGUI.FloatField("向内", dofP.swingZMin);
        ASGUI.FloatField("向外", dofP.swingZMax);
        ASGUI.EndHorizon();

        ASGUI.BeginHorizon();
        ASGUI.LabelField("", headWidth);
        dofP.swingZSlider = ASGUI.Slider(swingZ, dofP.swingZMin, dofP.swingZMax, OnSwingZChanged);
        ASGUI.EndHorizon();

        ASGUI.BeginHorizon();
        ASGUI.LabelField("骨骼", headWidth);
        ASGUI.DropdownEnum(bone, (int)ASBone.root, ASBoneTool.names, DropdownChange);
        ASGUI.EndHorizon();

        UpdateDOF();
    }
    void OnTwistChanged(float v, SliderWrapper s)
    {
        if (trans == null) return;
        var dr = trans.GetComOrAdd<DeltaRotation>();
        dr.euler.y = twist.value;
    }
    void OnSwingXChanged(float v, SliderWrapper s)
    {
        if (trans == null) return;
        var dr = trans.GetComOrAdd<DeltaRotation>();
        dr.euler.x = swingX.value;
    }
    void OnSwingZChanged(float v, SliderWrapper s)
    {
        if (trans == null) return;
        var dr = trans.GetComOrAdd<DeltaRotation>();
        dr.euler.z = swingZ.value;
    }
    void DropdownChange(int index)
    {
        if (dof != null) dofP.SaveASDOF(dof);//先保存
        bone = (ASBone)index;
        dof = mgr.GetDOF(bone);
        UpdateDOF();
    }
    void UpdateDOF()
    {
        if (dof == null) return;
        dofP.Update(dof);
        trans = avatar[bone];
        if (trans != null)
        {
            var dr = trans.GetComOrAdd<DeltaRotation>();
            dofP.twistSlider.slider.value = dr.euler.y;
            dofP.swingXSlider.slider.value = dr.euler.x;
            dofP.swingZSlider.slider.value = dr.euler.z;
        }
    }
    void Update()
    {

    }
}
