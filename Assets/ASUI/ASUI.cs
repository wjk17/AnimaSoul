using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OBJ = UnityEngine.Object;
using UnityEngine.Events;
using UnityEngine.UI;

public class FloatFieldWrapper
{
    public UnityAction<float, FloatFieldWrapper> onValueChanged;
    public InputField field;
    public FloatValue value;
    public FloatFieldWrapper(InputField f, FloatValue v)
    {
        field = f;
        value = v;
    }
    public void OnValueChanged(string s)
    {
        float.TryParse(s, out value.value);
        if (onValueChanged != null) onValueChanged(value, this);
    }
}
public class SliderWrapper
{
    public UnityAction<float, SliderWrapper> onSliderValueChanged;
    public Slider slider;
    public FloatValue value;
    public FloatValue min;
    public FloatValue max;
    public bool ignoreValueChanged = false;
    public void UpdateRange()
    {
        ignoreValueChanged = true;
        slider.minValue = min;
        slider.maxValue = max;
        ignoreValueChanged = false;
    }
    public SliderWrapper(Slider s, FloatValue v, FloatValue m, FloatValue mx)
    {
        slider = s;
        value = v;
        min = m;
        max = mx;
    }
    public void OnSliderChanged(float f)
    {
        if (ignoreValueChanged) return;
        value.value = f;
        if (onSliderValueChanged != null) onSliderValueChanged(f, this);
    }
}
public static class ASGUI
{
    public static Color labelColor = Color.black;
    public static Color floatLabelColor = Color.black;
    public static Color floatFieldColor = Color.black;
    public static Color dropdownColor = Color.blue;
    static List<UIHorizon> horizons;
    static UIHorizon horizon;
    static float ySpace = 10f;
    public static void BeginHorizon()
    {
        if (horizons == null) horizons = new List<UIHorizon>();
        horizon = new UIHorizon(horizon == null ? -ySpace : horizon.bottom - ySpace);
        horizons.Add(horizon);
    }
    public static void EndHorizon()//不能无UIEndHorizon，否则分母为0
    {
        var xScale = (parent as RectTransform).sizeDelta.x / horizon.right;
        foreach (var rt in horizon.rts)
        {
            rt.anchoredPosition = rt.anchoredPosition.SetX(rt.anchoredPosition.x * xScale);
            rt.sizeDelta = rt.sizeDelta.SetX(rt.sizeDelta.x * xScale);
        }
    }
    public static InputField inputFieldPrefab;
    public static Text labelPrefab;
    public static Dropdown dropdownPrefab;
    public static Slider sliderPrefab;
    public static Transform parent;
    internal static SliderWrapper Slider(FloatValue value, FloatValue min, FloatValue max, UnityAction<float, SliderWrapper> onSliderValueChanged)
    {
        var slider = OBJ.Instantiate(sliderPrefab.gameObject, parent).GetComponent<Slider>();
        var wrapper = new SliderWrapper(slider, value, min, max);
        wrapper.onSliderValueChanged = onSliderValueChanged;
        slider.onValueChanged.AddListener(wrapper.OnSliderChanged);
        slider.gameObject.SetActive(true);
        horizon.Add(slider);
        return wrapper;
    }
    public static void DropdownEnum(Enum enumValue, int count, string[] names, UnityAction<int> onValueChanged)
    {
        var drop = OBJ.Instantiate(dropdownPrefab.gameObject, parent).GetComponent<Dropdown>();
        drop.onValueChanged.AddListener(onValueChanged);
        drop.gameObject.SetActive(true);
        var enumType = enumValue.GetType();
        var values = Enum.GetValues(enumType) as int[];
        int i = 1;
        foreach (var n in values)
        {
            if (i > count) break;
            drop.options.Add(new Dropdown.OptionData(names[n]));
            i++;
        }
        int v = Convert.ToInt32(enumValue);
        Mathf.Clamp(v, 0, drop.options.Count - 1);
        drop.value = v;
        drop.itemText.color = dropdownColor;
        drop.captionText.text = drop.options[v].text;
        horizon.Add(drop);
    }
    public static void LabelField(string labelStr)
    {
        LabelField(labelStr, labelPrefab.rectTransform.sizeDelta.x);
    }
    public static void LabelField(string labelStr, float width)
    {
        var label = OBJ.Instantiate(labelPrefab.gameObject, parent).GetComponent<Text>();
        label.gameObject.SetActive(true);
        var rt = label.rectTransform;
        rt.sizeDelta = rt.sizeDelta.SetX(width);
        label.text = labelStr;
        label.color = labelColor;
        horizon.Add(label);
    }
    public static float FloatField(string labelStr, FloatValueIF value, UnityAction<float, FloatFieldWrapper> onValueChanged = null)
    {
        var label = OBJ.Instantiate(labelPrefab.gameObject, parent).GetComponent<Text>();
        label.gameObject.SetActive(true);
        label.text = labelStr;
        label.color = floatLabelColor;
        var inputField = OBJ.Instantiate(inputFieldPrefab.gameObject, parent).GetComponent<InputField>();
        inputField.contentType = InputField.ContentType.DecimalNumber;
        inputField.gameObject.SetActive(true);
        inputField.textComponent.color = floatFieldColor;
        inputField.text = value.ToString();
        var wrapper = new FloatFieldWrapper(inputField, value);
        wrapper.onValueChanged = onValueChanged;
        inputField.onValueChanged.AddListener(wrapper.OnValueChanged);
        value.field = inputField;
        horizon.Add(label);
        horizon.Add(inputField);
        return 0f;
    }
}
public class UIHorizon
{
    public List<RectTransform> rts;
    public RectTransform current;
    float y;
    public float bottom
    {
        get { return current.anchoredPosition.y - (current == null ? 0 : current.sizeDelta.y); }
    }
    public float right
    {
        get { return current == null ? 0 : current.anchoredPosition.x + current.sizeDelta.x; }
    }
    public void Add(GameObject go)
    {
        Add(go.GetComponent<RectTransform>());
    }
    public void Add(Component com)
    {
        Add(com.GetComponent<RectTransform>());
    }
    public void Add(RectTransform rt)
    {
        rts.Add(rt);
        rt.anchoredPosition = new Vector2(right, y);
        current = rt;
    }
    public UIHorizon(float y)
    {
        this.y = y;
        rts = new List<RectTransform>();
    }
}
[Serializable]
public class FloatValueIF : FloatValue
{
    public InputField field;
    public FloatValueIF(float f) : base(f) { }
    public override void Update()
    {
        if (field != null)
        {
            field.text = value.ToString();
        }
    }
}
[Serializable]
public abstract class FloatValue
{
    public float value;
    public abstract void Update();
    public FloatValue(float f)
    {
        value = f;
    }
    public override string ToString()
    {
        return value.ToString();
    }
    public static implicit operator float(FloatValue f)
    {
        return f.value;
    }
}
[Serializable]
public class FloatValueR : FloatValue
{
    public Text text;
    public FloatValueR(float f) : base(f) { }
    public override void Update()
    {
        if (text != null)
        {
            text.text = value.ToString();
        }
    }

}