using UnityEngine.UI;
using System;

[Serializable]
public class BoolValueToggle
{
    public bool Value
    {
        get { return toggle.isOn; }
        set { toggle.isOn = value; }
    }
    public bool defaultValue;
    public Toggle toggle;
    public BoolValueToggle(Toggle toggle, bool b)
    {
        this.toggle = toggle;
        toggle.isOn = b;
    }
    public BoolValueToggle(bool b)
    {
        defaultValue = b;
    }    
    public override string ToString()
    {
        return Value.ToString();
    }
    public static implicit operator bool(BoolValueToggle v)
    {
        return v.Value;
    }
}
[Serializable]
public class StringValueLabel : StringValue
{
    public Text label;
    public StringValueLabel(string f) : base(f) { }
    public override void Update()
    {
        if (label != null)
        {
            label.text = value;
        }
    }
}
[Serializable]
public class StringValueIF : StringValue
{
    public InputField field;
    public StringValueIF(string f) : base(f) { }
    public override void Update()
    {
        if (field != null)
        {
            field.text = value;
        }
    }
}
[Serializable]
public abstract class StringValue
{
    public string value;
    public abstract void Update();
    public StringValue(string f)
    {
        value = f;
    }
    public override string ToString()
    {
        return value.ToString();
    }
    public static implicit operator string(StringValue f)
    {
        return f.value;
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
