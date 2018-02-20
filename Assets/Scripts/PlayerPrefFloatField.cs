using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefFloatField : MonoBehaviour
{
    public string prefName;
    public InputField inputfield;    
    public void GetValue()
    {
        float v;
        var sc = float.TryParse(inputfield.text, out v);
        var f = PlayerPrefs.GetFloat(prefName, v);
        inputfield.text = f.ToString();
    }
    private void Reset()
    {
        prefName = gameObject.name;
        inputfield = GetComponent<InputField>();
    }
    private void Start()
    {
        GetValue();
        inputfield.onValueChanged.AddListener(OnValueChange);
    }
    void OnValueChange(string s)
    {
        float f;
        var sc = float.TryParse(s, out f);
        if (sc)
        {
            PlayerPrefs.SetFloat(prefName, f);
            PlayerPrefs.Save();
        }
    }
}