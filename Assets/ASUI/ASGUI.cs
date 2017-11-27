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

    public List<IMUICommand> imCommands = new List<IMUICommand>();
    public List<GLUICommand> glCommands = new List<GLUICommand>();
    private void OnGUI()
    {
        foreach (var command in imCommands)
        {
            IMUIHandler.ExecuteCommand(command);
        }
    }
    private void OnRenderObject()
    {
        GLUI.SetLineMaterial();
        foreach (var command in glCommands)
        {
            GLUIHandler.ExecuteCommand(command);
        }
    }
}