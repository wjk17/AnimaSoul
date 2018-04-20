using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(PlayerPrefToggle))]
public class PlayerPrefToggleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var o = (PlayerPrefToggle)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("CheckDuplicate"))
        {
            o.CheckDuplicate();
        }
        if (GUILayout.Button("PrintValue"))
        {
            o.CheckValue();
        }
    }
}
#endif
public class PlayerPrefToggle : MonoBehaviour
{
    public string prefName;
    public Toggle toggle;
    public void GetValue()
    {
        var v = PlayerPrefs.GetInt(prefName, toggle.isOn ? 1 : 0);
        toggle.isOn = v == 1 ? true : false;
    }
    private void Reset()
    {
        prefName = gameObject.name;
        toggle = GetComponent<Toggle>();
    }
    public void CheckValue()
    {
        var v = PlayerPrefs.GetInt(prefName, toggle.isOn ? 1 : 0);
        Debug.Log("value: " + v.ToString());
    }
    public void CheckDuplicate()
    {
        var ppts = FindObjectsOfType<PlayerPrefToggle>();
        bool duplicate = false;
        foreach (var ppt in ppts)
        {
            if (ppt != this && ppt.prefName == prefName)
            { duplicate = true; Debug.Log(ppt.gameObject.name); }
        }
        if (!duplicate) Debug.Log("No Duplicate");
    }
    private void Start()
    {
        GetValue();
        toggle.Init(OnValueChange);
    }
    void OnValueChange(bool value)
    {
        PlayerPrefs.SetInt(prefName, value ? 1 : 0);
        PlayerPrefs.Save();
    }
}
