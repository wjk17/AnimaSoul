using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIClipList : MonoSingleton<UIClipList>
{
    //public Button button;
    public Button buttonRefresh;
    public int index;
    public List<string> names;
    //public string folder = "Clips/";
    public string _clipPath = "/../Clips/";
    public string clipPath { get { return Application.dataPath + _clipPath; } }
    public GameObject itemPrefab;
    public string currentClipName;
    public List<GameObject> buttons;
    public float ySpace = 10;

    void Start()
    {
        //button.Init(ItemClick);
        buttonRefresh.Init(GetClipNamesInPath, true);
    }
    private void ItemClick()
    {
        Debug.Log(currentClipName);
        UIDOFEditor.I.f.inputFileName.text = currentClipName;// names[index];
        UIDOFEditor.I.f.buttonLoadClip.onClick.Invoke();
    }
    public void GetClipNamesInPath()
    {
        names = new List<string>();
        if (buttons != null)
        {
            foreach (var button in buttons)
            {
                Destroy(button);
            }
        }
        buttons = new List<GameObject>();
        DirectoryInfo dir = new DirectoryInfo(clipPath);
        FileInfo[] fis = dir.GetFiles("*.clip", SearchOption.TopDirectoryOnly);
        int i = 0;
        //DirectoryInfo[] dirs = dir.GetDirectories();
        foreach (var fi in fis)
        {
            var clipName = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
            names.Add(clipName);

            var btn = Instantiate(itemPrefab, transform);
            btn.transform.SetLocalPosY(i * ((itemPrefab.transform as RectTransform).sizeDelta.y + ySpace));
            buttons.Add(btn);
            btn.GetComponentInChildren<Text>().text = clipName;
            btn.GetComponent<Button>().onClick.AddListener(delegate { currentClipName = clipName; ItemClick(); });
            i++;
        }
    }
}
