using UnityEngine.UI;
using System;

[Serializable]
public class BoolValueToggle: BoolValue
{
    public Toggle toggle;
    public BoolValueToggle(bool f) : base(f) { }
    public override void Update()
    {
        if (toggle != null)
        {
            toggle.isOn = value;
        }
    }
}
[Serializable]
public abstract class BoolValue
{
    public bool value;
    public abstract void Update();
    public BoolValue(bool b)
    {
        value = b;
    }
    public override string ToString()
    {
        return value.ToString();
    }
    public static implicit operator bool(BoolValue v)
    {
        return v.value;
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
