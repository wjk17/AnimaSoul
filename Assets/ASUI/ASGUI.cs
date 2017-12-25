using System;
using System.Collections.Generic;
using UnityEngine;
public static class CmdHdlTool
{
    public static bool Contains<T>(this List<T> list, RectTransform item) where T : CommandHandler
    {
        foreach (T i in list)
        {
            if (i.owner == item) return true;
        }
        return false;
    }
    public static T Ele<T>(this List<T> list, RectTransform item) where T : CommandHandler
    {
        foreach (T i in list)
        {
            if (i.owner == item) return i;
        }
        return null;
    }
    //public static bool Contains(this List<CommandHandler> list, RectTransform item)
    //{
    //    foreach (CommandHandler i in list)
    //    {
    //        if (i.owner == item) return true;
    //    }
    //    return false;
    //}
}
public class ASGUI : MonoBehaviour
{
    public GameObject inputFieldPrefab;
    public GameObject labelPrefab;
    public GameObject dropdownPrefab;
    public GameObject sliderPrefab;
    public GameObject buttonPrefab;
    public GameObject togglePrefab;
    public new Camera camera;

    //public Dictionary<RectTransform, GLUIHandler> glHandlers = new Dictionary<RectTransform, GLUIHandler>();
    //public Dictionary<RectTransform, IMUIHandler> imHandlers = new Dictionary<RectTransform, IMUIHandler>();
    //[NonSerialized]
    public List<GLUIHandler> glHandlers = new List<GLUIHandler>();
    //[NonSerialized]
    public List<IMUIHandler> imHandlers = new List<IMUIHandler>();


    private RectTransform _owner;
    public RectTransform owner
    {
        set
        {
            _owner = value;
            if (!glHandlers.Contains(_owner))
            {
                glHandlers.Add(new GLUIHandler(_owner));
            }
            if (!imHandlers.Contains(_owner))
            {
                imHandlers.Add(new IMUIHandler(_owner));
            }
        }
        get { return _owner; }
    }
    private void OnGUI()
    {
        foreach (var hdl in imHandlers)
        {
            hdl.Execute();
        }
    }    
    public void ClearCmd()
    {
        glHandlers.Ele(owner).commands.Clear();
        imHandlers.Ele(owner).commands.Clear();
    }
    public void AddCommand(Command command)
    {
        if (command.GetType() == typeof(GLUICommand))
        {
            glHandlers.Ele(owner).commands.Add(command);
        }
        else if (command.GetType() == typeof(IMUICommand))
        {
            imHandlers.Ele(owner).commands.Add(command);
        }
        else throw null;
    }
    private void CameraPostRender()
    {
        GLUI.SetLineMaterial();
        foreach (var hdl in glHandlers)
        {
            hdl.Execute();
        }
    }
    private void Start()
    {
        var wrapper = camera.GetComOrAdd<CameraEventWrapper>();
        wrapper.onPostRender = CameraPostRender;
    }
    public List<InputCallBack> inputCallBacks = new List<InputCallBack>();
    public class InputCallBack
    {
        public InputCallBack() { }
        /// <summary>
        /// 降序
        /// </summary>
        public InputCallBack(Action gi, int order = 0) { getInput = gi; this.order = order; }
        public Action getInput;
        public int order;
    }
    public virtual int SortList(InputCallBack a, InputCallBack b)
    {
        if (a.order > b.order) { return -1; }//降序
        else if (a.order < b.order) { return 1; }
        return 0;
    }
    public void Update()
    {
        Events.used = false;
        inputCallBacks.Sort(SortList);
        foreach (var call in inputCallBacks)
        {
            call.getInput();
        }
    }
}