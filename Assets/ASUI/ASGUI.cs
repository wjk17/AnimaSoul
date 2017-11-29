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

    public Dictionary<object, GLUIHandler> glHandlers = new Dictionary<object, GLUIHandler>();
    public Dictionary<object, IMUIHandler> imHandlers = new Dictionary<object, IMUIHandler>();

    private object _owner;
    public object owner
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
}