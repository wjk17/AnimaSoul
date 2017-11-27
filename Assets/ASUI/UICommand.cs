using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringArg : Argument
{
    public string value;
}
public class Vector2Arg : Argument
{
    public Vector2 value;
}
public class FloatArg : Argument
{
    public float value;
}
public class Argument
{
}
public enum GLUICmdType
{
    LoadOrtho,
    DrawLineOrtho,
}
public class GLUICommand
{
    public GLUICmdType type;
    public object[] args;
}
public static class GLUIHandler
{
    public static void ExecuteCommand(GLUICommand cmd)
    {
        switch (cmd.type)
        {
            case GLUICmdType.LoadOrtho: GL.LoadOrtho(); break;
            case GLUICmdType.DrawLineOrtho:
                if (cmd.args.Length == 2)
                    GLUI.DrawLineOrtho((Vector2)cmd.args[0], (Vector2)cmd.args[1]);
                else
                    GLUI.DrawLineOrtho((Vector2)cmd.args[0], (Vector2)cmd.args[1], (Color)cmd.args[2]);
                break;
            default:
                throw null;
        }
    }
}
public enum IMUICmdType
{
    DrawText,
}
public class IMUICommand
{
    public IMUICmdType type;
    public object[] args;
}
public static class IMUIHandler
{
    public static void ExecuteCommand(IMUICommand cmd)
    {
        switch (cmd.type)
        {
            case IMUICmdType.DrawText:
                IMUI.DrawTextIM((string)cmd.args[0], (Vector2)cmd.args[1]);
                break;
            default:
                throw null;
        }
    }
}
