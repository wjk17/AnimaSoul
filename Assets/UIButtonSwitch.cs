using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(UIButtonSwitch))]
public class UIButtonSwitchEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var o = (UIButtonSwitch)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Switch"))
        {
            o.Switch();
        }
    }
}
#endif
public class UIButtonSwitch : MonoBehaviour
{
    public Button[] buttons;
    public int index = 0;
    public void Switch()
    {
        if (buttons.Length <= 0) return;
        index++;
        if (index > buttons.Length - 1) index = 0;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == index)
            {
                buttons[i].gameObject.SetActive(true);
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
    }
    private void Reset()
    {
        buttons = GetComponentsInChildren<Button>(true);

    }
    public void Reg(params UnityAction[] callbacks)
    {
        if (buttons.Length < callbacks.Length)
        {
            Debug.Log("事件数超过按钮数");
            return;
        }
        // 注册点击事件        
        for (int i = 0; i < callbacks.Length; i++)
        {
            buttons[i].onClick.AddListener(callbacks[i]);
            buttons[i].onClick.AddListener(Switch);
        }
    }
    private void Awake()
    {
        if (buttons == null) Reset();
        index = -1;
        Switch();
    }
}
