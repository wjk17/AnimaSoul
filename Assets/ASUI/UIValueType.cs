using UnityEngine.UI;
using System;

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
