using System;
using System.Collections.Generic;
using UnityEngine;

public class ASGUI : MonoBehaviour
{
    public GameObject inputFieldPrefab;
    public GameObject labelPrefab;
    public GameObject dropdownPrefab;
    public GameObject sliderPrefab;
    public GameObject buttonPrefab;
    public GameObject togglePrefab;
    public new Camera camera;

    public Dictionary<RectTransform, GLUIHandler> glHandlers = new Dictionary<RectTransform, GLUIHandler>();
    public Dictionary<RectTransform, IMUIHandler> imHandlers = new Dictionary<RectTransform, IMUIHandler>();

    private RectTransform _owner;
    public RectTransform owner
    {
        set
        {
            _owner = value;
            if (!glHandlers.ContainsKey(_owner))
            {
                glHandlers.Add(_owner, new GLUIHandler());
            }
            if (!imHandlers.ContainsKey(_owner))
            {
                imHandlers.Add(_owner, new IMUIHandler());
            }
        }
        get { return _owner; }
    }
    private void OnGUI()
    {
        foreach (var hdl in imHandlers)
        {
            hdl.Value.Execute();
        }
    }
    public void ClearCmd()
    {
        glHandlers[owner].commands.Clear();
        imHandlers[owner].commands.Clear();
    }
    public void AddCommand(Command command)
    {
        if (command.GetType() == typeof(GLUICommand))
        {
            glHandlers[owner].commands.Add(command);
        }
        else if (command.GetType() == typeof(IMUICommand))
        {
            imHandlers[owner].commands.Add(command);
        }
        else throw null;
    }
    private void CameraPostRender()
    {
        GLUI.SetLineMaterial();
        foreach (var hdl in glHandlers)
        {
            hdl.Value.Execute();
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