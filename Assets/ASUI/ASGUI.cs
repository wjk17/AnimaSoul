using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ASGUI : MonoBehaviour
{
    public GameObject inputFieldPrefab;
    public GameObject labelPrefab;
    public GameObject dropdownPrefab;
    public GameObject sliderPrefab;
    public GameObject buttonPrefab;
    public GameObject togglePrefab;
    public new Camera camera;

    public List<IMUICommand> imCommands;
    public List<GLUICommand> glCommands;
    private void OnGUI()
    {
        foreach (var command in imCommands)
        {
            IMUIHandler.ExecuteCommand(command);
        }
    }
    private void OnRenderObject()
    {
        foreach (var command in glCommands)
        {
            GLUIHandler.ExecuteCommand(command);
        }
    }
}