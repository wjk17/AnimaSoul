using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(LabelSlider))]
public class LabelSliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var o = (LabelSlider)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Apply"))
        {
            o.Apply();
        }
    }
}
#endif
public class LabelSlider : MonoBehaviour
{
    public Slider slider;
    public Text text;
    public Text valueText;
    public string labelName;
    public Action<float> onValueChanged;
    public float value
    {
        get { return slider.value; }
        set { slider.value = value; }
    }
    public float maxValue
    {
        get { return slider.maxValue; }
        set { slider.maxValue = value; }
    }
    public float minValue
    {
        get { return slider.minValue; }
        set { slider.minValue = value; }
    }
    public bool editable
    {
        set
        {
            var sliders = GetComponentsInChildren<Slider>(true);
            foreach (var sld in sliders)
            {
                sld.enabled = value;
            }
        }
    }
    public float nt
    {
        get
        {
            var t = value == minValue ? 0 : (value - minValue) / (maxValue - minValue);
            return t;
        }
        set
        {
            this.value = minValue + (maxValue - minValue) * value;
        }
    }
    public void OnValueChanged(float value)
    {
        if (valueText != null) valueText.text = value.ToString();
        if (onValueChanged != null) onValueChanged(slider.value);
    }
    public void Init(Vector3 MOM, Action<float> ovc, bool wholeNumbers = false)
    {
        Init(MOM.y, MOM.x, MOM.z, ovc, wholeNumbers);
    }
    public void Init(float origin, float min, float max, Action<float> ovc, bool wholeNumbers = false)
    {
        slider = GetComponentInChildren<Slider>();
        text = GetComponentInChildren<Text>();
        slider.wholeNumbers = wholeNumbers;
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = origin;
        onValueChanged = ovc;
        slider.onValueChanged.AddListener(OnValueChanged);
        OnValueChanged(value);
    }
#if UNITY_EDITOR
    public void Apply()
    {
        text = GetComponentInChildren<Text>();
        text.text = labelName;
        EditorUtility.SetDirty(text);//repaint
    }
#endif
}
