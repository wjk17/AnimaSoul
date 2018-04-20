using System;
using UnityEngine;

public static class Events
{
    public static bool used;
    public static void Use()
    {
        used = true;
    }
    public static bool Command
    {
        get
        {
            var command = Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
            return !used && command;
        }
    }
    public static bool Ctrl
    {
        get
        {
            var ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            return !used && ctrl;
        }
    }
    public static bool Shift
    {
        get
        {
            var shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            return !used && shift;
        }
    }
    public static bool Alt
    {
        get
        {
            var alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            return !used && alt;
        }
    }
    public static bool KeyDown(KeyCode code)
    {
        return !used && Input.GetKeyDown(code);
    }
    public static bool KeyUp(KeyCode code)
    {
        return !used && Input.GetKeyUp(code);
    }
    public static bool Key(KeyCode code)
    {
        return !used && Input.GetKey(code);
    }
    public static bool MouseDown(MouseButton button)
    {
        return !used && Input.GetMouseButtonDown((int)button);
    }
    public static bool MouseUp(MouseButton button)
    {
        return !used && Input.GetMouseButtonUp((int)button);
    }
    public static bool Mouse(MouseButton button)
    {
        return !used && Input.GetMouseButton((int)button);
    }
    public static bool Click
    {
        get
        {
            return MouseDown(MouseButton.Left) || MouseUp(MouseButton.Left) ||
                MouseDown(MouseButton.Right) || MouseUp(MouseButton.Right) ||
                MouseDown(MouseButton.Middle) || MouseUp(MouseButton.Middle);
        }
    }
    internal static float Axis(string v)
    {
        if (used) return 0;
        return Input.GetAxis(v);
    }
}