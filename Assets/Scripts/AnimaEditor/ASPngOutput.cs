using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(ASPngOutput))]
public class ASPngOutputEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var o = (ASPngOutput)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("SavePNG"))
        {
            o.SavePNG();
        }
    }
}
#endif
public class ASPngOutput : MonoBehaviour
{
    void Start()
    {

    }
    public void SavePNG()
    {
        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();

        if (Application.isPlaying)
            Destroy(tex);
        else DestroyImmediate(tex);

        // For testing purposes, also write to a file in the project folder
        File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
    }
}
