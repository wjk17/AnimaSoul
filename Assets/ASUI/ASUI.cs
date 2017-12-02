using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Obj = UnityEngine.Object;
using System;
public enum MouseButton
{
    Left = 0,
    Right,
    Middle,
};
/// <summary>
/// 左上角坐标
/// </summary>
public static class ASUI
{
    public static void swapPts(ref Vector2 p1, ref Vector2 p2)
    {
        var tmp = p1; p1 = p2; p2 = tmp;
    }
    public static void swapCodes(ref byte c1, ref byte c2)
    {
        var tmp = c1; c1 = c2; c2 = tmp;
    }
    public static Vector2[] Offset(this RectTransform rt)
    {
        var v = MathTool.ReverseY(rt.anchoredPosition);
        return new Vector2[] { v, v + rt.sizeDelta };
    }
    public static Color labelColor = Color.black;
    public static Color floatLabelColor = Color.black;
    public static Color floatFieldColor = Color.black;
    public static Color dropdownColor = Color.blue;
    /// <summary>
    /// 将锚点和轴心设置为左上角，这样画图形时能正确地裁剪到矩形区域。
    /// </summary>
    public static RectTransform owner
    {
        get { return I.owner; }
        set { I.owner = value; I.ClearCmd(); }
    }
    public static ASGUI I
    {
        get
        {
            if (_instance == null) _instance = Obj.FindObjectOfType<ASGUI>();
            return _instance;
        }
    }
    private static CanvasScaler _scaler;
    public static CanvasScaler scaler
    {
        get
        {
            if (_scaler == null) _scaler = Obj.FindObjectOfType<CanvasScaler>();
            return _scaler;
        }
        set
        {
            _scaler = value;
        }
    }
    public static float facterToRealPixel
    {
        get
        {
            return Screen.width / scaler.referenceResolution.x;
        }
    }
    public static float facterToReference
    {
        get
        {
            return scaler.referenceResolution.x / Screen.width;
        }
    }
    public static float mouseDist(Vector2 v) { return Vector2.Distance(v, mousePositionRef); }
    public static float mouseDistLT(Vector2 v) { return Vector2.Distance(v, mousePositionRefLT); }
    // 鼠标在设计时的分辨率下的位置
    public static Vector2 mousePositionRef { get { return Input.mousePosition * facterToReference; } }
    public static Vector2 mousePositionRefLT { get { var ip = Input.mousePosition * facterToReference; ip.y = scaler.referenceResolution.y - ip.y; return ip; } }

    static ASGUI _instance;
    static List<UIHorizon> horizons;
    static UIHorizon horizon;
    static float ySpace = 10f;
    public static Transform parent;
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
    public static void Toggle(string labelStr, UnityAction<bool> onToggle = null)
    {
        Toggle(labelStr, I.togglePrefab.GetComponent<RectTransform>().sizeDelta.x, onToggle);
    }
    public static void Toggle(string labelStr, float width, UnityAction<bool> onToggle = null)
    {
        var toggle = Obj.Instantiate(I.togglePrefab, parent).GetComponent<Toggle>();
        var rt = toggle.GetComponent<RectTransform>();
        rt.sizeDelta = rt.sizeDelta.SetX(width);
        toggle.onValueChanged.AddListener(onToggle);
        var label = toggle.GetComponentInChildren<Text>(true);
        label.text = labelStr;
        label.color = labelColor;
        horizon.Add(toggle);
    }
    public static void Button(string labelStr, UnityAction onClick = null)
    {
        var button = Obj.Instantiate(I.buttonPrefab, parent).GetComponent<Button>();
        button.onClick.AddListener(onClick);
        var label = button.GetComponentInChildren<Text>(true);
        label.text = labelStr;
        label.color = labelColor;
        horizon.Add(button);
    }
    internal static SliderWrapper IntSlider(FloatValue value, FloatValue min, FloatValue max, UnityAction<float, SliderWrapper> onSliderValueChanged)
    {
        var s = Slider(value, min, max, onSliderValueChanged);
        s.slider.wholeNumbers = true;
        return s;
    }
    internal static SliderWrapper Slider(FloatValue value, FloatValue min, FloatValue max, UnityAction<float, SliderWrapper> onSliderValueChanged)
    {
        var slider = Obj.Instantiate(I.sliderPrefab, parent).GetComponent<Slider>();
        var wrapper = new SliderWrapper(slider, value, min, max);
        wrapper.onSliderValueChanged = onSliderValueChanged;
        slider.onValueChanged.AddListener(wrapper.OnSliderChanged);
        horizon.Add(slider);
        return wrapper;
    }
    public static void DropdownEnum(Enum enumValue, int count, string[] names, UnityAction<int> onValueChanged)
    {
        var drop = Obj.Instantiate(I.dropdownPrefab, parent).GetComponent<Dropdown>();
        drop.onValueChanged.AddListener(onValueChanged);
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
        LabelField(labelStr, (I.labelPrefab.transform as RectTransform).sizeDelta.x);
    }
    public static void LabelField(string labelStr, float width)
    {
        var label = Obj.Instantiate(I.labelPrefab.gameObject, parent).GetComponent<Text>();
        var rt = label.rectTransform;
        rt.sizeDelta = rt.sizeDelta.SetX(width);
        label.text = labelStr;
        label.color = labelColor;
        horizon.Add(label);
    }
    public static float FloatField(string labelStr, FloatValueIF value, UnityAction<float, FloatFieldWrapper> onValueChanged = null)
    {
        var label = Obj.Instantiate(I.labelPrefab, parent).GetComponent<Text>();
        label.text = labelStr;
        label.color = floatLabelColor;
        var inputField = Obj.Instantiate(I.inputFieldPrefab, parent).GetComponent<InputField>();
        inputField.contentType = InputField.ContentType.DecimalNumber;
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
    internal static bool MouseOver(params RectTransform[] rts)
    {
        var ip = Input.mousePosition;
        ip *= ASUI.facterToReference;
        ip.y = ASUI.scaler.referenceResolution.y - ip.y;
        foreach (var rt in rts)
        {
            var rect = rt.rect;
            rect.position = Vector2.Scale(rt.anchoredPosition, Vector2.one.SetY(-1));
            if (rect.Contains(ip)) return true;
        }
        return false;
    }
}