using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private void Start()
    {
        GetValue();
        toggle.onValueChanged.AddListener(OnValueChange);
    }
    void OnValueChange(bool value)
    {
        PlayerPrefs.SetInt(prefName, value ? 1 : 0);
        PlayerPrefs.Save();
    }
}
