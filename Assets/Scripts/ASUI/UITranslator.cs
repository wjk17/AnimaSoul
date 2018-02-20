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
    public Toggle control;
    public Toggle update;
    public float range;
    private void Awake()
    {
        //control.isOn = false;
        //update.isOn = false;
    }
    private void Start()
    {
        sliderX.onValueChanged.AddListener(OnSliderChangeX);
        sliderY.onValueChanged.AddListener(OnSliderChangeY);
        sliderZ.onValueChanged.AddListener(OnSliderChangeZ);
        textX.onValueChanged.AddListener(OnTextChangeX);
        textY.onValueChanged.AddListener(OnTextChangeY);
        textZ.onValueChanged.AddListener(OnTextChangeZ);
        sliderX.minValue = -range;
        sliderX.maxValue = range;
        sliderY.minValue = -range;
        sliderY.maxValue = range;
        sliderZ.minValue = -range;
        sliderZ.maxValue = range;
        control.onValueChanged.AddListener(OnToggle);
    }
    public void OnClick()
    {
        foreach (var curve in UIClip.clip.curves)
        {
            UIClip.clip.AddPositionCurve(curve, UITimeLine.FrameIndex, curve.ast.transform.localPosition);
        }
    }
    void OnToggle(bool on)
    {
        if (on)
            GizmosAxis.I.controlObj = UIDOFEditor.I.ast.transform;
        else
            GizmosAxis.I.controlObj = UIDOFEditor.I.target;
    }
    bool ignoreChange;
    internal void UpdateValueDisplay()
    {
        if (UIDOFEditor.I.ast != null)
        {
            ignoreChange = true;
            var t = UIDOFEditor.I.ast.transform;
            sliderX.value = t.localPosition.x;
            sliderY.value = t.localPosition.y;
            sliderZ.value = t.localPosition.z;
            textX.text = t.localPosition.x.ToString();
            textY.text = t.localPosition.y.ToString();
            textZ.text = t.localPosition.z.ToString();
            ignoreChange = false;
        }
    }
    void OnSliderChangeX(float value)
    {
        OnSliderChange(1, value);
    }
    void OnSliderChangeY(float value)
    {
        OnSliderChange(2, value);
    }
    void OnSliderChangeZ(float value)
    {
        OnSliderChange(3, value);
    }
    void OnSliderChange(int index, float value)
    {
        if (!ignoreChange && update.isOn && UIDOFEditor.I.ast != null)
        {
            var t = UIDOFEditor.I.ast.transform;
            if (index == 1) t.localPosition = t.localPosition.SetX(value);
            else if (index == 2) t.localPosition = t.localPosition.SetY(value);
            else if (index == 3) t.localPosition = t.localPosition.SetZ(value);
            else Debug.LogError("");
            UpdateValueDisplay();
        }
    }
    void OnTextChangeX(string s)
    {
        OnTextChange(1, s);
    }
    void OnTextChangeY(string s)
    {
        OnTextChange(2, s);
    }
    void OnTextChangeZ(string s)
    {
        OnTextChange(3, s);
    }
    void OnTextChange(int index, string s)
    {
        if (!ignoreChange && update.isOn && UIDOFEditor.I.ast != null)
        {
            var t = UIDOFEditor.I.ast.transform;
            float result;
            bool success = float.TryParse(s, out result);
            if (success)
            {
                if (index == 1) t.localPosition = t.localPosition.SetX(result);
                else if (index == 2) t.localPosition = t.localPosition.SetY(result);
                else if (index == 3) t.localPosition = t.localPosition.SetZ(result);
                else Debug.LogError("");
            }
        }
    }
}
