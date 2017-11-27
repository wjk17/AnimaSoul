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
public enum GLUICommandType
{
    DrawLineOrtho,
}
public class GLUICommand
{
    public GLUICommandType type;
    public Argument[] args;
}
public static class GLUIHandler
{
    public static void ExecuteCommand(GLUICommand cmd)
    {
        switch (cmd.type)
        {
            case GLUICommandType.DrawLineOrtho:
                GLUI.DrawLineOrtho((cmd.args[0] as Vector2Arg).value, (cmd.args[0] as Vector2Arg).value);
                break;
            default:
                throw null;
        }
    }
}
public enum IMUICommandType
{
    DrawText,
}
public class IMUICommand
{
    public IMUICommandType type;
    public Argument[] arguments;
}
public static class IMUIHandler
{
    public static void ExecuteCommand(IMUICommand command)
    {

    }
}
