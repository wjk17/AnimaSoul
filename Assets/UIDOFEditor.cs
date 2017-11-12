using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public static class ASGUI
{
    public static InputField inputFieldPrefab;
    public static void LabelField(Rect position, GUIContent label)
    {
        var inputField = Object.Instantiate(inputFieldPrefab.gameObject, inputFieldPrefab.transform.parent);
        inputField.SetActive(true);
    }
    public static float FloatField(Rect position, GUIContent label, float value)
    {
        return 0f;
    }
}
public class UIDOFEditor : MonoBehaviour
{
    public InputField inputFieldPrefab;
    void Start()
    {
        ASGUI.inputFieldPrefab = inputFieldPrefab;
        inputFieldPrefab.gameObject.SetActive(false);
        ASGUI.LabelField(new Rect(), new GUIContent("自转"));
    }
    void Update()
    {

    }
}
