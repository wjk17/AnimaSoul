using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Esa;
public class GlueCodeGen : MonoBehaviour
{
    public Object template;
    public Object result;
    public string path1 { get { return Application.dataPath + "/GlueCodes/1.txt"; } }
    public string path { get { return Application.dataPath + "/GlueCodes/2.txt"; } }
    public string replacement_name = "";
    [Button]
    void Gen()
    {
        var text = File.ReadAllText(path1);
        text = text.Replace("#name", replacement_name);
        File.WriteAllText(path, text);
        AssetDatabase.Refresh();
        result = AssetDatabase.LoadMainAssetAtPath("Assets/GlueCodes/2.txt");
    }
    // Update is called once per frame
    void Update()
    {

    }
}
