using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GLUICmdType
{
    LoadOrtho,
    DrawLineOrtho,
}
[Serializable]
public class GLUICommand : Command
{
    public GLUICmdType type;
}
[Serializable]
public abstract class Command
{
    public int order;
    public object[] args;
}
public abstract class CommandHandler
{
    public CommandHandler() { }
    public List<Command> commands = new List<Command>();
    public abstract void ExecuteCommand(Command cmd);
    public virtual int SortList(Command a, Command b)
    {
        if (a.order > b.order) { return 1; }
        else if (a.order < b.order) { return -1; }
        return 0;
    }
    public void Execute()
    {
        commands.Sort(SortList);
        foreach (var command in commands)
        {
            ExecuteCommand(command);
        }
    }
    public bool ArgType(Command cmd)
    {
        return cmd.args.Length == 0;
    }
    public bool ArgType<T1>(Command cmd)
    {
        if (cmd.args.Length != 1) return false;
        if (!(cmd.args[0].GetType() == typeof(T1))) return false;
        return true;
    }
    public bool ArgType<T1, T2>(Command cmd)
    {
        if (cmd.args.Length != 2) return false;
        if (!(cmd.args[0].GetType() == typeof(T1))) return false;
        if (!(cmd.args[1].GetType() == typeof(T2))) return false;
        return true;
    }
    public bool ArgType<T1, T2, T3>(Command cmd)
    {
        if (cmd.args.Length != 3) return false;
        if (!(cmd.args[0].GetType() == typeof(T1))) return false;
        if (!(cmd.args[1].GetType() == typeof(T2))) return false;
        if (!(cmd.args[2].GetType() == typeof(T3))) return false;
        return true;
    }
    public bool ArgType<T1, T2, T3, T4>(Command cmd)
    {
        if (cmd.args.Length != 4) return false;
        if (!(cmd.args[0].GetType() == typeof(T1))) return false;
        if (!(cmd.args[1].GetType() == typeof(T2))) return false;
        if (!(cmd.args[2].GetType() == typeof(T3))) return false;
        if (!(cmd.args[3].GetType() == typeof(T4))) return false;
        return true;
    }
    public bool ArgType<T1, T2, T3, T4, T5>(Command cmd)
    {
        if (cmd.args.Length != 5) return false;
        if (!(cmd.args[0].GetType() == typeof(T1))) return false;
        if (!(cmd.args[1].GetType() == typeof(T2))) return false;
        if (!(cmd.args[2].GetType() == typeof(T3))) return false;
        if (!(cmd.args[3].GetType() == typeof(T4))) return false;
        if (!(cmd.args[4].GetType() == typeof(T5))) return false;
        return true;
    }
}
public class GLUIHandler : CommandHandler
{
    public override void ExecuteCommand(Command command)
    {
        var cmd = command as GLUICommand;
        switch (cmd.type)
        {
            case GLUICmdType.LoadOrtho: GL.LoadOrtho(); break;
            case GLUICmdType.DrawLineOrtho:
                if (ArgType<Vector2, Vector2>(cmd))
                {
                    GLUI.DrawLineOrtho((Vector2)cmd.args[0], (Vector2)cmd.args[1]);
                }
                else if (ArgType<Vector2, Vector2, Color>(cmd))
                {
                    GLUI.DrawLineOrtho((Vector2)cmd.args[0], (Vector2)cmd.args[1], (Color)cmd.args[2]);
                }
                else if (ArgType<Vector2, Vector2, float>(cmd))
                {
                    GLUI.DrawLineWidth((Vector2)cmd.args[0], (Vector2)cmd.args[1], (float)cmd.args[2]);
                }
                else if (ArgType<Vector2, Vector2, float, Color>(cmd))
                {
                    GLUI.DrawLineWidth((Vector2)cmd.args[0], (Vector2)cmd.args[1], (float)cmd.args[2], (Color)cmd.args[3]);
                }
                else { throw null; }
                break;
            default: throw null;
        }
    }
}
public enum IMUICmdType
{
    DrawText,
}
[Serializable]
public class IMUICommand : Command
{
    public IMUICmdType type;
}
public class IMUIHandler : CommandHandler
{
    public override void ExecuteCommand(Command command)
    {
        var cmd = command as IMUICommand;
        switch (cmd.type)
        {
            case IMUICmdType.DrawText:
                if (ArgType<string, Vector2>(cmd))
                    IMUI.DrawTextIM((string)cmd.args[0], (Vector2)cmd.args[1]);
                else if (ArgType<string, Vector2, Vector2>(cmd))
                    IMUI.DrawTextIM((string)cmd.args[0], (Vector2)cmd.args[1], (Vector2)cmd.args[2]);
                break;
            default:
                throw null;
        }
    }
}
