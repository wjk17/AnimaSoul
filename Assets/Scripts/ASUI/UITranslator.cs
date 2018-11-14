using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITranslator : MonoBehaviour
{
    public static UITranslator I
    {
        get { if (_i == null) _i = FindObjectOfType<UITranslator>(); return _i; }
    }
    static UITranslator _i;
    public Slider sliderX;
    public Slider sliderY;
    public Slider sliderZ;
    public InputField textX;
    public InputField textY;
    public InputField textZ;
    public Vector2 sliderRange;
    public Toggle control;
    public Toggle update;
    public Slider sliderSens;
    public InputField textSens;
    public float range;
    public Vector3 sensRange = new Vector3(0, 1, 2);
    public float sens
    {
        set
        {
            ignoreChanged = true;
            sliderX.maxValue =
                sliderY.maxValue =
                sliderZ.maxValue = sliderRange.y * sliderSens.value;
            sliderX.minValue =
                sliderY.minValue =
                sliderZ.minValue = sliderRange.x * sliderSens.value;
            ignoreChanged = false;
        }
    }
    bool ignoreChanged;
    List<float> valuePrev;
    public UnityEngine.UI.Button btnSetOrigin;
    private void Start()
    {
        this.AddInputCB();
        sliderX.Init(OnSliderChangedX);
        sliderY.Init(OnSliderChangedY);
        sliderZ.Init(OnSliderChangedZ);
        textX.Init(OnTextChangedX);
        textY.Init(OnTextChangedY);
        textZ.Init(OnTextChangedZ);
        sliderX.minValue = -range;
        sliderX.maxValue = range;
        sliderY.minValue = -range;
        sliderY.maxValue = range;
        sliderZ.minValue = -range;
        sliderZ.maxValue = range;
        control.Init(OnToggleControl, true);
        btnSetOrigin.Init(OnSetOrigin);
        UIDOFEditor.I.onDropdownChanged += () =>
        {
            if (control.isOn)
                GizmosAxis.I.controlObj = UIDOFEditor.I.ast.transform;
            UpdateValueDisplay();
        };
        sliderSens.Init(sensRange, OnSensChanged);
        textSens.Init(OnSensChanged);
    }
    private void OnSetOrigin()
    {
        var ast = UIDOFEditor.I.ast;
        if (ast != null)
        {
            ast.coord.originPos = ast.transform.localPosition;
        }
    }

    void OnSensChanged(string s)
    {
        if (ignoreChanged) return;
        float result;
        bool success = float.TryParse(s, out result);
        if (success)
        {
            sliderSens.value = result;
        }
    }
    private void OnSensChanged(float arg0)
    {
        if (ignoreChanged) return;
        sens = arg0;

        ignoreChanged = true;
        textSens.text = arg0.ToString();
        ignoreChanged = false;
    }

    public void OnClick()
    {
        foreach (var curve in UIClip.I.clip.curves)
        {
            UIClip.I.clip.AddEulerPosAllCurve(UITimeLine.I.frameIdx);
        }
    }
    void OnToggleControl(bool on)
    {
        if (on)
        {
            GizmosAxis.I.gameObject.SetActive(true);
            GizmosAxis.I.controlObj = UIDOFEditor.I.ast.transform;
        }
        else
        {
            GizmosAxis.I.gameObject.SetActive(false);
            GizmosAxis.I.controlObj = UIDOFEditor.I.target;
        }
    }

    internal void UpdateValueDisplay()
    {
        if (UIDOFEditor.I.ast != null)
        {
            ignoreChanged = true;
            var ast = UIDOFEditor.I.ast;
            sliderX.value = ast.pos.x;
            sliderY.value = ast.pos.y;
            sliderZ.value = ast.pos.z;
            textX.text = ast.pos.x.ToString();
            textY.text = ast.pos.y.ToString();
            textZ.text = ast.pos.z.ToString();
            ignoreChanged = false;
        }
    }
    void OnSliderChangedX(float value)
    {
        OnSliderChanged(1, value);
    }
    void OnSliderChangedY(float value)
    {
        OnSliderChanged(2, value);
    }
    void OnSliderChangedZ(float value)
    {
        OnSliderChanged(3, value);
    }
    void OnSliderChanged(int index, float value)
    {
        //var prev = valuePrev[index - 1];
        //var os = value - prev;
        //var v = prev + os * sens.y;
        //valuePrev[index - 1] = v;

        var v = value;
        if (!ignoreChanged && update.isOn && UIDOFEditor.I.ast != null)
        {
            if (index == 1) UIDOFEditor.I.ast.SetPosX(v);
            else if (index == 2) UIDOFEditor.I.ast.SetPosY(v);
            else if (index == 3) UIDOFEditor.I.ast.SetPosZ(v);
            else Debug.LogError("");
            UpdateValueDisplay();
        }
    }
    void OnTextChangedX(string s)
    {
        OnTextChanged(1, s);
    }
    void OnTextChangedY(string s)
    {
        OnTextChanged(2, s);
    }
    void OnTextChangedZ(string s)
    {
        OnTextChanged(3, s);
    }
    void OnTextChanged(int index, string s)
    {
        if (!ignoreChanged && update.isOn && UIDOFEditor.I.ast != null)
        {
            float result;
            bool success = float.TryParse(s, out result);
            if (success)
            {
                if (index == 1) sliderX.value = result;
                else if (index == 2) sliderY.value = result;
                else if (index == 3) sliderZ.value = result;
                else Debug.LogError("");
            }
        }
    }
}
