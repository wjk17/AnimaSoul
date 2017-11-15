using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using OBJ = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
public class DummyGUIStyle : ScriptableObject
{
    public GUIStyle style;
}
public class DummyEditor : EditorWindow
{
    [MenuItem("Assets/Save Editor Skin")]
    static public void SaveEditorSkin()
    {
        GUISkin skin = Instantiate(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector)) as GUISkin;
        AssetDatabase.CreateAsset(skin, "Assets/EditorSkin.guiskin");
    }
    [MenuItem("Assets/Save Editor Skin - Label")]
    static public void SaveEditorSkinLabel()
    {
        GUISkin skin = Instantiate(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector)) as GUISkin;
        DummyGUIStyle gs = new DummyGUIStyle();
        gs.style = skin.label;
        AssetDatabase.CreateAsset(gs, "Assets/EditorSkin.guiskin_label");
    }
}
#endif
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
