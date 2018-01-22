using UnityEngine.Events;
using UnityEngine.UI;

public class LabelWrapper
{
    public Text label;
    public StringValue value;
    public LabelWrapper(Text f, StringValueLabel v)
    {
        label = f;
        value = v;
    }
}
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
public class StringFieldWrapper
{
    public UnityAction<string, StringFieldWrapper> onValueChanged;
    public InputField field;
    public StringValue value;
    public StringFieldWrapper(InputField f, StringValue v)
    {
        field = f;
        value = v;
    }
    public void OnValueChanged(string s)
    {
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
