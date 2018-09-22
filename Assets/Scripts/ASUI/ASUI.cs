using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Obj = UnityEngine.Object;
using System;
/// <summary>
/// 左上角坐标
/// </summary>
public static class ASUI
{
    // 初始化UI控件
    public static void Init(this Button button, UnityAction onClick, bool trigger = false)
    {
        if (onClick != null) button.onClick.AddListener(onClick);
        if (trigger) onClick();
    }
    public static void Init(this Toggle toggle, UnityAction<bool> onToggle, bool trigger = false)
    {
        if (onToggle != null) toggle.onValueChanged.AddListener(onToggle);
        if (trigger) onToggle(toggle.isOn);
    }
    // trigger 是否立即触发事件
    public static void Init(this Dropdown drop, int value, IList<string> options, UnityAction<int> onValueChanged, bool trigger = false)
    {
        Init(drop, value, new List<string>(options), onValueChanged);
        if (trigger) onValueChanged(value);
    }
    public static void Init(this Dropdown drop, int value, List<string> options, UnityAction<int> onValueChanged)
    {
        drop.ClearOptions();
        drop.AddOptions(options);
        drop.value = value; 
        drop.onValueChanged.AddListener(onValueChanged); // add ofter set a default value, or it'll trigger the changed func
        drop.gameObject.AddComponent<DropDownLocateSelectedItem>();
    }
    public static void Init(this InputField input, int value, UnityAction<string> onValueChanged)
    {
        Init(input, value.ToString(), onValueChanged);
    }
    public static void Init(this InputField input, string text, UnityAction<string> onValueChanged, bool trigger = false)
    {
        input.text = text;
        input.onValueChanged.AddListener(onValueChanged);
        if (trigger) onValueChanged(text);
    }
    public static void Init(this InputField input, UnityAction<string> onValueChanged)
    {
        input.onValueChanged.AddListener(onValueChanged);
    }
    public static void Init(this Slider slider, UnityAction<float> onValueChanged)
    {
        slider.onValueChanged.AddListener(onValueChanged);
    }
    public static void Init(this Slider slider, Vector3 mdm, UnityAction<float> onValueChanged)
    {
        slider.minValue = mdm.x;
        slider.maxValue = mdm.z;
        slider.value = mdm.y;
        slider.onValueChanged.AddListener(onValueChanged);
    }
    public static void Init(this Slider slider, float min, float max, float defaultValue,
        UnityAction<float> onValueChanged)
    {
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = defaultValue;
        slider.onValueChanged.AddListener(onValueChanged);
    }
    public static void swapPts(ref Vector2 p1, ref Vector2 p2)
    {
        var tmp = p1; p1 = p2; p2 = tmp;
    }
    public static void swapCodes(ref byte c1, ref byte c2)
    {
        var tmp = c1; c1 = c2; c2 = tmp;
    }
    internal static Vector2 Abs(RectTransform rt)
    {
        var pr = rt.parent as RectTransform;
        var anchorSize = Vector2.Scale(pr.rect.size, (rt.anchorMax - rt.anchorMin));
        var size = anchorSize - rt.sizeDelta;
        var pos = rt.anchoredPosition - anchorSize;
        return pos;
        //return MathTool.ReverseY(p);
    }
    public static Vector2 AbsRefPos2(RectTransform rt)
    {
        var rtParent = rt.parent as RectTransform;
        Vector2 posParent = MathTool.ReverseY(rtParent.anchoredPosition);
        Vector2 pos = posParent;
        Vector2 anchorPos;
        var amin = rt.anchorMin;
        amin.y = 1 - amin.y;
        var amax = rt.anchorMax;
        amax.y = 1 - amax.y;
        var omin = rt.offsetMin;
        var omax = rt.offsetMax;

        if (amin == amax) // 九宫格锚点模式
        {
            anchorPos = Vector2.Scale(amin, rtParent.rect.size);
            pos += anchorPos + new Vector2(omin.x, -omax.y);
        }
        else if (amin == new Vector2(0, 0) && amax == new Vector2(1, 0))
        {
            pos.y += rtParent.rect.height;
            pos += MathTool.ReverseY(rt.anchoredPosition);
        }
        else if (amin == new Vector2(0, 0) && amax == new Vector2(0, 1))
        {
            anchorPos = MathTool.ReverseY(rt.anchoredPosition);
            pos += anchorPos;
        }
        else
        {
            pos += new Vector2(omin.x, -omax.y);// rt.anchoredPosition;
        }

        //p.y = scaler.referenceResolution.y - p.y;
        return pos;
    }
    public static Vector2 AbsRefPos(RectTransform rt)
    {
        var rtParent = rt.parent as RectTransform;
        Vector2 posParent = MathTool.ReverseY(rtParent.anchoredPosition);
        Vector2 pos = posParent;
        Vector2 anchorPos;
        var amin = rt.anchorMin;
        var amax = rt.anchorMax;
        var omin = rt.offsetMin;
        var omax = rt.offsetMax;

        if (amin == amax) // 九宫格锚点模式
        {
            anchorPos = Vector2.Scale(amin, rtParent.rect.size);
            pos += anchorPos + new Vector2(omin.x, -omax.y);
        }
        else if (amin == new Vector2(0, 0) && amax == new Vector2(1, 0))
        {
            pos.y += rtParent.rect.height;
            pos += MathTool.ReverseY(rt.anchoredPosition);

            //amin.y = 1 - amin.y;
            //anchorPos = Vector2.Scale(amin, rtParent.rect.size);
            //pos.y += anchorPos.y - omax.y;
            //pos.x += omin.x;
        }
        else if (amin == new Vector2(0, 0) && amax == new Vector2(0, 1))
        {
            anchorPos = MathTool.ReverseY(rt.anchoredPosition);
            pos += anchorPos;
        }
        else
        {
            pos += new Vector2(omin.x, -omax.y);// rt.anchoredPosition;
        }

        //p.y = scaler.referenceResolution.y - p.y;
        return pos;
    }
    public static Vector2 AbsPos(this RectTransform rt)
    {
        var pos = rt.anchoredPosition;
        if (canvasRT == null) throw null;
        var parent = rt.parent as RectTransform;
        if (parent == null) throw null;
        while (parent != canvasRT)
        {
            pos += parent.anchoredPosition;
            parent = parent.parent as RectTransform;
        }
        pos.y *= -1;
        return pos;
    }
    public static Vector2[] Rect(this RectTransform rt)
    {
        var v = AbsRefPos(rt);
        return new Vector2[] { v, v + rt.rect.size };
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
    private static RectTransform _canvasRT;
    public static RectTransform canvasRT
    {
        get
        {
            if (_canvasRT == null) _canvasRT = canvas.transform as RectTransform;
            return _canvasRT;
        }
        set { _canvasRT = value; }
    }
    private static Canvas _canvas;
    public static Canvas canvas
    {
        get
        {
            if (_canvas == null) _canvas = Obj.FindObjectOfType<Canvas>();
            return _canvas;
        }
        set { _canvas = value; }
    }
    private static CanvasScaler _scaler;
    public static CanvasScaler scaler
    {
        get
        {
            if (_scaler == null) _scaler = Obj.FindObjectOfType<CanvasScaler>();
            return _scaler;
        }
        set { _scaler = value; }
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
    internal static float Epsilon = 0.000001f;

    public static void BeginHorizon()
    {
        if (horizons == null) horizons = new List<UIHorizon>();
        horizon = new UIHorizon(horizon == null ? -ySpace : horizon.bottom - ySpace);
        horizons.Add(horizon);
    }
    public static void EndHorizon()//不能无UIEndHorizon，否则分母为0
    {
        var xScale = (parent as RectTransform).rect.width / horizon.right;
        foreach (var rt in horizon.rts)
        {
            rt.anchoredPosition = rt.anchoredPosition.SetX(rt.anchoredPosition.x * xScale);
            rt.sizeDelta = rt.sizeDelta.SetX(rt.rect.width * xScale);
        }
    }
    public static void Toggle(string labelStr, BoolValueToggle isOn, UnityAction<bool> onToggle = null)
    {
        Toggle(labelStr, I.togglePrefab.GetComponent<RectTransform>().rect.width, isOn, onToggle);
    }
    public static void Toggle(string labelStr, float width, BoolValueToggle value, UnityAction<bool> onToggle = null)
    {
        var toggle = Obj.Instantiate(I.togglePrefab, parent).GetComponent<Toggle>();
        toggle.isOn = value.defaultValue;
        value.toggle = toggle;
        toggle.onValueChanged.AddListener(onToggle);
        var rt = toggle.GetComponent<RectTransform>();
        rt.sizeDelta = rt.sizeDelta.SetX(width);
        var label = toggle.GetComponentInChildren<Text>(true);
        label.text = labelStr;
        label.color = labelColor;
        horizon.Add(toggle);
    }
    public static void Button(string labelStr)
    {
        Button(labelStr, default(UnityAction));
    }
    public static void Button(string labelStr, UnityAction onClick)
    {
        Button(labelStr, (I.buttonPrefab.transform as RectTransform).rect.width, onClick);
    }
    public static void Button(string labelStr, UnityAction<ButtonWrapper> onClick)
    {
        Button(labelStr, (I.buttonPrefab.transform as RectTransform).rect.width, onClick);
    }
    public static void Button(string labelStr, float width, UnityAction onClick)
    {
        var button = Obj.Instantiate(I.buttonPrefab, parent).GetComponent<Button>();
        button.onClick.AddListener(onClick);
        var label = button.GetComponentInChildren<Text>(true);
        label.text = labelStr;
        label.color = labelColor;
        var rt = button.GetComponent<RectTransform>();
        rt.sizeDelta = rt.sizeDelta.SetX(width);
        horizon.Add(button);
    }
    public static void Button(string labelStr, float width, UnityAction<ButtonWrapper> onClick)
    {
        var button = Obj.Instantiate(I.buttonPrefab, parent).GetComponent<Button>();
        var wrapper = new ButtonWrapper(button);
        button.onClick.AddListener(wrapper.OnClick);
        wrapper.onClick = onClick;
        var label = button.GetComponentInChildren<Text>(true);
        label.text = labelStr;
        label.color = labelColor;
        var rt = button.GetComponent<RectTransform>();
        rt.sizeDelta = rt.sizeDelta.SetX(width);
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
    public static void DropdownInt(int defaultValue, int count, string[] names, UnityAction<int> onValueChanged)
    {
        var drop = Obj.Instantiate(I.dropdownPrefab, parent).GetComponent<Dropdown>();
        drop.gameObject.AddComponent<DropDownLocateSelectedItem>();
        drop.onValueChanged.AddListener(onValueChanged);
        for (int i = 0; i < count; i++)
        {
            drop.options.Add(new Dropdown.OptionData(names[i]));
        }
        var v = Mathf.Clamp(defaultValue, 0, drop.options.Count - 1);
        drop.value = v;
        drop.itemText.color = dropdownColor;
        drop.captionText.text = drop.options[v].text;
        horizon.Add(drop);
    }
    public static void DropdownEnum(Enum enumValue, int count, string[] names, UnityAction<int> onValueChanged)
    {
        var drop = Obj.Instantiate(I.dropdownPrefab, parent).GetComponent<Dropdown>();
        drop.gameObject.AddComponent<DropDownLocateSelectedItem>();
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
        LabelField(labelStr, (I.labelPrefab.transform as RectTransform).rect.width);
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
    public static void LabelField(StringValueLabel value)
    {
        LabelField(value, (I.labelPrefab.transform as RectTransform).rect.width);
    }
    public static void LabelField(StringValueLabel value, float width)
    {
        var label = Obj.Instantiate(I.labelPrefab.gameObject, parent).GetComponent<Text>();
        var rt = label.rectTransform;
        rt.sizeDelta = rt.sizeDelta.SetX(width);
        label.text = value.value;

        value.label = label;

        var wrapper = new LabelWrapper(label, value);

        label.color = labelColor;
        horizon.Add(label);
    }
    public static float FloatFieldWithOutLabel(float width, FloatValueIF value, UnityAction<float, FloatFieldWrapper> onValueChanged = null)
    {
        var inputField = Obj.Instantiate(I.floatFieldPrefab, parent).GetComponent<InputField>();
        var rt = inputField.transform as RectTransform;
        rt.sizeDelta = rt.sizeDelta.SetX(width);
        inputField.contentType = InputField.ContentType.DecimalNumber;
        inputField.textComponent.color = floatFieldColor;
        inputField.text = value.ToString();
        var wrapper = new FloatFieldWrapper(inputField, value);
        wrapper.onValueChanged = onValueChanged;
        inputField.onValueChanged.AddListener(wrapper.OnValueChanged);
        value.field = inputField;
        horizon.Add(inputField);
        return 0f;
    }
    public static float FloatField(string labelStr, FloatValueIF value, UnityAction<float, FloatFieldWrapper> onValueChanged = null)
    {
        return FloatField(labelStr, value, float.MaxValue, onValueChanged);
    }
    public static float FloatField(string labelStr, FloatValueIF value, float maxRange, UnityAction<float, FloatFieldWrapper> onValueChanged = null)
    {
        var label = Obj.Instantiate(I.labelPrefab, parent).GetComponent<Text>();
        label.text = labelStr;
        label.color = floatLabelColor;
        var inputField = Obj.Instantiate(I.floatFieldPrefab, parent).GetComponent<InputField>();
        inputField.contentType = InputField.ContentType.DecimalNumber;
        inputField.textComponent.color = floatFieldColor;
        inputField.text = value.ToString();
        var wrapper = new FloatFieldWrapper(inputField, value, maxRange);
        wrapper.onValueChanged = onValueChanged;
        inputField.onValueChanged.AddListener(wrapper.OnValueChanged);
        value.field = inputField;
        horizon.Add(label);
        horizon.Add(inputField);
        return 0f;
    }
    public static float StringField(string labelStr, StringValueIF value, UnityAction<string, StringFieldWrapper> onValueChanged = null)
    {
        var label = Obj.Instantiate(I.labelPrefab, parent).GetComponent<Text>();
        label.text = labelStr;
        label.color = floatLabelColor;
        var inputField = Obj.Instantiate(I.stringFieldPrefab, parent).GetComponent<InputField>();
        inputField.textComponent.color = floatFieldColor;
        inputField.text = value.ToString();
        var wrapper = new StringFieldWrapper(inputField, value);
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
        ip *= facterToReference;
        ip.y = scaler.referenceResolution.y - ip.y;
        foreach (var rt in rts)
        {
            var rect = GetAbsRect(rt);
            if (rect.Contains(ip)) return true;
        }
        return false;
    }
    public static Rect GetAbsRect(RectTransform rt)
    {
        var rect = rt.rect;
        rect.position = AbsRefPos2(rt);
        //rect.position = MathTool.ReverseY(rt.anchoredPosition);
        //if (rt.name != "Area")
        //{
        //    //rect.position += Vector2.Scale((rt.parent as RectTransform).anchoredPosition, Vector2.one.SetY(-1));
        //    //rect.position += new Vector2(-rt.pivot.x * rt.rect.width, -(1 - rt.pivot.y) * rt.rect.height);
        //    rect.position = AbsRefPos(rt);
        //}
        return rect;
    }
}