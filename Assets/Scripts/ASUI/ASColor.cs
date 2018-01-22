using UnityEngine;

public class ASColor
{
    public float h;
    public float s;
    public float v;
    public ASColor(float h, float s, float v)
    {
        this.h = h;
        this.s = s;
        this.v = v;
    }
    public static ASColor R
    {
        get { return (ASColor)Color.red; }
    }
    public static ASColor G
    {
        get { return (ASColor)Color.green; }
    }
    public static ASColor B
    {
        get { return (ASColor)Color.blue; }
    }
    public static ASColor H
    {
        get { return new ASColor(1, 0, 0); }
    }
    public static ASColor S
    {
        get { return new ASColor(0, 1, 0); }
    }
    public static ASColor V
    {
        get { return new ASColor(0, 0, 1); }
    }
    public Color ToColor()
    {
        return Color.HSVToRGB(h, s, v);
    }
    public static explicit operator Color(ASColor color)
    {
        return color.ToColor();
    }
    public static explicit operator ASColor(Color color)
    {
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        return new ASColor(h, s, v);
    }
    public static ASColor operator *(float f, ASColor asc)
    {
        return asc * f;
    }
    public static ASColor operator *(ASColor asc, float f)
    {
        asc.h *= f;
        asc.s *= f;
        asc.v *= f;
        return asc;
    }
    public static Color operator +(Color color, ASColor asc)
    {
        return asc + color;
    }
    public static Color operator +(ASColor asc, Color color)
    {
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        h += asc.h;
        s += asc.s;
        v += asc.v;
        return Color.HSVToRGB(h, s, v);
    }
    public static Color operator -(Color color, ASColor asc)
    {
        return asc - color;
    }
    public static Color operator -(ASColor asc, Color color)
    {
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        h -= asc.h;
        s -= asc.s;
        v -= asc.v;
        return Color.HSVToRGB(h, s, v);
    }
}